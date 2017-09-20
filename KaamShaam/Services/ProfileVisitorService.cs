using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KaamShaam.DbEntities;
using KaamShaam.LocalModels;

namespace KaamShaam.Services
{
    public class ProfileVisitorService
    {
        public static LocalProfileVisit AddLocalVisit(LocalProfileVisit visit)
        {
            using (var dbContext = new KaamShaamEntities())
            {
                var obj = new ProfileVisit
                {
                    DateTime = DateTime.Now,
                    VistedBy = visit.VistedBy,
                    VistedOf = visit.VistedOf
                };
                dbContext.ProfileVisits.Add(obj);
                dbContext.SaveChanges();
                return obj.Mapper();
            }
        }
    }
}