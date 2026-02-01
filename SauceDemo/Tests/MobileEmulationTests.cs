using Microsoft.Playwright;
using SauceDemo.Pages;

namespace SauceDemo.Tests;

/// <summary>
/// Bonus Scenario #3: Mobile Emulation Tests
/// Runs tests simulating iPhone and Android devices.
/// </summary>
public class MobileEmulationTests : IAsyncLifetime
{
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;
    private IBrowserContext _context = null!;
    private IPage _page = null!;
    private string _currentDevice = string.Empty;

    public static TheoryData<string> Devices => new()
    {
        "iPhone 15 Pro Max",
        "Pixel 7"
    };

    public async ValueTask InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();

        // Mobile emulation uses Chromium (or WebKit for Safari-like behavior)
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = TestData.Settings.Headless
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (_context != null) await _context.DisposeAsync();
        if (_browser != null) await _browser.DisposeAsync();
        _playwright?.Dispose();
    }

    private async Task SetupDeviceAsync(string deviceName)
    {
        _currentDevice = deviceName;

        // Get device descriptor from Playwright's built-in device list
        var device = _playwright.Devices[deviceName];

        // Create context with device emulation settings
        _context = await _browser.NewContextAsync(device);
        _page = await _context.NewPageAsync();
    }

    private static void Log(string message)
    {
        TestContext.Current.TestOutputHelper?.WriteLine(message);
    }

    [Theory]
    [MemberData(nameof(Devices))]
    public async Task HappyPath_RunsOnMobileDevices(string deviceName)
    {
        //
        // ARRANGE
        //
        await SetupDeviceAsync(deviceName);

        var cartPage = new CartPage(_page);
        var checkoutPage = new CheckoutPage(_page);
        var inventoryPage = new InventoryPage(_page);
        var loginPage = new LoginPage(_page);

        Log($"\n[DEVICE: {_currentDevice}]");

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

        Log($"[PASSED] {_currentDevice}");
    }
}
