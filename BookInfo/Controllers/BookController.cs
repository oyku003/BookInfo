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
using BookInfo.Models;

namespace BookInfo.Controllers
{
    public class BookController : Controller
    {
        private BookManager bookManager = new BookManager();
        private CategoryManager categoryManager = new CategoryManager();
        private LikedManager likedManager = new LikedManager();

        public ActionResult Index()//Notları listeler
        {
            //var notes = bookManager.ListQueryable().Include("Category").Include("Owner").Where(x => x.Owner.Id == CurrentSession.User.Id).OrderByDescending(x => x.ModifiedOn);//ListQueryable = select * from note  , Include = join, Include içinde note entitysinde user için property adı owner oldugu için onu yazdık

            //return View(notes.ToList());

            return null;
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Book book)
        {
            ModelState.Remove("ModifiedUsername");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("CreatedOn");
            if (ModelState.IsValid)
            {
                book.ModifiedUsername = CurrentSession.User.ToString();
                bookManager.Insert(book);
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(CacheHelper.GetBooksFromCache(), "Id", "Title", book.CategoryId);
            return View(book);
        }
        
        public ActionResult Edit(int? id)
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
            ViewBag.CategoryId = new SelectList(CacheHelper.GetBooksFromCache(), "Id", "Title", note.CategoryId);
            return View(note);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Book book)
        {
            ModelState.Remove("ModifiedUsername");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("CreatedOn");
            if (ModelState.IsValid)
            {
                Book db_note = bookManager.Find(x => x.Id == book.Id);
                db_note.Name = book.Name;
                db_note.Description = book.Description;
                db_note.CategoryId = book.CategoryId;
                bookManager.Update(db_note);
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(CacheHelper.GetBooksFromCache(), "Id", "Title", book.CategoryId);
            return View(book);
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
            return View(book);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = bookManager.Find(x => x.Id == id);
            bookManager.Delete(book);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult GetLiked(int[] ids)
        {//contains = sql'deki in.   
            List<int> likedNoteIds = likedManager.List(x => x.LikedUser.Id == CurrentSession.User.Id && ids.Contains(x.Book.Id)).Select(
                x => x.Book.Id).ToList();
            return Json(new { result = likedNoteIds });
        }

        [HttpPost]
        public ActionResult SetLikeState(int noteid, bool liked)
        {
            int res = 0;
            Liked like = likedManager.Find(x => x.Book.Id == noteid && x.LikedUser.Id == CurrentSession.User.Id);//like'lanmış mı diye kontrol edeceğiz

            Book book = bookManager.Find(x => x.Id == noteid);//notu bulduk
            if (like != null && liked == false)//db'den like'lanmış olarak kayıt dönmeli ve önyüzden liked nesnesi false yani like'lanmamış olarak dönmeli yani false
            {
                res = likedManager.Delete(like);
            }
            else if (like == null && liked == true)
            {
                res = likedManager.Insert(new Liked()
                {
                    LikedUser = CurrentSession.User,
                    Book = book
                });
            }
            if (res > 0)//bir işlem yaptıysam
            {
                if (liked)
                {
                    book.LikeCount++;
                }
                else
                {
                    book.LikeCount--;
                }
                res = bookManager.Update(book);
                return Json(new { hasError = false, errorMessage = string.Empty, result = book.LikeCount });
            }
            return Json(new
            {
                hasError = true,
                errorMessage = "Beğenme işlemi gerçekleştirilemedi.",
                result = book.LikeCount
            });

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
    }
}
