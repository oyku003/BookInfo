using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BookInfo.Entities
{
    public class Category : EntityBase
    {
        [DisplayName("Kategori Adı")]
        public string Name { get; set; }
    }
}
