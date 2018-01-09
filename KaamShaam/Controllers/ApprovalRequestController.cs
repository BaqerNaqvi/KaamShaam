using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KaamShaam.AdminServices;
using KaamShaam.DbEntities;
using KaamShaam.LocalModels;
using KaamShaam.Models;
using KaamShaam.Services;
using Microsoft.AspNet.Identity;
using LocalUser = KaamShaam.AdminModels.LocalUser;

namespace KaamShaam.Controllers
{
    public class ApprovalRequestController : Controller
    {
        #region Jobs
        public ActionResult Jobs()
        {
            var cats = CategoryService.GetAllCategories();
            var jobs = JobService.GetAllJobs(false);
            if (jobs != null && jobs.Count > 0)
            {
                //jobs = jobs.Where(j => string.IsNullOrEmpty(j.Feedback)).ToList();
            }
            var page = new PaggingClass
            {
                CurrentPage = 0,
                ItemsPerPage = 15,
                TotalItems = jobs?.Count ?? 0,
                SortBy = "Date",
                SortOrder = "Des"
            };
            page = CalculateJobsWithPaging(ref jobs, page);
            page.CurrentPage = 1;
            return View(new ManageJobModel { Categories = cats, JobsList = jobs, Pagging = page });
        }
        [HttpPost]
        public ActionResult GetSortedListApproveJobs(PaggingClass page)
        {
            // if you make any change here, do also make in Approval Controller
            var jobs = JobService.GetAllJobs(false);
            var newPage = CalculateJobsWithPaging(ref jobs, page);
            var modelnew = new JobPartialPageModel
            {
                JobList = jobs,
                Page = new PaggingClass
                {
                    TotalPages = newPage.TotalPages,
                    TotalItems = newPage.TotalItems,
                    CurrentPage = page.CurrentPage + 1,
                    ItemsPerPage = page.ItemsPerPage,
                    SortBy = page.SortBy,
                    SortOrder = page.SortOrder,
                    CategoryId = page.CategoryId,
                    SearchTerm = page.SearchTerm
                }
            };
            return PartialView("~/Views/Job/ListViewJobsPartial.cshtml", modelnew);
        }

        [HttpPost]
        public ActionResult ChnageJobApproval(JobRequestModel model)
        {
           JobService.ChangeJobApproval(model.JobModel);

            var job = JobService.GetJobById(model.JobModel.Id);
           KaamShaam.Services.EmailService.SendEmail(job.JobPostedByObj.Email, "Job Status Updated - KamSham.Pk", job.JobPostedByObj.FullName + " we noticed that admin has updated one of your job. Please review your Jobs in manage job section.");
            KaamShaam.Services.EmailService.SendSms(job.JobPostedByObj.Mobile, "One of your job status has been changed. Please visit https://kamsham.pk");



            var jobs = JobService.GetAllJobs(false);
            if (jobs != null && jobs.Count > 0)
            {
                jobs = jobs.Where(j => string.IsNullOrEmpty(j.Feedback)).ToList();
            }
            model.PageObj.CurrentPage--;
            var newPage = CalculateJobsWithPaging(ref jobs, model.PageObj);
            var modelnew = new JobPartialPageModel
            {
                JobList = jobs,
                Page = new PaggingClass
                {
                    TotalPages = newPage.TotalPages,
                    TotalItems = newPage.TotalItems,
                    CurrentPage = model.PageObj.CurrentPage + 1,
                    ItemsPerPage = model.PageObj.ItemsPerPage,
                    SortBy = model.PageObj.SortBy,
                    SortOrder = model.PageObj.SortOrder,
                    CategoryId = model.PageObj.CategoryId,
                    SearchTerm = model.PageObj.SearchTerm
                }
            };
            return PartialView("~/Views/Job/ListViewJobsPartial.cshtml", modelnew);
        }

