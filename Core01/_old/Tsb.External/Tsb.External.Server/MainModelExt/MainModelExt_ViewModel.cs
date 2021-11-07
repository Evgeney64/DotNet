using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tsb.WCF.Web;
using Tsb.WCF.Web.Model;

namespace Tsb.External.Server.MainModelExt
{
    #region  VW_DEAL_CONSUM_PRIVAT_CLIENT
    public partial class VW_DEAL_CONSUM_PRIVAT_CLIENT : DEAL
    {
        #region Deal
        [ExtraDescription("Дата открытия ЛС")]
        public DateTime DEAL_DATE_BEG { get; set; }

        [ExtraDescription("Дата закрытия ЛС")]
        public DateTime? DEAL_DATE_END { get; set; }

        [ExtraDescription("Крайнее число передачи показаний")]
        public string SHOW_DAY_END { get; set; }

        [ExtraDescription("Сальдо")]
        public decimal? SALDO1 { get; set; }

        [ExtraDescription("Сальдо пени")]
        public decimal? SALDO2 { get; set; }
        #endregion

        #region Buyer
        [ExtraDescription("Код покупателя")]
        public long? PBUY_PARTNER_ID { get; set; }

        [ExtraDescription("Имя покупателя")]
        public string PBUY_PARTNER_NAME { get; set; }

        [ExtraDescription("Дата рождения покупателя")]
        public DateTime? PBUY_DATE_BORN { get; set; }

        [ExtraDescription("Место рождения покупателя")]
        public string PBUY_PLACE_BORN { get; set; }

        [ExtraDescription("Телефон покупателя")]
        public string PBUY_Phone
        {
            get
            {
                var values = this.PBUY_CONTACTs?
                    .Where(ss => ss.NCONTACT_ID == (int)NsiContact_Enum.StationPhone)
                    .Select(ss => ss.VALUE);
                return String.Join(",", values);
            }
        }

        [ExtraDescription("Мобильный телефон покупателя")]
        public string PBUY_CellPhone
        {
            get
            {
                var values = this.PBUY_CONTACTs?
                    .Where(ss => ss.NCONTACT_ID == (int)NsiContact_Enum.MobilePhone)
                    .Select(ss => ss.VALUE);
                return String.Join(",", values);
            }
        }

        [ExtraDescription("Электронная почта покупателя")]
        public string PBUY_Email
        {
            get
            {
                var values = this.PBUY_CONTACTs?
                    .Where(ss => ss.NCONTACT_ID == (int)NsiContact_Enum.Email)
                    .Select(ss => ss.VALUE);
                return String.Join(",", values);
            }
        }

        [NoMetaData]
        [JsonIgnore]
        public IEnumerable<PARTNER_CONTACT> PBUY_CONTACTs { get; set; }
        #endregion

        #region Facility
        [ExtraDescription("Код объекта снабжения")]
        public long FACILITY_ID { get; set; }

        [ExtraDescription("Код потребителя")]
        public long CONSUM_ID { get; set; }

        [ExtraDescription("Дата закрытия услуги")]
        public DateTime? FPROD_DATE_END { get; set; }

        [ExtraDescription("Состояние услуги")]
        public int? FPROD_STATE_TYPE { get; set; }

        [ExtraDescription("Причина перехода в состояние услуги")]
        public string FPROD_STATE_TYPE_REASON { get; set; }

        [ExtraDescription("Код типа объекта снабжения")]
        public new int? NFACILITY_ID { get; set; }

        [ExtraDescription("Тип объекта снабжения")]
        public string NFACILITY_NAME { get; set; }

        [ExtraDescription("Количество проживающих")]
        public string RESIDENT_COUNT { get; set; }

        [ExtraDescription("Количество прописанных")]
        public string RESIDENT_OFFICIAL_COUNT { get; set; }

        [ExtraDescription("Количество комнат")]
        public short? ROOMS { get; set; }

        [ExtraDescription("Этаж")]
        public short? FLOOR { get; set; }

        [ExtraDescription("Наличие лифта")]
        public short? LIFT { get; set; }

        [ExtraDescription("Управляющая компания")]
        public string UK_PARTNER_NAME { get; set; }

        [ExtraDescription("Адрес объекта снабжения")]
        public string ADDRESS { get; set; }

        [ExtraDescription("Код населенного пункта")]
        public long? NVILLAGE_ID { get; set; }

        [NoMetaData]
        [JsonIgnore]
        public VW_ADRESS VW_ADRESS { get; set; }
        #endregion

        #region ChargeItem / CalcItem
        [ExtraDescription("Участвует в распределении МОП")]
        public short? CHILD_IN_CALC_SCHEM { get; set; }

        [ExtraDescription("Код тарифа")]
        public int? NPRICE_ID { get; set; }

        [ExtraDescription("Наименование тарифа")]
        public string NPRICE_NAME { get; set; }

        [ExtraDescription("Ставка тарифа день")]
        public decimal? PRICE_SUMMA1 { get; set; }

        [ExtraDescription("Ставка тарифа ночь")]
        public decimal? PRICE_SUMMA2 { get; set; }

        [ExtraDescription("Ставка тарифа полупик")]
        public decimal? PRICE_SUMMA3 { get; set; }

        [ExtraDescription("Максимальный период начисления")]
        public DateTime? CHARGE_DATE_END_MAX { get; set; }
        #endregion

        #region Document
        [ExtraDescription("Максимальный отчетный период квитанции")]
        public DateTime? BILL_ACCOUNT_PERIOD_MAX { get; set; }

        [ExtraDescription("Дата последней оплаты")]
        public DateTime? PAYMENT_LAST_DATE_BEG { get; set; }

        [ExtraDescription("Сумма последней оплаты")]
        public decimal? PAYMENT_LAST_SUMMA { get; set; }

        [ExtraDescription("Имя сборщика последней оплаты")]
        public string PAYMENT_LAST_COLLECTOR_NAME { get; set; }
        #endregion
    }
    #endregion
}


