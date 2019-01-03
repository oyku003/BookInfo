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
using BookInfo.Filters;
using BookInfo.Models;
using BookInfo.ViewModels;

namespace BookInfo.Controllers
{
    [Auth]
    [AuthAdmin]
    [Exc]
    public class PublisherController : Controller
    {
        private BookManager bookManager = new BookManager();
        private CategoryManager categoryManager = new CategoryManager();
        private LikedManager likedManager = new LikedManager();
        private AuthorManager authorManager = new AuthorManager();
        private PublisherManager publisherManager = new PublisherManager();
        private CommentManager commentManager = new CommentManager();

        public ActionResult Index()
        {
            return View(publisherManager.List(x => x.IsActive == true));
        }
       
        public ActionResult Create()
        {
            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Publisher publisher)
        {
            if (ModelState.IsValid)
            {
                publisher.IsActive = true;

                if (publisherManager.Insert(publisher) > 0)
                {
                    OkViewModel ok = new OkViewModel()
                    {
                        Title = "Yayınevi ekleme işleminiz başarılı",
                        RedirectingUrl = "/Publisher/Index"
                    };

                    return View("Ok", ok);
                }
                ErrorViewModel error = new ErrorViewModel()
                {
                    Title = "Yayınevi ekleme işleminiz başarısız",
                    RedirectingUrl = "/Publisher/Index"
                };
                return View("Error", error);
            }

            return View(publisher);
        }
      
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Publisher publisher = publisherManager.Find(x => x.Id == id);
            if (publisher == null)
            {
                return HttpNotFound();
            }

            return View(publisher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Publisher publisher)
        {
            if (ModelState.IsValid)
            {
                Publisher newPublisher = publisherManager.Find(x => x.Id == publisher.Id);

                newPublisher.Name = publisher.Name;
                newPublisher.Id = publisher.Id;
                newPublisher.IsActive = true;
                if (publisherManager.Update(newPublisher) > 0)
                {
                    OkViewModel ok = new OkViewModel()
                    {
                        Title = "Yayınevi güncelleme işleminiz başarılı",
                        RedirectingUrl = "/Publisher/Index"
                    };

                    return View("Ok", ok);
                }
                ErrorViewModel error = new ErrorViewModel()
                {
                    Title = "Yazar güncelleme işleminiz başarısız",
                    RedirectingUrl = "/Publisher/Index"
                };
                return View("Error", error);
            }
            return View(publisher);
        }
       
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Publisher publisher = publisherManager.Find(x => x.Id == id);
            if (publisher == null)
            {
                return HttpNotFound();
            }
            publisher.IsActive = false;

            if (publisherManager.Update(publisher) > 0)
            {
                OkViewModel ok = new OkViewModel()
                {
                    Title = "Yayınevi silme işleminiz başarılı",
                    RedirectingUrl = "/Publisher/Index"
                };
            }
            ErrorViewModel error = new ErrorViewModel()
            {
                Title = "Yayınevi ekleme işleminiz başarısız",
                RedirectingUrl = "/Publisher/Index"
            };
            return View("Error", error);
        }      
    }
}
