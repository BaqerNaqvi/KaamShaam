using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using KaamShaam.AdminServices;
using KaamShaam.Models;
using KaamShaam.Services;
using System.Net.Mail;
using System.Text;


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
         //   var contractors = AdminService.GetUsersByType("Contractor");
         //   var gropuedCons= contractors.GroupBy(con => con.CategoryId).ToList().OrderByDescending( dd=>dd.Count());
            var listcons = new List<ContractorIndexPageModel>();
            //foreach (var gcon in gropuedCons)
            //{
            //    var obj = gcon.FirstOrDefault();
            //    listcons.Add(new ContractorIndexPageModel
            //    {
            //        CatName = obj.CatName,
            //        CatCount = gcon.Count(),
            //        CategoryId = obj.CategoryId
            //    });

            //}
            var cats = CategoryService.GetAllCategories();
            if (cats != null && cats.Any())
            {
                cats = cats.OrderByDescending(ca => ca.JobCount).ToList().ToList();
                foreach (var obj in cats)
                {
                    listcons.Add(new ContractorIndexPageModel
                    {
                        CatName = obj.Name,
                        CatCount = obj.JobCount,
                        CategoryId = obj.Id,
                        BgURL = obj.Image,
                        IconURL = obj.Icon
                    });
                }
            }
            var feed = AdminService.GetApprovedFeedbacks();
            return View(new HomePageWraper
            {
                BannersList = paths,
                ContractorCats = listcons,
                Feedback = feed
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