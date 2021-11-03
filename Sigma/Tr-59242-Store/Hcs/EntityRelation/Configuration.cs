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

        public LogConfiguration()
        {
        }
        public LogConfiguration(LogConfiguration source)
        {
            this.Mode = source.Mode;
        }
    }
}