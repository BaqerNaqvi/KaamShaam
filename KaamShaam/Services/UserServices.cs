using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Spatial;
using System.Linq;
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
                if (!string.IsNullOrEmpty(user.LocationCord) && user.LocationCord != "")
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
                    dbuser.IsApproved = true;

                    if (user.Type == "Vendor" || user.Type == "User")
                    {
                        dbuser.ContractorId = null;
                        dbuser.CategoryId = null;
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
        public static AspNetUser UpdateBasicInfo(LocalUser user)
        {
            using (var dbContext = new KaamShaamEntities())
            {
                var dbuser = dbContext.AspNetUsers.FirstOrDefault(u => u.Id == user.Id);
                if (dbuser != null)
                {
                    dbuser.FullName = user.FullName;
                    dbuser.Mobile = user.Mobile;
                    dbuser.CNIC = user.CNIC;
                    dbuser.Email = user.Email;
                    dbuser.IsApproved = false;
                    dbuser.Feedback = null;
                    dbContext.AspNetUsers.AddOrUpdate(dbuser);
                    dbContext.SaveChanges();
                }
                return dbuser;
            }
        }
        public static AspNetUser UpdateLocInfo(LocalUser user)
        {
            using (var dbContext = new KaamShaamEntities())
            {
                var dbuser = dbContext.AspNetUsers.FirstOrDefault(u => u.Id == user.Id);
                if (dbuser != null)
                {
                    DbGeography loc = null;
                    if (!string.IsNullOrEmpty(user.LocTempo) && user.LocTempo != "")
                    {
                        var latlng = user.LocTempo.Split('_');
                        if (latlng.Length == 2)
                        {
                            loc = Commons.Commons.ConvertLatLonToDbGeography(latlng[1], latlng[0]); // lat _ lng
                        }
                    } 
                    dbuser.Country = user.Country;
                    dbuser.City = user.City;
                    dbuser.LocationCord = loc;
                    dbuser.LocationName = user.LocationName;
                    dbuser.IsApproved = false;
                    dbuser.Feedback = null;
                    dbContext.AspNetUsers.AddOrUpdate(dbuser);
                    dbContext.SaveChanges();
                }
                return dbuser;
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
                    dbuser.IsApproved = false;
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
                    dbuser.IsApproved = false;
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


    }
}