using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BookInfo.Entities
{
    public class Author
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("İsim"), StringLength(25, ErrorMessage = "{0} alanı max. {1} karakter alır.")]
        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}