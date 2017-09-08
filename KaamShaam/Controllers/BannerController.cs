using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KaamShaam.AdminModels;
using KaamShaam.DbEntities;
using KaamShaam.Services;

namespace KaamShaam.Controllers
{
    [Authorize]
    public class BannerController : Controller
    {
        public ActionResult ListView()
        {
            //var baseUrl = Server.MapPath("~/Images/Banners/");
            //string[] images = Directory.GetFiles(baseUrl, "*")
            //                         .Select(Path.GetFileName)
            //                         .ToArray();
           var images = new List<BannerModel>();
            var fullImages = BannerService.GetAllBanners();
            if (fullImages!=null && fullImages.Any())
            {
                images = fullImages.Select(img => new BannerModel
              {
                  Id = img.Id,
                  FileName = img.FileName,
                  ShowOrder = img.ShowOrder,
                  Status = img.Status

              }).ToList();
            }
            return View(images);
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
                file.SaveAs(Server.MapPath("~/Images/Banners/") + fileName);
                int order = 0;

                var allBanners= BannerService.GetAllBanners();
                if (allBanners != null && allBanners.Any())
                {
                    order = allBanners.Count;
                }

                BannerService.AddBanner(new Banner
                {
                    FileName = fileName,
                    ShowOrder = order+1,
                    Status = false
                });
            }
            return Json("Uploaded " + Request.Files.Count + " files");
        }
        [HttpPost]
        public JsonResult DeleteBanner(BannerModel obj)
        {
            var baseUrl = Server.MapPath("~/Images/Banners/") + obj.FileName;
            System.IO.File.Delete(baseUrl);
            BannerService.DeleteBanner(obj.Id);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult EditBanner(BannerModel obj)
        {
            BannerService.EditBanner(obj);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}