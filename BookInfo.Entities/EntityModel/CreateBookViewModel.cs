using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookInfo.Entities.EntityModel
{
    public class CreateBookViewModel  : EntityBase
    {
        [DisplayName("Kitap"), Required, StringLength(40)]
        public string Name { get; set; }
        [DisplayName("Konu"), Required, DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [DisplayName("Yıl"), Required]
        public int Year { get; set; }
        [DisplayName("Sayfa"), Required]
        public int Page { get; set; }
        [DisplayName("Yazar"), Required]
        public int Author { get; set; }
        [DisplayName("Kategori"), Required]
        public int Category { get; set; }
        [DisplayName("Yayın evi"), Required]
        public int Publisher { get; set; }
    }
}
