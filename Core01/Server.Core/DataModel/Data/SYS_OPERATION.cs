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
    
    
    public partial class SYS_OPERATION : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return SOPERATION_ID; } }//;
        
        [KeyAttribute()]
        public int SOPERATION_ID { get; set; }//;
        
        public string SOPERATION_NAME { get; set; }//;
        
        public string NAME_IMAGE { get; set; }//;
        
        public string DATA { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        
        public System.Nullable<int> NODE_TYPE_ID { get; set; }//;
        
        public System.Nullable<int> PARENT_ID { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_SYS_OPERATION_PARENT
        [InverseProperty("PARENT_ID")]
        public virtual SYS_OPERATION SYS_OPERATION1 { get; set; }//;
        #endregion
        
        #region Navigation - children
        // FK_SYS_OPERATION_PARENT
        public virtual ICollection<SYS_OPERATION> SYS_OPERATION2 { get; set; }//;
        #endregion
        
        #region Constructor
        public SYS_OPERATION()
        {
            this.SYS_OPERATION2 = new HashSet<SYS_OPERATION>();
        }
        #endregion
    }
}
