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
    
    
    public partial class SYS_CHANGE_LIST_EVENT : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return SCHANGE_LIST_EVENT_ID; } }//;
        
        [KeyAttribute()]
        public long SCHANGE_LIST_EVENT_ID { get; set; }//;
        
        public long SCHANGE_LIST_ID { get; set; }//;
        
        public System.Nullable<long> EVENT_ID { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_SYS_CHANGE_LIST_EVENT_SYS_CHANGE_LIST_EVENT
        [InverseProperty("SCHANGE_LIST_ID")]
        public virtual SYS_CHANGE_LIST SYS_CHANGE_LIST { get; set; }//;
        #endregion
    }
}
