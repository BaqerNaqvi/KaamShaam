using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaamShaam.LocalModels
{
    public class ProfileWraperModel
    {
        public LocalUser BasicInfo { get; set; }

        public List<LocalCategory> Categoreis { get; set; }

        public List<UserDrowdownModel> Companies { get; set; }
    }

}