using BookInfo.DataAccessLayer.EntityFramework;
using BookInfo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookInfo.BusinessLayer
{
    public class Test
    {
        private Repository<Book> repo_note = new Repository<Book>();

        public Test()
        {
            List<Book> notes = repo_note.List();

        }
    }
}
