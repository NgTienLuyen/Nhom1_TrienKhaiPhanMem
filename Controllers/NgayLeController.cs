using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebQuanLyNhanSu.Controllers
{
    public class NgayLeController : Controller
    {
        DBDataContext db = new DBDataContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Them()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Them(NgayLe ngayLe)
        {
            if (ModelState.IsValid)
            {
                // Thêm ngày lễ vào cơ sở dữ liệu và lưu thay đổi
                db.NgayLes.InsertOnSubmit(ngayLe);
                db.SubmitChanges();
                Session["Message"] = "Thêm thành công";
                return RedirectToAction("NgayLe","Home");
            }

            // Nếu có lỗi, hiển thị lại form thêm mới
            return View(ngayLe);
        }
        public ActionResult Sua(int id)
        {
            var ngayLe = db.NgayLes.SingleOrDefault(u=>u.MaNgay == id);
            if (ngayLe == null)
            {
                return HttpNotFound();
            }
            return View(ngayLe);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sua(NgayLe ngayLe)
        {
            if (ModelState.IsValid)
            {
                var ngayLeInDB = db.NgayLes.FirstOrDefault(n => n.MaNgay == ngayLe.MaNgay);
                if (ngayLeInDB != null)
                {
                    ngayLeInDB.TenNgayLe = ngayLe.TenNgayLe;
                    ngayLeInDB.NgayBatDau = ngayLe.NgayBatDau;
                    ngayLeInDB.NgayKetThuc = ngayLe.NgayKetThuc;
                    ngayLeInDB.HeSo = ngayLe.HeSo;

                    db.SubmitChanges();
                    Session["Message"] = "Sửa thành công";
                    return RedirectToAction("NgayLe","Home");
                }
                else
                {
                    return HttpNotFound();
                }
            }
            return View(ngayLe);
        }
        public ActionResult Xoa(int id)
        {
            // Tìm ngày lễ cần xóa từ id
            var ngayLe = db.NgayLes.SingleOrDefault(u=> u.MaNgay == id);
            if (ngayLe == null)
            {
                return HttpNotFound();
            }

            // Xóa ngày lễ khỏi cơ sở dữ liệu
            db.NgayLes.DeleteOnSubmit(ngayLe);
            db.SubmitChanges();

            // Đặt thông báo xóa thành công vào ViewBag
            Session["Message"] = "Ngày lễ đã được xóa thành công.";

            // Chuyển hướng về trang danh sách ngày lễ
            return RedirectToAction("NgayLe","Home");
        }


    }
}