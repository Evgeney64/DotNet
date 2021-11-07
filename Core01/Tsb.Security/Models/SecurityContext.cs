using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Tsb.Security.Web.Models
{
    public partial class SecurityContext : DbContext
    {
        public SecurityContext()
        { }
        public SecurityContext(DbContextOptions<SecurityContext> options)
            : base(options)
        { }

        string connectionString;
        bool is_postgres;
        string postgresSchema;
        public SecurityContext(string _connStr, bool _isPostgr = false, string _postgrSchem = "gis_hcs")
        {
            connectionString = _connStr;
            is_postgres = _isPostgr;
            postgresSchema = _postgrSchem;
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
                    modelBuilder.HasDefaultSchema(postgresSchema);
            }

            base.OnModelCreating(modelBuilder);
        }

        public static SecurityContext CreateContext(string connectionStringName, bool _is_postgres = false)
        {
            if (connectionStringName == null)
            {
                throw new ArgumentNullException("connectionStringName");
            }

            var constructorInfo = typeof(SecurityContext).GetConstructor(
                new Type[] { typeof(DbContextOptions<SecurityContext>) }
                );
            if (constructorInfo == null)
            {
                throw new Exception("DbContext должен иметь конструктор с параметром EntityConnection.");
            }

            DbContextOptions<SecurityContext> contextOptions = null;
            if (_is_postgres == false)
            {
                contextOptions = new DbContextOptionsBuilder<SecurityContext>()
                    .UseSqlServer(connectionStringName)
                    .Options;
            }
            else
            {
                contextOptions = new DbContextOptionsBuilder<SecurityContext>()
                    .UseNpgsql(connectionStringName)
                    .Options;
            }

            SecurityContext context = (SecurityContext)constructorInfo.Invoke(new object[] { contextOptions });
            return context;
        }

		#region Save
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
        #endregion
    }
}
