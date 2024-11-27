using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace WebQuanLyNhanSu.Controllers
{
    public class MailController : Controller
    {
        DBDataContext db = new DBDataContext();
        // GET: Mail
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GuiMail(int id, string emailSubject, string emailContent)
        {
            try
            {
                var nv = db.Employees.SingleOrDefault(u=>u.EmployeeID == id);
                string recipientEmail = nv.Email;
                // Tạo đối tượng MailMessage với thông tin người gửi và người nhận
                MailMessage mail = new MailMessage("tienluyen008@gmail.com", recipientEmail);
                mail.Subject = emailSubject;
                mail.Body = emailContent;
                mail.IsBodyHtml = true;

                // Tạo đối tượng SmtpClient để gửi email
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential("tienluyen008@gmail.com", "aylb jyda xvem fcxl");
                smtpClient.EnableSsl = true;

                // Gửi email
                smtpClient.Send(mail);

                Session["Message"]  = "Email đã được gửi thành công!";
            }
            catch (Exception ex)
            {
                Session["Error"]  = "Đã xảy ra lỗi khi gửi email: " + ex.Message;
            }

            return RedirectToAction("GuiMail","Home");
        }
    }
}