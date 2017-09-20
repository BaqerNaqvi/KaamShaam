using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KaamShaam.AdminServices;
using KaamShaam.DbEntities;
using KaamShaam.Services;

namespace KaamShaam.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        public ActionResult ListView()
        {
            var data = CategoryAdminService.GetCategories();
            return View(data);
        }
        [HttpPost]
        public JsonResult UpdateCat(Category obj)
        {
           var catId=  CategoryAdminService.UpdateCategory(obj);
            return Json(catId, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdateStatus(Category obj)
        {
            CategoryAdminService.UpdateStatus(obj);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DeleteCat(Category obj)
        {
            CategoryAdminService.DeleteCat(obj);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Upload()
        {
            var type = Request.Form["type"];
            var catId = Request.Form["catId"];
            var url = "~/Images/NewCatsIcons";
            if (type=="background")
            {
                url = "~/Images/NewCatsImages";
            }
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath(url), catId+".png");
                file.SaveAs(path);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}