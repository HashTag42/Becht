using Microsoft.Playwright;

namespace SauceDemo.Pages;

/// <summary>
/// Page Object Model for the Sauce Demo Login Page.
/// Encapsulates all login page interactions.
/// </summary>
public class LoginPage
{
    //
    // URL
    //
    public const string Url = TestData.Urls.BaseUrl;

    //
    // CSS SELECTORS
    //
    private const string ErrorMessageSelector = "[data-test='error']";
    // Note the login button is actually an <input type="submit" rather than a button.
    private const string LoginButton = "#login-button";
    private const string PasswordInput = "#password";
    private const string UsernameInput = "#user-name";

    //
    // LOCATORS
    //
    public ILocator ErrorMessage => _page.Locator(ErrorMessageSelector);


    //
    // PRIVATE FIELDS
    //
    private readonly IPage _page;

    //
    // CONSTRUCTOR
    //
    public LoginPage(IPage page) => _page = page;

    //
    // METHODS
    //

    public async Task<string> GetErrorMessageAsync()
    {
        await ErrorMessage.WaitForAsync();
        return await ErrorMessage.TextContentAsync() ?? string.Empty;
    }

    public async Task<bool> IsErrorDisplayedAsync()
    {
        return await ErrorMessage.IsVisibleAsync();
    }

    // Perform the login action
    public async Task LoginAsync(string username, string password)
    {
        await _page.FillAsync(UsernameInput, username);
        await _page.FillAsync(PasswordInput, password);
        await _page.ClickAsync(LoginButton);
    }

    // Navigates to the login URL
    public async Task NavigateAsync() => await _page.GotoAsync(Url);
}