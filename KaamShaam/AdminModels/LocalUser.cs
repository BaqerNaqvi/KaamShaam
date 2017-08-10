using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KaamShaam.DbEntities;

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
    }
    public static class UserMapper
    {
        public static LocalUser MapUser(this AspNetUser source)
        {
            var roleName = source.AspNetRoles.Any() ? source.AspNetRoles.FirstOrDefault().Name : "";
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
                Status = (bool)source.Status,
                ContractorId = source.ContractorId,
                CategoryId = source.CategoryId,
                RoleName = roleName
            };
        }
    }
}