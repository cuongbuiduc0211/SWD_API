using System;
using System.Collections.Generic;

namespace ContentOutSourceAPI.Models
{
    public partial class TblPostsHavingKeywords
    {
        public int Id { get; set; }
        public int? PostId { get; set; }
        public int? KeywordId { get; set; }

        public virtual TblKeywords Keyword { get; set; }
        public virtual TblPosts Post { get; set; }
    }
}
