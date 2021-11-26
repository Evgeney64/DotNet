using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Logging;
using Server.Core.Context;

using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
//using Microsoft.EntityFrameworkCore.Query.Expressions;
//using Microsoft.EntityFrameworkCore.Query.Sql;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore.Storage;

namespace Server.Core.Model
{
    public partial class EntityServ : EntityService<EntityContext>
    {
        #region Context
        public EntityServ()
        { }
        public EntityServ(string connectionString)
            : base(connectionString)
        { }

        protected override EntityContext Context
        {
            get
            {
                if (base.Context == null)
                {
                    //base.Context = EntityContext.CreateContext(connectionString);
                    base.Context = new EntityContext(connectionString);
                }
                return base.Context;
            }
        }
        #endregion

        #region Services
        public IQueryable<village> Get_NSI_STREET_1()
        {
            IQueryable<village> items = Context.village;
            List<village> list = items.ToList(); ;

            //var entityFrameworkSqlLogger = new EntityFrameworkSqlLogger((m) =>
            //{
            //    Console.WriteLine($"SQL Query:\r\n{m.CommandText}\r\nElapsed:{m.Elapsed} millisecods\r\n\r\n");
            //});
            //var loggerFactory = LoggerFactory.Create(builder =>
            //{
            //    builder
            //    .AddFilter((category, level) =>
            //        category == DbLoggerCategory.Database.Command.Name
            //        && level == LogLevel.Information);
            //});
            //loggerFactory.AddProvider(new SingletonLoggerProvider(entityFrameworkSqlLogger));
            //using (var ordersDbContext = new OrdersDbContext(loggerFactory))
            //{
            //    var orderLines = ordersDbContext.OrderLines.Where(o => o.Id == Guid.Empty).ToList();
            //    orderLines = ordersDbContext.OrderLines.ToList();
            //}


            return items;
        }

        #region Gos
        //public IQueryable<village> Get_village() => Context.village;
        //public IQueryable<rgn> Get_rgn() => Context.rgn;

        //public IQueryable<street> GetStreets() => Context.street;
        //public IQueryable<type_street> GetTypeStreets() => Context.type_street;
        #endregion
        #endregion

        #region ToSql
        //public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        //{
        //    using (var enumerator = query.Provider.Execute<IEnumerable<TEntity>>(query.Expression).GetEnumerator())
        //    {
        //        var relationalCommandCache = enumerator.Private("_relationalCommandCache");
        //        var selectExpression = relationalCommandCache.Private<SelectExpression>("_selectExpression");
        //        var factory = relationalCommandCache.Private<IQuerySqlGeneratorFactory>("_querySqlGeneratorFactory");

        //        var sqlGenerator = factory.Create();
        //        var command = sqlGenerator.GetCommand(selectExpression);

        //        string sql = command.CommandText;
        //        return sql;
        //    }
        //}

        //private static object Private(this object obj, string privateField) => obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
        //private static T Private<T>(this object obj, string privateField) => (T)obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
        #endregion
    }


    //public static class IQueryableExtensions
    //{
    //    private static readonly TypeInfo QueryCompilerTypeInfo = typeof(QueryCompiler).GetTypeInfo();

    //    private static readonly FieldInfo QueryCompilerField = typeof(EntityQueryProvider).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryCompiler");

    //    private static readonly FieldInfo QueryModelGeneratorField = QueryCompilerTypeInfo.DeclaredFields.First(x => x.Name == "_queryModelGenerator");

    //    private static readonly FieldInfo DataBaseField = QueryCompilerTypeInfo.DeclaredFields.Single(x => x.Name == "_database");

    //    private static readonly PropertyInfo DatabaseDependenciesField = typeof(Database).GetTypeInfo().DeclaredProperties.Single(x => x.Name == "Dependencies");

    //    public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
    //    {
    //        var queryCompiler = (QueryCompiler)QueryCompilerField.GetValue(query.Provider);
    //        var modelGenerator = (QueryModelGenerator)QueryModelGeneratorField.GetValue(queryCompiler);
    //        var queryModel = modelGenerator.ParseQuery(query.Expression);
    //        var database = (IDatabase)DataBaseField.GetValue(queryCompiler);
    //        var databaseDependencies = (DatabaseDependencies)DatabaseDependenciesField.GetValue(database);
    //        var queryCompilationContext = databaseDependencies.QueryCompilationContextFactory.Create(false);
    //        var modelVisitor = (RelationalQueryModelVisitor)queryCompilationContext.CreateQueryModelVisitor();
    //        modelVisitor.CreateQueryExecutor<TEntity>(queryModel);
    //        var sql = modelVisitor.Queries.First().ToString();

    //        return sql;
    //    }
    //}
    public class EntityFrameworkSqlLogger : ILogger
    {
        #region Fields
        Action<EntityFrameworkSqlLogMessage> _logMessage;
        #endregion
        #region Constructor
        public EntityFrameworkSqlLogger(Action<EntityFrameworkSqlLogMessage> logMessage)
        {
            _logMessage = logMessage;
        }
        #endregion
        #region Implementation
        public IDisposable BeginScope<TState>(TState state)
        {
            return default;
        }
        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (eventId.Id != 20101)
            {
                //Filter messages that aren't relevant.
                //There may be other types of messages that are relevant for other database platforms...
                return;
            }
            if (state is IReadOnlyList<KeyValuePair<string, object>> keyValuePairList)
            {
                var entityFrameworkSqlLogMessage = new EntityFrameworkSqlLogMessage
                (
                    eventId,
                    (string)keyValuePairList.FirstOrDefault(k => k.Key == "commandText").Value,
                    (string)keyValuePairList.FirstOrDefault(k => k.Key == "parameters").Value,
                    (CommandType)keyValuePairList.FirstOrDefault(k => k.Key == "commandType").Value,
                    (int)keyValuePairList.FirstOrDefault(k => k.Key == "commandTimeout").Value,
                    (string)keyValuePairList.FirstOrDefault(k => k.Key == "elapsed").Value
                );
                _logMessage(entityFrameworkSqlLogMessage);
            }
        }
        #endregion
    }

    public class EntityFrameworkSqlLogMessage
    {
        public EntityFrameworkSqlLogMessage(
            EventId eventId,
            string commandText,
            string parameters,
            CommandType commandType,
            int commandTimeout,
            string elapsed
            )
        {
            EventId = eventId;
            CommandText = commandText;
            Parameters = parameters;
            CommandType = commandType;
            Elapsed = elapsed;
            CommandTimeout = commandTimeout;
        }
        public string Elapsed { get; }
        public int CommandTimeout { get; }
        public EventId EventId { get; }
        public string CommandText { get; }
        public string Parameters { get; }
        public CommandType CommandType { get; }
    }
    public class SingletonLoggerProvider : ILoggerProvider
    {
        #region Fields
        ILogger _logger;
        #endregion
        #region Constructor
        public SingletonLoggerProvider(ILogger logger)
        {
            _logger = logger;
        }
        #endregion
        #region Implementation
        public ILogger CreateLogger(string categoryName)
        {
            return _logger;
        }
        public void Dispose()
        {
        }
        #endregion
    }
}
