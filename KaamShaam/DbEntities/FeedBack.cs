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
    
    public partial class FeedBack
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PostedBy { get; set; }
        public bool IsApproved { get; set; }
        public Nullable<bool> Status { get; set; }
        public System.DateTime DateTime { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
    }
}