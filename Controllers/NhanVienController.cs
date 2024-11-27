using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebQuanLyNhanSu.Controllers
{
    public class NhanVienController : Controller
    {
        DBDataContext db = new DBDataContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ChamCong(int? page)
        {
            var user = Session["user"] as UserAccount;
            int maNhanVienInt = (int)user.EmployeeID;

            var attendanceList = db.Attendances
                .Where(a => a.EmployeeID == maNhanVienInt)
                .OrderBy(a => a.Ngay)
                .ToList();

            int pageSize = 7;
            int totalRecords = attendanceList.Count;

            if (totalRecords == 0)
            {
                ViewBag.Message = "Chưa có dữ liệu lịch sử chấm công trước đó.";
                return View(new List<Attendance>().ToPagedList(1, pageSize));
            }

            int lastPage = (int)Math.Ceiling((double)totalRecords / pageSize);
            int pageNumber = page ?? lastPage;

            IPagedList<Attendance> pagedAttendances = attendanceList.ToPagedList(pageNumber, pageSize);
            return View(pagedAttendances);
        }



        public ActionResult VietDon()
        {
            var user = Session["user"] as UserAccount;
            var leav = db.Leaves.Where(u=>u.EmployeeID==user.EmployeeID);
            return View(leav);
        }
        [HttpPost]
        public ActionResult VietDon(Leave leaveApplication)
        {
            if (ModelState.IsValid)
            {
                var user = Session["user"] as UserAccount;
                leaveApplication.EmployeeID = user.EmployeeID;
                // Lưu thông tin đơn xin nghỉ vào cơ sở dữ liệu
                db.Leaves.InsertOnSubmit(leaveApplication);
                db.SubmitChanges();

                // Chuyển hướng người dùng đến trang chính sau khi gửi đơn thành công
                return RedirectToAction("Index", "NhanVien");
            }
            else
            {
                // Nếu dữ liệu không hợp lệ, trả về trang viết đơn xin nghỉ với thông báo lỗi
                return View("VietDon", leaveApplication);
            }
        }
        public ActionResult XemKTKL()
        {
            var user = Session["user"] as UserAccount;
            var KTKL = db.RewardsPenalties.Where(u => u.EmployeeID == user.EmployeeID);
            return View(KTKL);
        }
        public ActionResult XemBL()
        {
            var user = Session["user"] as UserAccount;
            var bangluong = db.Salaries.SingleOrDefault(u=> u.EmployeeID == user.EmployeeID);
            return View(bangluong);
        }
        public ActionResult XemTT()
        {
            var user = Session["user"] as UserAccount;

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var thongtin = db.Employees.SingleOrDefault(u => u.EmployeeID == user.EmployeeID);
            var chedo = db.CheDos.SingleOrDefault(u => u.MaNV == user.EmployeeID);
            var phancong = db.PhanCongs.Where(u => u.MaNV == user.EmployeeID).ToList();

            if (thongtin == null)
            {
                ViewBag.ErrorMessage = "Không tìm thấy thông tin nhân viên.";
                return View();
            }

            if (chedo == null)
            {
                chedo = new CheDo
                {
                    MaCD = "Không có dữ liệu",
                    NgayCap = null
                };
            }

            ViewBag.TT = thongtin;
            ViewBag.CD = chedo;
            ViewBag.PC = phancong;

            return View();
        }


        public ActionResult DoiMK()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DoiMK(string oldPassword, string newPassword, string confirmPassword)
        {
            // Kiểm tra mật khẩu mới và xác nhận mật khẩu mới có khớp nhau không
            if (newPassword != confirmPassword)
            {
                ViewBag.ErrorMessage = "Mật khẩu mới và xác nhận mật khẩu mới không khớp nhau.";
                return View();
            }
            var user = Session["user"] as UserAccount;
            if (user != null)
            {
                var useracc = db.UserAccounts.SingleOrDefault(u=>u.UserID== user.UserID);
                if (useracc.Password != HashPassword(oldPassword))
                {
                    ViewBag.ErrorMessage = "Mật khẩu cũ không đúng";
                    return View();
                }
                useracc.Password = HashPassword(newPassword);
                db.SubmitChanges();
            }
            
            ViewBag.SuccessMessage = "Đổi mật khẩu thành công.";
            return View();
        }
        public static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}