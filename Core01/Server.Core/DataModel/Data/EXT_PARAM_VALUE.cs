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
    
    
    public partial class EXT_PARAM_VALUE : IEntityObject, IEntityLog, IEntityPeriod
    {
        
        #region Columns
        long IEntityObject.Id { get { return EXT_PARAM_VALUE_ID; } }//;
        
        [KeyAttribute()]
        public long EXT_PARAM_VALUE_ID { get; set; }//;
        
        public System.Nullable<long> EXT_PARAM_ID { get; set; }//;
        
        public System.DateTime DATE_BEG { get; set; }//;
        
        public System.Nullable<System.DateTime> DATE_END { get; set; }//;
        
        public string VALUE { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_EXT_PARAM_EXT_PARAM_VALUE
        [InverseProperty("EXT_PARAM_ID")]
        public virtual EXT_PARAM EXT_PARAM { get; set; }//;
        #endregion
    }
}
