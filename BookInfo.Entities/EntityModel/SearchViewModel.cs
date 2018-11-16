using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookInfo.Entities.EntityModel
{
    public class SearchViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public SearchType Type { get; set; }

        public enum SearchType
        {
            Book, Publisher, Author
        }
    }
}

