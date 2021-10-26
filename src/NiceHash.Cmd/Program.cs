// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NiceHash.Cmd.Commands;
using NiceHash.Cmd.Infrastructure;
using NiceHash.Core;
using Spectre.Console.Cli;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Local.json", true)
    .AddEnvironmentVariables()
    .Build();

var serviceCollection = new ServiceCollection()
    .AddLogging()
    .AddNiceHash(configuration);

CommandApp app = new(new TypeRegistrar(serviceCollection));
app.Configure(config =>
{
    config.AddCommand<StatsCommand>("stats");
    config.AddCommand<StartRigCommand>("start-rig");
    config.AddCommand<StopRigCommand>("stop-rig");
});

return await app.RunAsync(args);
