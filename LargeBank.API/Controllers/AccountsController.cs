using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using LargeBank.API;
using LargeBank.API.Models;

namespace LargeBank.API.Controllers
{
    public class AccountsController : ApiController
    {
        private LargeBankEntities db = new LargeBankEntities();

        // GET: api/Accounts
        public IQueryable<AccountModel> GetAccounts()
        {
            // Project the list of Account objects 
            //  onto a list of AccountModel objects
            return db.Accounts.Select(c => new AccountModel
            {
                AccountId = c.AccountId,
                AccountNumber = c.AccountNumber,
                Balance = c.Balance,
                CreatedDate = c.CreatedDate,
                CustomerId = c.CustomerId 
            }); 
        }

        // GET: api/Accounts/5
        [ResponseType(typeof(AccountModel))]
        public IHttpActionResult GetAccount(int id)
        {
            // Find Account object corresponding to account ID 
            Account dbAccount = db.Accounts.Find(id);

            if (dbAccount == null)
            {
                return NotFound();
            }

           // Populate new AccountModel object from Account object
            AccountModel modelAccount = new AccountModel
            {
                AccountId = dbAccount.AccountId,
                AccountNumber = dbAccount.AccountNumber,
                Balance = dbAccount.Balance,
                CreatedDate = dbAccount.CreatedDate,
                CustomerId = dbAccount.CustomerId
            };

            return Ok(modelAccount);
        }

        // GET: api/accounts/5/transactions
        // Get transactions belonging to account corresponding to account ID
        [Route("api/accounts/{accountId}/transactions")]
        public IHttpActionResult GetTransactions(int accountId)
        {
            // Validate request
            if (!AccountExists(accountId))
            {
                return BadRequest();
            }

            // Get list of accounts where the account ID
            //  matches the input account ID
            var dbTransactions = db.Transactions.Where(t => t.AccountId == accountId);

            if (dbTransactions.Count() == 0)
            {
                return NotFound();
            }

            // Return a list of TransactionModel objects, projected from
            //  the list of Transaction objects
            return Ok(dbTransactions.Select(t => new TransactionModel
            {
                AccountId = t.AccountId,
                Amount = t.Amount,
                TransactionDate = t.TransactionDate,
                TransactionId = t.TransactionId
            }));
        }

        // PUT: api/Accounts/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAccount(int id, AccountModel account)
        {
            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != account.AccountId)
            {
                return BadRequest();
            }

            if (!AccountExists(id))
            {
                return BadRequest();
            }

            // Get the account record corresponding to the account ID, then
            //   update its properties to the values in the input AccountModel object,,
            //   and then set an indicator that the record has been modified
            var dbAccount = db.Accounts.Find(id);
            dbAccount.Update(account);
            db.Entry(dbAccount).State = EntityState.Modified;

            // Perform update by saving changes to DB
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Accounts
        [ResponseType(typeof(AccountModel))]
        public IHttpActionResult PostAccount(AccountModel account)
        {
            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // Set up new Account object,
            //  and populate it with the values from 
            //  the input AccountModel object
            Account dbAccount = new Account();
            dbAccount.Update(account);

            // Add the new Account object to the list of Account objects
            db.Accounts.Add(dbAccount);

            // Save the changes to the DB
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }            

            // Update the AccountModel object with the new account ID
            //  that was placed in the Account object after the changes
            //  were saved to the DB
            account.AccountId = dbAccount.AccountId;
            return CreatedAtRoute("DefaultApi", new { id = dbAccount.AccountId }, account);
        }

        // DELETE: api/Accounts/5
        [ResponseType(typeof(AccountModel))]
        public IHttpActionResult DeleteAccount(int id)
        {
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return NotFound();
            }

            db.Accounts.Remove(account);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
            

            return Ok(account);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AccountExists(int id)
        {
            return db.Accounts.Count(e => e.AccountId == id) > 0;
        }

    }
}