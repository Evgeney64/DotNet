using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Tsb.External.Server.MainModelExt;
using Tsb.WCF.Web;
using Tsb.WCF.Web.Model;
using Tsb.WCF.Web.XTypes.Filter;

namespace Tsb.External.Server
{
    public partial class MainServExt : ServiceLib.EntityService<MainModelExtEdm>
    {
        public MainServExt()
        {
        }

        public MainServExt(string connectionName)
            : base(connectionName)
        {
        }

        #region DEAL

        #region VW_DEAL_CONSUM_PRIVAT_CLIENT
        [GetMethodByFilterAttribute(121, 100001)]
        [RequiresRole(FixedRole.admins, FixedRole.ViewDeal)]
        public IQueryable<VW_DEAL_CONSUM_PRIVAT_CLIENT> GetVwDealConsumPrivatClients_ByFilter(string sender, XElement filter)
        {
            #region Api from client
            if (Vm_Base != null && Vm_Base.UseApi && Vm_Base.ApiFromServer == false)
            {
                FilterParameter param = new FilterParameter
                {
                    SysTable = SysTable_Enum.Consum,
                    SysView = SysTable_Enum.None,
                    Filter = filter,
                };
                return this.ApiContext.Filter<VW_DEAL_CONSUM_PRIVAT_CLIENT>(param);
            }
            #endregion

            return getVwDealConsumPrivats_ByFilter(sender, filter);
        }
        private IQueryable<VW_DEAL_CONSUM_PRIVAT_CLIENT> getVwDealConsumPrivats_ByFilter(string sender, XElement filter)
        {
            #region
            XFilterDocument xFilterDoc = (XFilterDocument)filter;
            IQueryable<VW_DEAL_CONSUM_PRIVAT_CLIENT> query = this.Context.Get_VW_DEAL_CONSUM_PRIVAT_CLIENT();
            query = Public.FilterQuery<VW_DEAL_CONSUM_PRIVAT_CLIENT>(query, xFilterDoc.ItemsByTable("DEAL"));
            query = Public.FilterQuery<VW_DEAL_CONSUM_PRIVAT_CLIENT>(query, xFilterDoc.ItemsByTable("VW_DEAL_CONSUM_PRIVAT"));

            return query;
            #endregion
        }

        [GetMethodByFilterAttribute(121, 100001)]
        [RequiresRole(FixedRole.admins, FixedRole.ViewDeal)]
        [GetMethodByObjectId_Parents(SysTable_Enum.Consum, SysTable_Enum.Facility)]
        public IQueryable<VW_DEAL_CONSUM_PRIVAT_CLIENT> GetVwDealConsumPrivatClients_ByObjectId(string sender, long id, SysTable_Enum sysTable, System.Nullable<System.DateTime> date, XElement filter = null)
        {
            #region
            #region Api from client
            if (Vm_Base != null && Vm_Base.UseApi && Vm_Base.ApiFromServer == false)
            {
                ParentParameter param = new ParentParameter
                {
                    SysTable = SysTable_Enum.Consum,
                    SysView = SysTable_Enum.None,
                    ParentSysTable = sysTable,
                    Id = id,
                    DateCut = date,
                    Filter = filter,
                };
                return this.ApiContext.Children<VW_DEAL_CONSUM_PRIVAT_CLIENT>(param);
            }
            #endregion

            IQueryable<VW_DEAL_CONSUM_PRIVAT_CLIENT> query = getVwDealConsumPrivats_ByFilter(sender, filter);
            switch (sysTable)
            {
                case SysTable_Enum.Consum:
                    query = query.Where(ss => ss.CONSUM_ID == id);
                    //query = checkPermissionsFilter_VwConsum(sender, filter, query);
                    break;
                case SysTable_Enum.Facility:
                    query = query.Where(ss => ss.FACILITY_ID == id);
                    //query = checkPermissionsFilter_VwConsum(sender, filter, query);
                    break;
                default:
                    break;
            };

            if (date != null)
            {
                query = query.Where(Public.GetExpr_WhereDateCutDiapason<VW_DEAL_CONSUM_PRIVAT_CLIENT>(date));
            }

            query = query.OrderBy(ss => ss.DEAL_ID);

            return query;
            #endregion
        }
        #endregion

        #endregion
    }
}
