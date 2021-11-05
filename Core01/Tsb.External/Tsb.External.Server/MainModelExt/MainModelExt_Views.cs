using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Xml.Serialization;
using Tsb.WCF.Web;
using Tsb.Utils;
using System.Data.Entity.SqlServer;

namespace Tsb.External.Server.MainModelExt
{
    public partial class MainModelExtEdm
    {
        #region VW_DEAL_CONSUM_PRIVAT_CLIENT
        public IQueryable<VW_DEAL_CONSUM_PRIVAT_CLIENT> Get_VW_DEAL_CONSUM_PRIVAT_CLIENT()
        {
            #region
            
            int viewId = StaticContext.Current.GetTableId(nameof(VW_DEAL_CONSUM_PRIVAT_CLIENT));
            var nsiConfig = StaticContext.Current.GetDefaultNsiConfigView(viewId);
            var viewConfig = XmlHelper.NewObjectFromXmlString<VwDealConsumPrivatClientConfig>(nsiConfig?.Data?.ToString());

            #region queryExtParam
            var queryExtParam =
                from p in this.EXT_PARAM
                from pv in this.EXT_PARAM_VALUE
                    .Where(ss => ss.EXT_PARAM_ID == p.EXT_PARAM_ID)
                    .Where(ss => ss.DATE_BEG == this.EXT_PARAM_VALUE
                        .Where(ss1 => ss1.EXT_PARAM_ID == p.EXT_PARAM_ID)
                        .Max(ss1 => ss1.DATE_BEG))
                select new
                {
                    p.STABLE_ID,
                    p.ID,
                    p.NPARAM_ID,
                    pv.VALUE,
                };
            #endregion

            #region queryBuyer
            int[] contactTypes = new int[]
            {
                (int)NsiContact_Enum.StationPhone,
                (int)NsiContact_Enum.MobilePhone,
                (int)NsiContact_Enum.Email
            };
            var queryBuyer =
                from p in this.PARTNER
                select new
                {
                    p.PARTNER_ID,
                    p.PARTNER_NAME_CALC,
                    p.DATE_BORN,
                    p.PLACE_BORN,
                    PARTNER_CONTACTs = p.PARTNER_CONTACT
                        .Where(ss => contactTypes.Contains((int)ss.NCONTACT_ID))
                        .Where(ss => (ss.OUTDATE ?? 0) == 0)
                };
            #endregion

            #region querySaldo
            int? nSZGroupId1 = viewConfig?.Saldo?.NSZGroupId1;
            int? nSZGroupId2 = viewConfig?.Saldo?.NSZGroupId2;
            var querySaldo =
                from dr in this.DEAL_REVS
                    .Where(ss => ss.NREVS_ALGORITHM_ID == (int)NsiRevsAlgorithm_Enum.DzDebt)
                    .Where(ss => ss.CONSUM_ID == null)
                select new
                {
                    DEAL_ID = dr.DEAL_ID,
                    SALDO_NSZ_GROUP_ID = dr.NSZ_GROUP_ID,
                    SALDO_ACCOUNT_PERIOD = dr.ACCOUNT_PERIOD,
                    SALDO = dr.DEBIT_END - dr.CREDIT_END,
                };
            #endregion

            #region queryPartnerManagementCompany
            var queryPartnerManagementCompany =
                from f in this.FACILITY
                    .Where(ss => ss.NFACILITY_ID == (int)NsiFacility_Enum.MKD)
                from pr in this.PARTNER_RELATION
                    .Where(ss => ss.NPARTNER_RELATION_ID == (int)NsiPartnerRelation_Enum.DealResponsible_Buyer)
                    .Where(ss => ss.CHILD_ID == f.FACILITY_ID)
                from p in this.PARTNER
                    .Where(ss => ss.PARTNER_ID == pr.PARTNER_ID)
                select new
                {
                    f.FACILITY_ID,
                    f.BUILD_ID,
                    p.PARTNER_NAME_CALC,
                };
            #endregion

            #region queryCalcItemForDistr
            var queryCalcItemForDistr =
                from ci in this.CALC_ITEM
                    .Where(ss => ss.FACILITY_PRODUCT_ID != null)
                    .Where(ss => ss.NCALC_ITEM_ID == (int)NsiCalcItem_Enum.Seller)
                from cish in this.CALC_ITEM_SCHEM
                    .Where(ss => ss.CHILD_ID == ci.CALC_ITEM_ID)
                    .Where(ss => (ss.FOR_DISTR ?? 0) == 1)
                group ci by (long)ci.FACILITY_PRODUCT_ID
                into t
                select new
                {
                    FACILITY_PRODUCT_ID = t.Key,
                    IN_SCHEM = (short)1,
                };                
            #endregion

            #region queryPrice
            var queryPrice =
                from dp in this.DEAL_PRODUCT
                from chi in this.CHARGE_ITEM
                    .Where(ss => ss.DEAL_PRODUCT_ID == dp.DEAL_PRODUCT_ID)
                from chie in this.CHARGE_ITEM_EXT
                    .Where(ss => ss.CHARGE_ITEM_ID == chi.CHARGE_ITEM_ID)
                    .Where(ss => ss.DATE_BEG == this.CHARGE_ITEM_EXT
                        .Where(ss1 => ss1.CHARGE_ITEM_ID == chi.CHARGE_ITEM_ID)
                        .Max(ss1 => ss1.DATE_BEG))
                from np in this.NSI_PRICE
                    .Where(ss => ss.NPRICE_ID == chie.NPRICE_ID)
                from npe in this.NSI_PRICE_EXT
                    .Where(ss => ss.NPRICE_ID == np.NPRICE_ID)
                    .Where(ss => ss.DATE_BEG == this.NSI_PRICE_EXT
                        .Where(ss1 => ss1.NPRICE_ID == np.NPRICE_ID)
                        .Max(ss1 => ss1.DATE_BEG))
                        .DefaultIfEmpty()
                from npv1 in this.NSI_PRICE_VALUE
                    .Where(ss => ss.NPRICE_EXT_ID == npe.NPRICE_EXT_ID)
                    .Where(ss => ss.NZONE_ID == (int)NsiZone_Enum.OneZone || ss.NZONE_ID == (int)NsiZone_Enum.TwoZone_Day || ss.NZONE_ID == (int)NsiZone_Enum.ThreeZone_Pik)
                    .DefaultIfEmpty()
                from npv2 in this.NSI_PRICE_VALUE
                    .Where(ss => ss.NPRICE_EXT_ID == npe.NPRICE_EXT_ID)
                    .Where(ss => ss.NZONE_ID == (int)NsiZone_Enum.TwoZone_Night || ss.NZONE_ID == (int)NsiZone_Enum.ThreeZone_Night)
                    .DefaultIfEmpty()
                from npv3 in this.NSI_PRICE_VALUE
                    .Where(ss => ss.NPRICE_EXT_ID == npe.NPRICE_EXT_ID)
                    .Where(ss => ss.NZONE_ID == (int)NsiZone_Enum.ThreeZone_PPik)
                    .DefaultIfEmpty()
                select new
                {
                    dp.DEAL_ID,
                    np.NPRICE_ID,
                    np.NPRICE_NAME,
                    SUMMA1 = npv1.SUMMA,
                    SUMMA2 = npv2.SUMMA,
                    SUMMA3 = npv3.SUMMA,
                };
            #endregion

            #region queryChargeMax
            var queryChargeMax =
                from ch in this.CHARGE
                    .Where(ss => ss.NCHARGE_ID == (int)NsiCharge_Enum.Default)
                    .Where(ss => ss.STORNO_STATE == (int)StornoState_Enum.None)
                    .Where(ss => ss.DEAL_ID != null)
                group ch by ch.DEAL_ID
                into ch
                select new
                {
                    DEAL_ID = ch.Key,
                    DATE_END_MAX = ch.Max(ss => ss.DATE_END)
                };
            #endregion

            #region queryBillMax
            var queryBillMax =
                from b in this.DOCUMENT
                    .Where(ss => ss.NDOCUMENT_ID == (int)NsiDocument_Enum.Kvit)
                    .Where(ss => ss.DEAL_ID != null)
                group b by b.DEAL_ID
                into b
                select new
                {
                    DEAL_ID = b.Key,
                    ACCOUNT_PERIOD_MAX = b.Max(ss => ss.ACCOUNT_PERIOD),
                };
            #endregion

            #region queryPaymentLast
            var queryPaymentLast =
                from d in this.DEAL
                from doc in this.DOCUMENT
                    .Where(ss => ss.NSI_DOCUMENT.NDOCUMENT_GROUP_ID == (int)NsiDocumentGroup_Enum.Credit)
                    .Where(ss => ss.DEAL_ID == d.DEAL_ID)
                    .OrderByDescending(ss => ss.DATE_BEG)
                    .ThenByDescending(ss => ss.DOCUMENT_ID)
                    .Take(1)
                from bt in this.BANK_TRANSACTION
                    .Where(ss => ss.BANK_TRANSACTION_ID == doc.BANK_TRANSACTION_ID)
                from d_sel in this.DEAL
                    .Where(ss => ss.DEAL_ID == bt.DEAL_ID)
                    .DefaultIfEmpty()
                from de_sel in this.DEAL_EXT
                    .Where(ss => ss.DEAL_ID == d_sel.DEAL_ID)
                    .Where(ss => ss.DATE_BEG == this.DEAL_EXT
                        .Where(ss1 => ss1.DEAL_ID == d_sel.DEAL_ID)
                        .Max(ss1 => ss1.DATE_BEG))
                    .DefaultIfEmpty()
                from p_sel in this.PARTNER
                    .Where(ss => ss.PARTNER_ID == de_sel.SELLER_ID)
                    .DefaultIfEmpty()
                select new
                {
                    DEAL_ID = d.DEAL_ID,
                    PAYMENT_DATE_BEG = doc.DATE_BEG,
                    PAYMENT_SUMMA = doc.DOCUMENT_CREDIT.Sum(ss => ss.SUMMA),
                    SELLER_NAME = p_sel.PARTNER_NAME_CALC,
                };
            #endregion

            #region queryAddress
            var queryAddress = this.Get_VW_ADRESS();
            #endregion

            #region queryUser
            //var queryUser = this.Get_VW_SYS_USER();
            #endregion

            IQueryable<VW_DEAL_CONSUM_PRIVAT_CLIENT> query =
                from d in this.DEAL
                from de in this.DEAL_EXT
                    .Where(ss => ss.DEAL_ID == d.DEAL_ID)
                    .Where(ss => ss.DATE_BEG == this.DEAL_EXT
                        .Where(ss1 => ss1.DEAL_ID == d.DEAL_ID)
                        .Max(ss1 => ss1.DATE_BEG))
                    .DefaultIfEmpty()
                from ap in this.NSI_ACCOUNT_PERIOD
                    .Where(ss => ss.NACCOUNT_PERIOD_ID == d.NACCOUNT_PERIOD_ID)
                    .Where(ss => ss.ACCOUNT_PERIOD_TYPE == (int)NsiDepartament_Enum.Fiziki)
                from c in this.CONSUM
                    .Where(ss => ss.DEAL_ID == d.DEAL_ID)
                from f in this.FACILITY
                    .Where(ss => ss.FACILITY_ID == c.FACILITY_ID)
                from fe in this.FACILITY_EXT
                    .Where(ss => ss.FACILITY_ID == f.FACILITY_ID)
                    .Where(ss => ss.DATE_BEG == this.FACILITY_EXT
                        .Where(ss1 => ss1.FACILITY_ID == f.FACILITY_ID)
                        .Max(ss1 => ss1.DATE_BEG))
                    .DefaultIfEmpty()
                from fp in this.FACILITY_PRODUCT
                    .Where(ss => ss.FACILITY_ID == f.FACILITY_ID)
                from fpe in this.FACILITY_PRODUCT_EXT
                    .Where(ss => ss.FACILITY_PRODUCT_ID == fp.FACILITY_PRODUCT_ID)
                    .Where(ss => ss.DATE_BEG == this.FACILITY_PRODUCT_EXT
                        .Where(ss1 => ss1.FACILITY_PRODUCT_ID == fp.FACILITY_PRODUCT_ID)
                        .Max(ss1 => ss1.DATE_BEG))
                    .DefaultIfEmpty()

                from addr in queryAddress
                    .Where(ss => ss.BUILD_ID == f.BUILD_ID)
                    .DefaultIfEmpty()
                from nf in this.NSI_FACILITY
                    .Where(ss => ss.NFACILITY_ID == f.NFACILITY_ID)
                    .DefaultIfEmpty()
                from fpe_nrz in this.NSI_REAZON
                    .Where(ss => ss.NREAZON_ID == fpe.NREASON_ID)
                    .DefaultIfEmpty()
                from fep_6 in queryExtParam
                    .Where(ss => ss.STABLE_ID == (int)SysTable_Enum.Facility)
                    .Where(ss => ss.ID == f.FACILITY_ID)
                    .Where(ss => ss.NPARAM_ID == 6)
                    .DefaultIfEmpty()
                from fep_63 in queryExtParam
                    .Where(ss => ss.STABLE_ID == (int)SysTable_Enum.Facility)
                    .Where(ss => ss.ID == f.FACILITY_ID)
                    .Where(ss => ss.NPARAM_ID == 63)
                    .DefaultIfEmpty()
                from dep_119 in queryExtParam
                    .Where(ss => ss.STABLE_ID == (int)SysTable_Enum.Deal)
                    .Where(ss => ss.ID == d.DEAL_ID)
                    .Where(ss => ss.NPARAM_ID == 119)
                    .DefaultIfEmpty()
                from buyer in queryBuyer
                    .Where(ss => ss.PARTNER_ID == de.BUYER_ID)
                    .DefaultIfEmpty()
                from saldo1 in querySaldo
                    .Where(ss => ss.DEAL_ID == d.DEAL_ID)
                    .Where(ss => ss.SALDO_ACCOUNT_PERIOD == ap.ACCOUNT_PERIOD)
                    .Where(ss => ss.SALDO_NSZ_GROUP_ID == nSZGroupId1)
                    .DefaultIfEmpty()
                from saldo2 in querySaldo
                    .Where(ss => ss.DEAL_ID == d.DEAL_ID)
                    .Where(ss => ss.SALDO_ACCOUNT_PERIOD == ap.ACCOUNT_PERIOD)
                    .Where(ss => ss.SALDO_NSZ_GROUP_ID == nSZGroupId2)
                    .DefaultIfEmpty()
                from price in queryPrice
                    .Where(ss => ss.DEAL_ID == d.DEAL_ID)
                    .DefaultIfEmpty()
                from mc in queryPartnerManagementCompany
                    .Where(ss => ss.BUILD_ID == f.BUILD_ID)
                    .DefaultIfEmpty()
                from ch in queryChargeMax
                    .Where(ss => ss.DEAL_ID == d.DEAL_ID)
                    .DefaultIfEmpty()
                from bill in queryBillMax
                    .Where(ss => ss.DEAL_ID == d.DEAL_ID)
                    .DefaultIfEmpty()
                from paym in queryPaymentLast
                    .Where(ss => ss.DEAL_ID == d.DEAL_ID)
                    .DefaultIfEmpty()
                from calcItem in queryCalcItemForDistr
                    .Where(ss => ss.FACILITY_PRODUCT_ID == fp.FACILITY_PRODUCT_ID)
                    .DefaultIfEmpty()
                select new VW_DEAL_CONSUM_PRIVAT_CLIENT
                {
                    #region
                    
                    #region DEAL
                    DEAL_ID = d.DEAL_ID,
                    DEAL_NUM = d.DEAL_NUM,
                    DEAL_DATE_BEG = d.DATE_BEG,
                    DEAL_DATE_END = d.DATE_END,
                    SHOW_DAY_END = dep_119.VALUE,
                    SALDO1 = saldo1.SALDO,
                    SALDO2 = saldo2.SALDO,
                    #endregion

                    #region BUYER
                    PBUY_PARTNER_ID = buyer.PARTNER_ID,
                    PBUY_PARTNER_NAME = buyer.PARTNER_NAME_CALC,
                    PBUY_DATE_BORN = buyer.DATE_BORN,
                    PBUY_PLACE_BORN = buyer.PLACE_BORN,
                    PBUY_CONTACTs = buyer.PARTNER_CONTACTs,
                        //.Select(ss => new WCF.Web.Model.PARTNER_CONTACT
                        //{
                        //    NCONTACT_ID = ss.NCONTACT_ID,
                        //    VALUE = ss.VALUE,
                        //}),
                    #endregion

                    #region FACILITY / CONSUM
                    FACILITY_ID = f.FACILITY_ID,
                    CONSUM_ID = c.CONSUM_ID,
                    FPROD_DATE_END = fp.DATE_END,
                    FPROD_STATE_TYPE = fpe.STATE_TYPE,
                    FPROD_STATE_TYPE_REASON = fpe_nrz.NREAZON_NAME,
                    NFACILITY_ID = f.NFACILITY_ID,
                    NFACILITY_NAME = nf.NFACILITY_NAME,
                    RESIDENT_COUNT = fep_6.VALUE,
                    RESIDENT_OFFICIAL_COUNT = fep_63.VALUE,
                    ROOMS = fe.ROOMS,
                    FLOOR = fe.FLOOR,
                    LIFT = fe.LIFT,
                    UK_PARTNER_NAME = mc.PARTNER_NAME_CALC,
                    ADDRESS = f.ADDRESS,
                    NVILLAGE_ID = addr.NVILLAGE_ID,
                    VW_ADRESS = addr,
                    #endregion

                    #region CHARGE_ITEM / CALC_ITEM
                    CHILD_IN_CALC_SCHEM = calcItem.IN_SCHEM,
                    NPRICE_ID = price.NPRICE_ID,
                    NPRICE_NAME = price.NPRICE_NAME,
                    PRICE_SUMMA1 = price.SUMMA1,
                    PRICE_SUMMA2 = price.SUMMA2,
                    PRICE_SUMMA3 = price.SUMMA3,
                    CHARGE_DATE_END_MAX = ch.DATE_END_MAX,
                    #endregion

                    #region DOCUMENT
                    BILL_ACCOUNT_PERIOD_MAX = bill.ACCOUNT_PERIOD_MAX,
                    PAYMENT_LAST_DATE_BEG = paym.PAYMENT_DATE_BEG,
                    PAYMENT_LAST_SUMMA = paym.PAYMENT_SUMMA,
                    PAYMENT_LAST_COLLECTOR_NAME = paym.SELLER_NAME,
                    #endregion

                    #region SYS_USER
                    //MFY_SUSER_ID = d.MFY_SUSER_ID,
                    //MFY_DATE = d.MFY_DATE,
                    //SUSER_NAME = us.SUSER_NAME,
                    //SUSER_PNAME = us.SUSER_PNAME,
                    #endregion

                    #endregion
                };
            return query;
            #endregion
        }
        #endregion
    }

    [XmlRoot("Config")]
    public class VwDealConsumPrivatClientConfig
    {
        [XmlElement("Saldo")]
        public VwDealConsumPrivatClientConfigSaldo Saldo { get; set; }
    }

    public class VwDealConsumPrivatClientConfigSaldo
    {
        [XmlAttribute("NSZGroupId1")]
        public int NSZGroupId1 { get; set; }

        [XmlAttribute("NSZGroupId2")]
        public int NSZGroupId2 { get; set; }
    }
}