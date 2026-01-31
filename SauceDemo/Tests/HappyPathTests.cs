// Import Playwright for the Assertions class
using Microsoft.Playwright;
// Access the LoginPage page object
using SauceDemo.Pages;

namespace SauceDemo.Tests;

/// <summary>
/// Inherits from TestBase to access the Page property and
/// ensures browser setup/teardown runs before/after each test
/// </summary>
public class HappyPathTests : TestBase
{
    // Asynchronous xUnit test case
    [Fact]
    public async Task StandardUser_CanCompleteFullPurchaseFlow()
    {
        // Arrange
        var loginPage = new LoginPage(Page);
        var inventoryPage = new InventoryPage(Page);

        // Login
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync(LoginPage.StandardUser, LoginPage.Password);
        await Assertions.Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");

        // Add 3 items to the cart
        await inventoryPage.AddItemToCartByIndexAsync(0);
        await inventoryPage.AddItemToCartByIndexAsync(1);
        await inventoryPage.AddItemToCartByIndexAsync(2);
        Assert.Equal(3, await inventoryPage.GetCartItemCountAsync());
    }
}