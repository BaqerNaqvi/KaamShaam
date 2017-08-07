using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KaamShaam.AdminModels;
using KaamShaam.AdminServices;
using KaamShaam.Services;

namespace KaamShaam.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
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
    }
}