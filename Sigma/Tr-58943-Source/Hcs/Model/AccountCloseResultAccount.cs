﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hcs.Model
{
    public partial class AccountCloseResultAccount : ITransactionObjectEntity
    {
        public AccountCloseResultAccount()
        {
            AccountCloseResultAccountErrors = new HashSet<AccountCloseResultAccountError>();
        }

        public long uniqueId { get; set; }
        public Guid TransactionGUID { get; set; }
        [StringLength(32)]
        public string objectId { get; set; }
        [Key]
        public Guid TransportGUID { get; set; }
        public Guid AccountCloseTransportGUID { get; set; }
        [Required]
        [StringLength(30)]
        public string AccountNumber { get; set; }
        public Guid AccountGUID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }

        [ForeignKey(nameof(AccountCloseTransportGUID))]
        [InverseProperty(nameof(AccountCloseResult.AccountCloseResultAccounts))]
        public virtual AccountCloseResult AccountCloseTransportGU { get; set; }
        [InverseProperty(nameof(AccountCloseResultAccountError.AccountCloseAccountTransportGU))]
        public virtual ICollection<AccountCloseResultAccountError> AccountCloseResultAccountErrors { get; set; }
    }
}