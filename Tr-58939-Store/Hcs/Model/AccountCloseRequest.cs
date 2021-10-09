﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hcs.Model
{
    public partial class AccountCloseRequest
    {
        public long uniqueId { get; set; }
        public Guid TransactionGUID { get; set; }
        [StringLength(32)]
        public string objectId { get; set; }
        [Key]
        public Guid TransportGUID { get; set; }
        public Guid FIASHouseGuid { get; set; }
        [Required]
        [StringLength(30)]
        public string AccountNumber { get; set; }
        public Guid? AccountGUID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CloseDate { get; set; }
        [StringLength(250)]
        public string CloseDescription { get; set; }
        [Required]
        [StringLength(20)]
        public string CloseReasonCode { get; set; }
        public Guid CloseReasonGUID { get; set; }
    }
}