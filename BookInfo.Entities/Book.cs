using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BookInfo.Entities
{
    [Table("Books")]
    public class Book : EntityBase
    {
        [DisplayName("Kitap"), StringLength(25, ErrorMessage = "{0} alanı max. {1} karakter")]
        public string Name { get; set; }

        [DisplayName("Konu")]
        public string Description { get; set; }

        [DisplayName("Basım yılı")]
        public int Year { get; set; }

        [DisplayName("Sayfa")]
        public int Page { get; set; }

        [DisplayName("Beğenilme")]
        public int LikeCount { get; set; }

        [DisplayName("Tıklanma")]
        public int? Clicked { get; set; }

        [DisplayName("Yazar")]
        public virtual Author Author { get; set; }

        [DisplayName("Yayın evi")]
        public virtual Publisher Publisher { get; set; }

        [DisplayName("Kategori")]
        public int CategoryId { get; set; }

        public List<Comment> Comments { get; set; }

        public virtual Category Category { get; set; }

        public virtual List<Liked> Likes { get; set; }

        public Book()
        {
            Clicked = 0;
            Comments = new List<Comment>();
            Likes = new List<Liked>();
        }
    }
}
