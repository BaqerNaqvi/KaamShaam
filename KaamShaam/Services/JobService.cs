using System;
using System.Collections.Generic;
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
        public static List<CustomJobModel> GetAllJobs()
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var jobs = dbcontext.Jobs.ToList();
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
                Fee = Convert.ToInt32(job.Fee),
                CategoryId = job.CategoryId,
                Location = loc,
                Ststus = true,
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
                var jobs = dbcontext.Jobs.Where(j => j.PostedById != null && j.PostedById == userId).ToList();
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
                    dbObj.CategoryId = job.CategoryId;
                }
                dbcontext.SaveChanges();
            }
        }

        public static void DeleteJob(CustomJobModel job)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var dbObj = dbcontext.Jobs.FirstOrDefault(j => j.Id == job.Id);
                if (dbObj != null)
                {
                    dbcontext.Jobs.Remove(dbObj);
                }
                dbcontext.SaveChanges();
            }
        }
        public static void SuspendResumeJob(CustomJobModel job)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var dbObj = dbcontext.Jobs.FirstOrDefault(j => j.Id == job.Id);
                if (dbObj != null)
                {
                    dbObj.Ststus = !dbObj.Ststus;
                }
                dbcontext.SaveChanges();
            }
        }
    }
}