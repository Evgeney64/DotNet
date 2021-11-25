using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Server.Core.Model
{
    public partial class VW_NSI_VILLAGE : NSI_VILLAGE
    {
        public string NVILLAGE_TYPE_NAME { get; set; }
    }

    public partial class EntityServ
    {
        public IQueryable<VW_NSI_VILLAGE> Get_VW_NSI_VILLAGE()
        {
            IQueryable<VW_NSI_VILLAGE> items =
                from vil in Context.NSI_VILLAGE
                from tp in Context.NSI_VILLAGE_TYPE.Where(ss => ss.NVILLAGE_TYPE_ID == vil.NVILLAGE_TYPE_ID)
                select new VW_NSI_VILLAGE
                {
                    NVILLAGE_ID = vil.NVILLAGE_ID,
                    NVILLAGE_TYPE_ID = vil.NVILLAGE_TYPE_ID,
                    NVILLAGE_NAME = vil.NVILLAGE_NAME,
                    NVILLAGE_TYPE_NAME = tp.NVILLAGE_TYPE_NAME,
                };
            //string query_sql = (items as System.Data.Entity.Infrastructure.DbQuery<VW_NSI_VILLAGE>).ToString();
            //string query_sql1 = (items as DbQuery<VW_NSI_VILLAGE>).Sql;
            //string query_sql2 = items.ToQueryString();
            return items;
        }
    }
}
