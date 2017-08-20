using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
                    ContractorName = source.AspNetUser.FullName,
                    JobStatus = source.JobStatus,
                    JobStartDate = source.JobStartDate,
                    JobEndDate = source.JobEndDate,
                    JobId = source.JobId,
                    PurposalText= source.PurposalText,
                   // JobModel = source.Job.Mapper()
                };
                return obj;
            }
            return null;
        }
    }
}