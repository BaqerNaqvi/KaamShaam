//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KaamShaam.DbEntities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Job
    {
        public long Id { get; set; }
        public string JobTitle { get; set; }
        public System.Data.Entity.Spatial.DbGeography Location { get; set; }
        public long CategoryId { get; set; }
        public int Fee { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string PostedById { get; set; }
        public string LocationName { get; set; }
        public System.DateTime PostingDate { get; set; }
        public bool Ststus { get; set; }
    
        public virtual Category Category { get; set; }
    }
}
