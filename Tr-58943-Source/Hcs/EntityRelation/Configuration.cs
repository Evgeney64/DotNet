using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace Hcs.Configuration
{
    [Flags]
    public enum LogMode
    {
        None = 0,
        Request = 1,
        Response = 2,
        Trace = 4,
    }
    public class LogConfiguration
    {
        public LogMode Mode { get; set; }
    }
    public class EntityDataSourceConfiguration
    {
        private readonly int defaultCommandTimeout = 300;

        public string HcsConnectionStringName { get; set; }
        public int CommandTimeout { get; set; }
        public LogConfiguration Log { get; private set; }

        public EntityDataSourceConfiguration()
        {
            this.CommandTimeout = this.defaultCommandTimeout;
            this.Log = new LogConfiguration();
        }
    }
}