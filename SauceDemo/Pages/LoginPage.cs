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
    public const string Url = "https://www.saucedemo.com/";

    //
    // CSS SELECTORS
    //
    private const string UsernameInput = "#user-name";
    private const string PasswordInput = "#password";
    // Note the login button is actually an <input type="submit" rather than a button.
    private const string LoginButton = "#login-button";

    //
    // TEST CREDENTIALS
    //
    public const string StandardUser = "standard_user";
    public const string LockedOutUser = "locked_out_user";
    public const string ProblemUser = "problem_user";
    public const string PerformanceGlitchUser = "performance_glitch_user";
    public const string ErrorUser = "error_user";
    public const string VisualUser = "visual_user";
    public const string Password = "secret_sauce";

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

    // Navigates to the login URL
    public async Task NavigateAsync() => await _page.GotoAsync(Url);

    // Perform the login action
    public async Task LoginAsync(string username, string password)
    {
        await _page.FillAsync(UsernameInput, username);
        await _page.FillAsync(PasswordInput, password);
        await _page.ClickAsync(LoginButton);
    }
}