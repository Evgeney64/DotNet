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
    
    
    public partial class NSI_METER : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return NMETER_ID; } }//;
        
        [KeyAttribute()]
        public int NMETER_ID { get; set; }//;
        
        public string NMETER_NAME { get; set; }//;
        
        public System.Nullable<int> PARAM_COUNT { get; set; }//;
        
        public System.Nullable<int> PHASE_COUNT { get; set; }//;
        
        public System.Nullable<short> ELECTRON { get; set; }//;
        
        public System.Nullable<int> CONSTPARAMS { get; set; }//;
        
        public System.Nullable<int> NFACTORY_ID { get; set; }//;
        
        public System.Nullable<int> PERIOD_CHECK { get; set; }//;
        
        public System.Nullable<int> HIGHNOM_CUR { get; set; }//;
        
        public System.Nullable<decimal> ACCURACY_GRADE { get; set; }//;
        
        public System.Nullable<int> NPRODUCT_ID { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        
        public System.Nullable<int> FORMAT_ALL { get; set; }//;
        
        public System.Nullable<int> FORMAT_AFTER { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_NSI_METER_NSI_FACTORY
        [InverseProperty("NFACTORY_ID")]
        public virtual NSI_FACTORY NSI_FACTORY { get; set; }//;
        
        // FK_NSI_METER_NSI_PRODUCT
        [InverseProperty("NPRODUCT_ID")]
        public virtual NSI_PRODUCT NSI_PRODUCT { get; set; }//;
        #endregion
        
        #region Navigation - children
        // FK_EQUIPMENT_METER_NSI_METER
        public virtual ICollection<EQUIPMENT_METER> EQUIPMENT_METER { get; set; }//;
        
        // FK_NSI_METER_CONFIG_NSI_METER
        public virtual ICollection<NSI_METER_CONFIG> NSI_METER_CONFIG { get; set; }//;
        
        // FK_NSI_METER_PARAM_NSI_METER
        public virtual ICollection<NSI_METER_PARAM> NSI_METER_PARAM { get; set; }//;
        #endregion
        
        #region Constructor
        public NSI_METER()
        {
            this.EQUIPMENT_METER = new HashSet<EQUIPMENT_METER>();
            this.NSI_METER_CONFIG = new HashSet<NSI_METER_CONFIG>();
            this.NSI_METER_PARAM = new HashSet<NSI_METER_PARAM>();
        }
        #endregion
    }
}