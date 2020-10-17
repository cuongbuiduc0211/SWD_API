using System;
using System.Collections.Generic;

namespace ContentOutSourceAPI.Models
{
    public partial class PostHistory
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int? PostId { get; set; }
        public DateTime HistoryDate { get; set; }
        public int? StatusId { get; set; }

        public virtual TblPosts Post { get; set; }
        public virtual TblPostStatus Status { get; set; }
        public virtual TblUsers UsernameNavigation { get; set; }
    }
}
