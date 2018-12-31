using BookInfo.BusinessLayer;
using BookInfo.Entities;
//using MyEverNoteMvc.Filters;
using BookInfo.Models;
using PagedList;
using System.Collections.Generic;
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
        public ActionResult ShowBookComments(int id)
        {
            List<Comment> commentList = new List<Comment>();
            commentList = commentManager.List(x => x.Book.Id == id);
            ViewBag.ToplamYorumSayısı = commentList.Count;

            //return PartialView("_PartialUserComments", commentList.ToPagedList(page, 1));
            return PartialView(commentList);
        }

        [HttpPost]
        public ActionResult Update(int commentId, string newComment)
        {
            
            Comment comment = commentManager.Find(x => x.Id == commentId);
                      
            comment.Text = newComment;

            if (commentManager.Update(comment) > 0)
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult Delete(int commentId)
        {         
            Comment comment = commentManager.Find(x => x.Id == commentId);
       
            if (commentManager.Delete(comment) > 0)
            {
                return Json(new { hasError = false, errorMessage = string.Empty });
            }

            return Json(new { hasError = true, errorMessage = "Hata oluştu" });
        }
           
        [HttpPost]
        public JsonResult Create(int id, string commentText)
        {            
            Comment comment = new Comment();
            
            Book book = bookManager.Find(x => x.Id == id);
            comment.Book = book;
            comment.Owner = CurrentSession.User;
            comment.Text = commentText;
            comment.IsActive = true;

            var comments = commentManager.List(x => x.Book.Id == id);
            if (commentManager.Insert(comment) > 0)
            {
                return Json(new { hasError = false, errorMessage = string.Empty});
            }

            return Json(new { hasError = true, errorMessage = "Hata oluştu"});
        }

    }
}