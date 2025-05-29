using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Automation.Automation;
using dm.PulseShift.Automation.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

#region commented out code
//class Program
//{
//    static void Main(string[] args)
//    {
//        // Configure the service provider
//        var serviceCollection = new ServiceCollection();
//        ConfigureServices(serviceCollection);
//        var serviceProvider = serviceCollection.BuildServiceProvider();

//        // Initialize the Chrome driver
//        using (var driver = new ChromeDriver())
//        {
//            // Navigate to the desired website
//            driver.Navigate().GoToUrl("https://platform.senior.com.br/senior-x/#/");

//            try
//            {
//                // Locate login and password fields by their IDs (or other selectors)
//                IWebElement loginField = driver.FindElement(By.Id("username-input-field"));
//                loginField.SendKeys("09811120641@spassu.com.br");

//                IWebElement btnProximo = driver.FindElement(By.Id("nextBtn"));
//                btnProximo.Click();
//                IWebElement passwordField = driver.FindElement(By.Id("password-input-field"));
//                IWebElement loginButton = driver.FindElement(By.Id("loginbtn"));

//                // Enter information into the fields
//                passwordField.SendKeys("dTim0r&n01249");

//                // Click the login button
//                loginButton.Click();

//                // Wait a bit to see the result (optional)
//                Thread.Sleep(10000);

//                Console.WriteLine("Login realizado com sucesso");
//                IWebElement menuItemFavoritos = driver.FindElement(By.Id("menu-item-Favoritos"));
//                menuItemFavoritos.Click();
//                IWebElement menuMenuPonto = driver.FindElement(By.Id("apps-menu-item-0"));
//                menuMenuPonto.Click();
//                IWebElement menuMarcarPonto = driver.FindElement(By.Id("apps-menu-item-0"));
//                menuMarcarPonto.Click();

//                Thread.Sleep(5000);

//                // 1. Localize o iframe pelo seu ID ou Name
//                IWebElement iframeElement = driver.FindElement(By.Id("custom_iframe")); // Ou By.Name("ci")

//                // 2. Mude o contexto do driver para o iframe
//                driver.SwitchTo().Frame(iframeElement);

//                IWebElement btnRegistrarPonto = driver.FindElement(By.Id("btn-clocking-event-202410231016"));
//                btnRegistrarPonto.Click();

//                // Use the service provider to get the ITimeEntryAppService
//                var timeEntryAppService = serviceProvider.GetService<ITimeEntryAppService>();
//                var timeEntry = timeEntryAppService.CreateAsync().Result;
//            }
//            catch (NoSuchElementException ex)
//            {
//                Console.WriteLine($"Erro: Elemento não encontrado na página: {ex.Message}");
//            }
//            catch (NoSuchFrameException ex)
//            {
//                Console.WriteLine($"Erro: Iframe não encontrado: {ex.Message}");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Ocorreu um erro: {ex.Message}");
//            }
//            finally
//            {
//                // Close the browser when finished
//                driver.Quit();
//            }
//        }

//        Console.WriteLine("Pressione qualquer tecla para sair...");
//        Console.ReadKey();
//    }

//    private static void ConfigureServices(IServiceCollection services)
//    {
//        // Register your services here
//        services.AddTransient<ITimeEntryAppService, TimeEntryAppService>();
//    }
//}
#endregion

var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

var serviceCollection = new ServiceCollection();
ConfigureServices(serviceCollection);
var serviceProvider = serviceCollection.BuildServiceProvider();

while (true)
{
    if (IsRightToRunAutomation())
    {
        using var driver = new ChromeDriver();
        var automationService = new TimeEntryAutomationService(
            serviceProvider.GetService<ITimeEntryAppService>(),
            driver
        );
        await automationService.ExecuteAsync();
    }
    else
    {
        Console.WriteLine("Not the right time to run automation.");
    }
    var timeUntilNextExec = DecideTheNextExecution();
    Console.WriteLine($"Next execution in {timeUntilNextExec} seconds.");
    await Task.Delay(TimeSpan.FromSeconds(timeUntilNextExec));
}

static void ConfigureServices(IServiceCollection services)
{
    services.RegisterServices();
    services.AddTransient<IWebDriver>(provider => new ChromeDriver());
}

static long DecideTheNextExecution()
{
    var serviceCollection = new ServiceCollection();
    var serviceProvider = serviceCollection.BuildServiceProvider();
    var nonWorkingDayAppService = serviceProvider.GetService<IDayOffAppService>();
    var workScheduleAppService = serviceProvider.GetService<IWorkScheduleAppService>();
    var today = DateOnly.FromDateTime(DateTime.Now);
    var isDayOff = nonWorkingDayAppService.IsDayOffAsync(today).Result;

    long timeUntilNextExec;
    if (isDayOff)
    {
        timeUntilNextExec = nonWorkingDayAppService.CalcTimeUntilNextExecAsync(today).Result;
        return timeUntilNextExec;
    }

    timeUntilNextExec = workScheduleAppService.GetTimeForNextExecAsync().Result;
    return SimulateHumanError(timeUntilNextExec);
}

static long SimulateHumanError(long time)
{
    var marginOfError = (int)TimeSpan.FromMinutes(5).TotalSeconds;
    var rnd = new Random();
    var randomError = rnd.Next(marginOfError * -1, marginOfError);
    return time + randomError;
}

static bool IsRightToRunAutomation()
{
    var serviceCollection = new ServiceCollection();
    var serviceProvider = serviceCollection.BuildServiceProvider();
    var nonWorkingDayAppService = serviceProvider.GetService<IDayOffAppService>();
    var workScheduleAppService = serviceProvider.GetService<IWorkScheduleAppService>();
    var today = DateOnly.FromDateTime(DateTime.Now);
    var timeUntilNextExec = workScheduleAppService.GetTimeForNextExecAsync().Result;
    var isDayOff = nonWorkingDayAppService.IsDayOffAsync(today).Result;

    if (isDayOff) return false;
    if (timeUntilNextExec < 5 * 60) return true; // 5 minutes
    return false;
}