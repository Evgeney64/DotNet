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
    
    public partial class rgn : IEntityObject
    {
        #region Columns
        long IEntityObject.Id { get { return rgn_id; } }//;
        [KeyAttribute()]
        public int rgn_id { get; set; }//;
        public string rgn_name { get; set; }//;
        public string regioncode { get; set; }//;
        public string rgn1 { get; set; }//;
        public System.Nullable<int> fst_code { get; set; }//;
        public string fias { get; set; }//;
        public string oktmo { get; set; }//;
        public System.Nullable<int> level_id { get; set; }//;
        public System.Nullable<int> parent_level_id { get; set; }//;
        public System.Nullable<int> parent_id { get; set; }//;
        public short status_fias { get; set; }//;
        #endregion
        #region Navigation - children
        // FK_Partners_rgn__ml_rgn_id_PK_rgn
        public virtual ICollection<Partners> Partners { get; set; }//;
        // FK_Partners_rgn__ph_rgn_id_PK_rgn
        public virtual ICollection<Partners> Partners1 { get; set; }//;
        // FK_Partners_rgn__rgn_id_PK_rgn
        public virtual ICollection<Partners> Partners2 { get; set; }//;
        // FK_village_rgn
        public virtual ICollection<village> village { get; set; }//;
        #endregion
        #region Constructor
        public rgn()
        {
            this.Partners = new HashSet<Partners>();
            this.Partners1 = new HashSet<Partners>();
            this.Partners2 = new HashSet<Partners>();
            this.village = new HashSet<village>();
        }
        #endregion
    }
}
