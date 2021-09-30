namespace Server.Core.Public
{
    public enum SysTable_Enum
    {
        None = 0,

        #region Adress
        Nsi_Village = 17,
        Nsi_Street = 20,
        Build = 22,
        BuildAdress = 102201,
        Nsi_Oblast = 230,
        Nsi_Oblast_Region = 231,
        Nsi_Municipality = 171,
        #endregion

        #region Template
        Item_Temp = 70001, List_Temp = 70002, Tree_Temp = 700031, Raschet_Temp = 70004,
        #endregion

        #region Facility
        Facility = 3,
        Facility_Ext = 38,
        Facility_V = 100000 + Facility * 100 + 1,
        Facility_Nets = 100000 + Facility * 100 + 2,

        Facility_Product = 5,
        Facility_Product_Ext = 207,
        Facility_Partner = 175,
        #endregion

        #region Calc_Item / Charge_Item
        Calc_Item = 7,
        Calc_Item_Ext = 23,
        Calc_Item_Border = 100000 + Calc_Item * 100 + 1,
        Calc_Item_Connection = 100000 + Calc_Item * 100 + 2,
        Calc_Item_Restrict_App = 100000 + Calc_Item * 100 + 3, //100703

        Calc_Item_Lost = 212,
        Calc_Item_Work_Graph = 223,
        Calc_Item_Schem = 8,
        Calc_Item_HV = 738,

        Charge_Item = 205,
        Charge_Item_Ext = 178,
        Charge_Temp = 907,

        Charge = 293,
        Charge_Value = 388,
        Calc = 294,
        Calc_Ext = 758,
        Calc_Value = 317,
        Calc_Schem = 396,

        Vw_Calc_Item = 338,
        Vw_Calc_Item_From_Deal = 798,
        Vw_Calc_Item_From_Border = 865,
        Calc_Item_Param_Log = 906,
        //Vw_Calc_Item_Test = 796,
        //Vw_Calc_Item_From_Facility = 797,
        //Vw_Calc_Item_From_Rule = 819,
        #endregion

        #region Volume
        //Calc_Item_Expense = 359,
        //Expense = 272,
        //Expense_Value = 357,
        Calc_Item_Volume = 833,
        Volume = 834,
        Volume_Value = 835,
        #endregion

        #region Partner
        Partner = 1,

        //типы на основе Partner:
        PartnerJ = 100000 + Partner * 100 + 1,
        PartnerP = 100000 + Partner * 100 + 2,
        PartnerI = 100000 + Partner * 100 + 3,

        PartnerChild = 100000 + Partner * 100 + 4,
        PartnerSE = 100000 + Partner * 100 + 5,
        PartnerItog = 100000 + Partner * 100 + 6,
        //Нужно одновременно пополнять Tsb.Lib.Tsb.WCF.Web.NsiPartner_Enum!

        PartnerBuyer = 100000 + Partner * 100 + 10,
        PartnerSeller = 100000 + Partner * 100 + 11,
        PartnerPayer = 100000 + Partner * 100 + 12,
        PartnerOE = 100000 + Partner * 100 + 13,
        PartnerConsignee = 100000 + Partner * 100 + 14,

        Partner_Rs = 118,
        Partner_Contact = 119,
        Partner_Relation = 169,
        Partner_Relation_Child = 100000 + Partner_Relation * 100 + 1,
        Partner_Notify = 824,
        Partner_Slvd = 841,
        #endregion

        #region Deal
        Deal = 121,
        //Vw_Deal = 350,
        Vw_Deal_Saldo = 875,
        Vw_Deal_Saldo_From_Revs = 942,

        Deal_Product = 125,
        Deal_Document = 201,
        Deal_Financing = 200,

        Deal_Discount = 124,
        Deal_Product_Discount = 130,

        Limit = 134,
        Consum = 4,
        Consum_Ext = 25,
        Vw_Deal_Consum_Privat = 928,

        //Deal_Debt = 599,

        Deal_Revs = 620,
        Deal_Revs_Detail = 621,

        Deal_Attach = 852,
        #endregion

        #region Point

        Point_Meter_Show = 287,
        Vw_Point_Meter_Show_Calc = 910,
        Point_Meter_Show_Value = 307,
        Vw_Point_Meter_Show_Value_0 = 800,
        Vw_Point_Meter_Show_Value_1 = 806,
        Point_Meter_HV = 289,

        Point_Meter = 135,
        Vw_Point_Meter_Waste = 850,
        Point_Mtransf = 692,
        Vw_Point_Meter_Lik = 925,
        #endregion

        #region Equipment
        Equipment = 242,
        Equipment_Ext = 255,
        Equipment_Meter = 246,
        Equipment_Mtransf = 252,
        Equipment_Transf = 249,
        Equipment_Line = 253,
        Equipment_Seal = 254,
        Equipment_Waste = 847,
        #endregion

        #region Report
        Report = 422,
        Report_Table = 451,
        #endregion

        #region NSI
        Nsi_Metod_Get = 288,
        Nsi_Account_Period = 385,
        Vw_Nsi_Account_Period = 631,
        Vw_Nsi_Account_Period_1 = -631,

        Nsi_Meter = 26,
        Nsi_Facility = 35,
        Nsi_Mtransf = 251,
        Nsi_Transf = 248,
        Nsi_Bank = 141,
        Nsi_Discount = 142,
        Nsi_Discount_Value = 861,

        Nsi_Frier = 199,
        Nsi_Factory = 250,
        Nsi_Product = 24,
        //Nsi_Time_Zone = 243,
        Nsi_Zone = 431,
        Nsi_Zone_Union = 432,
        NSI = 40001,

        //NSIDynamic = 43,
        //NSIDynamic2 = 100000 + NSIDynamic * 100 + 2,       // 104302
        //NSIDynamicTree = 100000 + NSIDynamic * 100 + 3,     // 104303
        //NSIDynamicTree = 43001,     // 104303

        Net_Unit = 202,

        Nsi_Calc = 146,
        Nsi_Calc_Price = 100000 + Nsi_Calc * 100 + 1,       // 114601

        Nsi_Price = 378,
        Nsi_Price_Ext = 764,
        Nsi_Price_Value = 376,
        Nsi_Partner_Relation = 177,

        Nsi_Document_Relation = 554,

        Nsi_Meter_Param = 245,
        Nsi_Meter_Config = 701,

        Nsi_Task = 467,
        Nsi_Task_Config = 704,
        Task_Config_Relation = 705,
        Nsi_Product_Relation = 751,

        Nsi_Algorithm = 281,
        Nsi_Algorithm_Impl = 921,
        Nsi_Algorithm_Impl_Binding = 922,
        Nsi_Algorithm_Param = 782,

        Nsi_Norm = 779,
        Nsi_Norm_Period = 780,
        Nsi_Norm_Value = 781,

        Nsi_Calc_Matrix = 804,

        Nsi_Calc_Rule = 820,
        Nsi_Calc_Rule_Schem = 821,

        Nsi_Okved = 839,
        Facility_Okved = 845,
        Partner_Okved = 846,

        Nsi_Slvd = 843,
        Nsi_Slvd_Period = 844,

        Nsi_Compactor = 848,
        Nsi_EquipmentWaste = 849,

        Nsi_Invoice_Change_Method = 856,

        Nsi_Config = 894,
        Nsi_Config_Binding = 895,
        Nsi_Config_Data_Schem = 918,

        Nsi_Tag = 898,
        Nsi_Tag_Type = 899,

        Nsi_District = 909,

        Nsi_Hcs = 911,
        Nsi_Hcs_Value = 912,

        Nsi_Sz_Group = 913,
        Nsi_Sz_Group_Calc = 914,

        Nsi_Partner_Role = 122,
        #endregion

        #region Matrix
        Nsi_Matrix = 167,
        Nsi_Matrix_Period = 185,
        Nsi_Matrix_Param = 164,
        Nsi_Matrix_Param_Value = 166,
        Nsi_Matrix_Value = 170,
        Nsi_Matrix_Combination = 783,
        Nsi_Matrix_Period_Param = 784,
        Nsi_Matrix_Period_Param_Value = 785,

        nsi_matrix_Norm = 100000 + Nsi_Matrix_Period * 100 + 1,
        nsi_matrix_Algorithm = 100000 + Nsi_Matrix_Period * 100 + 7,

        nsi_matrix_Tarif2 = 100000 + Nsi_Matrix_Period * 100 + 2,
        nsi_matrix_Tarif11 = 100000 + Nsi_Matrix_Period * 100 + 11,

        nsi_matrix_ODN = 100000 + Nsi_Matrix_Period * 100 + 3,
        #endregion

        #region XmlConfig

        XmlConfig_Grid = 300001,

        #endregion

        #region Task
        Task = 466,
        Task_Temp = 70466,
        Task_detail = 464,
        Nsi_task_job = 459,
        Task_Trigger = 645,
        Task_Trigger_Time = 640,
        Task_Trigger_Event = 641,
        Task_Execution = 669,
        Task_Execution_Detail = 671,

        Share_point_task = 470,

        //nsi_matrix_Value = 118504,
        #endregion

        #region Document
        Document = 406,
        Vw_Document_Event = 886,
        Vw_Document_U = 941,

        //Document_Debit = 100000 + Document * 100 + 1,                   //140601
        //Document_Credit = 100000 + Document * 100 + 2,                  //140602
        Document_Debit = 421,
        Document_Credit = 416,
        Document_Invoice = 100000 + Document * 100 + 3,                 //140603

        ChargeProcessing = 100000 + Document * 100 + 4,                 //140604
        Document_Reestr = 100000 + Document * 100 + 5,                  //140605

        Document_Credit_Child = 100000 + Document * 100 + 6,            //140606
        Document_Credit_Distr = 100000 + Document * 100 + 7,            //140607

        Document_Debit_Charge_Value = 100000 + Document * 100 + 8,      //140608
        Document_Credit_Destination = 100000 + Document * 100 + 9,      //140609

        Document_Invoice_Charge = 100000 + Document * 100 + 10,         //140610   

        Document_Credit_Advance = 100000 + Document * 100 + 11,         //140611

        Document_Bill = 100000 + Document * 100 + 12,                   //140612 //список счетов

        //Debit_Simple = 421,

        Document_Detail = 403,
        Vw_Document_Detail_U = 938,
        Document_Debit_Detail = 418,
        Document_Credit_Charge = 423,

        Document_Storage = 439,
        Document_Storage_Items = 716,
        Document_Item = 734,

        Document_Relation = 555,
        Document_Relation_Child = 100000 + Document_Relation * 100 + 1,  //155501

        // запутанно - надо разобраться с Document_Credit_Child, Document_Credit и DocumentCredit

        Document_Detail_Revs = 648,

        Document_Restrict_App = 340,
        Document_Debit_Credit = 718,

        Document_Fd = 908,

        Fin_Block = 915,
        #endregion

        #region Salebook

        Salebook = 494,
        Salebook_Detail = 496,
        Salebook_Addlist = 495,

        #endregion

        #region Sys
        Sys_Table = 34,
        Sys_Table_Column = 45,
        Sys_Table_Tree = 43,
        Sys_Table_Tree_Relation = 730,
        Sys_Table_Column_Enum = 263,
        Sys_Table_Column_Enum_Group = 264,

        Sys_Operation = 218,

        Sys_Menu = 217,
        Sys_Menu_Main = 577,
        Sys_Config = 277,

        Sys_Role_Group = 309,
        Sys_Role_Group_Filter = 639,
        Sys_Table_Role = 312,
        Sys_User = 654,
        Sys_User_Group = 684,
        Sys_User_Group_Config = 685,
        Sys_User_Data = 687,
        Sys_User_Theme = 695,
        Sys_User_Call = 717,

        Sys_Group = 689,
        Sys_Group_Object = 690,
        Sys_Group_Filter = 832,

        Sys_Table_Column_Relation = 530,
        Vw_Sys_Table_Column_Relation = 537,
        Code_Mapping = 700,

        Work_Graph = 588,
        Work_Graph_Ext = 589,

        Sys_Log = 635,
        Log = 642,
        Log_Detail = 736,

        Sys_Client = 739,
        Sys_Module = 742,
        Sys_Version = 744,
        Sys_Template = 728,
        Sys_Template_Binding = 826,

        Sys_Help = 802,
        Vw_Sys_Help = 803,

        Sys_Auto_Test = 827,
        Sys_Auto_Test_Detail = 828,

        Sys_Table_Relation = 901,

        Sys_Counter = 634,

        Sys_Change_List = 923,
        Sys_Change_List_Event = 924,
        #endregion

        #region Event
        Event = 452,
        EventExt = 45200,

        Event_State = 571,
        Event_Schem = 677,
        Event_Calc = 771,

        //EventPrivat = 45201,
        //EventGos = 45202,
        Event2 = 100000 + Event * 100 + 3,                 //145203

        Nsi_Event = 497,
        Nsi_Event_State = 533,
        Nsi_Event_Config_Group = 686,
        Nsi_Event_Topic = 458,
        Nsi_Event_Subtopic = 746,
        #endregion

        #region Other

        SecurityMainPage = 80001,
        SecurityLogin = 80002,

        Scr_User = 90001,
        Scr_User_Group = 90002,
        Scr_Role = 90003,
        Scr_Principal = 90004,
        Scr_Role_Principal = 90006,
        Scr_User_Claim = 90007,
        Scr_Group = 90009,
        Scr_Role_Role = 90010,
        Scr_User_Login = 90011,

        //Test = 80003,

        Nsi_Reazon = 132,
        Nsi_Document = 411,
        NSI_PHONE_RANGES = 714,
        PHONES_CHANGE = 715,
        Bank_Transaction = 726,

        Yy_Order = 747,
        Yy_Order_Product = 748,
        //Yy_Common_Table_Tree = 801,

        Chat = 759,

        Ext_Param = 774,
        Ext_Param_Value = 775,
        //Nsi_Ext_Param = 776,
        Nsi_Param = 786,
        #endregion

        #region Virtual
        PointMeterShowValues_ASKUE = 831, // Объект АСКУЭ

        VW_FACILITY_CALC_ITEM = 888, // Базовый класс виртуальных сущностей
        Facility_CalcItem_ON = 836, // Объект недвижимости
        Facility_CalcItem_KP = 837, // Контейнерная площадка (место накопления)
        Facility_CalcItem_EL = 887, // Виртуальная сущность по электрике

        Consum_ChargeItem = 840, // Контейнерная площадка (место накопления)

        License = 851, // Лицензии

        Hcs_Object_Info = 916,          // ГИС ЖКХ, Информация об объекте передачи
        Hcs_Object_Info_Error = 917,    // ГИС ЖКХ, Информация об ошибках передачи
        Hcs_Param = 919,                // ГИС ЖКХ, Настройки
        #endregion

        #region Permissions (Simple)
        Deal_S = 838,
        Partner_S = 891,
        Facility_S = 892,
        Document_S = 893,
        #endregion

        VmBase = 77777,

        StaticContext_Config = 857,
        StaticContext_ConfigSection = 858,
        StaticContext_ConfigItem = 859,
    }
}
