using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Server.Core;

namespace ru.tsb.mvc
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<EntityContext>(options =>
            //    options.UseSqlServer(Configuration["Data:renovation_web:ConnectionString"]));
            //services.AddTransient<ICoreEdm, CoreEdm>();

            services.AddControllersWithViews();

            //services.AddDbContext<AppidentityDbContext>(options => 
            //    options.UseSqlServer(Configuration["Data:auth:ConnectionString"]));
            
            //services.AddIdentity<scr_user, IdentityRole>()
            //    .AddEntityFrameworkStores<AppidentityDbContext>()
            //    .AddDefaultTokenProviders();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region Common
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            #endregion

            #region Services
            // ************************************************************************
            #region 01 - Run()
            if (1 == 2)
            {
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Hello World !");
                });
            }
            #endregion

            #region 02 - Use() + Run() (1)
            if (1 == 2)
            {
                int x = 5;
                int y = 8;
                int z = 0;
                app.Use(async (context, next) =>
                {
                    z = x * y;
                    await next.Invoke();
                });

                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync($"x * y = {z}");
                });
            }
            #endregion

            #region 03 - () + Run() (2)
            if (1 == 2)
            {
                int x = 2;
                app.Use(async (context, next) =>
                {
                    x = x * 2;      // 2 * 2 = 4
                    await next.Invoke();    // вызов app.Run
                    x = x * 2;      // 8 * 2 = 16
                    await context.Response.WriteAsync($"Result: {x}");
                });

                app.Run(async (context) =>
                {
                    x = x * 2;  //  4 * 2 = 8
                    await Task.FromResult(0);
                });
            }
            #endregion

            #region 04 - Map()
            if (1 == 2)
            {
                app.Map("/index", Index);
                app.Map("/Home/about", About);
                //app.Map("/home", home =>
                //{
                //    home.Map("/index", Index);
                //    home.Map("/about", About);
                //});

                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Page Not Found");
                });
            }
            #endregion

            #region 05 - TokenMiddleware
            if (1 == 2)
            {
                // http://localhost:58982/Home/about?token=12345678
                app.UseMiddleware<TokenMiddleware>();

                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Page Not Found");
                });
            }
            #endregion

            #region 06 - TokenExtensions
            if (1 == 2)
            {
                // http://localhost:58982/Home/about?token=12345678
                app.UseToken();

                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Page Not Found");
                });
            }
            #endregion

            #region 07 - TokenParamExtensions
            if (1 == 2)
            {
                // http://localhost:58982/Home/about?token=555555
                app.UseTokenParam("555555");

                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Hello World");
                });
            }
            #endregion

            #region 08 - Authentication
            if (1 == 2)
            {
                // http://localhost:58982/about
                // http://localhost:58982/about?token=555555
                app.UseMiddleware<AuthenticationMiddleware>();
                app.UseMiddleware<RoutingMiddleware>();
            }
            #endregion

            #region 09 - ErrorHandling
            if (1 == 2)
            {
                // http://localhost:58982/about
                // http://localhost:58982/about?token=555555
                app.UseMiddleware<ErrorHandlingMiddleware>();
                app.UseMiddleware<AuthenticationMiddleware>();
                app.UseMiddleware<RoutingMiddleware>();
            }
            #endregion

            #region 10 - IWebHostEnvironment
            if (1 == 2)
            { 
                if (env.ApplicationName != null             // имя приложения
                    && env.EnvironmentName != null          // описание среды
                    && env.ContentRootPath != null          // путь к корневой папке приложения
                    && env.WebRootPath != null              // путь к папке wwwroot (статический контент)
                    && env.ContentRootFileProvider != null  // реализацию интерфейса Microsoft.AspNetCore.FileProviders.IFileProvider
                    && env.WebRootFileProvider != null      // реализацию интерфейса Microsoft.AspNetCore.FileProviders.IFileProvider
                    )
                { }
            }
            #endregion
            // ************************************************************************
            #endregion
        }

    }
}
