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
    
    public partial class NSI_EVENT_TOPIC : IEntityObject, IEntityLog
    {
        #region Columns
        long IEntityObject.Id { get { return NEVENT_TOPIC_ID; } }//;
        [KeyAttribute()]
        public int NEVENT_TOPIC_ID { get; set; }//;
        public string NEVENT_TOPIC_NAME { get; set; }//;
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
        #region Navigation - children
        // 
        // FK_EVENT_NSI_EVENT_TOPIC   [EVENT.NEVENT_TOPIC_ID]
        public virtual ICollection<EVENT> EVENT { get; set; }//;
        // 
        // FK_NSI_EVENT_SUBTOPIC_NSI_EVENT_TOPIC   [NSI_EVENT_SUBTOPIC.NEVENT_TOPIC_ID]
        public virtual ICollection<NSI_EVENT_SUBTOPIC> NSI_EVENT_SUBTOPIC { get; set; }//;
        #endregion
        #region Constructor
        public NSI_EVENT_TOPIC()
        {
            this.EVENT = new HashSet<EVENT>();
            this.NSI_EVENT_SUBTOPIC = new HashSet<NSI_EVENT_SUBTOPIC>();
        }
        #endregion
    }
}
