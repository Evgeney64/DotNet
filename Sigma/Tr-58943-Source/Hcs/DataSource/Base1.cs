using System;
using System.Collections.Generic;
using System.Text;

using Hcs.Configuration;

namespace Hcs.DataSource
{
    public class StoredProcDataSourceConfiguration : EntityDataSourceConfiguration
    {
        private readonly Dictionary<SysOperationCode, StoredProcConfiguration> storedProcs = new Dictionary<SysOperationCode, StoredProcConfiguration>();

        public string ExternalConnectionStringName { get; set; }
        public StoredProcConfiguration this[SysOperationCode operation]
        {
            get
            {
                if (this.storedProcs.ContainsKey(operation))
                {
                    return this.storedProcs[operation];
                }
                return null;
            }
            set
            {
                this.storedProcs[operation] = value;
            }
        }

        public StoredProcDataSourceConfiguration()
        {
        }
    }

    public class StoredProcConfiguration
    {
        public string PrepareProcedureName { get; set; }
        public string ResultProcedureName { get; set; }
        public string ListProcedureName { get; set; }
    }
}
