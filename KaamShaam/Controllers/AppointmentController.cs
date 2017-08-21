using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KaamShaam.AdminServices;
using KaamShaam.LocalModels;

namespace KaamShaam.Controllers
{
    public class AppointmentController : Controller
    {
        // GET: Appointment
        public ActionResult Upcoming()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetSuggestions(string query)
        {
            var data = new List<UserSuggestionModel>();
            var cons = AdminService.GetUsersByType("Contractor");
            foreach (var con in cons)
            {
                if (con.FullName.ToLower().Contains(query.ToLower()))
                {
                    data.Add(new UserSuggestionModel
                    {
                        id = con.Id,
                        label = con.FullName
                    });
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail()
        {
            return View();
        }
    }
}