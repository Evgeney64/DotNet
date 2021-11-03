using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

using ServiceLib;
using Server.Core.Types;

namespace ServiceLib
{
    public partial class EntityContext : DbContext
    {
        public EntityContext()
        { }
        public EntityContext(DbContextOptions<EntityContext> options)
            : base(options)
        { }

        string connectionString;
        bool is_postgres;
        public EntityContext(string _connectionString, bool _is_postgres = false)
        {
            connectionString = _connectionString;
            is_postgres = _is_postgres;
            //Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                if (connectionString != null)
                {
                    if (is_postgres == false)
                        optionsBuilder.UseSqlServer(connectionString);
                    else
                        optionsBuilder.UseNpgsql(connectionString);
                }
            }
            catch (Exception ex)
            { }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (connectionString != null)
            {
                if (is_postgres)
                    modelBuilder.HasDefaultSchema("gis_hcs");
            }

            base.OnModelCreating(modelBuilder);
        }

        public int SaveChanges(Guid transactionGuid)
        {
            foreach (var entry in this.ChangeTracker.Entries())
            {
                //if (entry.Entity is ITransactionEntity)
                //{
                //    if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                //    {
                //        ((ITransactionEntity)entry.Entity).TransactionGUID = transactionGuid;
                //    }
                //}
            }
            return base.SaveChanges();
        }
        public async Task<int> SaveChangesAsync(Guid transactionGuid)
        {
            foreach (var entry in this.ChangeTracker.Entries())
            {
                //if (entry.Entity is ITransactionEntity)
                //{
                //    if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                //    {
                //        ((ITransactionEntity)entry.Entity).TransactionGUID = transactionGuid;
                //    }
                //}
            }
            return await base.SaveChangesAsync();
            //return base.SaveChanges();
        }
        public static EntityContext CreateContext(string connectionStringName, bool _is_postgres = false)
        {
            if (connectionStringName == null)
            {
                throw new ArgumentNullException("connectionStringName");
            }

            var constructorInfo = typeof(EntityContext).GetConstructor(
                new Type[] { typeof(DbContextOptions<EntityContext>) }
                );
            if (constructorInfo == null)
            {
                throw new Exception("DbContext должен иметь конструктор с параметром EntityConnection.");
            }

            DbContextOptions<EntityContext> contextOptions = null;
            if (_is_postgres == false)
            {
                contextOptions = new DbContextOptionsBuilder<EntityContext>()
                    .UseSqlServer(connectionStringName)
                    .Options;
            }
            else
            {
                contextOptions = new DbContextOptionsBuilder<EntityContext>()
                    .UseNpgsql(connectionStringName)
                    .Options;
            }

            EntityContext context = (EntityContext)constructorInfo.Invoke(new object[] { contextOptions });
            return context;
        }
    }
}

namespace Server.Core.ViewModel
{
    public partial class VmBase
    {
        DataSourceConfiguration conf;
        protected EntityContext CreateContext()
        {
            var context = EntityContext.CreateContext(conf.ConnectionString, conf.is_postgres);
            //$$$
            //context.ObjectContext.Connection.Open();
            //context.ObjectContext.CommandTimeout = this.Configuration.CommandTimeout;
            AssemblyName assemblyName = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
            string versionNumber = assemblyName.Version.ToString();
            //var sysParams = context.SysParams
            //    .AsNoTracking()
            //    .FirstOrDefault();
            //$$$
            //if (sysParams == null || !String.Equals(sysParams.VersionNumber, versionNumber, StringComparison.Ordinal))
            //{
            //    throw new Exception(String.Format("Текущая версия промежуточной БД не поддерживается. Требуется версия {0}", versionNumber));
            //}

            return context;
        }
    }
}