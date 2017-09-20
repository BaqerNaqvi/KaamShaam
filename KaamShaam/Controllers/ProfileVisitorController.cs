using System;
using System.Collections.Generic;
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
    public class ProfileVisitorController : Controller
    {
        // GET: ProfileVisitor
        public ActionResult AddNewVisit(LocalProfileVisit visit)
        {
            var visitorId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (!string.IsNullOrEmpty(visitorId))
            {
                visit.VistedBy = visitorId;
                var dbVisit = ProfileVisitorService.AddLocalVisit(visit);
                return Json(dbVisit, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddNewRating(LocalUserRating rating)
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            rating.RatedBy = id;
            try
            {
                rating.Rating = Convert.ToDouble(rating.TempoRating);
                UserRatingService.AddRating(rating);
            }
            catch (Exception excep)
            {

            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// for dashborad
        /// </summary>
        /// <returns></returns>
        public ActionResult UserReviews()
        {
            var data = UserRatingService.GetAllRatingsForDash();
            return View(data);
        }
        [HttpPost]
        public JsonResult ChangeRatingApproval(LocalUserRating obj)
        {
            UserRatingService.ChangeApproval(obj);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

    }
}