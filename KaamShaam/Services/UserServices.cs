﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Net;
using System.Web;
using KaamShaam.DbEntities;
using KaamShaam.LocalModels;
using KaamShaam.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace KaamShaam.Services
{
    public static class UserServices
    {
        public static  void AddUserProperties(RegisterViewModel user, string userId)
        {
            var context = new ApplicationDbContext();
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            using (var dbContext = new KaamShaamEntities())
            {
                DbGeography loc = null;
                if (!String.IsNullOrEmpty(user.LocationCord) && user.LocationCord != "")
                {
                    var latlng = user.LocationCord.Split('_');
                    if (latlng.Length == 2)
                    {
                        loc = Commons.Commons.ConvertLatLonToDbGeography(latlng[1], latlng[0]); // lat _ lng
                    }
                }
                //https://cmatskas.com/working-with-dbgeography-points-and-polygons-in-net/
                var dbuser = dbContext.AspNetUsers.FirstOrDefault(u => u.Id == userId);
                if (dbuser != null)
                {
                    dbuser.FullName = user.FullName;
                  //  dbuser.UserName = user.FullName;
                    dbuser.Type = user.Type;
                    dbuser.Mobile = user.Mobile;
                    dbuser.CNIC = user.CNIC;
                    dbuser.LocationCord = loc;
                    dbuser.LocationName = user.LocationName;

                    //dbuser.ContractorId = user.ContractorId;
                    dbuser.CategoryId = user.CategoryId;

                    dbuser.Status = true;
                    dbuser.Language = "English";
                    dbuser.IsApproved = false;
                    dbuser.EditedAt = DateTime.Now;


                    if (user.Type == "Vendor" || user.Type == "User")
                    {
                        dbuser.ContractorId = null;
                        dbuser.CategoryId = null;
                        dbuser.IsApproved = true;
                    }
                    else
                    {
                        dbuser.Feedback = "Admin has not yet verified your account. Please contact admin.";
                    }
                    userManager.AddToRole(userId, user.Type);
                    dbContext.AspNetUsers.AddOrUpdate(dbuser);
                    dbContext.SaveChanges();
                }
            }
        }
        public static LocalUser GetUserById(string userId)
        {
            using (var dbContext = new KaamShaamEntities())
            {
                var dbuser = dbContext.AspNetUsers.FirstOrDefault(u => u.Id == userId);
                var model= dbuser.MapUser();
                return model;
            }
        }

        public static LocalUser GetUserByPhone(string phone)
        {
            using (var dbContext = new KaamShaamEntities())
            {
                var dbuser = dbContext.AspNetUsers.FirstOrDefault(u => u.Mobile == phone);
                var model = dbuser.MapUser();
                return model;
            }
        }
        public static AspNetUser UpdateBasicInfo(LocalUser user)
        {
            using (var dbContext = new KaamShaamEntities())
            {
                var dbuser = dbContext.AspNetUsers.FirstOrDefault(u => u.Id == user.Id);
                if (dbuser != null)
                {
                    //dbuser.FullName = user.FullName ?? dbuser.FullName;
                //    dbuser.Mobile = user.Mobile?? dbuser.Mobile;
                //    dbuser.CNIC = user.CNIC?? dbuser.CNIC;
                    dbuser.Email = user?.Email?? dbuser.Email;
                   // dbuser.IsApproved = !dbuser.AspNetRoles.Any( r=> r.Name.ToLower().Contains("contractor"));
                    dbuser.EditedAt = DateTime.Now;
                    dbuser.Feedback = null;


                    if (!string.IsNullOrEmpty(user.Mobile) && dbuser.Mobile != user.Mobile)
                    {

                        user.Mobile = user.Mobile.Substring(1).Replace("-", "");
                        user.Mobile = "92" + user.Mobile;


                        dbuser.Mobile = user.Mobile;
                        dbuser.PhoneNumberConfirmed = false;
                        dbuser.Feedback = "Verify your contact number or contact admin.";

                    }

                    if (!string.IsNullOrEmpty(user.FullName) && dbuser.FullName != user.FullName)
                    {
                        dbuser.FullName = user.FullName;
                        dbuser.IsApproved = false;
                        dbuser.Feedback = "Your updated name is not yet verified by the admin. Contact admin";
                    }

                    if (!string.IsNullOrEmpty(user.CNIC) && dbuser.CNIC != user.CNIC)
                    {
                        dbuser.CNIC = user.CNIC;
                        dbuser.IsApproved = false;
                        dbuser.Feedback = "CNIC is not yet verified by the admin. Contact admin";
                    }

                    dbContext.AspNetUsers.AddOrUpdate(dbuser);
                    dbContext.SaveChanges();
                }
                return dbuser;
            }
        }
        public static AspNetUser UpdateLocInfo(LocalUser user)
        {
            try
            {
                using (var dbContext = new KaamShaamEntities())
                {
                    var dbuser = dbContext.AspNetUsers.FirstOrDefault(u => u.Id == user.Id);
                    if (dbuser != null)
                    {
                        DbGeography loc = null;
                        if (!String.IsNullOrEmpty(user.LocTempo) && user.LocTempo != "")
                        {
                            var latlng = user.LocTempo.Split('_');
                            if (latlng.Length == 2)
                            {
                                loc = Commons.Commons.ConvertLatLonToDbGeography(latlng[1], latlng[0]); // lat _ lng
                            }
                        }
                        dbuser.Country = user.Country ?? "NA";
                        dbuser.City = user.City ?? "NA";
                        dbuser.LocationCord = loc;
                        dbuser.LocationName = user.LocationName;
                        //  dbuser.IsApproved = !dbuser.AspNetRoles.Any(r => r.Name.ToLower().Contains("contractor")); dbuser.EditedAt = DateTime.Now;

                        dbuser.Feedback = null;
                        dbContext.AspNetUsers.AddOrUpdate(dbuser);
                        dbContext.SaveChanges();
                        return dbuser;
                    }
                    return null;
                }
            }
            catch (Exception )
            {
                return null;
            }
        }
        public static void UpdateOtherInfo(LocalUser user)
        {
            using (var dbContext = new KaamShaamEntities())
            {
                var dbuser = dbContext.AspNetUsers.FirstOrDefault(u => u.Id == user.Id);
                if (dbuser != null)
                {                    
                    dbuser.Intro = user.Intro;
                    dbuser.Language = user.Language;
                 //   dbuser.IsApproved = !dbuser.AspNetRoles.Any(r => r.Name.ToLower().Contains("contractor"));
                    dbuser.EditedAt = DateTime.Now;
                    dbuser.Feedback = null;
                    dbContext.SaveChanges();
                }
            }
        }
        public static void UpdateCompanyInfo(LocalUser user)
        {
            using (var dbContext = new KaamShaamEntities())
            {
                var dbuser = dbContext.AspNetUsers.FirstOrDefault(u => u.Id == user.Id);
                if (dbuser != null)
                {
                    dbuser.CategoryId = user.CategoryId;
                    dbuser.ContractorId = user.ContractorId;
                 //   dbuser.IsApproved =! dbuser.AspNetRoles.Any(r => r.Name.ToLower().Contains("contractor"));
                    dbuser.EditedAt = DateTime.Now;

                    dbuser.Feedback = null;
                    dbContext.SaveChanges();
                }
            }
        }
        public static List<LocalUser> GetUserByType(string userType)
        {
            try
            {
                using (var dbContext = new KaamShaamEntities())
                {
                    var dbuser = dbContext.AspNetUsers.Where(u => u.Type == userType).ToList().Select(obj => obj.MapUser()).ToList();
                    return dbuser;
                }
            }
            catch (Exception sdsd)
            {
                return null;
            }
        }
        public static List<UserDrowdownModel> GetUserTypeDd(string userType)
        {
            using (var dbContext = new KaamShaamEntities())
            {
                var dbuser = dbContext.AspNetUsers.Where(u => u.Type == userType).ToList().Select(obj => obj.MappDD()).ToList();
                return dbuser;
            }
        }

        public static List<LocalUser> GetContractorCategoryId(long catId)
        {
            if (catId != null)
            {
                using (var dbContext = new KaamShaamEntities())
                {
                    var dbusers = dbContext.AspNetUsers
                        .Where(u => u.CategoryId == catId && u.Type == "Contractor" && 
                        (bool) u.Status && (bool) u.IsApproved).ToList();
                    return dbusers.Select(d=>d.MapUser()).ToList();
                }
            }
            return null;
        }

        public static void CreateUserAvatar(string path)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFile("http://www.pftec.com/wp-content/uploads/2015/03/default_user.png", path + "_110.png");
            webClient.DownloadFile("http://www.pftec.com/wp-content/uploads/2015/03/default_user.png", path + "_35.png");
        }
    }
}