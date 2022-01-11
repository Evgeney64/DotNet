// Для редактора XML


//Описание Auto-complete правил для конфигурации списков


//Item для списка
var gridItem = {
          attrs: {
              Field: [""],
              Title: [""],
              Type: ["Menu"],
              Image: [""],
              Width: [""],
              TableRef: [""],
              FieldRef: [""],
              Values: [""]
        },
        children: []
};

//Item для меню
var menuItem = {
          attrs: {
              SOPERATION_ID: [""],
              Title: [""],
              TargetTable: [""],
              SOPERATION_EXECUTE_ID: [""],
              Table: [""],
              Field: [""],
              Image: [""],
              Values: ["", "0", "1", "True", "False"]
          },
          children: []
};


// Схема для списка

var xmlTagsForGrid = {
        "!top": ["GRID"],

    GRID: {
        children: ["SECTION", "Item"]
        },

    SECTION: {
        attrs: {
            Name: [""],
            Default: ["True", "False"]
        },
        children: ["Item", "Params"]
    },

    Item: {
        attrs: {
//Комментарием обозначена секция для кодогенератора (см. StaticContext.GetJsForXmlEditor())
//gridattrs
Field: [""],
Field1: [""],
Table: [""],
SOPERATION_ID: ["", "None", "View", "Add", "Edit", "Del", "ListOpen", "Filter", "Save", "Exec", "DateExtSet", "Add2Edit", "AddComplex", "AddChildren", "MainList_From_MainMenu", "MainList_From_Row", "MainList_From_List", "EditList", "HelpOpen", "ListOpen_AsTree", "Reports", "Add2EditChildren", "Filter_AsTree", "Fill_Undepanded_List", "TabTreeV", "AddWithPreview", "AddChild_ToSchem", "DashBord_MainMenu", "DealInfo", "AddWizard", "EditWizard", "RefreshAll", "Diagram", "SysGroupObject_Add", "DelAllChildren", "SysGroupObject_Activate", "AddChildrenWizard", "Link", "EditMain", "Raschet", "RaschetCharge", "RaschetCharge_Storno", "CreateDocuments", "TaskExecute", "TaskAgent", "TaskSchedulerSync", "TaskExecution_Resume", "TaskExecution_Stop", "ContextClean", "ExtendCommand", "SendToSharePoint", "ViewItog", "AddChildren_ToParent", "AddObject_ToTask", "GetDealInfo_Credits", "GetDealInfo_Debets_Energy", "GetDealInfo_Debets_Services", "MainList_From_Chart", "MainList_From_Link", "Report", "AddToInvoice", "StornoDocument", "Events_Deny", "EventsState_Change", "Event_CreateReminderOfDocumentReturn", "CreateSalebook_Sale", "CreateSalebook_Buy", "ReCreateSalebook", "CorrectInvoice", "CreateSalebookAddlist", "UpdateDebt", "ReportPacket", "ReportExport", "GetReportForDocument", "DocumentPrint", "DownloadDocumentStorageContent", "Telephonogramm_AddEvent", "Telephonogramm_PlanSending", "AddWizard_Ann_CalcChange", "AddWizard_Ann_Registration", "AddWizard_Ann_SimpleQuestion", "AddWizard_Ann_OfAct", "PointMeterShowValue_Input", "PointMeterShow_ImportPrivat", "PointMeterShowValue_Import", "PointMeterHV_Import", "PointMeterShow_ExportPrivat", "PointMeterHV_ExportEmpty", "SipCall", "PointMeterHV_ExportWithValues", "PointMeterShowValue_ExportEmpty_Short", "PointMeterShowValue_ExportWithValues_Short", "EventResponse", "Add_Event_From_SysUserCall", "EventResponseNewFile", "AddWizard_GetTemplate", "Event_AddEMail", "PointMeterShowValue_ExportEmpty_Full", "GetCountNewItems_For_User", "PointMeterShowValue_ExportWithValues_Full", "ListOpen_AutoTab", "SelectorFromFilter", "SelectorFromEditor", "SelectorFromPeriodValues", "Authentication_TransferUser", "Authentication_AddToGroup", "Authentication_ChangePassword", "Authentication_ChangeEmail", "Authentication_SetLockout", "Authentication_ChangeUserGroup", "DashBord_Edit", "DashBord_ListOpen", "RaschetRecalc", "RaschetTotal", "RaschetOverage", "MainListOpen", "MainItemOpen", "Close", "EditOnly", "RegimEdit", "FindNode", "Group_Operation", "Group_ListOpen", "Group_AddChildren", "Fill_Undepanded_List_Add", "Fill_Undepanded_List_Del", "GridExportExcel", "IsWaiting_Completed", "SetAccountPeriod", "EditChildren", "DelChildren", "SysTableColumnRelation_Add1", "SysTableColumnRelation_Add2", "SysTableColumnRelation_Add3", "SysTableColumnRelation_Edit1", "SysTableColumnRelation_Edit2", "MainList_Expand"],
SOPERATION_EXECUTE_ID: ["", "None", "View", "Add", "Edit", "Del", "ListOpen", "Filter", "Save", "Exec", "DateExtSet", "Add2Edit", "AddComplex", "AddChildren", "MainList_From_MainMenu", "MainList_From_Row", "MainList_From_List", "EditList", "HelpOpen", "ListOpen_AsTree", "Reports", "Add2EditChildren", "Filter_AsTree", "Fill_Undepanded_List", "TabTreeV", "AddWithPreview", "AddChild_ToSchem", "DashBord_MainMenu", "DealInfo", "AddWizard", "EditWizard", "RefreshAll", "Diagram", "SysGroupObject_Add", "DelAllChildren", "SysGroupObject_Activate", "AddChildrenWizard", "Link", "EditMain", "Raschet", "RaschetCharge", "RaschetCharge_Storno", "CreateDocuments", "TaskExecute", "TaskAgent", "TaskSchedulerSync", "TaskExecution_Resume", "TaskExecution_Stop", "ContextClean", "ExtendCommand", "SendToSharePoint", "ViewItog", "AddChildren_ToParent", "AddObject_ToTask", "GetDealInfo_Credits", "GetDealInfo_Debets_Energy", "GetDealInfo_Debets_Services", "MainList_From_Chart", "MainList_From_Link", "Report", "AddToInvoice", "StornoDocument", "Events_Deny", "EventsState_Change", "Event_CreateReminderOfDocumentReturn", "CreateSalebook_Sale", "CreateSalebook_Buy", "ReCreateSalebook", "CorrectInvoice", "CreateSalebookAddlist", "UpdateDebt", "ReportPacket", "ReportExport", "GetReportForDocument", "DocumentPrint", "DownloadDocumentStorageContent", "Telephonogramm_AddEvent", "Telephonogramm_PlanSending", "AddWizard_Ann_CalcChange", "AddWizard_Ann_Registration", "AddWizard_Ann_SimpleQuestion", "AddWizard_Ann_OfAct", "PointMeterShowValue_Input", "PointMeterShow_ImportPrivat", "PointMeterShowValue_Import", "PointMeterHV_Import", "PointMeterShow_ExportPrivat", "PointMeterHV_ExportEmpty", "SipCall", "PointMeterHV_ExportWithValues", "PointMeterShowValue_ExportEmpty_Short", "PointMeterShowValue_ExportWithValues_Short", "EventResponse", "Add_Event_From_SysUserCall", "EventResponseNewFile", "AddWizard_GetTemplate", "Event_AddEMail", "PointMeterShowValue_ExportEmpty_Full", "GetCountNewItems_For_User", "PointMeterShowValue_ExportWithValues_Full", "ListOpen_AutoTab", "SelectorFromFilter", "SelectorFromEditor", "SelectorFromPeriodValues", "Authentication_TransferUser", "Authentication_AddToGroup", "Authentication_ChangePassword", "Authentication_ChangeEmail", "Authentication_SetLockout", "Authentication_ChangeUserGroup", "DashBord_Edit", "DashBord_ListOpen", "RaschetRecalc", "RaschetTotal", "RaschetOverage", "MainListOpen", "MainItemOpen", "Close", "EditOnly", "RegimEdit", "FindNode", "Group_Operation", "Group_ListOpen", "Group_AddChildren", "Fill_Undepanded_List_Add", "Fill_Undepanded_List_Del", "GridExportExcel", "IsWaiting_Completed", "SetAccountPeriod", "EditChildren", "DelChildren", "SysTableColumnRelation_Add1", "SysTableColumnRelation_Add2", "SysTableColumnRelation_Add3", "SysTableColumnRelation_Edit1", "SysTableColumnRelation_Edit2", "MainList_Expand"],
Nom: [""],
Title: [""],
Image: [""],
Visible: ["True", "False", "1", "0"],
Width: [""],
Mask: [""],
SumFormat: [""],
NeedSum: ["True", "False", "1", "0"],
IsModal: ["True", "False", "1", "0"],
ModalWindow_Title: [""],
ModalWindow_Message: [""],
Type: ["", "None", "Menu", "Progress", "Link"],
Editable: ["True", "False", "1", "0"],
TableRef: ["", "None", "Partner", "Facility", "Consum", "Facility_Product", "Calc_Item", "Calc_Item_Schem", "Nsi_Village", "Nsi_Street", "Build", "Nsi_Meter", "Sys_Table", "Nsi_Facility", "NSIDynamic", "Sys_Table_Tree", "Sys_Table_Column", "Partner_Rs", "Partner_Contact", "Deal", "Deal_Discount", "Deal_Product", "Deal_Product_Discount", "Nsi_Reazon", "Limit", "Point_Meter", "Nsi_Bank", "Nsi_Discount", "Nsi_Product", "Nsi_Calc", "Nsi_Matrix_Param", "Nsi_Matrix_Param_Value", "Nsi_Matrix", "Partner_Relation", "Nsi_Matrix_Value", "Nsi_Municipality", "Facility_Partner", "Nsi_Partner_Relation", "Nsi_Matrix_Period", "Deal_Financing", "Deal_Document", "Net_Unit", "Charge_Item", "Calc_Item_Lost", "Sys_Menu", "Calc_Item_Work_Graph", "Nsi_Oblast", "Nsi_Oblast_Region", "Equipment", "Nsi_Time_Zone", "Nsi_Meter_Param", "Equipment_Meter", "Nsi_Transf", "Equipment_Transf", "Nsi_Factory", "Nsi_Mtransf", "Equipment_Mtransf", "Equipment_Line", "Equipment_Seal", "Sys_Table_Column_Enum", "Calc_Item_Expense", "Sys_Config", "Point_Meter_Show", "Point_Meter_HV", "Charge", "Calc", "Calc_Ext", "Point_Meter_Show_Value", "Sys_Role_Group", "Sys_Table_Role", "Calc_Value", "Document_Restrict_App", "Calc_Item_Expense_Value", "Calc_Item_Expense_Period", "Nsi_Price_Value", "Nsi_Price", "Nsi_Account_Period", "Charge_Value", "Document_Detail", "Document", "Nsi_Document", "Document_Credit", "Document_Debit_Detail", "Document_Debit", "Report", "Document_Credit_Charge", "Document_Storage", "Report_Table", "Event", "Nsi_task_job", "Task_detail", "Task", "Share_point_task", "Salebook", "Salebook_Addlist", "Salebook_Detail", "Nsi_Event", "Sys_Table_Column_Relation", "Nsi_Event_State", "Vw_Sys_Table_Column_Relation", "Nsi_Document_Relation", "Document_Relation", "Event_State", "Sys_Menu_Main", "Work_Graph", "Work_Graph_Ext", "Deal_Revs", "Deal_Revs_Detail", "Sys_Log", "Sys_Role_Group_Filter", "Task_Trigger_Time", "Task_Trigger_Event", "Log", "Task_Trigger", "Document_Detail_Revs", "Sys_User", "Task_Execution", "Task_Execution_Detail", "Event_Schem", "Sys_User_Group", "Nsi_Event_Config_Group", "Sys_User_Data", "Sys_Group", "Sys_Group_Object", "Point_Mtransf", "Sys_User_Theme", "Code_Mapping", "Nsi_Meter_Config", "Sys_User_Call", "Document_Debit_Credit", "NSI_PHONE_RANGES", "PHONES_CHANGE", "NSI", "EventExt", "Item_Temp", "List_Temp", "Raschet_Temp", "Task_Temp", "VmBase", "SecurityMainPage", "SecurityLogin", "Scr_User", "Scr_User_Group", "Scr_Role", "Scr_Principal", "Scr_Principal_Group", "Scr_Role_Principal", "Scr_User_Sip", "Scr_Win_User", "Scr_Group", "Scr_Role_Role", "PartnerJ", "PartnerP", "PartnerI", "PartnerChild", "PartnerSE", "PartnerItog", "PartnerBuyer", "PartnerSeller", "PartnerPayer", "PartnerOE", "PartnerConsignee", "Facility_V", "Facility_Nets", "Calc_Item_Border", "Calc_Item_Connection", "Calc_Item_Restrict_App", "BuildAdress", "NSIDynamic2", "NSIDynamicTree", "Nsi_Calc_Price", "Partner_Relation_Child", "nsi_matrix_Norm", "nsi_matrix_Tarif2", "nsi_matrix_ODN", "nsi_matrix_Algorithm", "nsi_matrix_Tarif11", "Document_Invoice", "ChargeProcessing", "Document_Reestr", "Document_Credit_Child", "Document_Credit_Distr", "Document_Debit_Charge_Value", "Document_Credit_Destination", "Document_Invoice_Charge", "Document_Credit_Advance", "Document_Bill", "Event2", "Document_Relation_Child", "XmlConfig_Grid", "Tree_Temp"],
FieldRef: [""],
TextAlignment: ["", "left", "right", "center"],
AnyTableName: [""],
AnyTableField: [""],
CountFieldProgress: [""],
TotalFieldProgress: [""]
///gridattrs
        },
        children: []
    },
    Params: {
        attrs: {
            Type: ["GridOrderBy", "SectionCondition"]
        },
        children: ["Item"]
    },
};


