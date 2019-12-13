using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _config;

        public EmployeeController(IConfiguration config)
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
        // GET: api/Employee
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    string query = "SELECT Employee.Id AS 'Id', FirstName, LastName, isSupervisor, DepartmentId, Department.Name AS 'DepartmentName', Computer.manufacturer AS 'manufacturer', Computer.purchaseDate AS 'purchaseDate', Computer.decomissionDate AS 'decomission', ComputerId, Computer.make AS 'make' FROM Employee LEFT JOIN ComputerEmployee ON ComputerEmployee.EmployeeId = Employee.Id LEFT JOIN Computer ON Computer.Id = ComputerEmployee.ComputerId  LEFT JOIN Department ON DepartmentId = Department.Id";

                    cmd.CommandText = query;
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                        int EmployeeIdValue = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (reader.IsDBNull(reader.GetOrdinal("purchaseDate")) == false)
                        {
                            if (reader.IsDBNull(reader.GetOrdinal("decomission")) == false)
                            {
                                Employee employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName")),
                                    IsSupervisor = reader.GetBoolean(reader.GetOrdinal("isSupervisor"))
                                };
                                Computer AssignedComputer = new Computer
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                    make = reader.GetString(reader.GetOrdinal("make")),
                                    manufacturer = reader.GetString(reader.GetOrdinal("manufacturer")),
                                    decomissionDate = reader.GetDateTime(reader.GetOrdinal("decomission")),
                                    purchaseDate = reader.GetDateTime(reader.GetOrdinal("purchaseDate"))
                                };
                                if (employees.FirstOrDefault(employee => employee.Id == EmployeeIdValue) == null)
                                {
                                    employee.AssignedComputers.Add(AssignedComputer);
                                    employees.Add(employee);
                                }

                            }
                            else
                            {
                                Employee employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName")),
                                    IsSupervisor = reader.GetBoolean(reader.GetOrdinal("isSupervisor"))
                                  
                                };
                                Computer AssignedComputer = new Computer
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                    make = reader.GetString(reader.GetOrdinal("make")),
                                    manufacturer = reader.GetString(reader.GetOrdinal("manufacturer")),
                                    purchaseDate = reader.GetDateTime(reader.GetOrdinal("purchaseDate"))
                                };
                                if (employees.FirstOrDefault(employee => employee.Id == EmployeeIdValue) == null)
                                {
                                    employee.AssignedComputers.Add(AssignedComputer);
                                    employees.Add(employee);
                                }
                            }
                        }

                        else
                        {
                            Employee employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName")),
                                IsSupervisor = reader.GetBoolean(reader.GetOrdinal("isSupervisor"))

                            };
                            if (employees.FirstOrDefault(employee => employee.Id == EmployeeIdValue) == null)
                            {

                                employees.Add(employee);
                            }
                        }
                    }
                    reader.Close();
                    return Ok(employees);
                }
            }
        }

        // GET: api/Employee/5
        [HttpGet("{id}", Name = "GetEmployee")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"SELECT Employee.Id AS 'Id', FirstName, LastName, isSupervisor, DepartmentId, Department.Name AS 'DepartmentName',Computer.manufacturer AS 'manufacturer'," +
                        " Computer.purchaseDate AS 'purchaseDate', Computer.decomissionDate AS 'decomission', ComputerId, Computer.make AS 'make' FROM Employee " +
                        "LEFT JOIN ComputerEmployee ON ComputerEmployee.EmployeeId = Employee.Id LEFT JOIN Computer ON Computer.Id = ComputerEmployee.ComputerId  LEFT JOIN Department ON DepartmentId = Department.Id WHERE Employee.Id = @Id";
                    cmd.Parameters.Add(new SqlParameter("@Id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    Employee employee = null;
                    if (reader.Read())
                    {
                        int EmployeeIdValue = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (reader.IsDBNull(reader.GetOrdinal("purchaseDate")) == false)
                        {
                            if (reader.IsDBNull(reader.GetOrdinal("decomission")) == false)
                            {
                                employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName")),
                                    IsSupervisor = reader.GetBoolean(reader.GetOrdinal("isSupervisor")),
                               
                                };
                                Computer AssignedComputer = new Computer
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                    make = reader.GetString(reader.GetOrdinal("make")),
                                    manufacturer = reader.GetString(reader.GetOrdinal("manufacturer")),
                                    decomissionDate = reader.GetDateTime(reader.GetOrdinal("decomission")),
                                    purchaseDate = reader.GetDateTime(reader.GetOrdinal("purchaseDate"))
                                };
                                employee.AssignedComputers.Add(AssignedComputer);
                            }
                            else
                            {
                                employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName")),
                                    IsSupervisor = reader.GetBoolean(reader.GetOrdinal("isSupervisor")),
                                 
                                };
                                Computer AssignedComputer = new Computer
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                    make = reader.GetString(reader.GetOrdinal("make")),
                                    manufacturer = reader.GetString(reader.GetOrdinal("manufacturer")),
                                    purchaseDate = reader.GetDateTime(reader.GetOrdinal("purchaseDate"))
                                };
                                employee.AssignedComputers.Add(AssignedComputer);

                            }
                        }

                        else
                        {
                            employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName")),
                                IsSupervisor = reader.GetBoolean(reader.GetOrdinal("isSupervisor"))

                            };

                        }
                    }
                    reader.Close();

                    return Ok(employee);
                }
            }
        }

        // POST: api/Employee
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Employee employee)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Employee (firstName, lastName, DepartmentId, isSupervisor)
                                        OUTPUT INSERTED.Id
                                        VALUES (@firstName, @lastName, @DepartmentId, @isSupervisor)";
                    cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                    cmd.Parameters.Add(new SqlParameter("@DepartmentId", employee.DepartmentId));
                    cmd.Parameters.Add(new SqlParameter("@isSupervisor", employee.IsSupervisor));
                    int newId = (int)cmd.ExecuteScalar();
                    employee.Id = newId;
                    return CreatedAtRoute("GetEmployee", new { id = newId }, employee);
                }
            }
        }

        // PUT: api/Employee/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Employee employee)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Employee
                                            SET FirstName = @FirstName,
                                                LastName = @LastName,
                                                DepartmentId = @DepartmentId,
                                                isSupervisor = @isSupervisor
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@FirstName", employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@LastName", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@DepartmentId", employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@isSupervisor", employee.IsSupervisor));
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
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        //DELETE method for testing purposes

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
                        cmd.CommandText = @"DELETE FROM Employee WHERE Id = @id";
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
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        private bool EmployeeExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, FirstName, LastName, DepartmentId, isSupervisor
                        FROM Employee 
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
