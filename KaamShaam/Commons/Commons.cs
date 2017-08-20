using System;
using System.Data.Entity.Spatial;

namespace KaamShaam.Commons
{
    public static class Commons
    {
        public static DbGeography ConvertLatLonToDbGeography(string longitude, string latitude)
        {
            var point = string.Format("POINT({1} {0})", latitude, longitude);
            return DbGeography.FromText(point);
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