﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hcs.Model
{
    public partial class NsiExportResultField
    {
        public long uniqueId { get; set; }
        public Guid TransactionGUID { get; set; }
        [Key]
        public Guid TransportGUID { get; set; }
        public Guid NsiExportTransportGUID { get; set; }
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        [Required]
        [StringLength(1024)]
        public string Value { get; set; }

        [ForeignKey(nameof(NsiExportTransportGUID))]
        [InverseProperty(nameof(NsiExportResult.NsiExportResultFields))]
        public virtual NsiExportResult NsiExportTransportGU { get; set; }
    }
}