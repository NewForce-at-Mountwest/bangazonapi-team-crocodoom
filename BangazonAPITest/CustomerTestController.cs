using System;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Net;
using System.Collections.Generic;

namespace BangazonAPITest
{
    public class CustomerTestController
    {
        private Customer dummyCustomer { get; } = new Customer
        {
            FirstName = "Testy",
            LastName = "Testerson"
        };
        private string url { get; } = "/api/Customer";
        public async Task<Customer> CreateDummyCustomer()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Serialize the C# object into a JSON string
                string testCustomerAsJSON = JsonConvert.SerializeObject(dummyCustomer);
                // Use the client to send the request and store the response
                HttpResponseMessage response = await client.PostAsync(
                    url,
                    new StringContent(testCustomerAsJSON, Encoding.UTF8, "application/json")
                );
                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();
                // Deserialize the JSON into an instance of Customer
                Customer newlyCreatedCustomer = JsonConvert.DeserializeObject<Customer>(responseBody);
                return newlyCreatedCustomer;
            }
        }
        public async Task DeleteDummyCustomer(Customer CustomerToDelete)
        {
            using (HttpClient client = new APIClientProvider().Client)
            {
                HttpResponseMessage deleteResponse = await client.DeleteAsync($"{url}/{CustomerToDelete.Id}");
            }
        }
        [Fact]
        public async Task Create_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Create a new Customer in the db
                Customer newlyCreatedCustomer = await CreateDummyCustomer();
                // Try to get it again
                HttpResponseMessage response = await client.GetAsync($"{url}/{newlyCreatedCustomer.Id}");
                response.EnsureSuccessStatusCode();
                // Turn the response into JSON
                string responseBody = await response.Content.ReadAsStringAsync();
                // Turn the JSON into C#
                Customer newCustomer = JsonConvert.DeserializeObject<Customer>(responseBody);
                // Make sure it's really there
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(dummyCustomer.FirstName, newCustomer.FirstName);
                Assert.Equal(dummyCustomer.LastName, newCustomer.LastName);
                // Clean up after ourselves
                await DeleteDummyCustomer(newCustomer);
            }
        }
        [Fact]
        public async Task Delete_Customer()
        {
            // Note: with many of these methods, I'm creating dummy data and then testing to see if I can delete it. I'd rather do that for now than delete something else I (or a user) created in the database, but it's not essential-- we could test deleting anything 
            // Create a new Customer in the db
            Customer newTestyTesterson = await CreateDummyCustomer();
            // Delete it
            await DeleteDummyCustomer(newTestyTesterson);
            using (var client = new APIClientProvider().Client)
            {
                // Try to get it again
                HttpResponseMessage response = await client.GetAsync($"{url}{newTestyTesterson.Id}");
                // Make sure it's really gone
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
        [Fact]
        public async Task Get_All_Customers()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Try to get all of the Customers from /api/Customers
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                // Convert to JSON
                string responseBody = await response.Content.ReadAsStringAsync();
                // Convert from JSON to C#
                List<Customer> Customers = JsonConvert.DeserializeObject<List<Customer>>(responseBody);
                // Make sure we got back a 200 OK Status and that there are more than 0 Customers in our database
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(Customers.Count > 0);
            }
        }
        [Fact]
        public async Task Get_Single_Customer()
        {
            using (HttpClient client = new APIClientProvider().Client)
            {
                // Create a dummy Customer
                Customer newTestyTesterson = await CreateDummyCustomer();
                // Try to get it
                HttpResponseMessage response = await client.GetAsync($"{url}/{newTestyTesterson.Id}");
                response.EnsureSuccessStatusCode();
                // Turn the response into JSON
                string responseBody = await response.Content.ReadAsStringAsync();
                // Turn the JSON into C#
                Customer TestyTestersonFromDB = JsonConvert.DeserializeObject<Customer>(responseBody);
                // Did we get back what we expected to get back? 
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(newTestyTesterson.FirstName, TestyTestersonFromDB.FirstName);
                Assert.Equal(newTestyTesterson.LastName, TestyTestersonFromDB.LastName);
                // Clean up after ourselves-- delete the dummy Customer we just created
                await DeleteDummyCustomer(TestyTestersonFromDB);
            }
        }
        [Fact]
        public async Task Update_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Create a dummy Customer
                Customer newTestyTesterson = await CreateDummyCustomer();
                // Make a new title and assign it to our dummy Customer
                string newName = "TESTERGLARPLEGLORP";
                newTestyTesterson.FirstName = newName;
                // Convert it to JSON
                string modifiedTestyTestersonAsJSON = JsonConvert.SerializeObject(newTestyTesterson);
                // Try to PUT the newly edited Customer
                var response = await client.PutAsync(
                    $"{url}/{newTestyTesterson.Id}",
                    new StringContent(modifiedTestyTestersonAsJSON, Encoding.UTF8, "application/json")
                );
                // See what comes back from the PUT. Is it a 204? 
                string responseBody = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                // Get the edited Customer back from the database after the PUT
                var getModifiedCustomer = await client.GetAsync($"{url}/{newTestyTesterson.Id}");
                getModifiedCustomer.EnsureSuccessStatusCode();
                // Convert it to JSON
                string getCustomerBody = await getModifiedCustomer.Content.ReadAsStringAsync();
                // Convert it from JSON to C#
                Customer newlyEditedCustomer = JsonConvert.DeserializeObject<Customer>(getCustomerBody);
                // Make sure the title was modified correctly
                Assert.Equal(HttpStatusCode.OK, getModifiedCustomer.StatusCode);
                Assert.Equal(newName, newlyEditedCustomer.FirstName);
                // Clean up after yourself
                await DeleteDummyCustomer(newlyEditedCustomer);
            }
        }
        [Fact]
        public async Task Test_Get_NonExitant_Customer_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Try to get a Customer with an Id that could never exist
                HttpResponseMessage response = await client.GetAsync($"{url}/00000000");
                // It should bring back a 204 no content error
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
        }
        [Fact]
        public async Task Test_Delete_NonExistent_Customer_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Try to delete an Id that shouldn't exist 
                HttpResponseMessage deleteResponse = await client.DeleteAsync($"{url}0000000000");
                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }
    }
}
