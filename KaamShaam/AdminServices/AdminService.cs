using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using KaamShaam.AdminModels;
using KaamShaam.DbEntities;

namespace KaamShaam.AdminServices
{
    public static class AdminService
    {
        public static List<LocalUser> GetAllUsers()
        {
            using (var context = new KaamShaamEntities())
            {
                var data = context.AspNetUsers.ToList().Select(user => user.MapUser()).ToList();
                return data;
            }
        }

        public static void UpdateStatus(AspNetUser obj)
        {
            using (var context = new KaamShaamEntities())
            {
                var data = context.AspNetUsers.FirstOrDefault(x => x.Id == obj.Id);
                if (data != null)
                {
                    data.Status = obj.Status;
                }
                context.AspNetUsers.AddOrUpdate(data);
                context.SaveChanges();
            }
        }

        public static void DeleteUser(AspNetUser obj)
        {
            using (var context = new KaamShaamEntities())
            {
                var data = context.AspNetUsers.FirstOrDefault(x => x.Id == obj.Id);
                if (data != null)
                {
                    context.AspNetUsers.Remove(data);
                    context.SaveChanges();
                }

            }
        }

        public static List<LocalUser> GetUsersByType(string type)
        {
            using (var context = new KaamShaamEntities())
            {
                var data = context.AspNetUsers.Where(u => u.Type == type).ToList().Select(user => user.MapUser()).ToList();
                return data;
            }
        }
    }
}