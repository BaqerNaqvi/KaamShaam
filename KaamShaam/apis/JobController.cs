using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using KaamShaam.ApiModels;
using KaamShaam.Models;
using KaamShaam.Services;
using Microsoft.AspNet.Identity;

namespace KaamShaam.apis
{
    public class JobController : ApiController
    {
        [HttpPost]
        [Route("api/Job/Get")]
        public HttpResponseMessage GetJobs()
        {
            try
            {
                var allJobs = JobService.GetAllJobs(true);
                return Request.CreateResponse(HttpStatusCode.OK, new ApiResponseModel
                {
                    Success = true,
                    Message = "Successfully fetched jobs",
                    Data = allJobs
                });
            }
            catch (Exception excep)
            {
                var response = new ApiResponseModel
                {
                    Data = null,
                    Message = excep.InnerException.Message,
                    Success = false
                };
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }


        [HttpPost]
        [Route("api/Job/Post")]
        public HttpResponseMessage PostJob(CustomJobModel model)
        {
            try
            {
                #region not mapped
                if (model == null || string.IsNullOrEmpty(model.JobTitle) || string.IsNullOrEmpty(model.PostedById) ||
                            string.IsNullOrEmpty(model.Fee) || string.IsNullOrEmpty(model.LocationCords) ||
                            string.IsNullOrEmpty(model.LocationName) || string.IsNullOrEmpty(model.Mobile) ||
                                string.IsNullOrEmpty(model.Email) || model.CategoryId == 0)
                {
                    var response = new ApiResponseModel
                    {
                        Data = null,
                        Message = "Data is not mapped",
                        Success = false
                    };
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                } 
                #endregion
                var jobresponse= JobService.AddJob(model);
                var res = new ApiResponseModel
                {
                    Data = jobresponse,
                    Message = "Jon Posted. Find job id in Data",
                    Success = true
                };
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception excep)
            {
                var response = new ApiResponseModel
                {
                    Data = null,
                    Message = excep.InnerException.Message,
                    Success = false
                };
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
    }
}
