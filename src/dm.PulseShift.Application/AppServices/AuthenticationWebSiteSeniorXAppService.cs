using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace dm.PulseShift.Application.AppServices;

public class AuthenticationWebSiteSeniorXAppService(
    ILoginPageRepository loginPageRepository,
    ILogger<AuthenticationWebSiteSeniorXAppService> logger) : IAuthenticationWebSiteSeniorXAppService
{
    public bool Login(string username, string password)
    {
        logger.LogInformation("Attempting login for user: {Username}", username);
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("Username and password cannot be null or empty.");
        }
        try
        {
            loginPageRepository.NavigateToLoginPage();
            loginPageRepository.EnterUsername(username);
            loginPageRepository.ClickNextButton();
            loginPageRepository.EnterPassword(password);
            loginPageRepository.ClickAuthenticateButton();

            // Passo 5: Verificar Sucesso/Falha
            // A verificação mais robusta seria esperar por um elemento da página pós-login.
            // Exemplo:
            // bool loggedIn = _dashboardRepository.WaitForDashboardToLoad(TimeSpan.FromSeconds(10));
            // if (loggedIn) {
            //    _logger.LogInformation("Login successful for user: {Username}. Dashboard loaded.", username);
            //    return true;
            // }

            // Verificação alternativa: checar se mensagem de erro apareceu RAPIDAMENTE.
            // Se não apareceu em ~3 segundos, assumimos sucesso (não ideal, mas funciona).
            if (loginPageRepository.IsLoginErrorMessageDisplayed())
            {
                string errorMessage = loginPageRepository.GetLoginErrorMessageText();
                logger.LogWarning("Login failed for user {Username}. Error message: {ErrorMessage}", username, errorMessage);
                return false;
            }
            else
            {
                // Assumindo sucesso se nenhum erro foi detectado rapidamente.
                // Idealmente, confirme com um elemento da próxima página.
                logger.LogInformation("Login potentially successful for user: {Username} (No immediate error message detected). Add explicit dashboard verification.", username);
                return true;
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            logger.LogError(ex, "An error occurred during login for user: {Username}", username);
            Console.WriteLine($"An error occurred during login: {ex.Message}");
            return false;
        }
    }
}
