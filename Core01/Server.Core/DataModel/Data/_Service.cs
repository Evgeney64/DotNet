//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Server.Core.Model
{
    using System.Linq;
    using Server.Core.Context;
    
    public partial class EntityServ : EntityService<EntityContext>
    {
        public IQueryable<BUILD> Get_BUILD() => Context.BUILD;
        public IQueryable<CHAT> Get_CHAT() => Context.CHAT;
        public IQueryable<CODE_MAPPING> Get_CODE_MAPPING() => Context.CODE_MAPPING;
        public IQueryable<DEAL> Get_DEAL() => Context.DEAL;
        public IQueryable<DEAL_EXT> Get_DEAL_EXT() => Context.DEAL_EXT;
        public IQueryable<DOCUMENT> Get_DOCUMENT() => Context.DOCUMENT;
        public IQueryable<DOCUMENT_ITEM> Get_DOCUMENT_ITEM() => Context.DOCUMENT_ITEM;
        public IQueryable<DOCUMENT_RELATION> Get_DOCUMENT_RELATION() => Context.DOCUMENT_RELATION;
        public IQueryable<DOCUMENT_STORAGE> Get_DOCUMENT_STORAGE() => Context.DOCUMENT_STORAGE;
        public IQueryable<EQUIPMENT_METER> Get_EQUIPMENT_METER() => Context.EQUIPMENT_METER;
        public IQueryable<EVENT> Get_EVENT() => Context.EVENT;
        public IQueryable<EVENT_SCHEM> Get_EVENT_SCHEM() => Context.EVENT_SCHEM;
        public IQueryable<EVENT_STATE> Get_EVENT_STATE() => Context.EVENT_STATE;
        public IQueryable<EXT_PARAM> Get_EXT_PARAM() => Context.EXT_PARAM;
        public IQueryable<EXT_PARAM_VALUE> Get_EXT_PARAM_VALUE() => Context.EXT_PARAM_VALUE;
        public IQueryable<FACILITY> Get_FACILITY() => Context.FACILITY;
        public IQueryable<LOG> Get_LOG() => Context.LOG;
        public IQueryable<LOG_DETAIL> Get_LOG_DETAIL() => Context.LOG_DETAIL;
        public IQueryable<NSI_ACCOUNT_PERIOD> Get_NSI_ACCOUNT_PERIOD() => Context.NSI_ACCOUNT_PERIOD;
        public IQueryable<NSI_ALGORITHM> Get_NSI_ALGORITHM() => Context.NSI_ALGORITHM;
        public IQueryable<NSI_ALGORITHM_PARAM> Get_NSI_ALGORITHM_PARAM() => Context.NSI_ALGORITHM_PARAM;
        public IQueryable<NSI_BANK> Get_NSI_BANK() => Context.NSI_BANK;
        public IQueryable<NSI_CALC> Get_NSI_CALC() => Context.NSI_CALC;
        public IQueryable<NSI_CONFIG> Get_NSI_CONFIG() => Context.NSI_CONFIG;
        public IQueryable<NSI_CONFIG_DATA_SCHEM> Get_NSI_CONFIG_DATA_SCHEM() => Context.NSI_CONFIG_DATA_SCHEM;
        public IQueryable<NSI_CONTACT> Get_NSI_CONTACT() => Context.NSI_CONTACT;
        public IQueryable<NSI_DATA_SOURCE> Get_NSI_DATA_SOURCE() => Context.NSI_DATA_SOURCE;
        public IQueryable<NSI_DISTRICT> Get_NSI_DISTRICT() => Context.NSI_DISTRICT;
        public IQueryable<NSI_DOCUMENT_ITEM> Get_NSI_DOCUMENT_ITEM() => Context.NSI_DOCUMENT_ITEM;
        public IQueryable<NSI_EVENT> Get_NSI_EVENT() => Context.NSI_EVENT;
        public IQueryable<NSI_EVENT_STATE> Get_NSI_EVENT_STATE() => Context.NSI_EVENT_STATE;
        public IQueryable<NSI_EVENT_SUBTOPIC> Get_NSI_EVENT_SUBTOPIC() => Context.NSI_EVENT_SUBTOPIC;
        public IQueryable<NSI_EVENT_TOPIC> Get_NSI_EVENT_TOPIC() => Context.NSI_EVENT_TOPIC;
        public IQueryable<NSI_FACILITY> Get_NSI_FACILITY() => Context.NSI_FACILITY;
        public IQueryable<NSI_FACTORY> Get_NSI_FACTORY() => Context.NSI_FACTORY;
        public IQueryable<NSI_HCS> Get_NSI_HCS() => Context.NSI_HCS;
        public IQueryable<NSI_HCS_VALUE> Get_NSI_HCS_VALUE() => Context.NSI_HCS_VALUE;
        public IQueryable<NSI_METER> Get_NSI_METER() => Context.NSI_METER;
        public IQueryable<NSI_METER_CONFIG> Get_NSI_METER_CONFIG() => Context.NSI_METER_CONFIG;
        public IQueryable<NSI_METER_PARAM> Get_NSI_METER_PARAM() => Context.NSI_METER_PARAM;
        public IQueryable<NSI_MUNICIPALITY> Get_NSI_MUNICIPALITY() => Context.NSI_MUNICIPALITY;
        public IQueryable<NSI_MUNICIPALITY_TYPE> Get_NSI_MUNICIPALITY_TYPE() => Context.NSI_MUNICIPALITY_TYPE;
        public IQueryable<NSI_OBLAST> Get_NSI_OBLAST() => Context.NSI_OBLAST;
        public IQueryable<NSI_OBLAST_REGION> Get_NSI_OBLAST_REGION() => Context.NSI_OBLAST_REGION;
        public IQueryable<NSI_OKOPF> Get_NSI_OKOPF() => Context.NSI_OKOPF;
        public IQueryable<NSI_OKVED> Get_NSI_OKVED() => Context.NSI_OKVED;
        public IQueryable<NSI_PARAM> Get_NSI_PARAM() => Context.NSI_PARAM;
        public IQueryable<NSI_PHONE_RANGES> Get_NSI_PHONE_RANGES() => Context.NSI_PHONE_RANGES;
        public IQueryable<NSI_PRODUCT> Get_NSI_PRODUCT() => Context.NSI_PRODUCT;
        public IQueryable<NSI_REAZON> Get_NSI_REAZON() => Context.NSI_REAZON;
        public IQueryable<NSI_STREET> Get_NSI_STREET() => Context.NSI_STREET;
        public IQueryable<NSI_STREET_TYPE> Get_NSI_STREET_TYPE() => Context.NSI_STREET_TYPE;
        public IQueryable<NSI_SYS_TABLE_TREE_RELATION> Get_NSI_SYS_TABLE_TREE_RELATION() => Context.NSI_SYS_TABLE_TREE_RELATION;
        public IQueryable<NSI_TAG> Get_NSI_TAG() => Context.NSI_TAG;
        public IQueryable<NSI_TAG_TYPE> Get_NSI_TAG_TYPE() => Context.NSI_TAG_TYPE;
        public IQueryable<NSI_TASK> Get_NSI_TASK() => Context.NSI_TASK;
        public IQueryable<NSI_TASK_CONFIG> Get_NSI_TASK_CONFIG() => Context.NSI_TASK_CONFIG;
        public IQueryable<NSI_TASK_GROUP> Get_NSI_TASK_GROUP() => Context.NSI_TASK_GROUP;
        public IQueryable<NSI_TASK_JOB> Get_NSI_TASK_JOB() => Context.NSI_TASK_JOB;
        public IQueryable<NSI_TASK_JOB_SCHEM> Get_NSI_TASK_JOB_SCHEM() => Context.NSI_TASK_JOB_SCHEM;
        public IQueryable<NSI_TASK_RESULT> Get_NSI_TASK_RESULT() => Context.NSI_TASK_RESULT;
        public IQueryable<NSI_TASK_STATUS> Get_NSI_TASK_STATUS() => Context.NSI_TASK_STATUS;
        public IQueryable<NSI_TASK_TRIGGER> Get_NSI_TASK_TRIGGER() => Context.NSI_TASK_TRIGGER;
        public IQueryable<NSI_TASK_TRIGGER_TIME> Get_NSI_TASK_TRIGGER_TIME() => Context.NSI_TASK_TRIGGER_TIME;
        public IQueryable<NSI_VILLAGE> Get_NSI_VILLAGE() => Context.NSI_VILLAGE;
        public IQueryable<NSI_VILLAGE_TYPE> Get_NSI_VILLAGE_TYPE() => Context.NSI_VILLAGE_TYPE;
        public IQueryable<NSI_ZONE> Get_NSI_ZONE() => Context.NSI_ZONE;
        public IQueryable<NSI_ZONE_UNION> Get_NSI_ZONE_UNION() => Context.NSI_ZONE_UNION;
        public IQueryable<PARTNER> Get_PARTNER() => Context.PARTNER;
        public IQueryable<PARTNER_CONTACT> Get_PARTNER_CONTACT() => Context.PARTNER_CONTACT;
        public IQueryable<PARTNER_EXT> Get_PARTNER_EXT() => Context.PARTNER_EXT;
        public IQueryable<PARTNER_RELATION> Get_PARTNER_RELATION() => Context.PARTNER_RELATION;
        public IQueryable<PHONES_CHANGE> Get_PHONES_CHANGE() => Context.PHONES_CHANGE;
        public IQueryable<REPORT> Get_REPORT() => Context.REPORT;
        public IQueryable<REPORT_TABLE> Get_REPORT_TABLE() => Context.REPORT_TABLE;
        public IQueryable<SHARE_POINT_TASK> Get_SHARE_POINT_TASK() => Context.SHARE_POINT_TASK;
        public IQueryable<SYS_AUTO_TEST> Get_SYS_AUTO_TEST() => Context.SYS_AUTO_TEST;
        public IQueryable<SYS_AUTO_TEST_DETAIL> Get_SYS_AUTO_TEST_DETAIL() => Context.SYS_AUTO_TEST_DETAIL;
        public IQueryable<SYS_BASE_COLLECTION> Get_SYS_BASE_COLLECTION() => Context.SYS_BASE_COLLECTION;
        public IQueryable<SYS_BASE_RELATION> Get_SYS_BASE_RELATION() => Context.SYS_BASE_RELATION;
        public IQueryable<SYS_CHANGE_LIST> Get_SYS_CHANGE_LIST() => Context.SYS_CHANGE_LIST;
        public IQueryable<SYS_CHANGE_LIST_EVENT> Get_SYS_CHANGE_LIST_EVENT() => Context.SYS_CHANGE_LIST_EVENT;
        public IQueryable<SYS_CLIENT> Get_SYS_CLIENT() => Context.SYS_CLIENT;
        public IQueryable<SYS_COLUMN_RELATION> Get_SYS_COLUMN_RELATION() => Context.SYS_COLUMN_RELATION;
        public IQueryable<SYS_COMBO_ITEM> Get_SYS_COMBO_ITEM() => Context.SYS_COMBO_ITEM;
        public IQueryable<SYS_CONFIG> Get_SYS_CONFIG() => Context.SYS_CONFIG;
        public IQueryable<SYS_COUNTER> Get_SYS_COUNTER() => Context.SYS_COUNTER;
        public IQueryable<SYS_GROUP> Get_SYS_GROUP() => Context.SYS_GROUP;
        public IQueryable<SYS_GROUP_FILTER> Get_SYS_GROUP_FILTER() => Context.SYS_GROUP_FILTER;
        public IQueryable<SYS_GROUP_OBJECT> Get_SYS_GROUP_OBJECT() => Context.SYS_GROUP_OBJECT;
        public IQueryable<SYS_HELP> Get_SYS_HELP() => Context.SYS_HELP;
        public IQueryable<SYS_HELP_INDEX> Get_SYS_HELP_INDEX() => Context.SYS_HELP_INDEX;
        public IQueryable<SYS_LOG> Get_SYS_LOG() => Context.SYS_LOG;
        public IQueryable<SYS_MODULE> Get_SYS_MODULE() => Context.SYS_MODULE;
        public IQueryable<SYS_OPERATION> Get_SYS_OPERATION() => Context.SYS_OPERATION;
        public IQueryable<SYS_PACKET_CONFIG> Get_SYS_PACKET_CONFIG() => Context.SYS_PACKET_CONFIG;
        public IQueryable<SYS_TABLE> Get_SYS_TABLE() => Context.SYS_TABLE;
        public IQueryable<SYS_TABLE_COLUMN> Get_SYS_TABLE_COLUMN() => Context.SYS_TABLE_COLUMN;
        public IQueryable<SYS_TABLE_COLUMN_ENUM> Get_SYS_TABLE_COLUMN_ENUM() => Context.SYS_TABLE_COLUMN_ENUM;
        public IQueryable<SYS_TABLE_COLUMN_ENUM_GROUP> Get_SYS_TABLE_COLUMN_ENUM_GROUP() => Context.SYS_TABLE_COLUMN_ENUM_GROUP;
        public IQueryable<SYS_TABLE_COLUMN_RELATION> Get_SYS_TABLE_COLUMN_RELATION() => Context.SYS_TABLE_COLUMN_RELATION;
        public IQueryable<SYS_TABLE_COLUMN_TYPE> Get_SYS_TABLE_COLUMN_TYPE() => Context.SYS_TABLE_COLUMN_TYPE;
        public IQueryable<SYS_TABLE_RELATION> Get_SYS_TABLE_RELATION() => Context.SYS_TABLE_RELATION;
        public IQueryable<SYS_TABLE_TREE> Get_SYS_TABLE_TREE() => Context.SYS_TABLE_TREE;
        public IQueryable<SYS_TABLE_TREE_RELATION> Get_SYS_TABLE_TREE_RELATION() => Context.SYS_TABLE_TREE_RELATION;
        public IQueryable<SYS_TEMPLATE> Get_SYS_TEMPLATE() => Context.SYS_TEMPLATE;
        public IQueryable<SYS_TEMPLATE_PARAM> Get_SYS_TEMPLATE_PARAM() => Context.SYS_TEMPLATE_PARAM;
        public IQueryable<SYS_USER> Get_SYS_USER() => Context.SYS_USER;
        public IQueryable<SYS_USER_CALL> Get_SYS_USER_CALL() => Context.SYS_USER_CALL;
        public IQueryable<SYS_USER_CERTIFICATE> Get_SYS_USER_CERTIFICATE() => Context.SYS_USER_CERTIFICATE;
        public IQueryable<SYS_USER_DATA> Get_SYS_USER_DATA() => Context.SYS_USER_DATA;
        public IQueryable<SYS_USER_GROUP> Get_SYS_USER_GROUP() => Context.SYS_USER_GROUP;
        public IQueryable<SYS_USER_GROUP_CONFIG> Get_SYS_USER_GROUP_CONFIG() => Context.SYS_USER_GROUP_CONFIG;
        public IQueryable<SYS_USER_THEME> Get_SYS_USER_THEME() => Context.SYS_USER_THEME;
        public IQueryable<SYS_VERSION> Get_SYS_VERSION() => Context.SYS_VERSION;
        public IQueryable<TASK> Get_TASK() => Context.TASK;
        public IQueryable<TASK_CONFIG_RELATION> Get_TASK_CONFIG_RELATION() => Context.TASK_CONFIG_RELATION;
        public IQueryable<TASK_EXECUTION> Get_TASK_EXECUTION() => Context.TASK_EXECUTION;
        public IQueryable<TASK_EXECUTION_DETAIL> Get_TASK_EXECUTION_DETAIL() => Context.TASK_EXECUTION_DETAIL;
        public IQueryable<TASK_TRIGGER> Get_TASK_TRIGGER() => Context.TASK_TRIGGER;
        public IQueryable<TASK_TRIGGER_EVENT> Get_TASK_TRIGGER_EVENT() => Context.TASK_TRIGGER_EVENT;
        public IQueryable<TASK_TRIGGER_TIME> Get_TASK_TRIGGER_TIME() => Context.TASK_TRIGGER_TIME;
    }
}
