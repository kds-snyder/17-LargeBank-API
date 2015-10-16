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
    public class AccountControllerTests
    {
        [TestMethod]
        public void GetAccountsReturnsAccounts()
        {
            //Arrange: Instantiate AccountsController so its methods can be called
            var accountController = new AccountsController();

            //Act: Call the GetAccounts method
            IEnumerable<AccountModel> accounts = accountController.GetAccounts();

            //Assert: Verify that an array was returned with at least one element
            Assert.IsTrue(accounts.Count() > 0);
        }

        [TestMethod]
        public void GetAccountReturnsAccount()
        {
            int AccountIdForTest = 1;

            //Arrange: Instantiate AccountsController so its methods can be called
            var accountController = new AccountsController();

            //Act: Call the GetAccount method
            IHttpActionResult result = accountController.GetAccount(AccountIdForTest);

            //Assert: 
            // Verify that HTTP status code is OK
            // Verify that returned account ID is correct
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<AccountModel>));

            OkNegotiatedContentResult<AccountModel> contentResult =
                (OkNegotiatedContentResult<AccountModel>)result;
            Assert.IsTrue(contentResult.Content.AccountId == AccountIdForTest);
        }

        [TestMethod]
        public void GetTransactionsForAccountReturnsTransactions()
        {
            int accountIdForTest = 1;

            //Arrange: Instantiate AccountsController so its methods can be called
            var accountController = new AccountsController();

            //Act: Call the GetTransactionsForAccount method
            IHttpActionResult result = 
                accountController.GetTransactionsForAccount(accountIdForTest);

            //Assert: 
            // Verify that HTTP status code is OK
            // Verify that an array was returned with at least one element
            Assert.IsInstanceOfType(result,
                typeof(OkNegotiatedContentResult<IQueryable<TransactionModel>>));

            OkNegotiatedContentResult<IQueryable<TransactionModel>> contentResult =
                (OkNegotiatedContentResult<IQueryable<TransactionModel>>)result;
            Assert.IsTrue(contentResult.Content.Count() > 0);
        }

        [TestMethod]
        public void PostAccountCreatesAccount()
        {
            int customerIdForTest = 1;

            //Arrange: Instantiate AccountsController so its methods can be called
            var accountController = new AccountsController();

            //Act: 
            // Create a AccountModel object populated with test data,
            //  and call PostAccount
            var newAccount = new AccountModel
            {
                AccountNumber = "223344",
                Balance = 888.77M,
                CreatedDate = DateTime.Now,
                CustomerId = customerIdForTest                
            };
            IHttpActionResult result = accountController.PostAccount(newAccount);

            //Assert:
            // Verify that the HTTP result is CreatedAtRouteNegotiatedContentResult
            // Verify that the HTTP result body contains a nonzero account ID
            Assert.IsInstanceOfType
                (result, typeof(CreatedAtRouteNegotiatedContentResult<AccountModel>));
            CreatedAtRouteNegotiatedContentResult<AccountModel> contentResult =
                (CreatedAtRouteNegotiatedContentResult<AccountModel>)result;
            Assert.IsTrue(contentResult.Content.AccountId != 0);

            // Delete the test account 
            result = accountController.DeleteAccount(contentResult.Content.AccountId);
        }

        [TestMethod]
        public void PutAccountUpdatesAccount()
        {
            int accountIdForTest = 1;
            string accountNumberForTest = "XXXY1";
            decimal balanceForTest = 872345.34M;

            //Arrange: Instantiate AccountsController so its methods can be called
            var accountController = new AccountsController();

            //Act: 
            // Get an existing account, change it, and
            //  pass it to PutAccount           

            IHttpActionResult result = accountController.GetAccount(accountIdForTest);
            OkNegotiatedContentResult<AccountModel> contentResult =
                (OkNegotiatedContentResult<AccountModel>)result;
            AccountModel updatedAccount = (AccountModel)contentResult.Content;

            string accountNumberBeforeUpdate = updatedAccount.AccountNumber;
            decimal balanceBeforeUpdate = updatedAccount.Balance;

            updatedAccount.AccountNumber = accountNumberForTest;
            updatedAccount.Balance = balanceForTest;

            result = accountController.PutAccount
                                     (updatedAccount.AccountId, updatedAccount);

            //Assert: 
            // Verify that HTTP status code is OK
            // Get the account and verify that it was updated

            var statusCode = (StatusCodeResult)result;

            Assert.IsTrue(statusCode.StatusCode == System.Net.HttpStatusCode.NoContent);

            result = accountController.GetAccount(accountIdForTest);

            Assert.IsInstanceOfType(result,
                typeof(OkNegotiatedContentResult<AccountModel>));

            OkNegotiatedContentResult<AccountModel> readContentResult =
                (OkNegotiatedContentResult<AccountModel>)result;
            updatedAccount = (AccountModel)readContentResult.Content;

            Assert.IsTrue(updatedAccount.AccountNumber == accountNumberForTest);
            Assert.IsTrue(updatedAccount.Balance == balanceForTest);            

            updatedAccount.AccountNumber = accountNumberBeforeUpdate;
            updatedAccount.Balance = balanceBeforeUpdate;

            /*
            updatedAccount.AccountNumber = "1000";
            updatedAccount.Balance = 938M;     
            */

            result = accountController.PutAccount
                                 (updatedAccount.AccountId, updatedAccount);
        }

        [TestMethod]
        public void DeleteAccountDeletesAccount()
        {
            int customerIdForTest = 1;

            //Arrange:                       
            // Instantiate AccountsController so its methods can be called
            // Create a new account to be deleted, and get its account ID
            var accountController = new AccountsController();

            var account = new AccountModel
            {
                CustomerId = customerIdForTest,
                CreatedDate = DateTime.Now,
                AccountNumber = "5555",
                Balance = 998877.66M
            };
            IHttpActionResult result = accountController.PostAccount(account);
            CreatedAtRouteNegotiatedContentResult<AccountModel> contentResult =
                (CreatedAtRouteNegotiatedContentResult<AccountModel>)result;

            int accountIdToDelete = contentResult.Content.AccountId;

            //Act: Call DeleteAccount
            result = accountController.DeleteAccount(accountIdToDelete);

            //Assert: 
            // Verify that HTTP result is OK
            // Verify that reading deleted account returns result not found
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<Account>));

            result = accountController.GetAccount(accountIdToDelete);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
