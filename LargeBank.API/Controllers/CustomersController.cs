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
    public class CustomersController : ApiController
    {
        private LargeBankEntities db = new LargeBankEntities();

        // GET: api/Customers
        public IQueryable<CustomerModel> GetCustomers()
        {
            // Project the list of Customer objects 
            //  onto a list of CustomerModel objects
            return db.Customers.Select(c => new CustomerModel
            {
                Address1 = c.Address1,
                Address2 = c.Address2,
                City = c.City,
                CreatedDate = c.CreatedDate,
                CustomerId = c.CustomerId,
                FirstName = c.FirstName,
                LastName = c.LastName,
                State = c.State,
                Zip = c.Zip
            });
        }

        // GET: api/Customers/5
        [ResponseType(typeof(CustomerModel))]
        public IHttpActionResult GetCustomer(int id)
        {
            // Find Customer object corresponding to customer ID
            Customer dbCustomer = db.Customers.Find(id);

            if (dbCustomer == null)
            {
                return NotFound();
            }

            // Populate new CustomerModel object from Customer object
            CustomerModel modelCustomer = new CustomerModel
            {
                Address1 = dbCustomer.Address1,
                Address2 = dbCustomer.Address2,
                City = dbCustomer.City,
                CreatedDate = dbCustomer.CreatedDate,
                CustomerId = dbCustomer.CustomerId,
                FirstName = dbCustomer.FirstName,
                LastName = dbCustomer.LastName,
                State = dbCustomer.State,
                Zip = dbCustomer.Zip
            };

            return Ok(modelCustomer);
        }

        // PUT: api/Customers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCustomer(int id, CustomerModel customer)
        {
            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            if (!CustomerExists(id))
            {
                return BadRequest();
            }

            //  Get the customer record corresponding to the customer ID, then
            //   update its properties to the values in the input CustomerModel object,
            //   and then set an indicator that the record has been modified
            var dbCustomer = db.Customers.Find(id);
            dbCustomer.Update(customer);
            db.Entry(dbCustomer).State = EntityState.Modified;          

            // Perform update by saving changes to DB
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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

        // POST: api/Customers
        [ResponseType(typeof(CustomerModel))]
        public IHttpActionResult PostCustomer(CustomerModel customer)
        {
            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Set up new Customer object,
            //  and populate it with the values from 
            //  the input CustomerModel object
            Customer dbCustomer = new Customer();
            dbCustomer.Update(customer);

            // Add the new Customer object to the list of Customer objects
            db.Customers.Add(dbCustomer);

            // Save the changes to the DB
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
            
            // Update the CustomerModel object with the new customer ID
            //  that was placed in the Customer object after the changes
            //  were saved to the DB
            customer.CustomerId = dbCustomer.CustomerId;
            return CreatedAtRoute("DefaultApi", new { id = dbCustomer.CustomerId }, customer);
        }

        // DELETE: api/Customers/5
        [ResponseType(typeof(CustomerModel))]
        public IHttpActionResult DeleteCustomer(int id)
        {
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return NotFound();
            }

            db.Customers.Remove(customer);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
            
            return Ok(customer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerExists(int id)
        {
            return db.Customers.Count(e => e.CustomerId == id) > 0;
        }
    }
}