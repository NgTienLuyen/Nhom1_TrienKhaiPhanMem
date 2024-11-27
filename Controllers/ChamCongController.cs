using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQuanLyNhanSu;

namespace WebQuanLyNhanSu.Controllers
{
    public class ChamCongController : Controller
    {
        // Tạo một instance của DbContext (hoặc DBDataContext nếu bạn dùng LINQ to SQL)
        DBDataContext db = new DBDataContext();

        // Danh sách chấm công
        public ActionResult Index()
        {
            // Lấy danh sách chấm công từ bảng Attendances
            var chamCongList = db.Attendances.ToList();

            // Kiểm tra xem danh sách có null không, mặc dù với ToList() sẽ không null
            if (chamCongList == null)
            {
                chamCongList = new List<Attendance>(); // Đảm bảo danh sách không null
            }

            return View(chamCongList); // Truyền danh sách vào view
        }


        // Hiển thị form thêm mới chấm công
        public ActionResult Them()
        {
            return View();
        }

        // Xử lý thêm mới chấm công
        [HttpPost]
        public ActionResult Them(Attendance chamCong)
        {
            if (ModelState.IsValid)
            {
                // Chuyển đổi EmployeeId từ session sang int
                var employeeIdStr = HttpContext.Session["EmployeeId"]?.ToString();

                if (string.IsNullOrEmpty(employeeIdStr) || !int.TryParse(employeeIdStr, out int employeeId))
                {
                    return RedirectToAction("Login", "Account");
                }

                chamCong.EmployeeID = employeeId; // Gán EmployeeId

                // Thêm dữ liệu chấm công mới vào bảng Attendances
                db.Attendances.InsertOnSubmit(chamCong);
                db.SubmitChanges();

                // Thông báo thành công và chuyển hướng về trang danh sách
                TempData["Message"] = "Thêm chấm công thành công.";
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, hiển thị lại form
            return View(chamCong);
        }

        // Hiển thị form chỉnh sửa chấm công
        public ActionResult Sua(int id)
        {
            // Lấy thông tin chấm công theo id
            var chamCong = db.Attendances.SingleOrDefault(c => c.AttendanceID == id);

            // Nếu không tìm thấy, trả về lỗi
            if (chamCong == null)
            {
                return HttpNotFound();
            }

            return View(chamCong); // Truyền thông tin chấm công vào view
        }

        // Xử lý chỉnh sửa chấm công
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sua(Attendance chamCong)
        {
            if (ModelState.IsValid)
            {
                // Tìm chấm công cần sửa
                var chamCongInDB = db.Attendances.FirstOrDefault(c => c.AttendanceID == chamCong.AttendanceID);

                if (chamCongInDB != null)
                {
                    // Cập nhật các trường cần thiết
                    chamCongInDB.Ngay = chamCong.Ngay;
                    chamCongInDB.CheckIn = chamCong.CheckIn;
                    chamCongInDB.CheckOut = chamCong.CheckOut;
                    chamCongInDB.DiTre = chamCong.DiTre;
                    chamCongInDB.VeSom = chamCong.VeSom;

                    // Lưu thay đổi
                    db.SubmitChanges();

                    // Thông báo thành công và chuyển hướng về trang danh sách
                    TempData["Message"] = "Chỉnh sửa chấm công thành công.";
                    return RedirectToAction("Index");
                }
                else
                {
                    return HttpNotFound();
                }
            }

            // Nếu có lỗi, hiển thị lại form
            return View(chamCong);
        }

        // Xóa chấm công
        public ActionResult Xoa(int id)
        {
            // Tìm chấm công cần xóa
            var chamCong = db.Attendances.SingleOrDefault(c => c.AttendanceID == id);

            // Nếu không tìm thấy, trả về lỗi
            if (chamCong == null)
            {
                return HttpNotFound();
            }

            // Xóa chấm công khỏi cơ sở dữ liệu
            db.Attendances.DeleteOnSubmit(chamCong);
            db.SubmitChanges();

            // Thông báo thành công và chuyển hướng về trang danh sách
            TempData["Message"] = "Xóa chấm công thành công.";
            return RedirectToAction("Index");
        }
    }
}
