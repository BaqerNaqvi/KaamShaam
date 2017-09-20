using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KaamShaam.AdminModels;
using KaamShaam.AdminServices;
using KaamShaam.LocalModels;
using KaamShaam.Models;
using KaamShaam.Services;
using Microsoft.AspNet.Identity;

namespace KaamShaam.Controllers
{
    public class ContractorController : Controller
    {
        public ActionResult FindContractor(long? categoryId)
         {
            //string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
            //                      Request.ApplicationPath.TrimEnd('/');
            //var contractors = AdminService.GetUsersByType("Contractor");
            //if (categoryId != null && categoryId!=0)
            //{
            //    contractors = contractors.Where(con => con.CategoryId == categoryId).ToList();
            //}
            //var places= Commons.Commons.FormatMapData(contractors, baseUrl);
            var cats = CategoryService.GetAllCategories();
            var list = new MapPlaceWrapper
            {
                status = "",
                listing = null,
                CatsList = cats
            };

            return View(list);
        }
        [HttpPost]
        public ActionResult FindContractor(ContractorRequestModel model)
        {
            DbGeography userLoc = null;
            List<string> latlng = new List<string>();
            if (!string.IsNullOrEmpty(model.LocationCords) && model.LocationCords != "")
            {
                latlng = model.LocationCords.Split('_').ToList();
                if (latlng.Count == 2)
                {
                    userLoc = Commons.Commons.ConvertLatLonToDbGeography(latlng[1], latlng[0]); // lat _ lng
                }
            }
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                                 Request.ApplicationPath.TrimEnd('/');
            var contractors = AdminService.GetUsersByType("Contractor");
            if (model.CategoryId != 0)
            {
                contractors = contractors.Where(con => con.CategoryId == model.CategoryId).ToList();
            }
            var filteredCons = new List<AdminModels.LocalUser>();
            foreach (var con in contractors)
            {
                 var dist= Commons.GeodesicDistance.GetDistance((double)userLoc.Latitude, (double)userLoc.Longitude, Convert.ToDouble(con.lat), Convert.ToDouble(con.lng));
                if ((int) dist < Convert.ToInt16(model.Distance))
                {
                   var disst=  Math.Round((double)dist, 2);
                    con.DistanceFromOrigin = disst;
                  filteredCons.Add(con);  
                }
            }
            filteredCons = filteredCons.OrderBy(fil => fil.DistanceFromOrigin).ToList();
            var userLocMarker = Commons.Commons.FormatMapDataForResercher(new AdminModels.LocalUser
            {
               lat = latlng[0],
               lng = latlng[1]
            }, baseUrl);
            List<MapPlaceModel> places = new List<MapPlaceModel>();
            places.Add(userLocMarker);

            places.AddRange(Commons.Commons.FormatMapData(filteredCons, baseUrl));
            var list = new MapPlaceWrapper
            {
                status = "found",
                listing = places,
            };
            return PartialView("~/Views/Contractor/FindContractorPartial.cshtml", list);
        }
       
        [HttpGet]
        public ActionResult GetSuggestions(string query)
        {
            var data = new List<UserSuggestionModel>();
            var cats = CategoryService.GetAllCategories();
            foreach (var cat in cats)
            {
                if (cat.Name.ToLower().Contains(query.ToLower()))
                {
                    var contractorForThatCat = UserServices.GetContractorCategoryId(cat.Id);
                    data.Add(new UserSuggestionModel
                    {
                        id = cat.Id.ToString(),
                        label = cat.Name+" (with "+cat.JobCount+" Jobs , "+contractorForThatCat.Count+" Service Provider)"
                    });
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ViewProfile(string userId)
        {
            var contractor = UserServices.GetUserById(userId);

            bool canRate = false;
            var visitorId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (!string.IsNullOrEmpty(visitorId))
            {
                var isContractorInvoledInAnyJobOfCurrenUser =
                        contractor.JobHistories.Any(jh => jh.PostedById == visitorId);
                var isProfileHitByUser = contractor.ProfileVisits.Any(pv => pv.VistedBy == visitorId);
                canRate = isContractorInvoledInAnyJobOfCurrenUser || isProfileHitByUser;
            }
            contractor.CanRate = canRate;

            return View(contractor);
        }

        public ActionResult FindContractorApi(ContractorRequestModel model)
        {
            DbGeography userLoc = null;
            if (!string.IsNullOrEmpty(model.LocationCords) && model.LocationCords != "")
            {
                var latlng = model.LocationCords.Split('_');
                if (latlng.Length == 2)
                {
                    userLoc = Commons.Commons.ConvertLatLonToDbGeography(latlng[1], latlng[0]); // lat _ lng
                }
            }
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                                 Request.ApplicationPath.TrimEnd('/');
            var contractors = AdminService.GetUsersByType("Contractor");
            if (model.CategoryId != 0)
            {
                contractors = contractors.Where(con => con.CategoryId == model.CategoryId).ToList();
            }
            var filteredCons = new List<AdminModels.LocalUser>();
            foreach (var con in contractors)
            {
                var dist = Commons.GeodesicDistance.GetDistance((double)userLoc.Latitude, (double)userLoc.Longitude, Convert.ToDouble(con.lat), Convert.ToDouble(con.lng));
                if ((int)dist < Convert.ToInt16(model.Distance))
                {
                    con.DistanceFromOrigin = (int)dist;
                    filteredCons.Add(con);
                }
            }
            filteredCons = filteredCons.OrderBy(fil => fil.DistanceFromOrigin).ToList();
            var places = Commons.Commons.FormatMapData(filteredCons, baseUrl);
            var list = new MapPlaceWrapper
            {
                status = "found",
                listing = places,
            };
            return PartialView("~/Views/Contractor/FindContractorPartial.cshtml", list);
        }
    }
} 