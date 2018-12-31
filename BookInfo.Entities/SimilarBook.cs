using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookInfo.Entities
{
    [Table("SimilarBook")]
    public class SimilarBook
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int BookId { get; set; }

        public int SimilarBookId { get; set; }

    }
}
