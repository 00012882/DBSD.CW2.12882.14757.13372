using DBSD.CW2._12882._14757._13372.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSD.CW2._12882._14757._13372.DAL
{
    public interface IEmployeeRepository
    {
        IList<Employee> GetAll();
        void Insert(Employee emp);
        Employee GetById(int id);
        void Update(Employee emp);
    }
}
