using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Server.Core.Public;

namespace Server.Core.CoreModel
{
    public partial class village : IEntityObject
    {
        [KeyAttribute]
        public int village_id { get; set; }
        public string village_name { get; set; }
        public Nullable<int> tvillage_id { get; set; }
        public Nullable<int> rgn_id { get; set; }
        [ForeignKey("rgn_id")]
        public virtual rgn rgn { get; set; }
        long IEntityObject.Id { get { return village_id; } }

        public override string ToString()
        {
            return village_id + " [" + rgn_id + "] - " + village_name;
        }
    }

    public partial class rgn : IEntityObject
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public rgn()
        {
            this.villages = new HashSet<village>();
        }

        [KeyAttribute]
        public int rgn_id { get; set; }
        public string rgn_name { get; set; }
        public virtual ICollection<village> villages { get; set; }
        long IEntityObject.Id { get { return rgn_id; } }

        public override string ToString()
        {
            return rgn_id + " - " + rgn_name;
        }
    }
}
