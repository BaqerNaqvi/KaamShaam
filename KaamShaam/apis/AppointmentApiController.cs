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
    public class AppointmentApiController : ApiController
    {
        [HttpPost]
        [Route("api/Appointment/Get")]
        public HttpResponseMessage GetProposals(ApiRequestModel model)
        {
            try
            {
                #region validation
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
                #endregion
                var apps = AppointmentService.GetUserAppoinments(model.UserId);
                return Request.CreateResponse(HttpStatusCode.OK, new ApiResponseModel
                {
                    Success = true,
                    Message = "Successfully fetched all Appointments for the current user",
                    Data = apps
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