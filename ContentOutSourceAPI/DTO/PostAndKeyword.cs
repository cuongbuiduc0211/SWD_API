using ContentOutSourceAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentOutSourceAPI.DTO
{
    public class PostAndKeyword
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? CharacterLimit { get; set; }
        public long Amount { get; set; }
        public string PostType { get; set; }
        public string RelatedDocument { get; set; }
        public bool? IsPublic { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Status { get; set; }
        public List<string> listKeywords { get; set; }

       
        
    }
}
