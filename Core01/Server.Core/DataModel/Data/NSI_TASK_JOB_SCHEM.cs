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
    
    
    public partial class NSI_TASK_JOB_SCHEM : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return NTASK_JOB_SCHEM_ID; } }//;
        
        [KeyAttribute()]
        public int NTASK_JOB_SCHEM_ID { get; set; }//;
        
        public System.Nullable<int> PARENT_ID { get; set; }//;
        
        public System.Nullable<int> CHILD_ID { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_NSI_TASK_JOB_SCHEM_NSI_TASK_JOB
        [InverseProperty("CHILD_ID")]
        public virtual NSI_TASK_JOB NSI_TASK_JOB { get; set; }//;
        
        // FK_NSI_TASK_JOB_SCHEM_NSI_TASK_JOB1
        [InverseProperty("PARENT_ID")]
        public virtual NSI_TASK_JOB NSI_TASK_JOB1 { get; set; }//;
        #endregion
    }
}