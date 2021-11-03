using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

using Hcs.Model;
using System.Linq;
using System.Reflection;

namespace Hcs.Stores
{
    public partial class HcsContext : DbContext
    {
        //string connectionString;
        //bool is_postgres = false;
        //public HcsContext(string _connectionString, bool _is_postgres = false)
        //{
        //    connectionString = _connectionString;
        //    is_postgres = _is_postgres;
        //    //Database.EnsureCreated();
        //}
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    if (is_postgres)
        //        modelBuilder.HasDefaultSchema("gis_hcs");
        //    base.OnModelCreating(modelBuilder);
        //}

        public int SaveChanges(Guid transactionGuid)
        {
            foreach (var entry in this.ChangeTracker.Entries())
            {
                if (entry.Entity is ITransactionEntity)
                {
                    if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                    {
                        ((ITransactionEntity)entry.Entity).TransactionGUID = transactionGuid;
                    }
                }
            }
            return base.SaveChanges();
        }
        public async Task<int> SaveChangesAsync(Guid transactionGuid)
        {
            foreach (var entry in this.ChangeTracker.Entries())
            {
                if (entry.Entity is ITransactionEntity)
                {
                    if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                    {
                        ((ITransactionEntity)entry.Entity).TransactionGUID = transactionGuid;
                    }
                }
            }
            return await base.SaveChangesAsync();
            //return base.SaveChanges();
        }
        public static HcsContext CreateContext(string connectionStringName, bool _is_postgres = false)
        {
            if (connectionStringName == null)
            {
                throw new ArgumentNullException("connectionStringName");
            }

            var constructorInfo = typeof(HcsContext).GetConstructor(
                new Type[] { typeof(DbContextOptions<HcsContext>) }
                );
            if (constructorInfo == null)
            {
                throw new Exception("DbContext должен иметь конструктор с параметром EntityConnection.");
            }

            DbContextOptions<HcsContext> contextOptions = null;
            if (_is_postgres == false)
            {
                contextOptions = new DbContextOptionsBuilder<HcsContext>()
                    .UseSqlServer(connectionStringName)
                    .Options;
            }
            else
            {
                contextOptions = new DbContextOptionsBuilder<HcsContext>()
                    .UseNpgsql(connectionStringName)
                    .Options;
            }

            HcsContext context = (HcsContext)constructorInfo.Invoke(new object[] { contextOptions });
            return context;
            #region old
            //ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
            //if (connectionStringSettings == null)
            //{
            //    throw new SettingsPropertyNotFoundException();
            //}

            //EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            //entityBuilder.Provider = connectionStringSettings.ProviderName;
            //entityBuilder.ProviderConnectionString = connectionStringSettings.ConnectionString;
            //entityBuilder.Metadata = "res://*/Model.HCSEdm.csdl|res://*/Model.HCSEdm.ssdl|res://*/Model.HCSEdm.msl";
            //Hcs.Stores.HcsContext context = new Hcs.Stores.HcsContext(entityBuilder.ConnectionString);
            //return context;
            #endregion
        }

        
    }
    
    // todo: проверить, переделать
    public static class DbContextExtensions
    {
        // todo: проверить, переделать
        public static IQueryable Set(this DbContext context, Type setType)
        {
            MethodInfo method = typeof(DbContext).GetMethod(nameof(DbContext.Set), BindingFlags.Public | BindingFlags.Instance);
            method = method.MakeGenericMethod(setType);
            return method.Invoke(context, null) as IQueryable;
        }

        public static IQueryable FromSqlRaw1(this IQueryable source, string sql, params object[] parameters)
        {
            var methodInfo = MethodInfo
                .GetCurrentMethod().DeclaringType
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .First(m => m.Name == "Call_DbSet_FromSqlRaw");
            var genericMethodInfo = methodInfo.MakeGenericMethod(source.ElementType);

            object[] newParameters = (parameters == null)
                ? new object[] { source, sql }
                : new object[] { source, sql, parameters };

            return genericMethodInfo.Invoke(source, newParameters) as IQueryable;
        }

        private static IQueryable Call_DbSet_FromSqlRaw<TEntity>(DbSet<TEntity> source, string sql, params object[] parameters)
            where TEntity : class
            => RelationalQueryableExtensions.FromSqlRaw(source, sql, parameters);
    }
}

