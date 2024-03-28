using DBSD.CW2._12882._14757._13372.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace DBSD.CW2._12882._14757._13372.DAL
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private string ConnStr
        {
            get
            {
                return WebConfigurationManager.ConnectionStrings["BelissimoConnStr"].ConnectionString;
            }
        }
        public async Task<IEnumerable<Employee>> GetAll()
        {
            var list = new List<Employee>();
            using (var conn = new SqlConnection(ConnStr)) 
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "getAllEmployees";
                    cmd.CommandType = CommandType.StoredProcedure;
                    await conn.OpenAsync();
                    using (var rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            var emp = new Employee();
                            emp.EmployeeId = rdr.GetInt32(rdr.GetOrdinal("EmployeeId"));
                            emp.FirstName = rdr.GetString(rdr.GetOrdinal("FirstName"));
                            emp.LastName = rdr.GetString(rdr.GetOrdinal("LastName"));
                            emp.Phone = rdr.GetString(rdr.GetOrdinal("Phone"));
                            emp.Email = rdr.GetString(rdr.GetOrdinal("Email"));
                            emp.HireDate = rdr.GetDateTime(rdr.GetOrdinal("HireDate"));
                            if (!(rdr.IsDBNull(rdr.GetOrdinal("EmployeeImage"))))
                            {
                                emp.EmployeeImage = (byte[])rdr["EmployeeImage"];
                            }

                            emp.FullTimeEmployee = rdr.GetBoolean(rdr.GetOrdinal("FullTimeEmployee"));
                            list.Add(emp);
                        }
                        return list;
                    }
                    
                }
                
            } 
            
        }



        public Employee GetById(int id)
        {
            Employee employee = null;
            using(var conn = new SqlConnection(ConnStr))
            {
                using(var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT [EmployeeID]
                                              ,[FirstName]
                                              ,[LastName]
                                              ,[Phone]
                                              ,[Email]
                                              ,[HireDate]
                                              ,[EmployeeImage]
                                              ,[FullTimeEmployee]
                                          FROM [dbo].[Employees]
                                          WHERE EmployeeID = @EmployeeID
                                                                        ";
                    cmd.Parameters.AddWithValue("@EmployeeID", id);
                    conn.Open();

                    using(var rdr = cmd.ExecuteReader())
                    {
                        if(rdr.Read())
                        {
                            employee = new Employee()
                            {
                                EmployeeId = id,
                                FirstName = rdr.GetString(rdr.GetOrdinal("FirstName")),
                                LastName = rdr.GetString(rdr.GetOrdinal("LastName")),
                                Phone = rdr.GetString(rdr.GetOrdinal("Phone")),
                                Email = rdr.GetString(rdr.GetOrdinal("Email")),
                                HireDate = rdr.GetDateTime(rdr.GetOrdinal("HireDate")),
                                EmployeeImage = rdr.IsDBNull(rdr.GetOrdinal("EmployeeImage")) ? null : (byte[])rdr["EmployeeImage"],
                                FullTimeEmployee = rdr.GetBoolean(rdr.GetOrdinal("FullTimeEmployee")),
                            };
                        }
                    }
                }

                return employee;
            }
        }

        public int Insert(Employee emp)
        {
            using (var conn = new SqlConnection(ConnStr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "InsertEmployee";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FirstName", emp.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", emp.FirstName);
                    cmd.Parameters.AddWithValue("@Phone", emp.Phone);
                    cmd.Parameters.AddWithValue("@Email", emp.Email);
                    cmd.Parameters.AddWithValue("@HireDate", emp.HireDate);
                    if (emp.EmployeeImage != null)
                    {
                        cmd.Parameters.Add("@EmployeeImage", SqlDbType.VarBinary, -1).Value = emp.EmployeeImage;
                    }
                    else
                    {
                        cmd.Parameters.Add("@EmployeeImage", SqlDbType.VarBinary, -1).Value = DBNull.Value;
                    }
                    cmd.Parameters.AddWithValue("@FullTimeEmployee", emp.FullTimeEmployee);

                    var pError = cmd.Parameters.Add
                        (
                            "@Errors",
                            sqlDbType: SqlDbType.NVarChar,
                            size: 1000
                        );
                    pError.Direction = ParameterDirection.Output;

                    var pRetVal = cmd.Parameters.Add
                        (
                            "RetVal",
                            sqlDbType: SqlDbType.Int
                        );
                    pRetVal.Direction = ParameterDirection.ReturnValue;
                    conn.Open();

                    int? id = (int?)cmd.ExecuteScalar();
                    emp.EmployeeId = id ?? 0;

                    int code = (int)pRetVal.Value;
                    string err = pError.Value is DBNull
                        ? ""
                        : (string)pError.Value;

                    if(code != 0)
                    {
                        throw new Exception($"code={code}, error={err}");
                    }
                    return id ?? 0;
                }
            }
        }

        public void Update(Employee emp)
        {
            using(var conn = new SqlConnection(ConnStr))
            {
                using(var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE [dbo].[Employees]
                                           SET [FirstName] = @FirstName
                                              ,[LastName] = @LastName
                                              ,[Phone] = @Phone
                                              ,[Email] = @Email
                                              ,[HireDate] = @HireDate
                                              ,[EmployeeImage] = @EmployeeImage
                                              ,[FullTimeEmployee] = @FullTimeEmployee

                                         WHERE EmployeeID = @EmployeeID";

                    cmd.Parameters.AddWithValue("@FirstName", emp.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", emp.FirstName);
                    cmd.Parameters.AddWithValue("@Phone", emp.Phone);
                    cmd.Parameters.AddWithValue("@Email", emp.Email);
                    cmd.Parameters.AddWithValue("@HireDate", emp.HireDate);

                    if (emp.EmployeeImage != null)
                    {
                        cmd.Parameters.Add("@EmployeeImage", SqlDbType.VarBinary, -1).Value = emp.EmployeeImage;
                    }
                    else
                    {
                        cmd.Parameters.Add("@EmployeeImage", SqlDbType.VarBinary, -1).Value = DBNull.Value;
                    }

                    cmd.Parameters.AddWithValue("@FullTimeEmployee", emp.FullTimeEmployee);
                    cmd.Parameters.AddWithValue("@EmployeeID", emp.EmployeeId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int DeleteWithStoredProc(int id) 
        {
            using(var conn = new SqlConnection(ConnStr))
            {
                using(var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"[dbo].[delEmployees]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("id", id);

                    var pErr = cmd.CreateParameter();
                    pErr.ParameterName = "err";
                    pErr.Direction = ParameterDirection.Output;
                    pErr.SqlDbType = SqlDbType.VarChar;
                    pErr.Size = 1000;
                    cmd.Parameters.Add(pErr);

                    var pRet = cmd.CreateParameter();
                    pRet.Direction = ParameterDirection.ReturnValue;

                    cmd.Parameters.Add(pRet);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    int retVal = (int)pRet.Value;
                    string err = (string)pErr.Value;

                    if (retVal < 0)
                    {
                        throw new Exception(err);
                    }
                    return retVal;
                }
            }
        }

        public IList<Employee> Filter(string FirstName, string LastName, DateTime? HireDate, int page, int pageSize,
                               string sortField, bool sortFullTimeEmployee, out int totalCount)
        {
            IList<Employee> employees = new List<Employee>();
            using (var conn = new SqlConnection(ConnStr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    string fields =     @" [EmployeeID]
                                              ,[FirstName]
                                              ,[LastName]
                                              ,[Phone]
                                              ,[Email]
                                              ,[HireDate]
                                              ,[EmployeeImage]
                                              ,[FullTimeEmployee]";

                    string sql = @"SELECT 
                                              {0}
                                          FROM [dbo].[Employees]";

                    string whereSql =  "";
                    if (!string.IsNullOrWhiteSpace(FirstName))
                    {
                        whereSql += (whereSql.Length == 0 ? "" : " AND " ) + " FirstName like @FirstName + '%' ";
                        cmd.Parameters.AddWithValue("@FirstName", FirstName);
                    }
                    if (!string.IsNullOrWhiteSpace(LastName))
                    {
                        whereSql += (whereSql.Length == 0 ? "" : " AND ") + " LastName like @LastName + '%' ";
                        cmd.Parameters.AddWithValue("@LastName", LastName);
                    }
                    if (HireDate.HasValue)
                    {
                        whereSql += (whereSql.Length == 0 ? "" : " AND ") + " HireDate <=  @HireDate ";
                        cmd.Parameters.AddWithValue("@HireDate", HireDate);
                    }
                    if (!string.IsNullOrWhiteSpace(whereSql))
                    {
                        whereSql = " WHERE " + whereSql;
                    }

                    conn.Open();
                    cmd.CommandText = string.Format(sql, " count(*) ") + whereSql;
                    totalCount = (int) cmd.ExecuteScalar();

                    string pageSql = " OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY ";
                    cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize);

                    if(string.IsNullOrWhiteSpace(sortField))
                        sortField = "FirstName";

                    cmd.CommandText = string.Format ( sql, fields ) + whereSql 
                                + $" ORDER BY {sortField} {(sortFullTimeEmployee ? "FULLTIMEEMPLOYEE" : "ASC")}" + pageSql;

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var emp = new Employee();
                            emp.EmployeeId = rdr.GetInt32(rdr.GetOrdinal("EmployeeId"));
                            emp.FirstName = rdr.GetString(rdr.GetOrdinal("FirstName"));
                            emp.LastName = rdr.GetString(rdr.GetOrdinal("LastName"));
                            emp.Phone = rdr.GetString(rdr.GetOrdinal("Phone"));
                            emp.Email = rdr.GetString(rdr.GetOrdinal("Email"));
                            emp.HireDate = rdr.GetDateTime(rdr.GetOrdinal("HireDate"));
                            if (!(rdr.IsDBNull(rdr.GetOrdinal("EmployeeImage"))))
                            {
                                emp.EmployeeImage = (byte[])rdr["EmployeeImage"];
                            }

                            emp.FullTimeEmployee = rdr.GetBoolean(rdr.GetOrdinal("FullTimeEmployee"));

                            employees.Add(emp);
                        }
                    }
                }
            }

            return employees;
        }

        public IEnumerable<Employee> ImportFromXml(string xml)
        {
            List<Employee> employees = new List<Employee>();
            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();
                using (var cmd = new SqlCommand("InsertEmployee", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@xml", SqlDbType.Xml) { Value = xml });

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var emp = new Employee();
                            emp.EmployeeId = rdr.GetInt32(rdr.GetOrdinal("EmployeeId"));
                            emp.FirstName = rdr.GetString(rdr.GetOrdinal("FirstName"));
                            emp.LastName = rdr.GetString(rdr.GetOrdinal("LastName"));
                            emp.Phone = rdr.GetString(rdr.GetOrdinal("Phone"));
                            emp.Email = rdr.GetString(rdr.GetOrdinal("Email"));
                            emp.HireDate = rdr.GetDateTime(rdr.GetOrdinal("HireDate"));
                            if (!(rdr.IsDBNull(rdr.GetOrdinal("EmployeeImage"))))
                            {
                                emp.EmployeeImage = (byte[])rdr["EmployeeImage"];
                            }

                            emp.FullTimeEmployee = rdr.GetBoolean(rdr.GetOrdinal("FullTimeEmployee"));
                            employees.Add(emp);
                        }
                    }
                }
            }
            return employees;
        }

        public IEnumerable<Employee> ImportFromJson(string json)
        {
            throw new NotImplementedException();
        }
    }
}