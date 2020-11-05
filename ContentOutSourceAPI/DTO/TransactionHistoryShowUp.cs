using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentOutSourceAPI.DTO
{
    public class TransactionHistoryShowUp
    {
        public string? PostTitle { get; set; }
        public DateTime? TransactionDate { get; set; }
        public long? Amount { get; set; }
    }
}
