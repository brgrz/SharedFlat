﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SharedFlat.Sample
{
    public sealed class abcTenantServiceProviderConfiguration : ITenantConfiguration
    {
        public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddScoped<Xpto>(sp => new Xpto { Name = this.Tenant });
        }

        public string Tenant { get; } = "abc";
    }
}
