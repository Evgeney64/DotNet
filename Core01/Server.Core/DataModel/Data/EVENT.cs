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
    
    
    public partial class EVENT : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return EVENT_ID; } }//;
        
        [KeyAttribute()]
        public long EVENT_ID { get; set; }//;
        
        public System.Nullable<long> PARTNER_ID { get; set; }//;
        
        public System.Nullable<int> NINFO_TRANSFER_ID { get; set; }//;
        
        public System.Nullable<int> NEVENT_IMPORTANT_ID { get; set; }//;
        
        public System.Nullable<long> FACILITY_ID { get; set; }//;
        
        public System.Nullable<int> NEVENT_TOPIC_ID { get; set; }//;
        
        public System.Nullable<System.DateTime> DATE_BEG { get; set; }//;
        
        public string COMMENT { get; set; }//;
        
        public System.Nullable<int> NPERSONAL_ID { get; set; }//;
        
        public System.Nullable<long> DEAL_ID { get; set; }//;
        
        public string DETAIL { get; set; }//;
        
        public System.Nullable<int> NDATA_SOURCE_ID { get; set; }//;
        
        public System.Nullable<long> PARTNER_CONTACT_ID { get; set; }//;
        
        public string FROM_ADRESS { get; set; }//;
        
        public string FROM_NAME { get; set; }//;
        
        public string TOPIC { get; set; }//;
        
        public string PHONE { get; set; }//;
        
        public System.Nullable<long> COURIER_PARTNER_ID { get; set; }//;
        
        public System.Nullable<int> NEVENT_ID { get; set; }//;
        
        public System.Nullable<System.DateTime> DATE_PLAN { get; set; }//;
        
        public System.Nullable<decimal> SUMMA { get; set; }//;
        
        public string TO_ADRESS { get; set; }//;
        
        public string COURT_CASE_NUM { get; set; }//;
        
        public System.Nullable<System.DateTime> COURT_CASE_DATE_BEG { get; set; }//;
        
        public System.Nullable<System.DateTime> COURT_CASE_DATE_DECISION { get; set; }//;
        
        public System.Nullable<int> COURT_CASE_ORDER { get; set; }//;
        
        public System.Nullable<int> COURT_CASE_TYPE { get; set; }//;
        
        public System.Nullable<long> COURT_CASE_PARTNER_ID { get; set; }//;
        
        public string SP_URL { get; set; }//;
        
        public System.Nullable<long> DOCUMENT_ID { get; set; }//;
        
        public System.Nullable<int> COURT_CASE_DECISION { get; set; }//;
        
        public System.Nullable<long> FROM_PARTNER_ID { get; set; }//;
        
        public System.Nullable<long> TO_PARTNER_ID { get; set; }//;
        
        public string LETTER_NUM { get; set; }//;
        
        public System.Nullable<System.DateTime> LETTER_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> ISSUE_DATE { get; set; }//;
        
        public string DOC_NUM { get; set; }//;
        
        public System.Nullable<long> CURR_EVENT_STATE_ID { get; set; }//;
        
        public System.Nullable<long> DOCUMENT_ITEM_ID { get; set; }//;
        
        public System.Nullable<int> NEVENT_SUBTOPIC_ID { get; set; }//;
        
        public System.Nullable<decimal> DURATION_FACT { get; set; }//;
        
        public System.Nullable<decimal> DURATION_PLAN { get; set; }//;
        
        public System.Nullable<System.DateTime> START_DATE_PLAN { get; set; }//;
        
        public System.Nullable<long> PARTNER_NOTIFY_ID { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        
        public System.Nullable<long> PROCESS_ID { get; set; }//;
        
        public System.Nullable<long> CALC_ITEM_ID { get; set; }//;
        
        public string URL { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_EVENT_DEAL
        [InverseProperty("DEAL_ID")]
        public virtual DEAL DEAL { get; set; }//;
        
        // FK_EVENT_DOCUMENT
        [InverseProperty("DOCUMENT_ID")]
        public virtual DOCUMENT DOCUMENT { get; set; }//;
        
        // FK_EVENT_DOCUMENT_ITEM
        [InverseProperty("DOCUMENT_ITEM_ID")]
        public virtual DOCUMENT_ITEM DOCUMENT_ITEM { get; set; }//;
        
        // FK_EVENT_PROCESS_EVENT
        [InverseProperty("PROCESS_ID")]
        public virtual EVENT EVENT1 { get; set; }//;
        
        // FK_EVENT_FACILITY
        [InverseProperty("FACILITY_ID")]
        public virtual FACILITY FACILITY { get; set; }//;
        
        // FK_EVENT_NSI_DATA_SOURCE
        [InverseProperty("NDATA_SOURCE_ID")]
        public virtual NSI_DATA_SOURCE NSI_DATA_SOURCE { get; set; }//;
        
        // FK_EVENT_NSI_EVENT
        [InverseProperty("NEVENT_ID")]
        public virtual NSI_EVENT NSI_EVENT { get; set; }//;
        
        // FK_EVENT_NSI_EVENT1
        [InverseProperty("NINFO_TRANSFER_ID")]
        public virtual NSI_EVENT NSI_EVENT1 { get; set; }//;
        
        // FK_EVENT_NSI_EVENT_SUBTOPIC
        [InverseProperty("NEVENT_SUBTOPIC_ID")]
        public virtual NSI_EVENT_SUBTOPIC NSI_EVENT_SUBTOPIC { get; set; }//;
        
        // FK_EVENT_NSI_EVENT_TOPIC
        [InverseProperty("NEVENT_TOPIC_ID")]
        public virtual NSI_EVENT_TOPIC NSI_EVENT_TOPIC { get; set; }//;
        
        // FK_EVENT_PARTNER3
        [InverseProperty("TO_PARTNER_ID")]
        public virtual PARTNER PARTNER { get; set; }//;
        
        // FK_EVENT_PARTNER
        [InverseProperty("PARTNER_ID")]
        public virtual PARTNER PARTNER1 { get; set; }//;
        
        // FK_EVENT_PARTNER1
        [InverseProperty("COURIER_PARTNER_ID")]
        public virtual PARTNER PARTNER2 { get; set; }//;
        
        // FK_EVENT_PARTNER2
        [InverseProperty("FROM_PARTNER_ID")]
        public virtual PARTNER PARTNER3 { get; set; }//;
        
        // FK_EVENT_PARTNER_CONTACT
        [InverseProperty("PARTNER_CONTACT_ID")]
        public virtual PARTNER_CONTACT PARTNER_CONTACT { get; set; }//;
        #endregion
        
        #region Navigation - children
        // FK_EVENT_PROCESS_EVENT
        public virtual ICollection<EVENT> EVENT2 { get; set; }//;
        
        // FK_EVENT_SCHEM_EVENT
        public virtual ICollection<EVENT_SCHEM> EVENT_SCHEM { get; set; }//;
        
        // FK_EVENT_SCHEM_EVENT1
        public virtual ICollection<EVENT_SCHEM> EVENT_SCHEM1 { get; set; }//;
        
        // FK_EVENT_STATE_EVENT
        public virtual ICollection<EVENT_STATE> EVENT_STATE { get; set; }//;
        
        // FK_SYS_USER_CALL_EVENT
        public virtual ICollection<SYS_USER_CALL> SYS_USER_CALL { get; set; }//;
        #endregion
        
        #region Constructor
        public EVENT()
        {
            this.EVENT2 = new HashSet<EVENT>();
            this.EVENT_SCHEM = new HashSet<EVENT_SCHEM>();
            this.EVENT_SCHEM1 = new HashSet<EVENT_SCHEM>();
            this.EVENT_STATE = new HashSet<EVENT_STATE>();
            this.SYS_USER_CALL = new HashSet<SYS_USER_CALL>();
        }
        #endregion
    }
}
