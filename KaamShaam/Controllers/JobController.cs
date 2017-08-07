using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KaamShaam.Models;
using KaamShaam.Services;
using Microsoft.AspNet.Identity;

namespace KaamShaam.Controllers
{
    public class JobController : Controller
    {
        [HttpGet]
        public ActionResult PostJob()
        {
            var cats = CategoryService.GetAllCategories();

            return View(new CustomJobModel {Cats = cats});
        }

        [HttpPost]
        public ActionResult PostJob(CustomJobModel model)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    model.PostedById = id;
                }
                JobService.AddJob(model); 
            }
            return RedirectToAction("ManageJobs", "Job");
        }

        public ActionResult ManageJobs()
        {
            var cats = CategoryService.GetAllCategories();
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var jobs=JobService.GetUserJobs(id);
            var page = new PaggingClass
            {
                CurrentPage = 0,
                ItemsPerPage = 2,
                TotalItems = jobs.Count,
                SortBy = "Date",
                SortOrder = "Des"
            };
            page=CalculateJobsWithPaging(ref jobs, page);
            page.CurrentPage = 1;
            return View(new ManageJobModel {Categories = cats, JobsList = jobs, Pagging = page});
        }

        [HttpPost]
        public ActionResult EditJobDone(JobRequestModel model)
        {           
            JobService.EditJob(model.JobModel);
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var jobs = JobService.GetUserJobs(id);
            model.PageObj.CurrentPage--;
            CalculateJobsWithPaging(ref jobs, model.PageObj);

            return PartialView("~/Views/Job/ManageJobsPartials.cshtml", jobs);
        }

        [HttpPost]
        public ActionResult DeleteJob(JobRequestModel model)
        {
            JobService.DeleteJob(model.JobModel);
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var jobs = JobService.GetUserJobs(id);
            model.PageObj.CurrentPage--;
            CalculateJobsWithPaging(ref jobs, model.PageObj);
            return PartialView("~/Views/Job/ManageJobsPartials.cshtml", jobs);
        }

        [HttpPost]
        public ActionResult GetSortedJobs(PaggingClass page)
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var jobs = JobService.GetUserJobs(id);
            CalculateJobsWithPaging(ref jobs, page);
            return PartialView("~/Views/Job/ManageJobsPartials.cshtml", jobs);
        }

        [HttpPost]
        public ActionResult SuspendJob(JobRequestModel model)
        {
            JobService.SuspendResumeJob(model.JobModel);
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var jobs = JobService.GetUserJobs(id);
            model.PageObj.CurrentPage--;
            CalculateJobsWithPaging(ref jobs, model.PageObj);
            return PartialView("~/Views/Job/ManageJobsPartials.cshtml", jobs);
        }

        private PaggingClass CalculateJobsWithPaging(ref List<CustomJobModel> jobs,PaggingClass page)
        {
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
    }
}