using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaamShaam.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var bannerPath = Server.MapPath("/Images/Banners");
            DirectoryInfo d = new DirectoryInfo(bannerPath);
            FileInfo[] Files = d.GetFiles();
            var paths = new List<string>();
            foreach (FileInfo file in Files)
            {
               paths.Add(file.Name);
            }
            return View(paths);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}