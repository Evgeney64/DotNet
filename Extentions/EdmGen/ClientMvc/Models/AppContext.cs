using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    public partial class AppContext : DbContext
    {
        string connectionString;
        bool is_postgres;

        #region Constructor
        public AppContext()
        {
        }

        public AppContext(DbContextOptions<AppContext> options)
            : base(options)
        {
        }
        public AppContext(string _connectionString, bool _is_postgres = false)
        {
            connectionString = _connectionString;
            is_postgres = _is_postgres;
            //Database.EnsureCreated();
        }

        public static AppContext CreateContext(string connectionStringName, bool _is_postgres = false)
        {
            if (connectionStringName == null)
            {
                throw new ArgumentNullException("connectionStringName");
            }

            var constructorInfo = typeof(AppContext).GetConstructor(
                new Type[] { typeof(DbContextOptions<AppContext>) }
                );
            if (constructorInfo == null)
            {
                throw new Exception("DbContext должен иметь конструктор с параметром EntityConnection.");
            }

            DbContextOptions<AppContext> contextOptions = null;
            if (_is_postgres == false)
            {
                contextOptions = new DbContextOptionsBuilder<AppContext>()
                    .UseSqlServer(connectionStringName)
                    .Options;
            }
            else
            {
                contextOptions = new DbContextOptionsBuilder<AppContext>()
                    .UseNpgsql(connectionStringName)
                    .Options;
            }

            AppContext context = (AppContext)constructorInfo.Invoke(new object[] { contextOptions });
            return context;
        }
        #endregion

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
    }
}
