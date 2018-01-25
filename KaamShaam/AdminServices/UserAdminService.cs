using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KaamShaam.DbEntities;
using KaamShaam.LocalModels;
using LocalUser = KaamShaam.AdminModels.LocalUser;
using UserMapper = KaamShaam.AdminModels.UserMapper;

namespace KaamShaam.AdminServices
{
    public static class UserAdminService
    {
        public static List<KaamShaam.AdminModels.LocalUser> GetNotApprovedUsers(string type)
        {
            using (var dbContext = new KaamShaamEntities())
            {
                var dbCats = dbContext.AspNetUsers.
                    Where(cat => cat.Type == type &&
                   ((bool) !cat.IsApproved || !cat.PhoneNumberConfirmed)).ToList();
                if (dbCats != null && dbCats.Any())
                {
                   return dbCats.Select(pbj => UserMapper.MapUser(pbj)).ToList()
                        .OrderByDescending( o => o.EditedAtObj).ToList();
                }               
                return new List<LocalUser>();
            }
        }
       

        public static AspNetUser ApprovalStatus(KaamShaam.AdminModels.LocalUser obj)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var dbo = dbcontext.AspNetUsers.FirstOrDefault(l => l.Id == obj.Id);
                if (dbo != null)
                {
                    dbo.IsApproved = obj.IsApproved;
                    dbo.PhoneNumberConfirmed = true;
                    dbo.Feedback = null;
                    if (!obj.IsApproved)
                    {
                        dbo.Feedback = obj.Feedback;
                    }
                    dbcontext.SaveChanges();
                }
                return dbo;
            }
        }

        public static bool IsUserApproved(string userName)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var dbo = dbcontext.AspNetUsers.FirstOrDefault(u=> u.UserName.ToLower()== userName.ToLower());
                if (dbo != null)
                {
                    return (bool)dbo.IsApproved && (bool) dbo.Status;
                }
                return false;
            }
        }
    }
}