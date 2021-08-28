using System;
using System.IO;
using AutoMapper;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHibernate.NetCore;
using Smartflow.Common;
using Smartflow.API.Code;
using Smartflow.API.Profile;

namespace Smartflow.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            GlobalObjectService.Configuration = Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(e =>
            {
                e.Filters.Add(typeof(ApiControllerException));
                e.Filters.Add(typeof(ArgumentCheckAttribute));
            })
                .AddNewtonsoftJson(option =>
            {
                option.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                // 忽略循环引用
                option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                // 设置时间格式
                option.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                // 如字段为null值，该字段不会返回到前端
                // options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; 
            });

            Ioc.RegisterService(services);
            WorkflowGlobalService.RegisterService();
            services.AddAutoMapper((mapper) => mapper.AddProfile(typeof(SmartflowProfile)));
            XmlConfigurator.Configure(LogManager.CreateRepository(GlobalObjectService.Configuration.GetSection("Logging:Program").Value), new FileInfo("log4net.config"));
            services.AddHibernate(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"hibernate.cfg.xml"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors((cors) =>
            {
                cors.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });

            app.UseRouting();
            app.UseStaticFiles();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
