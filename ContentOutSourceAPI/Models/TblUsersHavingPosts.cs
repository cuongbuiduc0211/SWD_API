using System;
using System.Collections.Generic;

namespace ContentOutSourceAPI.Models
{
    public partial class TblUsersHavingPosts
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int PostId { get; set; }
        public string Status { get; set; }

        public virtual TblPosts Post { get; set; }
        public virtual TblUsers UsernameNavigation { get; set; }
    }
}
