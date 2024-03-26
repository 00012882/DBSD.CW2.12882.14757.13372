using DBSD.CW2._12882._14757._13372.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSD.CW2._12882._14757._13372.DAL
{
    public interface IEmployeeRepository
    {
        IList<Employee> GetAll();

        IList<Employee> Filter(string FirstName, string LastName, DateTime? HireDate, int page, int pageSize,
                                string sortField, bool sortFullTimeEmployee, out int totalCount);

        void Insert(Employee emp);
        Employee GetById(int id);
        void Update(Employee emp);
    }
}
