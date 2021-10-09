using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Hcs
{
    public interface ILoggable
    {
        Action<string, string> Log { get; set; }
    }

    public interface IDataStore2 : IDisposable
    {
        Task<Guid> CreateTransactionAsync(TransactionInfo transaction);
        Task<Guid> CreateTransactionAsync(Guid transactionGuid, TransactionInfo transaction);
        Task<bool> IsTransactionExistsAsync(Guid transactionGuid);

        Task SetTransactionResultAsync(Guid transactionGuid, TransactionResultInfo result);
        Task<TransactionResultInfo> GetTransactionResultAsync(Guid transactionGuid);

        Task SetTransactionStateAsync(Guid transactionGuid, TransactionStateInfo transactionState);
        Task<TransactionStateInfo> GetTransactionStateAsync(Guid transactionGuid);
        Task<TransactionStateInfo> GetTransactionStateAsync(Guid transactionGuid, SysOperationCode operation, TransactionState State);
        Task<IEnumerable<TransactionStateInfo>> GetTransactionStatesAsync(Guid transactionGuid);
        Task<IEnumerable<TransactionStateInfo>> GetTransactionStatesAsync(Guid transactionGuid, SysOperationCode operation, TransactionState State);

        Task SetTransactionLogAsync(Guid transactionGuid, TransactionLogInfo error);

        Task SetTransactionObjectsAsync(Guid transactionGuid, IEnumerable<ObjectInfo> objectList);
        Task SetTransactionObjectErrorsAsync(Guid transactionGuid, IEnumerable<ObjectInfoError> objectErrorList);
        Task<IEnumerable<ObjectInfo>> GetTransactionObjectsAsync(Guid transactionGuid);

        Task<IEnumerable<T>> ReadDataAsync<T>(Guid stateGuid)
            where T : class; //, ITransactionEntity;
        Task SaveDataAsync<T>(IEnumerable<T> data, Guid stateGuid)
            where T : class; //, ITransactionEntity;
    }

    public interface IDataSource2 : IDisposable
    {
        Task<IEnumerable<T>> TakeDataAsync<T>(Guid transactionGuid, IEnumerable<ObjectInfo> objectList)
            where T : class; //, ITransactionEntity;
        Task PassDataAsync<T>(IEnumerable<T> data, Guid transactionGuid, IEnumerable<ObjectInfo> objectList)
            where T : class; //, ITransactionEntity;
        // note: :(
        Task<IEnumerable<ObjectInfo>> ListAsync(SysOperationCode operationCode);
    }

    public interface IDataService2
    {
        Task<AsyncState<TRequestData, TResultData>> ImportAsync<TRequestData, TResultData>(IEnumerable<TRequestData> data, Guid transactionGuid, Action<string, string> log);
        Task<IEnumerable<TResultData>> ImportStateAsync<TRequestData, TResultData>(AsyncImportState<TRequestData> asyncState, Action<string, string> log);

        Task<Guid> ExportAsync<TRequestData, TResultData>(TRequestData data, Guid transactionGuid, Action<string, string> log);
        //Task<IEnumerable<TResultData>> ExportStateAsync<TRequestData, TResultData>(Guid asyncState, Action<string, string> log);
        // из-за справочников
        Task<IEnumerable<TResultData>> ExportStateAsync<TRequestData, TResultData>(AsyncExportState<TRequestData> asyncState, Action<string, string> log);
    }

    //public interface IFileService
    //{
    //    void PostAttachment(IAttachment attachment);
    //}

    public class ExternalParam
    {
        public long ObjectId { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set; }
    }

    public class FileParam
    {
        public int PartCount { get; set; }
        public int PartNumber { get; set; }
    }

    public class ObjectInfo
    {
        public string ObjectId { get; set; }
        public string Comment { get; set; }
        public int Operation { get; set; }
        public int Group { get; set; }
        public string Result { get; set; }

        public string Param { get; set; }

        public ObjectInfoError[] Errors { get; set; }
    }

    public class ErrorInfo
    {
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string ErrorStackTrace { get; set; }

        public override string ToString()
        {
            return $"{ErrorCode}: {ErrorDescription}\n{ErrorStackTrace}";
        }

        public static ErrorInfo Create(string errorCode, string errorDescription = null, string errorStackTrace = null)
        {
            return new ErrorInfo
            {
                ErrorCode = errorCode,
                ErrorDescription = errorDescription,
                ErrorStackTrace = errorStackTrace,
            };
        }
    }

    public class ObjectInfoError : ErrorInfo
    {
        public string ObjectId { get; set; }
    }

    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly TKey key;
        private readonly IEnumerable<TElement> values;

        public TKey Key
        {
            get
            {
                return key;
            }
        }

        public Grouping(TKey key, IEnumerable<TElement> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            this.key = key;
            this.values = values;
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class AsyncState<TRequestData, TResultData>
    {
        public AsyncImportState<TRequestData> RequestState { get; set; }
        public IEnumerable<TResultData> ResultData { get; set; }
    }
    public class AsyncImportState<TRequestData>
    {
        public Guid State { get; set; }
        public IEnumerable<TRequestData> Data { get; set; }
    }
    public class AsyncExportState<TRequestData>
    {
        public Guid State { get; set; }
        public TRequestData Data { get; set; }
    }

    public class TransactionInfo
    {
        public SysOperationCode OperationCode { get; private set; }
        public string Data { get; private set; }

        public static TransactionInfo Create(SysOperationCode operationCode)
        {
            return new TransactionInfo
            {
                OperationCode = operationCode,
            };
        }
        public static TransactionInfo Create(SysOperationCode operationCode, string data)
        {
            return new TransactionInfo
            {
                OperationCode = operationCode,
                Data = data,
            };
        }
    }

    public enum TransactionResult : byte
    {
        None = 0,
        Success = 1,
        TransactionError = 2,
        ObjectError = 3,
    }

    public class TransactionResultInfo
    {
        // todo: вычислять Result, IsCompleted
        public TransactionResult Result { get; private set; }
        public ErrorInfo Error { get; private set; }
        public IEnumerable<ObjectInfo> Objects { get; private set; }
        public bool IsCompleted { get; private set; }

        public static TransactionResultInfo None()
        {
            return new TransactionResultInfo
            {
                Result = TransactionResult.None,
                IsCompleted = false,
            };
        }
        public static TransactionResultInfo TransactionError(ErrorInfo error, bool isCompleted)
        {
            return new TransactionResultInfo
            {
                Result = TransactionResult.TransactionError,
                Error = error,
                IsCompleted = isCompleted,
            };
        }
        public static TransactionResultInfo Create(IEnumerable<ObjectInfo> objects)
        {
            if (objects == null || !objects.Any())
            {
                throw new ArgumentException(nameof(objects));
            }

            TransactionResult result = objects.Any(ss => ss.Errors != null && ss.Errors.Length > 0) ?
                TransactionResult.ObjectError :
                TransactionResult.Success;
            return new TransactionResultInfo
            {
                Result = result,
                Objects = objects,
                IsCompleted = true,
            };
        }
    }

    public enum TransactionState : byte
    {
        None = 0,
        Composed = 1,
        Sent = 2,
        Recieved = 3,
        Transferred = 4,
    }

    public class TransactionStateInfo
    {
        public Guid StateGUID { get; set; }
        public Guid? AsyncStateGUID { get; set; }
        public SysOperationCode OperationCode { get; set; }
        public TransactionState State { get; set; }
        public string Data { get; set; }

        public static TransactionStateInfo Create(SysOperationCode operationCode, TransactionState state, string data, Guid stateGUID, Guid? asyncStateGUID = null)
        {
            return new TransactionStateInfo
            {
                OperationCode = operationCode,
                State = state,
                Data = data,
                StateGUID = stateGUID,
                AsyncStateGUID = asyncStateGUID,
            };
        }
    }

    public class TransactionLogInfo
    {
        public string Data { get; set; }
        public ErrorInfo Error { get; set; }

        public static TransactionLogInfo Create(string data = null, ErrorInfo error = null)
        {
            return new TransactionLogInfo
            {
                Data = data,
                Error = error,
            };
        }
    }

    public static class Helper
    {
        public static Guid? ToNullableGuid(this string source)
        {
            if (source == null)
            {
                return null;
            }

            return source.ToGuid();
        }
        public static Guid ToGuid(this string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return Guid.Parse(source);
        }
    }

    public enum SysOperationCode
    {
        NsiExport = 2,

        AttachmentPost = 5,

        OrganizationExport = 12,

        ContractImport = 21,
        ContractExport = 22,

        HouseImport = 31,
        HouseExport = 32,
        HouseAnnul = 33,

        AccountImport = 41,
        AccountExport = 42,
        AccountClose = 43,

        SettlementImport = 51,

        PaymentDocumentImport = 61,
        PaymentDocumentExport = 62,

        DeviceImport = 71,
        DeviceExport = 72,

        DeviceValueImport = 81,
        DeviceValueExport = 82,

        OrderImport = 101,

        AckImport = 111,

        NotificationImport = 121,
    }

    [Flags]
    public enum LogMode
    {
        None = 0,
        Request = 1,
        Response = 2,
        Trace = 4,
    }

    public static class AsyncHelper
    {
        private readonly static TaskFactory _myTaskFactory;

        static AsyncHelper()
        {
            AsyncHelper._myTaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);
        }

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            TaskAwaiter<TResult> awaiter = AsyncHelper._myTaskFactory.StartNew<Task<TResult>>(() =>
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
                Thread.CurrentThread.CurrentUICulture = currentUICulture;
                return func();
            }).Unwrap<TResult>().GetAwaiter();
            return awaiter.GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            TaskAwaiter awaiter = AsyncHelper._myTaskFactory.StartNew<Task>(() =>
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
                Thread.CurrentThread.CurrentUICulture = currentUICulture;
                return func();
            }).Unwrap().GetAwaiter();
            awaiter.GetResult();
        }
    }

    //public static class EnumHelper
    //{
    //    public static string GetDescription(this Enum enumElement)
    //    {
    //        var field = enumElement.GetType().GetField(enumElement.ToString());
    //        var attribute = field.GetCustomAttribute<DescriptionAttribute>();
    //        return attribute != null ? attribute.Description : enumElement.ToString();
    //    }
    //}

    //public static class JsonHelper
    //{
    //    private static JsonSerializerSettings entityJsonSettings = new JsonSerializerSettings
    //    {
    //        ObjectCreationHandling = ObjectCreationHandling.Auto,
    //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    //        PreserveReferencesHandling = PreserveReferencesHandling.None,
    //    };
    //    //JsonConverter jsonConverter = new XElementJsonConverter();

    //    public static string Serialize<T>(T item)
    //    {
    //        var str = JsonConvert.SerializeObject(item, entityJsonSettings);
    //        return str;
    //    }
    //    public static T Deserialize<T>(string jsonString)
    //    {
    //        T obj = JsonConvert.DeserializeObject<T>(jsonString, entityJsonSettings);
    //        return obj;
    //    }
    //}
}
