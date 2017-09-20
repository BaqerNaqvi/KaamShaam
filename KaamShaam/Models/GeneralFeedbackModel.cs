using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KaamShaam.DbEntities;

namespace KaamShaam.Models
{
    public class GeneralFeedbackModel
    {
        public long Id { get; set; }

        public string Title { get; set; }


        public string Des { get; set; }

        public string PostedById { get; set; }

        public string PostedByName { get; set; }

        public bool IsApproved { get; set; }

        public bool Status { get; set; }

        public string DateTime { get; set; }
    }

    public static class MapperFeedback
    {
        public static GeneralFeedbackModel MapFeedback(this FeedBack source)
        {
            return new GeneralFeedbackModel
            {
                Id = source.Id,
                Title = source.Title,
                Des = source.Description,
                PostedById = source.PostedBy,
                PostedByName = source.AspNetUser.FullName,
                IsApproved = source.IsApproved,
                Status = (bool) source.Status,
                DateTime = source.DateTime.ToShortDateString()
            };
        }
    }
    
}