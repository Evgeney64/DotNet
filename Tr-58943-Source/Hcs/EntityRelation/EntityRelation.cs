using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel;

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
