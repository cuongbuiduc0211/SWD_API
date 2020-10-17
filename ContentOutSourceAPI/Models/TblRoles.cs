using System;
using System.Collections.Generic;

namespace ContentOutSourceAPI.Models
{
    public partial class TblRoles
    {
        public TblRoles()
        {
            TblUsers = new HashSet<TblUsers>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<TblUsers> TblUsers { get; set; }
    }
}
