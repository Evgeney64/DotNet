using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

using Server.Core.Context;
using Server.Core.Model;

namespace Server.Core.Context
{
    public partial class EntityContext : DbContext
    {
        #region Define
        string connectionString;
        bool is_postgres;
        string postgresSchema;
        #endregion

        #region Constructor
        public EntityContext()
        { }
        public EntityContext(DbContextOptions<EntityContext> options)
            : base(options)
        { }
        public EntityContext(string _connStr, bool _isPostgr = false, string _postgrSchem = "gis_hcs")
        {
            connectionString = _connStr;
            is_postgres = _isPostgr;
            postgresSchema = _postgrSchem;
            //Database.EnsureCreated();
        }
        #endregion

        #region OnConfiguring
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            #region
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
            #endregion
        }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region
            if (connectionString != null)
            {
                if (is_postgres)
                    modelBuilder.HasDefaultSchema(postgresSchema);
            }

            modelBuilder.Entity<payerlive>()
                .HasOne(u => u.Partners2)
                .WithMany(t => t.payerlive2)
                .HasForeignKey(t => t.reciever_id)
                ;

            modelBuilder.Entity<payerlive>()
                .HasOne(u => u.Partners3)
                .WithMany(t => t.payerlive3)
                .HasForeignKey(t => t.payer_id)
                ;

            //builder.Entity<Team>().HasMany(t => t.TeamMebers).WithOne(u => u.Team).HasForeignKey(u => u.ID);
            //builder.Entity<User>().HasOne(u => u.Team).WithMany(t => t.TeamMebers).HasForeignKey(t => t.ID);
            //modelBuilder.Entity<BUILD>().ToTable("BUILD");
            //modelBuilder.Entity<NSI_VILLAGE>().ToTable("Users");
            //modelBuilder.Entity<NSI_VILLAGE>().ToTable("Logs");

            base.OnModelCreating(modelBuilder);
            #endregion
        }

        #region CreateContext
        public static EntityContext CreateContext(string connectionStringName, bool _is_postgres = false)
        {
            #region
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
            #endregion
        }
        #endregion

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

#region old
//namespace Server.Core.ViewModel
//{
//    public partial class VmBase
//    {
//        DataSourceConfiguration conf;
//        protected EntityContext CreateContext()
//        {
//            var context = EntityContext.CreateContext(conf.ConnectionString, conf.is_postgres);
//            //$$$
//            //context.ObjectContext.Connection.Open();
//            //context.ObjectContext.CommandTimeout = this.Configuration.CommandTimeout;
//            AssemblyName assemblyName = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
//            string versionNumber = assemblyName.Version.ToString();
//            //var sysParams = context.SysParams
//            //    .AsNoTracking()
//            //    .FirstOrDefault();
//            //$$$
//            //if (sysParams == null || !String.Equals(sysParams.VersionNumber, versionNumber, StringComparison.Ordinal))
//            //{
//            //    throw new Exception(String.Format("Текущая версия промежуточной БД не поддерживается. Требуется версия {0}", versionNumber));
//            //}

//            return context;
//        }
//    }
//}
#endregion