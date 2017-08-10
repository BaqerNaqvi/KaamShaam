using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KaamShaam.AdminModels;
using KaamShaam.AdminServices;
using KaamShaam.DbEntities;

namespace KaamShaam.Controllers
{
    public class UserController : Controller
    {
        public ActionResult ListView()
        {
            var data = AdminService.GetAllUsers();
            return View(data);
        }
        public ActionResult Vendors()
        {
            var data = AdminService.GetUsersByType("Vendor");
            return View(data);
        }
        public ActionResult Contractors(string vendorId, string vendorName)
        {
            var data = AdminService.GetUsersByType("Contractor");
            if (!string.IsNullOrEmpty(vendorId))
            {
                data = data.Where(a => a.ContractorId == vendorId).ToList();
            }
            return View(new VendorPageModel
            {
                Contractors = data,
                Vendor = vendorName
            });
        }
        public ActionResult Users()
        {
            var data = AdminService.GetUsersByType("User");
            return View(data);
        }
        [HttpPost]
        public JsonResult UpdateStatus(AspNetUser obj)
        {
            AdminService.UpdateStatus(obj);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DeleteUser(AspNetUser obj)
        {
            AdminService.DeleteUser(obj);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}