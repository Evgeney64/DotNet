#region
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
//using System.Data.Metadata.Edm;
//using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using Hcs;
using Hcs.Configuration;
using Hcs.Model;
using Microsoft.Extensions.Configuration;
#endregion

namespace Hcs.Stores
{
    public partial class EntityDataStore : IDataStore, IDisposable, ILoggable
    {
        public string GetSysOperation()
        {
            //using (HcsContext context = new HcsContext(configuration.ConnectionStringName, is_postgres))
            using (HcsContext context = this.CreateContext())
            {
                string str = "";
                var items = context.SysOperation;
                if (items != null)
                {
                    try
                    {
                        List<SysOperation> itemsL = items.ToList();
                        { }
                        foreach (SysOperation sop in itemsL)
                        {
                            str += "<p>" + sop.OperationId + " - " + sop.OperationName + "</p>";
                        }
                    }
                    catch (Exception ex)
                    { }
                }
                return str;
            }
        }

        public string GetSysTransaction()
        {
            //using (HcsContext context = new HcsContext(configuration.ConnectionStringName, is_postgres))
            using (HcsContext context = this.CreateContext())
            {
                string str = "";
                var items = context.SysTransaction;
                if (items != null)
                {
                    try
                    {
                        List<SysTransaction> itemsL = items.ToList();
                        { }
                        foreach (SysTransaction sop in itemsL)
                        {
                            str += "<p>" + sop.TransactionGUID + "</p>";
                        }
                    }
                    catch (Exception ex)
                    { }
                }
                return str;
            }
        }

    }
}
