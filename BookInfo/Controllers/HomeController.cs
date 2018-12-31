using System;
using PagedList;
using System.Web;
using System.Linq;
using System.Net;
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
using static BookInfo.Entities.EntityModel.SearchViewModel;
using BookInfo.Common.Helpers;
using BookInfo.ML;

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
        private LikedManager likedManager = new LikedManager();
        private BookSessionInfoManager bookSessionInfoManager = new BookSessionInfoManager();
        private SimilarBookManager similarBookManager = new SimilarBookManager();

        public ActionResult Index()
        {
            ViewBag.KitapSayısı = bookManager.ListQueryable().Where(x => x.Id.ToString() != null).OrderByDescending(x => x.ModifiedOn).ToList().Count();
          
            return View();
        }
        public PartialViewResult GetBook(int? page)
        {
            var pageIndex = page ?? 1;

            var bookList = bookManager.ListQueryable().Where(x => x.Id.ToString() != null).OrderByDescending(x => x.ModifiedOn).ToList();

            ViewBag.KitapSayısı = bookList.Count();

            var model = bookList.Skip(3 * pageIndex - 3).Take(3).ToList();

            return PartialView("_IndexBook", model);
        }
        public ActionResult GetAprioriBook(int id)
        {
            List<Book> model = new List<Book>();

            var aprioriBook = similarBookManager.List().Where(x => x.BookId == id);

            foreach (var item in aprioriBook)
            {
                var bookInfo = bookManager.List().Where(x => x.Id == item.SimilarBookId).FirstOrDefault();             

                model.Add(bookInfo);    
            }
            return PartialView("_PartialAprioriBook", model);
        }
        public ActionResult GetPopularBook()
        {
            List<Book> model = new List<Book>();

            var popularBook = bookManager.List().OrderByDescending(x => x.Clicked).Take(6);

            foreach (var item in popularBook)
            {
               // var bookInfo = bookManager.List().Where(x => x.Id == item.SimilarBookId).FirstOrDefault();

                model.Add(item);
            }
            return PartialView("_PartialPopularBook", model);
        }
   
        public ActionResult Kitap(int Id)
        {
            new AprioriProcess().CreateAprioriRules();
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            bookSessionInfoManager.SaveBookInfo(Id);

            BookViewModel model = null;
            try
            {
                Book book = bookManager.ListQueryable().Include("Comments").Where(x => x.Id == Id).FirstOrDefault();

                if (book != null)
                {
                    model = new BookViewModel()
                    {
                        Id = book.Id,
                        Name = book.Name,
                        Author = book.Author.Name,
                        Description = book.Description,
                        Page = book.Page,
                        Publisher = book.Publisher.Name,
                        Year = book.Year,
                        Comments = book.Comments
                    };

                    int page = commentManager.List().Where(x => x.Book.Id == Id).Count();
                    if (page % 3 == 0)
                    {
                        ViewBag.PageCount = page / 3;
                    }
                    else
                    {
                        ViewBag.PageCount = page/3 +1;
                    }

                    var bookSumPoint = likedManager.List().Where(x => x.Book.Id == Id).Sum(x => x.Point);

                    var bookLikeCount = bookManager.List().Where(x => x.Id == Id).Select(x => x.LikeCount).First();
                    if (bookLikeCount != 0)
                    {
                        model.Point = ((float)bookSumPoint / bookLikeCount).ToString("0.#");
                    }
                    else
                    {
                        model.Point = "0";
                    }
                    book.Clicked++;
                    bookManager.Update(book);


                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Aradığınız kitap bilgisine ulaşılırken bir hata oluştu.Anasayfaya yönlendiriliyorsunuz...",
                    RedirectingUrl = "/Home/Index"
                };
                return View("Error", errorNotifyObj);
            }
            return View(model);
        }
        //public ActionResult Search(int page = 1, string search = "")
        //{
        //    if (string.IsNullOrWhiteSpace(search))
        //    {
        //        try
        //        {
        //            IPagedList<SearchViewModel> lst = null;
        //            List<SearchViewModel> lstSearching = new List<SearchViewModel>();

        //            if (search != "")
        //            {
        //                var books = bookManager.List().Where(x => x.Name.StartsWith(search)).ToList();
        //                var authors = authorManager.List().Where(x => x.Name.StartsWith(search)).ToList();
        //                var publishers = publisherManager.List().Where(x => x.Name.StartsWith(search)).ToList();

        //                if (books.Any())
        //                {
        //                    foreach (var book in books)
        //                    {
        //                        SearchViewModel subSearchViewModel = new SearchViewModel()
        //                        {
        //                            Description = book.Description,
        //                            Id = book.Id,
        //                            Title = book.Name,
        //                            Type = SearchType.Book
        //                        };

        //                        lstSearching.Add(subSearchViewModel);
        //                    }
        //                }
        //                if (authors.Any())
        //                {
        //                    foreach (var author in authors)
        //                    {
        //                        var subSearchViewModel = new SearchViewModel
        //                        {
        //                            Id = author.Id,
        //                            Title = author.Name,
        //                            Type = SearchType.Author
        //                        };

        //                        lstSearching.Add(subSearchViewModel);
        //                    }
        //                }
        //                if (publishers.Any())
        //                {
        //                    foreach (var publisher in publishers)
        //                    {
        //                        var subSearchViewModel = new SearchViewModel
        //                        {
        //                            Id = publisher.Id,
        //                            Title = publisher.Name,
        //                            Type = SearchType.Publisher
        //                        };

        //                        lstSearching.Add(subSearchViewModel);
        //                    }
        //                }
        //            }
        //            lst = lstSearching.ToPagedList(page, 1);
        //            return View(lst);
        //        }
        //        catch (Exception ex)
        //        {

        //            throw;
        //        }
        //    }
        //    return null;
        //}
        public ActionResult Search(int page = 1, string search = "")
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(search))
                {
                    IPagedList<Book> lst = null;
                    List<Book> lstSearching = new List<Book>();

                    var books = bookManager.List().Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();
                    var authorsBook = bookManager.List().Where(x => x.Author.Name.ToLower().Contains(search));
                    var publishersBook = bookManager.List().Where(x => x.Publisher.Name.ToLower().Contains(search));

                    //TODO:Foreach yapısını linq'ya çek
                    foreach (var item in books)
                    {
                        lstSearching.Add(item);
                    }

                    foreach (var item in authorsBook)
                    {
                        lstSearching.Add(item);
                    }

                    foreach (var item in publishersBook)
                    {
                        lstSearching.Add(item);
                    }

                    ViewBag.ToplamKayitSayisi = lstSearching.GroupBy(x => x.Id).Select(x => x.First()).Count(); 

                    ViewBag.Search = search;

                    return View(lstSearching.GroupBy(x => x.Id).Select(x => x.First()).ToPagedList(page, 2));
                }

            }
            catch (Exception ex)
            {
                //TODO:loglama yap
                throw;
            }

            // return View(lstSearching.ToPagedList(page, 1));

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

                    ProfileImage.SaveAs(Server.MapPath($"~/images/{filename}"));
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
        public PartialViewResult GetComment(int id, int? page)
        {
            var pageIndex = page ?? 1;

            var commentList = commentManager.List().Where(x => x.Book.Id == id).OrderByDescending(x=>x.CreatedOn);

            ViewBag.YorumSayısı = commentList.Count();

            var model = commentList.Skip(3 * pageIndex - 3).Take(3).ToList();
            
            int pageCount = commentManager.List().Where(x => x.Book.Id == id).Count();
            if (pageCount % 3 == 0)
            {
                ViewBag.PageCount = pageCount / 3;
            }
            else
            {
                ViewBag.PageCount = pageCount / 3 + 1;
            }

            return PartialView("_PartialUserComments", model);

        }
    }
}