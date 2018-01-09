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
using System.Text;
using Microsoft.AspNet.Identity;


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
            var listcons = new List<ContractorIndexPageModel>();
            var cats = CategoryService.GetAllCategories();
            if (cats != null && cats.Any())
            {
                cats = cats.OrderByDescending(ca => ca.ContractorCount).ToList().ToList();
                listcons.AddRange(cats.Select(obj => new ContractorIndexPageModel
                {
                    CatName = obj.Name, CatCount = obj.ContractorCount, CategoryId = obj.Id, BgURL = obj.Image, IconURL = obj.Icon
                }));
            }
            var feed = AdminService.GetApprovedFeedbacks();
            
            var jobs = JobService.GetReadyJobs(null).Take(9).ToList();
            return View(new HomePageWraper
            {
                BannersList = paths,
                ContractorCats = listcons,
                Feedback = feed,
                Jobs = jobs
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

        public ActionResult TermsAndCondition()
        {
            return View();
        }

        public JsonResult PostContactUsForm(ContacUsFormModel model)
        {
            var mobile = model.Phone;
            mobile = mobile.Substring(1).Replace("-", "");
            model.Phone = "92" + mobile;

            KaamShaam.Services.EmailService.SendEmail("Link2naqvi@gmail.com", "'"+model.Subject+"' | "+model.FullName+" - KamSham.pk", "Admin\n You have recieved following message from "+model.FullName+" - "+model.Phone+"\n'"+model.Message+"'");
            KaamShaam.Services.EmailService.SendSms(model.Phone, "Thank you for your feedback. We will get back to you soon.");


            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}