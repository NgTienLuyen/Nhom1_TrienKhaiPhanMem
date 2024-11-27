using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebQuanLyNhanSu.Controllers
{
    public class DuAnController : Controller
    {
        DBDataContext db = new DBDataContext();
        // GET: DuAn
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Them()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Them(Project project)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    project.TrangThai = "Chưa hoàn thành";
                    // Thêm dự án vào cơ sở dữ liệu
                    db.Projects.InsertOnSubmit(project);
                    db.SubmitChanges();
                    int lastProjectID = project.ProjectID;

                    PhanCong pc = new PhanCong
                    {
                        MaDA = lastProjectID,
                        MaNV = project.ManagerID.Value
                    };
                    db.PhanCongs.InsertOnSubmit(pc);
                    db.SubmitChanges();
                    // Gửi thông báo thành công
                    Session["Message"] = "Dự án đã được thêm thành công!";
                }
                else
                {
                    // Nếu dữ liệu không hợp lệ, hiển thị thông báo lỗi
                    Session["Message"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại!";
                }
            }
            catch (Exception ex)
            {
                // Nếu có lỗi xảy ra, hiển thị thông báo lỗi
                Session["Message"] = "Đã xảy ra lỗi khi thêm dự án: " + ex.Message;
            }

            // Sau khi thực hiện xong, chuyển hướng về trang danh sách dự án
            return RedirectToAction("DuAn", "Home");
        }
        public ActionResult Sua(int id)
        {
            var project = db.Projects.SingleOrDefault(i=> i.ProjectID==id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sua(Project project)
        {
            if (ModelState.IsValid)
            {
                // Lấy dự án cần sửa từ cơ sở dữ liệu
                var existingProject = db.Projects.SingleOrDefault(u=> u.ProjectID == project.ProjectID);

                if (existingProject != null)
                {
                    // Cập nhật thông tin dự án
                    existingProject.ProjectName = project.ProjectName;
                    existingProject.StartDate = project.StartDate;
                    existingProject.EndDate = project.EndDate;
                    existingProject.ManagerID = project.ManagerID;

                    // Lưu các thay đổi vào cơ sở dữ liệu
                    db.SubmitChanges();

                    // Gửi thông báo thành công
                    Session["Message"] = "Dự án đã được cập nhật thành công!";
                    return RedirectToAction("DuAn", "Home");
                }
                else
                {
                    // Nếu không tìm thấy dự án, hiển thị thông báo lỗi
                    Session["Message"] = "Không tìm thấy dự án để sửa!";
                }
            }

            // Nếu dữ liệu không hợp lệ, trở lại trang sửa với thông tin đã nhập
            return View(project);
        }
        public ActionResult PhanCong()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PhanCong(PhanCong phanCong)
        {
            if (ModelState.IsValid)
            {
                // Thêm phân công dự án vào cơ sở dữ liệu
                db.PhanCongs.InsertOnSubmit(phanCong);
                db.SubmitChanges();

                // Thông báo thành công
                Session["Message"] = "Thêm phân công dự án thành công!";
            }

            // Trở lại trang danh sách phân công dự án
            return RedirectToAction("PhanCongDuAn", "Home");
        }
        public ActionResult SuaPC(int id)
        {
            // Tìm phân công dự án cần sửa từ cơ sở dữ liệu
            var phanCong = db.PhanCongs.SingleOrDefault(pc => pc.MaPC == id);
            if (phanCong == null)
            {
                // Nếu không tìm thấy phân công dự án, chuyển hướng về trang danh sách
                return RedirectToAction("PhanCongDuAn", "Home");
            }

            return View(phanCong);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaPC(PhanCong phanCong)
        {
            if (ModelState.IsValid)
            {
                // Cập nhật thông tin phân công dự án trong cơ sở dữ liệu
                var phanCongInDb = db.PhanCongs.SingleOrDefault(pc => pc.MaPC == phanCong.MaPC);
                if (phanCongInDb != null)
                {
                    phanCongInDb.MaDA = phanCong.MaDA;
                    phanCongInDb.MaNV = phanCong.MaNV;
                    db.SubmitChanges();

                    // Thông báo thành công
                    Session["Message"] = "Cập nhật phân công dự án thành công!";
                }
            }

            // Trở lại trang danh sách phân công dự án
            return RedirectToAction("PhanCongDuAn", "Home");
        }   
        public ActionResult XoaPC(int id)
        {
            var phanCong = db.PhanCongs.SingleOrDefault(pc => pc.MaPC == id);
            if (phanCong != null)
            {
                db.PhanCongs.DeleteOnSubmit(phanCong);
                db.SubmitChanges();
                Session["Message"] = "Xóa phân công dự án thành công!";
            }
            else
            {
                Session["Message"] = "Không tìm thấy phân công dự án!";
            }

            return RedirectToAction("PhanCongDuAn", "Home");
        }

        public ActionResult Xoa(int id)
        {
            // Tìm dự án cần xóa từ id
            var project = db.Projects.SingleOrDefault(u=> u.ProjectID == id);
            if (project == null)
            {
                return HttpNotFound();
            }
            var assignments = db.PhanCongs.Where(a => a.MaDA == id);
            db.PhanCongs.DeleteAllOnSubmit(assignments);
            // Xóa dự án khỏi cơ sở dữ liệu
            db.Projects.DeleteOnSubmit(project);
            db.SubmitChanges();

            // Đặt thông báo xóa thành công vào ViewBag
            Session["Message"] = "Dự án đã được xóa thành công.";

            // Chuyển hướng về trang danh sách dự án
            return RedirectToAction("DuAn","Home");
        }

    }
}