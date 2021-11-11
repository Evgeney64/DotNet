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
    
    
    public partial class NSI_PARAM : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return NPARAM_ID; } }//;
        
        [KeyAttribute()]
        public int NPARAM_ID { get; set; }//;
        
        public string NPARAM_NAME { get; set; }//;
        
        public System.Nullable<int> STABLE_ID { get; set; }//;
        
        public System.Nullable<int> STABLE_COLUMN_ID { get; set; }//;
        
        public System.Nullable<int> STABLE_COLUMN_ENUM_GROUP_ID { get; set; }//;
        
        public System.Nullable<int> NALGORITHM_ID { get; set; }//;
        
        public string DETAIL { get; set; }//;
        
        public string COMMENT { get; set; }//;
        
        public System.Nullable<int> SOURCE_STABLE_ID { get; set; }//;
        
        public System.Nullable<int> SOURCE_STABLE_COLUMN_ID { get; set; }//;
        
        public System.Nullable<int> NEXT_PARAM_ID { get; set; }//;
        
        public System.Nullable<int> STABLE_COLUMN_ENUM_FID { get; set; }//;
        
        public System.Nullable<int> EXT_PARAM_STABLE_ID { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_NSI_PARAM_NSI_ALGORITHM
        [InverseProperty("NALGORITHM_ID")]
        public virtual NSI_ALGORITHM NSI_ALGORITHM { get; set; }//;
        
        // FK_NSI_PARAM_SYS_TABLE
        [InverseProperty("STABLE_ID")]
        public virtual SYS_TABLE SYS_TABLE { get; set; }//;
        
        // FK_NSI_PARAM_SOURCE_SYS_TABLE
        [InverseProperty("SOURCE_STABLE_ID")]
        public virtual SYS_TABLE SYS_TABLE1 { get; set; }//;
        
        // FK_NSI_PARAM_SYS_TABLE_COLUMN
        [InverseProperty("STABLE_COLUMN_ID")]
        public virtual SYS_TABLE_COLUMN SYS_TABLE_COLUMN { get; set; }//;
        
        // FK_NSI_PARAM_SOURCE_SYS_TABLE_COLUMN
        [InverseProperty("SOURCE_STABLE_COLUMN_ID")]
        public virtual SYS_TABLE_COLUMN SYS_TABLE_COLUMN1 { get; set; }//;
        
        // FK_NSI_PARAM_SYS_TABLE_COLUMN_ENUM_GROUP
        [InverseProperty("STABLE_COLUMN_ENUM_GROUP_ID")]
        public virtual SYS_TABLE_COLUMN_ENUM_GROUP SYS_TABLE_COLUMN_ENUM_GROUP { get; set; }//;
        #endregion
        
        #region Navigation - children
        // FK_EXT_PARAM_NSI_PARAM
        public virtual ICollection<EXT_PARAM> EXT_PARAM { get; set; }//;
        
        // FK_NSI_ALGORITHM_PARAM_NSI_PARAM
        public virtual ICollection<NSI_ALGORITHM_PARAM> NSI_ALGORITHM_PARAM { get; set; }//;
        #endregion
        
        #region Constructor
        public NSI_PARAM()
        {
            this.EXT_PARAM = new HashSet<EXT_PARAM>();
            this.NSI_ALGORITHM_PARAM = new HashSet<NSI_ALGORITHM_PARAM>();
        }
        #endregion
    }
}