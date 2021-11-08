using System;
using System.Collections.Generic;

using Server.Core.Public;
namespace Server.Core.Model
{
    public partial class NSI_VILLAGE_TYPE : IEntityObject, IEntityLog
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public NSI_VILLAGE_TYPE()
        //{
        //    this.NSI_VILLAGE = new HashSet<NSI_VILLAGE>();
        //}
    
        [System.ComponentModel.DataAnnotations.KeyAttribute]
    public long NVILLAGE_TYPE_ID { get; set; }
        public string NVILLAGE_TYPE_SNAME { get; set; }
        public string GNI_SOCR { get; set; }
        public string NVILLAGE_TYPE_NAME { get; set; }
        public Nullable<System.DateTime> CRT_DATE { get; set; }
        public Nullable<System.DateTime> MFY_DATE { get; set; }
        public Nullable<int> MFY_SUSER_ID { get; set; }
    
        /*
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [Newtonsoft.Json.JsonIgnore]
    	public virtual ICollection<NSI_VILLAGE> NSI_VILLAGE { get; set; }
        */
        long IEntityObject.Id { get { return NVILLAGE_TYPE_ID; } }
    }
}
