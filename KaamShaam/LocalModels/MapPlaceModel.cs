using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaamShaam.LocalModels
{
    public class MapPlaceModel
    {
        public string id { get; set; }

        public double? longitude { get; set; }

        public double? latitude { get; set; }

        public string image { get; set; }

        public string subjects { get; set; }

        public string title { get; set; }


        public string url { get; set; }

        public string featured { get; set; }

        public string marker { get; set; }
        public string CatName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int Distance { get; set; }
    }

    public class MapPlaceWrapper
    {
        public string status { get; set; }
        public List<MapPlaceModel> listing { get; set; }

        public List<LocalCategory> CatsList { get; set; }
    }
}