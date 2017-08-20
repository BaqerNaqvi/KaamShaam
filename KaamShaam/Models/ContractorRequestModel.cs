using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KaamShaam.Models
{
    public class ContractorRequestModel
    {
        public string LocationCords { get; set; }

        public long CategoryId { get; set; }

        public string Distance { get; set; }

        public PaggingClass PageObj { get; set; }
    }
}