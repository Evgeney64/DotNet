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
    
    
    public partial class PARTNER_RELATION : IEntityObject, IEntityLog, IEntityPeriod
    {
        
        #region Columns
        long IEntityObject.Id { get { return PARTNER_RELATION_ID; } }//;
        
        [KeyAttribute()]
        public long PARTNER_RELATION_ID { get; set; }//;
        
        public System.Nullable<long> PARTNER_ID { get; set; }//;
        
        public System.Nullable<long> CHILD_ID { get; set; }//;
        
        public System.Nullable<int> NPARTNER_RELATION_ID { get; set; }//;
        
        public string COMMENT { get; set; }//;
        
        public System.DateTime DATE_BEG { get; set; }//;
        
        public System.Nullable<System.DateTime> DATE_END { get; set; }//;
        
        public System.Nullable<long> PARTNER_CONTACT_ID { get; set; }//;
        
        public System.Nullable<int> NPARTNER_FUNCTION_ID { get; set; }//;
        
        public string PARTNER_RELATION_NAME { get; set; }//;
        
        public string NAME_CASE_R { get; set; }//;
        
        public string NAME_CASE_D { get; set; }//;
        
        public string DETAIL { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        
        public System.Nullable<long> DOCUMENT_ID { get; set; }//;
        #endregion
        
        #region Navigation - parents
        // FK_PARTNER_RELATION_DOCUMENT
        [InverseProperty("DOCUMENT_ID")]
        public virtual DOCUMENT DOCUMENT { get; set; }//;
        
        // FK_PARTNER_RELA_PARTNER
        [InverseProperty("PARTNER_ID")]
        public virtual PARTNER PARTNER { get; set; }//;
        
        // FK_PARTNER_RELATION_PARTNER_CONTACT
        [InverseProperty("PARTNER_CONTACT_ID")]
        public virtual PARTNER_CONTACT PARTNER_CONTACT { get; set; }//;
        #endregion
    }
}
