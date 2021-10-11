using System;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Infrastructure;
//using Microsoft.EntityFrameworkCore.Metadata;
using System.Configuration;
//using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

using Hcs.Model;

namespace Hcs.Model
{
    public partial class HcsContext// : DbContext
    {
        //public static HcsContext CreateContext(string connectionStringName)
        //{
        //    if (connectionStringName == null)
        //    {
        //        throw new ArgumentNullException("connectionStringName");
        //    }

        //    var constructorInfo = typeof(HcsContext).GetConstructor(
        //        new Type[] { typeof(DbContextOptions<HcsContext>) }
        //        );
        //    if (constructorInfo == null)
        //    {
        //        throw new Exception("DbContext должен иметь конструктор с параметром EntityConnection.");
        //    }
        //    DbContextOptions contextOptions = new DbContextOptionsBuilder()
        //        .UseOracle(connectionStringName)
        //        .Options;

        //    HcsContext context = (HcsContext)constructorInfo.Invoke(new object[] { contextOptions });
        //    return context;
        //}
    }
}
