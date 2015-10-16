using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LargeBank.API.Controllers;
using LargeBank.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;

namespace LargeBank.API.Test
{
    [TestClass]
    public class TransactionControllerTests
    {
        [TestMethod]
        public void GetTransactionsReturnsTransactions()
        {
            //Arrange: Instantiate TransactionsController so its methods can be called
            var transactionController = new TransactionsController();

            //Act: Call the GetTransactions method
            IEnumerable<TransactionModel> transactions = transactionController.GetTransactions();

            //Assert: Verify that an array was returned with at least one element
            Assert.IsTrue(transactions.Count() > 0);

        }

        [TestMethod]
        public void GetTransactionReturnsTransaction()
        {
            int transactionIdForTest = 1;

            //Arrange: Instantiate TransactionsController so its methods can be called
            var transactionController = new TransactionsController();

            //Act: Call the GetCustomer method
            IHttpActionResult result = transactionController.GetTransaction(transactionIdForTest);

            //Assert: 
            // Verify that HTTP status code is OK
            // Verify that returned transaction ID is correct
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<TransactionModel>));

            OkNegotiatedContentResult<TransactionModel> contentResult =
                (OkNegotiatedContentResult<TransactionModel>)result;
            Assert.IsTrue(contentResult.Content.TransactionId == transactionIdForTest);
        }

        [TestMethod]
        public void PostTransactionCreatesTransaction()
        {
            int accountIdForTest = 1;

            //Arrange: Instantiate TransactionsController so its methods can be called
            var transactionController = new TransactionsController();

            //Act: 
            // Create a TransactionModel object populated with test data,
            //  and call PostTransaction
            var newTransaction = new TransactionModel
            {
                AccountId = accountIdForTest,
                Amount = -555M,
                TransactionDate = DateTime.Now

            };
            IHttpActionResult result = transactionController.PostTransaction(newTransaction);

            //Assert:
            // Verify that the HTTP result is CreatedAtRouteNegotiatedContentResult
            // Verify that the HTTP result body contains a nonzero transaction ID
            Assert.IsInstanceOfType
                (result, typeof(CreatedAtRouteNegotiatedContentResult<TransactionModel>));
            CreatedAtRouteNegotiatedContentResult<TransactionModel> contentResult =
                (CreatedAtRouteNegotiatedContentResult<TransactionModel>)result;
            Assert.IsTrue(contentResult.Content.TransactionId != 0);

            // Delete the test transaction 
            result = transactionController.DeleteTransaction(contentResult.Content.TransactionId);
        }

        [TestMethod]
        public void PutTransactionUpdatesTransaction()
        {            
            int transactionIdForTest = 1;
            decimal newTransactionAmount;

            //Arrange: Instantiate TransactionsController so its methods can be called
            var transactionController = new TransactionsController();

            //Act: 
            // Get an existing transaction, change it, and
            //  pass it to PutTransaction         

            IHttpActionResult result = 
                transactionController.GetTransaction(transactionIdForTest);
            OkNegotiatedContentResult<TransactionModel> contentResult =
                (OkNegotiatedContentResult<TransactionModel>)result;
            TransactionModel updatedTransaction = (TransactionModel)contentResult.Content;

            decimal amountBeforeUpdate = updatedTransaction.Amount;

            updatedTransaction.Amount += 500.23M;
            newTransactionAmount = updatedTransaction.Amount;

            result = transactionController.PutTransaction
                                 (updatedTransaction.TransactionId, updatedTransaction);

            //Assert: 
            // Verify that HTTP status code is OK
            // Get the transaction and verify that it was updated

            var statusCode = (StatusCodeResult)result;

            Assert.IsTrue(statusCode.StatusCode == System.Net.HttpStatusCode.NoContent);

            result = transactionController.GetTransaction(transactionIdForTest);

            Assert.IsInstanceOfType(result,
                typeof(OkNegotiatedContentResult<TransactionModel>));

            OkNegotiatedContentResult<TransactionModel> readContentResult =
                (OkNegotiatedContentResult<TransactionModel>)result;
            updatedTransaction = (TransactionModel)readContentResult.Content;

            Assert.IsTrue(updatedTransaction.Amount == newTransactionAmount);

            updatedTransaction.Amount = amountBeforeUpdate;

            /*
            updatedTransaction.Amount = 1000M;
            */

            result = transactionController.PutTransaction
                                 (updatedTransaction.TransactionId, updatedTransaction);
        }

        [TestMethod]
        public void DeleteTransactionDeletesTransaction()
        {
            int accountIdForTest = 1;

            //Arrange:
            // Instantiate TransactionsController so its methods can be called
            // Create a new transaction to be deleted, and get its transaction ID 
            var transactionController = new TransactionsController();

            var transaction = new TransactionModel
            {
                AccountId = accountIdForTest,
                Amount = 3451.87M,
                TransactionDate = DateTime.Now
            };
            IHttpActionResult result = 
                    transactionController.PostTransaction(transaction);
            CreatedAtRouteNegotiatedContentResult<TransactionModel> contentResult =
                (CreatedAtRouteNegotiatedContentResult<TransactionModel>)result;

            int transactionIdToDelete = contentResult.Content.TransactionId;

            //Act: Call DeleteTransaction
            result = transactionController.DeleteTransaction(transactionIdToDelete);

            //Assert: 
            // Verify that HTTP result is OK
            // Verify that reading deleted transaction returns result not found
            Assert.IsInstanceOfType(result, 
                typeof(OkNegotiatedContentResult<Transaction>));

            result = transactionController.GetTransaction(transactionIdToDelete);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }

}

