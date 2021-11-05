using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

[assembly: InternalsVisibleTo("Tsb.Server.Core")]
[assembly: InternalsVisibleTo("Tsb.Tasks.Tests")]

namespace Tsb.Extensions.Task
{    
    [AttributeUsage(AttributeTargets.Property)]
    public class TaskConfigAttribute : Attribute
    {
        public string Config { get; private set; }
        public int Order { get; private set; }

        public TaskConfigAttribute(string config) :
            this(config, int.MaxValue)
        {
        }
        public TaskConfigAttribute(string config, int order)
        {
            this.Config = config;
            this.Order = order;
        }
    }

    public class TaskParam
    {
    }

    /// <summary>
    /// Информация о типе бизнес-процесса, в контексте которого создан экземпляр пакетной задачи.
    /// (изначально это значение XML атрибута NTASK_GROUP_ID в TASK.DETAIL).
    /// Интерфейс для реализации в наследниках класса TaskParam.
    /// </summary>
    public interface ITaskTopicInfo
    {
        int TopicId { get; }
    }

    /* 
     * убрать из Param поля, которые есть в таблице
     * запихать их в TaskDetailInfo ? - тогда ее возвращать из CreateTaskDetails
     */
    public class TaskDetailParam
    {
        [XmlAttribute("STableId")]
        [Display(AutoGenerateField = false)]
        public int STableId { get; set; }
        [XmlIgnore]
        [Display(AutoGenerateField = false)]
        public bool STableIdSpecified
        {
            get
            {
                return this.STableId != 0;
            }
        }

        [XmlAttribute("Id")]
        [Display(AutoGenerateField = false)]
        public long Id { get; set; }
        [XmlIgnore]
        [Display(AutoGenerateField = false)]
        public bool IdSpecified
        {
            get
            {
                return this.Id != 0;
            }
        }

        [XmlAttribute("Comment")]
        [Display(AutoGenerateField = false)]
        public string Comment { get; set; }
    }

    public class TaskResult
    {
        public bool Result { get; set; }
        public bool CanRestart { get; set; }
        public string ResultCode { get; set; }
        public string TextResult { get; set; }
        public long? DocumentId { get; set; }
        public bool DocumentIdSpecified { get; set; }

        public static TaskResult True()
        {
            return True(String.Empty);
        }
        public static TaskResult True(string resultText)
        {
            return True(String.Empty, resultText);
        }
        public static TaskResult True(string resultCode, string resultText)
        {
            return new TaskResult
            {
                Result = true,
                ResultCode = resultCode,
                TextResult = resultText,
            };
        }
        public static TaskResult True(string resultText, long? documentId)
        {
            return new TaskResult
            {
                Result = true,
                ResultCode = String.Empty,
                TextResult = resultText,
                DocumentId = documentId,
                DocumentIdSpecified = true,
            };
        }
        public static TaskResult False()
        {
            return False(String.Empty);
        }
        public static TaskResult False(string resultText)
        {
            return False(String.Empty, resultText);
        }
        public static TaskResult False(string resultCode, string resultText)
        {
            return new TaskResult
            {
                Result = false,
                CanRestart = true,
                ResultCode = resultCode,
                TextResult = resultText,
            };
        }
        public static TaskResult False(string resultText, long? documentId)
        {
            return new TaskResult
            {
                Result = false,
                CanRestart = true,
                ResultCode = String.Empty,
                TextResult = resultText,
                DocumentId = documentId,
                DocumentIdSpecified = true,
            };
        }
    }
}

namespace Tsb.Extensions.Task.Generic
{
    internal interface ITaskInfo
    {
        long TaskId { get; set; }
        long ExecutionId { get; set; }
        long? DocumentId { get; set; }
        TaskParam Param { get; set; }
        Action<string, string> LogHandler { get; set; }
    }

    internal interface ITaskDetailInfo
    {
        long ExecutionDetailId { get; set; }
        long? DocumentDetailId { get; set; }
        TaskDetailParam Param { get; set; }
        Action<string, string> LogHandler { get; set; }
    }

    internal interface ITaskHandler
    {
        bool IsInitialExecution { get; set; }
        bool IsExecutionCompleted { get; set; }
        bool IsExecutionCompletedTotally { get; set; }
        bool AllDetailsAreSuccessful { get; set; }
        bool StopExecutionOnDetailError { get; }
        int MaxThreadCount { get; }
        Action<string, string> LogHandler { get; set; }

