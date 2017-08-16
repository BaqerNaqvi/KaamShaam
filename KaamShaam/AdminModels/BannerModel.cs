using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaamShaam.AdminModels
{
    public class BannerModel
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public int ShowOrder { get; set; }
        public bool Status { get; set; }
    }
}