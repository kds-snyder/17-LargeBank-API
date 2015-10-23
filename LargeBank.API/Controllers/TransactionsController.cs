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
    public class TransactionsController : ApiController
    {
        private LargeBankEntities db = new LargeBankEntities();

        // GET: api/Transactions
        public IQueryable<TransactionModel> GetTransactions()
        {
            // Project the list of Transactio objects 
            //  onto a list of TransactioModel objects
            return db.Transactions.Select(c => new TransactionModel
            {
                AccountId = c.AccountId,
                Amount = c.Amount,
                TransactionDate = c.TransactionDate,
                TransactionId = c.TransactionId
            });            
        }

        // GET: api/Transactions/5
        [ResponseType(typeof(TransactionModel))]
        public IHttpActionResult GetTransaction(int id)
        {
            // Find Transaction object corresponding to transaction ID 
            Transaction dbTransaction = db.Transactions.Find(id);

            if (dbTransaction == null)
            {
                return NotFound();
            }

            // Populate new TransactionModel object from Transaction object
            TransactionModel modelTransaction = new TransactionModel
            {
                AccountId = dbTransaction.AccountId,
                Amount = dbTransaction.Amount,
                TransactionDate = dbTransaction.TransactionDate,
                TransactionId = dbTransaction.TransactionId
            };

            return Ok(modelTransaction);
        }

        // PUT: api/Transactions/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTransaction(int id, TransactionModel transaction)
        {
            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != transaction.TransactionId)
            {
                return BadRequest();
            }

            if (!TransactionExists(id))
            {
                return BadRequest();
            }

            // Get the transaction record corresponding to the transaction ID, then
            //   update its properties to the values in the input TransactionModel object,,
            //   and then set an indicator that the record has been modified
            var dbTransaction = db.Transactions.Find(id);
            dbTransaction.Update(transaction);
            db.Entry(dbTransaction).State = EntityState.Modified;

            // Perform update by saving changes to DB
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception("Unable to update the transaction in the database.");
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Transactions
        [ResponseType(typeof(TransactionModel))]
        public IHttpActionResult PostTransaction(TransactionModel transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           
            // Set up new Transactio object,
            //  and populate it with the values from 
            //  the input TransactionModel object
            Transaction dbTransaction = new Transaction();
            dbTransaction.Update(transaction);

            // Add the new Transaction object to the list of Transaction objects
            db.Transactions.Add(dbTransaction);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw new Exception("Unable to add the transaction to the database.");
            }

            // Update the TransactionModel object with the new transaction ID
            //  that was placed in the Transaction object after the changes
            //  were saved to the DB
            transaction.TransactionId = dbTransaction.TransactionId;
            return CreatedAtRoute("DefaultApi", new { id = dbTransaction.TransactionId }, transaction);
        }

        // DELETE: api/Transactions/5
        [ResponseType(typeof(TransactionModel))]
        public IHttpActionResult DeleteTransaction(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return NotFound();
            }

            db.Transactions.Remove(transaction);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw new Exception("Unable to delete the transaction from the database.");
            }
            
            return Ok(transaction);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TransactionExists(int id)
        {
            return db.Transactions.Count(e => e.TransactionId == id) > 0;
        }

    }
}