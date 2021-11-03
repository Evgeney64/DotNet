using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Core.Types
{
    public class DataSourceConfiguration
    {
        public string ConnectionString { get; set; }
        public int CommandTimeout { get; set; }
        public bool is_postgres { get; set; }
    }
}
