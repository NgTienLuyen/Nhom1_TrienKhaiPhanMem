using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebQuanLyNhanSu.Controllers
{
    public class CheDoController : Controller
    {
        DBDataContext db = new DBDataContext();

        // GET: CheDo
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
        public ActionResult Them(CheDo cheDo)
        {
            if (ModelState.IsValid)
            {
                // Thêm đối tượng chế độ vào cơ sở dữ liệu
                db.CheDos.InsertOnSubmit(cheDo);
             
                Session["Message"] = "Thêm mới thành công!";
                var luong = db.Salaries.SingleOrDefault(u=>u.EmployeeID == cheDo.MaNV);
                luong.Deductions = (decimal)10.5;
                db.SubmitChanges();
                // Chuyển hướng đến trang danh sách chế độ sau khi thêm thành công
                return RedirectToAction("QLCheDo","Home");
            }

            // Nếu dữ liệu không hợp lệ, trở lại trang thêm mới với thông tin đã nhập
            return View(cheDo);
        }
        public ActionResult Sua(string id)
        {
            // Tìm chế độ cần sửa trong cơ sở dữ liệu
            var cheDo = db.CheDos.FirstOrDefault(c => c.MaCD == id);

            if (cheDo == null)
            {
                return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy chế độ
            }

            return View(cheDo); // Trả về view để chỉnh sửa thông tin chế độ
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sua(CheDo cheDo)
        {
            if (ModelState.IsValid)
            {
                // Tìm chế độ cần sửa trong cơ sở dữ liệu
                var cheDoToUpdate = db.CheDos.FirstOrDefault(c => c.MaCD == cheDo.MaCD);

                if (cheDoToUpdate != null)
                {
                    // Cập nhật thông tin chế độ
                    cheDoToUpdate.MaNV = cheDo.MaNV;
                    cheDoToUpdate.NoiCap = cheDo.NoiCap;
                    cheDoToUpdate.NgayCap = cheDo.NgayCap;
                    cheDoToUpdate.Note = cheDo.Note;

                    // Lưu các thay đổi vào cơ sở dữ liệu
                    db.SubmitChanges();
                    Session["Message"] = "Sửa thành công!";
                    // Chuyển hướng đến trang danh sách chế độ sau khi cập nhật thành công
                    return RedirectToAction("QLCheDo", "Home");
                }
                else
                {
                    return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy chế độ
                }
            }

            // Nếu dữ liệu không hợp lệ, trở lại trang sửa chế độ với thông tin đã nhập
            return View(cheDo);
        }
        public ActionResult Xoa(string id)
        {
            var cheDoToDelete = db.CheDos.FirstOrDefault(c => c.MaCD == id);
            if (cheDoToDelete != null)
            {
                // Xóa chế độ khỏi cơ sở dữ liệu
                db.CheDos.DeleteOnSubmit(cheDoToDelete);
                db.SubmitChanges();
                Session["Message"] = "Xóa thành công!";
                return RedirectToAction("QLCheDo", "Home");
            }
            else
            {
                return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy chế độ
            }
        }

    }
}