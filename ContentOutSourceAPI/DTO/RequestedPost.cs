using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentOutSourceAPI.DTO
{
    public class RequestedPost
    {       
        public string Username { get; set; }
        public int PostId { get; set; }
        public string Status { get; set; }
    }
}
