using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentOutSourceAPI.DTO
{
    public class TransactionHistoryDTO
    {
        public int PostId { get; set; }
        public string Giver { get; set; }
        public string Receiver { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
