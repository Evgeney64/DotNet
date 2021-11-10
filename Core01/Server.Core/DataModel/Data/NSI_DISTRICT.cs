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
    
    
    public partial class NSI_DISTRICT : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return NDISTRICT_ID; } }//;
        
        [KeyAttribute()]
        public int NDISTRICT_ID { get; set; }//;
        
        public System.Nullable<int> PARENT_ID { get; set; }//;
        
        public System.Nullable<int> DISTRICT_TYPE { get; set; }//;
        
        public string NDISTRICT_NAME { get; set; }//;
        
        public string NDISTRICT_NUM { get; set; }//;
        
        public System.Nullable<long> PARTNER_ID { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_NSI_DISTRICT_NSI_DISTRICT1
        [InverseProperty("PARENT_ID")]
        public virtual NSI_DISTRICT NSI_DISTRICT { get; set; }//;
        
        // FK_NSI_DISTRICT_NSI_DISTRICT
        [InverseProperty("PARTNER_ID")]
        public virtual PARTNER PARTNER { get; set; }//;
        #endregion
        
        #region Navigation - children
        // FK_NSI_DISTRICT_NSI_DISTRICT1
        public virtual ICollection<NSI_DISTRICT> NSI_DISTRICT { get; set; }//;
        #endregion
        
        #region Constructor
        public NSI_DISTRICT()
        {
            this.NSI_DISTRICT = new HashSet<NSI_DISTRICT>();
        }
        #endregion
    }
}
