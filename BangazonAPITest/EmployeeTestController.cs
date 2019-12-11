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
    public class EmployeeTestController
    {
        private Employee dummyEmployee { get; } = new Employee
        {
            FirstName = "Testy",
            LastName = "Testerson",
            DepartmentId = 1,
            DepartmentName = "Tech",
            AssignedComputer = null,
            IsSupervisor = true

        };
        private string url { get; } = "/api/Employee";
        public async Task<Employee> CreateDummyEmployee()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Serialize the C# object into a JSON string
                string testEmployeeAsJSON = JsonConvert.SerializeObject(dummyEmployee);
                // Use the client to send the request and store the response
                HttpResponseMessage response = await client.PostAsync(
                    url,
                    new StringContent(testEmployeeAsJSON, Encoding.UTF8, "application/json")
                );
                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();
                // Deserialize the JSON into an instance of Employee
                Employee newlyCreatedEmployee = JsonConvert.DeserializeObject<Employee>(responseBody);
                return newlyCreatedEmployee;
            }
        }
        public async Task DeleteDummyEmployee(Employee EmployeeToDelete)
        {
            using (HttpClient client = new APIClientProvider().Client)
            {
                HttpResponseMessage deleteResponse = await client.DeleteAsync($"{url}/{EmployeeToDelete.Id}");
            }
        }
        [Fact]
        public async Task Create_Employee()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Create a new Employee in the db
                Employee newlyCreatedEmployee = await CreateDummyEmployee();
                // Try to get it again
                HttpResponseMessage response = await client.GetAsync($"{url}/{newlyCreatedEmployee.Id}");
                response.EnsureSuccessStatusCode();
                // Turn the response into JSON
                string responseBody = await response.Content.ReadAsStringAsync();
                // Turn the JSON into C#
                Employee newEmployee = JsonConvert.DeserializeObject<Employee>(responseBody);
                // Make sure it's really there
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(dummyEmployee.FirstName, newEmployee.FirstName);
                Assert.Equal(dummyEmployee.LastName, newEmployee.LastName);
                // Clean up after ourselves
                await DeleteDummyEmployee(newEmployee);
            }
        }
        [Fact]
        public async Task Delete_Employee()
        {
            // Note: with many of these methods, I'm creating dummy data and then testing to see if I can delete it. I'd rather do that for now than delete something else I (or a user) created in the database, but it's not essential-- we could test deleting anything 
            // Create a new Employee in the db
            Employee newTestyTesterson = await CreateDummyEmployee();
            // Delete it
            await DeleteDummyEmployee(newTestyTesterson);
            using (var client = new APIClientProvider().Client)
            {
                // Try to get it again
                HttpResponseMessage response = await client.GetAsync($"{url}{newTestyTesterson.Id}");
                // Make sure it's really gone
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
        [Fact]
        public async Task Get_All_Employees()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Try to get all of the Employees from /api/Employees
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                // Convert to JSON
                string responseBody = await response.Content.ReadAsStringAsync();
                // Convert from JSON to C#
                List<Employee> Employees = JsonConvert.DeserializeObject<List<Employee>>(responseBody);
                // Make sure we got back a 200 OK Status and that there are more than 0 Employees in our database
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(Employees.Count > 0);
            }
        }
        [Fact]
        public async Task Get_Single_Employee()
        {
            using (HttpClient client = new APIClientProvider().Client)
            {
                // Create a dummy Employee
                Employee newTestyTesterson = await CreateDummyEmployee();
                // Try to get it
                HttpResponseMessage response = await client.GetAsync($"{url}/{newTestyTesterson.Id}");
                response.EnsureSuccessStatusCode();
                // Turn the response into JSON
                string responseBody = await response.Content.ReadAsStringAsync();
                // Turn the JSON into C#
                Employee TestyTestersonFromDB = JsonConvert.DeserializeObject<Employee>(responseBody);
                // Did we get back what we expected to get back? 
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(newTestyTesterson.FirstName, TestyTestersonFromDB.FirstName);
                Assert.Equal(newTestyTesterson.LastName, TestyTestersonFromDB.LastName);
                Assert.Equal(newTestyTesterson.DepartmentId, TestyTestersonFromDB.DepartmentId);
                Assert.Equal(newTestyTesterson.IsSupervisor, TestyTestersonFromDB.IsSupervisor);
                // Clean up after ourselves-- delete the dummy Employee we just created
                await DeleteDummyEmployee(TestyTestersonFromDB);
            }
        }
        [Fact]
        public async Task Update_Employee()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Create a dummy Employee
                Employee newTestyTesterson = await CreateDummyEmployee();
                // Make a new title and assign it to our dummy Employee
                string newName = "TESTERGLARPLEGLORP";
                newTestyTesterson.FirstName = newName;
                // Convert it to JSON
                string modifiedTestyTestersonAsJSON = JsonConvert.SerializeObject(newTestyTesterson);
                // Try to PUT the newly edited Employee
                var response = await client.PutAsync(
                    $"{url}/{newTestyTesterson.Id}",
                    new StringContent(modifiedTestyTestersonAsJSON, Encoding.UTF8, "application/json")
                );
                // See what comes back from the PUT. Is it a 204? 
                string responseBody = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                // Get the edited Employee back from the database after the PUT
                var getModifiedEmployee = await client.GetAsync($"{url}/{newTestyTesterson.Id}");
                getModifiedEmployee.EnsureSuccessStatusCode();
                // Convert it to JSON
                string getEmployeeBody = await getModifiedEmployee.Content.ReadAsStringAsync();
                // Convert it from JSON to C#
                Employee newlyEditedEmployee = JsonConvert.DeserializeObject<Employee>(getEmployeeBody);
                // Make sure the title was modified correctly
                Assert.Equal(HttpStatusCode.OK, getModifiedEmployee.StatusCode);
                Assert.Equal(newName, newlyEditedEmployee.FirstName);
                // Clean up after yourself
                await DeleteDummyEmployee(newlyEditedEmployee);
            }
        }
        [Fact]
        public async Task Test_Get_NonExitant_Employee_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Try to get a Employee with an Id that could never exist
                HttpResponseMessage response = await client.GetAsync($"{url}/00000000");
                // It should bring back a 204 no content error
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
        }
        [Fact]
        public async Task Test_Delete_NonExistent_Employee_Fails()
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