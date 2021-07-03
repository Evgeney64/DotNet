using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Server.Core.Public
{
    public enum ConnectionType_Enum
    {
        None = 0,
        Auth = 1,
        Data = 2,
    }
}



