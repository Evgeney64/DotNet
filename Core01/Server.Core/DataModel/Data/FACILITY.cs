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
    
    public partial class FACILITY : IEntityObject, IEntityLog, IEntityPeriod
    {
        #region Columns
        long IEntityObject.Id { get { return FACILITY_ID; } }//;
        [KeyAttribute()]
        public long FACILITY_ID { get; set; }//;
        public System.Nullable<long> PARENT_ID { get; set; }//;
        public System.Nullable<long> BUILD_ID { get; set; }//;
        public string FACILITY_NAME { get; set; }//;
        public System.Nullable<int> NFACILITY_ID { get; set; }//;
        public System.DateTime DATE_BEG { get; set; }//;
        public System.Nullable<System.DateTime> DATE_END { get; set; }//;
        public string COMMENT { get; set; }//;
        public string FLAT { get; set; }//;
        public string NUM { get; set; }//;
        public System.Nullable<int> CLOSE_NREAZON_ID { get; set; }//;
        public string CALC_PARAM { get; set; }//;
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        public string DETAIL { get; set; }//;
        public System.Nullable<int> FUNCTION_TYPE { get; set; }//;
        public string CADASTRE { get; set; }//;
        public string ADDRESS { get; set; }//;
        public string FLOOR { get; set; }//;
        public string PORCH { get; set; }//;
        public System.Nullable<decimal> COORD_X { get; set; }//;
        public System.Nullable<decimal> COORD_Y { get; set; }//;
        public string IMG { get; set; }//;
        public System.Nullable<long> CURR_STATE_ID { get; set; }//;
        public System.Nullable<long> CURR_EVENT_ID { get; set; }//;
        public System.Nullable<int> NDISTRICT_ID { get; set; }//;
        public string ROOM { get; set; }//;
        #endregion
        #region Navigation - parents
        // 
        // FK_FACILITY_BUILD   [BUILD.BUILD_ID]
        [ForeignKey("BUILD_ID")]
        public virtual BUILD BUILD { get; set; }//;
        // 
        // FK_FACILITY_FACILITY   [FACILITY.FACILITY_ID]   #1
        [ForeignKey("PARENT_ID")]
        public virtual FACILITY FACILITY1 { get; set; }//;
        // 
        // FK_FACILITY_NSI_FACILITY   [NSI_FACILITY.NFACILITY_ID]
        [ForeignKey("NFACILITY_ID")]
        public virtual NSI_FACILITY NSI_FACILITY { get; set; }//;
        #endregion
        #region Navigation - children
        // 
        // FK_DOCUMENT_FACILITY   [DOCUMENT.FACILITY_ID]
        public virtual ICollection<DOCUMENT> DOCUMENT { get; set; }//;
        // 
        // FK_EVENT_FACILITY   [EVENT.FACILITY_ID]
        public virtual ICollection<EVENT> EVENT { get; set; }//;
        // 
        // FK_EVENT_STATE_FACILITY   [EVENT_STATE.FACILITY_ID]
        public virtual ICollection<EVENT_STATE> EVENT_STATE { get; set; }//;
        // 
        // FK_FACILITY_FACILITY   [FACILITY.PARENT_ID]   #2
        public virtual ICollection<FACILITY> FACILITY2 { get; set; }//;
        #endregion
        #region Constructor
        public FACILITY()
        {
            this.EVENT = new HashSet<EVENT>();
            this.EVENT_STATE = new HashSet<EVENT_STATE>();
            this.FACILITY2 = new HashSet<FACILITY>();
            this.DOCUMENT = new HashSet<DOCUMENT>();
        }
        #endregion
    }
}
