using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LargeBank.API.Models
{
    public class AccountModel
    {
        public int AccountId { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }

        //URL for transactions belonging to account
        public string TransactionsRelativeUrl
        {
            get
            {
                return "/api/accounts/" + AccountId + "/transactions";
            }
        }
    }
}