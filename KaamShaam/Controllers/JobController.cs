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
        #region Admin

        public ActionResult ListView()
        {
            var cats = CategoryService.GetAllCategories();
            var allJobs = JobService.GetAllJobs(true);
            var page = new PaggingClass
            {
                CurrentPage = 0,
                ItemsPerPage = 15,
                TotalItems = allJobs.Count,
                SortBy = "Date",
                SortOrder = "Des"
            };
            page = CalculateJobsWithPaging(ref allJobs, page);
            page.CurrentPage = 1;
            return View(new ManageJobModel { Categories = cats, JobsList = allJobs, Pagging = page });
        }
        [HttpPost]
        public ActionResult GetSortedListViewJobs(PaggingClass page)
        {
            // if you make any change here, do also make in Approval Controller
            var jobs = JobService.GetAllJobs(true);
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
        /// <summary>
        /// For Admin
        /// </summary>
        [HttpPost]
        public ActionResult DeleteJobByAdmin(JobRequestModel model)
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            JobService.DeleteJob(model.JobModel, id);
            var jobs = JobService.GetAllJobs(true);
            model.PageObj.CurrentPage--;
            var newPage = CalculateJobsWithPaging(ref jobs, model.PageObj);
            var modelnew = new JobPartialPageModel
            {
                JobList = jobs,
                Page = new PaggingClass
                {
                    TotalPages = newPage.TotalPages,
                    TotalItems = newPage.TotalItems,
                    CurrentPage = model.PageObj.CurrentPage+1,
                    ItemsPerPage = model.PageObj.ItemsPerPage,
                    SortBy = model.PageObj.SortBy,
                    SortOrder = model.PageObj.SortOrder,
                    CategoryId = model.PageObj.CategoryId,
                    SearchTerm = model.PageObj.SearchTerm
                }
            };
            return PartialView("~/Views/Job/ListViewJobsPartial.cshtml", modelnew);

        }

        /// <summary>
        /// For Admin
        /// </summary>
        [HttpPost]
        public ActionResult SuspendJobByAdmin(JobRequestModel model)
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            JobService.SuspendResumeJob(model.JobModel, id);
            var jobs = JobService.GetAllJobs(true);
            model.PageObj.CurrentPage--;
            var newPage = CalculateJobsWithPaging(ref jobs, model.PageObj);
            var modelnew = new JobPartialPageModel
            {
                JobList = jobs,
                Page = new PaggingClass
                {
                    TotalPages = newPage.TotalPages,
                    TotalItems = newPage.TotalItems,
                    CurrentPage = model.PageObj.CurrentPage+1,
                    ItemsPerPage = model.PageObj.ItemsPerPage,
                    SortBy = model.PageObj.SortBy,
                    SortOrder = model.PageObj.SortOrder,
                    CategoryId = model.PageObj.CategoryId,
                    SearchTerm = model.PageObj.SearchTerm

                }
            };
            return PartialView("~/Views/Job/ListViewJobsPartial.cshtml", modelnew);
        }
        #endregion

        #region Post Job

        [Authorize]
        [HttpGet]
        public ActionResult PostJob()
        {
            var cats = CategoryService.GetAllCategories();

            return View(new CustomJobModel { Cats = cats });
        }
        [HttpPost]
        [Authorize]
        public ActionResult PostJob(CustomJobModel model)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    model.PostedById = id;
                }
                model.Email = " ";
                JobService.AddJob(model);
            }
            return RedirectToAction("ManageJobs", "Job");
        }
        #endregion

        #region Manage Job

        [Authorize]
        public ActionResult ManageJobs()
        {
            var cats = CategoryService.GetAllCategories();
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var jobs = JobService.GetUserJobs(id);
            var page = new PaggingClass
            {
                CurrentPage = 0,
                ItemsPerPage = 15,
                TotalItems = jobs.Count,
                SortBy = "Date",
                SortOrder = "Des"
            };
            page = CalculateJobsWithPaging(ref jobs, page);
            page.CurrentPage = 1;
            return View(new ManageJobModel { Categories = cats, JobsList = jobs, Pagging = page });
        }
        [HttpPost]
        public ActionResult EditJobDone(JobRequestModel model)
        {
            JobService.EditJob(model.JobModel);
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var jobs = JobService.GetUserJobs(id);
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
            return PartialView("~/Views/Job/ManageJobsPartials.cshtml", modelnew);
        }

        /// <summary>
        /// For Owner
        /// </summary>
        [HttpPost]
        [Authorize]
        public ActionResult DeleteJob(JobRequestModel model)
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            JobService.DeleteJob(model.JobModel, id);
            var jobs = JobService.GetUserJobs(id);
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
            return PartialView("~/Views/Job/ManageJobsPartials.cshtml", modelnew);
        }

        [HttpPost]

        public ActionResult GetSortedJobs(PaggingClass page)
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var jobs = JobService.GetUserJobs(id);
            var newPage=CalculateJobsWithPaging(ref jobs, page);
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
            return PartialView("~/Views/Job/ManageJobsPartials.cshtml", modelnew);
        }
        /// <summary>
        /// For Owner
        /// </summary>
        [HttpPost]
        public ActionResult SuspendJob(JobRequestModel model)
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            JobService.SuspendResumeJob(model.JobModel, id);
            var jobs = JobService.GetUserJobs(id);
            model.PageObj.CurrentPage--;
             var newPage=CalculateJobsWithPaging(ref jobs, model.PageObj);
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
            return PartialView("~/Views/Job/ManageJobsPartials.cshtml", modelnew);
        }

        #endregion

        #region Find & Apply Job
        public ActionResult FindJobs(string jobId)
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var cats = CategoryService.GetAllCategories();
            var jobs = JobService.GetReadyJobs(id);
            if (jobId != null && jobs.Count>0)
            {
                jobs = jobs.Where(j => j.Id==Convert.ToDouble(jobId)).ToList();
               
            }

            foreach (var jo in jobs)
            {
                jo.Mobile = "03xx-xxxxxxx";
            }
            var page = new PaggingClass
            {
                CurrentPage = 0,
                ItemsPerPage = 15,
                TotalItems = jobs.Count,
                SortBy = "Date",
                SortOrder = "Des"
            };
            page = CalculateJobsWithPaging(ref jobs, page);
            page.CurrentPage = 1;
            return View(new ManageJobModel { Categories = cats, JobsList = jobs, Pagging = page , Str = jobId ?? ""});
        }

        [HttpPost]
        public ActionResult FindJobsSorted(PaggingClass page)
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var jobs = JobService.GetReadyJobs(id);
            foreach (var jo in jobs)
            {
                jo.Mobile = "03xx-xxxxxxx";
            }
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
            return PartialView("~/Views/Job/ManageJobsPartials.cshtml", modelnew);
        }

        [HttpPost]
        public ActionResult ApplyJob(JobRequestModel model)
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (model.JobModel.IfIApplied)
            {
                JobService.ApplyJob(new CustomJobHistory
                {
                    JobId = model.JobModel.Id,
                    PurposalText = model.JobModel.PurposalText
                }, id);
            }
            else
            {
                JobService.CancelJob(new CustomJobHistory
                {
                    JobId = model.JobModel.Id,
                    PurposalText = model.JobModel.PurposalText
                }, id);
            }

            var jobs = JobService.GetReadyJobs(id);
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
            return PartialView("~/Views/Job/ManageJobsPartials.cshtml", modelnew);
        }

        #endregion

        #region Applied

        public ActionResult AppliedJobs()
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var cats = CategoryService.GetAllCategories();
            var jobs = JobService.GetAppliedJobs(id);
            var page = new PaggingClass
            {
                CurrentPage = 0,
                ItemsPerPage = 15,
                TotalItems = jobs.Count,
                SortBy = "Date",
                SortOrder = "Des"
            };
            page = CalculateJobsWithPaging(ref jobs, page);
            page.CurrentPage = 1;
            return View(new ManageJobModel { Categories = cats, JobsList = jobs, Pagging = page });
        }

        [HttpPost]
        public ActionResult CancelJob(JobRequestModel model)
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (model.JobModel.IfIApplied)
            {
                JobService.ApplyJob(new CustomJobHistory
                {
                    JobId = model.JobModel.Id,
                    PurposalText = model.JobModel.PurposalText
                }, id);
            }
            else
            {
                JobService.CancelJob(new CustomJobHistory
                {
                    JobId = model.JobModel.Id,
                    PurposalText = model.JobModel.PurposalText
                }, id);
            }

            var jobs = JobService.GetAppliedJobs(id);
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
            return PartialView("~/Views/Job/ManageJobsPartials.cshtml", modelnew);
        }

        [HttpPost]
        public ActionResult AppliedJobsSorted(PaggingClass page)
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var jobs = JobService.GetAppliedJobs(id);
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
            return PartialView("~/Views/Job/ManageJobsPartials.cshtml", modelnew);
        }

        #endregion

        #region My Current Jobs | I applied & Got Job

        public ActionResult MyCurrentJobs()
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var cats = CategoryService.GetAllCategories();
            var jobs = JobService.GetMyCurrentJobsForContractor(id);
            var page = new PaggingClass
            {
                CurrentPage = 0,
                ItemsPerPage = 15,
                TotalItems = jobs.Count,
                SortBy = "Date",
                SortOrder = "Des"
            };
            page = CalculateJobsWithPaging(ref jobs, page);
            page.CurrentPage = 1;
            return View(new ManageJobModel { Categories = cats, JobsList = jobs, Pagging = page });
        }

        [HttpPost]
        public ActionResult MyCurrentJobsSorted(PaggingClass page)
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var jobs = JobService.GetMyCurrentJobsForContractor(id);
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
            return PartialView("~/Views/Job/ManageJobsPartials.cshtml", modelnew);
        }

        #endregion

        #region Job Proposals

        public ActionResult JobProposals()
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var cats = CategoryService.GetAllCategories();
            var jobs = JobService.GetJobsProposals(id);
            var page = new PaggingClass
            {
                CurrentPage = 0,
                ItemsPerPage = 15,
                TotalItems = jobs.Count,
                SortBy = "Date",
                SortOrder = "Des"
            };
            page = CalculateJobsWithPaging(ref jobs, page);
            page.CurrentPage = 1;
            return View(new ManageJobModel { Categories = cats, JobsList = jobs, Pagging = page });
        }

        [HttpPost]
        public ActionResult AssignJob(JobRequestModel model)
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
           
                JobService.AcceptJobProposal(new CustomJobHistory
                {
                    JobId = model.JobModel.Id,
                    PurposalText = model.JobModel.PurposalText,
                    ContractorId = model.JobModel.ContractorId
                }, id);

            var jobs = JobService.GetJobsProposals(id);
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
            return PartialView("~/Views/Job/JobProposalsPartials.cshtml", modelnew);
        }

        [HttpPost]
        public ActionResult GetJobProposalsSorted(PaggingClass page)
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var jobs = JobService.GetJobsProposals(id);
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
            return PartialView("~/Views/Job/JobProposalsPartials.cshtml", modelnew);
        }

        #endregion


        #region Previous Jobs For Contractor

        public ActionResult PreviousJobs()
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var cats = CategoryService.GetAllCategories();
            var jobs = JobService.GetPreviousJobsForContractor(id);
            
            var page = new PaggingClass
            {
                CurrentPage = 0,
                ItemsPerPage = 15,
                TotalItems = jobs.Count,
                SortBy = "Date",
                SortOrder = "Des"
            };
            page = CalculateJobsWithPaging(ref jobs, page);
            page.CurrentPage = 1;
            return View(new ManageJobModel { Categories = cats, JobsList = jobs, Pagging = page });
        }

        [HttpPost]
        public ActionResult PreviousJobsSorted(PaggingClass page)
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var jobs = JobService.GetPreviousJobsForContractor(id);
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
            return PartialView("~/Views/Job/ContractorPreviousJobsPartial.cshtml", modelnew);
        }

        #endregion

        private PaggingClass CalculateJobsWithPaging(ref List<CustomJobModel> jobs,PaggingClass page)
        {
            if (!string.IsNullOrEmpty(page.SearchTerm))
            {
                page.SearchTerm = page.SearchTerm.ToLower();
                jobs = jobs.Where(j => j.JobTitle.ToLower().Contains(page.SearchTerm) || 
                j.Email.ToLower().Contains(page.SearchTerm) || j.JobPostedBy.ToLower().Contains(page.SearchTerm) ||
                j.Fee.ToLower().Contains(page.SearchTerm) || j.LocationName.ToLower().Contains(page.SearchTerm) ||
                j.Mobile.ToLower().Contains(page.SearchTerm) || j.CatName.ToLower().Contains(page.SearchTerm)).ToList();
            }
            if (page.CategoryId != 0)
            {
                jobs = jobs.Where(j => j.CategoryId == page.CategoryId).ToList();
            }

            // i did it

            page.TotalItems = jobs.Count;
            if (page.SortBy == "Title")
            {
               // jobs = page.SortOrder == "Des" ? jobs.OrderByDescending(o => o.JobTitle).ToList() : jobs.OrderBy(o => o.JobTitle).ToList();
               jobs= jobs.OrderBy(o => o.JobTitle).ToList();
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