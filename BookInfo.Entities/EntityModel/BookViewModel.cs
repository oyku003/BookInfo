using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookInfo.Entities.EntityModel
{
    public class BookViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public int Year { get; set; }

        public int Page { get; set; }

        public string Publisher { get; set; }

        public List<Comment> Comments { get; set; }

        public BookViewModel()
        {
            Description = null;
        }
    }
}
