using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace KaamShaam.LocalModels
{
    public static class Commons
    {
        public static DbGeography ConvertLatLonToDbGeography(string longitude, string latitude)
        {
            var point = string.Format("POINT({1} {0})", latitude, longitude);
            return DbGeography.FromText(point);
        }
    }
}