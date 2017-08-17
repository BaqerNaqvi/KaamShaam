using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KaamShaam.DbEntities;
using KaamShaam.LocalModels;
using UserMapper = KaamShaam.AdminModels.UserMapper;

namespace KaamShaam.AdminServices
{
    public static class UserAdminService
    {
        public static List<KaamShaam.AdminModels.LocalUser> GetNotApprovedUsers(string type)
        {
            using (var dbContext = new KaamShaamEntities())
            {
                var dbCats = dbContext.AspNetUsers.Where(cat =>cat.Type==type &&  !cat.IsApproved && string.IsNullOrEmpty(cat.Feedback)).ToList().Select(pbj => UserMapper.MapUser(pbj)).ToList();
                return dbCats;
            }
        }

        public static void ApprovalStatus(KaamShaam.AdminModels.LocalUser obj)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var dbo = dbcontext.AspNetUsers.FirstOrDefault(l => l.Id == obj.Id);
                if (dbo != null)
                {
                    dbo.IsApproved = obj.IsApproved;
                    if (!obj.IsApproved)
                    {
                        dbo.Feedback = obj.Feedback;
                    }
                    dbcontext.SaveChanges();
                }
            }
        }

        public static bool IsUserApproved(string userName)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var dbo = dbcontext.AspNetUsers.FirstOrDefault(u=> u.UserName.ToLower()== userName.ToLower());
                if (dbo != null)
                {
                    return dbo.IsApproved && (bool) dbo.Status;
                }
                return false;
            }
        }
    }
}