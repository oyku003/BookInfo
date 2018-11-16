using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using BookInfo.Models;
using BookInfo.Entities;
using System.Data.Entity;
using BookInfo.ViewModels;
using BookInfo.BusinessLayer;
using System.Collections.Generic;
using BookInfo.Entities.EntityModel;
using BookInfo.BusinessLayer.Result;
using static BookInfo.Entities.Messages.ErrorMessageCode;
using PagedList;
using static BookInfo.Entities.EntityModel.SearchViewModel;

namespace BookInfo.Controllers
{
    public class HomeController : Controller
    {
        private UserManager userManager = new UserManager();
        private BookManager bookManager = new BookManager();
        private CategoryManager categoryManager = new CategoryManager();
        private CommentManager commentManager = new CommentManager();
        private AuthorManager authorManager = new AuthorManager();
        private PublisherManager publisherManager = new PublisherManager();

        public ActionResult Index()
        {
            return View(bookManager.ListQueryable().Where(x => x.Id.ToString() != null).OrderByDescending(x => x.ModifiedOn).ToList());
        }
        //public ActionResult Kitap(int Id)
        //{
        //BookViewModel model;
        //try
        //{
        //    Book book = bookManager.ListQueryable().Include("Comments").Where(x => x.Id == Id).FirstOrDefault();

        //    if (book != null)
        //    {
        //         model = new BookViewModel()
        //        {
        //            Name = book.Name,
        //            Author = book.Author.Name,
        //            Description = book.Description,
        //            Page = book.Page,
        //            Publisher = book.Publisher.Name,
        //            Year = book.Year,
        //            Comments = book.Comments
        //        };
        //        return View(model);
        //    }
        //}
        //catch (Exception ex)
        //{

