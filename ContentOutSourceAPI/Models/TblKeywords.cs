using System;
using System.Collections.Generic;

namespace ContentOutSourceAPI.Models
{
    public partial class TblKeywords
    {
        public TblKeywords()
        {
            TblPostsHavingKeywords = new HashSet<TblPostsHavingKeywords>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<TblPostsHavingKeywords> TblPostsHavingKeywords { get; set; }
    }
}
