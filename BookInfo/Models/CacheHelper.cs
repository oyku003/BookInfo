using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using BookInfo.BusinessLayer;
using BookInfo.Entities;
using System.Data.Entity;
namespace BookInfo.Models
{
    public class CacheHelper
    {
        public static IQueryable<Comment> GetBooksFromCache()
        {
            var result = WebCache.Get("book-cache");
            if (result == null)
            {
                CommentManager commentManager = new CommentManager();
                result = commentManager.ListQueryable().Include("Book").Where(x=>x.IsActive == true).OrderByDescending(x=>x.CreatedOn).Take(5);
                WebCache.Set("category-cache", result, 20, true);
            }
            return result;
        }
        public static void RemoveBooksFromCache()
        {
            Remove("book-cache");
        }

        public static void Remove(string key)
        {
            WebCache.Remove(key);
        }
    }
}