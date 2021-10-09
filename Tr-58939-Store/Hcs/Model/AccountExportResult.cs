﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hcs.Model
{
    public partial class AccountExportResult
    {
        public AccountExportResult()
        {
            AccountExportResultPercentPremises = new HashSet<AccountExportResultPercentPremise>();
        }

        public long uniqueId { get; set; }
        public Guid TransactionGUID { get; set; }
        [Key]
        public Guid TransportGUID { get; set; }
        public Guid AccountGUID { get; set; }
        [StringLength(30)]
        public string AccountNumber { get; set; }
        [Required]
        [StringLength(10)]
        public string UnifiedAccountNumber { get; set; }
        [StringLength(13)]
        public string ServiceID { get; set; }
        public bool IsRSOAccount { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreationDate { get; set; }
        public short? LivingPersonsNumber { get; set; }
        [Column(TypeName = "decimal(25, 4)")]
        public decimal? TotalSquare { get; set; }
        [Column(TypeName = "decimal(25, 4)")]
        public decimal? ResidentialSquare { get; set; }
        [Column(TypeName = "decimal(25, 4)")]
        public decimal? HeatedArea { get; set; }
        [StringLength(20)]
        public string AccountCloseReasonCode { get; set; }
        public Guid? AccountCloseReasonGUID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CloseDate { get; set; }
        [StringLength(250)]
        public string CloseDescription { get; set; }
        public bool? IsRenter { get; set; }
        public bool? IsAccountsDivided { get; set; }
        public Guid? OrgVersionGUID { get; set; }

        [InverseProperty(nameof(AccountExportResultPercentPremise.AccountExportTransportGU))]
        public virtual ICollection<AccountExportResultPercentPremise> AccountExportResultPercentPremises { get; set; }
    }
}