// Схема для редактора

var xmlTagsForEditor = {
        "!top": ["EDIT"],

    EDIT: {
        children: ["PAGE", "Params"]
        },

    PAGE: {
        attrs: {
            Header: ["", "Основное"],
        },
        children: ["EXPANDER"]
        },

    EXPANDER: {
        attrs: {
            IsExpanded: ["True", "False"],
        },
        children: ["HEADER", "BODY"]
    },

    HEADER: {
        attrs: {
            Title: ["", "Основное", "Коды"],
        },
        children: ["Item"]
    },

    BODY: {
        children: ["Item"]
     },


    Item: {
        attrs: {
//Комментарием обозначена секция для кодогенератора (см. StaticContext.GetJsForXmlEditor())
//editorattrs
Field: [""],
Field1: [""],
Property: [""],
Table: [""],
TargetTable: [""],
Control: [""],
Content: [""],
ContentTypeFromField: [""],
SOPERATION_EXECUTE_ID: ["", "None", "View", "Add", "Edit", "Del", "ListOpen", "Filter", "Save", "Exec", "DateExtSet", "Add2Edit", "AddComplex", "AddChildren", "MainList_From_MainMenu", "MainList_From_Row", "MainList_From_List", "EditList", "HelpOpen", "ListOpen_AsTree", "Reports", "Add2EditChildren", "Filter_AsTree", "Fill_Undepanded_List", "TabTreeV", "AddWithPreview", "AddChild_ToSchem", "DashBord_MainMenu", "DealInfo", "AddWizard", "EditWizard", "RefreshAll", "Diagram", "SysGroupObject_Add", "DelAllChildren", "SysGroupObject_Activate", "AddChildrenWizard", "Link", "EditMain", "Raschet", "RaschetCharge", "RaschetCharge_Storno", "CreateDocuments", "TaskExecute", "TaskAgent", "TaskSchedulerSync", "TaskExecution_Resume", "TaskExecution_Stop", "ContextClean", "ExtendCommand", "SendToSharePoint", "ViewItog", "AddChildren_ToParent", "AddObject_ToTask", "GetDealInfo_Credits", "GetDealInfo_Debets_Energy", "GetDealInfo_Debets_Services", "MainList_From_Chart", "MainList_From_Link", "Report", "AddToInvoice", "StornoDocument", "Events_Deny", "EventsState_Change", "Event_CreateReminderOfDocumentReturn", "CreateSalebook_Sale", "CreateSalebook_Buy", "ReCreateSalebook", "CorrectInvoice", "CreateSalebookAddlist", "UpdateDebt", "ReportPacket", "ReportExport", "GetReportForDocument", "DocumentPrint", "DownloadDocumentStorageContent", "Telephonogramm_AddEvent", "Telephonogramm_PlanSending", "AddWizard_Ann_CalcChange", "AddWizard_Ann_Registration", "AddWizard_Ann_SimpleQuestion", "AddWizard_Ann_OfAct", "PointMeterShowValue_Input", "PointMeterShow_ImportPrivat", "PointMeterShowValue_Import", "PointMeterHV_Import", "PointMeterShow_ExportPrivat", "PointMeterHV_ExportEmpty", "SipCall", "PointMeterHV_ExportWithValues", "PointMeterShowValue_ExportEmpty_Short", "PointMeterShowValue_ExportWithValues_Short", "EventResponse", "Add_Event_From_SysUserCall", "EventResponseNewFile", "AddWizard_GetTemplate", "Event_AddEMail", "PointMeterShowValue_ExportEmpty_Full", "GetCountNewItems_For_User", "PointMeterShowValue_ExportWithValues_Full", "ListOpen_AutoTab", "SelectorFromFilter", "SelectorFromEditor", "SelectorFromPeriodValues", "Authentication_TransferUser", "Authentication_AddToGroup", "Authentication_ChangePassword", "Authentication_ChangeEmail", "Authentication_SetLockout", "Authentication_ChangeUserGroup", "DashBord_Edit", "DashBord_ListOpen", "RaschetRecalc", "RaschetTotal", "RaschetOverage", "MainListOpen", "MainItemOpen", "Close", "EditOnly", "RegimEdit", "FindNode", "Group_Operation", "Group_ListOpen", "Group_AddChildren", "Fill_Undepanded_List_Add", "Fill_Undepanded_List_Del", "GridExportExcel", "IsWaiting_Completed", "SetAccountPeriod", "EditChildren", "DelChildren", "SysTableColumnRelation_Add1", "SysTableColumnRelation_Add2", "SysTableColumnRelation_Add3", "SysTableColumnRelation_Edit1", "SysTableColumnRelation_Edit2", "MainList_Expand"],
ValueType: ["", "None", "Value", "ValueIsNull", "NotEqual", "Like", "StartWith", "ListValues", "MoreThan", "LessThan", "MoreOrEqualThan", "LessOrEqualThan", "FieldsCompare", "DateCutDiapason", "Diapason", "LDiapason", "RDiapason", "LRDiapason", "XmlProperty"],
Nom: [""],
XColumnId: [""],
Title: [""],
IsAuto: ["True", "False", "1", "0"],
IsReadOnly: [""],
IsEmptyItem: ["True", "False", "1", "0"],
IsRequired: ["True", "False", "1", "0"],
Visible: ["True", "False", "1", "0"],
MultiSelect: [""],
DefaultValue: [""],
Width: [""],
Height: [""],
Mask: [""],
UnmaskOnPost: ["True", "False", "1", "0"],
Top: [""],
IsModal: ["True", "False", "1", "0"],
ModalWindow_Title: [""],
ModalWindow_Message: [""],
Editable: ["True", "False", "1", "0"],
XmlFormat: [""],
IsExpanded: ["True", "False", "1", "0"]
///editorattrs
        },
        children: ["Params"]
    },
    Params: {
        attrs: {
            Type: ["ComboBoxContent", "ComboBoxDefault", "SelectorFilter", "FieldConstant", "ControlAttributesFromEntity"]
            /*
                    SectionCondition = 1,
        PageCondition = 14,

        MenuFilter = 2,
        SelectorFilter = 3,

        FieldConstant = 4,
        EntityDefault = 13,
        DefaultValue = 15,

        SelectorResult = 6,
        TargetResult = 7,

        ListByRelation = 11,
        ListByChildTable = 12,
        //GetByObject = 13,
        //SelectorParentObject = 13,

        ComboBoxContent = 21,
        ComboBoxDefault = 22,
        ComboBoxFilter = 23,

        GridOrderBy = 31,

        Link = 32,

        //AutoOpenTabs = 31,

        OperationRule = 41,
        ParentPermission = 42,
        HtmlAttributes = 43,
        ParameterAttribute = 44,

        MenuType = 5,
        MenuMainType = 15,

        DataSource = 50,
        GridDataSource = 53,
        GridMenu = 54,
        ValueItem = 55,
        Grid_IsDynamic = 58,
        //PageOption = 56,

        TabEditSave_Content = 56,
        TabEditSave_Redirect = 57,

        PreviewDialog = 104,
        
        AloneButtonsDisabled = 61,
        MenuFilter_ForDynamicControl = 62,
        ControlAttributesFromEntity = 63,

            */
        },
        children: ["Item"]
    },
    };

    // Схема для меню

    var xmlTagsForMenu = {
        "!top": ["MENU"],

    MENU: {
        children: ["SECTION"]
        },

    SECTION: {
        attrs: {
            Name: [""],
            Default: ["True", "False"]
        },
        children: ["OPERATIONS", "LIST_OPEN", "Params"]
    },

    OPERATIONS: {
        children: ["Item", "Separator", "Group"]
    },

    LIST_OPEN: {
        children: ["Item", "Separator", "Group"]
    },

    Item: {
        attrs: {
//Комментарием обозначена секция для кодогенератора (см. StaticContext.GetJsForXmlEditor())
//menuattrs
Field: [""],
Field1: [""],
Property: [""],
Table: [""],
SourceTable: [""],
TargetTable: [""],
TargetTableParent: [""],
TargetTableExecute: [""],
Control: [""],
Content: [""],
SOPERATION_ID: ["", "None", "View", "Add", "Edit", "Del", "ListOpen", "Filter", "Save", "Exec", "DateExtSet", "Add2Edit", "AddComplex", "AddChildren", "MainList_From_MainMenu", "MainList_From_Row", "MainList_From_List", "EditList", "HelpOpen", "ListOpen_AsTree", "Reports", "Add2EditChildren", "Filter_AsTree", "Fill_Undepanded_List", "TabTreeV", "AddWithPreview", "AddChild_ToSchem", "DashBord_MainMenu", "DealInfo", "AddWizard", "EditWizard", "RefreshAll", "Diagram", "SysGroupObject_Add", "DelAllChildren", "SysGroupObject_Activate", "AddChildrenWizard", "Link", "EditMain", "Raschet", "RaschetCharge", "RaschetCharge_Storno", "CreateDocuments", "TaskExecute", "TaskAgent", "TaskSchedulerSync", "TaskExecution_Resume", "TaskExecution_Stop", "ContextClean", "ExtendCommand", "SendToSharePoint", "ViewItog", "AddChildren_ToParent", "AddObject_ToTask", "GetDealInfo_Credits", "GetDealInfo_Debets_Energy", "GetDealInfo_Debets_Services", "MainList_From_Chart", "MainList_From_Link", "Report", "AddToInvoice", "StornoDocument", "Events_Deny", "EventsState_Change", "Event_CreateReminderOfDocumentReturn", "CreateSalebook_Sale", "CreateSalebook_Buy", "ReCreateSalebook", "CorrectInvoice", "CreateSalebookAddlist", "UpdateDebt", "ReportPacket", "ReportExport", "GetReportForDocument", "DocumentPrint", "DownloadDocumentStorageContent", "Telephonogramm_AddEvent", "Telephonogramm_PlanSending", "AddWizard_Ann_CalcChange", "AddWizard_Ann_Registration", "AddWizard_Ann_SimpleQuestion", "AddWizard_Ann_OfAct", "PointMeterShowValue_Input", "PointMeterShow_ImportPrivat", "PointMeterShowValue_Import", "PointMeterHV_Import", "PointMeterShow_ExportPrivat", "PointMeterHV_ExportEmpty", "SipCall", "PointMeterHV_ExportWithValues", "PointMeterShowValue_ExportEmpty_Short", "PointMeterShowValue_ExportWithValues_Short", "EventResponse", "Add_Event_From_SysUserCall", "EventResponseNewFile", "AddWizard_GetTemplate", "Event_AddEMail", "PointMeterShowValue_ExportEmpty_Full", "GetCountNewItems_For_User", "PointMeterShowValue_ExportWithValues_Full", "ListOpen_AutoTab", "SelectorFromFilter", "SelectorFromEditor", "SelectorFromPeriodValues", "Authentication_TransferUser", "Authentication_AddToGroup", "Authentication_ChangePassword", "Authentication_ChangeEmail", "Authentication_SetLockout", "Authentication_ChangeUserGroup", "DashBord_Edit", "DashBord_ListOpen", "RaschetRecalc", "RaschetTotal", "RaschetOverage", "MainListOpen", "MainItemOpen", "Close", "EditOnly", "RegimEdit", "FindNode", "Group_Operation", "Group_ListOpen", "Group_AddChildren", "Fill_Undepanded_List_Add", "Fill_Undepanded_List_Del", "GridExportExcel", "IsWaiting_Completed", "SetAccountPeriod", "EditChildren", "DelChildren", "SysTableColumnRelation_Add1", "SysTableColumnRelation_Add2", "SysTableColumnRelation_Add3", "SysTableColumnRelation_Edit1", "SysTableColumnRelation_Edit2", "MainList_Expand"],
SOPERATION_EXECUTE_ID: ["", "None", "View", "Add", "Edit", "Del", "ListOpen", "Filter", "Save", "Exec", "DateExtSet", "Add2Edit", "AddComplex", "AddChildren", "MainList_From_MainMenu", "MainList_From_Row", "MainList_From_List", "EditList", "HelpOpen", "ListOpen_AsTree", "Reports", "Add2EditChildren", "Filter_AsTree", "Fill_Undepanded_List", "TabTreeV", "AddWithPreview", "AddChild_ToSchem", "DashBord_MainMenu", "DealInfo", "AddWizard", "EditWizard", "RefreshAll", "Diagram", "SysGroupObject_Add", "DelAllChildren", "SysGroupObject_Activate", "AddChildrenWizard", "Link", "EditMain", "Raschet", "RaschetCharge", "RaschetCharge_Storno", "CreateDocuments", "TaskExecute", "TaskAgent", "TaskSchedulerSync", "TaskExecution_Resume", "TaskExecution_Stop", "ContextClean", "ExtendCommand", "SendToSharePoint", "ViewItog", "AddChildren_ToParent", "AddObject_ToTask", "GetDealInfo_Credits", "GetDealInfo_Debets_Energy", "GetDealInfo_Debets_Services", "MainList_From_Chart", "MainList_From_Link", "Report", "AddToInvoice", "StornoDocument", "Events_Deny", "EventsState_Change", "Event_CreateReminderOfDocumentReturn", "CreateSalebook_Sale", "CreateSalebook_Buy", "ReCreateSalebook", "CorrectInvoice", "CreateSalebookAddlist", "UpdateDebt", "ReportPacket", "ReportExport", "GetReportForDocument", "DocumentPrint", "DownloadDocumentStorageContent", "Telephonogramm_AddEvent", "Telephonogramm_PlanSending", "AddWizard_Ann_CalcChange", "AddWizard_Ann_Registration", "AddWizard_Ann_SimpleQuestion", "AddWizard_Ann_OfAct", "PointMeterShowValue_Input", "PointMeterShow_ImportPrivat", "PointMeterShowValue_Import", "PointMeterHV_Import", "PointMeterShow_ExportPrivat", "PointMeterHV_ExportEmpty", "SipCall", "PointMeterHV_ExportWithValues", "PointMeterShowValue_ExportEmpty_Short", "PointMeterShowValue_ExportWithValues_Short", "EventResponse", "Add_Event_From_SysUserCall", "EventResponseNewFile", "AddWizard_GetTemplate", "Event_AddEMail", "PointMeterShowValue_ExportEmpty_Full", "GetCountNewItems_For_User", "PointMeterShowValue_ExportWithValues_Full", "ListOpen_AutoTab", "SelectorFromFilter", "SelectorFromEditor", "SelectorFromPeriodValues", "Authentication_TransferUser", "Authentication_AddToGroup", "Authentication_ChangePassword", "Authentication_ChangeEmail", "Authentication_SetLockout", "Authentication_ChangeUserGroup", "DashBord_Edit", "DashBord_ListOpen", "RaschetRecalc", "RaschetTotal", "RaschetOverage", "MainListOpen", "MainItemOpen", "Close", "EditOnly", "RegimEdit", "FindNode", "Group_Operation", "Group_ListOpen", "Group_AddChildren", "Fill_Undepanded_List_Add", "Fill_Undepanded_List_Del", "GridExportExcel", "IsWaiting_Completed", "SetAccountPeriod", "EditChildren", "DelChildren", "SysTableColumnRelation_Add1", "SysTableColumnRelation_Add2", "SysTableColumnRelation_Add3", "SysTableColumnRelation_Edit1", "SysTableColumnRelation_Edit2", "MainList_Expand"],
SectionNom: [""],
SectionNom_Filt: [""],
SectionNom_Menu: [""],
SectionNom_Grid: [""],
SectionNom_Edit: [""],
Nom: [""],
Title: [""],
Image: [""],
Visible: ["True", "False", "1", "0"],
IsDefault: ["True", "False", "1", "0"],
IsDefaultPage: ["True", "False", "1", "0"],
MenuNewItemsPicType: [""],
MenuNewItemsCountNeed: ["True", "False", "1", "0"],
IsModal: ["True", "False", "1", "0"],
ModalWindow_Title: [""],
ModalWindow_Message: [""]
///menuattrs
        },
        children: []
    },

    Separator: {},

    Group: {
        attrs: {
            SOPERATION_ID: [""],
            Title: [""]
        },
        children: ["Item"]
    },

    Params: {
        attrs: {
            Type: ["SectionCondition", "SectionNom", "MenuFilter", "PreviewDialog", "ParameterAttribute"],
            PreviewDialog: ["Inline"]
        },
        children: ["Item", "PREVIEW_DIALOG"]
    },

    PREVIEW_DIALOG: {
        attrs: {
            Modal: ["True", "False"],
        },
        //children: ["EXPANDER"]

    }
};
      

