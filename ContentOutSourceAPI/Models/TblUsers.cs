using System;
using System.Collections.Generic;

namespace ContentOutSourceAPI.Models
{
    public partial class TblUsers
    {
        public TblUsers()
        {
            PostHistory = new HashSet<PostHistory>();
            TblUsersHavingPosts = new HashSet<TblUsersHavingPosts>();
            TransactionHistoryGiverNavigation = new HashSet<TransactionHistory>();
            TransactionHistoryReceiverNavigation = new HashSet<TransactionHistory>();
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public long Amount { get; set; }
        public string Fullname { get; set; }
        public int Rating { get; set; }
        public string Avatar { get; set; }
        public string Status { get; set; }
       

        public virtual TblRoles Role { get; set; }
        public virtual ICollection<PostHistory> PostHistory { get; set; }
        public virtual ICollection<TblUsersHavingPosts> TblUsersHavingPosts { get; set; }
        public virtual ICollection<TransactionHistory> TransactionHistoryGiverNavigation { get; set; }
        public virtual ICollection<TransactionHistory> TransactionHistoryReceiverNavigation { get; set; }
    }
}
