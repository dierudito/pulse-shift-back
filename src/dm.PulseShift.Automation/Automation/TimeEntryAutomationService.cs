using dm.PulseShift.Application.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace dm.PulseShift.Automation.Automation;

public class TimeEntryAutomationService(ITimeEntryAppService timeEntryAppService, IWebDriver driver)
{
    private bool _pointRegistrationFailed = false;
    private const int DefaultTimeoutSeconds = 10;
    private readonly WebDriverWait _wait = new(driver, TimeSpan.FromSeconds(DefaultTimeoutSeconds));

    public async Task ExecuteAsync()
    {
        try
        {
            await NavigateToLoginPageAsync();
            await LoginAsync("09811120641@spassu.com.br", "dTim0r&n01249");
            await NavigateToTimeEntryPageAsync();
            await RegisterTimeEntryAsync();

            if (!_pointRegistrationFailed)
            {
                var timeEntry = await timeEntryAppService.CreateAsync();
                Console.WriteLine($"Time entry created successfully with ID: {timeEntry?.Data?.Id}");
            }
            else
            {
                DisplayCriticalFailureMessage();
            }
        }
        catch (NoSuchElementException ex)
        {
            HandleAutomationError($"Element not found on the page: {ex.Message}");
        }
        catch (NoSuchFrameException ex)
        {
            HandleAutomationError($"Iframe not found: {ex.Message}");
        }
        catch (WebDriverException ex)
        {
            HandleAutomationError($"Error during browser automation: {ex.Message}");
        }
        catch (Exception ex)
        {
            HandleAutomationError($"An unexpected error occurred: {ex.Message}");
        }
        finally
        {
            driver.Quit();
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private async Task NavigateToLoginPageAsync()
    {
        driver.Navigate().GoToUrl("https://platform.senior.com.br/senior-x/#/");
        await Task.Delay(TimeSpan.FromSeconds(2));
        ValidatePageTransition(driver.Url.StartsWith("https://platform.senior.com.br/login"), "Navigation to login page failed.");
    }

    private async Task LoginAsync(string username, string password)
    {
        IWebElement loginField = driver.FindElement(By.Id("username-input-field"));
        loginField.SendKeys(username);

        IWebElement nextButton = driver.FindElement(By.Id("nextBtn"));
        nextButton.Click();
        await Task.Delay(TimeSpan.FromSeconds(2));
        ValidatePageTransition(driver.FindElement(By.Id("password-input-field")).Displayed, "Transition to password input failed.");

        IWebElement passwordField = driver.FindElement(By.Id("password-input-field"));
        IWebElement loginButton = driver.FindElement(By.Id("loginbtn"));

        passwordField.SendKeys(password);
        loginButton.Click();
        await Task.Delay(TimeSpan.FromSeconds(5));
        Console.WriteLine("Currently browser address: {0}", driver.Url);
        ValidatePageTransition(driver.Url.StartsWith("https://platform.senior.com.br/senior-x"), "Login failed.");
    }

    private async Task NavigateToTimeEntryPageAsync()
    {
        IWebElement favoritesMenuItem = driver.FindElement(By.Id("menu-item-Favoritos"));
        favoritesMenuItem.Click();
        await Task.Delay(TimeSpan.FromSeconds(2));
        ValidateElementVisibility(driver.FindElement(By.Id("apps-menu-item-0")), "Navigation to favorites failed.");

        IWebElement timeEntryMenu = driver.FindElement(By.Id("apps-menu-item-0"));
        timeEntryMenu.Click();
        await Task.Delay(TimeSpan.FromSeconds(2));
        ValidateElementVisibility(driver.FindElement(By.Id("apps-menu-item-0")), "Navigation to time entry menu failed.");

        IWebElement markTimeEntrySubMenu = driver.FindElement(By.Id("apps-menu-item-0"));
        markTimeEntrySubMenu.Click();
        await Task.Delay(TimeSpan.FromSeconds(5));
        ValidateElementPresence(By.Id("custom_iframe"), "Time entry iframe not found.");
    }

    private async Task RegisterTimeEntryAsync()
    {
        IWebElement iframeElement = driver.FindElement(By.Id("custom_iframe"));
        driver.SwitchTo().Frame(iframeElement);

        IWebElement registerButton = driver.FindElement(By.Id("btn-clocking-event-202410231016"));
        registerButton.Click();
        await Task.Delay(TimeSpan.FromSeconds(5)); // Wait for registration
        
        var isSuccessful = IsTimeEntryRegistrationSuccessfulCombinedAsync();

        if (!isSuccessful)
        {
            HandleAutomationError("Time entry registration failed.");
            _pointRegistrationFailed = true;
        }
        else
        {
            Console.WriteLine("Time entry registered successfully.");
        }

        // More robust checks could be implemented here
        driver.SwitchTo().DefaultContent();
    }

    private void ValidatePageTransition(bool condition, string errorMessage)
    {
        if (!condition)
        {
            HandleAutomationError(errorMessage);
            throw new WebDriverException(errorMessage);
        }
    }

    private void ValidateElementVisibility(IWebElement element, string errorMessage)
    {
        if (!element.Displayed)
        {
            HandleAutomationError(errorMessage);
            throw new NoSuchElementException(errorMessage);
        }
    }

    private void ValidateElementPresence(By by, string errorMessage)
    {
        try
        {
            driver.FindElement(by);
        }
        catch (NoSuchElementException)
        {
            HandleAutomationError(errorMessage);
            throw new NoSuchElementException(errorMessage);
        }
    }

    private bool IsTimeEntryRegistrationSuccessfulCombinedAsync() =>
        IsTimeEntryRegistrationSuccessfulByMessage() || IsTimeEntryRegistrationSuccessfulByLastEntry();

    private bool IsTimeEntryRegistrationSuccessfulByMessage()
    {
        try
        {
            // Espera até que a mensagem de sucesso esteja visível
            var successMessage = _wait.Until(drv => drv.FindElement(By.CssSelector(".ui-messages.ui-messages-success")));
            return successMessage.Displayed;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    private bool IsTimeEntryRegistrationSuccessfulByLastEntry()
    {
        try
        {
            // Espera até que o título de últimas marcações esteja visível
            var lastEntriesTitle = _wait.Until(drv => drv.FindElement(By.XPath("//p[contains(text(), 'Últimas marcações nesta página')]")));
            if (!lastEntriesTitle.Displayed) return false;

            // Espera até que a primeira entrada de hora na lista de últimas marcações esteja visível
            var lastEntryTime = _wait.Until(drv => drv.FindElement(By.XPath("//div[contains(text(), 'Últimas marcações nesta página')]/following-sibling::div//div[@class='ui-g-3']")));
            return lastEntryTime.Displayed;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    private void HandleAutomationError(string errorMessage)
    {
        Console.WriteLine($"Error: {errorMessage}");
        _pointRegistrationFailed = true;
    }

    private static void DisplayCriticalFailureMessage()
    {
        Console.WriteLine("**************************************************");
        Console.WriteLine("***** CRITICAL FAILURE: Time entry registration failed! *****");
        Console.WriteLine("**************************************************");
        Console.Beep();
    }
}