using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hcs.Store
{

    public partial class SysTransaction
    {
        public SysTransaction()
        {
            //SysTransactionLogs = new HashSet<SysTransactionLog>();
            //SysTransactionParams = new HashSet<SysTransactionParam>();
            //SysTransactionState2s = new HashSet<SysTransactionState2>();
        }

        [Key]
        public Guid TransactionGUID { get; set; }
        public Guid? ListGUID { get; set; }
        [Required]
        [StringLength(32)]
        public string ClientId { get; set; }
        public int OperationId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndDate { get; set; }
        public int StatusId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastExecutionDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? NextExecutionDate { get; set; }
        public int? ResultId { get; set; }
        [StringLength(32)]
        public string ErrorCode { get; set; }
        [StringLength(4000)]
        public string ErrorDescription { get; set; }

        //[ForeignKey(nameof(OperationId))]
        //[InverseProperty(nameof(SysOperation.SysTransactions))]
        //public virtual SysOperation Operation { get; set; }
        //[InverseProperty(nameof(SysTransactionLog.TransactionGU))]
        //public virtual ICollection<SysTransactionLog> SysTransactionLogs { get; set; }
        //[InverseProperty(nameof(SysTransactionParam.TransactionGU))]
        //public virtual ICollection<SysTransactionParam> SysTransactionParams { get; set; }
        //[InverseProperty(nameof(SysTransactionState2.TransactionGU))]
        //public virtual ICollection<SysTransactionState2> SysTransactionState2s { get; set; }
    }

}