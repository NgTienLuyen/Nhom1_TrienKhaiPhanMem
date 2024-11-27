using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebQuanLyNhanSu.Controllers
{
    public class KTKLController : Controller
    {

        DBDataContext db = new DBDataContext();
        // GET: KTKL
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemKyLuat(int employeeID, string type, decimal amount, string reason)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Create a new RewardsPenalty object
                    var rewardsPenalty = new RewardsPenalty
                    {
                        EmployeeID = employeeID,
                        Type = type,
                        Amount = amount,
                        Reason = reason,
                        Ngay = DateTime.Now.Date
                    };

                    // Add the rewards/penalty object to the data context
                    db.RewardsPenalties.InsertOnSubmit(rewardsPenalty);
                    var luong = db.Salaries.SingleOrDefault(s => s.EmployeeID == employeeID);
                    if (type == "Phạt")
                    {
                        luong.Phat += amount;
                    }
                    else
                    {
                        luong.Bonus += amount;
                    }
                    
                    // Save changes to the database
                    db.SubmitChanges();

                    // Redirect to home page or any other page as needed
                    return RedirectToAction("KTKL", "Home");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                ViewBag.Error = "An error occurred while adding the rewards/penalty: " + ex.Message;
            }

            // If there's an error or validation fails, return the view with the same model
            return View("KTKL");
        }

        [HttpPost]
        public ActionResult SuaKyLuat(int rewardPenaltyID, string type1, string reason1)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Retrieve the existing rewards/penalty record from the database
                    var existingRecord = db.RewardsPenalties.FirstOrDefault(rp => rp.RewardPenaltyID == rewardPenaltyID);

                    // Check if the record exists
                    if (existingRecord != null)
                    {
                        // Update the existing record with the new information
                        existingRecord.Type = type1;
                        existingRecord.Reason = reason1;

                        // Submit changes to the database
                        db.SubmitChanges();

                        // Redirect to home page or any other page as needed
                        return RedirectToAction("KTKL", "Home", new { success = true });
                    }
                    else
                    {
                        // Handle case where the record does not exist
                        ViewBag.Error = "The rewards/penalty record for employee with ID " + rewardPenaltyID + " does not exist.";
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                ViewBag.Error = "An error occurred while updating the rewards/penalty: " + ex.Message;
            }

            // If there's an error or validation fails, return the view with the same model
            return View("KTKL");
        }
        [HttpPost]
        public ActionResult XoaKyLuat(int rewardPenaltyID)
        {
           try {
               var existingRecord = db.RewardsPenalties.FirstOrDefault(rp => rp.RewardPenaltyID == rewardPenaltyID);
                    if (existingRecord != null)
                    {
                        db.RewardsPenalties.DeleteOnSubmit(existingRecord);
                    var luong = db.Salaries.SingleOrDefault(s => s.EmployeeID == existingRecord.EmployeeID);
                    if (existingRecord.Type == "Phạt")
                    {
                        luong.Phat -= existingRecord.Amount;
                    }
                    else
                    {
                        luong.Bonus -= existingRecord.Amount;
                    }
                    db.SubmitChanges();
                return RedirectToAction("KTKL", "Home", new { success = true });
                }
            else
                {
                    ViewBag.Error = "The rewards/penalty record for employee with ID " + rewardPenaltyID + " does not exist.";
                }
            }
        catch (Exception ex)
        {
            // Handle exceptions
            ViewBag.Error = "An error occurred while updating the rewards/penalty: " + ex.Message;
        }

        // If there's an error or validation fails, return the view with the same model
        return View("KTKL");
    }
            [HttpGet]
            public ActionResult GetEmployeeName(int employeeID)
            {
                var employee = db.Employees.FirstOrDefault(e => e.EmployeeID == employeeID);
                if (employee != null)
                {
                    return Content($"{employee.FirstName} {employee.LastName}");
                }
                else
                {
                    return Content("");
                }
            }
    }
}