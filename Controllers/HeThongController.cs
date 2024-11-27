using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebQuanLyNhanSu.Controllers
{
    public class HeThongController : Controller
    {
        DBDataContext db = new DBDataContext();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Xoa(int id)
        {
            // Tìm người dùng cần xóa từ ID
            var user = db.UserAccounts.FirstOrDefault(u => u.UserID == id);
            if (user == null)
            {
                // Trả về HTTP 404 nếu không tìm thấy người dùng
                return HttpNotFound();
            }

            // Xóa người dùng khỏi cơ sở dữ liệu
            db.UserAccounts.DeleteOnSubmit(user);
            db.SubmitChanges();
            Session["Message"] = "Xóa thành công!";
            // Trả về kết quả thành công
            return Json(new { success = true });
        }
        public ActionResult Them()
        {
            // Thực hiện bất kỳ logic nào cần thiết ở đây

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
        [HttpPost]
        public ActionResult Them(UserAccount user)
        {
            if (ModelState.IsValid)
            {
                string pass = user.Password;
                user.Password = HashPassword(pass);
                    db.UserAccounts.InsertOnSubmit(user);
                    db.SubmitChanges();
                Session["Message"] = "Thêm mới thành công!";
                // Sau khi thêm mới thành công, chuyển hướng về trang danh sách người dùng hệ thống
                return RedirectToAction("HeThong", "Home");
            }

            // Nếu ModelState không hợp lệ, hiển thị lại form thêm mới với các thông báo lỗi
            return View(user);
        }

    }

}