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
                ApiSecret = configuration["NiceHashApi:ApiSecret"],
                FreeCurrencyApiKey = configuration["NiceHashApi:FreeCurrencyApiKey"],
                MainCurrency = configuration["NiceHashApi:MainCurrency"]
            };

            return serviceCollection.AddNiceHash(apiConfig);
        }

        public static IServiceCollection AddNiceHash(this IServiceCollection serviceCollection, NiceHashConfig apiConfig)
            => serviceCollection
                .AddSingleton(apiConfig)
                .AddSingleton<IConfigProvider, DefaultConfigProvider>()
                .AddNiceHashWithoutConfig();

        public static IServiceCollection AddNiceHash(this IServiceCollection serviceCollection, IConfigProvider configProvider)
            => serviceCollection
                .AddSingleton(configProvider)
                .AddNiceHashWithoutConfig();

        public static IServiceCollection AddNiceHash<TConfigProvider>(this IServiceCollection serviceCollection)
            where TConfigProvider : class, IConfigProvider
            => serviceCollection
                .AddTransient<IConfigProvider, TConfigProvider>()
                .AddNiceHashWithoutConfig();

        public static IServiceCollection AddNiceHashWithoutConfig(this IServiceCollection serviceCollection)
            => serviceCollection
                .AddTransient<INiceHashService, NiceHashService>()
                .AddTransient<IRigsManagementService, RigsManagementService>()
                .AddTransient<IWalletService, WalletService>()
                .AddTransient<ICurrencyExchangeService, CurrencyExchangeService>()
                .AddHttpClient()
                .AddMemoryCache();
    }
}
