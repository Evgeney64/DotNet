using System;
using System.Collections.Generic;
using Server.Core.Public;

namespace Server.Core.Model
{
    public partial class NSI_STREET_TYPE : IEntityObject, IEntityLog
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public NSI_STREET_TYPE()
        //{
        //    this.NSI_STREET = new HashSet<NSI_STREET>();
        //}
    
        [System.ComponentModel.DataAnnotations.KeyAttribute]
    public long NSTREET_TYPE_ID { get; set; }
        public string NSTREET_TYPE_NAME { get; set; }
        public string GNI_SOCR { get; set; }
        public Nullable<System.DateTime> CRT_DATE { get; set; }
        public Nullable<System.DateTime> MFY_DATE { get; set; }
        public Nullable<int> MFY_SUSER_ID { get; set; }
    
     //   [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
     //   [Newtonsoft.Json.JsonIgnore]
    	//public virtual ICollection<NSI_STREET> NSI_STREET { get; set; }
        
        long IEntityObject.Id { get { return NSTREET_TYPE_ID; } }
    }
}