        IEnumerable<TaskDetailParam> CreateTaskDetails(ITaskInfo taskInfo);
        TaskResult BeforeExecute(ITaskInfo taskInfo);
        TaskResult AfterExecute(ITaskInfo taskInfo);
        TaskResult Execute(ITaskInfo taskInfo, ITaskDetailInfo taskDetailInfo);
    }

    internal interface ITaskHandlerAsync
    {
        bool IsInitialExecution { get; set; }
        bool IsExecutionCompleted { get; set; }
        bool IsExecutionCompletedTotally { get; set; }
        bool AllDetailsAreSuccessful { get; set; }
        bool StopExecutionOnDetailError { get; }
        int MaxThreadCount { get; }

        System.Threading.Tasks.Task<IEnumerable<TaskDetailParam>> CreateTaskDetailsAsync(ITaskInfo taskInfo);
        System.Threading.Tasks.Task<TaskResult> BeforeExecuteAsync(ITaskInfo taskInfo);
        System.Threading.Tasks.Task<TaskResult> AfterExecuteAsync(ITaskInfo taskInfo);
        System.Threading.Tasks.Task<TaskResult> ExecuteAsync(ITaskInfo taskInfo, ITaskDetailInfo taskDetailInfo);
    }

    public class TaskInfo<T> : ITaskInfo
        where T : TaskParam
    {
        public long TaskId { get; private set; }
        public long ExecutionId { get; private set; }
        public long? DocumentId { get; private set; }
        public T Param { get; private set; }

        public void Log(string header, string message)
        {
            var logHandler = ((ITaskInfo)this).LogHandler;
            if (logHandler != null)
            {
                logHandler(header, message);
            }
        }

        #region ITaskInfo
        long ITaskInfo.TaskId
        {
            get
            {
                return this.TaskId;
            }
            set
            {
                this.TaskId = value;
            }
        }
        long ITaskInfo.ExecutionId
        {
            get
            {
                return this.ExecutionId;
            }
            set
            {
                this.ExecutionId = value;
            }
        }
        long? ITaskInfo.DocumentId
        {
            get
            {
                return this.DocumentId;
            }
            set
            {
                this.DocumentId = value;
            }
        }
        TaskParam ITaskInfo.Param
        {
            get
            {
                return this.Param;
            }
            set
            {
                this.Param = (T)value;
                //try
                //{
                //    this.Param = (T)value;
                //}
                //catch (InvalidCastException invalidCastException)
                //{
                //    ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(T));
                //}
            }
        }

        Action<string, string> ITaskInfo.LogHandler
        {
            get;
            set;
        }
        #endregion
    }

    public class TaskDetailInfo<T> : ITaskDetailInfo
        where T : TaskDetailParam
    {
        public long ExecutionDetailId { get; private set; }
        public long? DocumentDetailId { get; private set; }
        public T Param { get; private set; }

        public void Log(string header, string message)
        {
            var logHandler = ((ITaskDetailInfo)this).LogHandler;
            if (logHandler != null)
            {
                logHandler(header, message);
            }
        }

        #region ITaskDetailInfo
        long ITaskDetailInfo.ExecutionDetailId
        {
            get
            {
                return this.ExecutionDetailId;
            }
            set
            {
                this.ExecutionDetailId = value;
            }
        }
        long? ITaskDetailInfo.DocumentDetailId
        {
            get
            {
                return this.DocumentDetailId;
            }
            set
            {
                this.DocumentDetailId = value;
            }
        }
        TaskDetailParam ITaskDetailInfo.Param
        {
            get
            {
                return this.Param;
            }
            set
            {
                this.Param = (T)value;
                //try
                //{
                //    this.Param = (T)value;
                //}
                //catch (InvalidCastException invalidCastException)
                //{
                //    ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(T));
                //}
            }
        }

        Action<string, string> ITaskDetailInfo.LogHandler
        {
            get;
            set;
        }
        #endregion
    }

    public abstract class TaskHandler : TaskHandler<TaskParam>
    {
    }

    public abstract class TaskHandler<T> : TaskHandler<T, TaskDetailParam>
        where T : TaskParam
    {
    }

