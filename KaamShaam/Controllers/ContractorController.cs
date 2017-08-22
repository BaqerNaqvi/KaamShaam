using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                                  Request.ApplicationPath.TrimEnd('/');
            var contractors = AdminService.GetUsersByType("Contractor");
            if (categoryId != null && categoryId!=0)
            {
                contractors = contractors.Where(con => con.CategoryId == categoryId).ToList();
            }
            var places=FormatMapData(contractors, baseUrl);
            var cats = CategoryService.GetAllCategories();
            var list = new MapPlaceWrapper
            {
                status = "found",
                listing = places,
                CatsList = cats
            };

            return View(list);
        }
        [HttpPost]
        public ActionResult FindContractor(ContractorRequestModel model)
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
                 var dist= Commons.GeodesicDistance.GetDistance((double)userLoc.Latitude, (double)userLoc.Longitude, (double)con.lat, (double)con.lng);
                if ((int) dist < Convert.ToInt16(model.Distance))
                {
                    con.DistanceFromOrigin = (int) dist;
                  filteredCons.Add(con);  
                }
            }
            filteredCons = filteredCons.OrderBy(fil => fil.DistanceFromOrigin).ToList();
            var places = FormatMapData(filteredCons, baseUrl);
            var list = new MapPlaceWrapper
            {
                status = "found",
                listing = places,
            };
            return PartialView("~/Views/Contractor/FindContractorPartial.cshtml", list);
        }
        public List<MapPlaceModel> FormatMapData(List<AdminModels.LocalUser> contractors, string baseUrl)
        {
            var places = new List<MapPlaceModel>();
            foreach (var cont in contractors)
            {
                places.Add(new MapPlaceModel
                {
                    id = cont.Id,
                    latitude = cont.lat,
                    longitude = cont.lng,
                    image = AccountController.SetImagePath(cont.Id, "110x110", baseUrl + "/Images/"),
                    title = cont.FullName,
                    subjects = cont.Mobile,
                    url = "#",
                    featured = "no",
                    marker = "../Images/icons/markerone.png",
                    CatName = cont.CatName,
                    Phone = cont.Mobile,
                    Email = cont.Email,
                    Distance = cont.DistanceFromOrigin
                });
            }
            return places;
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
            return View(contractor);
        }
    }
} 