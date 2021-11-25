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
    
    public partial class NSI_TAG : IEntityObject, IEntityLog
    {
        #region Columns
        long IEntityObject.Id { get { return NTAG_ID; } }//;
        [KeyAttribute()]
        public int NTAG_ID { get; set; }//;
        public int NTAG_TYPE_ID { get; set; }//;
        public string NTAG_NAME { get; set; }//;
        public string TAG { get; set; }//;
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
        #region Navigation - parents
        // 
        // FK_NSI_TAG_NSI_TAG_TYPE   [NSI_TAG_TYPE.NTAG_TYPE_ID]
        [ForeignKey("NTAG_TYPE_ID")]
        public virtual NSI_TAG_TYPE NSI_TAG_TYPE { get; set; }//;
        #endregion
        #region Navigation - children
        // 
        // FK_DOCUMENT_NSI_TAG   [DOCUMENT.NTAG_ID]
        public virtual ICollection<DOCUMENT> DOCUMENT { get; set; }//;
        // 
        // FK_DOCUMENT_ITEM_NSI_TAG   [DOCUMENT_ITEM.NTAG_ID]
        public virtual ICollection<DOCUMENT_ITEM> DOCUMENT_ITEM { get; set; }//;
        // 
        // FK_SYS_HELP_NSI_TAG   [SYS_HELP.NTAG_ID]
        public virtual ICollection<SYS_HELP> SYS_HELP { get; set; }//;
        #endregion
        #region Constructor
        public NSI_TAG()
        {
            this.SYS_HELP = new HashSet<SYS_HELP>();
            this.DOCUMENT_ITEM = new HashSet<DOCUMENT_ITEM>();
            this.DOCUMENT = new HashSet<DOCUMENT>();
        }
        #endregion
    }
}
