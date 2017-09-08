using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using KaamShaam.ApiModels;
using KaamShaam.Services;

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
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiResponseModel
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
    }
}
