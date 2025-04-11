namespace dm.PulseShift.Domain.Interfaces.Repositories;

public interface ILoginPageRepository
{
    void NavigateToLoginPage();
    void EnterUsername(string username);
    void ClickNextButton();
    void EnterPassword(string password);
    void ClickAuthenticateButton();
    bool IsLoginErrorMessageDisplayed();
    string GetLoginErrorMessageText();
}