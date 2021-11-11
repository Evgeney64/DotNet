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
    
    
    public partial class PARTNER : IEntityObject, IEntityLog, IEntityPeriod
    {
        
        #region Columns
        long IEntityObject.Id { get { return PARTNER_ID; } }//;
        
        [KeyAttribute()]
        public long PARTNER_ID { get; set; }//;
        
        public string PARTNER_NUM { get; set; }//;
        
        public string CONTACT_INFO { get; set; }//;
        
        public System.DateTime DATE_BEG { get; set; }//;
        
        public System.Nullable<System.DateTime> DATE_END { get; set; }//;
        
        public System.Nullable<short> FOREIGNER { get; set; }//;
        
        public System.Nullable<int> NPARTNER_ID { get; set; }//;
        
        public string FAM { get; set; }//;
        
        public string IM { get; set; }//;
        
        public string OT { get; set; }//;
        
        public string IO { get; set; }//;
        
        public System.Nullable<System.DateTime> DATE_BORN { get; set; }//;
        
        public System.Nullable<System.DateTime> DATE_DEATH { get; set; }//;
        
        public string PLACE_BORN { get; set; }//;
        
        public System.Nullable<short> SEX { get; set; }//;
        
        public string POLICE { get; set; }//;
        
        public System.Nullable<System.DateTime> DATE_POLICE { get; set; }//;
        
        public System.Nullable<int> OLD_ID { get; set; }//;
        
        public string RG_NUM { get; set; }//;
        
        public System.Nullable<System.DateTime> RG_DT { get; set; }//;
        
        public System.Nullable<int> OKONH { get; set; }//;
        
        public string OKVED { get; set; }//;
        
        public string OKPO { get; set; }//;
        
        public string COATO { get; set; }//;
        
        public string SOOGU { get; set; }//;
        
        public string KFS { get; set; }//;
        
        public string KOPF { get; set; }//;
        
        public System.Nullable<int> BUDGET_ID { get; set; }//;
        
        public string POST_BOX { get; set; }//;
        
        public string OGRN { get; set; }//;
        
        public string KPP { get; set; }//;
        
        public string NAME_CASE_R { get; set; }//;
        
        public string NAME_CASE_D { get; set; }//;
        
        public string FULL_NAME_CASE_R { get; set; }//;
        
        public string FULL_NAME_CASE_D { get; set; }//;
        
        public string PARTNER_NAME_CALC { get; set; }//;
        
        public System.Nullable<short> RECALL_SHOW { get; set; }//;
        
        public System.Nullable<short> RECALL_DEBT { get; set; }//;
        
        public System.Nullable<short> RECALL_SMS { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        
        public string ADRESS { get; set; }//;
        
        public System.Nullable<long> CURR_STATE_ID { get; set; }//;
        
        public System.Nullable<long> CURR_EVENT_ID { get; set; }//;
        
        public System.Guid SYN_GUID { get; set; }//;
        
        public string RG_SER { get; set; }//;
        #endregion
        
        #region Navigation - children
        // FK_DEAL_PARTNER
        public virtual ICollection<DEAL> DEAL { get; set; }//;
        
        // FK_DEAL_PARTNER1
        public virtual ICollection<DEAL> DEAL1 { get; set; }//;
        
        // FK_DEAL_EXT_PARTNER1
        public virtual ICollection<DEAL_EXT> DEAL_EXT { get; set; }//;
        
        // FK_DEAL_EXT_PARTNER2
        public virtual ICollection<DEAL_EXT> DEAL_EXT1 { get; set; }//;
        
        // FK_DEAL_EXT_PARTNER3
        public virtual ICollection<DEAL_EXT> DEAL_EXT2 { get; set; }//;
        
        // FK_DEAL_EXT_PARTNER_1
        public virtual ICollection<DEAL_EXT> DEAL_EXT3 { get; set; }//;
        
        // FK_DOCUMENT_PARTNER
        public virtual ICollection<DOCUMENT> DOCUMENT { get; set; }//;
        
        // FK_DOCUMENT_ITEM_PARTNER
        public virtual ICollection<DOCUMENT_ITEM> DOCUMENT_ITEM { get; set; }//;
        
        // FK_EVENT_PARTNER3
        public virtual ICollection<EVENT> EVENT { get; set; }//;
        
        // FK_EVENT_PARTNER
        public virtual ICollection<EVENT> EVENT1 { get; set; }//;
        
        // FK_EVENT_PARTNER1
        public virtual ICollection<EVENT> EVENT2 { get; set; }//;
        
        // FK_EVENT_PARTNER2
        public virtual ICollection<EVENT> EVENT3 { get; set; }//;
        
        // FK_EVENT_STATE_PARTNER
        public virtual ICollection<EVENT_STATE> EVENT_STATE { get; set; }//;
        
        // FK_NSI_ACCOUNT_PERIOD_PARTNER
        public virtual ICollection<NSI_ACCOUNT_PERIOD> NSI_ACCOUNT_PERIOD { get; set; }//;
        
        // FK_NSI_DISTRICT_NSI_DISTRICT
        public virtual ICollection<NSI_DISTRICT> NSI_DISTRICT { get; set; }//;
        
        // FK_PARTNER_CONTA_PARTNER
        public virtual ICollection<PARTNER_CONTACT> PARTNER_CONTACT { get; set; }//;
        
        // FK_PARTNER_EXT_PARTNER
        public virtual ICollection<PARTNER_EXT> PARTNER_EXT { get; set; }//;
        
        // FK_PARTNER_RELA_PARTNER
        public virtual ICollection<PARTNER_RELATION> PARTNER_RELATION { get; set; }//;
        
        // FK_SYS_USER_PARTNER
        public virtual ICollection<SYS_USER> SYS_USER { get; set; }//;
        #endregion
        
        #region Constructor
        public PARTNER()
        {
            this.DEAL = new HashSet<DEAL>();
            this.DEAL1 = new HashSet<DEAL>();
            this.DEAL_EXT = new HashSet<DEAL_EXT>();
            this.DEAL_EXT1 = new HashSet<DEAL_EXT>();
            this.DEAL_EXT2 = new HashSet<DEAL_EXT>();
            this.DEAL_EXT3 = new HashSet<DEAL_EXT>();
            this.DOCUMENT = new HashSet<DOCUMENT>();
            this.DOCUMENT_ITEM = new HashSet<DOCUMENT_ITEM>();
            this.EVENT = new HashSet<EVENT>();
            this.EVENT1 = new HashSet<EVENT>();
            this.EVENT2 = new HashSet<EVENT>();
            this.EVENT3 = new HashSet<EVENT>();
            this.EVENT_STATE = new HashSet<EVENT_STATE>();
            this.NSI_ACCOUNT_PERIOD = new HashSet<NSI_ACCOUNT_PERIOD>();
            this.NSI_DISTRICT = new HashSet<NSI_DISTRICT>();
            this.PARTNER_CONTACT = new HashSet<PARTNER_CONTACT>();
            this.PARTNER_EXT = new HashSet<PARTNER_EXT>();
            this.PARTNER_RELATION = new HashSet<PARTNER_RELATION>();
            this.SYS_USER = new HashSet<SYS_USER>();
        }
        #endregion
    }
}
