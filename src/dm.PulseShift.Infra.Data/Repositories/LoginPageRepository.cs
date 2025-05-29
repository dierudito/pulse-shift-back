using dm.PulseShift.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace dm.PulseShift.Infra.Data.Repositories;

public class LoginPageRepository(IWebDriver driver, ILogger<LoginPageRepository> logger) : ILoginPageRepository
{
    private readonly WebDriverWait _wait = new(driver, TimeSpan.FromSeconds(0));
    private const string BaseUrl = "https://platform.senior.com.br";
    private readonly By _usernameField = By.Id("username-input-field");
    private readonly By _nextButton = By.Id("nextBtn");
    private readonly By _passwordField = By.Id("password-input-field");
    private readonly By _authenticateButton = By.Id("loginbtn");
    private readonly By _errorMessageContainer = By.Id("message_div");
    private readonly By _errorMessageSpan = By.Id("message");

    public void NavigateToLoginPage()
    {
        logger.LogInformation("Navigating to Login Page: {BaseUrl}", BaseUrl);
        driver.Navigate().GoToUrl(BaseUrl);
        try
        {
            // Espera o campo de usuário estar visível como indicador que a página carregou
            _wait.Until(ExpectedConditions.ElementIsVisible(_usernameField));
            logger.LogInformation("Login page loaded successfully, username field visible.");
        }
        catch (WebDriverTimeoutException ex)
        {
            logger.LogError(ex, "Timeout waiting for username field to be visible on login page.");
            // Talvez capturar screenshot aqui
            throw; // Re-lança a exceção para falhar o teste
        }
    }
    public void EnterUsername(string username)
    {
        try
        {
            logger.LogDebug("Waiting for username field to be visible and enabled.");
            var userElement = _wait.Until(ExpectedConditions.ElementToBeClickable(_usernameField)); // Espera estar visível e habilitado
            userElement.Clear();
            userElement.SendKeys(username);
            logger.LogInformation("Username entered successfully.");
        }
        catch (WebDriverTimeoutException ex)
        {
            logger.LogError(ex, "Timeout or error entering username into field {Locator}", _usernameField);
            throw;
        }
    }
    public void ClickNextButton()
    {
        try
        {
            logger.LogInformation("Attempting to click the 'Next' button.");
            var nextButtonElement = _wait.Until(ExpectedConditions.ElementToBeClickable(_nextButton));
            nextButtonElement.Click();
            logger.LogInformation("'Next' button clicked successfully.");

            // IMPORTANTE: Esperar o campo de senha ficar visível após clicar em Próximo
            logger.LogDebug("Waiting for password field to become visible after clicking 'Next'.");
            _wait.Until(ExpectedConditions.ElementIsVisible(_passwordField));
            logger.LogDebug("Password field is now visible.");
        }
        catch (WebDriverTimeoutException ex)
        {
            logger.LogError(ex, "Timeout or error clicking 'Next' button ({Locator}) or waiting for password field ({PasswordFieldLocator}).", _nextButton, _passwordField);
            throw;
        }
    }
    public void EnterPassword(string password)
    {
        try
        {
            // A espera pela visibilidade já foi feita (idealmente) em ClickNextButton,
            // mas uma verificação extra por clicabilidade aqui é segura.
            logger.LogDebug("Waiting for password field to be clickable.");
            var passElement = _wait.Until(ExpectedConditions.ElementToBeClickable(_passwordField));
            passElement.Clear();
            passElement.SendKeys(password);
            logger.LogInformation("Password entered successfully.");
        }
        catch (WebDriverTimeoutException ex)
        {
            logger.LogError(ex, "Timeout or error entering password into field {Locator}", _passwordField);
            throw;
        }
    }
    public void ClickAuthenticateButton()
    {
        try
        {
            logger.LogInformation("Attempting to click the 'Authenticate' button.");
            // O botão Autenticar também fica visível após clicar em Próximo. Esperar por ele.
            var authButtonElement = _wait.Until(ExpectedConditions.ElementToBeClickable(_authenticateButton));
            authButtonElement.Click();
            logger.LogInformation("'Authenticate' button clicked successfully.");
            // Aqui, poderia adicionar uma espera pelo desaparecimento de algum elemento do login
            // ou pelo aparecimento de um elemento do dashboard (seria melhor na camada de App ou Teste)
        }
        catch (WebDriverTimeoutException ex)
        {
            logger.LogError(ex, "Timeout or error clicking 'Authenticate' button ({Locator}).", _authenticateButton);
            throw;
        }
    }
    public bool IsLoginErrorMessageDisplayed()
    {
        try
        {
            // Espera CURTA para verificar se a DIV de erro está visível
            // Não queremos esperar o timeout completo se o login for bem-sucedido
            var shortWait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));

            // Verifica se a DIV container da mensagem está visível (não tem mais a classe 'hidden')
            var errorContainer = shortWait.Until(ExpectedConditions.ElementIsVisible(_errorMessageContainer));

            bool isDisplayed = errorContainer.Displayed; // Confirma a visibilidade
            if (isDisplayed)
            {
                logger.LogWarning("Login error message container is visible.");
            }
            else
            {
                logger.LogInformation("Login error message container not found or not visible within the short wait period.");
            }
            return isDisplayed;
        }
        catch (WebDriverTimeoutException)
        {
            // É esperado um timeout se não houver erro e o container não ficar visível
            logger.LogInformation("Login error message container did not become visible (expected for successful login).");
            return false;
        }
        catch (NoSuchElementException)
        {
            // Caso o elemento nem exista (menos provável com ID)
            logger.LogInformation("Login error message container element does not exist.");
            return false;
        }
    }
    public string GetLoginErrorMessageText()
    {
        if (IsLoginErrorMessageDisplayed()) // Primeiro verifica se está visível
        {
            try
            {
                // Se o container está visível, o span interno deve existir
                var errorMessageElement = driver.FindElement(_errorMessageSpan); // Não precisa de wait longo aqui
                string errorText = errorMessageElement.Text;
                logger.LogWarning("Retrieved login error message text: {ErrorText}", errorText);
                return errorText;
            }
            catch (NoSuchElementException ex)
            {
                logger.LogError(ex, "Error message container was visible, but the inner span ({Locator}) was not found.", _errorMessageSpan);
                return string.Empty; // Ou lançar exceção? Depende da estratégia
            }
        }
        logger.LogInformation("Login error message is not displayed, returning empty string.");
        return string.Empty;
    }
}
