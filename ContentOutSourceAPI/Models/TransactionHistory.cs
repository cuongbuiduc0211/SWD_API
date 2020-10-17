using System;
using System.Collections.Generic;

namespace ContentOutSourceAPI.Models
{
    public partial class TransactionHistory
    {
        public int Id { get; set; }
        public string Giver { get; set; }
        public string Receiver { get; set; }
        public DateTime? TransactionDate { get; set; }
        public long? Amount { get; set; }

        public virtual TblUsers GiverNavigation { get; set; }
        public virtual TblUsers ReceiverNavigation { get; set; }
    }
}
