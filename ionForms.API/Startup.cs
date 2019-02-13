using ionForms.API.Entities;
using ionForms.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace ionForms.API
{
    public class Startup
    {
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("corsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));


            services.AddMvc()
                .AddMvcOptions(o => o.OutputFormatters.Add(
                    new XmlDataContractSerializerOutputFormatter()));

            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

#if DEBUG
            services.AddTransient<IMailService, LocalMailService>();
#else
            services.AddTransient<IMailService, LocalMailService>();
            //services.AddTransient<IMailService, CloudMailService>();
#endif

            var connectionString = Startup.Configuration["connectionStrings:connFDConfig"];
            services.AddDbContext<FDDataContext>(o => o.UseSqlServer(connectionString));

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IFormRepository, FormRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            FDDataContext fdDataContext)
        {
            loggerFactory.AddNLog();
            app.UseCors("corsPolicy");

            //throw new System.Exception("test error");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/error");
                //app.UseHsts();
            }

            //app.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy",
            //        builder => builder.AllowAnyOrigin()
            //        .AllowAnyMethod()
            //        .AllowAnyHeader()
            //        .AllowCredentials());
            //});

            fdDataContext.EnsureSeedDataForContext();
            app.UseStatusCodePages();

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Entities.Account, Models.AccountWithoutFormsDto>();
                cfg.CreateMap<Entities.Account, Models.AccountDto>();
                cfg.CreateMap<Models.AccountForCreateDto, Entities.Account>();
                cfg.CreateMap<Models.AccountForUpdateDto, Entities.Account>();
                cfg.CreateMap<Entities.Account, Models.AccountForUpdateDto>();
                cfg.CreateMap<Entities.Form, Models.FormDto>();
                cfg.CreateMap<Models.FormForCreationDto, Entities.Form>();
                cfg.CreateMap<Models.FormForUpdateDto, Entities.Form>();
                cfg.CreateMap<Entities.Form, Models.FormForUpdateDto>();
                cfg.CreateMap<Entities.Column, Models.ColumnDto>();
                cfg.CreateMap<Models.ColumnForCreationDto, Entities.Column>();
                cfg.CreateMap<Models.ColumnForUpdateDto, Entities.Column>();
                cfg.CreateMap<Entities.Column, Models.ColumnForUpdateDto>();

            });

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}