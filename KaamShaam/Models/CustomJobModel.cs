using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
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

        [Required]
        public string Fee { get; set; }
        [Required]
        public string Mobile { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string PostedById { get; set; }

        public List<LocalCategory> Cats { get; set; }

        public string PostingDate { get; set; }

        public DateTime PostingDateObj { get; set; }

        public bool Status { get; set; }

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
                PostingDateObj = source.PostingDate,
                Status = source.Ststus,
                PostingDate= source.PostingDate.ToShortDateString()+" "+ source.PostingDate.ToShortTimeString()
            };
        }
    }
}