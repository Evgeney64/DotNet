using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Server.Core.Public
{
    public abstract class SysTableAttribute : Attribute
    {
        public SysTable_Enum SysTable { get; private set; }

        public SysTableAttribute(SysTable_Enum sysTable)
        {
            this.SysTable = sysTable;
        }
    }

    #region Shared
    [AttributeUsage(AttributeTargets.All)]
    public class SelectorTextAttribute : System.Attribute
    {
        public string MainProperty { get; set; }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class SelectorText1_Attribute : System.Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All)]
    public class EntityTextAttribute : System.Attribute
    {
        public EntityTextAttribute()
        {
            ;//Для автогенерации атрибута на клиенте обязательно нужен свой конструктор (http://jeffhandley.com/archive/2010/09/30/RiaServicesValidationAttributePropagation.aspx)
        }
    }
    #endregion

}
