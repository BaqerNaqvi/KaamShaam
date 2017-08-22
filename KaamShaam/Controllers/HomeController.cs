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

            try
            {
                MailMessage message = new System.Net.Mail.MailMessage();
                string fromEmail = "baqer@gmail.com";
                string fromPW = "mypw";
                string toEmail = "baqer.naqvi@afiniti.com";
                message.From = new MailAddress(fromEmail);
                message.To.Add(toEmail);
                message.Subject = "Hello";
                message.Body = "Hello Bob ";
                message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential("link2naqvi@gmail.com", "Spacein786");

                    smtpClient.Send(message.From.ToString(), message.To.ToString(),
                                    message.Subject, message.Body);
                }
            }
            catch (Exception ffg)
            {

            }
            return View();
        }
        public ActionResult Contact()
        {
           
            return View();
        }
    }
}