using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace Hcs
{
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

            entityNavigationRecurce(item, type, 0);
        }
        private void entityNavigationRecurce(IEntityRelation item, Type type, int step)
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

                // необязательное ограничение рекурсии
                if (step < 10)
                {
                    entityNavigationRecurce(item1, type1, step + 1);
                }
            }

        }

    }
}
