using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaamShaam.Models
{
    public class JobProposalModel
    {
        public long Id { get; set; }
      
        public string JobTitle { get; set; }

        public string CatName { get; set; }

        public string Fee { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }

        public string PostedById { get; set; }
        public string ContractorId { get; set; }  // for job proposal

        public string ContractorName { get; set; }  // for job proposal
    }
}