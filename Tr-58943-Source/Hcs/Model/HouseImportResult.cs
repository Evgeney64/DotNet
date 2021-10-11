﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hcs.Model
{
    public partial class HouseImportResult : ITransactionObjectEntity
    {
        public HouseImportResult()
        {
            HouseImportResultBlocks = new HashSet<HouseImportResultBlock>();
            HouseImportResultEntrances = new HashSet<HouseImportResultEntrance>();
            HouseImportResultErrors = new HashSet<HouseImportResultError>();
            HouseImportResultLivingRooms = new HashSet<HouseImportResultLivingRoom>();
            HouseImportResultPremises = new HashSet<HouseImportResultPremise>();
        }

        public long uniqueId { get; set; }
        public Guid TransactionGUID { get; set; }
        [StringLength(32)]
        public string objectId { get; set; }
        [Key]
        public Guid TransportGUID { get; set; }
        public Guid? HouseGUID { get; set; }
        [StringLength(31)]
        public string HouseUniqueNumber { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }

        [InverseProperty(nameof(HouseImportResultBlock.HouseImportTransportGU))]
        public virtual ICollection<HouseImportResultBlock> HouseImportResultBlocks { get; set; }
        [InverseProperty(nameof(HouseImportResultEntrance.HouseImportTransportGU))]
        public virtual ICollection<HouseImportResultEntrance> HouseImportResultEntrances { get; set; }
        [InverseProperty(nameof(HouseImportResultError.HouseImportTransportGU))]
        public virtual ICollection<HouseImportResultError> HouseImportResultErrors { get; set; }
        [InverseProperty(nameof(HouseImportResultLivingRoom.HouseImportTransportGU))]
        public virtual ICollection<HouseImportResultLivingRoom> HouseImportResultLivingRooms { get; set; }
        [InverseProperty(nameof(HouseImportResultPremise.HouseImportTransportGU))]
        public virtual ICollection<HouseImportResultPremise> HouseImportResultPremises { get; set; }
    }
}