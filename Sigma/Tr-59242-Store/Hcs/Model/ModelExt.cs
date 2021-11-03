﻿using System;
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

    //[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    //public class NullableAttribute : Attribute
    //{
    //    public NullableAttribute()
    //    {
    //    }
    //}

    //[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    //public class StringLengthAttribute : Attribute
    //{
    //    public short Length { get; private set; }

    //    public StringLengthAttribute(short length)
    //    {
    //        this.Length = length;
    //    }
    //}

    #region interfaces
    public partial class AccountExportRequest : ITransactionEntity
    {
    }
    public partial class AccountExportResult : ITransactionEntity
    {
    }
    public partial class AccountExportResultPercentPremise : ITransactionEntity
    {
    }

    public partial class AccountImportRequest : ITransactionObjectEntity
    {
    }
    public partial class AccountImportRequestPayer : ITransactionObjectEntity
    {
    }
    public partial class AccountImportRequestPercentPremise : ITransactionObjectEntity
    {
    }
    public partial class AccountImportRequestReason : ITransactionObjectEntity
    {
    }
    public partial class AccountImportResult : IResultEntity<AccountImportResultError>, IResultEntity, ITransactionObjectEntity
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
    public partial class AccountImportResultError : IError, ITransactionObjectEntity
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

    public partial class AckImportCancellationRequest : ITransactionObjectEntity
    {
    }
    public partial class AckImportRequest : ITransactionObjectEntity
    {
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

    public partial class AttachmentPostRequest : IAttachment, ITransactionObjectEntity
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
    public partial class AttachmentPostResult : ITransactionObjectEntity
    {
    }
    public partial class AttachmentPostResultCopy : ITransactionObjectEntity
    {
    }

    public partial class ContractImportRequest : ITransactionObjectEntity
    {
    }
    public partial class ContractImportRequestAttachment : ITransactionObjectEntity
    {
    }
    public partial class ContractImportRequestObjectAddress : ITransactionObjectEntity
    {
    }
    public partial class ContractImportRequestObjectServiceResource : ITransactionObjectEntity
    {
    }
    public partial class ContractImportRequestParty : ITransactionObjectEntity
    {
    }
    public partial class ContractImportRequestSubject : ITransactionObjectEntity
    {
    }
    public partial class ContractImportRequestSubjectQualityIndicator : ITransactionObjectEntity
    {
    }
    public partial class ContractImportResult : IResultEntity<ContractImportResultError>, IResultEntity, ITransactionObjectEntity
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
    public partial class ContractImportResultError : IError, ITransactionObjectEntity
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

    public partial class DeviceExportRequest : ITransactionEntity
    {
    }
    public partial class DeviceExportResult : ITransactionEntity
    {
    }
    public partial class DeviceExportResultAccount : ITransactionEntity
    {
    }

    public partial class DeviceImportArchiveRequest : ITransactionObjectEntity
    {
    }
    public partial class DeviceImportReplaceRequest : ITransactionObjectEntity
    {
    }
    public partial class DeviceImportReplaceRequestValue : ITransactionObjectEntity
    {
    }
    public partial class DeviceImportRequest : ITransactionObjectEntity
    {
        //public bool DeviceValuesHasSended { get; set; } //todo: Реализовать выборку инофрмации о том, были ли отправлены данные по показания в ГИС.
    }
    public partial class DeviceImportRequestAccount : ITransactionObjectEntity
    {
    }
    public partial class DeviceImportRequestAddress : ITransactionObjectEntity
    {
    }
    public partial class DeviceImportRequestLinkedDevice : ITransactionObjectEntity
    {
    }
    public partial class DeviceImportRequestValue : ITransactionObjectEntity
    {
    }
    public partial class DeviceImportResult : IResultEntity<DeviceImportResultError>, IResultEntity, ITransactionObjectEntity
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
    public partial class DeviceImportResultError : IError, ITransactionObjectEntity
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

    public partial class DeviceValueExportRequest : ITransactionEntity
    {
    }
    public partial class DeviceValueExportRequestDevice : ITransactionEntity
    {
    }
    public partial class DeviceValueExportRequestDeviceType : ITransactionEntity
    {
    }
    public partial class DeviceValueExportRequestMunicipalResource : ITransactionEntity
    {
    }
    public partial class DeviceValueExportResult : ITransactionEntity
    {
    }

    public partial class DeviceValueImportRequest : ITransactionObjectEntity
    {
    }
    public partial class DeviceValueImportResult : IResultEntity<DeviceValueImportResultError>, IResultEntity, ITransactionObjectEntity
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
    public partial class DeviceValueImportResultError : IError, ITransactionObjectEntity
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

    public partial class HouseExportRequest : ITransactionEntity
    {
    }
    public partial class HouseExportResult : ITransactionEntity
    {
    }
    public partial class HouseExportResultBlock : ITransactionEntity
    {
    }
    public partial class HouseExportResultEntrance : ITransactionEntity
    {
    }
    public partial class HouseExportResultLivingRoom : ITransactionEntity
    {
    }
    public partial class HouseExportResultPremise : ITransactionEntity
    {
    }

    public partial class HouseImportRequest : IGKN_EGRP, ITransactionObjectEntity
    {
    }
    public partial class HouseImportRequestEntrance : IHouseAnnulment, ITransactionObjectEntity
    {
    }
    public partial class HouseImportRequestBlock : IGKN_EGRP, IHouseAnnulment, ITransactionObjectEntity
    {
    }
    public partial class HouseImportRequestPremise : IGKN_EGRP, IHouseAnnulment, ITransactionObjectEntity
    {
    }
    public partial class HouseImportRequestLivingRoom : IGKN_EGRP, IHouseAnnulment, ITransactionObjectEntity
    {
    }
    public partial class HouseImportResult : IResultEntity<HouseImportResultError>, IResultEntity, ITransactionObjectEntity
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
    public partial class HouseImportResultError : IError, ITransactionObjectEntity
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
    public partial class HouseImportResultEntrance : IResultEntity<HouseImportResultEntranceError>, ITransactionObjectEntity
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
    public partial class HouseImportResultEntranceError : IError, ITransactionObjectEntity
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
    public partial class HouseImportResultBlock : IResultEntity<HouseImportResultBlockError>, ITransactionObjectEntity
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
    public partial class HouseImportResultBlockError : IError, ITransactionObjectEntity
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
    public partial class HouseImportResultPremise : IResultEntity<HouseImportResultPremiseError>, ITransactionObjectEntity
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
    public partial class HouseImportResultPremiseError : IError, ITransactionObjectEntity
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
    public partial class HouseImportResultLivingRoom : IResultEntity<HouseImportResultLivingRoomError>, ITransactionObjectEntity
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
    public partial class HouseImportResultLivingRoomError : IError, ITransactionObjectEntity
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

    public partial class NotificationImportDeleteRequest : ITransactionObjectEntity
    {
    }
    public partial class NotificationImportRequest : ITransactionObjectEntity
    {
    }
    public partial class NotificationImportRequestAccountDebt : ITransactionObjectEntity
    {
    }
    public partial class NotificationImportResult : ITransactionObjectEntity
    {
    }
    public partial class NotificationImportResultError : ITransactionObjectEntity
    {
    }

    public partial class NsiExportRequest : ITransactionEntity
    {
    }
    public partial class NsiExportResult : ITransactionEntity
    {
    }
    public partial class NsiExportResultField : ITransactionEntity
    {
    }

    public partial class OrderImportCancellationRequest : ITransactionObjectEntity
    {
    }
    public partial class OrderImportRequest : ITransactionObjectEntity
    {
    }
    public partial class OrderImportResult : IResultEntity<OrderImportResultError>, IResultEntity, ITransactionObjectEntity
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
    public partial class OrderImportResultError : IError, ITransactionObjectEntity
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

    public partial class OrganizationExportRequest : ITransactionEntity
    {
    }
    public partial class OrganizationExportRequestData : ITransactionEntity
    {
    }
    public partial class OrganizationExportResult : ITransactionEntity
    {
    }
    public partial class OrganizationExportResultEntp : ITransactionEntity
    {
    }
    public partial class OrganizationExportResultLegal : ITransactionEntity
    {
    }
    public partial class OrganizationExportResultRole : ITransactionEntity
    {
    }

    public partial class PaymentExportRequest : ITransactionEntity
    {
    }
    public partial class PaymentExportRequestAccount : ITransactionEntity
    {
    }
    public partial class PaymentExportRequestDocument : ITransactionEntity
    {
    }
    public partial class PaymentExportResult : ITransactionEntity
    {
    }

    public partial class PaymentImportRequest : ITransactionObjectEntity
    {
    }
    public partial class PaymentImportRequestChargesMunicipalService : ITransactionObjectEntity
    {
    }
    public partial class PaymentImportRequestChargesMunicipalServiceNorm : ITransactionObjectEntity
    {
    }
    public partial class PaymentImportRequestDebtMunicipalService : ITransactionObjectEntity
    {
    }
    public partial class PaymentImportRequestPenaltyAndCourtCost : ITransactionObjectEntity
    {
    }
    public partial class PaymentImportWithdrawRequest : ITransactionObjectEntity
    {
    }
    public partial class PaymentImportResult : IResultEntity<PaymentImportResultError>, IResultEntity, ITransactionObjectEntity
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
    public partial class PaymentImportResultError : IError, ITransactionObjectEntity
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

    public partial class SettlementImportAnnulmentRequest : ITransactionObjectEntity
    {
    }
    public partial class SettlementImportRequest : ITransactionObjectEntity
    {
    }
    public partial class SettlementImportRequestPeriod : ITransactionObjectEntity
    {
    }
    public partial class SettlementImportRequestPeriodAnnulment : ITransactionObjectEntity
    {
    }
    public partial class SettlementImportRequestPeriodInfo : ITransactionObjectEntity
    {
    }
    public partial class SettlementImportResult : IResultEntity<SettlementImportResultError>, IResultEntity, ITransactionObjectEntity
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
    public partial class SettlementImportResultError : IError, ITransactionObjectEntity
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

    //public partial class NotificationImportResult : IResultEntity<NotificationImportResultError>, IResultEntity
    //{
    //    ICollection<NotificationImportResultError> IResultEntity<NotificationImportResultError>.ResultErrors
    //    {
    //        get
    //        {
    //            return this.NotificationImportResultErrors;
    //        }
    //        set
    //        {
    //            this.NotificationImportResultErrors = value;
    //        }
    //    }

    //    IEnumerable<IError> IResultEntity.ResultErrors
    //    {
    //        get
    //        {
    //            return this.NotificationImportResultErrors;
    //        }
    //    }
    //}
    //public partial class NotificationImportResultError : IError
    //{
    //    Guid IError.ParentTransportGUID
    //    {
    //        get
    //        {
    //            return this.NotificationImportTransportGUID;
    //        }
    //        set
    //        {
    //            this.NotificationImportTransportGUID = value;
    //        }
    //    }
    //}

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

    #endregion

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
        public static E CreateError<E>(this ITransactionObjectEntity source, string description)
            where E : IError, new()
        {
            return source.CreateError<E>(HcsErrorCode.HCS_DAT_00001.ToString(), description);
        }
        public static E CreateError<E>(this ITransactionObjectEntity source, CommonException exception)
            where E : IError, new()
        {
            return source.CreateError<E>(exception.Code, exception.Message);
        }

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
        public static T AddError<T, E>(this T result, string description)
            where T : IResultEntity<E>, new()
            where E : IError, new()
        {
            var error = result.CreateError<E>(description);
            return result.AddError(error);
        }

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