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
using LocalUser = KaamShaam.AdminModels.LocalUser;

namespace KaamShaam.apis
{
    public class ContractorApiController : ApiController
    {
        [HttpPost]
        [Route("api/Contractor/Get")]
        public HttpResponseMessage FindContractors(ContractorRequestModel model)
        {
            try
            {
                //if (model == null)
                //{
                //    var response = new ApiResponseModel
                //    {
                //        Data = null,
                //        Message = "Mandatory  are null.",
                //        Success = false
                //    };
                //    return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                //}

                var contractors = AdminService.GetUsersByType("Contractor");
                if (model.CategoryId != 0)
                {
                    contractors = contractors.Where(con => con.CategoryId == model.CategoryId).ToList();
                }
                var filteredCons = new List<AdminModels.LocalUser>(contractors);
                if (!string.IsNullOrEmpty(model.LocationCords) && model.LocationCords != "" && !string.IsNullOrEmpty(model.Distance))
                {
                    var latlng = model.LocationCords.Split('_');
                    if (latlng.Length == 2)
                    {
                        var userLoc = Commons.Commons.ConvertLatLonToDbGeography(latlng[1], latlng[0]);
                        if (userLoc!=null && userLoc.Latitude!=null && userLoc.Longitude!=null)
                        {
                            filteredCons=new List<LocalUser>();
                            foreach (var con in contractors)
                            {
                                var dist = Commons.GeodesicDistance.GetDistance((double)userLoc.Latitude, (double)userLoc.Longitude, Convert.ToDouble(con.lat), Convert.ToDouble(con.lng));
                                if (dist!=null && (int)dist < Convert.ToInt16(model.Distance))
                                {
                                    con.DistanceFromOrigin = (double)dist;
                                    filteredCons.Add(con);
                                }
                            } 
                        }
                    }
                }
                string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                                     HttpContext.Current.Request.ApplicationPath.TrimEnd('/');
              
               
                filteredCons = filteredCons.OrderBy(fil => fil.DistanceFromOrigin).ToList();
                var places = Commons.Commons.FormatMapData(filteredCons, baseUrl);
                var allJobs = new MapPlaceWrapper
                {
                    status = "found",
                    listing = places,
                };
                return Request.CreateResponse(HttpStatusCode.OK, new ApiResponseModel
                {
                    Success = true,
                    Message = "Successfully fetched Contractors",
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
        [Route("api/Contractor/Profile")]
        public HttpResponseMessage ViewProfile(ContractorRequestModel model)
        {
            try
            {
                if (model == null || model.ContractorId == null || model.VisitedBy==null)
                {
                    var responseError = new ApiResponseModel
                    {
                        Data = model,
                        Message = "Data is not mapped",
                        Success = false
                    };
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, responseError);
                }
                //if (!string.IsNullOrEmpty(model.VisitedBy))
                //{
                //    var dbVisit = ProfileVisitorService.AddLocalVisit(new LocalProfileVisit
                //    {
                //        VistedOf = model.ContractorId,
                //        VistedBy = model.VisitedBy
                //    });
                //}
                var contractor = UserServices.GetUserById(model.ContractorId);

                var isContractorInvoledInAnyJobOfCurrenUser =
                    contractor.JobHistories.Any(jh => jh.PostedById == model.VisitedBy);

                var isProfileHitByUser = contractor.ProfileVisits.Any(pv => pv.VistedBy == model.VisitedBy || pv.VistedOf == model.VisitedBy);

                bool canRate = isContractorInvoledInAnyJobOfCurrenUser || isProfileHitByUser;

                contractor.CanRate = canRate;
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
