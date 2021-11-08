using System;
using System.Linq;

namespace Server.Core.Model
{
    public partial class VW_NSI_STREET : NSI_STREET
    {
        public string NSTREET_TYPE_NAME { get; set; }

        public string NVILLAGE_NAME { get; set; }
        public Nullable<long> NVILLAGE_TYPE_ID { get; set; }
        public string NVILLAGE_TYPE_NAME { get; set; }
    }

    public partial class EntityServ
    {
        public IQueryable<VW_NSI_STREET> Get_VW_NSI_STREET()
        {
            IQueryable<VW_NSI_STREET> items =
                from str in Context.NSI_STREET
                from tps in Context.NSI_STREET_TYPE.Where(ss => ss.NSTREET_TYPE_ID == str.NSTREET_TYPE_ID)

                from vil in Context.NSI_VILLAGE.Where(ss => ss.NVILLAGE_ID == str.NVILLAGE_ID)
                from tpv in Context.NSI_VILLAGE_TYPE.Where(ss => ss.NVILLAGE_TYPE_ID == vil.NVILLAGE_TYPE_ID)
                select new VW_NSI_STREET
                {
                    NSTREET_ID = str.NSTREET_ID,
                    NSTREET_TYPE_ID = str.NSTREET_TYPE_ID,
                    NSTREET_NAME = str.NSTREET_NAME,
                    NSTREET_TYPE_NAME = tps.NSTREET_TYPE_NAME,

                    NVILLAGE_ID = vil.NVILLAGE_ID,
                    NVILLAGE_TYPE_ID = vil.NVILLAGE_TYPE_ID,
                    NVILLAGE_NAME = vil.NVILLAGE_NAME,
                    NVILLAGE_TYPE_NAME = tpv.NVILLAGE_TYPE_NAME,
                };
            return items;
        }
    }
}
