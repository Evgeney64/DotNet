using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
//using System.Data.Entity;
//using System.Data.Metadata.Edm;
//using System.Data.Objects;
//using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Oracle.ManagedDataAccess.Client;

using Hcs;
//using Hcs.DataSource
//using Hcs.Configuration;
//using HCS.Model;
//using Oracle.ManagedDataAccess.Client;
using Hcs.Configuration;
using Hcs.Model;

namespace Hcs.DataSource
{
    #region StoredProcDataSourceConfiguration
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
            this[SysOperationCode.NsiExport] = new StoredProcConfiguration
            {
                PrepareProcedureName = "HCS_NsiExport",
                ResultProcedureName = "HCS_NsiExport_Result",
                ListProcedureName = "HCS_NsiExport_List",
            };
            this[SysOperationCode.OrganizationExport] = new StoredProcConfiguration
            {
                PrepareProcedureName = "hcs_organizationexport",
                ResultProcedureName = "hcs_organizationexport_result",
                ListProcedureName = "hcs_organizationexport_list",
            };
            this[SysOperationCode.AccountImport] = new StoredProcConfiguration
            {
                PrepareProcedureName = "HCS_AccountImport",
                ResultProcedureName = "HCS_AccountImport_Result",
                ListProcedureName = "HCS_AccountImport_List",
            };
            this[SysOperationCode.AccountExport] = new StoredProcConfiguration
            {
                PrepareProcedureName = "HCS_AccountExport",
                ResultProcedureName = "HCS_AccountExport_Result",
                ListProcedureName = "HCS_AccountExport_List",
            };
            this[SysOperationCode.AccountClose] = new StoredProcConfiguration
            {
                PrepareProcedureName = "HCS_AccountClose",
                ResultProcedureName = "HCS_AccountClose_Result",
                ListProcedureName = "HCS_AccountClose_List",
            };
            this[SysOperationCode.AckImport] = new StoredProcConfiguration
            {
                PrepareProcedureName = "HCS_AckImport",
                ResultProcedureName = "HCS_AckImport_Result",
                ListProcedureName = "HCS_AckImport_List",
            };
            this[SysOperationCode.ContractImport] = new StoredProcConfiguration
            {
                PrepareProcedureName = "HCS_ContractImport",
                ResultProcedureName = "HCS_ContractImport_Result",
                ListProcedureName = "HCS_ContractImport_List",
            };
            this[SysOperationCode.SettlementImport] = new StoredProcConfiguration
            {
                PrepareProcedureName = "HCS_SettlementImport",
                ResultProcedureName = "HCS_SettlementImport_Result",
                ListProcedureName = "HCS_SettlementImport_List",
            };
            this[SysOperationCode.DeviceImport] = new StoredProcConfiguration
            {
                PrepareProcedureName = "HCS_DeviceImport",
                ResultProcedureName = "HCS_DeviceImport_Result",
                ListProcedureName = "HCS_DeviceImport_List",
            };
            this[SysOperationCode.DeviceExport] = new StoredProcConfiguration
            {
                PrepareProcedureName = "HCS_DeviceExport",
                ResultProcedureName = "HCS_DeviceExport_Result",
                ListProcedureName = "HCS_DeviceExport_List",
            };
            this[SysOperationCode.DeviceValueImport] = new StoredProcConfiguration
            {
                PrepareProcedureName = "HCS_DeviceValueImport",
                ResultProcedureName = "HCS_DeviceValueImport_Result",
                ListProcedureName = "HCS_DeviceValueImport_List",
            };
            this[SysOperationCode.DeviceValueExport] = new StoredProcConfiguration
            {
                PrepareProcedureName = "HCS_DeviceValueExport",
                ResultProcedureName = "HCS_DeviceValueExport_Result",
                ListProcedureName = "HCS_DeviceValueExport_List",
            };
            this[SysOperationCode.HouseImport] = new StoredProcConfiguration
            {
                PrepareProcedureName = "HCS_HouseImport",
                ResultProcedureName = "HCS_HouseImport_Result",
                ListProcedureName = "HCS_HouseImport_List",
            };
            this[SysOperationCode.HouseExport] = new StoredProcConfiguration
            {
                PrepareProcedureName = "HCS_HouseExport",
                ResultProcedureName = "HCS_HouseExport_Result",
                ListProcedureName = "HCS_HouseExport_List",
            };
            this[SysOperationCode.NotificationImport] = new StoredProcConfiguration
            {
                PrepareProcedureName = "HCS_NotificationImport",
                ResultProcedureName = "HCS_NotificationImport_Result",
                ListProcedureName = "HCS_NotificationImport_List",
            };
            this[SysOperationCode.OrderImport] = new StoredProcConfiguration
            {
                PrepareProcedureName = "HCS_OrderImport",
                ResultProcedureName = "HCS_OrderImport_Result",
                ListProcedureName = "HCS_OrderImport_List",
            };
            this[SysOperationCode.PaymentDocumentImport] = new StoredProcConfiguration
            {
                PrepareProcedureName = "HCS_PaymentDocumentImport",
                ResultProcedureName = "HCS_PaymentDocumentImport_Result",
                ListProcedureName = "HCS_PaymentDocumentImport_List",
            };
            this[SysOperationCode.PaymentDocumentExport] = new StoredProcConfiguration
            {
                PrepareProcedureName = "HCS_PaymentDocumentExport",
                ResultProcedureName = "HCS_PaymentDocumentExport_Result",
                ListProcedureName = "HCS_PaymentDocumentExport_List",
            };
        }
    }
    #endregion

    #region StoredProcConfiguration
    public class StoredProcConfiguration
    {
        public string PrepareProcedureName { get; set; }
        public string ResultProcedureName { get; set; }
        public string ListProcedureName { get; set; }
    }
    #endregion
}
