using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KaamShaam.DbEntities;

namespace KaamShaam.LocalModels
{
    public class LocalUserRating
    {
        public long Id { get; set; }
        public double Rating { get; set; }
        public string Comments { get; set; }
        public string RatedTo { get; set; }
        public string RatedBy { get; set; }
        public System.DateTime DateTime { get; set; }
        public bool IsApproved { get; set; }
        public long JobId { get; set; }
        public string EditedAt { get; set; }

        public virtual LocalUser RatedByUser { get; set; }
        public virtual LocalUser RatedToUser { get; set; }

        public string RatedByUserFullName { get; set; }
        public string RatedToUserFullName { get; set; }
        public string RatedByUserAddress { get; set; }

        public string TempoRating { get; set; }
    }

    public static class RatingMapper
    {
        public static LocalUserRating Mapper(this UserRating source)
        {
            var obj = new LocalUserRating
            {
                DateTime = source.DateTime,
                Id = source.Id,
                IsApproved = source.IsApproved,
                JobId = source.JobId,
                EditedAt = source.EditedAt.ToLongDateString(),
                Comments = source.Comments,
                RatedBy = source.RatedBy,
                RatedByUser = new LocalUser(),
                RatedTo = source.RatedTo,
                RatedToUser = new LocalUser(),
                Rating = source.Rating,
                RatedByUserFullName = source.RatedByUser.FullName,
                RatedByUserAddress= source.RatedByUser.LocationName,
                RatedToUserFullName = source.RatedToUser.FullName
                
            };
            return obj;
        }
    }
}