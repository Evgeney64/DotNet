using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Tsb.WCF.Web.SystemSpace.SysTables;

namespace Tsb.Extensions.Algotithm
{
    #region enums
    [Flags]
    public enum AlgConfigDisplay : byte
    {
        None = 0,
        Algorithm = 1,
        Task = 2,

        All = 255,
    }
    #endregion

    #region Attributes
    [AttributeUsage(AttributeTargets.Property)]
    public class AlgConfigAttribute : Attribute
    {
        #region
        public string Config { get; private set; }
        public int Order { get; private set; }
        public AlgConfigDisplay Display { get; private set; }
        public int Group { get; private set; }

        #region old
        // cases "SoperationId_StableId"
        // "0_StableId" - для всех операций данного объекта
        // "SoperationId_0" - для всех объектов данной операции

        // Разрешенные случаи "SoperationId_StableId_NalgorithmId"
        //public List<string> PermitCases { get; private set; }

        // Запрещенные случаи "-SoperationId_StableId_NalgorithmId"
        //public List<string> ForbiddenCases { get; private set; }
        #endregion

        public AlgConfigAttribute(
            string config,
            int order = 0,
            AlgConfigDisplay display = AlgConfigDisplay.All,
            //string[] cases = null,
            int group = 0
            )
        {
            this.Config = config;
            this.Order = order;
            this.Display = display;
            this.Group = group;

            #region old
            //if (cases != null)
            //{
            //    PermitCases = new List<string>();
            //    ForbiddenCases = new List<string>();
            //    foreach (string one_case in cases)
            //    {
            //        if (one_case.Substring(0, 1) == "-")
            //            ForbiddenCases.Add(one_case.Substring(1, one_case.Length - 1));
            //        else
            //            PermitCases.Add(one_case);
            //    }
            //}
            #endregion
        }

        #region old
        //public AlgConfigAttribute(string config
        //    , AlgConfigDisplay display
        //    , int order = 9
        //    , string[] cases = null
        //    )
        //{
        //    this.Config = config;
        //    this.Display = display;
        //    this.Order = order;

        //    if (cases != null)
        //    {
        //        PermitCases = new List<string>();
        //        ForbiddenCases = new List<string>();
        //        foreach (string str in cases)
        //        {
        //            if (str.Substring(0, 1) == "-")
        //                ForbiddenCases.Add(str.Substring(1, str.Length - 1));
        //            else
        //                PermitCases.Add(str);
        //        }
        //    }
        //}
        //public AlgConfigAttribute(string config, string[] forbiddenCases, int order = 0)
        //{
        //    this.Config = config;
        //    this.Display = AlgConfigDisplay.All;
        //    this.ForbiddenCases = forbiddenCases;
        //    this.Order = order;
        //}

        //public AlgConfigAttribute(string config, params int[] operations) :
        //    this(config, int.MaxValue, AlgConfigDisplay.All, operations)
        //{
        //}
        //public AlgConfigAttribute(string config, int order) :
        //    this(config, order, AlgConfigDisplay.All)
        //{
        //}
        //public AlgConfigAttribute(string config, AlgConfigDisplay display, params int[] operations) :
        //    this(config, int.MaxValue, display, operations)
        //{
        //}
        #endregion
        #endregion
    }
    #endregion

    #region TaskContext / VmBaseContext
    public sealed class TaskContext
    {
        public long TaskId { get; internal set; }
        public long TaskExecutionId { get; internal set; }
        internal Action<string, string> LogHandler { get; set; }

        public void Log(string header, string message)
        {
            if (this.LogHandler != null)
            {
                this.LogHandler(header, message);
            }
        }
    }

    public sealed class VmBaseContext
    {
        public string uri_paths_str { get; internal set; }
        public string header_paths_str { get; internal set; }
        public string guid_last_str { get; internal set; }
    }

    public enum ExecutionContextType
    {
        Unknown = 0,
        VmBase,
        Task,
    }
    #endregion

    #region AlgParam / AlgItemParam
    public class AlgParam 
    {
        [XmlIgnore]
        public string ErrorMessage { get; set; }
    }
    public class ListAlgParam : AlgParam
    {
        [XmlIgnore]
        public IEnumerable<long> Ids { get; set; }
        
        [XmlIgnore]
        public virtual int STableId { get; set; }

        public virtual string Filter { get; set; }
    }

    public class AlgItemParam 
    { 
    }
    public class ListAlgItemParam : AlgItemParam
    {
        public int STableId { get; set; }
        public long Id { get; set; }
        public string Comment { get; set; }
    }
    #endregion

    #region interfaces
    public interface IAlgHandlerWithConfig
    {
        string Config { get; set; }
    }

    public interface IAlgParamWithConfig
    {
        int ConfigId { get; }
    }

    public interface IAlgContext
    {
        object Context { get; set; }
    }

    internal interface IAlgHandler : IAlgContext
    {
        IEnumerable<AlgItemParam> CreateList(AlgParam param);
        ServiceResult BeforeRun(AlgParam param);
        ServiceResult AfterRun(AlgParam param);
        ServiceResult RunItem(AlgParam param, AlgItemParam itemParam);
    }

