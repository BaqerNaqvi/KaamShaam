using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using KaamShaam.LocalModels;
using KaamShaam.Services;
using Microsoft.AspNet.Identity;
using Microsoft.SqlServer.Types;
using SqlServerTypes;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;

namespace KaamShaam.Controllers
{
    public class ProfileController : Controller
    {
        [Authorize]
        public ActionResult Update()
        {            
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var user = UserServices.GetUserById(id);
            var cats = CategoryService.GetAllCategories();
            var companies = UserServices.GetUserTypeDd("Vendor");
            return View(new ProfileWraperModel {BasicInfo = user, Categoreis= cats, Companies = companies});
        } 
        public ActionResult UpdateBasicInfo(LocalUser user)
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            user.Id = id;
            var updatedUser= UserServices.UpdateBasicInfo(user).MapUser();

            SetUserSession(updatedUser);
            return Json(true, JsonRequestBehavior.AllowGet); 
        }
        public ActionResult UpdateLocationInfo(LocalUser user)
         {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            user.Id = id;
            var updatedUser = UserServices.UpdateLocInfo(user).MapUser();
           SetUserSession(updatedUser);

            return Json(true, JsonRequestBehavior.AllowGet); 
        }
        public ActionResult UpdateOtherInfo(LocalUser user)
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            user.Id = id;
            UserServices.UpdateOtherInfo(user);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateCompanyInfo(LocalUser user)
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            user.Id = id;
            UserServices.UpdateCompanyInfo(user);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Upload()
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var uid = System.Web.HttpContext.Current.User.Identity.GetUserId();
                HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                //To save file, use SaveAs method
                var pathForProfileImage = Server.MapPath("~/Images/Profiles/") + uid + ".png";
                file.SaveAs(pathForProfileImage);
                var baseUrl = Server.MapPath("~/Images/Profiles/");
                AppUtils.Common.GenerateImages(uid, baseUrl);


                string imgbaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                       Request.ApplicationPath.TrimEnd('/') + "/Images/";


                Session["Photo"] = imgbaseUrl + "Profiles/" + uid + "_110.png";

            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public void SetUserSession(LocalUser user)
        {
            #region Session
            Session["UserName"] = user.FullName;
            Session["Address"] = user.LocationName;

            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                   Request.ApplicationPath.TrimEnd('/') + "/Images/";


            var img = baseUrl + "Profiles/" + user.Id + "_110.png";
            Session["Photo"] = AppUtils.Common.ReturnImage(img, "110x110");
            #endregion
        }
    }
} 