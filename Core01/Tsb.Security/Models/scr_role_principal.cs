//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tsb.Security.Web.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class scr_role_principal
    {
        public int role_principal_id { get; set; }
        public int role_id { get; set; }
        public int principal_id { get; set; }
        public short is_deny { get; set; }
        public Nullable<int> group_id { get; set; }
    
        public virtual scr_group scr_group { get; set; }
        public virtual scr_principal scr_principal { get; set; }
        public virtual scr_role scr_role { get; set; }
    }
}
