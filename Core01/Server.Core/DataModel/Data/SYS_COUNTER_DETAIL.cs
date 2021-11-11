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
    
    
    public partial class SYS_COUNTER_DETAIL : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return SCOUNTER_DETAIL_ID; } }//;
        
        [KeyAttribute()]
        public long SCOUNTER_DETAIL_ID { get; set; }//;
        
        public int SCOUNTER_ID { get; set; }//;
        
        public long ID { get; set; }//;
        
        public long VALUE { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        
        public int STABLE_ID { get; set; }//;
        
        public System.Nullable<System.DateTime> DATE_BEG { get; set; }//;
        
        public System.Nullable<System.DateTime> DATE_END { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_SYS_COUNTER_DETAIL_SYS_COUNTER
        [InverseProperty("SCOUNTER_ID")]
        public virtual SYS_COUNTER SYS_COUNTER { get; set; }//;
        
        // FK_SYS_COUNTER_DETAIL_SYS_TABLE
        [InverseProperty("STABLE_ID")]
        public virtual SYS_TABLE SYS_TABLE { get; set; }//;
        #endregion
        
        #region Navigation - children
        // FK_SYS_COUNTER_DETAIL_FREE_NUM_SYS_COUNTER_DETAIL
        public virtual ICollection<SYS_COUNTER_DETAIL_FREE_NUM> SYS_COUNTER_DETAIL_FREE_NUM { get; set; }//;
        
        // FK_SYS_COUNTER_DETAIL_FREE_NUM_SYS_COUNTER_DETAIL
        public virtual ICollection<SYS_COUNTER_DETAIL_FREE_NUM> SYS_COUNTER_DETAIL_FREE_NUM1 { get; set; }//;
        #endregion
        
        #region Constructor
        public SYS_COUNTER_DETAIL()
        {
            this.SYS_COUNTER_DETAIL_FREE_NUM = new HashSet<SYS_COUNTER_DETAIL_FREE_NUM>();
            this.SYS_COUNTER_DETAIL_FREE_NUM1 = new HashSet<SYS_COUNTER_DETAIL_FREE_NUM>();
        }
        #endregion
    }
}
