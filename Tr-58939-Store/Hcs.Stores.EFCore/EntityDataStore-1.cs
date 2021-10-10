using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace Hcs.DataSources
{
    [Flags]
    public enum LogMode
    {
        None = 0,
        Request = 1,
        Response = 2,
        Trace = 4,
    }
    public class LogConfiguration
    {
        public LogMode Mode { get; set; }
    }
    public class EntityDataSourceConfiguration
    {
        private readonly int defaultCommandTimeout = 300;

        public string HcsConnectionStringName { get; set; }
        public int CommandTimeout { get; set; }
        public LogConfiguration Log { get; private set; }

        public EntityDataSourceConfiguration()
        {
            this.CommandTimeout = this.defaultCommandTimeout;
            this.Log = new LogConfiguration();
        }
    }

    public class DataSourceException : CommonException
    {
        private static readonly string defaultMessage = "Ошибка при работе с источником данных.";
        private static readonly string defaultCode = "STR_GEN_00000";

        public DataSourceException(string code)
            : base(defaultMessage, code)
        {
        }
        public DataSourceException(string code, Exception innerException)
            : base(defaultMessage, code, innerException)
        {
        }
        public DataSourceException()
            : base(defaultMessage, defaultCode)
        {
        }
        public DataSourceException(Exception innerException)
            : base(defaultMessage, defaultCode, innerException)
        {
        }

        //$$$
        //public DataSourceException(System.Data.SqlClient.SqlException sqlException)
        //    : base(defaultMessage, String.Format("STR_SQL_{0:00000}", sqlException.Number), sqlException)
        //{
        //}
    }

}

namespace Hcs
{
    public interface IEntityRelation
    {
        Type Type { get; }
        // временное свойство
        string ParentKey { get; }
        // временное свойство
        IEnumerable<KeyValuePair<string, Type>> ReferenceKeys { get; }
        // временное свойство
        IEnumerable<KeyValuePair<string, Type>> PrivateNsiReferenceKeys { get; }

        IEnumerable<KeyValuePair<string, IEntityRelation>> Navigation { get; }
        IEnumerable<string> References { get; }
    }

    #region EntityRelation
    public class EntityRelationBuilder
    {
        public readonly IDictionary<Type, IEntityRelation> entities = new Dictionary<Type, IEntityRelation>();

        public EntityRelationBuilder Entity<TEntity>()
        {
            IEntityRelation entityRelation;
            if (!entities.TryGetValue(typeof(TEntity), out entityRelation))
            {
                entityRelation = new EntityRelation<TEntity>();
                entities[typeof(TEntity)] = entityRelation;
            }

            return this;
        }
        public IEntityRelation EntitySet<TEntity>()
        {
            IEntityRelation entityRelation;
            if (!entities.TryGetValue(typeof(TEntity), out entityRelation))
            {
                entityRelation = new EntityRelation<TEntity>();
                entities[typeof(TEntity)] = entityRelation;
            }

            return entityRelation;
        }
        public EntityRelationBuilder Entity<TEntity>(Action<EntityRelation<TEntity>> buildAction)
        {
            if (buildAction == null)
            {
                throw new ArgumentNullException(nameof(buildAction));
            }

            IEntityRelation entityRelation;
            if (!entities.TryGetValue(typeof(TEntity), out entityRelation))
            {
                entityRelation = new EntityRelation<TEntity>();
                entities[typeof(TEntity)] = entityRelation;
            }
            buildAction.Invoke((EntityRelation<TEntity>)entityRelation);

            return this;
        }
        public EntityRelation<TEntity> GetEntityRelation<TEntity>()
        {
            IEntityRelation entityRelation;
            if (!this.entities.TryGetValue(typeof(TEntity), out entityRelation))
            {
                throw new Exception(String.Format("Не заданы зависимости для типа {0}.", typeof(TEntity)));
            }
            return (EntityRelation<TEntity>)entityRelation;
        }