        //    throw;
        //}
        //return View(model);
        //}
        public ActionResult Search(int page = 1, string search = "")
        {
            try
            {
                IPagedList<SearchViewModel> lst = null;
                List<SearchViewModel> lstSearching = new List<SearchViewModel>();

                if (search != "")
                {
                    var books = bookManager.List().Where(x => x.Name.Contains(search)).ToList();
                    var authors = authorManager.List().Where(x => x.Name.Contains(search)).ToList();
                    var publishers = publisherManager.List().Where(x => x.Name.Contains(search)).ToList();

                    if (books.Any())
                    {
                        foreach (var book in books)
                        {
                            SearchViewModel subSearchViewModel = new SearchViewModel()
                            {
                                Description = book.Description,
                                Id = book.Id,
                                Title = book.Name,
                                Type = SearchType.Book
                            };

                            lstSearching.Add(subSearchViewModel);
                        }
                    }
                    if (authors.Any())
                    {
                        foreach (var author in authors)
                        {
                            var subSearchViewModel = new SearchViewModel
                            {
                                Id = author.Id,
                                Title = author.Name,
                                Type = SearchType.Author
                            };

                            lstSearching.Add(subSearchViewModel);
                        }
                    }
                    if (publishers.Any())
                    {
                        foreach (var publisher in publishers)
                        {
                            var subSearchViewModel = new SearchViewModel
                            {
                                Id = publisher.Id,
                                Title = publisher.Name,
                                Type = SearchType.Publisher
                            };

                            lstSearching.Add(subSearchViewModel);
                        }
                    }
                }
                lst = lstSearching.ToPagedList(page, 1);
                return View(lst);
            }
            catch (Exception ex)
            {

                throw;
            }
            return null;
        }
        public ActionResult MostLiked()
        {
            return View("Index", bookManager.ListQueryable().OrderByDescending(x => x.LikeCount).ToList());
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult ShowProfile()
        {
            BusinessLayerResult<BookUser> res = userManager.GetUserById(CurrentSession.User.Id);
            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Geçersiz İŞlem",
                    Items = res.Errors
                };
                return View("Error", errorNotifyObj);
            }
            return View(res.Result);
        }
        public ActionResult EditProfile()
        {
            BusinessLayerResult<BookUser> res = userManager.GetUserById(CurrentSession.User.Id);
            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Geçersiz İşlem",
                    Items = res.Errors
                };
                return View("Error", errorNotifyObj);
            }
            return View(res.Result);
        }
        [HttpPost]
        public ActionResult EditProfile(BookUser model, HttpPostedFileBase ProfileImage)
        {
            ModelState.Remove("ModifiedUsername");//sayfayı kontrol ederken modeldeki bütün zorunlu alanlara bakar.Bu alan ilgili sayfada olmadığı için kontrolden önce remove yapıyoruz.
            if (ModelState.IsValid)
            {
                if (ProfileImage != null &&
               (ProfileImage.ContentType == "image/jpeg" ||
                ProfileImage.ContentType == "image/jpg" ||
                ProfileImage.ContentType == "image/png"))
                {
                    string filename = $"user_{model.Id}.{ProfileImage.ContentType.Split('/')[1]}";

                    ProfileImage.SaveAs(Server.MapPath($"~/iamges/{filename}"));
                    model.ProfileImageFilename = filename;

                }

                BusinessLayerResult<BookUser> res = userManager.UpdateUser(model);
                if (res.Errors.Count > 0)
                {
                    ErrorViewModel errorNotifyObj = new ErrorViewModel()
                    {
                        Items = res.Errors,
                        Title = "Profil Güncellenemedi",
                        RedirectingUrl = "/Home/EditProfile"
                    };
                    return View("Error", errorNotifyObj);
                }
                CurrentSession.Set<BookUser>("login", res.Result);//Profil güncellendiği için session güncellendi. n
            }
            return RedirectToAction("ShowProfile");
        }
        public ActionResult Delete()
        {
            BusinessLayerResult<BookUser> res = userManager.RemoveUserById(CurrentSession.User.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Profil Silinemedi",
                    Items = res.Errors,
                    RedirectingUrl = "/Home/ShowProfile"
                };
                return View("Error", errorNotifyObj);
            }
            Session.Clear();
            return RedirectToAction("Index");
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)//model uygunsa
            {
                BusinessLayerResult<BookUser> res = userManager.Login(model);
                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    if (res.Errors.Find(x => x.Code == UserIsNotActive) != null)
                    {
                        ViewBag.SetLink = "E-Posta Gönder";//hata kodu bu şekilde düzenlenebilir!!
                    }
                    return View(model);
                }
                CurrentSession.Set<BookUser>("login", res.Result);//session'a kullanıcı bilgi saklama
                return RedirectToAction("Index");//yönlendirme...
            }
            return View(model);
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                BusinessLayerResult<BookUser> res = userManager.Register(model);
                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(model);
                }
                OkViewModel notifyObj = new OkViewModel()
                {
                    Title = "Kayıt Başarılı",
                    RedirectingUrl = "/Home/Login",

                };
                notifyObj.Items.Add("  Lütfen e-posta adresinize gönderdiğimiz aktivasyon linkine tıklayarak hesabınızı aktive ediniz.Hesabınızı aktive etmeden not eklyemez ve beğenme yapamazsınız.");
                return View("Ok", notifyObj);
            }
            return View(model);
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }
        public ActionResult UserActivate(Guid id)
        {
            BusinessLayerResult<BookUser> res = userManager.ActivateUser(id);
            if (res.Errors.Count > 0)
            {
                ErrorViewModel notifyObj = new ErrorViewModel()
                {
                    Title = "Geçersiz İşlem",
                    Items = res.Errors
                };
                return RedirectToAction("Error", notifyObj);
            }
            OkViewModel okNotifyObj = new OkViewModel()
            {
                Title = "Hesabınız aktifleştirildi.",
                RedirectingUrl = "/Home/Login"
            };
            okNotifyObj.Items.Add("Hesabınız aktifleştirildi.Artık yorum ve beğeni yapabilirsiniz.");

            return RedirectToAction("Ok", okNotifyObj);
        }
        public ActionResult AccessDenied()
        {
            return View();
        }
        public ActionResult HasError()
        {
            return View();
        }
        public ActionResult ByCategory(int Id)
        {
            Book book = bookManager.ListQueryable().Where(x => x.Id == Id).FirstOrDefault();
            if (book != null)
            {
                return View("Kitap", book);
            }
            ErrorViewModel errorNotifyObj = new ErrorViewModel()
            {
                Title = "Kitap bulunamadı.Anasayfaya yönlendiriliyorsunuz"
            };
            return View("Error", errorNotifyObj);
        }

    }
}