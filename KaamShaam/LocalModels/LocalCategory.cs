using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KaamShaam.DbEntities;

namespace KaamShaam.LocalModels
{
    public class LocalCategory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public bool IsApproved { get; set; }
        public string Feedback { get; set; }
        public int JobCount { get; set; }

        public int ContractorCount { get; set; }
        public string Image { get; set; }
        public string Icon { get; set; }
        public string EditedAt { get; set; }

    }

    public static class CategoryMapper
    {
        public static LocalCategory Mapper(this Category source)
        {
            return new LocalCategory
            {
                Id = source.Id,
                Name = source.Name,
                Status = source.Status,
                IsApproved = source.IsApproved,
                Feedback = source.Feedback,
                JobCount = source.Jobs.Count,
                Image = "/Images/NewCatsImages/"+source.Id+".png",
                Icon = "/Images/NewCatsIcons/" + source.Id + ".png",
                EditedAt = Convert.ToDateTime(source.EditedAt).ToLongDateString()
            };
        }
    }

}