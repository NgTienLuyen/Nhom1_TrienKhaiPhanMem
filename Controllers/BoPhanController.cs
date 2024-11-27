using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebQuanLyNhanSu.Controllers
{
    public class BoPhanController : Controller
    {
        DBDataContext db = new DBDataContext();
        // GET: BoPhan
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Them()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Them(Department department)
        {
            if (ModelState.IsValid)
            {
                // Thêm bộ phận vào cơ sở dữ liệu
                db.Departments.InsertOnSubmit(department);
                db.SubmitChanges();
                Session["Message"] = "Thêm mới thành công!";
                return RedirectToAction("PhongBan", "Home");
            }
            return View(department);
        }
        public ActionResult Sua(int id)
        {
            // Lấy thông tin phòng ban từ cơ sở dữ liệu dựa trên id
            var department = db.Departments.SingleOrDefault(u=>u.DepartmentID==id);
            if (department == null)
            {
                return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy phòng ban
            }

            return View(department); // Trả về view sửa với dữ liệu của phòng ban cần sửa
        }
        [HttpPost]
        public ActionResult LuuSua(Department department)
        {
            if (ModelState.IsValid)
            {
                // Tìm phòng ban cần cập nhật trong cơ sở dữ liệu dựa trên ID
                var existingDepartment = db.Departments.FirstOrDefault(d => d.DepartmentID == department.DepartmentID);

                if (existingDepartment != null)
                {
                    // Cập nhật thông tin phòng ban từ dữ liệu được gửi từ form
                    existingDepartment.DepartmentName = department.DepartmentName;
                    existingDepartment.TrangThai = department.TrangThai;
                    existingDepartment.NgayThanhLap = department.NgayThanhLap;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.SubmitChanges();
                    Session["Message"] = "Sửa thành công!";
                    return RedirectToAction("PhongBan","Home"); // Chuyển hướng về trang danh sách phòng ban sau khi cập nhật thành công
                }
                else
                {
                    // Nếu không tìm thấy phòng ban cần cập nhật, hiển thị thông báo lỗi
                    ModelState.AddModelError("", "Không tìm thấy phòng ban cần cập nhật.");
                }
            }

            // Nếu dữ liệu không hợp lệ hoặc không tìm thấy phòng ban, hiển thị lại trang sửa với thông báo lỗi
            return View("Sua", department);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhatTrangThai(int departmentID, string status)
        {
            // Tìm phòng ban trong cơ sở dữ liệu
            var department = db.Departments.FirstOrDefault(d => d.DepartmentID == departmentID);

            // Kiểm tra xem phòng ban có tồn tại không
            if (department != null)
            {
                // Cập nhật trạng thái của phòng ban
                department.TrangThai = status;

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SubmitChanges();
                Session["Message"] = status + " thành công!";
                // Chuyển hướng đến trang danh sách phòng ban
                return RedirectToAction("PhongBan","Home");
            }

            // Trả về trang lỗi hoặc xử lý khác nếu cần
            return View("Error");
        }


    }
}