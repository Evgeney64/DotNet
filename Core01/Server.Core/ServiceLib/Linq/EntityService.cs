#region using
using System;
using Microsoft.EntityFrameworkCore;

using Server.Core.Public;

#endregion

namespace ServiceLib
{
    public abstract partial class EntityServiceLogging
    {
        public readonly static string LogCrtDateFieldName = "CRT_DATE";
        public readonly static string LogMfyDateFieldName = "MFY_DATE";
        public readonly static string LogMfySUserIdFieldName = "MFY_SUSER_ID";

        public readonly static string[] LogFieldNames = new string[] { LogCrtDateFieldName, LogMfyDateFieldName, LogMfySUserIdFieldName };
    }

    public abstract partial class EntityService<TContext> : EntityServiceLogging, IDisposable
        where TContext : DbContext, new()
    {
        #region Define
        //private ApiContext apiContext;
        protected string connectionString;

        protected virtual TContext Context { get; set; }
        //protected TContext Context
        //{
        //    get
        //    {
        //        if (this.context == null)
        //        {
        //            this.context = this.CreateContext();
        //        }
        //        return this.context;
        //    }
        //}
        //private TContext context;
        #endregion

        #region Constructor
        public EntityService()
        { }
        protected EntityService(string _connectionString)
        {
            connectionString = _connectionString;
        }
        #endregion

        #region CreateContext
        protected virtual TContext CreateContext()
        {
            return CreateContext(connectionString);
        }
        public static TContext CreateContext(string connectionStringName)
        {
            #region
            var constructorInfo = typeof(TContext).GetConstructor(new Type[] { typeof(DbContextOptions<EntityContext>) });
            if (constructorInfo == null)
            {
                throw new Exception("DbContext должен иметь конструктор с параметром EntityConnection.");
            }

            DbContextOptions<EntityContext> contextOptions = new DbContextOptionsBuilder<EntityContext>()
                .UseSqlServer(connectionStringName)
                .Options;

            TContext context = (TContext)constructorInfo.Invoke(new object[] { contextOptions });
            return context;
            #endregion
        }
        //protected virtual ApiContext CreateApiContext()
        //{
        //    if (StaticContext.UseApi)
        //    {
        //        return new ApiContext();
        //    }

        //    throw new InvalidOperationException("BillBerry API отключен.");
        //}
        #endregion

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.Context != null)
                {
                    this.Context.Dispose();
                }
            }
        }
        public void Dispose()
        {
            this.Dispose(true);
        }
        #endregion

        #region archive
        /*
        private static MethodInfo filterMethodInfo = typeof(EntityService<TContext>).GetMethod("Filter", BindingFlags.Public | BindingFlags.Static);
        private static MethodInfo checkMethodInfo = typeof(EntityService<TContext>).GetMethod("Check", BindingFlags.Public | BindingFlags.Static);
        private static ConcurrentDictionary<Type, GroupsMethods> groupsMethodsDict = new ConcurrentDictionary<Type, GroupsMethods>();

        private static GroupsMethods GetGroupsMethods(Type typeEntity)
        {
            #region
            Func<Type, GroupsMethods> valueFactory = type =>
            {
                return new GroupsMethods
                {
                    Filter = (Func<TContext, IQueryable, IEnumerable<int>, IEnumerable<int>, IQueryable>)Delegate.CreateDelegate(typeof(Func<TContext, IQueryable, IEnumerable<int>, IEnumerable<int>, IQueryable>), filterMethodInfo.MakeGenericMethod(new Type[] { type })),
                    Check = (Action<TContext, object, IEnumerable<int>, IEnumerable<int>>)Delegate.CreateDelegate(typeof(Action<TContext, object, IEnumerable<int>, IEnumerable<int>>), checkMethodInfo.MakeGenericMethod(new Type[] { type }))
                };
            };

            return groupsMethodsDict.GetOrAdd(typeEntity, valueFactory);
            #endregion
        }
        private static bool TryGetGroups(IGroupPrincipal principal, IEnumerable<string> roles, out IEnumerable<int> grantGroups, out IEnumerable<int> denyGroups)
        {
            #region
            grantGroups = Enumerable.Empty<int>();
            denyGroups = Enumerable.Empty<int>();
            foreach (string role in roles)
            {
                if (!principal.IsInRole(role))
                {
                    return false;
                }
                IGroups groupsForRole = principal.GetGroupsForRole(role);
                grantGroups = grantGroups.Union(groupsForRole.Grant);
                denyGroups = denyGroups.Union(groupsForRole.Deny);
            }
            return grantGroups.Any() || denyGroups.Any();
            #endregion
        }

        public static IQueryable Filter<T>(TContext context, IQueryable sourceQuery, IEnumerable<int> grantGroups, IEnumerable<int> denyGroups)
        {
            #region
            IQueryable<T> queryRoot = sourceQuery as IQueryable<T> ?? Queryable.AsQueryable<T>(new T[0]);
            if (context is IContextWithGroup<T>)
            {
                queryRoot = ((IContextWithGroup<T>)context).Filter(queryRoot, grantGroups, denyGroups);
            }
            return queryRoot;
            #endregion
        }
        public static void Check<T>(TContext context, object entity, IEnumerable<int> grantGroups, IEnumerable<int> denyGroups)
        {
            #region
            if (context is IContextWithGroup<T> && entity is T && !((IContextWithGroup<T>)context).Check((T)entity, grantGroups, denyGroups))
            {
                throw new UnauthorizedAccessException();
            }
            #endregion
        }

        private bool TryGetPrincipalAndRoles(DomainOperationEntry method, out IGroupPrincipal principal, out IEnumerable<string> roles)
        {
            #region
            roles = Enumerable.Empty<string>();
            principal = this.ServiceContext.User as IGroupPrincipal;
            if (principal != null)
            {
                IEnumerable<RequiresRoleExtAttribute> roleAttributes = method.Attributes.OfType<RequiresRoleExtAttribute>();
                foreach (RequiresRoleExtAttribute roleAttribute in roleAttributes)
                {
                    roles = roles.Union(roleAttribute.Roles);
                }
            }
            return roles.Any();
            #endregion
        }
        private object OriginalEntity(object entity)
        {
            #region
            Type typeEntitySet = typeof(ObjectSet<>).MakeGenericType(entity.GetType());

            string entitySetName = String.Concat(
                this.ObjectContext.GetType().Name,
                ".",
                this.ObjectContext
                    .GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .SingleOrDefault(pr => pr.PropertyType == typeEntitySet).Name);

            EntityKey entityKey = this.ObjectContext.CreateEntityKey(entitySetName, entity);
            object originalEntity;
            this.ObjectContext.TryGetObjectByKey(entityKey, out originalEntity);

            return originalEntity;
            #endregion
        }
        public override IEnumerable Query(QueryDescription queryDescription, out IEnumerable<ValidationResult> validationErrors, out int totalCount)
        {
            #region
            try
            {
                IGroupPrincipal principal;
                IEnumerable<string> roles;
                IEnumerable<int> grantGroups, denyGroups;
                if (TryGetPrincipalAndRoles(queryDescription.Method, out principal, out roles))
                {
                    if (queryDescription.Method.Attributes.OfType<FilterAttribute>().Any() && TryGetGroups(principal, roles, out grantGroups, out denyGroups))
                    {
                        Type entityType = queryDescription.Method.AssociatedType;
                        if (typeof(EntityObject).IsAssignableFrom(entityType))
                        {
                            IQueryable query = GetGroupsMethods(entityType).Filter(this.ObjectContext, null, grantGroups, denyGroups);
                            if (query != null)
                            {
                                if (queryDescription.Query != null)
                                {
                                    query = query.Provider.CreateQuery(new Visitor(query.Expression).Visit(queryDescription.Query.Expression));
                                }
                                queryDescription = new QueryDescription(queryDescription.Method, queryDescription.ParameterValues, queryDescription.IncludeTotalCount, query);
                            }
                        }
                    }
                }
                return base.Query(queryDescription, out validationErrors, out totalCount);
            }
            catch (Exception e)
            {
                if (WebConfigurationHelper.GetConfigSectionLog().IsOn)
                {
                    string message = e.InnerException != null ? String.Format("[{0}: {1}]", e.Message, e.InnerException.Message) : String.Format("[{0}]", e.Message);
                    Tsb.WCF.Web.SystemSpace.SysTables.TablesServ tablesServ = new Tsb.WCF.Web.SystemSpace.SysTables.TablesServ();
                    tablesServ.Insert_SysLog(null, new SYS_LOG { DATE_BEG = DateTime.Now, COMMENT = message });
                }
                throw;
            }
            #endregion
        }
        public override object Invoke(InvokeDescription invokeDescription, out IEnumerable<ValidationResult> validationErrors)
        {
            #region
            IGroupPrincipal principal;
            IEnumerable<string> roles;
            IEnumerable<int> grantGroups, denyGroups;
            if (TryGetPrincipalAndRoles(invokeDescription.Method, out principal, out roles))
            {
                for (int i = 0; i < invokeDescription.Method.Parameters.Count; i++)
                {
                    DomainOperationParameter parameter = invokeDescription.Method.Parameters[i];
                    if (invokeDescription.Method.Parameters[i].Attributes.OfType<CheckAttribute>().Any() && TryGetGroups(principal, roles, out grantGroups, out denyGroups))
                    {
                        Type entityType = parameter.ParameterType;
                        if (typeof(EntityObject).IsAssignableFrom(entityType))
                        {
                            //object originalEntity = this.OriginalEntity(invokeDescription.ParameterValues[i]);
                            //if (originalEntity != null)
                            //{
                            //    GetGroupsMethods(entityType).Check(this.ObjectContext, originalEntity, groups);
                            //}
                            GetGroupsMethods(entityType).Check(this.ObjectContext, invokeDescription.ParameterValues[i], grantGroups, denyGroups);
                        }
                    }
                }
            }
            return base.Invoke(invokeDescription, out validationErrors);
            #endregion
        }

        private class Visitor : ExpressionVisitor
        {
            #region
            private Expression root;

            public Visitor(Expression root)
            {
                this.root = root;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Arguments.Count > 0 && node.Arguments[0].NodeType == ExpressionType.Constant && ((ConstantExpression)node.Arguments[0]).Value is IQueryable)
                {
                    List<Expression> list = new List<Expression> { this.root };
                    list.AddRange(node.Arguments.Skip<Expression>(1));
                    return Expression.Call(node.Method, list.ToArray());
                }
                return base.VisitMethodCall(node);
            }
            #endregion
        }

        private class GroupsMethods
        {
            public Func<TContext, IQueryable, IEnumerable<int>, IEnumerable<int>, IQueryable> Filter { get; set; }
            public Action<TContext, object, IEnumerable<int>, IEnumerable<int>> Check { get; set; }
        }
        */
        #endregion
    }
}

namespace Tsb.WCF.Web
{
    public static class EntityServiceExtensions
    {
        public static void AddObject<TEntity>(this DbSet<TEntity> dbSet, TEntity entity)
            where TEntity : class
        {
            dbSet.Add(entity);
        }
        public static void DeleteObject<TEntity>(this DbSet<TEntity> dbSet, TEntity entity)
            where TEntity : class
        {
            dbSet.Remove(entity);
        }
    }
}