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
    
    public partial class ProfileVisit
    {
        public long Id { get; set; }
        public string VistedBy { get; set; }
        public string VistedOf { get; set; }
        public System.DateTime DateTime { get; set; }
    
        public virtual AspNetUser Vistor { get; set; }
        public virtual AspNetUser Owner { get; set; }
    }
}
