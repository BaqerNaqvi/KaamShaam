using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using KaamShaam.DbEntities;
using KaamShaam.LocalModels;

namespace KaamShaam.Models
{

    public class CustomJobModel
    {
        
        public long Id { get; set; }
        [Required]
        [Display(Name = "Job Title & Description ")]
        public string JobTitle { get; set; }
        public string LocationCords { get; set; }
        public string LocationName { get; set; }

        [Required]
        [Display(Name = "Category")]
        public long CategoryId { get; set; }

        public string CatName { get; set; }
     
        public string Fee { get; set; }
        [Required]
        public string Mobile { get; set; }
        [EmailAddress]
        public string Email { get; set; }

        public string PostedById { get; set; }

        public List<LocalCategory> Cats { get; set; }

        public string PostingDate { get; set; }

        public DateTime PostingDateObj { get; set; }

        public bool UserStatus { get; set; }

        public bool AdminStatus { get; set; }

        public string JobPostedBy { get; set; }

        public string Feedback { get; set; }

        public bool IsApproved { get; set; }


        public List<CustomJobHistory> JobHistory { get; set; }

        public string PurposalText { get; set; } // being when apply job
        public bool IfIApplied { get; set; }

        public string ContractorId { get; set; }  // for job proposal

        public string ContractorName { get; set; }  // for job proposal

        public double lat { get; set; }
        public double lng { get; set; }

        [ScriptIgnore]
        public LocalUser JobPostedByObj { get; set; }

        public bool CanRate { get; set; }
        public int JobStatus { get; set; }


        public string RatingStarForConForPrevJob { get; set; }

        public string RatingStringForConForPrevJob { get; set; }
    }

    public static class JobMapper
    {
        public static CustomJobModel Mapper(this Job source)
        {
            return new CustomJobModel
            {
                Id = source.Id,
                CategoryId = source.CategoryId,
                CatName = source.Category.Name,
                JobTitle = source.JobTitle,
                Email = source.Email,
                Mobile = source.Mobile,
                LocationName = source.LocationName,
                Fee = source.Fee.ToString(),
                PostedById = source.PostedById,
                IsApproved = source.IsApproved,
                PostingDateObj = source.PostingDate,
                UserStatus = source.UserStstus,
                AdminStatus = source.AdminStatus,
                JobPostedBy = source.AspNetUser.FullName,
                Feedback= source.FeedBack,
                PostingDate = source.PostingDate.ToShortDateString()+" "+ source.PostingDate.ToShortTimeString(),
                JobHistory = source.JobHistories?.Select(j => j.Mapper()).ToList(),
                lat = (double) source.Location.Latitude,
                lng = (double) source.Location.Longitude,
                JobPostedByObj = source.AspNetUser.MapUser(),
                RatingStarForConForPrevJob = "(NA)",
                RatingStringForConForPrevJob="(NA)"
            };
        }
    }
}