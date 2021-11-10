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
        // FK_SYS_CLIENT_SYS_CLIENT
        [InverseProperty("PARENT_ID")]
        public virtual SYS_CLIENT SYS_CLIENT { get; set; }//;
        #endregion
        
        #region Navigation - children
        // FK_NSI_ALGORITHM_IMPL_BINDING_SYS_CLIENT
        public virtual ICollection<NSI_ALGORITHM_IMPL_BINDING> NSI_ALGORITHM_IMPL_BINDING { get; set; }//;
        
        // FK_NSI_CONFIG_BINDING_SYS_CLIENT
        public virtual ICollection<NSI_CONFIG_BINDING> NSI_CONFIG_BINDING { get; set; }//;
        
        // FK_SYS_CLIENT_SYS_CLIENT
        public virtual ICollection<SYS_CLIENT> SYS_CLIENT { get; set; }//;
        
        // FK_SYS_HELP_SYS_CLIENT
        public virtual ICollection<SYS_HELP> SYS_HELP { get; set; }//;
        
        // FK_SYS_HELP_INDEX_SYS_CLIENT
        public virtual ICollection<SYS_HELP_INDEX> SYS_HELP_INDEX { get; set; }//;
        
        // FK_SYS_TEMPLATE_BINDING_SYS_CLIENT
        public virtual ICollection<SYS_TEMPLATE_BINDING> SYS_TEMPLATE_BINDING { get; set; }//;
        
        // FK_SYS_USER_GROUP_CONFIG_SYS_CLIENT
        public virtual ICollection<SYS_USER_GROUP_CONFIG> SYS_USER_GROUP_CONFIG { get; set; }//;
        #endregion
        
        #region Constructor
        public SYS_CLIENT()
        {
            this.NSI_ALGORITHM_IMPL_BINDING = new HashSet<NSI_ALGORITHM_IMPL_BINDING>();
            this.NSI_CONFIG_BINDING = new HashSet<NSI_CONFIG_BINDING>();
            this.SYS_CLIENT = new HashSet<SYS_CLIENT>();
            this.SYS_HELP = new HashSet<SYS_HELP>();
            this.SYS_HELP_INDEX = new HashSet<SYS_HELP_INDEX>();
            this.SYS_TEMPLATE_BINDING = new HashSet<SYS_TEMPLATE_BINDING>();
            this.SYS_USER_GROUP_CONFIG = new HashSet<SYS_USER_GROUP_CONFIG>();
        }
        #endregion
    }
}
