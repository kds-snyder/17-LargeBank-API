using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using LargeBank.API.Controllers;
using LargeBank.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;

namespace LargeBank.API.Test
{
    [TestClass]
    public class CustomerControllerTests
    {
        [TestMethod]
        public void GetCustomersReturnsCustomers()
        {
            //Arrange: Instantiate CustomersController so its methods can be called
            var customerController = new CustomersController();

            //Act: Call the GetCustomers method
            IEnumerable<CustomerModel> customers = customerController.GetCustomers();

            //Assert: Verify that an array was returned with at least one element
            Assert.IsTrue(customers.Count() > 0);
        
        }

        [TestMethod]
        public void GetCustomerReturnsCustomer()
        {
            int CustomerIdForTest = 1;

            //Arrange: Instantiate CustomersController so its methods can be called
            var customerController = new CustomersController();

            //Act: Call the GetCustomer method
            IHttpActionResult result = customerController.GetCustomer(CustomerIdForTest);

            //Assert: 
            // Check that HTTP status code is OK
            // Check that returned customer ID is correct
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<CustomerModel>));

            OkNegotiatedContentResult<CustomerModel> contentResult =
                (OkNegotiatedContentResult<CustomerModel>)result;
            Assert.IsTrue(contentResult.Content.CustomerId == CustomerIdForTest);
        }

        [TestMethod]
        public void PostcustomerCreatesCustomer()
        {
            //Arrange: Instantiate CustomersController so its methods can be called
            var customerController = new CustomersController();

            //Act: 
            // Create a CustomerModel object populated with test data,
            //  and call PostCustomer
            var newCustomer = new CustomerModel
            {
                FirstName = "Testy",
                LastName = "McTesterson",
                Address1 = "Land of QA",
                Address2 = "34 Broome St",
                City = "San Francisco",
                State = "CA",
                Zip = "92456"
            };
            IHttpActionResult result = customerController.PostCustomer(newCustomer);

            //Assert:
            // Verify that the HTTP result is CreatedAtRouteNegotiatedContentResult
            // Verify that the HTTP result body contains a nonzero customer ID
            Assert.IsInstanceOfType
                (result, typeof(CreatedAtRouteNegotiatedContentResult<CustomerModel>));
            CreatedAtRouteNegotiatedContentResult<CustomerModel> contentResult =
                (CreatedAtRouteNegotiatedContentResult<CustomerModel>)result;
            Assert.IsTrue(contentResult.Content.CustomerId != 0);

            // Delete the test customer 
            result = customerController.DeleteCustomer(contentResult.Content.CustomerId);
        }

        [TestMethod]
        public void PutCustomerUpdatesCustomer()
        {
            int CustomerIdForTest = 1;

            //Arrange: Instantiate CustomersController so its methods can be called
            var customerController = new CustomersController();

            //Act: 
            // Get an existing customer, change it, and
            //  pass it to PutCustomer           

            IHttpActionResult result = customerController.GetCustomer(CustomerIdForTest);
            OkNegotiatedContentResult<CustomerModel> contentResult =
                (OkNegotiatedContentResult<CustomerModel>)result;
            CustomerModel updatedCustomer = (CustomerModel)contentResult.Content;

            string address1BeforeUpdate = updatedCustomer.Address1;
            string cityBeforeUpdate = updatedCustomer.City;
            string stateBeforeUpdate = updatedCustomer.State;
            string zipBeforeUpdate = updatedCustomer.Zip;

            updatedCustomer.Address1 = "567 State Street";
            updatedCustomer.City = "Philadelphia";
            updatedCustomer.State = "PA";
            updatedCustomer.Zip = "14378";

            result = customerController.PutCustomer
                                     (updatedCustomer.CustomerId, updatedCustomer);

            //Assert: 
            // Check that HTTP status code is OK
            // Get the customer and check that it was updated

            var statusCode = (StatusCodeResult)result;

            Assert.IsTrue(statusCode.StatusCode == System.Net.HttpStatusCode.NoContent);

            result = customerController.GetCustomer(CustomerIdForTest);

            Assert.IsInstanceOfType(result,
                typeof(OkNegotiatedContentResult<CustomerModel>));

            OkNegotiatedContentResult<CustomerModel> readContentResult =
                (OkNegotiatedContentResult<CustomerModel>)result;
            updatedCustomer = (CustomerModel)readContentResult.Content;

            Assert.IsTrue(updatedCustomer.Address1 == "567 State Street");
            Assert.IsTrue(updatedCustomer.City == "Philadelphia");
            Assert.IsTrue(updatedCustomer.State == "PA");
            Assert.IsTrue(updatedCustomer.Zip == "14378");

            updatedCustomer.Address1 = address1BeforeUpdate;
            updatedCustomer.City = cityBeforeUpdate;
            updatedCustomer.State = stateBeforeUpdate;
            updatedCustomer.Zip = zipBeforeUpdate;
            
            /*
            updatedCustomer.Address1 = "123 Fake Street";
            updatedCustomer.City = "San Diego";
            updatedCustomer.State = "CA";
            updatedCustomer.Zip = "92101";
            */
            
            result = customerController.PutCustomer
                                 (updatedCustomer.CustomerId, updatedCustomer);
        }

        [TestMethod]
        public void DeleteCustomerDeletesCustomer()
        {

            //Arrange:
            // Create a new customer to be deleted, and get its customer ID           
            var customerController = new CustomersController();
 
            var customer = new CustomerModel
            {
                FirstName = "Jim",
                LastName = "McDonald",
                Address1 = "Farm",
                Address2 = "Yard",
                City = "Denver",
                State = "CO",
                Zip = "56432"
            };
            IHttpActionResult result = customerController.PostCustomer(customer);
            CreatedAtRouteNegotiatedContentResult<CustomerModel> contentResult =
                (CreatedAtRouteNegotiatedContentResult<CustomerModel>)result;

            int customerIdToDelete = contentResult.Content.CustomerId;

            //Act: Call DeleteCustomer 
            result = customerController.DeleteCustomer(customerIdToDelete);

            //Assert: 
            // Check that HTTP result is OK
            // Check that reading deleted customer returns result not found
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<Customer>));
           
            result = customerController.GetCustomer(customerIdToDelete);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));                      
        }
    }
}
