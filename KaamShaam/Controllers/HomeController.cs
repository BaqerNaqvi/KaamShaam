using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KaamShaam.AdminServices;
using KaamShaam.Models;
using KaamShaam.Services;

namespace KaamShaam.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var paths = new List<string>();
            var bannerPath = Server.MapPath("/Images/Banners");
            var imgs = BannerService.GetActiveBanners();
            if (imgs != null && imgs.Any())
            {
                paths.AddRange(imgs.Select(banner => banner.FileName));
            }
            var contractors = AdminService.GetUsersByType("Contractor");
            var gropuedCons= contractors.GroupBy(con => con.CategoryId).ToList().OrderByDescending( dd=>dd.Count());
            var listcons = new List<ContractorIndexPageModel>();
            foreach (var gcon in gropuedCons)
            {
                var obj = gcon.FirstOrDefault();
                listcons.Add(new ContractorIndexPageModel
                {
                    CatName = obj.CatName,
                    CatCount = gcon.Count(),
                    CategoryId = obj.CategoryId
                });

            }
            return View(new HomePageWraper
            {
                BannersList = paths,
                ContractorCats = listcons
            });
        }
        public ActionResult About()
        {
            
            return View();
        }
        public ActionResult Contact()
        {
           
            return View();
        }
    }
}