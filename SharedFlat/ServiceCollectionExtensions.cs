﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace SharedFlat
{
    public static class ServiceCollectionExtensions
    {
        public static TenantIdentification AddTenantIdentification(this IServiceCollection services)
        {
            return new TenantIdentification(services);
;        }

        public static IServiceCollection AddTenantLocations(this IServiceCollection services)
        {
            return services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Insert(0, new TenantViewLocationExpander());
            });
        }

        public static IServiceCollection AddTenantService(this IServiceCollection services)
        {
            return services
                .AddHttpContextAccessor()
                .AddScoped<ITenantService, TenantService>();
        }

        public static IServiceCollection AddQueryStringIdentificationService(this IServiceCollection services, string tenantKey = nameof(TenantService.Tenant))
        {
            return services.AddSingleton<ITenantIdentificationService, QueryStringTenantIdentificationService>(sp => new QueryStringTenantIdentificationService(sp.GetRequiredService<IConfiguration>(), tenantKey));
        }

        public static IServiceCollection AddHostIdentificationService(this IServiceCollection services)
        {
            return services.AddSingleton<ITenantIdentificationService, HostTenantIdentificationService>();
        }

        public static DbContextIdentification AddTenantDbContextIdentitication(this IServiceCollection services)
        {
            return new DbContextIdentification(services);
        }

        public static IServiceCollection AddTenantMiddleware(this IServiceCollection services)
        {
            return services.AddSingleton<IStartupFilter, TenantStartupFilter>();
        }
    }
}
