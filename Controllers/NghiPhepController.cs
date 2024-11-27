using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebQuanLyNhanSu.Controllers
{
    public class NghiPhepController : Controller
    {
        DBDataContext db = new DBDataContext();
        // GET: NghiPhep
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Duyet(int id)
        {
            var leave = db.Leaves.SingleOrDefault(u => u.LeaveID == id);
            if (leave != null && leave.Status == null)
            {
                leave.Status = "Đã duyệt";
                db.SubmitChanges();
                Session["Message"] = "Duyệt thành công!";
                return RedirectToAction("NghiPhep", "Home");
            }
            else
            {
                Session["Message"] = "Không thể duyệt đơn đã được duyệt";
                return RedirectToAction("NghiPhep", "Home");
            }

        }

        public ActionResult KhongDuyet(int id)
        {

            var leave = db.Leaves.SingleOrDefault(u => u.LeaveID == id);
            if (leave != null && leave.Status == null)
            {
                leave.Status = "Không duyệt";
                db.SubmitChanges();
                Session["Message"] = "Đã hủy đơn thành công!";
                return RedirectToAction("NghiPhep", "Home");
            }
            else
            {
                Session["Message"] = "Không thể hủy đơn đã được duyệt";
                return RedirectToAction("NghiPhep", "Home");
            }
        }
    }
}