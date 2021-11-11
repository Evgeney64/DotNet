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
    
    
    public partial class TASK_EXECUTION_DETAIL : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return TASK_EXECUTION_DETAIL_ID; } }//;
        
        [KeyAttribute()]
        public long TASK_EXECUTION_DETAIL_ID { get; set; }//;
        
        public long TASK_EXECUTION_ID { get; set; }//;
        
        public System.Nullable<int> STABLE_ID { get; set; }//;
        
        public System.Nullable<long> ID { get; set; }//;
        
        public int NTASK_STATUS_ID { get; set; }//;
        
        public string COMMENT { get; set; }//;
        
        public System.Nullable<System.DateTime> DATE_BEG { get; set; }//;
        
        public System.Nullable<System.DateTime> DATE_END { get; set; }//;
        
        public System.Nullable<long> DOCUMENT_ID { get; set; }//;
        
        public string PARAM { get; set; }//;
        
        public System.Nullable<int> NTASK_RESULT_ID { get; set; }//;
        
        public System.Nullable<int> THREAD_ID { get; set; }//;
        
        public string RESULT { get; set; }//;
        
        public string RESULT_CODE { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_TASK_EXECUTION_DETAIL_DOCUMENT
        [InverseProperty("DOCUMENT_ID")]
        public virtual DOCUMENT DOCUMENT { get; set; }//;
        
        // FK_TASK_EXECUTION_DETAIL_NSI_TASK_RESULT
        [InverseProperty("NTASK_RESULT_ID")]
        public virtual NSI_TASK_RESULT NSI_TASK_RESULT { get; set; }//;
        
        // FK_TASK_EXECUTION_DETAIL_NSI_TASK_STATUS
        [InverseProperty("NTASK_STATUS_ID")]
        public virtual NSI_TASK_STATUS NSI_TASK_STATUS { get; set; }//;
        
        // FK_TASK_EXECUTION_DETAIL_TASK_EXECUTION
        [InverseProperty("TASK_EXECUTION_ID")]
        public virtual TASK_EXECUTION TASK_EXECUTION { get; set; }//;
        #endregion
    }
}
