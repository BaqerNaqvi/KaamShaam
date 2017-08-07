using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaamShaam.Models
{
    public class JobRequestModel
    {
        public CustomJobModel JobModel { get; set; }

        public PaggingClass PageObj { get; set; }
    }
}