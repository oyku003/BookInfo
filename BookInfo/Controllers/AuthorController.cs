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
using BookInfo.ViewModels;

namespace BookInfo.Controllers
{
    public class AuthorController : Controller
    {
        private BookManager bookManager = new BookManager();
        private CategoryManager categoryManager = new CategoryManager();
        private LikedManager likedManager = new LikedManager();
        private AuthorManager authorManager = new AuthorManager();
        private PublisherManager publisherManager = new PublisherManager();
        private CommentManager commentManager = new CommentManager();
               
        public ActionResult Index()
        {
            return View(authorManager.List(x=>x.IsActive == true));
        }
              
        public ActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Create( Author author)
        {
            if (ModelState.IsValid)
            {
                author.IsActive = true;

                if (authorManager.Insert(author) > 0)
                {
                    OkViewModel ok = new OkViewModel()
                    {
                        Title = "Yazar ekleme işleminiz başarılı",
                        RedirectingUrl = "/Author/Index"
                    };

                    return View("Ok", ok);
                }
                ErrorViewModel error = new ErrorViewModel()
                {
                    Title = "Yazar ekleme işleminiz başarısız",
                    RedirectingUrl = "/Author/Index"
                };
                return View("Error", error);
            }

            return View(author);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = authorManager.Find(x=>x.Id == id);
            if (author == null)
            {
                return HttpNotFound();
            }

            return View(author);
        }


        [HttpPost]
        public ActionResult Edit(Author author)
        {
            if (ModelState.IsValid)
            {
                Author newAuthor = authorManager.Find(x=>x.Id == author.Id);

                newAuthor.Name = author.Name;
                newAuthor.Id = author.Id;
                newAuthor.IsActive = true;
                if (authorManager.Update(newAuthor) > 0)
                {
                    OkViewModel ok = new OkViewModel()
                    {
                        Title = "Yazar ekleme işleminiz başarılı",
                        RedirectingUrl = "/Author/Index"
                    };

                    return View("Ok", ok);
                }
                ErrorViewModel error = new ErrorViewModel()
                {
                    Title = "Yazar ekleme işleminiz başarısız",
                    RedirectingUrl = "/Author/Index"
                };
                return View("Error", error);
            }
            return View(author);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = authorManager.Find(x=>x.Id == id);
            if (author == null)
            {
                return HttpNotFound();
            }
            author.IsActive = false;

            if (authorManager.Update(author) > 0)
            {
                OkViewModel ok = new OkViewModel()
                {
                    Title = "Yazar silme işleminiz başarılı",
                    RedirectingUrl = "/Author/Index"
                };
            }
            ErrorViewModel error = new ErrorViewModel()
            {
                Title = "Yazar ekleme işleminiz başarısız",
                RedirectingUrl = "/Author/Index"
            };
            return View("Error", error);
        }

    }
}
