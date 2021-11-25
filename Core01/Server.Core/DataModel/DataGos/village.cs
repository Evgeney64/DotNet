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
    
    public partial class village : IEntityObject
    {
        #region Columns
        long IEntityObject.Id { get { return village_id; } }//;
        public System.Nullable<int> xAdr { get; set; }//;
        [KeyAttribute()]
        public int village_id { get; set; }//;
        public string village_name { get; set; }//;
        public System.Nullable<int> tvillage_id { get; set; }//;
        public System.Nullable<System.DateTime> ver_date { get; set; }//;
        public System.Nullable<short> ver_u_id { get; set; }//;
        public System.Nullable<int> district_id { get; set; }//;
        public string old_name { get; set; }//;
        public System.Nullable<int> municipal_id { get; set; }//;
        public System.Nullable<int> rgn_id { get; set; }//;
        public string fias { get; set; }//;
        public string oktmo { get; set; }//;
        public System.Nullable<int> level_id { get; set; }//;
        public System.Nullable<int> parent_level_id { get; set; }//;
        public System.Nullable<int> parent_id { get; set; }//;
        public int statusCentr { get; set; }//;
        public short status_fias { get; set; }//;
        #endregion
        #region Navigation - parents
        // 
        // FK_village_rgn   [rgn.rgn_id]
        [ForeignKey("rgn_id")]
        public virtual rgn rgn { get; set; }//;
        // 
        // FK_type_village_village__tvillage_id_PK_type_village   [type_village.tvillage_id]
        [ForeignKey("tvillage_id")]
        public virtual type_village type_village { get; set; }//;
        #endregion
        #region Navigation - children
        // 
        // FK_village_partners__village_id_PK_village   [Partners.village_id]
        public virtual ICollection<Partners> Partners { get; set; }//;
        // 
        // FK_village_partners__ph_village_id_PK_village   [Partners.ph_village_id]   #1
        public virtual ICollection<Partners> Partners1 { get; set; }//;
        // 
        // FK_village_street__village_id_PK_village   [street.village_id]
        public virtual ICollection<street> street { get; set; }//;
        #endregion
        #region Constructor
        public village()
        {
            this.Partners = new HashSet<Partners>();
            this.Partners1 = new HashSet<Partners>();
            this.street = new HashSet<street>();
        }
        #endregion
    }
}