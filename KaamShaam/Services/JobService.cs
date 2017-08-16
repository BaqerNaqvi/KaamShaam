using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;
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
                && j.IsApproved== isApproved &&( isApproved || j.UserStstus)).ToList();
                return jobs.Select(j => j.Mapper()).ToList();
            }
        }       
        public static void AddJob(CustomJobModel job)
        {
            DbGeography loc = null;
            if (!string.IsNullOrEmpty(job.LocationCords) && job.LocationCords != "")
            {
                var latlng = job.LocationCords.Split('_');
                if (latlng.Length == 2)
                {
                    loc = Commons.ConvertLatLonToDbGeography(latlng[1], latlng[0]); // lat _ lng
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
                var dbObj = dbcontext.Jobs.FirstOrDefault(j => j.Id == job.Id);
                if (dbObj != null)
                {
                    dbObj.JobTitle = job.JobTitle;
                    dbObj.Mobile = job.Mobile;
                    dbObj.Email = job.Email;
                    dbObj.Fee = Convert.ToInt32(job.Fee);
                    dbObj.IsApproved = false;
                    dbObj.FeedBack = null;
                    dbObj.CategoryId = job.CategoryId;
                }
                dbcontext.SaveChanges();
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

        public static void ChangeJobApproval(CustomJobModel job)
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
                    dbcontext.SaveChanges();
                }
            }
        }
    }
}