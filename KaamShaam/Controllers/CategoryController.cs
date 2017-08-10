using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KaamShaam.AdminServices;
using KaamShaam.DbEntities;
using KaamShaam.Services;

namespace KaamShaam.Controllers
{
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
            CategoryAdminService.UpdateCategory(obj);
            return Json(true, JsonRequestBehavior.AllowGet);
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
    }
}