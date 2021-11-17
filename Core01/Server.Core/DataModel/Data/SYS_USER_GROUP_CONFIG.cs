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
    
    public partial class SYS_USER_GROUP_CONFIG : IEntityObject, IEntityLog
    {
        #region Columns
        long IEntityObject.Id { get { return SUSER_GROUP_CONFIG_ID; } }//;
        [KeyAttribute()]
        public int SUSER_GROUP_CONFIG_ID { get; set; }//;
        public System.Nullable<int> PRINCIPAL_ID { get; set; }//;
        public int STABLE_ID { get; set; }//;
        public int SCONFIG_TYPE_ID { get; set; }//;
        public int SCONFIG_ID { get; set; }//;
        public int SMODULE_ID { get; set; }//;
        public int SCLIENT_ID { get; set; }//;
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
        #region Navigation - parents
        // 
        // FK_SYS_USER_GROUP_CONFIG_SYS_CLIENT   [SYS_CLIENT.SCLIENT_ID]
        [ForeignKey("SCLIENT_ID")]
        public virtual SYS_CLIENT SYS_CLIENT { get; set; }//;
        // 
        // FK_SYS_USER_GROUP_CONFIG_SYS_CONFIG   [SYS_CONFIG.SCONFIG_ID]
        [ForeignKey("SCONFIG_ID")]
        public virtual SYS_CONFIG SYS_CONFIG { get; set; }//;
        // 
        // FK_SYS_USER_GROUP_CONFIG_SYS_MODULE   [SYS_MODULE.SMODULE_ID]
        [ForeignKey("SMODULE_ID")]
        public virtual SYS_MODULE SYS_MODULE { get; set; }//;
        #endregion
    }
}
