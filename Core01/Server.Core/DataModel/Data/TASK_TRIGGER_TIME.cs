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
    
    
    public partial class TASK_TRIGGER_TIME : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return TASK_TRIGGER_ID; } }//;
        
        [KeyAttribute()]
        public long TASK_TRIGGER_ID { get; set; }//;
        
        public int NTASK_TRIGGER_TIME_ID { get; set; }//;
        
        public System.Nullable<short> INTERVAL { get; set; }//;
        
        public System.Nullable<short> DAYS_OF_WEEK { get; set; }//;
        
        public System.Nullable<int> DAYS_OF_MONTH { get; set; }//;
        
        public System.Nullable<short> WEEKS_OF_MONTH { get; set; }//;
        
        public System.Nullable<short> MONTHS_OF_YEAR { get; set; }//;
        
        public System.Nullable<short> ON_LAST { get; set; }//;
        
        public string DAYS_OF_WEEK_ { get; set; }//;
        
        public string DAYS_OF_MONTH_ { get; set; }//;
        
        public string WEEKS_OF_MONTH_ { get; set; }//;
        
        public string MONTHS_OF_YEAR_ { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_TASK_TRIGGER_TIME_NSI_TASK_TRIGGER_TIME
        [InverseProperty("NTASK_TRIGGER_TIME_ID")]
        public virtual NSI_TASK_TRIGGER_TIME NSI_TASK_TRIGGER_TIME { get; set; }//;
        
        // FK_TASK_TRIGGER_TIME_TASK_TRIGGER
        [InverseProperty("TASK_TRIGGER_ID")]
        public virtual TASK_TRIGGER TASK_TRIGGER { get; set; }//;
        #endregion
    }
}
