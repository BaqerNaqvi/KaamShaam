using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using KaamShaam.DbEntities;
using KaamShaam.LocalModels;
using KaamShaam.Services;

namespace KaamShaam.AdminModels
{
    public class LocalUser
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Mobile { get; set; }
        [Required]
        public string CNIC { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string City { get; set; }
        public string Intro { get; set; }
        public string Language { get; set; }
        public bool Status { get; set; }

        public long? CategoryId { get; set; }
        public string ContractorId { get; set; }

        public string RoleName { get; set; }

        public string Password { get; set; } // for admin to change password

        public bool IsApproved { get; set; }
        public string Feedback { get; set; }

        public string LocationName { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string CatName { get; set; }

        [ScriptIgnore]
        public DbGeography Location { get; set; }
        public double DistanceFromOrigin { get; set; }
        [ScriptIgnore]
        public string EditedAt { get; set; }

        public List<LocalProfileVisit> ProfileVisits { get; set; }
        public List<LocalUserRating> UserRatings { get; set; }
        public double Score { get; set; }
        public bool CanRate { get; set; }

        public string LocTempo { get; set; }  // for api


    }
    public static class UserMapper
    {
        public static LocalUser MapUser(this AspNetUser source)
        {
            LocalCategory cat = null;
            if (source.CategoryId != 0)
            {
                cat = CategoryService.GetCategoryById(source.CategoryId);
            }
            var roleName = source.AspNetRoles.Any() ? source.AspNetRoles.FirstOrDefault().Name : "";

            var profileVists = new List<LocalProfileVisit>();
            if (source.ProfileVisitors != null && source.ProfileVisitors.Any())
            {
                profileVists = source.ProfileVisitors.Select(prf => prf.Mapper()).ToList();
            }

            var profileRatings = new List<LocalUserRating>();
            if (source.YourRatings != null && source.YourRatings.Any())
            {
                profileRatings = source.YourRatings.Where( df=> df.IsApproved).Select(prf => prf.Mapper()).ToList();
            }

            var score = UserRatingService.GetRatingsInFloat(source.Id);

            var tempoLoc = " 31.476535115002306_74.32158172130585";
            if (source.LocationCord != null)
            {
                tempoLoc = source.LocationCord.Latitude + "_" + source.LocationCord.Longitude;
            }

            var userObj= new LocalUser
            {
                CNIC = source.CNIC??"(Not Provided)",
                Email = source.Email?? "(Not Provided)",
                FullName = source.FullName?? "(Not Provided)", 
                Id = source.Id,
                Mobile = source.Mobile?? "(Not Provided)",
                Type = source.Type?? "(Not Provided)",
                UserName = source.UserName?? "(Not Provided)",
                Country = source.Country?? "(Not Provided)",
                City = source.City?? "(Not Provided)",

                LocTempo = tempoLoc,

                Intro = source.Intro?? "(Not Provided)",
                Language = source.Language?? "(Not Provided)",
                Status = (bool)source.Status,
                ContractorId = source.ContractorId?? "(Not Provided)",
                CategoryId = source.CategoryId??0,
                RoleName = roleName?? "(Not Provided)",
                IsApproved = (bool) source.IsApproved,
                Feedback = source.Feedback?? " - ",
                LocationName= source.LocationName?? "(Not Provided)",
                lat = (source.LocationCord.Latitude).ToString(),
                lng = (source.LocationCord.Longitude).ToString(),
                CatName = cat?.Name?? "(Not Provided)",
                Location = source.LocationCord,
                Password = source.PasswordHash,
                EditedAt = Convert.ToDateTime(source.EditedAt).ToLongDateString(),
                ProfileVisits = profileVists,
                Score = score,
                UserRatings = profileRatings,
                CanRate = false
            };
            return userObj;
        }
    }
}