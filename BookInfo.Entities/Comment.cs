using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BookInfo.Entities
{
    [Table("Comments")]
    public class Comment : EntityBase
    {
        [Required, StringLength(300)]
        public string Text { get; set; }

        public virtual Book Book { get; set; }
        public virtual BookUser Owner { get; set; }

    }
}
