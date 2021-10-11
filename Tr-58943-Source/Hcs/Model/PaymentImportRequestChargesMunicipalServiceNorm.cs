﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hcs.Model
{
    public partial class PaymentImportRequestChargesMunicipalServiceNorm : ITransactionObjectEntity
    {
        public long uniqueId { get; set; }
        public Guid TransactionGUID { get; set; }
        [StringLength(32)]
        public string objectId { get; set; }
        [Key]
        public Guid TransportGUID { get; set; }
        [Column(TypeName = "decimal(20, 6)")]
        public decimal? IndividualConsumptionCurrentValue { get; set; }
        [Column(TypeName = "decimal(20, 6)")]
        public decimal? HouseOverallNeedsCurrentValue { get; set; }
        [Column(TypeName = "decimal(22, 7)")]
        public decimal? HouseTotalIndividualConsumption { get; set; }
        [Column(TypeName = "decimal(22, 7)")]
        public decimal? HouseTotalHouseOverallNeeds { get; set; }
        [Column(TypeName = "decimal(22, 7)")]
        public decimal? HouseOverallNeedsNorm { get; set; }
        [Column(TypeName = "decimal(22, 7)")]
        public decimal? IndividualConsumptionNorm { get; set; }

        [ForeignKey(nameof(TransportGUID))]
        [InverseProperty(nameof(PaymentImportRequestChargesMunicipalService.PaymentImportRequestChargesMunicipalServiceNorm))]
        public virtual PaymentImportRequestChargesMunicipalService TransportGU { get; set; }
    }
}