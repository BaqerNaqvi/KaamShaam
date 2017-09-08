using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KaamShaam.AdminServices;
using KaamShaam.LocalModels;
using KaamShaam.Models;
using KaamShaam.Services;
using Microsoft.AspNet.Identity;

namespace KaamShaam.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        // GET: Appointment
        public ActionResult Upcoming()
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var apps = AppointmentService.GetUserAppoinments(id);
            return View(apps);
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

        [HttpPost]
        public ActionResult AddApp(CustomAppoinmentModel app)
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            app.CreatedBy = id;
            AppointmentService.AddAppointment(app);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(CustomAppoinmentModel app)
        {
            AppointmentService.DeleteAppointment(app);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(CustomAppoinmentModel app)
        {
            AppointmentService.EditAppointment(app);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDayEvents(string dateObj)
        {
            try
            {
                var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
                DateTime dt = DateTime.ParseExact(dateObj, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var apps = AppointmentService.GetDaAppoinments(dt, id);
                return View(new DayAppointmentModel
                {
                    Appoinments = apps,
                    DateName = dt.ToLongDateString()
                });
            }
            catch (Exception df)
            {
                return View(new DayAppointmentModel
                {
                    Appoinments = new List<CustomAppoinmentModel>(),
                    DateName = DateTime.Now.ToLongTimeString()
                });
            }
        }
    }
}