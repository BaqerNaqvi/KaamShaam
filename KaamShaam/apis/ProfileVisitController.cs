using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using KaamShaam.ApiModels;
using KaamShaam.LocalModels;
using KaamShaam.Services;

namespace KaamShaam.apis
{
    public class ProfileVisitController : ApiController
    {
        [HttpPost]
        [Route("api/ProfileVisit/Set")]
        public HttpResponseMessage GetProposals(LocalProfileVisit model)
        {
            try
            {
                #region validation
                if (model == null || model.VistedOf == null || model.VistedBy==null)
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

                var res = ProfileVisitorService.AddLocalVisit(model);
                return Request.CreateResponse(HttpStatusCode.OK, new ApiResponseModel
                {
                    Success = true,
                    Message = "Successfully add profile visit",
                    Data = res
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
