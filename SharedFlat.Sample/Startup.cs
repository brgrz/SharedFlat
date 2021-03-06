﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedFlat.Sample.Models;

namespace SharedFlat.Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddTenantService()
                .AddTenantMiddleware()  //not used, can be omitted
                .AddTenantLocations()   //for Razor views
                .AddTenantIdentification()
                    //.TenantForHeader()
                    //.DynamicTenant(x => "abc", new [] { "abc", "xyz" })
                    //.TenantForQueryString()
                    //.TenantForSourceIP()
                    .TenantForHost(options =>
                    {
                        this.Configuration.BindTenantsMapping(options.Mapping);
                    })
                    //.StaticTenant("abc")
                .AddTenantDbContextIdentitication()
                    //.Dummy();
                    .FilterByTenant();
            
            services.AddTenantConfiguration();

            services.AddHttpContextAccessor();

            services.AddDbContext<BlogContext>((serviceProvider, options) =>
            {
                //options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"));
                options
                    .UseInMemoryDatabase("Blog");
                    //.UseInternalServiceProvider(serviceProvider);
            });

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddRouting();

            services.Configure<PerTenantSettings>("abc", options =>
            {
                this.Configuration.BindTenantConfiguration("abc", options);
            });

            services.Configure<PerTenantSettings>("abc", options =>
            {
                options.NumberOption = 1;
                options.StringOption = "abc";
            });

            services.Configure<PerTenantSettings>("xyz", options =>
            {
                options.NumberOption = 2;
                options.StringOption = "xyz";
            });

            services.ConfigureOptions<PreConfigurePerTenantSettings>();
            services.ConfigureOptions<PostConfigurePerTenantSettings>();
            services.PostConfigure<PerTenantSettings>(options =>
            {
            });
            services.PostConfigureAll<PerTenantSettings>(options =>
            {
            });
            services.PostConfigureAll<PerTenantSettings>(options =>
            {
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseTenants();   //not used, can be omitted

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();             
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
