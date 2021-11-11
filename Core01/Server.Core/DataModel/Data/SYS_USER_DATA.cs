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
    
    
    public partial class SYS_USER_DATA : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return SUSER_DATA_ID; } }//;
        
        [KeyAttribute()]
        public int SUSER_DATA_ID { get; set; }//;
        
        public int SUSER_ID { get; set; }//;
        
        public int SUSER_DATA_TYPE_ID { get; set; }//;
        
        public int SMODULE_ID { get; set; }//;
        
        public System.Nullable<int> SCONFIG_ID { get; set; }//;
        
        public string DATA { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_SYS_USER_DATA_SYS_CONFIG
        [InverseProperty("SCONFIG_ID")]
        public virtual SYS_CONFIG SYS_CONFIG { get; set; }//;
        
        // FK_SYS_USER_DATA_SYS_MODULE
        [InverseProperty("SMODULE_ID")]
        public virtual SYS_MODULE SYS_MODULE { get; set; }//;
        
        // FK_SYS_USER_DATA_SYS_USER
        [InverseProperty("SUSER_ID")]
        public virtual SYS_USER SYS_USER { get; set; }//;
        #endregion
    }
}