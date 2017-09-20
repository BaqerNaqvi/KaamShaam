﻿using System;
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
        public static bool DeleteUser(AspNetUser userObj)
        {
            AspNetUser tempo=null;
            using (var context = new KaamShaamEntities())
            {
                tempo = context.AspNetUsers.FirstOrDefault(x => x.Id == userObj.Id);
                var contextnew = new ApplicationDbContext();
                var userStore = new UserStore<ApplicationUser>(contextnew);
                var userManager = new UserManager<ApplicationUser>(userStore);
                foreach (var role in tempo.AspNetRoles.ToList())
                {
                    userManager.RemoveFromRole(userObj.Id, role.Name);
                }
            }
            
            using (var context = new KaamShaamEntities())
            {               
                try
                {
                    var data = context.AspNetUsers.FirstOrDefault(x => x.Id == userObj.Id);
                    if (data != null)
                    {
                        #region User Deletion
                        if (data.Type == "User")
                        {
                            var jobsposted = context.Jobs.Where(j => j.PostedById == data.Id).ToList();
                            if (jobsposted.Any())
                            {
                                foreach (var jpost in jobsposted)
                                {
                                    if (jpost.JobHistories != null && jpost.JobHistories.Any())
                                    {
                                        foreach (var jhistory in jpost.JobHistories.ToList())
                                        {
                                            context.JobHistories.Remove(jhistory);
                                        }
                                    }
                                    context.Jobs.Remove(jpost);
                                }
                            }
                        }
                        #endregion
                        #region Contractor Deletion
                        if (data.Type == "Contractor")
                        {
                            var appliedJobs = context.JobHistories.Where(j => j.ContractorId == data.Id).ToList();
                            if (appliedJobs.Any())
                            {
                                foreach (var appJob in appliedJobs)
                                {
                                    context.JobHistories.Remove(appJob);
                                }
                            }
                        }
                        #endregion

                       var apps= context.Appointments.Where(app => app.CreatedBy == data.Id || app.WithId == data.Id).ToList();
                        if (apps.Any())
                        {
                            foreach (var app in apps)
                            {
                                context.Appointments.Remove(app);
                            }
                        }

                        context.AspNetUsers.Remove(data);
                        context.SaveChanges();
                        return true;
                    }
                }
                catch (System.Exception exp)
                {

                }
            }
            return false;
        }
        public static List<LocalUser> GetUsersByType(string type)
        {
            using (var context = new KaamShaamEntities())
            {
                var data = context.AspNetUsers.Where(u => u.Type == type).ToList()
                   .Select(user => user.MapUser()).ToList();
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
                        dbuser.EditedAt = DateTime.Now;

                    }
                    dbcontext.SaveChanges();
                }
            }
        }


        #region FeedBack
        public static FeedBack AddFeedback(GeneralFeedbackModel feedback)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var obj = new FeedBack
                {
                    Title = feedback.Title,
                    Description = feedback.Des,
                    PostedBy = feedback.PostedById,
                    Status = false,
                    IsApproved = false,
                    DateTime = DateTime.Now
                };
                dbcontext.FeedBacks.Add(obj);
                dbcontext.SaveChanges();
                return obj;
            }
        }

        public static List<GeneralFeedbackModel> GetAllFeedbacks()
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var feedback = dbcontext.FeedBacks.ToList().Select(fd => fd.MapFeedback()).ToList();
                return feedback;
            }
        }

        public static List<GeneralFeedbackModel> GetApprovedFeedbacks()
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var feedback = dbcontext.FeedBacks.Where(feed => feed.IsApproved && (bool) feed.Status).ToList().Select(fd => fd.MapFeedback()).ToList();
                return feedback;
            }
        }

        public static FeedBack ChangeFeedbackStatus(GeneralFeedbackModel feedback)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var dbobj = dbcontext.FeedBacks.FirstOrDefault(fd => fd.Id == feedback.Id);
                if (dbobj != null)
                {
                    dbobj.Status = feedback.Status;
                }
                dbcontext.SaveChanges();
                return dbobj;
            }
        }

        public static FeedBack ChangeFeedbackApproval(GeneralFeedbackModel feedback)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var dbobj = dbcontext.FeedBacks.FirstOrDefault(fd => fd.Id == feedback.Id);
                if (dbobj != null)
                {
                    dbobj.IsApproved = feedback.IsApproved;
                }
                dbcontext.SaveChanges();
                return dbobj;
            }
        }
        #endregion
    }
}