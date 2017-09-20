using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using KaamShaam.AdminServices;
using KaamShaam.ApiModels;
using KaamShaam.LocalModels;
using KaamShaam.Models;
using KaamShaam.Services;

namespace KaamShaam.apis
{
    public class UserApiController : ApiController
    {
        [HttpPost]
        [Route("api/User/Profile")]
        public HttpResponseMessage ViewProfile(ContractorRequestModel model)
        {
            try
            {
                if (model == null || model.UserId == null)
                {
                    var responseError = new ApiResponseModel
                    {
                        Data = model,
                        Message = "Data is not mapped",
                        Success = false
                    };
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, responseError);
                }
               

                var contractor = UserServices.GetUserById(model.UserId);

                if (string.IsNullOrEmpty(model.VisitedBy))
                {
                    var isContractorInvoledInAnyJobOfCurrenUser =
                        contractor.JobHistories.Any(jh => jh.PostedById == model.VisitedBy);

                    var isProfileHitByUser = contractor.ProfileVisits.Any(pv => pv.VistedBy == model.VisitedBy || pv.VistedOf == model.VisitedBy);

                    bool canRate = isContractorInvoledInAnyJobOfCurrenUser || isProfileHitByUser;

                    contractor.CanRate = canRate;
                }

                var response = new ApiResponseModel
                {
                    Data = contractor,
                    Message = "Profile successfully sent",
                    Success = true
                };
                return Request.CreateResponse(HttpStatusCode.OK, response);
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