// Настройка элементов

//editorKind - "grid", "editor", "filter", "menu"
function setupXmlEditor(textAreaId, editorKind)
{
    try {
        var editor = CodeMirror.fromTextArea(document.getElementById(textAreaId), {
            mode: "xml",
            lineNumbers: true,
            foldGutter: true,
            extraKeys: {
                "'<'": completeAfter,
                "'/'": completeIfAfterLt,
                "' '": completeIfInTag,
                "'='": completeIfInTag,
                "F11": function (cm) {
                    cm.setOption("fullScreen", !cm.getOption("fullScreen"));
                },
                "Esc": function (cm) {
                    if (cm.getOption("fullScreen")) cm.setOption("fullScreen", false);
                },
                "Ctrl-Space": "autocomplete",
                "Ctrl-Q": "toggleComment"
            },
            hintOptions: { schemaInfo: getTagsForXmlEditor(editorKind) },
            //allowUnquoted: true,
            autoRefresh: true,
            matchTags: true,
            autoCloseTags: true,
            selectionPointer: true,
            gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"],
            cursorBlinkRate: 400,
            //height: 501,
            //Width: 500,
            workDelay: 10,
            //showCursorWhenSelecting: true
        });
        //editor.setSize(null, 502);

        //editor.on("focus", function (cm, event) { cm.refresh(); });
        editor.on("blur", function (cm, event) { cm.save(); });
        //editor.on("click", function (cm, event) { control_Click(textAreaId); });
        editor.refresh();
        return editor;
    }
    catch(e) {
        return;
    }
}

