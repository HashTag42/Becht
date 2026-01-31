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
        // ARRANGE
        var cartPage = new CartPage(Page);
        var checkoutPage = new CheckoutPage(Page);
        var inventoryPage = new InventoryPage(Page);
        var loginPage = new LoginPage(Page);

        // STEP 1: Login
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync(LoginPage.StandardUser, LoginPage.Password);
        await Assertions.Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");

        // STEP 2: Add 3 items to the cart
        await inventoryPage.AddItemToCartByIndexAsync(0);
        await inventoryPage.AddItemToCartByIndexAsync(1);
        await inventoryPage.AddItemToCartByIndexAsync(2);
        Assert.Equal(3, await inventoryPage.GetCartItemCountAsync());

        // STEP 3: Remove 1 item
        await inventoryPage.RemoveItemFromCartByIndexAsync(0);
        Assert.Equal(2, await inventoryPage.GetCartItemCountAsync());

        // STEP 4: Go to cart
        await inventoryPage.ClickShoppingCartAsync();
        Assert.True(await cartPage.IsOnPageAsync(), "Should navigate to cart page");
        Assert.Equal(2, await cartPage.GetCartItemCountAsync());

        // STEP 5: Checkout
        await cartPage.ClickCheckoutAsync();
        Assert.True(await checkoutPage.IsOnStepOneAsync(), "Should navigate to checkout step one");

        // STEP 6: Fill shipping info
        await checkoutPage.FillShippingInfoAsync("Cesar", "Garcia", "12345");
        await checkoutPage.ClickContinueAsync();
        Assert.True(await checkoutPage.IsOnStepTwoAsync(), "Should navigate to checkout step two");

        // STEP 7: Modify total to $500
        await checkoutPage.ModifyTotalAsync("Total: $500.00");
        var modifiedTotal = await checkoutPage.GetTotalAsync();
        Assert.Equal("Total: $500.00", modifiedTotal);

        // STEP 8: Finish order

        // STEP 9: Go back home

        // Remove before shipping to production
        await Task.Delay(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
    }
}