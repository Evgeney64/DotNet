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
    
    
    public partial class NSI_VILLAGE : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return NVILLAGE_ID; } }//;
        
        [KeyAttribute()]
        public long NVILLAGE_ID { get; set; }//;
        
        public string NVILLAGE_NAME { get; set; }//;
        
        public System.Nullable<long> NVILLAGE_TYPE_ID { get; set; }//;
        
        public System.Nullable<long> NOBLAST_ID { get; set; }//;
        
        public System.Nullable<long> NOBLAST_REGION_ID { get; set; }//;
        
        public System.Nullable<int> NMUNICIPALITY_ID { get; set; }//;
        
        public string GNI_CODE { get; set; }//;
        
        public string FIAS { get; set; }//;
        
        public System.Nullable<int> NDATA_SOURCE_ID { get; set; }//;
        
        public System.Nullable<long> PARENT_ID { get; set; }//;
        
        public System.Nullable<int> VILLAGE_KIND { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        
        public System.Nullable<int> NMUNICIPALITY_TYPE_ID { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_NSI_VILLAGE_NSI_MUNICIPALITY
        [InverseProperty("NMUNICIPALITY_ID")]
        public virtual NSI_MUNICIPALITY NSI_MUNICIPALITY { get; set; }//;
        
        // FK_NSI_VILLAGE_NSI_MUNICIPALITY_TYPE
        [InverseProperty("NMUNICIPALITY_TYPE_ID")]
        public virtual NSI_MUNICIPALITY_TYPE NSI_MUNICIPALITY_TYPE { get; set; }//;
        
        // FK_NSI_VILLAGE_NSI_OBLAST
        [InverseProperty("NOBLAST_ID")]
        public virtual NSI_OBLAST NSI_OBLAST { get; set; }//;
        
        // FK_NSI_VILLAGE_NSI_OBLAST_REGION
        [InverseProperty("NOBLAST_REGION_ID")]
        public virtual NSI_OBLAST_REGION NSI_OBLAST_REGION { get; set; }//;
        
        // FK_NSI_VILLAGE_NSI_VILLAGE
        [InverseProperty("PARENT_ID")]
        public virtual NSI_VILLAGE NSI_VILLAGE { get; set; }//;
        
        // FK_NSI_VILLAGE_NSI_VILLAGE_TYPE
        [InverseProperty("NVILLAGE_TYPE_ID")]
        public virtual NSI_VILLAGE_TYPE NSI_VILLAGE_TYPE { get; set; }//;
        #endregion
        
        #region Navigation - children
        // FK_BUILD_NSI_VILLAGE1
        public virtual ICollection<BUILD> BUILD { get; set; }//;
        
        // FK_BUILD_NSI_VILLAGE2
        public virtual ICollection<BUILD> BUILD1 { get; set; }//;
        
        // FK_NSI_STREET_NSI_VILLAGE
        public virtual ICollection<NSI_STREET> NSI_STREET { get; set; }//;
        
        // FK_NSI_VILLAGE_NSI_VILLAGE
        public virtual ICollection<NSI_VILLAGE> NSI_VILLAGE { get; set; }//;
        #endregion
        
        #region Constructor
        public NSI_VILLAGE()
        {
            this.BUILD = new HashSet<BUILD>();
            this.BUILD1 = new HashSet<BUILD>();
            this.NSI_STREET = new HashSet<NSI_STREET>();
            this.NSI_VILLAGE = new HashSet<NSI_VILLAGE>();
        }
        #endregion
    }
}
