﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BangazonAPI.Models;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CustomerController(IConfiguration config)
        {
            _config = config;
        }
        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: api/Customer
        [HttpGet]
        public async Task<IActionResult> Get(string include, string q)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    string query = $"SELECT Id, FirstName, LastName FROM Customer";
                    if (include == "products")
                    {
                        query = "SELECT Customer.Id AS 'Id', FirstName, LastName, product.Id AS 'ProductId'," +
                            " product.ProductTypeId AS 'productTypeId', product.price AS 'Price', product.title AS 'title', " +
                            "product.quantity AS 'quantity', product.description AS 'Description' FROM Customer " +
                            "Join product ON product.customerId = Customer.Id ";
                    }
                    if (include == "payments")
                    {
                        query = "SELECT Customer.Id AS 'Id', FirstName, LastName, PaymentType.AcctNumber AS 'acct#'," +
                            "PaymentType.Id AS 'paymentId', PaymentType.Name AS 'PaymentName' FROM Customer JOIN PaymentType ON PaymentType.CustomerId = Customer.Id";
                    }
                    if (q != null)
                    {
                        query += $" WHERE FirstName LIKE '%{q}%' OR LastName LIKE '%{q}%'";
                    }
                    cmd.CommandText = query;
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Customer> customers = new List<Customer>();
                    while (reader.Read())
                    { int CustomerIdColumn = reader.GetOrdinal("Id");
                        int CustomerId = reader.GetInt32(CustomerIdColumn);
                        Customer customer = new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))
                        };
                     
                       
                        if (include == "payments")
                        {
                            Payment payment = new Payment
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("paymentId")),
                                AcctNumber = reader.GetString(reader.GetOrdinal("acct#")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("PaymentName"))
                            };
                            if (customers.FirstOrDefault(customer => customer.Id == CustomerId) == null)
                            {
                                customer.payments.Add(payment);
                                customers.Add(customer);
                            }
                            else
                            {
                                Customer customerWithPayment = customers.FirstOrDefault(customer => customer.Id == CustomerId);
                                customerWithPayment.payments.Add(payment);
                            }
                        }
                     
                        if (include == "products")
                        {
                            Product product = new Product
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                ProductTypeId = reader.GetInt32(reader.GetOrdinal("productTypeId")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("Id")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("quantity"))

                            };
                            if (customers.FirstOrDefault(customer => customer.Id == CustomerId) == null)
                            {
                                customer.products.Add(product);
                                customers.Add(customer);
                            }
                            else
                            {
                                Customer customerWithProducts = customers.FirstOrDefault(customer => customer.Id == CustomerId);
                                customerWithProducts.products.Add(product);
                            }
                              
                        }
                        else
                        {
                            if (customers.FirstOrDefault(customer => customer.Id == CustomerId) == null)
                            {
                                customers.Add(customer);
                            }
                        }

                    }
                    reader.Close();
                    return Ok(customers);
                }
            }
        }

        // GET: api/Customer/5
        [HttpGet("{id}", Name = "GetCustomer")]
       
            public async Task<IActionResult> Get([FromRoute] int id)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT Id, FirstName, LastName FROM Customer WHERE Customer.Id = @Id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = cmd.ExecuteReader();
                        Customer customer = null;
                        if (reader.Read())
                        {
                            customer = new Customer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))

                            };
                        }
                        reader.Close();

                        return Ok(customer);
                    }
                } 
            }
        

        // POST: api/Customer
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer customer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Customer (firstName, lastName)
                                        OUTPUT INSERTED.Id
                                        VALUES (@firstName, @lastName)";
                    cmd.Parameters.Add(new SqlParameter("@firstName", customer.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", customer.LastName));
                    int newId = (int)cmd.ExecuteScalar();
                    customer.Id = newId;
                    return CreatedAtRoute("GetCustomer", new { id = newId }, customer);
                }
            }
        }

        // PUT: api/Customer/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Customer customer)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Customer
                                            SET FirstName = @FirstName,
                                                LastName = @LastName
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@FirstName", customer.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@LastName", customer.LastName));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
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
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Customer WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
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
        }
        private bool CustomerExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, FirstName, LastName
                        FROM Customer 
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