        public List<string> EntityRelations = new List<string>();
        public void EntityRelationSet(Type type)
        {
            MethodInfo method = typeof(EntityRelationBuilder).GetMethod("EntitySet");
            MethodInfo methodGen = method.MakeGenericMethod(new[] { type });
            IEntityRelation item = (IEntityRelation)methodGen.Invoke(this, new object[] { });

            EntityNavigationRecurce(item, type, 0);
        }
        public void EntityNavigationRecurce(IEntityRelation item, Type type, int step)
        {
            foreach (PropertyInfo prop in type.GetProperties()
                .Where(ss => ss.CustomAttributes
                    .Where(ss1 => ss1.AttributeType.Name == "InversePropertyAttribute").Count() > 0)
                )
            {
                if (EntityRelations.Contains(prop.Name))
                    continue;

                Type type1 = prop.PropertyType;
                if (prop.PropertyType.GetGenericArguments().Count() == 1)
                    type1 = prop.PropertyType.GetGenericArguments().Single();

                MethodInfo method1 = item.GetType().GetMethod("NavigateSet");
                MethodInfo methodGen1 = method1.MakeGenericMethod(new[] { type1 });
                IEntityRelation item1 = (IEntityRelation)methodGen1.Invoke(item, new object[] { prop.Name });
                EntityRelations.Add(prop.Name);

                if (step < 10)
                {
                    EntityNavigationRecurce(item1, type1, step + 1);
                }
            }

        }

    }

    public class EntityRelation : IEntityRelation
    {
        protected readonly IDictionary<string, IEntityRelation> NavigationStore = new Dictionary<string, IEntityRelation>();
        protected readonly ICollection<string> ReferenceStore = new HashSet<string>();
        protected readonly IDictionary<string, Type> ReferenceKeysStore = new Dictionary<string, Type>();
        protected readonly IDictionary<string, Type> PrivateNsiReferenceKeysStore = new Dictionary<string, Type>();

        public Type Type { get; private set; }
        public string ParentKey { get; protected set; }
        public IEnumerable<KeyValuePair<string, Type>> ReferenceKeys
        {
            get
            {
                return this.ReferenceKeysStore;
            }
        }
        public IEnumerable<KeyValuePair<string, Type>> PrivateNsiReferenceKeys
        {
            get
            {
                return this.PrivateNsiReferenceKeysStore;
            }
        }
        public IEnumerable<KeyValuePair<string, IEntityRelation>> Navigation
        {
            get
            {
                return this.NavigationStore;
            }
        }
        public IEnumerable<string> References
        {
            get
            {
                return this.ReferenceStore;
            }
        }

        public EntityRelation(Type type)
        {
            this.Type = type;
        }

        private void GetTypes(IEntityRelation entityRelation, ICollection<IEntityRelation> typeList)
        {
            foreach (var child in entityRelation.Navigation)
            {
                this.GetTypes(child.Value, typeList);
            }
            typeList.Add(entityRelation);
        }
        public virtual IEnumerable<IEntityRelation> GetRelations()
        {
            ICollection<IEntityRelation> typeList = new HashSet<IEntityRelation>();
            this.GetTypes(this, typeList);
            return typeList;
        }

