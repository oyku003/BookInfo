using System;
using BookInfo.Entities;
using BookInfo.Common.Helpers;
using BookInfo.Entities.EntityModel;
using BookInfo.BusinessLayer.Result;
using BookInfo.BusinessLayer.Abstract;
using BookInfo.DataAccessLayer.EntityFramework;
using static BookInfo.Entities.Messages.ErrorMessageCode;

namespace BookInfo.BusinessLayer
{
   
    public class UserManager : ManagerBase<BookUser>
    {
        private readonly Repository<BookUser> repo_user = new Repository<BookUser>();

        public BusinessLayerResult<BookUser> Login(LoginViewModel data)
        {
            BusinessLayerResult<BookUser> res = new BusinessLayerResult<BookUser>();

            res.Result = Find(x => x.Username == data.UserName && x.Password == data.Password);

            if (res.Result != null)
            {
                if (!res.Result.IsActive)
                {
                    res.AddError(UserIsNotActive, "Kullanıcı aktifleştirilmemiştir.");
                    res.AddError(CheckYourEmail, "Lütfen e-posta adresinizi kontrol ediniz.");
                }

            }
            else
            {
                res.AddError(UsernameOrPassWrong, "Kullanıcı adı ya da şifreniz uyuşmuyor.");
            }
            return res;
        }       

        public BusinessLayerResult<BookUser> Register(RegisterViewModel model)
        {
            BookUser user = Find(x => x.Username == model.UserName || x.Email == model.Email);
            BusinessLayerResult<BookUser> res = new BusinessLayerResult<BookUser>();
            if (user != null)
            {
                if (model.Email == user.Email)
                {
                    res.AddError(EmailAlreadyExists, "E-Posta zaten mevcut");
                }
                if (model.UserName == user.Username)
                {
                    res.AddError(UsernameAlreadyExists, "Kullanıcı adı zaten mevcut");
                }
            }
            else
            {
                BookUser meetinUser = new BookUser()
                {
                    Username = model.UserName,
                    Email = model.Email,
                    Password = model.Password,
                    ActivateGuid = Guid.NewGuid(),
                    IsActive = false,
                    IsAdmin = false,
                    ProfileImageFilename = "user_boy.jpg"

                };

                int dbResult = repo_user.Insert(meetinUser);

                if (dbResult > 0)
                {
                    res.Result = Find(x => x.Email == model.Email && x.Username == model.UserName);

                    string siteUri = ConfigHelper.Get<string>("SiteRootUri");
                    string activateUri = $"{siteUri}/Home/UserActivate/{res.Result.ActivateGuid}";
                    string body = $"Merhaba {res.Result.Username};<br><br> Hesabınızı aktifleştirmek için <a href='{activateUri}' target='_blank' > tıklayınız</a>.";
                    MailHelper.SendMail(body, res.Result.Email, "MyEvernote Hesap Aktifleştirme");
                }
            }

            return res;
        }

        public BusinessLayerResult<BookUser> ActivateUser(Guid id)
        {
            BusinessLayerResult<BookUser> res = new BusinessLayerResult<BookUser>();
            res.Result = Find(x => x.ActivateGuid == id);

            if (res.Result != null)
            {
                if (res.Result.IsActive)
                {
                    res.AddError(UserAlreadyActive, "Kullanıcı zaten aktif edilmiştir.");

                    return res;
                }

                res.Result.IsActive = true;
                repo_user.Update(res.Result);

            }
            else
            {
                res.AddError(ActivateIdDoesNotExist, "Aktifleştirilecek kullanıcı bulunamadı.");
            }
            return res;
        }

        public BusinessLayerResult<BookUser> GetUserById(int id)
        {
            BusinessLayerResult<BookUser> res = new BusinessLayerResult<BookUser>();
            res.Result = Find(x => x.Id == id);

            if (res.Result == null)
            {
                res.AddError(UserCouldNotFind, "Kullanıcı bulunamadı");
            }
            return res;
        }

        public BusinessLayerResult<BookUser> UpdateUser(BookUser model)
        {
            BookUser db_user = Find(x => x.Username == model.Username || x.Email == model.Email);
            BusinessLayerResult<BookUser> res = new BusinessLayerResult<BookUser>();

            if (db_user != null && db_user.Id != model.Id) //Update işlemini yapan kişi miyim onu sorguluyor.
            {
                if (db_user.Username == model.Username) //Başka birinin username'i ile değiştirmesin kendininkini.
                {
                    res.AddError(UsernameAlreadyExists, "Kullanıcı adı kayıtlı.");
                }
                if (db_user.Email == model.Email) ////Başka birinin email'i ile değiştirmesin kendininkini.
                {
                    res.AddError(EmailAlreadyExists, "E-posta adresi kayıtlı.");
                }

                return res;
            }

            res.Result = Find(x => x.Id == model.Id); //Hata yoksa update edilecek kullanıcıyı bir daha çekip garantiye alıyorum kendimi.
            res.Result.Email = model.Email;
            res.Result.Name = model.Name;
            res.Result.Surname = model.Surname;
            res.Result.Password = model.Password;
            res.Result.Username = model.Username;

            if (string.IsNullOrEmpty(model.ProfileImageFilename) == false) //Profil image dosyası gelmiş mi? Gümcelliyorsa
            {
                res.Result.ProfileImageFilename = model.ProfileImageFilename; //Dosya adını da güncelle
            }

            if (base.Update(res.Result) == 0)
            {
                res.AddError(ProfileCouldNotUpdate, "Profil güncellenemedi.");
            }

            return res;
        }

        public BusinessLayerResult<BookUser> RemoveUserById(int id)
        {
            BusinessLayerResult<BookUser> res = new BusinessLayerResult<BookUser>();
            BookUser user = Find(x => x.Id == id);

            if (user != null)
            {
                if (Delete(user) == 0)
                {
                    res.AddError(UserCouldNotRemove, "Kullanıcı silinemedi");
                    return res;
                }
            }
            else
            {
                res.AddError(UserCouldNotFind, "Kullanıcı bulunamadı.");
            }
            return res;
        }
    }


}
