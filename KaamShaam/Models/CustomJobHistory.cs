using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using KaamShaam.DbEntities;

namespace KaamShaam.Models
{
    public class CustomJobHistory
    {
        public long Id { get; set; }
        public long JobId { get; set; }
        public string ContractorId { get; set; }
        public System.DateTime JobStartDate { get; set; }
        public System.DateTime JobEndDate { get; set; }
        public string Feedback { get; set; }
        public int JobStatus { get; set; }

        public string ContractorName { get; set; }

        public CustomJobModel JobModel { get; set; }
        public string PurposalText { get; set; }

        #region for contracor profile
        public string JobTitle { get; set; }

        public string Des { get; set; }

        public string Fee { get; set; }

        public string PostedById { get; set; }
        #endregion

    }

    public static class JobHistoryMapper
    { 
        public static CustomJobHistory Mapper(this JobHistory source)
        {
            if (source != null)
            {
                var obj = new CustomJobHistory
                {
                    Feedback = source.Feedback,
                    Id = source.Id,
                    ContractorId = source.ContractorId,
                    PostedById = source.Job.PostedById,
                    ContractorName = source.AspNetUser.FullName,
                    JobStatus = source.JobStatus,
                    JobStartDate = source.JobStartDate,
                    JobEndDate = source.JobEndDate,
                    JobId = source.JobId,
                    PurposalText= source.PurposalText,
                    JobTitle = source.Job.Category.Name,
                    Des = source.Job.JobTitle,
                    Fee = source.Job.Fee.ToString()
                   // JobModel = source.Job.Mapper()
                };
                return obj;
            }
            return null;
        }
    }
}