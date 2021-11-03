using System;
using System.Collections.Generic;

using Server.Core.Public;
namespace Server.Core.CoreModel
{
    public partial class NSI_VILLAGE : IEntityObject, IEntityLog
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public NSI_VILLAGE()
        //{
        //    this.NSI_STREET = new HashSet<NSI_STREET>();
        //    this.NSI_VILLAGE1 = new HashSet<NSI_VILLAGE>();
        //    this.BUILD = new HashSet<BUILD>();
        //    this.BUILD1 = new HashSet<BUILD>();
        //}
    
        [System.ComponentModel.DataAnnotations.KeyAttribute]
    public long NVILLAGE_ID { get; set; }
        public string NVILLAGE_NAME { get; set; }
        public Nullable<long> NVILLAGE_TYPE_ID { get; set; }
        public Nullable<long> NOBLAST_ID { get; set; }
        public Nullable<long> NOBLAST_REGION_ID { get; set; }
        public Nullable<int> NMUNICIPALITY_ID { get; set; }
        public string GNI_CODE { get; set; }
        public string FIAS { get; set; }
        public Nullable<int> NDATA_SOURCE_ID { get; set; }
        public Nullable<long> PARENT_ID { get; set; }
        public Nullable<int> VILLAGE_KIND { get; set; }
        public Nullable<System.DateTime> CRT_DATE { get; set; }
        public Nullable<System.DateTime> MFY_DATE { get; set; }
        public Nullable<int> MFY_SUSER_ID { get; set; }
        public Nullable<int> NMUNICIPALITY_TYPE_ID { get; set; }
    
        /*
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [Newtonsoft.Json.JsonIgnore]
    	public virtual ICollection<NSI_STREET> NSI_STREET { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [Newtonsoft.Json.JsonIgnore]
    	public virtual ICollection<NSI_VILLAGE> NSI_VILLAGE1 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
    	public virtual NSI_VILLAGE NSI_VILLAGE2 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
    	public virtual NSI_VILLAGE_TYPE NSI_VILLAGE_TYPE { get; set; }
        [Newtonsoft.Json.JsonIgnore]
    	public virtual NSI_MUNICIPALITY NSI_MUNICIPALITY { get; set; }
        [Newtonsoft.Json.JsonIgnore]
    	public virtual NSI_OBLAST NSI_OBLAST { get; set; }
        [Newtonsoft.Json.JsonIgnore]
    	public virtual NSI_OBLAST_REGION NSI_OBLAST_REGION { get; set; }
        [Newtonsoft.Json.JsonIgnore]
    	public virtual NSI_MUNICIPALITY_TYPE NSI_MUNICIPALITY_TYPE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [Newtonsoft.Json.JsonIgnore]
    	public virtual ICollection<BUILD> BUILD { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [Newtonsoft.Json.JsonIgnore]
    	public virtual ICollection<BUILD> BUILD1 { get; set; }
        */
        long IEntityObject.Id { get { return NVILLAGE_ID; } }
    }
}
