using dm.PulseShift.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

public class AutomationRunnerService: IHostedService
{
    private readonly ILogger<AutomationRunnerService> _logger;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly IServiceProvider _serviceProvider;
    public AutomationRunnerService(
            ILogger<AutomationRunnerService> logger, IHostApplicationLifetime lifetime, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _lifetime = lifetime;
        _serviceProvider = serviceProvider;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Automation Runner Service starting.");

        // Registra um callback para quando a aplicação estiver iniciando
        _lifetime.ApplicationStarted.Register(() =>
        {
            // Executa a lógica principal em uma Task separada para não bloquear o StartAsync
            Task.Run(async () =>
            {
                IWebDriver? driver = null; // Manter referência para Quit explícito em caso de erro
                try
                {
                    _logger.LogDebug("Creating DI Scope for automation task.");
                    // Criar um escopo DI para esta execução.
                    // Serviços Scoped (como IWebDriver, Repositories) serão criados aqui.
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var scopedProvider = scope.ServiceProvider;

                        // Obter serviços dentro do escopo
                        driver = scopedProvider.GetService<IWebDriver>(); // Obter para Quit explícito
                        if (driver == null) throw new InvalidOperationException("IWebDriver could not be resolved.");

                        var authService = scopedProvider.GetRequiredService<IAuthenticationWebSiteSeniorXAppService>();
                        var credentials = scopedProvider.GetRequiredService<IOptions<TestCredentials>>().Value;

                        _logger.LogInformation("Attempting login via Console App for user: {username}", credentials.Username);

                        // Executar a lógica de login
                        bool success = authService.Login(credentials.Username, credentials.Password);

                        if (success)
                        {
                            _logger.LogInformation("Login successful!");
                            // --- Aqui você pode adicionar mais chamadas a outros AppServices ---
                            // Ex: var navigationService = scopedProvider.GetRequiredService<INavigationAppService>();
                            //     navigationService.NavigateToModule("Financeiro");
                            // -----------------------------------------------------------------
                        }
                        else
                        {
                            _logger.LogError("Login failed.");
                        }
                    } // Fim do using(scope) - Serviços Scoped (IWebDriver) serão descartados aqui.
                    _logger.LogDebug("DI Scope disposed.");

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during automation execution.");
                    // Tentar fechar o driver explicitamente se ainda existir e o Dispose do escopo falhar
                    if (driver != null)
                    {
                        _logger.LogWarning("Attempting explicit WebDriver Quit due to error.");
                        try { driver.Quit(); } catch (Exception qEx) { _logger.LogError(qEx, "Error during explicit WebDriver Quit."); }
                    }
                }
                finally
                {
                    // Sinaliza para o Host que a aplicação pode terminar
                    _logger.LogInformation("Automation task finished. Stopping application.");
                    _appLifetime.StopApplication();
                }
            });
        });

        return Task.CompletedTask; // StartAsync retorna rapidamente
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Aqui você pode fazer a limpeza necessária antes de parar a aplicação
        return Task.CompletedTask;
    }
}