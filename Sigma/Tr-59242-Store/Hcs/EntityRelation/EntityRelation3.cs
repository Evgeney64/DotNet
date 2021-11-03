using Hcs.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace Hcs
{
    public partial class EntityRelationBuilder
    {
        public IEntityRelation EntitySet<TEntity>()
        {
            IEntityRelation entityRelation;
            if (!entities.TryGetValue(typeof(TEntity), out entityRelation))
            {
                entityRelation = new EntityRelation<TEntity>(this);
                entities[typeof(TEntity)] = entityRelation;
            }

            return entityRelation;
        }

        public List<string> EntityRelations = new List<string>();
        public void EntityRelationSetAllTypes()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(EntityRelationBuilder));

            List<Type> types = assembly.GetTypes()
                .Where(ss => ss.FullName.Contains("Hcs.Model")
                    && ss.FullName.Contains("<>") == false
                    && ss.IsClass
                    && ss.BaseType.FullName == "System.Object"
                    )
                .OrderBy(ss => ss.FullName)
                .ToList();
            foreach (Type type in types)
                EntityRelationSet(type);
        }
        public void EntityRelationSet(Type type)
        {
            MethodInfo method = typeof(EntityRelationBuilder).GetMethod("EntitySet");
            if (method != null)
            {
                MethodInfo methodGen = method.MakeGenericMethod(new[] { type });
                if (methodGen != null)
                {
                    IEntityRelation item = (IEntityRelation)methodGen.Invoke(this, new object[] { });
                    entityNavigationRecurce(item, type, 0);
                }
            }
        }
        private void entityNavigationRecurce(IEntityRelation item, Type type, int step)
        {
            foreach (PropertyInfo prop in type.GetProperties()
                .Where(ss => ss.CustomAttributes
                    .Where(ss1 => ss1.AttributeType.Name == "InversePropertyAttribute").Count() > 0)
                )
            {
                // необязательное ограничение рекурсии
                if (step >= 10)
                    break;

                if (EntityRelations.Contains(prop.Name))
                    continue;

                Type type1 = prop.PropertyType;
                if (prop.PropertyType.GetGenericArguments().Count() == 1)
                    type1 = prop.PropertyType.GetGenericArguments().Single();

                MethodInfo method1 = item.GetType().GetMethod("NavigateSet");
                if (method1 != null)
                {
                    MethodInfo methodGen1 = method1.MakeGenericMethod(new[] { type1 });
                    if (methodGen1 != null)
                    {
                        IEntityRelation item1 = (IEntityRelation)methodGen1.Invoke(item, new object[] { prop.Name });
                        EntityRelations.Add(prop.Name);

                        // необязательное ограничение рекурсии
                        //if (step < 10)
                        {
                            entityNavigationRecurce(item1, type1, step + 1);
                        }
                    }
                }
            }

        }
    }
}
