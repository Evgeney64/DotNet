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
    
    
    public partial class EXT_PARAM : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return EXT_PARAM_ID; } }//;
        
        [KeyAttribute()]
        public long EXT_PARAM_ID { get; set; }//;
        
        public System.Nullable<int> STABLE_ID { get; set; }//;
        
        public System.Nullable<long> ID { get; set; }//;
        
        public System.Nullable<int> NPARAM_ID { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_EXT_PARAM_NSI_PARAM
        [InverseProperty("NPARAM_ID")]
        public virtual NSI_PARAM NSI_PARAM { get; set; }//;
        #endregion
        
        #region Navigation - children
        // FK_EXT_PARAM_EXT_PARAM_VALUE
        public virtual ICollection<EXT_PARAM_VALUE> EXT_PARAM_VALUE { get; set; }//;
        #endregion
        
        #region Constructor
        public EXT_PARAM()
        {
            this.EXT_PARAM_VALUE = new HashSet<EXT_PARAM_VALUE>();
        }
        #endregion
    }
}
