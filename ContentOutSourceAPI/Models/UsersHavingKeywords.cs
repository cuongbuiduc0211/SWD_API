using System;
using System.Collections.Generic;

namespace ContentOutSourceAPI.Models
{
    public partial class UsersHavingKeywords
    {
        public string Username { get; set; }
        public int? KeywordId { get; set; }

        public virtual TblKeywords Keyword { get; set; }
        public virtual TblUsers UsernameNavigation { get; set; }
    }
}
