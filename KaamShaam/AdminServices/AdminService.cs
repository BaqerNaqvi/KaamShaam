using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Security;
using KaamShaam.AdminModels;
using KaamShaam.Controllers;
using KaamShaam.DbEntities;
using KaamShaam.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using UserMapper = KaamShaam.LocalModels.UserMapper;

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
        public static List<LocalUser> GetAdminUsers()
        {
            using (var context = new KaamShaamEntities())
            {
                var adminUsers = context.AspNetUsers.Where( user => user.AspNetRoles.Any(role => role.Name=="Admin" || role.Name == "Super Admin")).ToList();
                return adminUsers.Select(admin => admin.MapUser()).ToList();
            }
        }
        public static LocalUser FindUserByUsername(string username)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var userObj=  dbcontext.AspNetUsers.FirstOrDefault(user => user.UserName.ToLower() == username.ToLower());
                if (userObj != null)
                {
                    return userObj.MapUser();
                }
                return null;
            }
        }
        public static LocalUser AddUserToRole(MakeAdminModel adminModel)
        {

            using (var dbcontext = new KaamShaamEntities())
            {
                var context = new ApplicationDbContext();
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var userObj = dbcontext.AspNetUsers.FirstOrDefault(user => user.Id.ToLower() == adminModel.Id);
                try
                {
                    if (userObj != null)
                    {
                        foreach (var role in userObj.AspNetRoles.ToList())
                        {
                            userManager.RemoveFromRole(userObj.Id, role.Name);
                        }
                        userManager.AddToRole(userObj.Id, adminModel.Role);
                    }
                }
                catch (System.Exception xfdf)
                {

                }
                return userObj?.MapUser();
            }
        }
        public static void RemoveUserFromRole(MakeAdminModel adminModel)
        {
            var context = new ApplicationDbContext();
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            userManager.RemoveFromRole(adminModel.Id, adminModel.Role);
        }
        public static void EditUserByAdmin(LocalUser user)
        {
            var cont = new AccountController();
            using (var dbcontext = new KaamShaamEntities())
            {
                var dbuser = dbcontext.AspNetUsers.FirstOrDefault(obj => obj.Id == user.Id);
                if (dbuser != null)
                {
                    if (!string.IsNullOrEmpty(user.Mobile))
                    {
                        dbuser.Mobile = user.Mobile;
                        dbuser.Feedback = null;
                        dbuser.IsApproved = false;

                    }
                    dbcontext.SaveChanges();
                }
            }
        }
    }
}