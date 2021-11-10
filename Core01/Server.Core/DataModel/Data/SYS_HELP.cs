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
    
    
    public partial class SYS_HELP : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return SHELP_ID; } }//;
        
        [KeyAttribute()]
        public long SHELP_ID { get; set; }//;
        
        public System.Nullable<long> PARENT_ID { get; set; }//;
        
        public int SVERSION_ID { get; set; }//;
        
        public int SCLIENT_ID { get; set; }//;
        
        public int NDOCUMENT_ITEM_ID { get; set; }//;
        
        public int TYPE_ID { get; set; }//;
        
        public string SHELP_NAME { get; set; }//;
        
        public string DETAIL { get; set; }//;
        
        public string NUM { get; set; }//;
        
        public System.Nullable<int> NTAG_ID { get; set; }//;
        
        public System.Nullable<long> ROOT_ID { get; set; }//;
        
        public System.Nullable<int> STABLE_ID { get; set; }//;
        
        public System.Nullable<long> ID { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        
        public System.Nullable<int> NEVENT_STATE_ID { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_SYS_HELP_NSI_DOCUMENT_ITEM
        [InverseProperty("NDOCUMENT_ITEM_ID")]
        public virtual NSI_DOCUMENT_ITEM NSI_DOCUMENT_ITEM { get; set; }//;
        
        // FK_SYS_HELP_NSI_EVENT_STATE
        [InverseProperty("NEVENT_STATE_ID")]
        public virtual NSI_EVENT_STATE NSI_EVENT_STATE { get; set; }//;
        
        // FK_SYS_HELP_NSI_TAG
        [InverseProperty("NTAG_ID")]
        public virtual NSI_TAG NSI_TAG { get; set; }//;
        
        // FK_SYS_HELP_SYS_CLIENT
        [InverseProperty("SCLIENT_ID")]
        public virtual SYS_CLIENT SYS_CLIENT { get; set; }//;
        
        // FK_SYS_HELP_SYS_HELP
        [InverseProperty("PARENT_ID")]
        public virtual SYS_HELP SYS_HELP { get; set; }//;
        
        // FK_SYS_HELP_SYS_VERSION
        [InverseProperty("SVERSION_ID")]
        public virtual SYS_VERSION SYS_VERSION { get; set; }//;
        #endregion
        
        #region Navigation - children
        // FK_SYS_HELP_SYS_HELP
        public virtual ICollection<SYS_HELP> SYS_HELP { get; set; }//;
        
        // FK_SYS_HELP_INDEX_SYS_HELP
        public virtual ICollection<SYS_HELP_INDEX> SYS_HELP_INDEX { get; set; }//;
        #endregion
        
        #region Constructor
        public SYS_HELP()
        {
            this.SYS_HELP = new HashSet<SYS_HELP>();
            this.SYS_HELP_INDEX = new HashSet<SYS_HELP_INDEX>();
        }
        #endregion
    }
}
