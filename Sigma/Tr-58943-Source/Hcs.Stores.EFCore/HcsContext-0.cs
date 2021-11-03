using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

using Hcs.Model;

namespace Hcs.Stores
{
    public partial class HcsContext : DbContext
    {
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



        public static HcsContext CreateContext(string connectionStringName)
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

            DbContextOptions<HcsContext> contextOptions = new DbContextOptionsBuilder<HcsContext>()
                .UseSqlServer(connectionStringName)
                .Options;

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
}

