﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hcs.Model
{
    public partial class PaymentExportRequestDocument
    {
        public long uniqueId { get; set; }
        public Guid TransactionGUID { get; set; }
        [StringLength(32)]
        public string objectId { get; set; }
        [Key]
        public Guid TransportGUID { get; set; }
        public Guid PaymentExportTransportGUID { get; set; }
        [StringLength(18)]
        public string PaymentDocumentID { get; set; }
        [StringLength(30)]
        public string PaymentDocumentNumber { get; set; }

        [ForeignKey(nameof(PaymentExportTransportGUID))]
        [InverseProperty(nameof(PaymentExportRequest.PaymentExportRequestDocuments))]
        public virtual PaymentExportRequest PaymentExportTransportGU { get; set; }
    }
}