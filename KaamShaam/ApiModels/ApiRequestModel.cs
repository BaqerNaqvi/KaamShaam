using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KaamShaam.DbEntities;

namespace KaamShaam.ApiModels
{
    public class ApiRequestModel
    {
        #region job
        public long JobId { get; set; }  // in job api
        public string PostedById { get; set; }

        public string Feedback { get; set; }
        public string ContractorId { get; set; }

        #endregion

        #region Appointment

        public string UserId { get; set; }

        #endregion

        #region rating
        public double Rating { get; set; }
        public string Comments { get; set; }
        public string RatedTo { get; set; }
        public string RatedBy { get; set; }
        #endregion

        #region Profile Picture
        public string ProfilePic { get; set; } 
        #endregion

    }
}