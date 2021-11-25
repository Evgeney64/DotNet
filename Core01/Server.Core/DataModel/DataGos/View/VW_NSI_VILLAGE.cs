using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Core.Model
{
    public partial class VW_NSI_VILLAGE : village
    {
        public long NVILLAGE_ID { get; set; }
        public string NVILLAGE_NAME { get; set; }
        public int? NVILLAGE_TYPE_ID { get; set; }
        public string NVILLAGE_TYPE_NAME { get; set; }
    }

    public partial class EntityServ
    {
        public IQueryable<VW_NSI_VILLAGE> Get_VW_NSI_VILLAGE()
        {
            IQueryable<VW_NSI_VILLAGE> items =
                from vil in Context.village
                from tp in Context.type_village.Where(ss => ss.tvillage_id == (int)vil.tvillage_id)
                select new VW_NSI_VILLAGE
                {
                    NVILLAGE_ID = vil.village_id,
                    NVILLAGE_TYPE_ID = vil.tvillage_id,
                    NVILLAGE_NAME = vil.village_name,
                    NVILLAGE_TYPE_NAME = tp.tvillage_name,
                };
            return items;
        }
    }
}
