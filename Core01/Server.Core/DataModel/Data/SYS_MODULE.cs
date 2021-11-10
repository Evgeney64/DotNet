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
    
    
    public partial class SYS_MODULE : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return SMODULE_ID; } }//;
        
        [KeyAttribute()]
        public int SMODULE_ID { get; set; }//;
        
        public string SMODULE_NAME { get; set; }//;
        
        public string SMODULE_SNAME { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
        
        #region Navigation - children
        // FK_NSI_ALGORITHM_IMPL_BINDING_SYS_MODULE
        public virtual ICollection<NSI_ALGORITHM_IMPL_BINDING> NSI_ALGORITHM_IMPL_BINDING { get; set; }//;
        
        // FK_NSI_CONFIG_BINDING_SYS_MODULE
        public virtual ICollection<NSI_CONFIG_BINDING> NSI_CONFIG_BINDING { get; set; }//;
        
        // FK_SYS_TEMPLATE_BINDING_SYS_MODULE
        public virtual ICollection<SYS_TEMPLATE_BINDING> SYS_TEMPLATE_BINDING { get; set; }//;
        
        // FK_SYS_USER_DATA_SYS_MODULE
        public virtual ICollection<SYS_USER_DATA> SYS_USER_DATA { get; set; }//;
        
        // FK_SYS_USER_GROUP_CONFIG_SYS_MODULE
        public virtual ICollection<SYS_USER_GROUP_CONFIG> SYS_USER_GROUP_CONFIG { get; set; }//;
        
        // FK_SYS_VERSION_SYS_MODULE
        public virtual ICollection<SYS_VERSION> SYS_VERSION { get; set; }//;
        #endregion
        
        #region Constructor
        public SYS_MODULE()
        {
            this.NSI_ALGORITHM_IMPL_BINDING = new HashSet<NSI_ALGORITHM_IMPL_BINDING>();
            this.NSI_CONFIG_BINDING = new HashSet<NSI_CONFIG_BINDING>();
            this.SYS_TEMPLATE_BINDING = new HashSet<SYS_TEMPLATE_BINDING>();
            this.SYS_USER_DATA = new HashSet<SYS_USER_DATA>();
            this.SYS_USER_GROUP_CONFIG = new HashSet<SYS_USER_GROUP_CONFIG>();
            this.SYS_VERSION = new HashSet<SYS_VERSION>();
        }
        #endregion
    }
}
