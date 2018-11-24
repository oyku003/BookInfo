using BookInfo.BusinessLayer;
using BookInfo.Entities;
//using MyEverNoteMvc.Filters;
using BookInfo.Models;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MyEverNoteMvc.Controllers
{
    public class CommentController : Controller
    {
        private BookManager bookManager = new BookManager();
        private CommentManager commentManager = new CommentManager();

        public ActionResult ShowNoteComments(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Note note = noteManager.ListQueryable().Include("Comments").FirstOrDefault(x=>x.Id == id);
            Book book = bookManager.Find(x => x.Id == id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return PartialView("_PartialComments", book.Comments);
        }

        [HttpPost]
        public ActionResult Edit(int? id, string text)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = commentManager.Find(x => x.Id == id);

            if (comment == null)
            {
                return new HttpNotFoundResult();
            }
            comment.Text = text;

            if (commentManager.Update(comment) > 0)
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);

        }
       
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = commentManager.Find(x => x.Id == id);
            if (comment == null)
            {
                return new HttpNotFoundResult();
            }

            if (commentManager.Delete(comment) > 0)
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }
       
        [HttpPost]
        public ActionResult Create(Comment comment, int bookid)
        {
            ModelState.Remove("ModifiedUsername");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("CreatedOn");
            if (ModelState.IsValid)
            {
                if (bookid == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Book book = bookManager.Find(x => x.Id == bookid);

                if (book == null)
                {
                    return new HttpNotFoundResult();
                }
                comment.Book = book;
                comment.Owner = CurrentSession.User;

                if (commentManager.Insert(comment) > 0)
                {
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }
    }
}