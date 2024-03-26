using DBSD.CW2._12882._14757._13372.DAL;
using DBSD.CW2._12882._14757._13372.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using X.PagedList;

namespace DBSD.CW2._12882._14757._13372.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index(string FirstName, string LastName, DateTime? HireDate, int? page, string sort)
        {
            int pageNumber = page ?? 1;
            int totalCount;
            int pageSize = 3;

            string sortField = "FirstName";
            bool sortFullTimeEmployee = false;
            if (!string.IsNullOrWhiteSpace(sort))
            {
                string[] arr = sort.Split('_');
                if(arr?.Length == 2)
                {
                    sortField = arr[0];
                    sortFullTimeEmployee = arr[1] == "FULLTIMEEMPLOYEE";
                }
            }

            var repository = new EmployeeRepository();
            var employees = repository.Filter(FirstName, LastName, HireDate, pageNumber, pageSize,
                sortField, sortFullTimeEmployee, out totalCount);
            var pagedList = new StaticPagedList<Employee>(employees, pageNumber, pageSize, totalCount);

            ViewBag.FirstNameSort = sort == "FirstName_ASC" ? "FirstName_FULLTIMEEMPLOYEE" : "FirstName_ASC";
            ViewBag.LastNameSort = sort == "LastName_ASC" ? "LastName_FULLTIMEEMPLOYEE" : "LastName_ASC";
            ViewBag.CurrentPage = page;
            ViewBag.CurrentSort = sort;

            return View(pagedList);
        }

        // GET: Employee/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        public ActionResult Create(Employee emp, HttpPostedFileBase imageFile)
        {
            var repository = new EmployeeRepository();
            try
            {
                if(imageFile?.ContentLength > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        imageFile.InputStream.CopyTo(stream);
                        emp.EmployeeImage = stream.ToArray();
                    }
                }

                repository.Insert(emp);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int id)
        {
            var repository = new EmployeeRepository();
            var emp = repository.GetById(id);
            return View(emp);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        public ActionResult Edit(Employee emp)
        {
            var repository = new EmployeeRepository();
            try
            {
                repository.Update(emp);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int id)
        {
            var repository = new EmployeeRepository(); 
            var employee = repository.GetById(id);
            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Employee employee)
        {
            try
            {
                var repository = new EmployeeRepository();
                repository.DeleteWithStoredProc(id);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        public FileResult ShowPhoto(int id)
        {
            var repository = new EmployeeRepository();
            var employee = repository.GetById(id);
            if (employee != null && employee.EmployeeImage?.Length > 0)
            {
                return File(employee.EmployeeImage, "image/jpeg", employee.FirstName + ".jpg");
            }
            return null;
        }
    }
}