    internal interface IAlgHandlerAsync : IAlgContext
    {
        Task<IEnumerable<AlgItemParam>> CreateListAsync(AlgParam param, Action<string, string> log);
        Task<ServiceResult> BeforeRunAsync(AlgParam param, Action<string, string> log);
        Task<ServiceResult> AfterRunAsync(AlgParam param, Action<string, string> log);
        Task<ServiceResult> RunItemAsync(AlgParam param, AlgItemParam itemParam, Action<string, string> log);
    }
    #endregion

    public abstract class AlgHandler<T> : AlgHandler<T, AlgItemParam>
        where T : AlgParam
    {
    }
    public abstract class AlgHandler<T, D> : IAlgHandler
        where T : AlgParam
        where D : AlgItemParam, new()
    {
        #region Defune
        public object Context { get; private set; }
        public ExecutionContextType ContextType
        {
            get
            {
                if (this.Context is TaskContext)
                {
                    return ExecutionContextType.Task;
                }
                if (this.Context is VmBaseContext)
                {
                    return ExecutionContextType.VmBase;
                }
                return ExecutionContextType.Unknown;
            }
        }

        public virtual IEnumerable<D> CreateList(T param)
        {
            return new D[] 
                { 
                    new D()
                };
        }
        public virtual ServiceResult BeforeRun(T param)
        {
            return new ServiceResult();
        }
        public virtual ServiceResult AfterRun(T param)
        {
            return new ServiceResult();
        }
        public abstract ServiceResult RunItem(T param, D itemParam);
        #endregion

        public ServiceResult Run(T param)
        {
            #region

            #region CreateList (создание списка)
            IEnumerable<D> listParams = this.CreateList(param);
            if (listParams == null || listParams.Count() == 0)
            {
                ServiceResult result1 = new ServiceResult
                {
                    Error = true,
                    ErrorMessage = "Нет записей для обработки",
                };
                if (param.ErrorMessage != null)
                    result1.ErrorMessage = param.ErrorMessage;
                return result1;
            }
            #endregion

            ServiceResult result = new ServiceResult();

            #region BeforeRun (подготовка выполнения)
            ServiceResult beforeResult = this.BeforeRun(param);
            beforeResult.AlgotithmStatus = AlgotithmStatus_Enum.BeforeRun;
            if (beforeResult.Error)
                return beforeResult;
            result.AddResult(beforeResult);
            #endregion

            foreach (D listParam in listParams)
            {
                #region RunItem (выполнение)
                try
                {
                    ServiceResult itemResult = this.RunItem(param, listParam);
                    itemResult.AlgotithmStatus = AlgotithmStatus_Enum.RunItem;
                    result.AddResult(itemResult);
                }
                #region catch
                catch (Exception ex)
                {
                    result = new ServiceResult
                    {
                        Error = true,
                        ErrorMessage = "Общая ошибка AlgHandler.Run()",
                    };
                    if (ex.InnerException != null && ex.InnerException.Message != null)
                        result.ErrorMessageInner = ex.InnerException.Message;
                    else if (ex.Message != null)
                        result.ErrorMessageInner = ex.Message;

                    return result;
                    // log
                }
                #endregion
                #endregion
            }

            #region AfterRun (завершение выполнения)
            ServiceResult afterResult = this.AfterRun(param);
            if (afterResult.Error)
                return afterResult;
            afterResult.AlgotithmStatus = AlgotithmStatus_Enum.AfterRun;
            result.AddResult(afterResult);
            #endregion

            result.Error = (result.CountErrors > 0 && result.Count == result.CountErrors);
            return result;
            #endregion
        }

        #region IAlgHandler
        object IAlgContext.Context
        {
            get
            {
                return this.Context;
            }
            set
            {
                this.Context = value;
            }
        }

        IEnumerable<AlgItemParam> IAlgHandler.CreateList(AlgParam param)
        {
            T typedParam = (T)param;
            return this.CreateList(typedParam);
        }
        ServiceResult IAlgHandler.BeforeRun(AlgParam param)
        {
            T typedParam = (T)param;
            return this.BeforeRun(typedParam);
        }
        ServiceResult IAlgHandler.AfterRun(AlgParam param)
        {
            T typedParam = (T)param;
            return this.AfterRun(typedParam);
        }
        ServiceResult IAlgHandler.RunItem(AlgParam param, AlgItemParam itemParam)
        {
            T typedParam = (T)param;
            D typeditemParam = (D)itemParam;
            return this.RunItem(typedParam, typeditemParam);
        }
        #endregion
    }

