using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KaamShaam.DbEntities;

namespace KaamShaam.LocalModels
{
    public class LocalProfileVisit
    {
        public long Id { get; set; }
        public string VistedBy { get; set; }
        public string VistedOf { get; set; }
        public string DateTime { get; set; }

        public virtual LocalUser Vistor { get; set; }
        public virtual LocalUser Owner { get; set; }

        public string VisitorName { get; set; }
    }

    public static class LocalProfileVisitMapper
    {

        public static LocalProfileVisit Mapper(this ProfileVisit source)
        {
            var obj = new LocalProfileVisit
            {
                Id= source.Id,
                DateTime = source.DateTime.ToLongDateString() + source.DateTime.ToLongTimeString(),
                VistedBy = source.VistedBy,
             //   Owner = source.Owner.MapUser(),
               VistedOf = source.VistedOf,
             //   Vistor = source.Vistor.MapUser()
            // VisitorName = source.Vistor.FullName
            };
            return obj;
        }
    }
}