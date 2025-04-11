
using dm.PulseShift.Automation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // context.Configuration contém a config lida de appsettings.json, env vars, etc.
        // Registra todos os serviços da automação definidos no CrossCutting
        services.RegisterAutomationServices(context.Configuration);

        // Registra a classe que contém a lógica principal da nossa aplicação console
        services.AddHostedService<AutomationRunnerService>();
    })
    .Build();

// Executa o host. Ele encontrará e executará o AutomationRunnerService.
await host.RunAsync();