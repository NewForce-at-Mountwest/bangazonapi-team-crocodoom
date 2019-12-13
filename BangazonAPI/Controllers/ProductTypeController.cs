using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BangazonAPI.Models;
using System.Data.SqlClient;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
        // Connecting to the SQL Database
        private readonly IConfiguration _config;

        public ProductTypeController(IConfiguration config)
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

        //Getting all Product Types
        // GET: api/ProductType
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
<<<<<<< HEAD:BangazonAPI/Controllers/EmployeeController.cs
                    string query = "SELECT Employee.Id AS 'Id', FirstName, LastName, isSupervisor, DepartmentId, Department.Name AS 'DepartmentName', Computer.manufacturer AS 'manufacturer', Computer.purchaseDate AS 'purchaseDate', Computer.decomissionDate AS 'decomission', ComputerId, Computer.make AS 'make' FROM Employee LEFT JOIN ComputerEmployee ON ComputerEmployee.EmployeeId = Employee.Id LEFT JOIN Computer ON Computer.Id = ComputerEmployee.ComputerId  LEFT JOIN Department ON DepartmentId = Department.Id";

                    cmd.CommandText = query;
=======
                    cmd.CommandText = @"SELECT Name
                                        FROM ProductType";
>>>>>>> master:BangazonAPI/Controllers/ProductTypeController.cs
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<ProductType> productTypes = new List<ProductType>();

                    while (reader.Read())
                    {
<<<<<<< HEAD:BangazonAPI/Controllers/EmployeeController.cs
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
=======
                        ProductType productType = new ProductType
                        {
                            Name = reader.GetString(reader.GetOrdinal("Name"))

                        };

                        productTypes.Add(productType);
>>>>>>> master:BangazonAPI/Controllers/ProductTypeController.cs
                    }
                    reader.Close();

                    return Ok(productTypes);
                }
            }
        }

        // Get a single ProductType by id
        // GET: api/ProductType/5
        [HttpGet("{id}", Name = "GetProductType")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
<<<<<<< HEAD:BangazonAPI/Controllers/EmployeeController.cs
                    cmd.CommandText = $"SELECT Employee.Id AS 'Id', FirstName, LastName, isSupervisor, DepartmentId, Department.Name AS 'DepartmentName',Computer.manufacturer AS 'manufacturer'," +
                        " Computer.purchaseDate AS 'purchaseDate', Computer.decomissionDate AS 'decomission', ComputerId, Computer.make AS 'make' FROM Employee " +
                        "LEFT JOIN ComputerEmployee ON ComputerEmployee.EmployeeId = Employee.Id LEFT JOIN Computer ON Computer.Id = ComputerEmployee.ComputerId  LEFT JOIN Department ON DepartmentId = Department.Id WHERE Employee.Id = @Id";
                    cmd.Parameters.Add(new SqlParameter("@Id", id));
=======
                    cmd.CommandText = @"
                        SELECT
                             Name
                        FROM ProductType
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
>>>>>>> master:BangazonAPI/Controllers/ProductTypeController.cs
                    SqlDataReader reader = cmd.ExecuteReader();

                    ProductType productType = null;

                    if (reader.Read())
                    {
<<<<<<< HEAD:BangazonAPI/Controllers/EmployeeController.cs
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
=======
                        productType = new ProductType
                        {
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };
>>>>>>> master:BangazonAPI/Controllers/ProductTypeController.cs
                    }
                    reader.Close();

                    return Ok(productType);
                }
            }
        }

        //Post a new Product Type
        // POST: api/ProductType
        public async Task<IActionResult> Post([FromBody] ProductType productType)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO ProductType (Name)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name)";

                    cmd.Parameters.Add(new SqlParameter("@name", productType.Name));


                    int newId = (int)cmd.ExecuteScalar();
                    productType.Id = newId;
                    return CreatedAtRoute("GetProductType", new { id = newId }, productType);
                }
            }
        }

        //Update the PaymentType
        // PUT: api/PaymentType/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] ProductType productType)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE ProductType
                                            SET Name = @name
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@name", productType.Name));



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
                if (!ProductTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

<<<<<<< HEAD:BangazonAPI/Controllers/EmployeeController.cs
        //DELETE method for testing purposes

=======
        //SoftDeleting a ProductType if there is no product assiociated with it
        // DELETE: api/ApiWithActions/5
>>>>>>> master:BangazonAPI/Controllers/ProductTypeController.cs
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductType([FromRoute] int id, bool HardDelete)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        if (HardDelete == true)
                        {
                            cmd.CommandText = @"DELETE ProductType
                                              WHERE id = @id";
                        }
                        else
                        {
                            cmd.CommandText = @"UPDATE ProductType
                                            SET isActive = 0
                                            WHERE id = @id";
                        }
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
                if (!ProductTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool ProductTypeExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Name
                        FROM ProductType
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }


    }
}
