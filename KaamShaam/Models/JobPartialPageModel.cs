using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaamShaam.Models
{
    public class JobPartialPageModel
    {
        public List<CustomJobModel> JobList { get; set; }

        public PaggingClass Page { get; set; }
    }
}