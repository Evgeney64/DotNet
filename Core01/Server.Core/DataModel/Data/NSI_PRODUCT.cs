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
    
    
    public partial class NSI_PRODUCT : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return NPRODUCT_ID; } }//;
        
        [KeyAttribute()]
        public int NPRODUCT_ID { get; set; }//;
        
        public string NPRODUCT_NAME { get; set; }//;
        
        public System.Nullable<short> ACTIVE { get; set; }//;
        
        public System.Nullable<long> NUNIT_ID { get; set; }//;
        
        public System.Nullable<System.DateTime> DATE_START { get; set; }//;
        
        public System.Nullable<short> ENE_CARRIER { get; set; }//;
        
        public System.Nullable<short> PARTNER_EXISTS { get; set; }//;
        
        public System.Nullable<short> PRODUCT_KIND_CHANGE { get; set; }//;
        
        public string NAME_IMAGE { get; set; }//;
        
        public System.Nullable<int> PARENT_ID { get; set; }//;
        
        public string NPRODUCT_SNAME { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_NSI_PRODUCT_NSI_PRODUCT
        [InverseProperty("PARENT_ID")]
        public virtual NSI_PRODUCT NSI_PRODUCT1 { get; set; }//;
        #endregion
        
        #region Navigation - children
        // FK_DOCUMENT_NSI_PRODUCT
        public virtual ICollection<DOCUMENT> DOCUMENT { get; set; }//;
        
        // FK_NSI_CALC_NSI_PRODUCT
        public virtual ICollection<NSI_CALC> NSI_CALC { get; set; }//;
        
        // FK_NSI_METER_NSI_PRODUCT
        public virtual ICollection<NSI_METER> NSI_METER { get; set; }//;
        
        // FK_NSI_PRODUCT_NSI_PRODUCT
        public virtual ICollection<NSI_PRODUCT> NSI_PRODUCT2 { get; set; }//;
        #endregion
        
        #region Constructor
        public NSI_PRODUCT()
        {
            this.DOCUMENT = new HashSet<DOCUMENT>();
            this.NSI_CALC = new HashSet<NSI_CALC>();
            this.NSI_METER = new HashSet<NSI_METER>();
            this.NSI_PRODUCT2 = new HashSet<NSI_PRODUCT>();
        }
        #endregion
    }
}
