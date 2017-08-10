using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KaamShaam.AdminModels;

namespace KaamShaam.Controllers
{
    public class BannerController : Controller
    {
        public ActionResult ListView()
        {
            var baseUrl = Server.MapPath("~/Images/Banners/");
            string[] images = Directory.GetFiles(baseUrl, "*")
                                     .Select(Path.GetFileName)
                                     .ToArray();
            var list = images.Select(img => new BannerModel { Path = img }).ToList();
            return View(list);
        }
        public JsonResult Upload()
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string mimeType = file.ContentType;
                System.IO.Stream fileContent = file.InputStream;
                //To save file, use SaveAs method
                file.SaveAs(Server.MapPath("~/Images/Banners/") + fileName); //File will be saved in application root
            }
            return Json("Uploaded " + Request.Files.Count + " files");
        }
        [HttpPost]
        public JsonResult DeleteBanner(BannerModel obj)
        {
            var baseUrl = Server.MapPath("~/Images/Banners/") + obj.Path;
            System.IO.File.Delete(baseUrl);

            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}