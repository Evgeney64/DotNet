﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hcs.Model
{
    public partial class PaymentImportResult
    {
        public PaymentImportResult()
        {
            PaymentImportResultErrors = new HashSet<PaymentImportResultError>();
        }

        public long uniqueId { get; set; }
        public Guid TransactionGUID { get; set; }
        [StringLength(32)]
        public string objectId { get; set; }
        [Key]
        public Guid TransportGUID { get; set; }
        public Guid? PaymentDocumentGUID { get; set; }
        [StringLength(18)]
        public string PaymentDocumentID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }

        [InverseProperty(nameof(PaymentImportResultError.PaymentImportTransportGU))]
        public virtual ICollection<PaymentImportResultError> PaymentImportResultErrors { get; set; }
    }
}