using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KaamShaam.LocalModels;

namespace KaamShaam.Models
{
    public class ManageJobModel
    {
        public PaggingClass Pagging { get; set; }
        public List<CustomJobModel> JobsList { get; set; }

        public List<LocalCategory> Categories { get; set; }

        public string Str { get; set; }
    }
}