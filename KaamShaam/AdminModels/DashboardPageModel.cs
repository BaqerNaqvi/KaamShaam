using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KaamShaam.DbEntities;
using KaamShaam.LocalModels;

namespace KaamShaam.AdminModels
{
    public class DashboardPageModel
    {
        public int UserCount { get; set; }
        public int VendorsCount { get; set; }
        public int ContractorCount { get; set; }

        public int JobsCount { get; set; }

        public List<LocalCategory> Categories { get; set; }
    }
}