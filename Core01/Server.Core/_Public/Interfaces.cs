using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Server.Core.Model
{
    public interface IEntityObject
    {
        long Id { get; }
    }

    public interface IEntityError
    {
        bool Error { get; }
        string ErrorMessage { get; }
    }

    public interface IGroupFilter
    {
        //IQueryable GetObjectsByFilter(XElement filter);
        IQueryable<long> GetObjectIdsByFilter(SysTable_Enum sysTable, XElement filter);
        IQueryable<long> GetObjectIdsByParent(SysTable_Enum sysTable, SysTable_Enum parentSysTable);
    }

    public interface IEntityTree : IEntityObject
    {
        long ParentId { get; set; }
        string CustomField1 { get; set; }
        string CustomField2 { get; set; }
        bool hasChildren { get; set; }
        bool IsExpanded { get; set; }
        bool IsMarked { get; set; }
    }

    public interface IEntityLog : IEntityObject
    {
        DateTime? CRT_DATE { get; set; }
        DateTime? MFY_DATE { get; set; }
        int? MFY_SUSER_ID { get; set; }
    }

    public interface IEntitySyn : IEntityObject
    {
        Guid? SYN_GUID { get; set; }
    }

    public interface IEntityPeriod : IEntityObject
    {
        DateTime DATE_BEG { get; set; }
        DateTime? DATE_END { get; set; }
    }

    public interface IEntityCommonTree
    {
        Guid Guid { get; set; }
        Guid ParentGuid { get; set; }
        string Name { get; set; }
        bool hasChildren { get; set; }
        bool IsExpanded { get; set; }
        bool IsMarked { get; set; }
    }
}
