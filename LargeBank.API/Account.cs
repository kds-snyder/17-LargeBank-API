//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LargeBank.API
{
    using Models;
    using System;
    using System.Collections.Generic;

    public partial class Account
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Account()
        {
            this.Statements = new HashSet<Statement>();
            this.Transactions = new HashSet<Transaction>();
        }
    
        public int AccountId { get; set; }
        public int CustomerId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
    
        public virtual Customer Customer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Statement> Statements { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transaction> Transactions { get; set; }

        // Update properties in Account class from the input object
        public void Update(AccountModel modelAccount)
        {
            // If adding new account, set created date
            if (modelAccount.AccountId == 0)
            {
                CreatedDate = DateTime.Now;
            }

            // Copy values from input object to Account properties
            AccountNumber = modelAccount.AccountNumber;
            Balance = modelAccount.Balance;
            CustomerId = modelAccount.CustomerId;
        }
    }
}
