using System;
using System.Collections.Generic;

namespace ContentOutSourceAPI.Models
{
    public partial class TblPostStatus
    {
        public TblPostStatus()
        {
            PostHistory = new HashSet<PostHistory>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<PostHistory> PostHistory { get; set; }
    }
}
