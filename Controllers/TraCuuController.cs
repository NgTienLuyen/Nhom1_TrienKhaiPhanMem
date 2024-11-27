using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebQuanLyNhanSu.Controllers
{
    public class TraCuuController : Controller
    {
        DBDataContext db = new DBDataContext();
        // GET: TraCuu
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TraCuuThongTin(string strSearch)
        {
            ViewBag.Search = strSearch;

            var employees = db.Employees.ToList();

            if (!string.IsNullOrEmpty(strSearch))
            {
                employees = db.Employees.Where(s =>
                    s.FirstName.Contains(strSearch) ||
                    s.LastName.Contains(strSearch) ||
                    s.PhoneNumber.Contains(strSearch) ||
                    s.Address.Contains(strSearch) ||
                    s.Department.DepartmentName.Contains(strSearch) ||
                    s.Projects.Any(p => p.ProjectName.Contains(strSearch)) ||
                    s.UserAccounts.Any(u => u.Username.Contains(strSearch)))
                    .OrderBy(s => s.LastName)
                    .ThenBy(s => s.FirstName)
                    .ToList();
            }

            return View(employees);
        }
    }
}