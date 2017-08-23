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
        public double? lat { get; set; }
        public double? lng { get; set; }
        public string CatName { get; set; }

        [ScriptIgnore]
        public DbGeography Location { get; set; }
        public int DistanceFromOrigin { get; set; }
    }
    public static class UserMapper
    {
        public static LocalUser MapUser(this AspNetUser source)
        {
            LocalCategory cat = null;
            if (source.CategoryId != null)
            {
                cat = CategoryService.GetCategoryById(source.CategoryId);
            }
            var roleName = source.AspNetRoles.Any() ? source.AspNetRoles.FirstOrDefault().Name : "";
            return new LocalUser
            {
                CNIC = source.CNIC??"(No Val)",
                Email = source.Email?? "(No Val)",
                FullName = source.FullName?? "(No Val)", 
                Id = source.Id,
                Mobile = source.Mobile?? "(No Val)",
                Type = source.Type?? "(No Val)",
                UserName = source.UserName?? "(No Val)",
                Country = source.Country?? "(No Val)",
                City = source.City?? "(No Val)",
                
                Intro = source.Intro?? "(No Val)",
                Language = source.Language?? "(No Val)",
                Status = (bool)source.Status,
                ContractorId = source.ContractorId?? "(No Val)",
                CategoryId = source.CategoryId??0,
                RoleName = roleName?? "(No Val)",
                IsApproved = (bool) source.IsApproved,
                Feedback = source.Feedback?? "(No Val)",
                LocationName= source.LocationName?? "(No Val)",
                lat = source.LocationCord.Latitude??0,
                lng = source.LocationCord.Longitude??0,
                CatName = cat?.Name?? "(No Val)",
                Location = source.LocationCord,
                Password = source.PasswordHash
            };
        }
    }
}