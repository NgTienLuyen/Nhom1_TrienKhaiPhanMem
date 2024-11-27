using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebQuanLyNhanSu.Controllers
{
    public class PhatController : Controller
    {
        DBDataContext db = new DBDataContext();
        // GET: Phat
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PhatDiTre(int employeeID,decimal amount, string reason)
        {
            RewardsPenalty rewardsPenalty = new RewardsPenalty {
                EmployeeID = employeeID,
                Type = "Phạt",
                Amount = amount,
                Reason = reason,
                Ngay = DateTime.Now.Date
            };
            db.RewardsPenalties.InsertOnSubmit(rewardsPenalty);
            var nv = db.Salaries.SingleOrDefault(u=>u.EmployeeID==employeeID);
            nv.Phat += amount;
            db.SubmitChanges();
            Session["Message"] = "Ghi phiếu phạt thành công!";
            return RedirectToAction("BangCong","Home");
        }
        public ActionResult PhatDuAn(int projectID, decimal soTien, string lyDo)
        {
            var duan = db.PhanCongs.Where(u=>u.MaDA == projectID);
            foreach (var u in duan)
            {

                RewardsPenalty rewardsPenalty = new RewardsPenalty
                {
                    EmployeeID = u.MaNV,
                    Type = "Phạt",
                    Amount = soTien,
                    Reason = lyDo,
                    Ngay = DateTime.Now.Date
                };
                db.RewardsPenalties.InsertOnSubmit(rewardsPenalty);
                var nv = db.Salaries.SingleOrDefault(i => i.EmployeeID == rewardsPenalty.EmployeeID);
                nv.Phat += soTien;
                db.SubmitChanges();
            }
            Session["Message"] = "Ghi phiếu phạt thành công!";
            return RedirectToAction("DuAn","Home");
        }
    }
}