    public abstract class TaskHandler<T, D> : ITaskHandler
        where T : TaskParam
        where D : TaskDetailParam, new()
    {
        private readonly ConcurrentDictionary<int, Action<string, string>> logHandlers = new ConcurrentDictionary<int, Action<string, string>>();

        protected bool IsInitialExecution { get; private set; }
        protected bool IsExecutionCompleted { get; private set; }
        protected bool IsExecutionCompletedTotally { get; private set; }
        protected bool AllDetailsAreSuccessful { get; private set; }
        protected virtual bool StopExecutionOnDetailError
        {
            get
            {
                return false;
            }
        }
        protected virtual int MaxThreadCount
        {
            get
            {
                return -1;
            }
        }
        protected Action<string, string> LogHandler
        {
            get
            {
                Action<string, string> result;
                if (this.logHandlers.TryGetValue(System.Threading.Thread.CurrentThread.ManagedThreadId, out result))
                {
                    return result;
                }
                return null;
            }
            private set
            {
                this.logHandlers.AddOrUpdate(System.Threading.Thread.CurrentThread.ManagedThreadId, value, (id, v) => value);
            }
        }

        protected void Log(string header, string message)
        {
            if (this.LogHandler != null)
            {
                this.LogHandler(header, message);
            }
        }
        public virtual IEnumerable<D> CreateTaskDetails(TaskInfo<T> taskInfo)
        {
            return new D[] 
                { 
                    //this.CreateTaskDetail(new D())
                    new D()
                };
        }
        public virtual TaskResult BeforeExecute(TaskInfo<T> taskInfo)
        {
            return TaskResult.True();
        }
        public virtual TaskResult AfterExecute(TaskInfo<T> taskInfo)
        {
            return TaskResult.True();
        }
        public abstract TaskResult Execute(TaskInfo<T> taskInfo, TaskDetailInfo<D> taskDetailInfo);

        #region ITaskHandler
        bool ITaskHandler.IsInitialExecution
        {
            get
            {
                return this.IsInitialExecution;
            }
            set
            {
                this.IsInitialExecution = value;
            }
        }
        bool ITaskHandler.IsExecutionCompleted
        {
            get
            {
                return this.IsExecutionCompleted;
            }
            set
            {
                this.IsExecutionCompleted = value;
            }
        }
        bool ITaskHandler.IsExecutionCompletedTotally
        {
            get
            {
                return this.IsExecutionCompletedTotally;
            }
            set
            {
                this.IsExecutionCompletedTotally = value;
            }
        }
        bool ITaskHandler.AllDetailsAreSuccessful
        {
            get
            {
                return this.AllDetailsAreSuccessful;
            }
            set
            {
                this.AllDetailsAreSuccessful = value;
            }
        }
        bool ITaskHandler.StopExecutionOnDetailError
        {
            get
            {
                return this.StopExecutionOnDetailError;
            }
        }
        int ITaskHandler.MaxThreadCount
        {
            get
            {
                return this.MaxThreadCount;
            }
        }
        Action<string, string> ITaskHandler.LogHandler
        {
            get
            {
                return this.LogHandler;
            }
            set
            {
                this.LogHandler = value;
            }
        }

        IEnumerable<TaskDetailParam> ITaskHandler.CreateTaskDetails(ITaskInfo taskInfo)
        {
            TaskInfo<T> typedTaskInfo = (TaskInfo<T>)taskInfo;
            return this.CreateTaskDetails(typedTaskInfo);
        }
        TaskResult ITaskHandler.BeforeExecute(ITaskInfo taskInfo)
        {
            TaskInfo<T> typedTaskInfo = (TaskInfo<T>)taskInfo;
            return this.BeforeExecute(typedTaskInfo);
        }
        TaskResult ITaskHandler.AfterExecute(ITaskInfo taskInfo)
        {
            TaskInfo<T> typedTaskInfo = (TaskInfo<T>)taskInfo;
            return this.AfterExecute(typedTaskInfo);
        }
        TaskResult ITaskHandler.Execute(ITaskInfo taskInfo, ITaskDetailInfo taskDetailInfo)
        {
            TaskInfo<T> typedTaskInfo = (TaskInfo<T>)taskInfo;
            TaskDetailInfo<D> typedTaskDetailInfo = (TaskDetailInfo<D>)taskDetailInfo;
            return this.Execute(typedTaskInfo, typedTaskDetailInfo);
        }
        #endregion
    }

