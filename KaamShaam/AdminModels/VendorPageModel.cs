using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaamShaam.AdminModels
{
    public class VendorPageModel
    {
        public List<LocalUser> Contractors { get; set; }

        public string Vendor { get; set; }
    }
}