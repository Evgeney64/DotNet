//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tsb.WCF.Web.CoreModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class EQUIPMENT_METER : IEntityObject, IEntityLog
    {
        [System.ComponentModel.DataAnnotations.KeyAttribute]
    public long EQUIPMENT_ID { get; set; }
        public Nullable<int> NMETER_ID { get; set; }
        public string ZAV_NUM { get; set; }
        public Nullable<System.DateTime> MANUFACT_DATE { get; set; }
        public Nullable<System.DateTime> CHECK_DATE { get; set; }
        public Nullable<System.DateTime> USE_DATE_END { get; set; }
        public Nullable<int> NMETER_TIMER_ID { get; set; }
        public string TIMER_NUM { get; set; }
        public Nullable<System.DateTime> DATE_EPD { get; set; }
        public string EPD { get; set; }
        public Nullable<long> PARTNER_PERS_ID { get; set; }
        public Nullable<System.DateTime> CRT_DATE { get; set; }
        public Nullable<System.DateTime> MFY_DATE { get; set; }
        public Nullable<int> MFY_SUSER_ID { get; set; }
    
        [Newtonsoft.Json.JsonIgnore]
    	public virtual NSI_METER NSI_METER { get; set; }
        
        long IEntityObject.Id { get { return EQUIPMENT_ID; } }
    }
}