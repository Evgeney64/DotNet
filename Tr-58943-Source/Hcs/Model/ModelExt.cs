using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

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

    public interface IError : ITransactionObjectEntity
    {
        Guid ParentTransportGUID { get; set; }
        string ErrorCode { get; set; }
        string ErrorDescription { get; set; }
    }

    public interface IResultEntity : ITransactionObjectEntity
    {
        IEnumerable<IError> ResultErrors { get; }
    }

    public interface IResultEntity<E> : ITransactionObjectEntity
        where E : IError
    {
        ICollection<E> ResultErrors { get; set; }
    }

    public interface IAttachment
    {
        string Name { get; }
        string Description { get; }
        byte[] Attachment { get; }
        Guid AttachmentGUID { get; set; }
        string AttachmentHASH { get; }
    }

    public interface IGKN_EGRP
    {
        string CadastralNumber { get; }
    }

    public interface IHouseAnnulment
    {
        string AnnulmentReasonCode { get; }
        Guid? AnnulmentReasonGUID { get; }
        string AnnulmentInfo { get; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DecimalPrecisionAttribute : Attribute
    {
        public byte Precision { get; private set; }
        public byte Scale { get; private set; }

        public DecimalPrecisionAttribute(byte precision, byte scale)
        {
            this.Precision = precision;
            this.Scale = scale;
        }
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

    public partial class DeviceImportRequest
    {
        public bool DeviceValuesHasSended { get; set; } //todo: Реализовать выборку инофрмации о том, были ли отправлены данные по показания в ГИС.
    }

    public partial class AttachmentPostRequest : IAttachment
    {
        byte[] IAttachment.Attachment
        {
            get
            {
                return this.AttachmentBody;
            }
        }
        string IAttachment.AttachmentHASH
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        Guid IAttachment.AttachmentGUID { get; set; }

        public IAttachment New()
        {
            return new AttachmentPostRequest
            {
                Name = this.Name,
                Description = this.Description,
                AttachmentBody = this.AttachmentBody,
            };
        }
    }

    public partial class HouseImportRequest : IGKN_EGRP
    {
    }
    public partial class HouseImportRequestEntrance : IHouseAnnulment
    {
    }
    public partial class HouseImportRequestBlock : IGKN_EGRP, IHouseAnnulment
    {
    }
    public partial class HouseImportRequestPremise : IGKN_EGRP, IHouseAnnulment
    {
    }
    public partial class HouseImportRequestLivingRoom : IGKN_EGRP, IHouseAnnulment
    {
    }

    public partial class ContractImportRequest
    {
        public ContractImportRequestAttachment[] ContractImportRequestAttachmentsArray
        {
            get
            {
                return this.ContractImportRequestAttachments == null ?
                    null :
                    this.ContractImportRequestAttachments.ToArray();
            }
            set
            {
                this.ContractImportRequestAttachments = value == null ?
                    null :
                    new HashSet<ContractImportRequestAttachment>(value);
            }
        }
        public ContractImportRequestObjectAddress[] ContractImportRequestObjectAddressesArray
        {
            get
            {
                return this.ContractImportRequestObjectAddresses == null ?
                    null :
                    this.ContractImportRequestObjectAddresses.ToArray();
            }
            set
            {
                this.ContractImportRequestObjectAddresses = value == null ?
                    null :
                    new HashSet<ContractImportRequestObjectAddress>(value);
            }
        }
        public ContractImportRequestSubject[] ContractImportRequestSubjectsArray
        {
            get
            {
                return this.ContractImportRequestSubjects == null ?
                    null :
                    this.ContractImportRequestSubjects.ToArray();
            }
            set
            {
                this.ContractImportRequestSubjects = value == null ?
                    null :
                    new HashSet<ContractImportRequestSubject>(value);
            }
        }
    }
    public partial class ContractImportRequestSubject
    {
        public ContractImportRequestObjectServiceResource[] ContractImportRequestObjectServiceResourcesArray
        {
            get
            {
                return this.ContractImportRequestObjectServiceResources == null ?
                    null :
                    this.ContractImportRequestObjectServiceResources.ToArray();
            }
            set
            {
                this.ContractImportRequestObjectServiceResources = value == null ?
                    null :
                    new HashSet<ContractImportRequestObjectServiceResource>(value);
            }
        }
        public ContractImportRequestSubjectQualityIndicator[] ContractImportRequestSubjectQualityIndicatorsArray
        {
            get
            {
                return this.ContractImportRequestSubjectQualityIndicators == null ?
                    null :
                    this.ContractImportRequestSubjectQualityIndicators.ToArray();
            }
            set
            {
                this.ContractImportRequestSubjectQualityIndicators = value == null ?
                    null :
                    new HashSet<ContractImportRequestSubjectQualityIndicator>(value);
            }
        }
    }
    public partial class ContractImportRequestObjectAddress
    {
        public ContractImportRequestObjectServiceResource[] ContractImportRequestObjectServiceResourcesArray
        {
            get
            {
                return this.ContractImportRequestObjectServiceResources == null ?
                    null :
                    this.ContractImportRequestObjectServiceResources.ToArray();
            }
            set
            {
                this.ContractImportRequestObjectServiceResources = value == null ?
                    null :
                    new HashSet<ContractImportRequestObjectServiceResource>(value);
            }
        }
        public ContractImportRequestSubjectQualityIndicator[] ContractImportRequestSubjectQualityIndicatorsArray
        {
            get
            {
                return this.ContractImportRequestSubjectQualityIndicators == null ?
                    null :
                    this.ContractImportRequestSubjectQualityIndicators.ToArray();
            }
            set
            {
                this.ContractImportRequestSubjectQualityIndicators = value == null ?
                    null :
                    new HashSet<ContractImportRequestSubjectQualityIndicator>(value);
            }
        }
    }

    public partial class ContractImportResult : IResultEntity<ContractImportResultError>, IResultEntity
    {
        ICollection<ContractImportResultError> IResultEntity<ContractImportResultError>.ResultErrors
        {
            get
            { 
                return this.ContractImportResultErrors; 
            }
            set
            {
                this.ContractImportResultErrors = value;
            }
        }

        IEnumerable<IError> IResultEntity.ResultErrors
        {
            get
            {
                return this.ContractImportResultErrors;
            }
        }
    }
    public partial class ContractImportResultError : IError
    {
        Guid IError.ParentTransportGUID
        {
            get
            {
                return this.ContractImportTransportGUID;
            }
            set
            {
                this.ContractImportTransportGUID = value;
            }
        }
    }

    public partial class AccountImportRequest
    {
        public AccountImportRequestPercentPremise[] AccountImportRequestPercentPremisesArray
        { 
            get
            {
                return this.AccountImportRequestPercentPremises == null ?
                    null :
                    this.AccountImportRequestPercentPremises.ToArray();
            }
            set
            {
                this.AccountImportRequestPercentPremises = value == null ?
                    null :
                    new HashSet<AccountImportRequestPercentPremise>(value);
            }
        }
        public AccountImportRequestReason[] AccountImportRequestReasonsArray
        {
            get
            {
                return this.AccountImportRequestReasons == null ?
                    null :
                    this.AccountImportRequestReasons.ToArray();
            }
            set
            {
                this.AccountImportRequestReasons = value == null ?
                    null :
                    new HashSet<AccountImportRequestReason>(value);
            }
        }
    }

    public partial class AccountImportResult : IResultEntity<AccountImportResultError>, IResultEntity
    {
        ICollection<AccountImportResultError> IResultEntity<AccountImportResultError>.ResultErrors
        {
            get
            {
                return this.AccountImportResultErrors;
            }
            set
            {
                this.AccountImportResultErrors = value;
            }
        }

        IEnumerable<IError> IResultEntity.ResultErrors
        {
            get
            {
                return this.AccountImportResultErrors;
            }
        }
    }
    public partial class AccountImportResultError : IError
    {
        Guid IError.ParentTransportGUID
        {
            get
            {
                return this.AccountImportTransportGUID;
            }
            set
            {
                this.AccountImportTransportGUID = value;
            }
        }
    }

    public partial class HouseImportResult : IResultEntity<HouseImportResultError>, IResultEntity
    {
        //private IResultEntity GetAllResults()
        //{
        //    var results = new List<IResultEntity>();
        //    results.Add(this);
            
        //    results.AddRange(this.HouseImportResultEntrances);
        //    foreach (var entrance in this.HouseImportResultEntrances)
        //    {
        //        results.AddRange(entrance.HouseImportResultPremises);
        //        foreach (var premise in entrance.HouseImportResultPremises)
        //        {
        //            results.AddRange(premise.HouseImportResultLivingRooms);
        //        }
        //    }

        //    return results;
        //}
        
        ICollection<HouseImportResultError> IResultEntity<HouseImportResultError>.ResultErrors
        {
            get
            {
                return this.HouseImportResultErrors;
            }
            set
            {
                this.HouseImportResultErrors = value;
            }
        }

        IEnumerable<IError> IResultEntity.ResultErrors
        {
            get
            {
                var errors = new List<IError>();
                if (this.HouseImportResultErrors != null)
                {
                    errors.AddRange(this.HouseImportResultErrors);
                }
                if (this.HouseImportResultEntrances != null)
                {
                    errors.AddRange(this.HouseImportResultEntrances.SelectMany(ss => ss.HouseImportResultEntranceErrors));
                }
                if (this.HouseImportResultPremises != null)
                {
                    errors.AddRange(this.HouseImportResultPremises.SelectMany(ss => ss.HouseImportResultPremiseErrors));
                }
                if (this.HouseImportResultBlocks != null)
                {
                    errors.AddRange(this.HouseImportResultBlocks.SelectMany(ss => ss.HouseImportResultBlockErrors));
                }
                if (this.HouseImportResultLivingRooms != null)
                {
                    errors.AddRange(this.HouseImportResultLivingRooms.SelectMany(ss => ss.HouseImportResultLivingRoomErrors));
                }

                return errors;
            }
        }
    }
    public partial class HouseImportResultError : IError
    {
        Guid IError.ParentTransportGUID
        {
            get
            {
                return this.HouseImportTransportGUID;
            }
            set
            {
                this.HouseImportTransportGUID = value;
            }
        }
    }
    public partial class HouseImportResultEntrance : IResultEntity<HouseImportResultEntranceError>
    {
        ICollection<HouseImportResultEntranceError> IResultEntity<HouseImportResultEntranceError>.ResultErrors
        {
            get
            {
                return this.HouseImportResultEntranceErrors;
            }
            set
            {
                this.HouseImportResultEntranceErrors = value;
            }
        }
    }
    public partial class HouseImportResultEntranceError : IError
    {
        Guid IError.ParentTransportGUID
        {
            get
            {
                return this.HouseImportEntranceTransportGUID;
            }
            set
            {
                this.HouseImportEntranceTransportGUID = value;
            }
        }
    }
    public partial class HouseImportResultBlock : IResultEntity<HouseImportResultBlockError>
    {
        ICollection<HouseImportResultBlockError> IResultEntity<HouseImportResultBlockError>.ResultErrors
        {
            get
            {
                return this.HouseImportResultBlockErrors;
            }
            set
            {
                this.HouseImportResultBlockErrors = value;
            }
        }
    }
    public partial class HouseImportResultBlockError : IError
    {
        Guid IError.ParentTransportGUID
        {
            get
            {
                return this.HouseImportBlockTransportGUID;
            }
            set
            {
                this.HouseImportBlockTransportGUID = value;
            }
        }
    }
    public partial class HouseImportResultPremise : IResultEntity<HouseImportResultPremiseError>
    {
        ICollection<HouseImportResultPremiseError> IResultEntity<HouseImportResultPremiseError>.ResultErrors
        {
            get
            {
                return this.HouseImportResultPremiseErrors;
            }
            set
            {
                this.HouseImportResultPremiseErrors = value;
            }
        }
    }
    public partial class HouseImportResultPremiseError : IError
    {
        Guid IError.ParentTransportGUID
        {
            get
            {
                return this.HouseImportPremiseTransportGUID;
            }
            set
            {
                this.HouseImportPremiseTransportGUID = value;
            }
        }
    }
    public partial class HouseImportResultLivingRoom : IResultEntity<HouseImportResultLivingRoomError>
    {
        ICollection<HouseImportResultLivingRoomError> IResultEntity<HouseImportResultLivingRoomError>.ResultErrors
        {
            get
            {
                return this.HouseImportResultLivingRoomErrors;
            }
            set
            {
                this.HouseImportResultLivingRoomErrors = value;
            }
        }
    }
    public partial class HouseImportResultLivingRoomError : IError
    {
        Guid IError.ParentTransportGUID
        {
            get
            {
                return this.HouseImportLivingRoomTransportGUID;
            }
            set
            {
                this.HouseImportLivingRoomTransportGUID = value;
            }
        }
    }

    public partial class DeviceImportResult : IResultEntity<DeviceImportResultError>, IResultEntity
    {
        ICollection<DeviceImportResultError> IResultEntity<DeviceImportResultError>.ResultErrors
        {
            get
            {
                return this.DeviceImportResultErrors;
            }
            set
            {
                this.DeviceImportResultErrors = value;
            }
        }

        IEnumerable<IError> IResultEntity.ResultErrors
        {
            get
            {
                return this.DeviceImportResultErrors;
            }
        }
    }
    public partial class DeviceImportResultError : IError
    {
        Guid IError.ParentTransportGUID
        {
            get
            {
                return this.DeviceImportTransportGUID;
            }
            set
            {
                this.DeviceImportTransportGUID = value;
            }
        }
    }

    public partial class DeviceValueImportResult : IResultEntity<DeviceValueImportResultError>, IResultEntity
    {
        ICollection<DeviceValueImportResultError> IResultEntity<DeviceValueImportResultError>.ResultErrors
        {
            get
            {
                return this.DeviceValueImportResultErrors;
            }
            set
            {
                this.DeviceValueImportResultErrors = value;
            }
        }

        IEnumerable<IError> IResultEntity.ResultErrors
        {
            get
            {
                return this.DeviceValueImportResultErrors;
            }
        }
    }
    public partial class DeviceValueImportResultError : IError
    {
        Guid IError.ParentTransportGUID
        {
            get
            {
                return this.DeviceValueImportTransportGUID;
            }
            set
            {
                this.DeviceValueImportTransportGUID = value;
            }
        }
    }

    public partial class OrderImportResult : IResultEntity<OrderImportResultError>, IResultEntity
    {
        ICollection<OrderImportResultError> IResultEntity<OrderImportResultError>.ResultErrors
        {
            get
            {
                return this.OrderImportResultErrors;
            }
            set
            {
                this.OrderImportResultErrors = value;
            }
        }

        IEnumerable<IError> IResultEntity.ResultErrors
        {
            get
            {
                return this.OrderImportResultErrors;
            }
        }
    }
    public partial class OrderImportResultError : IError
    {
        Guid IError.ParentTransportGUID
        {
            get
            {
                return this.OrderImportTransportGUID;
            }
            set
            {
                this.OrderImportTransportGUID = value;
            }
        }
    }

    public partial class AckImportResult : IResultEntity<AckImportResultError>, IResultEntity
    {
        ICollection<AckImportResultError> IResultEntity<AckImportResultError>.ResultErrors
        {
            get
            {
                return this.AckImportResultErrors;
            }
            set
            {
                this.AckImportResultErrors = value;
            }
        }

        IEnumerable<IError> IResultEntity.ResultErrors
        {
            get
            {
                return this.AckImportResultErrors;
            }
        }
    }
    public partial class AckImportResultError : IError
    {
        Guid IError.ParentTransportGUID
        {
            get
            {
                return this.AckImportTransportGUID;
            }
            set
            {
                this.AckImportTransportGUID = value;
            }
        }
    }

    public partial class SettlementImportResult : IResultEntity<SettlementImportResultError>, IResultEntity
    {
        ICollection<SettlementImportResultError> IResultEntity<SettlementImportResultError>.ResultErrors
        {
            get
            {
                return this.SettlementImportResultErrors;
            }
            set
            {
                this.SettlementImportResultErrors = value;
            }
        }

        IEnumerable<IError> IResultEntity.ResultErrors
        {
            get
            {
                return this.SettlementImportResultErrors;
            }
        }
    }
    public partial class SettlementImportResultError : IError
    {
        Guid IError.ParentTransportGUID
        {
            get
            {
                return this.SettlementImportTransportGUID;
            }
            set
            {
                this.SettlementImportTransportGUID = value;
            }
        }
    }

    public partial class PaymentImportResult : IResultEntity<PaymentImportResultError>, IResultEntity
    {
        ICollection<PaymentImportResultError> IResultEntity<PaymentImportResultError>.ResultErrors
        {
            get
            {
                return this.PaymentImportResultErrors;
            }
            set
            {
                this.PaymentImportResultErrors = value;
            }
        }

        IEnumerable<IError> IResultEntity.ResultErrors
        {
            get
            {
                return this.PaymentImportResultErrors;
            }
        }
    }
    public partial class PaymentImportResultError : IError
    {
        Guid IError.ParentTransportGUID
        {
            get
            {
                return this.PaymentImportTransportGUID;
            }
            set
            {
                this.PaymentImportTransportGUID = value;
            }
        }
    }

    public partial class NotificationImportResult : IResultEntity<NotificationImportResultError>, IResultEntity
    {
        ICollection<NotificationImportResultError> IResultEntity<NotificationImportResultError>.ResultErrors
        {
            get
            {
                return this.NotificationImportResultErrors;
            }
            set
            {
                this.NotificationImportResultErrors = value;
            }
        }

        IEnumerable<IError> IResultEntity.ResultErrors
        {
            get
            {
                return this.NotificationImportResultErrors;
            }
        }
    }
    public partial class NotificationImportResultError : IError
    {
        Guid IError.ParentTransportGUID
        {
            get
            {
                return this.NotificationImportTransportGUID;
            }
            set
            {
                this.NotificationImportTransportGUID = value;
            }
        }
    }

    //public partial class NsiExportResult : System.Xml.Serialization.IXmlSerializable
    //{
    //    System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
    //    {
    //        return null;
    //    }
    //    void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
    //    {
    //        foreach (var prop in this.GetType()
    //            .GetProperties()
    //            .Where(p => p.PropertyType.IsValueType))
    //        {
    //            object value = prop.GetValue(this);
    //            if (value != null)
    //            {
    //                writer.WriteAttributeString(prop.Name, value.ToString());
    //            }
    //        }
    //        if (this.NsiExportResultFields != null && this.NsiExportResultFields.Any())
    //        {
    //            writer.WriteStartElement("NsiExportResultFields");
    //            foreach (var nsiExportResultField in this.NsiExportResultFields)
    //            {
    //                writer.WriteStartElement("NsiExportResultField");
    //                ((System.Xml.Serialization.IXmlSerializable)nsiExportResultField).WriteXml(writer);
    //                writer.WriteEndElement();
    //            }
    //            writer.WriteEndElement();
    //        }
    //    }
    //}
    //public partial class NsiExportResultField : System.Xml.Serialization.IXmlSerializable
    //{
    //    System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
    //    {
    //        return null;
    //    }
    //    void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
    //    {
    //        foreach (var prop in this.GetType()
    //            .GetProperties()
    //            .Where(p => p.PropertyType.IsValueType))
    //        {
    //            object value = prop.GetValue(this);
    //            if (value != null)
    //            {
    //                writer.WriteAttributeString(prop.Name, value.ToString());
    //            }
    //        }
    //    }
    //}

    public enum OrganizationType : byte
    {
        [Description("Физическое лицо")]
        Individual = 1,
        [Description("Частный предприниматель")]
        Entp = 2,
        [Description("Юридическое лицо")]
        Legal = 3,
    }

    public enum DeviceType : byte
    {
        [Description("Индивидуальный ПУ")]
        Individual = 1,
        [Description("Коллективный (общедомовой) ПУ")]
        Collective = 2,
        [Description("Общий (квартирный) ПУ")]
        CollectiveApartment = 3,
        [Description("Комнатный")]
        LivingRoom = 4,
    }

    public enum IndividualDeviceType : byte
    {
        [Description("Индивидуальный ПУ в жилом доме")]
        ApartmentHouse = 1,
        [Description("Индивидуальный ПУ в нежилом помещении")]
        NonResidentialPremise = 2,
        [Description("Индивидуальный ПУ в жилом помещении")]
        ResidentialPremise = 3,
    }

    public enum DeviceValueType : byte
    {
        [Description("Базовое показание")]
        Base = 1,
        [Description("Текущее показание")]
        Current = 2,
        [Description("Контрольное показание")]
        Control = 3,
    }

    public enum HouseType : byte
    {
        [Description("МКД")]
        ApartmentHouse = 0,

        [Description("Жилой дом")]
        LivingHouse = 1,

        [Description("Дом блокированной застройки")]
        BlockHouse = 2,
    }

    public enum PremisesType : byte
    {
        [Description("Нежилое")]
        NonResidential = 0,

        [Description("Жилое")]
        Residential = 1,
    }

    public enum ServiceError
    {
        None,
        Unknown,

        INT002000,
        // Нет объектов для экспорта
        INT002012,
    }

    public static class Extensions
    {
        public static E CreateError<E>(this ITransactionObjectEntity source, string code, string description)
            where E : IError, new()
        {
            return new E
            {
                objectId = source.objectId,
                TransportGUID = Guid.NewGuid(),
                ParentTransportGUID = source.TransportGUID,
                ErrorCode = code,
                ErrorDescription = description,
            };
        }
        //public static E CreateError<E>(this ITransactionObjectEntity source, string description)
        //    where E : IError, new()
        //{
        //    return source.CreateError<E>(PerformServiceError.HCS_DAT_00001.ToString(), description);
        //}
        //public static E CreateError<E>(this ITransactionObjectEntity source, CommonException exception)
        //    where E : IError, new()
        //{
        //    return source.CreateError<E>(exception.Code, exception.Message);
        //}
        
        public static T AddError<T, E>(this T result, E error)
            where T : IResultEntity<E>, new()
            where E : IError, new()
        {
            if (result.ResultErrors == null)
            {
                result.ResultErrors = new List<E>();
            }

            result.ResultErrors.Add(error);

            return result;
        }
        public static T AddError<T, E>(this T result, string code, string description)
            where T : IResultEntity<E>, new()
            where E : IError, new()
        {
            var error = result.CreateError<E>(code, description);
            return result.AddError(error);
        }
        //public static T AddError<T, E>(this T result, string description)
        //    where T : IResultEntity<E>, new()
        //    where E : IError, new()
        //{
        //    var error = result.CreateError<E>(description);
        //    return result.AddError(error);
        //}

        public static T CreateResult<T, E>(this ITransactionObjectEntity source)
            where T : IResultEntity<E>, new()
            where E : IError, new()
        {
            return new T
            {
                objectId = source.objectId,
                TransportGUID = source.TransportGUID,
            };
        }

        public static T CreateErrorResult<T, E>(this E error)
            where T : IResultEntity<E>, new()
            where E : IError, new()
        {
            return new T
            {
                objectId = error.objectId,
                TransportGUID = error.ParentTransportGUID,
                ResultErrors = new List<E> { error },
            };
        }
        //public static T CreateErrorResult<T, E>(this ITransactionObjectEntity source, string description)
        //    where T : IResultEntity<E>, new()
        //    where E : IError, new()
        //{
        //    return source.CreateError<E>(description).CreateErrorResult<T, E>();
        //}
        public static IEnumerable<T> CreateErrorResult<T, E>(this IEnumerable<E> errors)
            where T : IResultEntity<E>, new()
            where E : IError, new()
        {
            var results = errors.GroupBy(e => new { e.ParentTransportGUID, e.objectId }).Select(g => new T
            {
                objectId = g.Key.objectId,
                TransportGUID = g.Key.ParentTransportGUID,
                ResultErrors = g.ToList(),
            });
            return results;
        }

        public static decimal Round(this decimal source, byte precision, byte scale)
        {
            return Decimal.Round(source, scale);
        }
        public static Nullable<decimal> Round(this Nullable<decimal> source, byte precision, byte scale)
        {
            if (source.HasValue)
            {
                return Extensions.Round(source.Value, precision, scale);
            }
            return source;
        }
    }
}