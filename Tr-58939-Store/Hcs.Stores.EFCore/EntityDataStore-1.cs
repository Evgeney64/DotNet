using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace Hcs.Model
{
    public interface ITransactionEntity
    {
        Guid TransactionGUID { get; set; }
        Guid TransportGUID { get; set; }
    }

    public interface ITransactionObjectEntity : ITransactionEntity
    {
        string objectId { get; set; }
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NullableAttribute : Attribute
    {
        public NullableAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class StringLengthAttribute : Attribute
    {
        public short Length { get; private set; }

        public StringLengthAttribute(short length)
        {
            this.Length = length;
        }
    }
    public partial class AttachmentPostRequest : ITransactionObjectEntity
    {
        public long uniqueId { get; set; }
        public System.Guid TransactionGUID { get; set; }
        [Nullable, StringLength(32)]
        public string objectId { get; set; }
        public System.Guid TransportGUID { get; set; }
        [StringLength(1024)]
        public string Name { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public byte[] AttachmentBody { get; set; }
        public short numCopies { get; set; }

    }

    public partial class AttachmentPostResultCopy : ITransactionObjectEntity
    {
        public long uniqueId { get; set; }
        public System.Guid TransactionGUID { get; set; }
        [Nullable, StringLength(32)]
        public string objectId { get; set; }
        public System.Guid TransportGUID { get; set; }
        public System.Guid AttachmentPostTransportGUID { get; set; }
        public System.Guid AttachmentGUID { get; set; }

        public virtual AttachmentPostResult AttachmentPostResult { get; set; }

    }

    public partial class AttachmentPostResult : ITransactionObjectEntity
    {
        public AttachmentPostResult()
        {
            this.AttachmentPostResultCopies = new HashSet<AttachmentPostResultCopy>();
        }

        public long uniqueId { get; set; }
        public System.Guid TransactionGUID { get; set; }
        [Nullable, StringLength(32)]
        public string objectId { get; set; }
        public System.Guid TransportGUID { get; set; }
        [StringLength(1024)]
        public string AttachmentHASH { get; set; }
        [Nullable, StringLength(32)]
        public string ErrorCode { get; set; }
        [Nullable, StringLength(128)]
        public string ErrorDescription { get; set; }

        public virtual ICollection<AttachmentPostResultCopy> AttachmentPostResultCopies { get; set; }

    }

}