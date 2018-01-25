using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using KaamShaam.ApiModels;
using KaamShaam.LocalModels;
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
                var allJobs = JobService.GetReadyJobs(null).ToList();
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

        [HttpPost]
        [Route("api/Job/GetProposals")]
        public HttpResponseMessage GetProposals(ApiRequestModel model)
        {
            try
            {
                #region validation
                if (model == null || model.PostedById == null)
                {
                    var responseError = new ApiResponseModel
                    {
                        Data = model,
                        Message = "Data is not mapped",
                        Success = false
                    };
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, responseError);
                } 
                #endregion
                var jobs = JobService.GetJobsProposals(model.PostedById);
                return Request.CreateResponse(HttpStatusCode.OK, new ApiResponseModel
                {
                    Success = true,
                    Message = "Successfully fetched all job proposals for the current user",
                    Data = jobs
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
        [Route("api/Job/FeedBack")]
        public HttpResponseMessage FeedBack(ApiRequestModel model)
        {
            try
            {
                #region not mapped
                if (model == null || string.IsNullOrEmpty(model.Comments) || string.IsNullOrEmpty(model.RatedBy) ||
                            string.IsNullOrEmpty(model.RatedTo) ||
                            Math.Abs(model.Rating) < 0.0)
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
                UserRatingService.AddRating(new LocalUserRating
                {
                    Comments = model.Comments,
                    RatedBy = model.RatedBy,
                    RatedTo = model.RatedTo,
                    Rating = model.Rating
                });

                var res = new ApiResponseModel
                {
                    Data = model,
                    Message = "Jon Feedback Posted.",
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


        [HttpPost]
        [Route("api/Job/History")]
        public HttpResponseMessage History(ApiRequestModel model)
        {
            try
            {
                #region not mapped
                if (model == null ||
                            model.JobId == 0)
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
                var history = JobService.GetJobById(model.JobId);

                var res = new ApiResponseModel
                {
                    Data = history,
                    Message = "Job fetched for history.",
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


        [HttpPost]
        [Route("api/Job/GetAllJobsForUser")]
        public HttpResponseMessage GetAll(ApiRequestModel model)
        {
            try
            {
                #region not mapped
                if (model == null ||
                            model.UserId == null)
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
                var history = JobService.GetUserJobs(model.UserId);

                var res = new ApiResponseModel
                {
                    Data = history,
                    Message = "Jobs fetched for user.",
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

        [HttpPost]
        [Route("api/Job/Apply")]
        public HttpResponseMessage ApplyJob(ApplyJobApiModel model)
        {
            try
            {
                #region not mapped
                if (model == null || string.IsNullOrEmpty(model.AppliedBy) || model.JobId <0.0)
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
                JobService.ApplyJob(new CustomJobHistory
                {
                    JobId = model.JobId,
                    PurposalText = "Applied via mobile app"
                }, model.AppliedBy);
                var res = new ApiResponseModel
                {
                    Data = model,
                    Message = "Job Applied.",
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

        [HttpPost]
        [Route("api/Job/Start")]
        public HttpResponseMessage AcceptProposal(AcceptProposalApiModel model)
        {
            try
            {
                #region not mapped
                if (model == null || string.IsNullOrEmpty(model.ContractorId) || model.JobId < 0.0)
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
                JobService.AcceptJobProposal(new CustomJobHistory
                {
                    JobId = model.JobId,
                    PurposalText = "Api job started",
                    ContractorId = model.ContractorId
                }, model.ContractorId);

                var res = new ApiResponseModel
                {
                    Data = model,
                    Message = "Job Started.",
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


        [HttpPost]
        [Route("api/Job/Close")]
        public HttpResponseMessage CloseJob(CloseJobApiModel model)
        {
            try
            {
                #region not mapped
                if (model == null || string.IsNullOrEmpty(model.Comments) || string.IsNullOrEmpty(model.RatedBy)
                    || string.IsNullOrEmpty(model.RatedTo)
                     || model.Rating < 0.0
                    || model.JobId < 0.0)
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

                #region obj
                var rating = new LocalUserRating
                {
                    DateTime = DateTime.Now,
                    IsApproved = false,
                    JobId = model.JobId,
                    Comments = model.Comments,
                    RatedBy = model.RatedBy,
                    RatedTo = model.RatedTo,
                    Rating = model.Rating
                }; 
                #endregion
              
                UserRatingService.AddRating(rating);
                if (rating.JobId > 0)
                {
                    JobService.MarkJobDone(rating.JobId, rating.RatedTo);
                }
                var res = new ApiResponseModel
                {
                    Data = model,
                    Message = "Job Closed.",
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


        [HttpPost]
        [Route("api/Job/Contractor/Previous")]
        public HttpResponseMessage GetContractorPreviousJobs(ApiRequestModel model)
        {
            try
            {
                #region not mapped
                if (model==null || string.IsNullOrEmpty(model.ContractorId))
                {
                    var response = new ApiResponseModel
                    {
                        Data = null,
                        Message = "Data is not mapped",
                        Success = false
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                #endregion

                var allJobs = JobService.GetPreviousJobsForContractor(model.ContractorId);
                return Request.CreateResponse(HttpStatusCode.OK, new ApiResponseModel
                {
                    Success = true,
                    Message = "Successfully fetched previous jobs for contractor",
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
