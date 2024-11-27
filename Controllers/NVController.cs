using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace WebQuanLyNhanSu.Controllers
{
    public class NVController : Controller
    {
        DBDataContext db = new DBDataContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetEmployeeDetail(int id)
        {
            var employee = db.Employees.SingleOrDefault(u => u.EmployeeID == id);
            if (employee == null)
            {
                return HttpNotFound();
            }

            var employeeDetail = new Employee
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                BirthDate = employee.BirthDate,
                Address = employee.Address,
                PhoneNumber = employee.PhoneNumber,
                Email = employee.Email,
                CCCD = employee.CCCD,
                NoiCap = employee.NoiCap,
                NgayDK = employee.NgayDK,
                NgayKi = employee.NgayKi,
                NgayHetHan = employee.NgayHetHan,
                ChucVu = employee.ChucVu,
                GioiTinh = employee.GioiTinh,
                DanToc = employee.DanToc,
                HocVan = employee.HocVan,
                DepartmentID = employee.DepartmentID,
                TrangThai = employee.TrangThai
            };

            return Json(employeeDetail, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            // Tìm người dùng cần xóa từ ID
            var user = db.Employees.FirstOrDefault(u => u.EmployeeID == id);
            if (user == null)
            {
                // Trả về HTTP 404 nếu không tìm thấy người dùng
                return HttpNotFound();
            }

            // Xóa người dùng khỏi cơ sở dữ liệu
            db.Employees.DeleteOnSubmit(user);
            db.SubmitChanges();

            Session["Message"] = "Xóa nhân viên đã thành công!";
            return Json(new { success = true });
        }
       
        public ActionResult Edit(int id)
        {
            var employee = db.Employees.FirstOrDefault(e => e.EmployeeID == id);
            if (employee == null)
            {
                return HttpNotFound();
            }

            return View(employee);
        }

        // POST: /Employee/Edit/{id}
        [HttpPost]
        public ActionResult Edit(int id, Employee updatedEmployee)
        {
            if (!ModelState.IsValid)
            {
                return View(updatedEmployee);
            }

            try
            {
                var employeeToUpdate = db.Employees.FirstOrDefault(e => e.EmployeeID == id);
                if (employeeToUpdate == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật thông tin từ updatedEmployee
                employeeToUpdate.FirstName = updatedEmployee.FirstName;
                employeeToUpdate.LastName = updatedEmployee.LastName;
                employeeToUpdate.BirthDate = updatedEmployee.BirthDate;
                employeeToUpdate.Address = updatedEmployee.Address;
                employeeToUpdate.PhoneNumber = updatedEmployee.PhoneNumber;
                employeeToUpdate.Email = updatedEmployee.Email;
                employeeToUpdate.CCCD = updatedEmployee.CCCD;
                employeeToUpdate.NoiCap = updatedEmployee.NoiCap;
                employeeToUpdate.NgayDK = updatedEmployee.NgayDK;
                employeeToUpdate.NgayKi = updatedEmployee.NgayKi;
                employeeToUpdate.NgayHetHan = updatedEmployee.NgayHetHan;
                employeeToUpdate.ChucVu = updatedEmployee.ChucVu;
                employeeToUpdate.GioiTinh = updatedEmployee.GioiTinh;
                employeeToUpdate.DanToc = updatedEmployee.DanToc;
                employeeToUpdate.HocVan = updatedEmployee.HocVan;
                employeeToUpdate.DepartmentID = updatedEmployee.DepartmentID;
                employeeToUpdate.TrangThai = updatedEmployee.TrangThai;

                db.SubmitChanges();
                Session["Message"] = "Sửa thành công!";
                return RedirectToAction("QLNV", "Home"); // Hoặc chuyển hướng đến trang chi tiết nhân viên
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật nhân viên: " + ex.Message);
                return View(updatedEmployee);
            }
        }
        public ActionResult ThemNhanVien()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemNhanVien(FormCollection form)
        {
            try
            {
                var hoTenDem = form["firstName"];
                var ten = form["lastName"];
                var ngaySinh = form["birthDate"] != null ? DateTime.Parse(form["birthDate"]) : DateTime.Now;
                var diaChi = form["address"];
                var soDienThoai = form["phoneNumber"];
                var email = form["email"];
                var cccd = form["CCCD"];
                var noiCap = form["NoiCap"];
                var ngayDK = form["NgayDK"] != null ? DateTime.Parse(form["NgayDK"]) : DateTime.Now;
                var ngayKi = form["NgayKi"] != null ? DateTime.Parse(form["NgayKi"]) : DateTime.Now;
                var ngayHetHan = form["NgayHetHan"] != null ? DateTime.Parse(form["NgayHetHan"]) : DateTime.Now;
                var chucVu = form["ChucVu"];
                var gioiTinh = form["GioiTinh"];
                var danToc = form["DanToc"];
                var hocVan = form["HocVan"];
                var departmentID = int.Parse(form["DepartmentID"]);
                var trangThai = form["TrangThai"];

                // Validate Email
                if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    ModelState.AddModelError("", "Email không đúng định dạng.");
                    return View();
                }

                // Validate CCCD
                if (!Regex.IsMatch(cccd, @"^\d{12}$"))
                {
                    ModelState.AddModelError("", "Số CCCD phải là 12 chữ số.");
                    return View();
                }

                // Validate PhoneNumber
                if (!Regex.IsMatch(soDienThoai, @"^\d{10}$"))
                {
                    ModelState.AddModelError("", "Số điện thoại phải là 10 chữ số.");
                    return View();
                }

                // Validate Chức Vụ
                if (!new[] { "Quanli", "Nhanvien", "Ketoan" }.Contains(chucVu))
                {
                    ModelState.AddModelError("", "Chức vụ không hợp lệ.");
                    return View();
                }

                // Validate Giới Tính
                if (!new[] { "Nam", "Nữ" }.Contains(gioiTinh))
                {
                    ModelState.AddModelError("", "Giới tính không hợp lệ.");
                    return View();
                }

                // Validate Trạng Thái
                if (!new[] { "Đang làm", "Chưa" }.Contains(trangThai))
                {
                    ModelState.AddModelError("", "Trạng thái không hợp lệ.");
                    return View();
                }

                var newEmployee = new Employee
                {
                    FirstName = hoTenDem,
                    LastName = ten,
                    BirthDate = ngaySinh,
                    Address = diaChi,
                    PhoneNumber = soDienThoai,
                    Email = email,
                    CCCD = cccd,
                    NoiCap = noiCap,
                    NgayDK = ngayDK,
                    NgayKi = ngayKi,
                    NgayHetHan = ngayHetHan,
                    ChucVu = chucVu,
                    GioiTinh = gioiTinh,
                    DanToc = danToc,
                    HocVan = hocVan,
                    DepartmentID = departmentID,
                    TrangThai = trangThai
                };

                db.Employees.InsertOnSubmit(newEmployee);
                db.SubmitChanges();
                Session["Message"] = "Thêm mới thành công!";
                return RedirectToAction("QLNV", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
                return View();
            }
        }

    }
}