using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KaamShaam.DbEntities;
using KaamShaam.LocalModels;
using KaamShaam.Models;
using LocalUser = KaamShaam.AdminModels.LocalUser;

namespace KaamShaam.Services
{
    public static class UserRatingService
    {
        public static void AddRating(LocalUserRating source)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var obj = new UserRating
                {
                    DateTime = DateTime.Now,
                    IsApproved = false,
                    JobId = source.JobId,
                    EditedAt = DateTime.Now,
                    Comments = source.Comments,
                    RatedBy = source.RatedBy,
                    RatedTo = source.RatedTo,
                    Rating = source.Rating
                };
                dbcontext.UserRatings.Add(obj);
                dbcontext.SaveChanges();
               // return obj.Mapper();
            }
        }

        public static List<LocalUserRating> GetAllRatingsForDash()
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var newRatings = new List<LocalUserRating>();
                var ratnigs =dbcontext.UserRatings.ToList();
                if (ratnigs.Any())
                {
                    newRatings = ratnigs.Select(rt => rt.Mapper()).ToList().OrderByDescending( r => r.DateTime).ToList();
                }
                return newRatings;
            }
        }

        public static LocalUserRating GetRatingByJobId(long jobid)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var newRatings = new LocalUserRating();
                var ratnigs = dbcontext.UserRatings.FirstOrDefault( r => r.JobId==jobid);
                if (ratnigs!=null)
                {
                    newRatings = ratnigs.Mapper()
                    ;
                }
                return newRatings;
            }
        }


        public static double GetRatingsInFloat(string ofUserId)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                double newRatings = 0.0;
                var ratnigs = dbcontext.UserRatings.Where(rate => rate.RatedTo == ofUserId && rate.IsApproved).ToList();
                if (ratnigs.Any())
                {
                    newRatings = ratnigs.Sum(rrr => rrr.Rating)/ratnigs.Count;                   
                }
                return newRatings;
            }
        }

        public static void ChangeApproval(LocalUserRating rate)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var dbobj = dbcontext.UserRatings.FirstOrDefault(fd => fd.Id == rate.Id);
                if (dbobj != null)
                {
                    dbobj.IsApproved = rate.IsApproved;
                }
                dbcontext.SaveChanges();
            }
        }
    }
}