using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Hcs.Store
{
    public partial class Public
    {
        DataSourceConfiguration conf;
        public Public(DataSourceConfiguration _conf)
        {
            conf = _conf;
        }

        public string GetSysOperation()
        {
            //using (ApplicationContext context = new ApplicationContext(conf.ConnectionString, conf.is_postgres))
            using (ApplicationContext context = this.CreateContext())
            {
                string str = "";
                IQueryable<SysOperation> sopr = context.SysOperation;
                if (sopr != null)
                {
                    try
                    {
                        List<SysOperation> sysOperations = null;
                        try
                        {
                            sysOperations = sopr.ToList();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Ошибка");
                        }
                        foreach (SysOperation sop in sysOperations)
                        {
                            str += "<p>" + sop.OperationId + " - " + sop.OperationName + "</p>";
                        }
                    }
                    catch (Exception ex)
                    { }
                }
                return str;
            }
        }
        public string GetSysTransaction()
        {
            //using (ApplicationContext context = new ApplicationContext(conf.ConnectionString, conf.is_postgres))
            using (ApplicationContext context = this.CreateContext())
            {
                string str = "";
                DateTime date = new DateTime(2021, 11, 1);
                IQueryable<SysTransaction> items = context.SysTransaction.Where(ss => ss.StartDate >= date);
                if (items != null)
                {
                    try
                    {
                        List<SysTransaction> itemsL = items.ToList();
                        { }
                        foreach (SysTransaction sop in itemsL)
                        {
                            str += "<p>" + sop.TransactionGUID + "</p>";
                        }

                        #region ChangeTracker
                        if (1 == 1)
                        {
                            int i = 0;
                            foreach (SysTransaction sop in itemsL)
                            {
                                sop.ResultId = i;
                                i++;
                                if (i > 5)
                                    break;
                            }
                            context.SysTransaction.Add(new SysTransaction
                            {
                                TransactionGUID = Guid.NewGuid(),
                                ResultId = -1,
                            });
                            SysTransaction sopLast = itemsL.LastOrDefault();
                            context.SysTransaction.Remove(sopLast);

                            List<EntityEntry> entries = context.ChangeTracker.Entries()
                                .Where(ss => ss.State != EntityState.Unchanged)
                                .ToList();
                            { }
                            foreach (EntityEntry entry in entries)
                            {
                                if (entry.Entity is SysTransaction)
                                {
                                    ((SysTransaction)entry.Entity).TransactionGUID = Guid.NewGuid();
                                }
                            }
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    { }
                }
                return str;
            }
        }

        protected ApplicationContext CreateContext()
        {
            var context = ApplicationContext.CreateContext(conf.ConnectionString, conf.is_postgres);
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