        private PaggingClass CalculateJobsWithPaging(ref List<CustomJobModel> jobs, PaggingClass page)
        {
            if (!string.IsNullOrEmpty(page.SearchTerm))
            {
                page.SearchTerm = page.SearchTerm.ToLower();
                jobs = jobs.Where(j => j.JobTitle.ToLower().Contains(page.SearchTerm) ||
                j.Email.ToLower().Contains(page.SearchTerm) || j.JobPostedBy.ToLower().Contains(page.SearchTerm) ||
                j.Fee.ToLower().Contains(page.SearchTerm) || j.LocationName.ToLower().Contains(page.SearchTerm) ||
                j.Mobile.ToLower().Contains(page.SearchTerm)).ToList();
            }
            if (page.CategoryId != 0)
            {
                jobs = jobs.Where(j => j.CategoryId == page.CategoryId).ToList();
            }
            page.TotalItems = jobs.Count;
            if (page.SortBy == "Title")
            {
                jobs = page.SortOrder == "Des" ? jobs.OrderByDescending(o => o.JobTitle).ToList() : jobs.OrderBy(o => o.JobTitle).ToList();
            }
            else if (page.SortBy == "Category")
            {
                jobs = page.SortOrder == "Des" ? jobs.OrderByDescending(o => o.CatName).ToList() : jobs.OrderBy(o => o.CatName).ToList();
            }
            else if (page.SortBy == "Date")
            {
                jobs = page.SortOrder == "Des" ? jobs.OrderByDescending(o => o.PostingDateObj).ToList() : jobs.OrderBy(o => o.PostingDateObj).ToList();
            }
            else if (page.SortBy == "Fee")
            {
                jobs = page.SortOrder == "Des" ? jobs.OrderByDescending(o => Convert.ToInt32(o.Fee)).ToList() : jobs.OrderBy(o => Convert.ToInt32(o.Fee)).ToList();
            }

            jobs = jobs.Skip((page.CurrentPage) * page.ItemsPerPage).Take(page.ItemsPerPage).ToList();
            var tPages = page.TotalItems / page.ItemsPerPage;
            if (page.TotalItems % page.ItemsPerPage > 0)
            {
                tPages++;
            }
            page.TotalPages = tPages == 0 ? 1 : tPages;

            return page;
        }

        #endregion

        #region Categories

        public ActionResult Categories()
        {
            var data = CategoryAdminService.GetNotApprovedCategories();
            return View(data);
        }
        public ActionResult ChnageJobApprovalCats(LocalCategory model)
        {
            CategoryAdminService.ApprovalStatus(model);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Users
        public ActionResult ApproveUsers()
        {
            var data = UserAdminService.GetNotApprovedUsers("User");
            return View(data);
        }
        public ActionResult ChnageUserApproval(LocalUser model)
        {
           var user=  UserAdminService.ApprovalStatus(model);
            var feedback = "";
            if (!model.IsApproved)
            {
                KaamShaam.Services.EmailService.SendEmail(user.Email, "User Account Status Changed - KamSham.Pk", user.FullName + " admin has deleted your account. Please review your account.\n Feedback : " + feedback);
                // sms is in DeleteUser method
            }
            else
            {
                KaamShaam.Services.EmailService.SendEmail(user.Email, "User Account Status Changed - KamSham.Pk", user.FullName + " we noticed that admin has updated your account status. Please review your account." + feedback);
                KaamShaam.Services.EmailService.SendSms(user.Mobile, "Your account status has been changed. Please visit https://kamsham.pk");
            }


            if (!model.IsApproved)
            {
                AdminService.DeleteUser(new AspNetUser { Id = model.Id }, model.Feedback);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Contractors
        public ActionResult ApproveContractors()
        {
            var data = UserAdminService.GetNotApprovedUsers("Contractor");
            return View(data);
        }
        public ActionResult ChnageContractorsApproval(LocalUser model)
        {
            var user = UserAdminService.ApprovalStatus(model);
            var feedback = "";
            if (!model.IsApproved)
            {
                KaamShaam.Services.EmailService.SendEmail(user.Email, "Contractor Account Status Changed - KamSham.Pk", user.FullName + " admin has deleted your account. Please review your account.\n Feedback : " + feedback);
                // sms is in DeleteUser method
            }
            else
            {
                KaamShaam.Services.EmailService.SendEmail(user.Email, "Contractor Account Status Changed - KamSham.Pk", user.FullName + " we noticed that admin has updated your account status. Please review your account." + feedback);
                KaamShaam.Services.EmailService.SendSms(user.Mobile, "Your account status has been changed. Please visit https://kamsham.pk");
            }

            if (!model.IsApproved)
            {
                AdminService.DeleteUser(new AspNetUser { Id = model.Id }, model.Feedback);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Vendors
        public ActionResult ApproveVendors()
        {
            var data = UserAdminService.GetNotApprovedUsers("Vendor");
            return View(data);
        }
        public ActionResult ChnageVendorsApproval(LocalUser model)
        {
            UserAdminService.ApprovalStatus(model);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region

        public ActionResult ApproveFeedback()
        {
            var data = AdminService.GetAllFeedbacks();
            return View(data);
        }
       

        #endregion
    }
}