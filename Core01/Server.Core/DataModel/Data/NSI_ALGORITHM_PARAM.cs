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
    
    
    public partial class NSI_ALGORITHM_PARAM : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return NALGORITHM_PARAM_ID; } }//;
        
        [KeyAttribute()]
        public int NALGORITHM_PARAM_ID { get; set; }//;
        
        public int NALGORITHM_ID { get; set; }//;
        
        public int NPARAM_ID { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_NSI_ALGORITHM_PARAM_NSI_ALGORITHM
        [InverseProperty("NALGORITHM_ID")]
        public virtual NSI_ALGORITHM NSI_ALGORITHM { get; set; }//;
        
        // FK_NSI_ALGORITHM_PARAM_NSI_PARAM
        [InverseProperty("NPARAM_ID")]
        public virtual NSI_PARAM NSI_PARAM { get; set; }//;
        #endregion
    }
}
