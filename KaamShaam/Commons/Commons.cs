using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using KaamShaam.Controllers;
using KaamShaam.LocalModels;

namespace KaamShaam.Commons
{
    public static class Commons
    {
        public static DbGeography ConvertLatLonToDbGeography(string longitude, string latitude)
        {
            var point = string.Format("POINT({1} {0})", latitude, longitude);
            return DbGeography.FromText(point);
        }

        public static List<MapPlaceModel> FormatMapData(List<AdminModels.LocalUser> contractors, string baseUrl)
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
                    marker = baseUrl+ "/Images/icons/contractPin.png",
                    CatName = cont.CatName,
                    Phone = cont.Mobile,
                    Email = cont.Email,
                    Distance = cont.DistanceFromOrigin,
                    score = cont.Score,
                    votes = cont.UserRatings.Count
                });
            }
            return places;
        }

        public static MapPlaceModel FormatMapDataForResercher(AdminModels.LocalUser researcher, string baseUrl)
        {
            return new MapPlaceModel
            {
                latitude = researcher.lat,
                longitude = researcher.lng,
                marker = baseUrl + "/Images/icons/userPin.png",
            };
        }
    }
    public static class GeodesicDistance
    {
        private static double Radians(double degrees)
        {
            return (0.017453292519943295 * degrees);
        }

        public static double? GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371; // km

            double sLat1 = Math.Sin(Radians(lat1));
            double sLat2 = Math.Sin(Radians(lat2));
            double cLat1 = Math.Cos(Radians(lat1));
            double cLat2 = Math.Cos(Radians(lat2));
            double cLon = Math.Cos(Radians(lon1) - Radians(lon2));

            double cosD = sLat1 * sLat2 + cLat1 * cLat2 * cLon;

            double d = Math.Acos(cosD);

            double dist = R * d;

            return dist;
        }
    }
}