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
    
    public partial class type_village : IEntityObject
    {
        #region Columns
        long IEntityObject.Id { get { return tvillage_id; } }//;
        [KeyAttribute()]
        public int tvillage_id { get; set; }//;
        public string tvillage_name { get; set; }//;
        public string tvillage_sname { get; set; }//;
        public string full_name { get; set; }//;
        #endregion
        #region Navigation - children
        // FK_type_village_village__tvillage_id_PK_type_village
        public virtual ICollection<village> village { get; set; }//;
        #endregion
        #region Constructor
        public type_village()
        {
            this.village = new HashSet<village>();
        }
        #endregion
    }
}
