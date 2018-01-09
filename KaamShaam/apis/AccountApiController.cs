using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
                        var baseUrlforimg = System.Web.Hosting.HostingEnvironment.MapPath("/Images/Profiles/");
                        UserServices.CreateUserAvatar(baseUrlforimg + user.Id );
                        response.Data = user;

                        var content = "Hi " + model.FullName +
                                    "!\nYou have been successfully registered as a " + model.Type +
                                    " at KamSham.Pk.";
                        if (model.Type == "Contractor")
                        {
                            content = content + "You will be able to login once we approve your account information.";
                        }
                        content = content + "\n-KamSham Team\n+923084449991";
                        KaamShaam.Services.EmailService.SendEmail(user.Email, "Registration Notification | KamSham.Pk", content);

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
                if (model == null || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.Email))
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

                    #region Check credential of unapproved user
                    var signInManager = HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
                    var result =
                        await
                            signInManager.PasswordSignInAsync(model.Email, model.Password,
                                true,
                                shouldLockout: false);
                    switch (result)
                    {
                        case SignInStatus.Failure:
                        {
                            response.Message = "Invalid Username/Password.";
                            break;
                        }
                    } 
                    #endregion
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
                            var usermanager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                            var uid = usermanager.FindByEmail(model.Email).Id;
                            response.Message = "Logged-in successfully";
                            response.Success = true;
                            response.JToken = "a%&@JK*@#CG|wJ";
                            response.UserId = uid;
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

       
        [Route("api/User/UploadImage")]
        public HttpResponseMessage PostImage(ApiRequestModel model)
        {
            if (model == null ||  string.IsNullOrEmpty(model.UserId) ||  string.IsNullOrEmpty(model.ProfilePic))
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiResponseModel
                {
                    Success = false,
                    Message = "Data not mapped",
                    Data = model
                });
            }
            try
            {
                string Pic_Path = System.Web.HttpContext.Current.Server.MapPath("~\\Images\\Profiles\\" + model.UserId + ".png");
                using (FileStream fs = new FileStream(Pic_Path, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        byte[] data = Convert.FromBase64String(model.ProfilePic);
                        bw.Write(data);
                        bw.Close();
                    }
                }

                var baseUrl = System.Web.HttpContext.Current.Server.MapPath("~/Images/Profiles/");
                AppUtils.Common.GenerateImages(model.UserId, baseUrl);

                //for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                //{
                //    var file = HttpContext.Current.Request.Files[i]; //Uploaded file
                //                                                     //To save file, use SaveAs method
                //    var pathForProfileImage = System.Web.Hosting.HostingEnvironment.MapPath("/Images/Profiles/");
                //    file.SaveAs(pathForProfileImage + model.UserId + ".png");
                //    AppUtils.Common.GenerateImages(model.UserId, pathForProfileImage);

                //}
                return Request.CreateResponse(HttpStatusCode.OK, new ApiResponseModel
                {
                    Success = true,
                    Message = "Successfully User avatar updated",
                    Data = null
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


        [Route("api/User/UploadProfile")]
        public HttpResponseMessage UpdateProfile(LocalUser model)
        {
            if (model == null || model.FullName == null || model.Mobile == null || model.CNIC == null 
                || model.Intro == null || model.Id == null)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiResponseModel
                {
                    Success = false,
                    Message = "Data not mapped",
                    Data = model
                });
            }
            try
            {
                var updatedUser = UserServices.UpdateBasicInfo(model);
                return Request.CreateResponse(HttpStatusCode.OK, new ApiResponseModel
                {
                    Success = true,
                    Message = "Successfully User profile updated",
                    Data = model
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

        [Route("api/User/ChangePassword")]
        public HttpResponseMessage ChangePassword(KaamShaam.AdminModels.LocalUser usermodel)
        {
            if (usermodel == null || usermodel.Id == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ApiResponseModel
                {
                    Success = false,
                    Message = "Data not mapped",
                    Data = usermodel
                });
            }
            var localUsermanager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            ApplicationUser user = localUsermanager.FindById(usermodel.Id);
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ApiResponseModel
                {
                    Success = false,
                    Message = "User not found!",
                    Data = usermodel
                });
            }
            user.PasswordHash = localUsermanager.PasswordHasher.HashPassword(usermodel.Password);
            var result = localUsermanager.Update(user);
            if (!result.Succeeded)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ApiResponseModel
                {
                    Success = false,
                    Message = "Failed to update password",
                    Data = usermodel
                });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new ApiResponseModel
            {
                Success = true,
                Message = "Password updated",
                Data = usermodel
            });
        }
    }
}
