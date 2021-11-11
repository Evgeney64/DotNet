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
    
    
    public partial class NSI_ALGORITHM_IMPL_BINDING : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return NALGORITHM_IMPL_BINDING_ID; } }//;
        
        [KeyAttribute()]
        public int NALGORITHM_IMPL_BINDING_ID { get; set; }//;
        
        public int NALGORITHM_ID { get; set; }//;
        
        public int NALGORITHM_IMPL_ID { get; set; }//;
        
        public int SMODULE_ID { get; set; }//;
        
        public int SCLIENT_ID { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_NSI_ALGORITHM_IMPL_BINDING_NSI_ALGORITHM_IMPL
        [InverseProperty("NALGORITHM_IMPL_ID")]
        public virtual NSI_ALGORITHM_IMPL NSI_ALGORITHM_IMPL { get; set; }//;
        
        // FK_NSI_ALGORITHM_IMPL_BINDING_NSI_ALGORITHM_IMPL_Dummy
        [InverseProperty("NALGORITHM_ID")]
        public virtual NSI_ALGORITHM_IMPL NSI_ALGORITHM_IMPL1 { get; set; }//;
        
        // FK_NSI_ALGORITHM_IMPL_BINDING_NSI_ALGORITHM_IMPL_Dummy
        [InverseProperty("NALGORITHM_ID")]
        public virtual NSI_ALGORITHM_IMPL NSI_ALGORITHM_IMPL2 { get; set; }//;
        
        // FK_NSI_ALGORITHM_IMPL_BINDING_SYS_CLIENT
        [InverseProperty("SCLIENT_ID")]
        public virtual SYS_CLIENT SYS_CLIENT { get; set; }//;
        
        // FK_NSI_ALGORITHM_IMPL_BINDING_SYS_MODULE
        [InverseProperty("SMODULE_ID")]
        public virtual SYS_MODULE SYS_MODULE { get; set; }//;
        #endregion
    }
}