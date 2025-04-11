using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Text;

namespace dm.PulseShift.Automation;

public static class DependencyInjectionRegistry
{
    public static void RegisterAutomationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<WebDriverSettings>(configuration.GetSection("WebDriverSettings"));
        services.Configure<ApplicationSettings>(configuration.GetSection("ApplicationSettings"));
        services.Configure<Credentials>(configuration.GetSection("Credentials"));

        services.AddScoped<IWebDriver>(provider =>
        {
            var webDriverSettings = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<WebDriverSettings>>().Value;
            var logger = provider.GetRequiredService<ILogger<IWebDriver>>(); // Log para o WebDriver
                                                                             // Lógica de criação do WebDriver (Chrome, Firefox, etc.) como antes...
            IWebDriver driver;
            switch (webDriverSettings.BrowserType.ToLowerInvariant())
            {
                case "firefox":
                    // ... firefox setup ...
                    driver = new FirefoxDriver(/*...*/);
                    break;
                case "chrome":
                default:
                    var chromeOptions = new ChromeOptions();
                    chromeOptions.AddArgument("--start-maximized");
                    // chromeOptions.AddArgument("--headless"); // Descomente para rodar sem UI
                    logger.LogInformation("Creating Chrome Driver instance for console app.");
                    driver = new ChromeDriver(webDriverSettings.DriverPath ?? ".", chromeOptions);
                    break;
            }
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(webDriverSettings.PageLoadTimeoutSeconds);
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(webDriverSettings.ScriptTimeoutSeconds);
            return driver;
        });
    }
}