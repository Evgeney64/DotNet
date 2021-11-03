﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hcs.Model
{
    public partial class ContractImportRequestAttachment
    {
        public long uniqueId { get; set; }
        public Guid TransactionGUID { get; set; }
        [StringLength(32)]
        public string objectId { get; set; }
        [Key]
        public Guid TransportGUID { get; set; }
        public Guid ContractImportTransportGUID { get; set; }
        [Required]
        [StringLength(1024)]
        public string Name { get; set; }
        [Required]
        [StringLength(500)]
        public string Description { get; set; }
        public byte[] AttachmentBody { get; set; }
        public string AttachmentHASH { get; set; }
        public Guid? AttachmentGUID { get; set; }

        [ForeignKey(nameof(ContractImportTransportGUID))]
        [InverseProperty(nameof(ContractImportRequest.ContractImportRequestAttachments))]
        public virtual ContractImportRequest ContractImportTransportGU { get; set; }
    }
}