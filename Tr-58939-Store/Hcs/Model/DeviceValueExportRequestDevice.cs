﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hcs.Model
{
    public partial class DeviceValueExportRequestDevice
    {
        [Key]
        public Guid TransportGUID { get; set; }
        public Guid DeviceValueExportTransportGUID { get; set; }
        public Guid DeviceGUID { get; set; }
        public long uniqueId { get; set; }
        public Guid TransactionGUID { get; set; }

        [ForeignKey(nameof(DeviceValueExportTransportGUID))]
        [InverseProperty(nameof(DeviceValueExportRequest.DeviceValueExportRequestDevices))]
        public virtual DeviceValueExportRequest DeviceValueExportTransportGU { get; set; }
    }
}