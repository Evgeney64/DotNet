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
    
    public partial class NSI_ALGORITHM : IEntityObject, IEntityLog
    {
        #region Columns
        long IEntityObject.Id { get { return NALGORITHM_ID; } }//;
        [KeyAttribute()]
        public int NALGORITHM_ID { get; set; }//;
        public string NALGORITHM_NAME { get; set; }//;
        public string HANDLER_LIB { get; set; }//;
        public string HANDLER_TYPE { get; set; }//;
        public string HANDLER_METHOD { get; set; }//;
        public string COMMENT { get; set; }//;
        public System.Nullable<int> PARENT_ID { get; set; }//;
        public System.Nullable<int> NODE_TYPE_ID { get; set; }//;
        public System.Nullable<int> NALGORITHM_TYPE_ID { get; set; }//;
        public System.Nullable<int> PRECISION_LEVEL { get; set; }//;
        public System.Nullable<int> DEFAULT_NALGORITHM_ID { get; set; }//;
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        public string DEVELOPER { get; set; }//;
        public string INFO { get; set; }//;
        #endregion
        #region Navigation - parents
        // 
        // FK_NSI_ALGORITHM_NSI_ALGORITHM   [NSI_ALGORITHM.NALGORITHM_ID]   #1
        [ForeignKey("PARENT_ID")]
        public virtual NSI_ALGORITHM NSI_ALGORITHM1 { get; set; }//;
        #endregion
        #region Navigation - children
        // 
        // FK_NSI_ALGORITHM_NSI_ALGORITHM   [NSI_ALGORITHM.PARENT_ID]   #2
        public virtual ICollection<NSI_ALGORITHM> NSI_ALGORITHM2 { get; set; }//;
        // 
        // FK_NSI_ALGORITHM_PARAM_NSI_ALGORITHM   [NSI_ALGORITHM_PARAM.NALGORITHM_ID]
        public virtual ICollection<NSI_ALGORITHM_PARAM> NSI_ALGORITHM_PARAM { get; set; }//;
        // 
        // FK_NSI_PARAM_NSI_ALGORITHM   [NSI_PARAM.NALGORITHM_ID]
        public virtual ICollection<NSI_PARAM> NSI_PARAM { get; set; }//;
        // 
        // FK_TASK_NSI_ALGORITHM   [TASK.NALGORITHM_ID]
        public virtual ICollection<TASK> TASK { get; set; }//;
        #endregion
        #region Constructor
        public NSI_ALGORITHM()
        {
            this.TASK = new HashSet<TASK>();
            this.NSI_PARAM = new HashSet<NSI_PARAM>();
            this.NSI_ALGORITHM_PARAM = new HashSet<NSI_ALGORITHM_PARAM>();
            this.NSI_ALGORITHM2 = new HashSet<NSI_ALGORITHM>();
        }
        #endregion
    }
}
