using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookInfo.BusinessLayer;
using BookInfo.Entities;
using BookInfo.Entities.EntityModel;
using BookInfo.Models;
using BookInfo.ViewModels;

namespace BookInfo.Controllers
{
    public class BookController : Controller
    {
        private BookManager bookManager = new BookManager();
        private CategoryManager categoryManager = new CategoryManager();
        private LikedManager likedManager = new LikedManager();
        private AuthorManager authorManager = new AuthorManager();
        private PublisherManager publisherManager = new PublisherManager();
        private CommentManager commentManager = new CommentManager();
        /// <summary>
        /// Kitapları listeler
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var books = bookManager.ListQueryable().OrderByDescending(x => x.ModifiedOn);

            return View(books.ToList());
        }

        public ActionResult MyLikedNotes()
        {
            var books = likedManager.ListQueryable().Include("LikedUser").Include("Note").Where(x => x.Id == CurrentSession.User.Id).Select(x => x.Book).Include("Category").Include("Owner").OrderByDescending(x => x.ModifiedOn);//whereden dönen nesneden notlar arasındakileri modifed on a göre tersten sıraladık

            return View("Index", books.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book note = bookManager.Find(x => x.Id == id.Value);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(CacheHelper.GetBooksFromCache(), "Id", "Title");
            ViewBag.Yazar = authorManager.List();
            ViewBag.Kategori = categoryManager.List();
            ViewBag.Yayınevi = publisherManager.List();

            return View(new CreateBookViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateBookViewModel createBookViewModel)
        {
            ModelState.Remove("ModifiedUsername");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("CreatedOn");
            if (ModelState.IsValid)
            {
                var author = authorManager.List(x => x.Id == createBookViewModel.Author).SingleOrDefault();
                var publisher = publisherManager.List(x => x.Id == createBookViewModel.Publisher).SingleOrDefault();
                var category = categoryManager.List(x => x.Id == createBookViewModel.Category).SingleOrDefault();

                Book book = new Book()
                {
                    ModifiedUsername = CurrentSession.User.ToString(),
                    Category = category,
                    Author = author,
                    Publisher = publisher,
                    Name = createBookViewModel.Name,
                    Description = createBookViewModel.Description,
                    Year = createBookViewModel.Year,
                    Page = createBookViewModel.Page
                };

                int res = bookManager.Insert(book);
                if (res > 0)
                {
                    OkViewModel ok = new OkViewModel()
                    {
                        Title = "Kitap ekleme işleminiz başarılıdır.",
                        RedirectingUrl = "/Book/Index"
                    };

                    return View("Ok", ok);
                }
            }

            ViewBag.Yazar = authorManager.List();
            ViewBag.Kategori = categoryManager.List();
            ViewBag.Yayınevi = publisherManager.List();

            return View(createBookViewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = bookManager.Find(x => x.Id == id.Value);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.Yazar = authorManager.List();
            ViewBag.Kategori = categoryManager.List();
            ViewBag.Yayınevi = publisherManager.List();
            CreateBookViewModel createBookViewModel = new CreateBookViewModel()
            {
                Id = book.Id,
                Category = book.Category.Id,
                Author = book.Author.Id,
                Publisher = book.Publisher.Id,
                Name = book.Name,
                Page = book.Page,
                Year = book.Year
                
            };
            return View(createBookViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreateBookViewModel book)
        {
            ModelState.Remove("ModifiedUsername");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("CreatedOn");
            if (ModelState.IsValid)
            {
                Book db_book = bookManager.Find(x => x.Id == book.Id);
                var author = authorManager.List(x => x.Id == book.Author).SingleOrDefault();
                var publisher = publisherManager.List(x => x.Id == book.Publisher).SingleOrDefault();
                var category = categoryManager.List(x => x.Id == book.Category).SingleOrDefault();

                if (db_book != null)
                {
                    db_book.Name = book.Name;
                    db_book.Description = book.Description;
                    db_book.Category = category;
                    db_book.Author = author;
                    db_book.Publisher = publisher;
                    db_book.Page = book.Page;
                    db_book.Year = book.Year;
                    
                    if (bookManager.Update(db_book) > 0)
                    {
                        OkViewModel ok = new OkViewModel()
                        {
                            Title = "Kitap güncelleme işleminiz başarılı",
                            RedirectingUrl = "/Book/Index"
                        }; 
                    }
                }                
            }
            ErrorViewModel errorViewModel = new ErrorViewModel()
            {
                Title = "Kitap güncelleme işlemi başarısız",
                RedirectingUrl = "/Book/Index"
            };
            return View("Error", errorViewModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = bookManager.Find(x => x.Id == id.Value);
            if (book == null)
            {
                return HttpNotFound();
            }
            var deletedBookComments = commentManager.List(x => x.Book.Id == id);
            if (deletedBookComments.Count > 0)
            {
                foreach (var item in deletedBookComments)
                {
                    commentManager.Delete(item);
                }
            }
            if (bookManager.Delete(book) > 0)
            {
                OkViewModel ok = new OkViewModel()
                {
                    Title = "Kitap silme işleminiz başarılı",
                    RedirectingUrl = "/Book/Index"
                };
            }
            ErrorViewModel errorViewModel = new ErrorViewModel()
            {
                Title = "Kitap silme işlemi başarısız",
                RedirectingUrl = "/Book/Index"
            };
            return View("Error", errorViewModel);
        }

        [HttpPost]
        public ActionResult GetLiked(int[] ids)
        {
            if (ids != null)
            {
                List<int> likedNoteIds = likedManager.List(x => x.LikedUser.Id == CurrentSession.User.Id && ids.Contains(x.Book.Id)).Select(
               x => x.Book.Id).ToList();

                return Json(new { result = likedNoteIds });
            }
            return null;

        }

        public ActionResult GetNoteText(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = bookManager.Find(x => x.Id == id.Value);
            if (book == null)
            {
                return HttpNotFound();
            }

            return PartialView("_PartialNoteText", book);
        }

        [HttpPost]
        public JsonResult SetLikeState(int id, int liked)
        {
            int res = 0;
            Liked like = likedManager.Find(x => x.Book.Id == id && x.LikedUser.Id == CurrentSession.User.Id);//like'lanmış mı diye kontrol edeceğiz

            Book book = bookManager.Find(x => x.Id == id);
            if (like != null && liked <= 0)//db'den like'lanmış olarak kayıt dönmeli ve önyüzden liked nesnesi false yani like'lanmamış olarak dönmeli yani false
            {
                res = likedManager.Delete(like);
            }
            else if (like == null && liked > 0)
            {
                res = likedManager.Insert(new Liked()
                {
                    LikedUser = CurrentSession.User,
                    Book = book,
                    Point = liked
                });
            }
            else if (like != null && liked > 0)
            {
                like.Book = book;
                like.LikedUser.Id = CurrentSession.User.Id;
                like.Point = liked;
                res = likedManager.Update(like);
            }
            if (res > 0)//bir işlem yaptıysam
            {
                if (liked > 0)
                {
                    if (like != null && like.LikedUser.Id != CurrentSession.User.Id)
                    {
                        book.LikeCount++;
                    }
                    else if (like == null)
                    {
                        book.LikeCount++;
                    }
                }
                else
                {
                    book.LikeCount--;
                }
                res = bookManager.Update(book);

                var bookSumPoint = likedManager.List().Where(x => x.Book.Id == id).Sum(x => x.Point);

                var bookLikeCount = bookManager.List().Where(x => x.Id == id).Select(x => x.LikeCount).First();

                var avgPoint = bookSumPoint / bookLikeCount;

                return Json(new { hasError = false, errorMessage = string.Empty, result = avgPoint });
            }

            return Json(new
            {
                hasError = true,
                errorMessage = "Beğenme işlemi gerçekleştirilemedi.",
                result = book.LikeCount
            });

        }
    }
}