//! Править синхронно с SYS_CONFIG.XmlFormat
function getTagsForXmlEditor(editorKind)
    {
        editorKind = editorKind.toLowerCase();
        if (editorKind == "grid")
        {
            return xmlTagsForGrid;
        }
        if (editorKind == "editor")
        {
            return xmlTagsForEditor;
        }
        if (editorKind == "menu")
        {
            return xmlTagsForMenu;
        }
        return null;
    }

//Auto-complete functions
function completeAfter(cm, pred) {
    var cur = cm.getCursor();
    if (!pred || pred()) setTimeout(function() {
        if (!cm.state.completionActive)
        cm.showHint({completeSingle: false});
    }, 100);
    return CodeMirror.Pass;
    }

    function completeIfAfterLt(cm) {
    return completeAfter(cm, function() {
        var cur = cm.getCursor();
        return cm.getRange(CodeMirror.Pos(cur.line, cur.ch - 1), cur) == "<";
    });
    }

    function completeIfInTag(cm) {
    return completeAfter(cm, function() {
        var tok = cm.getTokenAt(cm.getCursor());
        if (tok.type == "string" && (!/['"]/.test(tok.string.charAt(tok.string.length - 1)) || tok.string.length == 1)) return false;
        var inner = CodeMirror.innerMode(cm.getMode(), tok.state).state;
        return inner.tagName;
    });
}      
