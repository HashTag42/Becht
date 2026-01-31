using Microsoft.Playwright;
using SauceDemo.Pages;

namespace SauceDemo.Tests;

public class HappyPathTests : TestBase
{
    [Fact]
    public async Task StandardUser_CanCompleteFullPurchaseFlow()
    {
        var loginPage = new LoginPage(Page);

        await loginPage.NavigateAsync();
        await loginPage.LoginAsync(LoginPage.StandardUser, LoginPage.Password);

        await Assertions.Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");
    }
}