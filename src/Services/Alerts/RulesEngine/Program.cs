using MediatR;
using RulesEngine;
using RulesEngine.Configuration;
using RulesEngine.InMemoryDataDI;

IConfiguration configuration = null;
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        configuration = config.Build();
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddRulesEngineConfiguration(configuration);
        services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
        services.AddOptions<MqttConfig>().BindConfiguration(MqttConfig.SectionName);
    })
    .Build();

host.Services.GetRequiredService<IConfiguration>();
host.Run();
