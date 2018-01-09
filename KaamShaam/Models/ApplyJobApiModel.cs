using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaamShaam.Models
{
    public class ApplyJobApiModel
    {
        public long JobId { get; set; }

        public string AppliedBy { get; set; }
    }

    public class AcceptProposalApiModel
    {
        public long JobId { get; set; }

        public string ContractorId { get; set; }
    }

    public class CloseJobApiModel
    {
        public long JobId { get; set; }

        public string Comments { get; set; }

        public string RatedBy { get; set; }

        public string RatedTo { get; set; }

        public double Rating { get; set; }
    }
}
