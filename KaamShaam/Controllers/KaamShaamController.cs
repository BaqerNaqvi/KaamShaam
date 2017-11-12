using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using KaamShaam.AdminServices;
using KaamShaam.DbEntities;
using KaamShaam.Models;
using Microsoft.AspNet.Identity;

namespace KaamShaam.Controllers
{
    public class KaamShaamController : Controller
    {
        public ActionResult ComminSoon()
        {
            return View();
        }

        public JsonResult SendFeedback(GeneralFeedbackModel model)
        {
            var id=System.Web.HttpContext.Current.User.Identity.GetUserId();
            model.PostedById = id;
            AdminServices.AdminService.AddFeedback(model);
            var email = System.Web.HttpContext.Current.User.Identity.GetUserName();
            KaamShaam.Services.EmailService.SendEmail(email, "FeedBack - KamSham.pk","Thank you for your feedback. We will get back to you soon");
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewFeedback()
        {
            var data = AdminService.GetAllFeedbacks();
            return View(data);
        }

        [HttpPost]
        public JsonResult ChangeFeedbackStatus(GeneralFeedbackModel obj)
        {
            AdminService.ChangeFeedbackStatus(obj);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChangeFeedbackApproval(GeneralFeedbackModel obj)
        {
            AdminService.ChangeFeedbackApproval(obj);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
