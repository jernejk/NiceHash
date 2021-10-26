// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NiceHash.Core;

Console.WriteLine("Hello, World!");

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Local.json", true)
    .AddEnvironmentVariables()
    .Build();

// NOTE: A new bug introduced in .NET 5? This is always and Get<ApiConfig> fails.
//var config = configuration.GetSection("NiceHashApi");

var serviceProvider = new ServiceCollection()
    .AddLogging()
    .AddNiceHash(configuration)
    .BuildServiceProvider();

// Configure console logging
serviceProvider.GetService<ILoggerFactory>();

var logger = serviceProvider
    .GetService<ILoggerFactory>()
    .CreateLogger<Program>();

logger.LogDebug("Starting application");

//do the actual work here
var walletService = serviceProvider.GetService<IWalletService>();

var wallet = await walletService.GetWalletCurrencies();
Console.WriteLine($"Balance: {wallet.Currencies[0].Available} {wallet.Currencies[0].Currency}");

var rigsManagementService = serviceProvider.GetService<IRigsManagementService>();
var workers = await rigsManagementService.GetActiveWorkers();

foreach (var worker in workers.Workers)
{
    Console.WriteLine($"Worker: {worker.RigName}; Speed: {worker.SpeedAccepted}; Profitability: {worker.Profitability}");
}

var rigs = await rigsManagementService.GetRigs();

//foreach (var rig in rigs.MiningRigs)
//{
//    Console.WriteLine($"Rig: {rig.Name} ({rig.RigId}); Status: {rig.MinerStatus}; Power: {rig.Devices.Where(x => x.PowerUsage > 0).Sum(x => x.PowerUsage)}");

//    if (rig.MinerStatus == "MINING")
//    {
//        var a = await rigsManagementService.StopRig(rig.RigId);
//    }
//    else
//    {
//        var a = await rigsManagementService.StartRig(rig.RigId);
//    }
//}
