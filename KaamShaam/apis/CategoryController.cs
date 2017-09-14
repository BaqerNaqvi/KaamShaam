using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using KaamShaam.AdminServices;
using KaamShaam.ApiModels;
using KaamShaam.LocalModels;
using KaamShaam.Services;
using Microsoft.AspNet.Identity.Owin;

namespace KaamShaam.apis
{
    public class CategoryController : ApiController
    {
        [HttpPost]
        [Route("api/Category/Get")]
        public HttpResponseMessage GetCategories()
        {
            try
            {
                var allCats = CategoryService.GetAllCategories();
                return Request.CreateResponse(HttpStatusCode.OK, new ApiResponseModel
                {
                    Success = true,
                    Message = "Successfully fetched categories",
                    Data = allCats
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
