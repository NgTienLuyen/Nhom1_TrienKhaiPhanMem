using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace WebQuanLyNhanSu.Controllers
{
    public class LuongController : Controller
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
        public ActionResult Them(int employeeID, decimal basicSalary)
        {
            // Kiểm tra xem mã nhân viên đã tồn tại trong cơ sở dữ liệu chưa
            var existingEmployee = db.Employees.FirstOrDefault(e => e.EmployeeID == employeeID);

            if (existingEmployee != null)
            {
                // Mã nhân viên đã tồn tại, tiến hành thêm mới bảng lương

                // Kiểm tra xem bảng lương của nhân viên đã tồn tại chưa
                var existingSalary = db.Salaries.FirstOrDefault(s => s.EmployeeID == employeeID);

                if (existingSalary == null)
                {
                    // Tạo mới đối tượng Salary
                    Salary newSalary = new Salary
                    {
                        EmployeeID = employeeID,
                        BasicSalary = basicSalary,
                        Bonus = 0,
                        Phat = 0,
                        Deductions = 0,
                        SoGioLam = 0,
                        OT =0 ,
                        GioNgayLe = 0,
                        TongSoGioLam =0
                    };

                    // Thêm bảng lương mới vào cơ sở dữ liệu
                    db.Salaries.InsertOnSubmit(newSalary);
                    db.SubmitChanges();

                    // Chuyển hướng về trang danh sách bảng lương sau khi thêm thành công
                    return RedirectToAction("BangLuong", "Home");
                }
                else
                {
                    // Bảng lương của nhân viên đã tồn tại, không thể thêm mới
                    ModelState.AddModelError("", "Bảng lương cho nhân viên này đã tồn tại.");
                }
            }
            else
            {
                // Mã nhân viên không tồn tại, hiển thị thông báo lỗi
                ModelState.AddModelError("", "Mã nhân viên không tồn tại.");
            }

            // Nếu có lỗi xảy ra, hiển thị lại view "Them" với thông báo lỗi
            return View();
        }

        public ActionResult Sua(int id)
        {
            var salary = db.Salaries.FirstOrDefault(s => s.SalaryID == id);

            if (salary == null)
            {
                return RedirectToAction("BangLuong", "Luong");
            }

            return View(salary);
        }

        [HttpPost]
        public ActionResult Sua(Salary model)
        {
            if (ModelState.IsValid)
            {
                var salaryToUpdate = db.Salaries.FirstOrDefault(s => s.SalaryID == model.SalaryID);

                if (salaryToUpdate != null)
                {
                    // Cập nhật các trường trong bảng lương
                    salaryToUpdate.BasicSalary = model.BasicSalary;
                    salaryToUpdate.Bonus = model.Bonus;
                    salaryToUpdate.Phat = model.Phat;
                    salaryToUpdate.SoGioLam = model.SoGioLam;
                    salaryToUpdate.OT = model.OT;
                    salaryToUpdate.GioNgayLe = model.GioNgayLe;

                    db.SubmitChanges();
                    return RedirectToAction("BangLuong", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Không tìm thấy bảng lương cần sửa.");
                }
            }

            return View(model);
        }

        public ActionResult Xoa(int id)
        {
            try
            {
                // Tìm lương cần xóa
                var salary = db.Salaries.FirstOrDefault(s => s.SalaryID == id);
                if (salary == null)
                {
                    // Trả về mã lỗi nếu không tìm thấy lương
                    return HttpNotFound();
                }

                // Xóa lương khỏi cơ sở dữ liệu
                db.Salaries.DeleteOnSubmit(salary);
                db.SubmitChanges();

                // Trả về kết quả thành công
                return RedirectToAction("BangLuong", "Home");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có bất kỳ ngoại lệ nào xảy ra
                // Ví dụ: Log lỗi, hiển thị thông báo lỗi, v.v.
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi xóa lương: " + ex.Message;
                return View("Error"); // Điều hướng đến trang lỗi
            }
        }
        public ActionResult InFileExcel()
        {
            var salaries = db.Salaries.ToList(); // Lấy dữ liệu từ cơ sở dữ liệu

            byte[] excelContents = GenerateExcel(salaries); // Gọi phương thức GenerateExcel để tạo nội dung file Excel
            foreach (var sl in salaries)
            {
                sl.OT = 0;
                sl.SoGioLam = 0;
                sl.TongSoGioLam = 0;
                sl.Phat = 0;
                sl.Bonus = 0;
                sl.GioNgayLe = 0;
                db.SubmitChanges();
            }
            return File(excelContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Bảng Lương.xlsx");
        }

        private byte[] GenerateExcel(IEnumerable<Salary> salaries)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };
                    sheets.Append(sheet);

                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    // Tạo header cho file Excel
                    Row headerRow = new Row();
                    headerRow.Append(
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue("Mã Nhân Viên") },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue("Lương Cơ Bản") },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue("Thưởng") },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue("Phạt") },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue("Tiền công làm") },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue("Tiền OT") },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue("Tiền làm ngày lễ") },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue("Khấu trừ") },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue("Tổng tiền") }
                    );
                    sheetData.AppendChild(headerRow);

                    // Thêm dữ liệu vào file Excel
                    foreach (var salary in salaries)
                    {
                        Row newRow = new Row();
                        newRow.Append(
                            new Cell() { DataType = CellValues.Number, CellValue = new CellValue(salary.EmployeeID.ToString()) },
                            new Cell() { DataType = CellValues.Number, CellValue = new CellValue((double)salary.BasicSalary) },
                            new Cell() { DataType = CellValues.Number, CellValue = new CellValue((double)salary.Bonus) },
                            new Cell() { DataType = CellValues.Number, CellValue = new CellValue((double)salary.Phat) },
                            new Cell() { DataType = CellValues.Number, CellValue = new CellValue((double)(salary.SoGioLam * salary.BasicSalary)) },
                            new Cell() { DataType = CellValues.Number, CellValue = new CellValue((double)(salary.OT * 2 * salary.BasicSalary)) },
                            new Cell() { DataType = CellValues.Number, CellValue = new CellValue((double)(salary.GioNgayLe * 3 * salary.BasicSalary)) },
                            new Cell() { DataType = CellValues.Number, CellValue = new CellValue(salary.Deductions.ToString()) },
                            new Cell() { DataType = CellValues.Number, CellValue = new CellValue((double)(salary.SoGioLam * salary.BasicSalary + salary.OT * 2 * salary.BasicSalary + salary.GioNgayLe * 3 * salary.BasicSalary + salary.Bonus - salary.Phat) * 0.895) }
                        );

                        sheetData.AppendChild(newRow);
                    }

                    workbookPart.Workbook.Save();
                }

                return memoryStream.ToArray();
            }
        }
        

    }
}