using System;
using System.Collections.Generic;

namespace ContentOutSourceAPI.Models
{
    public partial class TblPosts
    {
        public TblPosts()
        {
            PostHistory = new HashSet<PostHistory>();
            TblPostsHavingKeywords = new HashSet<TblPostsHavingKeywords>();
            TblUsersHavingPosts = new HashSet<TblUsersHavingPosts>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? CharacterLimit { get; set; }
        public long Amount { get; set; }
        public string PostType { get; set; }
        public string RelatedDocument { get; set; }
        public Boolean IsPublic { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }

        public virtual ICollection<PostHistory> PostHistory { get; set; }
        public virtual ICollection<TblPostsHavingKeywords> TblPostsHavingKeywords { get; set; }
        public virtual ICollection<TblUsersHavingPosts> TblUsersHavingPosts { get; set; }
    }
}
