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
    
    
    public partial class TASK_CONFIG_RELATION : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return TASK_CONFIG_RELATION_ID; } }//;
        
        [KeyAttribute()]
        public long TASK_CONFIG_RELATION_ID { get; set; }//;
        
        public System.Nullable<int> NTASK_CONFIG_ID { get; set; }//;
        
        public System.Nullable<long> CHILD_ID { get; set; }//;
        
        public System.Nullable<int> STABLE_ID { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_TASK_CONFIG_RELATION_NTASK_CONFIG_ID
        [InverseProperty("NTASK_CONFIG_ID")]
        public virtual NSI_TASK_CONFIG NSI_TASK_CONFIG { get; set; }//;
        
        // FK_TASK_CONFIG_RELATION_STABLE_ID
        [InverseProperty("STABLE_ID")]
        public virtual SYS_TABLE SYS_TABLE { get; set; }//;
        #endregion
    }
}