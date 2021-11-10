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
    
    
    public partial class NSI_EVENT : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return NEVENT_ID; } }//;
        
        [KeyAttribute()]
        public int NEVENT_ID { get; set; }//;
        
        public string NEVENT_NAME { get; set; }//;
        
        public System.Nullable<int> NEVENT_GROUP_ID { get; set; }//;
        
        public string SP_LIST { get; set; }//;
        
        public System.Nullable<short> SP_AUTO { get; set; }//;
        
        public System.Nullable<short> WIN_LOG_SAVE { get; set; }//;
        
        public System.Nullable<int> NEVENT_KIND_ID { get; set; }//;
        
        public string DATA { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        
        public System.Nullable<int> NCONTACT_ID { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_NSI_EVENT_NSI_CONTACT
        [InverseProperty("NCONTACT_ID")]
        public virtual NSI_CONTACT NSI_CONTACT { get; set; }//;
        #endregion
        
        #region Navigation - children
        // FK_EVENT_NSI_EVENT
        public virtual ICollection<EVENT> EVENT { get; set; }//;
        
        // FK_EVENT_NSI_EVENT1
        public virtual ICollection<EVENT> EVENT1 { get; set; }//;
        
        // FK_TASK_EVENT_TRIGGER_NSI_EVENT
        public virtual ICollection<TASK_TRIGGER_EVENT> TASK_TRIGGER_EVENT { get; set; }//;
        #endregion
        
        #region Constructor
        public NSI_EVENT()
        {
            this.EVENT = new HashSet<EVENT>();
            this.EVENT1 = new HashSet<EVENT>();
            this.TASK_TRIGGER_EVENT = new HashSet<TASK_TRIGGER_EVENT>();
        }
        #endregion
    }
}
