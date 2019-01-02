using System.Collections.Generic;

namespace BookInfo.Entities.EntityModel
{
    public class UserCommentViewModel
    {
        public List<Comment> Comments { get; set; }
        public int PageCount { get; set; }
        public int CommentCount { get; set; }
    }
}
