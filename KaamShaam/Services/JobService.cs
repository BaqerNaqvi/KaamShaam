using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Migrations;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Media;
using System.Web;
using KaamShaam.ApiModels;
using KaamShaam.DbEntities;
using KaamShaam.LocalModels;
using KaamShaam.Models;

namespace KaamShaam.Services
{
    public static class JobService
    {
        public static List<CustomJobModel> GetAllJobs(bool isApproved=false)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var jobs = dbcontext.Jobs.Where(j=> !j.IsRecycled 
                && j.IsApproved== isApproved &&( isApproved || j.UserStstus)).ToList()
                .OrderByDescending( j=> j.PostingDate).ToList();
                return jobs.Select(j => j.Mapper()).ToList();
            }
        }       
        public static long AddJob(CustomJobModel job)
        {
            DbGeography loc = null;
            if (!string.IsNullOrEmpty(job.LocationCords) && job.LocationCords != "")
            {
                var latlng = job.LocationCords.Split('_');
                if (latlng.Length == 2)
                {
                    loc = Commons.Commons.ConvertLatLonToDbGeography(latlng[1], latlng[0]); // lat _ lng
                }
            }
            var obj = new Job
            {
                JobTitle = job.JobTitle,
                Mobile = job.Mobile,
                Email = job.Email,
                Fee = !string.IsNullOrEmpty(job.Fee)? Convert.ToInt32(job.Fee):0,
                CategoryId = job.CategoryId,
                Location = loc,
                UserStstus = true,
                AdminStatus = true,
                IsApproved = false,
                IsRecycled = false,
                PostedById = job.PostedById,
                LocationName = job.LocationName,
                PostingDate = DateTime.Now
            };
            using (var dbcontext = new KaamShaamEntities())
            {
                dbcontext.Jobs.Add(obj);
                dbcontext.SaveChanges();
                return obj.Id;
            }
        }
        public static List<CustomJobModel> GetUserJobs(string userId)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var jobs = dbcontext.Jobs.Where(j =>  j.PostedById == userId  && !j.IsRecycled).ToList();
                return jobs.Select(j => j.Mapper()).ToList();
            }
        }
        public static void EditJob(CustomJobModel job)
        {           
            using (var dbcontext = new KaamShaamEntities())
            {
                try
                {
                    var dbObj = dbcontext.Jobs.FirstOrDefault(j => j.Id == job.Id);
                    if (dbObj != null)
                    {
                        dbObj.JobTitle = job.JobTitle;
                        dbObj.Mobile = job.Mobile;
                        dbObj.Email = job.Email ?? "NA";
                        dbObj.Fee = Convert.ToInt32(job.Fee);
                        dbObj.IsApproved = false;
                        dbObj.FeedBack = null;
                        dbObj.CategoryId = job.CategoryId;
                    }
                    dbcontext.SaveChanges();
                }
                catch (Exception cxc)
                {

                }
            }
        }
        public static void DeleteJob(CustomJobModel job, string loggedInUserid)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var dbObj = dbcontext.Jobs.FirstOrDefault(j => j.Id == job.Id);
                if (dbObj != null)
                {
                    if (loggedInUserid == dbObj.PostedById)
                    {
                        dbObj.IsRecycled = true;
                    }
                    else
                    {
                        dbObj.IsRecycled = true;
                    }
                }
                dbcontext.SaveChanges();
            }
        }
        public static void SuspendResumeJob(CustomJobModel job, string loggedInUserid)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var dbObj = dbcontext.Jobs.FirstOrDefault(j => j.Id == job.Id);
                if (dbObj != null)
                {
                    var postedByUser = dbObj.AspNetUser;
                    KaamShaam.Services.EmailService.SendEmail(postedByUser.Email, "Job Status Changed - KamSham.Pk", postedByUser.FullName + " We noticed some changes in your job '" + dbObj.JobTitle + "'. Please review your jobs.");
                    KaamShaam.Services.EmailService.SendSms(postedByUser.Mobile, "Please review your changes in jobs at https://kamsham.pk");




                    if (loggedInUserid == dbObj.PostedById)
                    {
                        dbObj.UserStstus = !dbObj.UserStstus;
                    }
                    else
                    {
                        dbObj.AdminStatus = !dbObj.AdminStatus;
                    }
                }
                dbcontext.SaveChanges();
            }
        }
        public static Job ChangeJobApproval(CustomJobModel job)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var dbJob= dbcontext.Jobs.FirstOrDefault(l => l.Id == job.Id);
                if (dbJob != null)
                {
                    dbJob.IsApproved = job.IsApproved;
                    if (!job.IsApproved)
                    {
                        dbJob.FeedBack = job.Feedback;
                    }
                    else
                    {
                        dbJob.FeedBack = null;
                    }
                    dbcontext.SaveChanges();
                }
                return dbJob;
            }
        }
        public static List<CustomJobModel> GetReadyJobs(string loggedInUserId)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var jobs = dbcontext.Jobs.Where(j => !j.IsRecycled
                && j.IsApproved && j.UserStstus && !j.IsRecycled && j.AdminStatus).ToList();

                jobs = jobs.Where(j=>
                (j.JobHistories.Count==0 || 
                (j.JobHistories.Count>0 &&
                j.JobHistories.All(h => h.JobStatus == (int)Commons.Enums.JobHistoryStatus.Applying)))).ToList();              
                var mappedJobs= jobs.Select(j => j.Mapper()).ToList().OrderByDescending( o=> o.PostingDateObj).ToList();

                foreach (var obj in mappedJobs)
                {
                    if (obj.JobHistory!=null)
                    {
                        obj.IfIApplied = obj.JobHistory.Any
                       (his =>
                           his.ContractorId == loggedInUserId &&
                          his.JobStatus == (int)Commons.Enums.JobHistoryStatus.Applying);
                    }
                }
                return mappedJobs;
            }
        }

        public static void ApplyJob(CustomJobHistory job, string contractorId)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                try
                {
                    dbcontext.JobHistories.Add(new JobHistory
                    {
                        Feedback = "empty",
                        JobStartDate = DateTime.Now,
                        JobEndDate = DateTime.Now,
                        ContractorId = contractorId,
                        PurposalText = job.PurposalText,
                        JobId = job.JobId,
                        JobStatus = (int)KaamShaam.Commons.Enums.JobHistoryStatus.Applying,
                    });
                    dbcontext.SaveChanges();

                    var jobobj = JobService.GetJobById(job.JobId);
                    var posterUser = UserServices.GetUserById(job.PostedById);
                    KaamShaam.Services.EmailService.SendEmail(posterUser.Email, "Job Activity","There is a new job proposal on your job '"+jobobj.JobTitle+"'. Visit https://kamsham.pk");
                    KaamShaam.Services.EmailService.SendSms(posterUser.Mobile, "New proposal at your job posted at https://kamsham.pk");

                }
                catch (Exception dfdf)
                {

                }
            }
        }

        public static void CancelJob(CustomJobHistory job, string contractorId)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                try
                {
                    var jobHistory =
                        dbcontext.JobHistories.FirstOrDefault(
                            ojb => ojb.ContractorId == contractorId && ojb.JobId == job.JobId);
                    if (jobHistory!=null)
                    {
                        dbcontext.JobHistories.Remove(jobHistory);
                        dbcontext.SaveChanges();

                    }
                }
                catch (Exception dfdf)
                {

                }
            }
        }

        public static List<CustomJobModel> GetAppliedJobs(string loggedInUserId)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var jobs = dbcontext.Jobs.Where(j => !j.IsRecycled
                && j.IsApproved && j.UserStstus).ToList();

                var mappedJobs = jobs.Select(j => j.Mapper()).ToList().OrderByDescending( o=> o.PostingDateObj).ToList();
                List<CustomJobModel> tempo= new List<CustomJobModel>();
                foreach (var obj in mappedJobs)
                {
                    if (obj.JobHistory != null)
                    {
                       var isStatus=  obj.JobHistory.Any(
                            t =>
                                t.ContractorId == loggedInUserId &&
                                t.JobStatus ==(int)Commons.Enums.JobHistoryStatus.Applying);

                        if (isStatus)
                        {
                            obj.IfIApplied = true;
                            tempo.Add(obj);
                        }
                    }
                }
                return tempo;
            }
        }

        public static List<CustomJobModel> GetJobsProposals(string loggedInUserId)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var jobs = dbcontext.Jobs.Where(j => !j.IsRecycled
                && j.IsApproved && j.UserStstus && j.PostedById==loggedInUserId).ToList();

                var mappedJobs = jobs.Select(j => j.Mapper()).ToList().OrderByDescending( jo => jo.PostingDateObj).ToList();
                List<CustomJobModel> tempo = new List<CustomJobModel>();
                foreach (var obj in mappedJobs)
                {
                    if (obj.JobHistory != null)
                    {
                        //var isStatus = obj.JobHistory.Any(
                        //     t =>t.JobStatus == (int)Commons.Enums.JobHistoryStatus.Applying);
                        var histories = obj.JobHistory;
                        foreach (var  his in histories)
                        {
                           // if(his.JobStatus != (int)Commons.Enums.JobHistoryStatus.End)
                            {
                                var cc = new CustomJobModel
                                {
                                    Id = obj.Id,
                                    ContractorId = his.ContractorId,
                                    CatName = obj.CatName,
                                    PostedById = obj.PostedById,
                                    Email = obj.Email,
                                    Fee = obj.Fee,
                                    JobTitle = obj.JobTitle,
                                    ContractorName = his.ContractorName,
                                    Mobile = obj.Mobile,
                                    PostingDate = obj.PostingDate,
                                    JobStatus = his.JobStatus,
                                    CanRate = his.JobStatus == (int)Commons.Enums.JobHistoryStatus.Continue
                                };
                                tempo.Add(cc);
                            }
                        }

                        //if (isStatus)
                        //{
                        //    tempo.Add(obj);
                        //}
                    }
                }
                return tempo;
            }
        }

        public static List<CustomJobModel> GetMyCurrentJobsForContractor(string loggedInUserId)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var jobs = dbcontext.Jobs.Where(j => !j.IsRecycled
                && j.IsApproved && j.UserStstus && j.JobHistories.Any(
                             t =>t.ContractorId==loggedInUserId &&
                             t.JobStatus == (int)Commons.Enums.JobHistoryStatus.Continue)).ToList();
                var mappedJobs = jobs.Select(j => j.Mapper()).ToList().OrderByDescending( j=> j.PostingDateObj).ToList();
                return mappedJobs;
            }
        }

        public static List<CustomJobModel> GetPreviousJobsForContractor(string loggedInContractorId)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var jobs = dbcontext.Jobs.Where(j => !j.IsRecycled
                && j.IsApproved && j.UserStstus && j.JobHistories.Any(
                             t => t.ContractorId == loggedInContractorId &&
                             t.JobStatus == (int)Commons.Enums.JobHistoryStatus.End)).ToList();
                var mappedJobs = jobs.Select(j => j.Mapper()).ToList().OrderByDescending(j => j.PostingDateObj).ToList();
                foreach (var job in mappedJobs)
                {
                    var rate = UserRatingService.GetRatingByJobId(job.Id);
                    if (rate!=null)
                    {
                        job.RatingStarForConForPrevJob = rate.Rating.ToString();
                        job.RatingStringForConForPrevJob = rate.Comments; 
                    }
                }
                return mappedJobs;
            }
        }

        public static void AcceptJobProposal(CustomJobHistory job, string contractorId)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                try
                {
                    var dbjob = dbcontext.Jobs.FirstOrDefault(j => j.Id == job.JobId);
                    if (dbjob != null)
                    {
                        var jhistory= dbjob.JobHistories.ToList();
                        foreach (var history in jhistory)
                        {
                            if (history.ContractorId == job.ContractorId)
                            {
                                history.JobStatus = (int) Commons.Enums.JobHistoryStatus.Continue;
                                history.Feedback = "Job started with continue status";
                            }
                            else
                            {
                                dbcontext.JobHistories.Remove(history);
                            }
                        }
                        dbcontext.SaveChanges();
                    }

                    var conractUser = UserServices.GetUserById(contractorId);
                    KaamShaam.Services.EmailService.SendEmail(conractUser.Email, "Job Assignment", "You have been assigned a new job '" + dbjob.JobTitle + "'. Visit https://kamsham.pk");
                    KaamShaam.Services.EmailService.SendSms(conractUser.Mobile, "Your proposal for job has been accepted at https://kamsham.pk");

                }
                catch (Exception dfdf)
                {

                }
            }
        }

        public static CustomJobModel GetJobById(long jobId)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var jobs = dbcontext.Jobs.FirstOrDefault(job => job.Id == jobId).Mapper();
                return jobs;
            }
        }

        public static void Feedback(ApiRequestModel model)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var jobObj = dbcontext.Jobs.FirstOrDefault(job => job.Id == model.JobId);
                jobObj.FeedBack = model.Feedback;
                dbcontext.SaveChanges();
            }
        }

        public static void MarkJobDone(long jobid, string conId)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                try
                {
                    var his = dbcontext.JobHistories.FirstOrDefault(hs => hs.JobId == jobid && hs.ContractorId == conId);
                    his.JobStatus = 3;
                    dbcontext.SaveChanges();

                    var myjob = dbcontext.Jobs.FirstOrDefault(jhj => jhj.Id == jobid);
                    myjob.UserStstus = true;

                    var otherHIstoryes = dbcontext.JobHistories.Where(jo => jo.JobId == jobid && jo.Id != his.Id);
                    if (otherHIstoryes != null && otherHIstoryes.Any())
                    {
                        foreach (var oo in otherHIstoryes)
                        {
                            dbcontext.JobHistories.Remove(oo);
                        }
                    }
                    dbcontext.SaveChanges();
                    KaamShaam.Services.EmailService.SendEmail(myjob.AspNetUser.Email, "Job Status Updated - KamSham.Pk", " Your job status has been updated. Please review your jobs section.");
                    KaamShaam.Services.EmailService.SendSms(myjob.AspNetUser.Mobile, "Your job status has been updated at https://kamsham.pk");

                }
                catch (Exception)
                {

                }
            }
        }
    }
}