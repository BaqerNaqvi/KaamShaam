using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json.Linq;
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
using KaamShaam.Models;
using KaamShaam.Services;

namespace KaamShaam.apis
{
    public class AccountApiController : ApiController
    {
        [HttpPost]
        [Route("api/User/Registration")]
        public async Task<HttpResponseMessage> RegisterUser(RegisterViewModel model)
        {
            try
            {
                #region Validation
                HttpResponseMessage endResponse;
                var response = new ApiResponseModel { Data = model };
                if (model == null || !ModelState.IsValid)
                {
                    response.Message = "Mandatory data is missing. ";
                    foreach (var error in ModelState)
                    {
                        response.Message = response.Message + "\n" + error.Key;
                    }
                    response.Success = false;
                    endResponse = Request.CreateResponse(HttpStatusCode.BadRequest, response);
                    return endResponse;
                }
                if (model.Type == "Contractor" && string.IsNullOrEmpty(model.CNIC))
                {
                    response.Message = "CNIC is missing. ";
                    response.Success = false;
                    endResponse = Request.CreateResponse(HttpStatusCode.BadRequest, response);
                    return endResponse;
                }
                if (model.Type == "Contractor" && model.CategoryId == 0)
                {
                    response.Message = "Category is missing for contractor ";
                    response.Success = false;
                    endResponse = Request.CreateResponse(HttpStatusCode.BadRequest, response);
                    return endResponse;
                }
                if (model.LocationCord == null || string.IsNullOrEmpty(model.LocationName))
                {
                    response.Message = "LocationCord(lat_lng)/LocationName is missing ";
                    response.Success = false;
                    endResponse = Request.CreateResponse(HttpStatusCode.BadRequest, response);
                    return endResponse;
                } 
                #endregion
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var usermanager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var result = await usermanager.CreateAsync(user, model.Password);
                var isUserCreated = result.Succeeded;
                if (isUserCreated)
                {
                    try
                    {
                        response.Message = model.FullName + " has been registered";
                        response.Success = true;
                        UserServices.AddUserProperties(model, user.Id);
                    }
                    catch (Exception excep)
                    {
                        var foundUser =  usermanager.FindById(user.Id);
                        var responseMesg = await usermanager.DeleteAsync(user);
                        if (responseMesg.Succeeded)
                        {
                            // user is deleted
                        }
                        response.Message = "Data is missing."+excep.InnerException.Message;
                        response.Success = false;
                    }
                   
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        response.Message = response.Message + "\n" + error;
                    }
                    response.Success = false;
                }
                endResponse = Request.CreateResponse(HttpStatusCode.OK, response);
                return endResponse;
            }
            catch (Exception excep)
            {
                var response = new ApiResponseModel
                {
                    Data = model,
                    Message = excep.InnerException.Message,
                    Success = false
                };
                var endResponse = Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                return endResponse;
            }
        }

        [HttpPost]
        [Route("api/User/Login")]
        public async Task<HttpResponseMessage> LoginUser(LoginViewModel model)
        {
            try
            {
                HttpResponseMessage endResponse;
                var response = new ApiResponseModel {Data = model};
                if (model == null)
                {
                    response.Success = false;
                    response.Message = "Mandatory data fields are missing/not mapped or not in right format";
                    endResponse = Request.CreateResponse(HttpStatusCode.BadRequest, response);
                    return endResponse;
                }
                var isApproved = UserAdminService.IsUserApproved(model.Email);
                if (!isApproved)
                {
                    response.Message = "Account is not approved by Admin.";
                    response.Success = false;

                }
                else
                {
                    var signInManager = HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
                    var result =
                        await
                            signInManager.PasswordSignInAsync(model.Email, model.Password,
                                true,
                                shouldLockout: false);
                    switch (result)
                    {
                        case SignInStatus.Success:
                        {
                            response.Message = "Logged-in successfully";
                            response.Success = true;
                            response.JToken = "a%&@JK*@#CG|wJ";
                            break;
                        }
                        default: /* Optional */
                        {
                            response.Message = "Invalid Username/Password.";
                            response.Success = false;
                            response.JToken = null;
                            break;
                        }
                    }
                }
                endResponse = Request.CreateResponse(HttpStatusCode.OK, response);
                return endResponse;
            }
            catch (Exception excep)
            {
                var response = new ApiResponseModel
                {
                    Data = model,
                    Message = excep.InnerException.Message,
                    Success = false
                };
                var endResponse = Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                return endResponse;
            }
        }
    }
}
