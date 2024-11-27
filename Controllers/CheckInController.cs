using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebQuanLyNhanSu.Controllers
{
    public class CheckInController : Controller
    {
        DBDataContext db = new DBDataContext();

        // Trang Index (Check-in)
        public ActionResult Index()
        {
            return View();
        }

        // Xử lý Check-in
        [HttpPost]
        public ActionResult Index(int employeeID)
        {
            var checkIn = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
            var ngay = DateTime.Now.Date;

            // Kiểm tra giờ check-in so với 7h30
            string diTre = checkIn > TimeSpan.FromHours(7) ? "Có" : "Không";

            // Tạo đối tượng Attendance mới
            var newAttendance = new Attendance
            {
                EmployeeID = employeeID,
                CheckIn = checkIn,
                Ngay = ngay.Date, // Lấy ngày từ datetime
                DiTre = diTre
            };

            db.Attendances.InsertOnSubmit(newAttendance);
            db.SubmitChanges();

            // Sau khi thêm mới, chuyển hướng về trang chính
            return RedirectToAction("Index");
        }

        // Trang Checkout (Check-out)
        public ActionResult Checkout()
        {
            return View();
        }

        // Xử lý Check-out
        [HttpPost]
        public ActionResult Checkout(int employeeID)
        {
            var checkout = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
            var ngay = DateTime.Now.Date;

            // Kiểm tra giờ checkout so với 16h00
            string vesom = checkout < TimeSpan.FromHours(16) ? "Có" : "Không";

            // Tìm kiếm Attendance của nhân viên theo EmployeeID và ngày hiện tại
            var dd = db.Attendances.SingleOrDefault(u => u.EmployeeID == employeeID && u.Ngay.Value.Date == ngay);

            if (dd == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy, trả về lỗi
            }

            dd.CheckOut = checkout;
            var time = (int?)((checkout - dd.CheckIn.Value).TotalHours - 1);

            int sogiolam = 0;
            int sogiolamle = 0;

            // Kiểm tra nếu ngày hôm nay có là ngày lễ không
            var le = db.NgayLes.ToList();
            foreach (var ngayle in le)
            {
                if (ngayle.NgayBatDau <= ngay && ngay <= ngayle.NgayKetThuc)
                {
                    sogiolamle += ngayle.HeSo * (int)((checkout - dd.CheckIn.Value).TotalHours - 1);
                }
            }

            // Tính số giờ làm và xử lý theo các trường hợp
            if (time < 0 && sogiolamle == 0)
            {
                dd.Timer = 0;
            }
            else if (time < 8 && sogiolamle == 0)
            {
                sogiolam += (int)time;
                dd.Timer = time;
            }
            else if (sogiolamle == 0)
            {
                sogiolam += 8;
                dd.Timer = 8;
            }

            int ot = 0;
            // Kiểm tra có làm thêm giờ (OT) hay không
            if (vesom == "Không" && sogiolamle == 0)
            {
                dd.OT = (int?)((checkout - TimeSpan.FromHours(16)).TotalHours);
                ot += (int)dd.OT;
            }
            else
            {
                dd.OT = 0;
            }

            dd.VeSom = vesom;

            // Cập nhật thông tin lương của nhân viên
            var sl = db.Salaries.SingleOrDefault(u => u.EmployeeID == employeeID);

            if (sl != null)
            {
                sl.SoGioLam += sogiolam;
                sl.OT += ot;
                sl.GioNgayLe += sogiolamle;

                if (sogiolamle == 0)
                {
                    sl.TongSoGioLam += sl.OT + sl.SoGioLam;
                }
                else
                {
                    sl.TongSoGioLam += sogiolamle;
                }
            }
            else
            {
                // Nếu không có bản ghi lương, tạo mới bản ghi Salary
                sl = new Salary
                {
                    EmployeeID = employeeID,
                    SoGioLam = sogiolam,
                    OT = ot,
                    GioNgayLe = sogiolamle,
                    TongSoGioLam = sogiolamle == 0 ? (sogiolam + ot) : sogiolamle
                };
                db.Salaries.InsertOnSubmit(sl);
            }

            db.SubmitChanges();

            // Sau khi cập nhật lương và chấm công, chuyển hướng về trang checkout
            return RedirectToAction("Checkout");
        }
    }
}
