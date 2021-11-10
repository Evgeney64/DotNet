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
    
    
    public partial class NSI_EVENT_SUBTOPIC : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return NEVENT_SUBTOPIC_ID; } }//;
        
        [KeyAttribute()]
        public int NEVENT_SUBTOPIC_ID { get; set; }//;
        
        public int NEVENT_TOPIC_ID { get; set; }//;
        
        public string NEVENT_SUBTOPIC_NAME { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_NSI_EVENT_SUBTOPIC_NSI_EVENT_TOPIC
        [InverseProperty("NEVENT_TOPIC_ID")]
        public virtual NSI_EVENT_TOPIC NSI_EVENT_TOPIC { get; set; }//;
        #endregion
        
        #region Navigation - children
        // FK_EVENT_NSI_EVENT_SUBTOPIC
        public virtual ICollection<EVENT> EVENT { get; set; }//;
        #endregion
        
        #region Constructor
        public NSI_EVENT_SUBTOPIC()
        {
            this.EVENT = new HashSet<EVENT>();
        }
        #endregion
    }
}