    public abstract class AlgHandlerAsync<T, D> : IAlgHandlerAsync
        where T : AlgParam
        where D : AlgItemParam, new()
    {
        public object Context { get; private set; }
        public ExecutionContextType ContextType
        {
            get
            {
                if (this.Context is TaskContext)
                {
                    return ExecutionContextType.Task;
                }
                if (this.Context is VmBaseContext)
                {
                    return ExecutionContextType.VmBase;
                }
                return ExecutionContextType.Unknown;
            }
        }

        public virtual Task<IEnumerable<D>> CreateListAsync(T param, Action<string, string> log)
        {
            return System.Threading.Tasks.Task.FromResult<IEnumerable<D>>(new D[] { new D() });
        }
        public virtual Task<ServiceResult> BeforeRunAsync(T param, Action<string, string> log)
        {
            return System.Threading.Tasks.Task.FromResult(new ServiceResult());
        }
        public virtual Task<ServiceResult> AfterRunAsync(T param, Action<string, string> log)
        {
            return System.Threading.Tasks.Task.FromResult(new ServiceResult());
        }
        public abstract Task<ServiceResult> RunItemAsync(T param, D itemParam, Action<string, string> log);

        public async Task<ServiceResult> RunAsync(T param)
        {
            #region
            //string logString = String.Empty;
            //Log log = (h, m) => logString += $"{h}:\n{m}\n\n";

            #region CreateList (создание списка)
            IEnumerable<D> listParams = await this.CreateListAsync(param, null);
            if (listParams.Count() == 0)
            {
                return new ServiceResult
                {
                    Error = true,
                    ErrorMessage = "Нет записей для обработки",
                };
            }
            #endregion

            ServiceResult result = new ServiceResult();

            #region BeforeRun (подготовка выполнения)
            ServiceResult beforeResult = await this.BeforeRunAsync(param, null);
            beforeResult.AlgotithmStatus = AlgotithmStatus_Enum.BeforeRun;
            result.AddResult(beforeResult);
            if (beforeResult.Error)
                return result;
            #endregion

            foreach (D listParam in listParams)
            {
                #region RunItem (выполнение)
                //try
                {
                    ServiceResult itemResult = await this.RunItemAsync(param, listParam, null);
                    itemResult.AlgotithmStatus = AlgotithmStatus_Enum.RunItem;
                    result.AddResult(itemResult);
                }
                //catch (Exception ex)
                //{
                //    result = new ServiceResult
                //    {
                //        Error = true,
                //        ErrorMessage = "Общая ошибка AlgHandler.Run()",
                //    };
                //    if (ex.InnerException != null && ex.InnerException.Message != null)
                //        result.ErrorMessageInner = ex.InnerException.Message;
                //    else if (ex.Message != null)
                //        result.ErrorMessageInner = ex.Message;

                //    return result;
                //    // log
                //}
                #endregion
            }

            #region AfterRun (завершение выполнения)
            ServiceResult afterResult = await this.AfterRunAsync(param, null);
            afterResult.AlgotithmStatus = AlgotithmStatus_Enum.AfterRun;
            result.AddResult(afterResult);
            #endregion

            result.Error = (result.CountErrors > 0 && result.Count == result.CountErrors);
            return result;
            #endregion
        }
        public ServiceResult Run(T param)
        {
            //ServiceResult result = this.RunAsync(param)
            //    .GetAwaiter()
            //    .GetResult();

            ServiceResult result = AsyncHelper.RunSync(() => this.RunAsync(param));
            return result;
        }

        #region IAlgHandlerAsync
        object IAlgContext.Context
        {
            get
            {
                return this.Context;
            }
            set
            {
                this.Context = value;
            }
        }

        async Task<IEnumerable<AlgItemParam>> IAlgHandlerAsync.CreateListAsync(AlgParam param, Action<string, string> log)
        {
            T typedParam = (T)param;
            return await this.CreateListAsync(typedParam, log);
        }
        Task<ServiceResult> IAlgHandlerAsync.BeforeRunAsync(AlgParam param, Action<string, string> log)
        {
            T typedParam = (T)param;
            return this.BeforeRunAsync(typedParam, log);
        }
        Task<ServiceResult> IAlgHandlerAsync.AfterRunAsync(AlgParam param, Action<string, string> log)
        {
            T typedParam = (T)param;
            return this.AfterRunAsync(typedParam, log);
        }
        Task<ServiceResult> IAlgHandlerAsync.RunItemAsync(AlgParam param, AlgItemParam itemParam, Action<string, string> log)
        {
            T typedParam = (T)param;
            D typeditemParam = (D)itemParam;
            return this.RunItemAsync(typedParam, typeditemParam, log);
        }
        #endregion
    }
}

namespace Tsb.Extensions
{
    public static class AsyncHelper
    {
        private readonly static TaskFactory _myTaskFactory;

        static AsyncHelper()
        {
            AsyncHelper._myTaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);
        }

        public static TResult RunSync<TResult>(Func<System.Threading.Tasks.Task<TResult>> func)
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

        public static void RunSync(Func<System.Threading.Tasks.Task> func)
        {
            CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            TaskAwaiter awaiter = AsyncHelper._myTaskFactory.StartNew<System.Threading.Tasks.Task>(() =>
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
                Thread.CurrentThread.CurrentUICulture = currentUICulture;
                return func();
            }).Unwrap().GetAwaiter();
            awaiter.GetResult();
        }
    }
}
