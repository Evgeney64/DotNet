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
    
    public partial class NSI_VILLAGE_TYPE : IEntityObject, IEntityLog
    {
        #region Columns
        long IEntityObject.Id { get { return NVILLAGE_TYPE_ID; } }//;
        [KeyAttribute()]
        public long NVILLAGE_TYPE_ID { get; set; }//;
        public string NVILLAGE_TYPE_SNAME { get; set; }//;
        public string GNI_SOCR { get; set; }//;
        public string NVILLAGE_TYPE_NAME { get; set; }//;
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
        #region Navigation - children
        // 
        // FK_NSI_VILLAGE_NSI_VILLAGE_TYPE   [NSI_VILLAGE.NVILLAGE_TYPE_ID]
        public virtual ICollection<NSI_VILLAGE> NSI_VILLAGE { get; set; }//;
        #endregion
        #region Constructor
        public NSI_VILLAGE_TYPE()
        {
            this.NSI_VILLAGE = new HashSet<NSI_VILLAGE>();
        }
        #endregion
    }
}