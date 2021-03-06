﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KaamShaam.AdminModels;
using KaamShaam.AdminServices;
using KaamShaam.Services;

namespace KaamShaam.Controllers
{
    [Authorize(Roles = "Admin,Super Admin")]
    public class AdminController : Controller
    {
        public ActionResult Stats()
        {
            var vendors = AdminService.GetUsersByType("Vendor");
            var contractors = AdminService.GetUsersByType("Contractor");
            var users = AdminService.GetUsersByType("User");
            var jobs = JobService.GetAllJobs();

            var cats = CategoryAdminService.GetCategories();
            return View(new DashboardPageModel
            {
                ContractorCount = contractors.Count,
                UserCount = users.Count,
                VendorsCount = vendors.Count,
                Categories = cats,
                JobsCount = jobs.Count
            });
        }
        public ActionResult AdminUsers()
        {
            var admins= AdminService.GetAdminUsers();
            return View(admins);
        } 
        public ActionResult FindUserByEmail(MakeAdminModel model)
        {
            var user = AdminService.FindUserByUsername(model.Email);
            if (user == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(user, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddUserInRole(MakeAdminModel model)
        {
            var user = AdminService.AddUserToRole(model);
            KaamShaam.Services.EmailService.SendEmail(user.Email, "User Account Status Changed - KamSham.Pk", user.FullName + " we noticed that admin has updated your account role. Please visit https://kamsham.pk and review your account.");
            KaamShaam.Services.EmailService.SendSms(user.Mobile, "Your account status has been changed. Please visit https://kamsham.pk");

            if (user == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(user, JsonRequestBehavior.AllowGet);
        } 
        public ActionResult RemoveFromRole(MakeAdminModel model)
        {
            AdminService.RemoveUserFromRole(model);
            return Json(true, JsonRequestBehavior.AllowGet);           
        }        
    }
}