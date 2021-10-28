using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NiceHash.Core.Config;
using NiceHash.Core.Services;

namespace NiceHash.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddNiceHash(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            NiceHashConfig apiConfig = new()
            {
                // Need to manually parse atm as IConfiguration.GetSection("NiceHashApi").Get<ApiConfig>() seems to be bugged.
                BaseUri = new Uri(configuration["NiceHashApi:BaseUri"]),
                OrganizationId = configuration["NiceHashApi:OrganizationId"],
                ApiKey = configuration["NiceHashApi:ApiKey"],
                ApiSecret = configuration["NiceHashApi:ApiSecret"]
            };

            return serviceCollection.AddNiceHash(apiConfig);
        }

        public static IServiceCollection AddNiceHash(this IServiceCollection serviceCollection, NiceHashConfig apiConfig)
            => serviceCollection
                .AddTransient<INiceHashService, NiceHashService>()
                .AddTransient<IRigsManagementService, RigsManagementService>()
                .AddTransient<IWalletService, WalletService>()
                .AddTransient<IConfigProvider, DefaultConfigProvider>()
                .AddSingleton(apiConfig)
                .AddHttpClient()
                .AddMemoryCache();
    }
}