        // copyright
        protected static Expression RemoveConvert(Expression expression)
        {
            while (expression != null && (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked))
            {
                expression = RemoveConvert(((UnaryExpression)expression).Operand);
            }
            return expression;
        }
        // copyright
        protected static bool TryParsePath(Expression expression, out string path)
        {
            string str;
            string str1;
            string str2;
            path = null;
            Expression expression1 = RemoveConvert(expression);
            MemberExpression memberExpression = expression1 as MemberExpression;
            MethodCallExpression methodCallExpression = expression1 as MethodCallExpression;
            if (memberExpression != null)
            {
                string name = memberExpression.Member.Name;
                if (!EntityRelation.TryParsePath(memberExpression.Expression, out str))
                {
                    return false;
                }
                path = (str == null ? name : string.Concat(str, ".", name));
            }
            else if (methodCallExpression != null)
            {
                if (methodCallExpression.Method.Name == "Select" && methodCallExpression.Arguments.Count == 2)
                {
                    if (!EntityRelation.TryParsePath(methodCallExpression.Arguments[0], out str1))
                    {
                        return false;
                    }
                    if (str1 != null)
                    {
                        LambdaExpression item = methodCallExpression.Arguments[1] as LambdaExpression;
                        if (item != null)
                        {
                            if (!EntityRelation.TryParsePath(item.Body, out str2))
                            {
                                return false;
                            }
                            if (str2 != null)
                            {
                                path = string.Concat(str1, ".", str2);
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            return true;
        }
    }

    public class EntityRelation<TEntity> : EntityRelation
    {
        public EntityRelation()
            : base(typeof(TEntity))
        {
        }

        //public Expression<Func<TEntity, ICollection<TElement>>> navigationProperty<TEntity, TElement>(
        //    TEntity entity,
        //    TElement element
        //    )
        //{
        //    return new Expression<Func<TEntity, ICollection<TElement>>>();
        //}

        public EntityRelation<TElement> NavigateStr<TElement>(string str)
        {
            //IEntityRelation entityMap;
            //if (!(this.NavigationStore.TryGetValue(str, out entityMap) && entityMap is EntityRelation<TElement>))
            //{
            //    entityMap = new EntityRelation<TElement>();
            //    this.NavigationStore[str] = entityMap;
            //}
            this.NavigationStore[str] = new EntityRelation<TElement>();
            return (EntityRelation<TElement>)this.NavigationStore[str];
        }
        public IEntityRelation NavigateSet<TElement>(string str)
        {
            this.NavigationStore[str] = new EntityRelation<TElement>();
            return (EntityRelation<TElement>)this.NavigationStore[str];
        }

        public EntityRelation<TElement> Navigate<TElement>(
            Expression<Func<TEntity, ICollection<TElement>>> navigationProperty
            )
        {
            string str;
            if (!TryParsePath(navigationProperty.Body, out str) || str == null)
            {
                throw new ArgumentException("navigationProperty");
            }

            IEntityRelation entityMap;
            if (!(this.NavigationStore.TryGetValue(str, out entityMap) && entityMap is EntityRelation<TElement>))
            {
                entityMap = new EntityRelation<TElement>();
                this.NavigationStore[str] = entityMap;
            }

            return (EntityRelation<TElement>)entityMap;
        }
        public EntityRelation<TElement> Navigate<TElement>(Expression<Func<TEntity, TElement>> navigationProperty)
        {
            string str;
            if (!TryParsePath(navigationProperty.Body, out str) || str == null)
            {
                throw new ArgumentException("navigationProperty");
            }

            IEntityRelation entityMap;
            if (!(this.NavigationStore.TryGetValue(str, out entityMap) && entityMap is EntityRelation<TElement>))
            {
                entityMap = new EntityRelation<TElement>();
                this.NavigationStore[str] = entityMap;
            }

            return (EntityRelation<TElement>)entityMap;
        }
        public EntityRelation<TEntity> Reference<TElement>(Expression<Func<TEntity, TElement>> referenceProperty)
        {
            string str;
            if (!TryParsePath(referenceProperty.Body, out str) || str == null)
            {
                throw new ArgumentException("referenceProperty");
            }

            if (!this.ReferenceStore.Contains(str))
            {
                this.ReferenceStore.Add(str);
            }

            return this;
        }
        // временный метод
        public EntityRelation<TEntity> Key(Expression<Func<TEntity, long>> parentKeyProperty)
        {
            string str;
            if (!TryParsePath(parentKeyProperty.Body, out str) || str == null)
            {
                throw new ArgumentException("parentKeyProperty");
            }

            this.ParentKey = str;

            return this;
        }
        // временные методы
        public EntityRelation<TEntity> ReferenceKey<TElement>(Expression<Func<TEntity, long?>> referenceKeyProperty, Expression<Func<TEntity, TElement>> referenceProperty)
        {
            string str;
            if (!TryParsePath(referenceProperty.Body, out str) || str == null)
            {
                throw new ArgumentException("referenceProperty");
            }

            return this.ReferenceKey(referenceKeyProperty, typeof(TElement));
        }
        public EntityRelation<TEntity> ReferenceKey(Expression<Func<TEntity, long?>> referenceKeyProperty, Type referenceType)
        {
            string keyStr;
            if (!TryParsePath(referenceKeyProperty.Body, out keyStr) || keyStr == null)
            {
                throw new ArgumentException("referenceKeyProperty");
            }

            if (referenceType == null)
            {
                throw new ArgumentNullException("referenceType");
            }

            if (!this.ReferenceKeysStore.ContainsKey(keyStr))
            {
                this.ReferenceKeysStore.Add(keyStr, referenceType);
            }

            return this;
        }
        // временные методы
        public EntityRelation<TEntity> PrivateNsiReferenceKey<TElement>(Expression<Func<TEntity, string>> referenceKeyProperty, Expression<Func<TEntity, TElement>> referenceProperty)
        {
            string str;
            if (!TryParsePath(referenceProperty.Body, out str) || str == null)
            {
                throw new ArgumentException("referenceProperty");
            }

            return this.PrivateNsiReferenceKey(referenceKeyProperty, typeof(TElement));
        }
        public EntityRelation<TEntity> PrivateNsiReferenceKey(Expression<Func<TEntity, string>> referenceKeyProperty, Type referenceType)
        {
            string keyStr;
            if (!TryParsePath(referenceKeyProperty.Body, out keyStr) || keyStr == null)
            {
                throw new ArgumentException("referenceKeyProperty");
            }

            if (referenceType == null)
            {
                throw new ArgumentNullException("referenceType");
            }

            if (!this.PrivateNsiReferenceKeysStore.ContainsKey(keyStr))
            {
                this.PrivateNsiReferenceKeysStore.Add(keyStr, referenceType);
            }

            return this;
        }

    }
    #endregion

    public class CommonException : Exception
    {
        public string Code { get; private set; }
        // todo: убрать затычку
        public bool CanRestart
        {
            get
            {
                if (this.Code == PerformServiceError.HCS_SRV_10000.ToString() || this.Code == PerformServiceError.HCS_SRV_20000.ToString())
                {
                    return true;
                }
                if (this.Code != null)
                {
                    string codePrefix = this.Code.Substring(0, 3);
                    return codePrefix == "EXP" || codePrefix == "AUT" || codePrefix == "STR" || codePrefix == "SRC";
                }
                return true;
            }
        }

        public CommonException(string message, string code)
            : base(message)
        {
            this.Code = code;
        }
        public CommonException(string message, string code, Exception innerException)
            : base(message, innerException)
        {
            this.Code = code;
        }
    }

    public enum PerformServiceError
    {
        [Description("Запрос принят.")]
        HCS_SRV_10000,
        [Description("Запрос в обработке.")]
        HCS_SRV_20000,

        [Description("Не удалось сформировать данные для запроса в ГИС ЖКХ.")]
        HCS_DAT_00001,
        [Description("Не удалось прочитать данные, полученные от ГИС ЖКХ.")]
        HCS_DAT_00002,
        [Description("Данные не соответствуют схеме.")]
        HCS_DAT_09999,

        [Description("Не указан ФИАС.")]
        HCS_DAT_10001,
        [Description("Тип дома в ГИС ЖКХ отличается от указанного.")]
        HCS_DAT_10002,
    }

}

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