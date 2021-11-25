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
    
    public partial class SYS_CLIENT : IEntityObject, IEntityLog
    {
        #region Columns
        long IEntityObject.Id { get { return SCLIENT_ID; } }//;
        [KeyAttribute()]
        public int SCLIENT_ID { get; set; }//;
        public string SCLIENT_NAME { get; set; }//;
        public System.Nullable<int> PARENT_ID { get; set; }//;
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
        #region Navigation - parents
        // 
        // FK_SYS_CLIENT_SYS_CLIENT   [SYS_CLIENT.SCLIENT_ID]   #1
        [ForeignKey("PARENT_ID")]
        public virtual SYS_CLIENT SYS_CLIENT1 { get; set; }//;
        #endregion
        #region Navigation - children
        // 
        // FK_SYS_CLIENT_SYS_CLIENT   [SYS_CLIENT.PARENT_ID]   #2
        public virtual ICollection<SYS_CLIENT> SYS_CLIENT2 { get; set; }//;
        // 
        // FK_SYS_HELP_SYS_CLIENT   [SYS_HELP.SCLIENT_ID]
        public virtual ICollection<SYS_HELP> SYS_HELP { get; set; }//;
        // 
        // FK_SYS_HELP_INDEX_SYS_CLIENT   [SYS_HELP_INDEX.SCLIENT_ID]
        public virtual ICollection<SYS_HELP_INDEX> SYS_HELP_INDEX { get; set; }//;
        // 
        // FK_SYS_USER_GROUP_CONFIG_SYS_CLIENT   [SYS_USER_GROUP_CONFIG.SCLIENT_ID]
        public virtual ICollection<SYS_USER_GROUP_CONFIG> SYS_USER_GROUP_CONFIG { get; set; }//;
        #endregion
        #region Constructor
        public SYS_CLIENT()
        {
            this.SYS_CLIENT2 = new HashSet<SYS_CLIENT>();
            this.SYS_HELP = new HashSet<SYS_HELP>();
            this.SYS_HELP_INDEX = new HashSet<SYS_HELP_INDEX>();
            this.SYS_USER_GROUP_CONFIG = new HashSet<SYS_USER_GROUP_CONFIG>();
        }
        #endregion
    }
}
