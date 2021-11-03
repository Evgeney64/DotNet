using System;
using System.Collections.Generic;

using Server.Core.Public;
namespace Server.Core.CoreModel
{
    public partial class NSI_STREET : IEntityObject, IEntityLog
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public NSI_STREET()
        //{
        //    this.BUILD = new HashSet<BUILD>();
        //}
    
        [System.ComponentModel.DataAnnotations.KeyAttribute]
        public long NSTREET_ID { get; set; }
        public Nullable<long> NVILLAGE_ID { get; set; }
        public Nullable<long> NSTREET_TYPE_ID { get; set; }
        public string NSTREET_NAME { get; set; }
        public string GNI_CODE { get; set; }
        public string FIAS { get; set; }
        public Nullable<int> NDATA_SOURCE_ID { get; set; }
        public Nullable<System.DateTime> CRT_DATE { get; set; }
        public Nullable<System.DateTime> MFY_DATE { get; set; }
        public Nullable<int> MFY_SUSER_ID { get; set; }

        //[Newtonsoft.Json.JsonIgnore]
        //public virtual NSI_STREET_TYPE NSI_STREET_TYPE { get; set; }
        //[Newtonsoft.Json.JsonIgnore]
        //public virtual NSI_VILLAGE NSI_VILLAGE { get; set; }
        //   [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //   [Newtonsoft.Json.JsonIgnore]
        //public virtual ICollection<BUILD> BUILD { get; set; }

        long IEntityObject.Id { get { return NSTREET_ID; } }
    }
}
