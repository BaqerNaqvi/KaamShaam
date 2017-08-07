using KaamShaam.DbEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;
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

        public DbGeography Location { get; set; }
        public long? CategoryId { get; set; }
        public string ContractorId { get; set; }
        public string LocationName { get; set; }

        public string LocTempo { get; set; }  // for update loc info 

        public string CategoryName { get; set; }
        public string CompanyName { get; set; }

        public string ProfileImg { get; set; }

    }

    public static class UserMapper
    {
        public static LocalUser MapUser(this AspNetUser source)
        {
            if (source == null)
            {
                return null;
            }
            LocalCategory cat = null;
            if (source.CategoryId!=null)
            {
                cat=CategoryService.GetCategoryById(source.CategoryId);
            }
            LocalUser contract = null;
            if (source.ContractorId != null)
            {
                contract= UserServices.GetUserById(source.ContractorId); //company
            }
            
            return new LocalUser
            {
                CNIC = source.CNIC,
                Email = source.Email,
                FullName = source.FullName,
                Id = source.Id,
                Mobile = source.Mobile,
                Type = source.Type,
                UserName = source.UserName,
                Country = source.Country,
                City = source.City,
                Intro = source.Intro,
                Language = source.Language,

                Location = source.LocationCord,
                CategoryId = source.CategoryId,
                ContractorId =source.ContractorId, //company 
                LocationName= source.LocationName,
                CategoryName = cat?.Name,
                CompanyName= contract?.FullName

            };
        }
    }

  
}