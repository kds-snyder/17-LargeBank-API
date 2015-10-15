using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LargeBank.API.Models
{
    public class TransactionModel
    {
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
    }
}