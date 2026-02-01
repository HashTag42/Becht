using Microsoft.Playwright;
using SauceDemo.Pages;

namespace SauceDemo.Tests;

/// <summary>
/// Multi-Browser Tests - Runs tests across different browsers.
/// Each test receives the browser type as a parameter.
/// </summary>
public class MultiBrowserTests : IAsyncLifetime
{
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;
    private IBrowserContext _context = null!;
    private IPage _page = null!;
    private string _currentBrowser = "chromium";

    public static TheoryData<string> Browsers => new()
    {
        "chromium",
        "msedge",
        "firefox"
    };

    public async ValueTask InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_context != null) await _context.DisposeAsync();
        if (_browser != null) await _browser.DisposeAsync();
        _playwright?.Dispose();
    }

    private async Task SetupBrowserAsync(string browserType)
    {
        _currentBrowser = browserType;

        (IBrowserType type, string? channel) = browserType switch
        {
            "chrome" => (_playwright.Chromium, "chrome"),
            "msedge" => (_playwright.Chromium, "msedge"),
            "firefox" => (_playwright.Firefox, "firefox"),
            // "webkit" => (_playwright.Webkit, "webkit"),
            _ => (_playwright.Chromium, "chromium")
        };

        _browser = await type.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Channel = channel,
            Headless = TestData.Settings.Headless
        });

        _context = await _browser.NewContextAsync();
        _page = await _context.NewPageAsync();
    }

    private void Log(string message)
    {
        TestContext.Current.TestOutputHelper?.WriteLine(message);
    }

    [Theory]
    [MemberData(nameof(Browsers))]
    public async Task HappyPath_RunsInDifferentBrowsers(string browserType)
    {
        //
        // ARRANGE
        //
        await SetupBrowserAsync(browserType);

        var cartPage = new CartPage(_page);
        var checkoutPage = new CheckoutPage(_page);
        var inventoryPage = new InventoryPage(_page);
        var loginPage = new LoginPage(_page);

        Log($"\n[BROWSER: {_currentBrowser.ToUpper()}]");

        //
        // ACT & ASSERT
        //

        // STEP 1: Login
        Log("[STEP 1] Login");
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync(TestData.Credentials.StandardUser, TestData.Credentials.Password);
        await Assertions.Expect(_page).ToHaveURLAsync(InventoryPage.Url);

        // STEP 2: Add 3 items to the cart
        Log("[STEP 2] Add 3 items to the cart");
        await inventoryPage.AddItemToCartByIndexAsync(0);
        await inventoryPage.AddItemToCartByIndexAsync(1);
        await inventoryPage.AddItemToCartByIndexAsync(2);
        await Assertions.Expect(inventoryPage.CartBadge).ToHaveTextAsync("3");

        // STEP 3: Remove 1 item
        Log("[STEP 3] Remove 1 item");
        await inventoryPage.RemoveItemFromCartByIndexAsync(0);
        await Assertions.Expect(inventoryPage.CartBadge).ToHaveTextAsync("2");

        // STEP 4: Go to cart
        Log("[STEP 4] Go to cart");
        await inventoryPage.ClickShoppingCartAsync();
        Assert.True(await cartPage.IsOnPageAsync(), "Should navigate to cart page");
        await Assertions.Expect(cartPage.CartItems).ToHaveCountAsync(2);

        // STEP 5: Checkout
        Log("[STEP 5] Checkout");
        await cartPage.ClickCheckoutAsync();
        await Assertions.Expect(_page).ToHaveURLAsync(CheckoutPage.StepOneUrl);

        // STEP 6: Fill shipping info
        Log("[STEP 6] Fill shipping info");
        await checkoutPage.FillShippingInfoAsync(
            TestData.Shipping.FirstName,
            TestData.Shipping.LastName,
            TestData.Shipping.PostalCode);

        await checkoutPage.ClickContinueAsync();
        await Assertions.Expect(_page).ToHaveURLAsync(CheckoutPage.StepTwoUrl);

        // STEP 7: Verify checkout summary
        Log("[STEP 7] Verify checkout summary");
        await Assertions.Expect(checkoutPage.Total).ToContainTextAsync("$");

        // STEP 8: Finish order
        Log("[STEP 8] Finish order");
        await checkoutPage.ClickFinishAsync();
        Assert.True(await checkoutPage.IsOnCompletePageAsync(), "Should navigate to checkout complete page");
        await Assertions.Expect(checkoutPage.CompleteHeaderText).ToHaveTextAsync("Thank you for your order!");

        // STEP 9: Go back home
        Log("[STEP 9] Go back home");
        await checkoutPage.ClickBackHomeAsync();
        await Assertions.Expect(_page).ToHaveURLAsync(InventoryPage.Url);

        Log($"[PASSED] {_currentBrowser.ToUpper()}");
    }
}
