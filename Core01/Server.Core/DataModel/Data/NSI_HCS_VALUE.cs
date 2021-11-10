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
    
    
    public partial class NSI_HCS_VALUE : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return NHCS_VALUE_ID; } }//;
        
        [KeyAttribute()]
        public int NHCS_VALUE_ID { get; set; }//;
        
        public int NHCS_ID { get; set; }//;
        
        public System.Nullable<int> SOURCE_ID { get; set; }//;
        
        public string CODE { get; set; }//;
        
        public System.Guid GUID { get; set; }//;
        
        public System.DateTime MODIFIED { get; set; }//;
        
        public string VALUE { get; set; }//;
        
        public string DETAIL { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_NSI_HCS_VALUE_NSI_HCS
        [InverseProperty("NHCS_ID")]
        public virtual NSI_HCS NSI_HCS { get; set; }//;
        #endregion
    }
}
