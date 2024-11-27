using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebQuanLyNhanSu.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        DBDataContext db = new DBDataContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Gioithieu()
        {
            return View();
        }
        public ActionResult DangKi()
        {
            return View();
        }
        public ActionResult QLCheDo()
        {
            var message = Session["Message"];
            ViewBag.Message = message;
            Session.Remove("Message");
            return View(db.CheDos.ToList());
        }
        public ActionResult QLNV()
        {
            var message = Session["Message"];
            ViewBag.Message = message;
            Session.Remove("Message");
            var nv = db.Employees.ToList();
            return View(nv);
        }
        public ActionResult GuiMail()
        {
            var message = Session["Message"];
            ViewBag.Message = message;
            Session.Remove("Message");
            return View();
        }
        public ActionResult PhongBan()
        {
            var message = Session["Message"];
            ViewBag.Message = message;
            Session.Remove("Message");
            return View(db.Departments.ToList());
        }
        public ActionResult NgayLe()
        {
            var message = Session["Message"];
            ViewBag.Message = message;
            Session.Remove("Message");
            return View(db.NgayLes.ToList());
        }
        public ActionResult BangCong()
        {
            var message = Session["Message"];
            ViewBag.Message = message;
            Session.Remove("Message");

            // Lấy danh sách các attendance trong tháng hiện tại
            DateTime today = DateTime.Today;
            var attendances = db.Attendances.Where(a => a.Ngay.Value.Month == today.Month && a.Ngay.Value.Year == today.Year).ToList();

            // Dictionary để lưu số lần đi trễ, số lần về sớm và tên của từng nhân viên
            Dictionary<int, Tuple<string, int, int>> soLanDiTreVaVeSom = new Dictionary<int, Tuple<string, int, int>>();

            foreach (var attendance in attendances)
            {
                // Lấy thông tin tên của nhân viên từ cơ sở dữ liệu
                string tenNhanVien = db.Employees.FirstOrDefault(e => e.EmployeeID == attendance.EmployeeID)?.LastName;

                // Kiểm tra nếu không tìm thấy tên hoặc không đi trễ và không về sớm, bỏ qua
                if (string.IsNullOrEmpty(tenNhanVien) || attendance.DiTre == "Không" && attendance.VeSom == "Không")
                    continue;

                // thực hiện các thông t
                int employeeID = attendance.EmployeeID ?? 0;
                int soLanDiTre = attendance.DiTre == "Có" ? 1 : 0;
                int soLanVeSom = attendance.VeSom == "Có" ? 1 : 0;

                // Nếu nhân viên đã có trong dictionary, tăng số lần đi trễ hoặc số lần về sớm lên tương ứng
                if (soLanDiTreVaVeSom.ContainsKey(employeeID))
                {
                    var tuple = soLanDiTreVaVeSom[employeeID];
                    soLanDiTreVaVeSom[employeeID] = Tuple.Create(tuple.Item1, tuple.Item2 + soLanDiTre, tuple.Item3 + soLanVeSom);
                }
                else // Nếu không, thêm vào dictionary và set số lần đi trễ và số lần về sớm
                {
                    soLanDiTreVaVeSom.Add(employeeID, Tuple.Create(tenNhanVien, soLanDiTre, soLanVeSom));
                }
            }

            // Lưu dictionary vào ViewBag
            ViewBag.SoLanDiTreVaVeSom = soLanDiTreVaVeSom;

            return View(attendances);
        }

        public ActionResult KTKL()
        {
            // Lấy danh sách nhân viên từ database
            var employees = db.RewardsPenalties.ToList();
            var nv = db.Employees.ToList();
            ViewBag.NV = nv;

            // Kiểm tra nếu danh sách nhân viên là null
            if (employees == null)
            {
                // Xử lý trường hợp danh sách nhân viên là null
                // Trong trường hợp này, ta tạo ra một danh sách nhân viên mới, trống
                employees = new List<WebQuanLyNhanSu.RewardsPenalty>();
            }
            ViewBag.Employees = employees;

            // Trả về View với danh sách nhân viên
            return View(employees);
        }
        public ActionResult BangLuong()
        {
            return View(db.Salaries.ToList());
        }
        public ActionResult TDBangLuong()
        {
            return View();
        }
        public ActionResult TinhLuong()
        {
            var sl = db.Salaries.ToList();
            return View(sl);
        }
        public ActionResult DuAn()
        {
            var message = Session["Message"];
            ViewBag.Message = message;
            Session.Remove("Message");
            var duan = db.Projects.ToList();
            ViewBag.chuaht = duan.Where(u=> u.EndDate < DateTime.Now.Date && u.TrangThai=="Chưa hoàn thành");
            return View(duan);
        }
        public ActionResult PhanCongDuAn()
        {
            var message = Session["Message"];
            ViewBag.Message = message;
            Session.Remove("Message");
            return View(db.PhanCongs.ToList());
        }
        public ActionResult NghiPhep()
        {
            var message = Session["Message"];
            ViewBag.Message = message;
            Session.Remove("Message");
            var leaves = db.Leaves.ToList();
            return View(leaves);
        }
        public ActionResult BaoCaoNhanSu()
        {
            return View();
        }
        public ActionResult BaoCaoLuong()
        {
            return View();
        }
        public ActionResult TraCuu()
        {
            return View(db.Employees.ToList());
        }
        public ActionResult HeThong()
        {
            var message = Session["Message"];
            ViewBag.Message = message;
            Session.Remove("Message");
            return View(db.UserAccounts.ToList());
        }
        public ActionResult DangNhap()
        {
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
        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("DangNhap", "Home");
        }
        [HttpPost]
        public ActionResult ClearExpiredOtp(int? otp)
        {
            var user = Session["user"] as UserAccount;
            if (user != null && user.OTP == otp)
            {
                user.OTP = null;
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ErrorMessage = "OTP khong dung hoac da het han!";
                return View("DangNhap");
            }
        }

        [HttpPost]
        public ActionResult DangNhap(string username, string password)
        {
            // Kiểm tra thông tin đăng nhập
            var user = db.UserAccounts.FirstOrDefault(u => u.Username == username && u.Password == HashPassword(password));
            if (user != null)
            {
                // Lấy thông tin nhân viên tương ứng
                var nguoi = db.Employees.FirstOrDefault(u => u.EmployeeID == user.EmployeeID);

                // Lưu thông tin người dùng và chức vụ vào session
                Session["user"] = user;
                Session["name"] = nguoi.FirstName;
                Session["ChucVu"] = nguoi.ChucVu;

                // Điều hướng trực tiếp theo chức vụ
                if (nguoi.ChucVu == "Nhanvien")
                {
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "NhanVien") });
                }
                else
                {
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
                }
            }
            else
            {
                // Trả về JSON khi đăng nhập thất bại
                return Json(new { success = false, message = "Tên đăng nhập hoặc mật khẩu không đúng." });
            }
        }


        [HttpPost]
        public ActionResult XacThucOTP(int otp)
        {
            var user = Session["user"] as UserAccount;
            if (user != null && user.OTP == otp)
            {
                // Xóa mã OTP sau khi xác thực thành công
                user.OTP = null;
                db.SubmitChanges();

                // Trả về quyền người dùng sau khi OTP được xác thực
                return Json(new { success = true, ChucVu = Session["ChucVu"] });
            }
            else
            {
                // Trả về lỗi nếu mã OTP không hợp lệ
                return Json(new { success = false, message = "Mã OTP không đúng hoặc đã hết hạn." });
            }
        }
        [HttpPost]
        public ActionResult DangKi(string username, string password, string email)
        {
            // Kiểm tra người dùng đã tồn tại
            if (db.UserAccounts.Any(u => u.Username == username))
            {
                return Json(new { success = false, message = "Tên người dùng hoặc email đã tồn tại." });
            }

            // Tạo tài khoản mới
            var newUser = new UserAccount
            {
                Username = username,
                Password = HashPassword(password),
                
            };

            // Tạo OTP và lưu vào database
            var otp = GenerateOTP();
            newUser.OTP = otp;
            db.UserAccounts.InsertOnSubmit(newUser);
            db.SubmitChanges();

            // Gửi OTP qua email
            SendOTPToEmail(email, otp);

            // Lưu user vào session tạm thời
            Session["newUser"] = newUser;

            return Json(new { success = true, message = "OTP đã được gửi đến email của bạn." });
        }

        [HttpPost]
        public ActionResult XacNhanOTPdangki(int otp)
        {
            var newUser = Session["newUser"] as UserAccount;
            if (newUser != null && newUser.OTP == otp)
            {
                newUser.OTP = null;  // Xóa OTP sau khi xác thực
                db.SubmitChanges();
                Session.Remove("newUser");  // Xóa session tạm thời

                return Json(new { success = true, message = "Đăng ký thành công!" });
            }
            else
            {
                return Json(new { success = false, message = "OTP không đúng hoặc đã hết hạn." });
            }
        }

        private int GenerateOTP()
        {
            // Lấy ngày giờ hiện tại
            DateTime now = DateTime.Now;

            // Tạo một seed từ ngày giờ hiện tại
            long seed = now.Ticks;

            // Tạo một đối tượng Random mới với seed từ ngày giờ hiện tại
            Random random = new Random((int)seed);

            // Tạo số ngẫu nhiên trong khoảng từ 100000 đến 999999
            int otp = random.Next(100000, 999999);

            // Trả về mã OTP được tạo
            return otp;
        }

        private void SendOTPToEmail(string email, int otp)
        {
            try
            {
                string emailSubject = "Mã OTP của bạn";
                string emailContent = $"Mã OTP của bạn là: {otp}. Mã này có hiệu lực trong 5 phút.";

                MailMessage mail = new MailMessage("tienluyen008@gmail.com", email);
                mail.Subject = emailSubject;
                mail.Body = emailContent;
                mail.IsBodyHtml = true;

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential("tienluyen008@gmail.com", "aylb jyda xvem fcxl");
                smtpClient.EnableSsl = true;

                smtpClient.Send(mail);

                Session["Message"] = "OTP đã được gửi thành công!";
            }
            catch (Exception ex)
            {
                Session["Error"] = "Đã xảy ra lỗi khi gửi OTP: " + ex.Message;
            }
        }


    }
}