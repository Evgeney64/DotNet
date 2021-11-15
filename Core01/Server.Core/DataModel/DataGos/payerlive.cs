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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class payerlive : IEntityObject
    {
        #region Columns
        long IEntityObject.Id { get { return Adr; } }//;
        [KeyAttribute()]
        public int Adr { get; set; }//;
        public int pay_id { get; set; }//;
        public System.DateTime date_beg { get; set; }//;
        public System.Nullable<int> retail_portion { get; set; }//;
        public System.Nullable<int> ftarif_alg_id { get; set; }//;
        public System.Nullable<int> feder_portion { get; set; }//;
        public System.Nullable<int> reg_portion { get; set; }//;
        public System.Nullable<int> mun_portion { get; set; }//;
        public System.Nullable<System.DateTime> date_end { get; set; }//;
        public System.Nullable<int> raise_nazn_id { get; set; }//;
        public System.Nullable<decimal> portion { get; set; }//;
        public System.Nullable<int> okonh { get; set; }//;
        public string okved { get; set; }//;
        public System.Nullable<bool> dog_buy_sale { get; set; }//;
        public System.Nullable<bool> ms_biz { get; set; }//;
        public System.Nullable<int> out_portion { get; set; }//;
        public System.Nullable<int> srs_id { get; set; }//;
        public System.Nullable<int> department_id { get; set; }//;
        public System.Nullable<int> spers_calc { get; set; }//;
        public System.Nullable<int> spers_dog { get; set; }//;
        public System.Nullable<int> spers_insp { get; set; }//;
        public System.Nullable<int> spers_buh { get; set; }//;
        public System.Nullable<int> spers_fin { get; set; }//;
        public System.Nullable<int> ref1_id { get; set; }//;
        public System.Nullable<int> ref2_id { get; set; }//;
        public System.Nullable<int> ref3_id { get; set; }//;
        public System.Nullable<int> ref4_id { get; set; }//;
        public System.Nullable<int> ref5_id { get; set; }//;
        public System.Nullable<int> ref6_id { get; set; }//;
        public System.Nullable<int> ref7_id { get; set; }//;
        public System.Nullable<int> ref8_id { get; set; }//;
        public System.Nullable<int> ref9_id { get; set; }//;
        public System.Nullable<int> ref10_id { get; set; }//;
        public System.Nullable<int> ref11_id { get; set; }//;
        public System.Nullable<int> ref12_id { get; set; }//;
        public System.Nullable<int> ref13_id { get; set; }//;
        public System.Nullable<int> ref14_id { get; set; }//;
        public System.Nullable<int> ref15_id { get; set; }//;
        public System.Nullable<decimal> contract_sum { get; set; }//;
        public System.Nullable<decimal> budget_sum { get; set; }//;
        public System.Nullable<decimal> unbudget_sum { get; set; }//;
        public System.Nullable<decimal> other_sum { get; set; }//;
        public string dog_num_ext { get; set; }//;
        public System.Nullable<System.DateTime> date_contract { get; set; }//;
        public System.Nullable<System.DateTime> date_cancel { get; set; }//;
        public System.Nullable<int> rpt47a_group_id { get; set; }//;
        public System.Nullable<bool> bitExtBefore { get; set; }//;
        public System.Nullable<int> operator_id { get; set; }//;
        public System.Nullable<int> reciever_id { get; set; }//;
        public bool reciever_eq_buyer { get; set; }//;
        public System.Nullable<int> payer_id { get; set; }//;
        public System.Nullable<byte> PercPayerType_id { get; set; }//;
        public byte invoiceStatusUPD { get; set; }//;
        public System.Nullable<byte> perc_type { get; set; }//;
        public System.Nullable<short> perc_sleep { get; set; }//;
        public int billPaymentDeadlineDayType { get; set; }//;
        public System.Nullable<int> perc_batch_id { get; set; }//;
        public System.Nullable<bool> perc_grouped { get; set; }//;
        public string identGosContract { get; set; }//;
        public System.Nullable<System.DateTime> ver_date { get; set; }//;
        public System.Nullable<short> ver_u_id { get; set; }//;
        #endregion
        #region Navigation - parents
        // FK_payerlive_partners__reciever_id_PK_partners
        [ForeignKey("reciever_id")]
        public virtual Partners Partners { get; set; }//;
        // FK_payerlive_partners__payer_id_PK_partners
        [ForeignKey("payer_id")]
        public virtual Partners Partners1 { get; set; }//;
        #endregion
    }
}
