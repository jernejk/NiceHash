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
    config.SetApplicationVersion("1.0");
    config.ValidateExamples();

    config.AddCommand<StatsCommand>("stats")
        .WithDescription("Get wallet and rigs status.")
        .WithExample(new[] { "stats" });

    config.AddCommand<StartRigCommand>("start-rig")
        .WithDescription("Start all rigs.")
        .WithExample(new[] { "start-rig" });

    config.AddCommand<StopRigCommand>("stop-rig")
        .WithDescription("Stop all rigs.")
        .WithExample(new[] { "stop-rig" });
});

return await app.RunAsync(args);
