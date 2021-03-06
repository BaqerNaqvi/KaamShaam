﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaamShaam.Models
{
    public class HomePageWraper
    {
        public List<string> BannersList { get; set; }

        public List<ContractorIndexPageModel> ContractorCats { get; set; }

        public List<GeneralFeedbackModel> Feedback { get; set; }


        public List<CustomJobModel> Jobs { get; set; }
    }
}