    public abstract class TaskHandlerAsync<T, D> : ITaskHandlerAsync
        where T : TaskParam
        where D : TaskDetailParam, new()
    {
        protected bool IsInitialExecution { get; private set; }
        protected bool IsExecutionCompleted { get; private set; }
        protected bool IsExecutionCompletedTotally { get; private set; }
        protected bool AllDetailsAreSuccessful { get; private set; }
        protected virtual bool StopExecutionOnDetailError
        {
            get
            {
                return false;
            }
        }
        protected virtual int MaxThreadCount
        {
            get
            {
                return -1;
            }
        }

        public virtual System.Threading.Tasks.Task<IEnumerable<D>> CreateTaskDetailsAsync(TaskInfo<T> taskInfo)
        {
            return System.Threading.Tasks.Task.FromResult<IEnumerable<D>>(new D[] { new D() });
        }
        public virtual System.Threading.Tasks.Task<TaskResult> BeforeExecuteAsync(TaskInfo<T> taskInfo)
        {
            return System.Threading.Tasks.Task.FromResult(TaskResult.True());
        }
        public virtual System.Threading.Tasks.Task<TaskResult> AfterExecuteAsync(TaskInfo<T> taskInfo)
        {
            return System.Threading.Tasks.Task.FromResult(TaskResult.True());
        }
        public abstract System.Threading.Tasks.Task<TaskResult> ExecuteAsync(TaskInfo<T> taskInfo, TaskDetailInfo<D> taskDetailInfo);

        #region ITaskHandlerAsync
        bool ITaskHandlerAsync.IsInitialExecution
        {
            get
            {
                return this.IsInitialExecution;
            }
            set
            {
                this.IsInitialExecution = value;
            }
        }
        bool ITaskHandlerAsync.IsExecutionCompleted
        {
            get
            {
                return this.IsExecutionCompleted;
            }
            set
            {
                this.IsExecutionCompleted = value;
            }
        }
        bool ITaskHandlerAsync.IsExecutionCompletedTotally
        {
            get
            {
                return this.IsExecutionCompletedTotally;
            }
            set
            {
                this.IsExecutionCompletedTotally = value;
            }
        }
        bool ITaskHandlerAsync.AllDetailsAreSuccessful
        {
            get
            {
                return this.AllDetailsAreSuccessful;
            }
            set
            {
                this.AllDetailsAreSuccessful = value;
            }
        }
        bool ITaskHandlerAsync.StopExecutionOnDetailError
        {
            get
            {
                return this.StopExecutionOnDetailError;
            }
        }
        int ITaskHandlerAsync.MaxThreadCount
        {
            get
            {
                return this.MaxThreadCount;
            }
        }

        async System.Threading.Tasks.Task<IEnumerable<TaskDetailParam>> ITaskHandlerAsync.CreateTaskDetailsAsync(ITaskInfo taskInfo)
        {
            TaskInfo<T> typedTaskInfo = (TaskInfo<T>)taskInfo;
            return await this.CreateTaskDetailsAsync(typedTaskInfo);
        }
        System.Threading.Tasks.Task<TaskResult> ITaskHandlerAsync.BeforeExecuteAsync(ITaskInfo taskInfo)
        {
            TaskInfo<T> typedTaskInfo = (TaskInfo<T>)taskInfo;
            return this.BeforeExecuteAsync(typedTaskInfo);
        }
        System.Threading.Tasks.Task<TaskResult> ITaskHandlerAsync.AfterExecuteAsync(ITaskInfo taskInfo)
        {
            TaskInfo<T> typedTaskInfo = (TaskInfo<T>)taskInfo;
            return this.AfterExecuteAsync(typedTaskInfo);
        }
        System.Threading.Tasks.Task<TaskResult> ITaskHandlerAsync.ExecuteAsync(ITaskInfo taskInfo, ITaskDetailInfo taskDetailInfo)
        {
            TaskInfo<T> typedTaskInfo = (TaskInfo<T>)taskInfo;
            TaskDetailInfo<D> typedTaskDetailInfo = (TaskDetailInfo<D>)taskDetailInfo;
            return this.ExecuteAsync(typedTaskInfo, typedTaskDetailInfo);
        }
        #endregion
    }
}
