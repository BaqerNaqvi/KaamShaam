using KaamShaam.DbEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using KaamShaam.Models;
using KaamShaam.Services;
using Microsoft.SqlServer.Types;

namespace KaamShaam.LocalModels
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

        [ScriptIgnore]
        public DbGeography Location { get; set; }
        public long? CategoryId { get; set; }
        public string ContractorId { get; set; }
        public string LocationName { get; set; }

        public string LocTempo { get; set; }  // for update loc info 

        public string CategoryName { get; set; }
        public string CompanyName { get; set; }

        public string ProfileImg { get; set; }


        public List<LocalProfileVisit> ProfileVisits { get; set; }

        public List<LocalUserRating> UserRatings { get; set; }

        public double Score { get; set; }


        public List<CustomJobHistory> JobHistories { get; set; }

        public bool CanRate { get; set; }

        public List<string> Roles { get; set; }

    }

    public static class UserMapper
    {
        public static LocalUser MapUser(this AspNetUser source)
        {
            if (source == null)
            {
                return null;
            }

            var userRoles = new List<string>();
            try
            {
                if (source.AspNetRoles != null && source.AspNetRoles.Any())
                {
                    userRoles = source.AspNetRoles.Select(ro => ro.Name).ToList();
                }
            }
            catch (Exception gf){}

            LocalCategory cat = null;
            if (source.CategoryId!=null && source.CategoryId != 0)
            {
                cat=CategoryService.GetCategoryById(source.CategoryId);
            }
            LocalUser contract = null;
            if (!string.IsNullOrEmpty(source.ContractorId))
            {
                contract= UserServices.GetUserById(source.ContractorId); //company
            }

            var profileVists = new List<LocalProfileVisit>();
            try
            {
                if (source.ProfileVisitors != null && source.ProfileVisitors.Any())
                {
                    profileVists = source.ProfileVisitors.Select(prf => prf.Mapper()).ToList();
                }
            }
            catch (Exception sds)  {}

            var profileRatings = new List<LocalUserRating>();
            try
            {
                if (source.YourRatings != null && source.YourRatings.Any())
                {
                    profileRatings = source.YourRatings.Where(df => df.IsApproved).Select(prf => prf.Mapper()).ToList();
                }
            }
            catch (Exception)  { }

            var score = UserRatingService.GetRatingsInFloat(source.Id);

            var jobHistories = new List<CustomJobHistory>();
            try
            {
                if (source.JobHistories != null && source.JobHistories.Any())
                {
                    jobHistories = source.JobHistories.Select(prf => prf.Mapper()).ToList();
                }
            }
            catch (Exception) { }

            var tempoLoc = " 31.476535115002306_74.32158172130585";
            if (source.LocationCord != null)
            {
                tempoLoc = source.LocationCord.Latitude + "_" + source.LocationCord.Longitude;
            }

            return new LocalUser
            {
                

                Location = source.LocationCord,
               
                CategoryName = cat?.Name??"",
                CompanyName= contract?.FullName??"",
                ProfileVisits = profileVists,
                LocTempo = tempoLoc,
                Score = score,
                UserRatings = profileRatings,
                ProfileImg = "",


                CNIC = source.CNIC ?? "(Not Provided)",
                Email = source.Email ?? "(Not Provided)",
                FullName = source.FullName ?? "(Not Provided)",
                Id = source.Id,
                Mobile = source.Mobile ?? "(Not Provided)",
                Type = source.Type ?? "(Not Provided)",
                UserName = source.UserName ?? "(Not Provided)",
                Country = source.Country ?? "(Not Provided)",
                City = source.City ?? "(Not Provided)",

                Intro = source.Intro ?? "(Not Provided)",
                Language = source.Language ?? "(Not Provided)",
                ContractorId = source.ContractorId ?? "(Not Provided)",
                CategoryId = source.CategoryId ?? 0,
                LocationName = source.LocationName ?? "(Not Provided)",
                JobHistories = jobHistories,
                CanRate = false,
                Roles = userRoles
            };
        }
    }

  
}