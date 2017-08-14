using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaamShaam.Models
{
    public class PaggingClass
    {
        public int TotalItems { get; set; }

        public int ItemsPerPage { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public string SortBy { get; set; }

        public string SortOrder { get; set; }

        public long CategoryId { get; set; }

        public string SearchTerm { get; set; }

    }
}