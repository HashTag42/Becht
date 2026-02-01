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
        //
        // ARRANGE
        //
        var cartPage = new CartPage(Page);
        var checkoutPage = new CheckoutPage(Page);
        var inventoryPage = new InventoryPage(Page);
        var loginPage = new LoginPage(Page);

        //
        // ACT & ASSERT
        //

        // STEP 1: Login
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync(TestData.Credentials.StandardUser, TestData.Credentials.Password);
        await Assertions.Expect(Page).ToHaveURLAsync(InventoryPage.Url);

        // STEP 2: Add 3 items to the cart
        await inventoryPage.AddItemToCartByIndexAsync(0);
        await inventoryPage.AddItemToCartByIndexAsync(1);
        await inventoryPage.AddItemToCartByIndexAsync(2);
        await Assertions.Expect(inventoryPage.CartBadge).ToHaveTextAsync("3");

        // STEP 3: Remove 1 item
        await inventoryPage.RemoveItemFromCartByIndexAsync(0);
        await Assertions.Expect(inventoryPage.CartBadge).ToHaveTextAsync("2");

        // STEP 4: Go to cart
        await inventoryPage.ClickShoppingCartAsync();
        Assert.True(await cartPage.IsOnPageAsync(), "Should navigate to cart page");
        await Assertions.Expect(cartPage.CartItems).ToHaveCountAsync(2);

        // STEP 5: Checkout
        await cartPage.ClickCheckoutAsync();
        await Assertions.Expect(Page).ToHaveURLAsync(CheckoutPage.StepOneUrl);

        // STEP 6: Fill shipping info
        await checkoutPage.FillShippingInfoAsync(
            TestData.Shipping.FirstName,
            TestData.Shipping.LastName,
            TestData.Shipping.PostalCode);

        await checkoutPage.ClickContinueAsync();
        await Assertions.Expect(Page).ToHaveURLAsync(CheckoutPage.StepTwoUrl);

        // STEP 7: Modify total to $500
        await checkoutPage.ModifyTotalAsync("Total: $500.00");
        await Assertions.Expect(checkoutPage.Total).ToHaveTextAsync("Total: $500.00");

        // STEP 8: Finish order
        await checkoutPage.ClickFinishAsync();
        Assert.True(await checkoutPage.IsOnCompletePageAsync(), "Should navigate to checkout complete page");
        await Assertions.Expect(checkoutPage.CompleteHeaderText).ToHaveTextAsync("Thank you for your order!");

        // STEP 9: Go back home
        await checkoutPage.ClickBackHomeAsync();
        await Assertions.Expect(Page).ToHaveURLAsync(InventoryPage.Url);

        await TakeScreenshotAsync("HappyPath_Complete");

        // For debugging purposes
        await TestData.DebugDelayAsync(TestContext.Current.CancellationToken);
    }
}