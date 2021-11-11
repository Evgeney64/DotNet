//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Server.Core.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    
    public partial class NSI_CONTACT : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return NCONTACT_ID; } }//;
        
        [KeyAttribute()]
        public int NCONTACT_ID { get; set; }//;
        
        public string NCONTACT_NAME { get; set; }//;
        
        public string MASK { get; set; }//;
        
        public string CONNECTION_PREFIX { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
        
        #region Navigation - children
        // FK_NSI_EVENT_NSI_CONTACT
        public virtual ICollection<NSI_EVENT> NSI_EVENT { get; set; }//;
        
        // FK_PARTNER_CONTACT_NSI_CONTACT
        public virtual ICollection<PARTNER_CONTACT> PARTNER_CONTACT { get; set; }//;
        #endregion
        
        #region Constructor
        public NSI_CONTACT()
        {
            this.NSI_EVENT = new HashSet<NSI_EVENT>();
            this.PARTNER_CONTACT = new HashSet<PARTNER_CONTACT>();
        }
        #endregion
    }
}