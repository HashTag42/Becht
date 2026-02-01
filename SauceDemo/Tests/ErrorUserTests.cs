using Microsoft.Playwright;
using SauceDemo.Pages;

namespace SauceDemo.Tests;

/// <summary>
/// Scenario 5: Error User
/// Login with error_user, go through checkout, report issues with Last Name field
/// and errors preventing Finish button from working.
/// </summary>
public class ErrorUserTests : TestBase
{
    [Fact]
    public async Task ErrorUser_ReportsFormAndFinishErrors()
    {
        var issues = new List<string>();

        //
        // ARRANGE
        //
        var loginPage = new LoginPage(Page);
        var inventoryPage = new InventoryPage(Page);
        var cartPage = new CartPage(Page);
        var checkoutPage = new CheckoutPage(Page);

        //
        // ACT & ASSERT
        //

        // Step 1: Login
        TestContext.Current.TestOutputHelper?.WriteLine("\n[STEP 1] Logging in with error_user...");
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync(TestData.Credentials.ErrorUser, TestData.Credentials.Password);

        if (!await inventoryPage.IsOnPageAsync())
        {
            issues.Add("CRITICAL: Could not login with error_user");
            ReportIssues(issues);
            Assert.Fail("Login failed");
        }

        await TakeScreenshotAsync("ErrorUser_AfterLogin");

        // Step 2: Add 3 items to cart
        TestContext.Current.TestOutputHelper?.WriteLine("[STEP 2] Adding 3 items to cart...");
        for (int i = 0; i < 3; i++)
        {
            await inventoryPage.AddItemToCartByIndexAsync(i);
        }

        var cartCount = await inventoryPage.GetCartItemCountAsync();
        TestContext.Current.TestOutputHelper?.WriteLine($"[INFO] Cart count after adding: {cartCount}");

        if (cartCount != 3)
        {
            issues.Add($"ISSUE: Expected 3 items in cart, but found {cartCount}");
        }

        // Step 3: Remove 1 item
        TestContext.Current.TestOutputHelper?.WriteLine("[STEP 3] Removing 1 item from cart...");
        await inventoryPage.RemoveItemFromCartByIndexAsync(0);

        cartCount = await inventoryPage.GetCartItemCountAsync();
        TestContext.Current.TestOutputHelper?.WriteLine($"[INFO] Cart count after removal: {cartCount}");

        if (cartCount != 2)
        {
            issues.Add($"ISSUE: Expected 2 items in cart after removal, but found {cartCount}");
        }

        // Step 4: Go to cart
        TestContext.Current.TestOutputHelper?.WriteLine("[STEP 4] Going to cart...");
        await inventoryPage.ClickShoppingCartAsync();

        if (!await cartPage.IsOnPageAsync())
        {
            issues.Add("ISSUE: Could not navigate to cart page");
            ReportIssues(issues);
            Assert.Fail("Navigation to cart failed");
        }

        // Step 5: Checkout
        TestContext.Current.TestOutputHelper?.WriteLine("[STEP 5] Starting checkout...");
        await cartPage.ClickCheckoutAsync();

        if (!await checkoutPage.IsOnStepOneAsync())
        {
            issues.Add("ISSUE: Could not navigate to checkout step one");
            ReportIssues(issues);
            Assert.Fail("Navigation to checkout failed");
        }

        // Step 6: Fill form - test each field individually
        TestContext.Current.TestOutputHelper?.WriteLine("[STEP 6] Testing checkout form fields...");

        // Test First Name
        var firstNameSuccess = await checkoutPage.TryFillFirstNameAsync("John");
        TestContext.Current.TestOutputHelper?.WriteLine($"[INFO] First Name field: {(firstNameSuccess ? "OK" : "FAILED")}");
        if (!firstNameSuccess)
        {
            issues.Add("ISSUE: Could not enter First Name");
        }

        // Test Last Name (expected to fail per requirements)
        var lastNameSuccess = await checkoutPage.TryFillLastNameAsync("Doe");
        TestContext.Current.TestOutputHelper?.WriteLine($"[INFO] Last Name field: {(lastNameSuccess ? "OK" : "FAILED")}");
        if (!lastNameSuccess)
        {
            issues.Add("ISSUE: Could not enter Last Name - field does not accept input");
        }

        // Test Postal Code
        var postalCodeSuccess = await checkoutPage.TryFillPostalCodeAsync("12345");
        TestContext.Current.TestOutputHelper?.WriteLine($"[INFO] Postal Code field: {(postalCodeSuccess ? "OK" : "FAILED")}");
        if (!postalCodeSuccess)
        {
            issues.Add("ISSUE: Could not enter Postal Code");
        }

        await TakeScreenshotAsync("ErrorUser_CheckoutForm");

        // Step 7: Try to continue
        TestContext.Current.TestOutputHelper?.WriteLine("[STEP 7] Attempting to continue...");
        await checkoutPage.ClickContinueAsync();
        await Page.WaitForTimeoutAsync(1000);

        if (await checkoutPage.IsErrorDisplayedAsync())
        {
            var errorMsg = await checkoutPage.GetErrorMessageAsync();
            issues.Add($"CHECKOUT ERROR: {errorMsg}");
            TestContext.Current.TestOutputHelper?.WriteLine($"[INFO] Form error displayed: {errorMsg}");
        }

        // Check if we made it to step two
        if (await checkoutPage.IsOnStepTwoAsync())
        {
            TestContext.Current.TestOutputHelper?.WriteLine("[INFO] Made it to checkout step two");

            // Step 8: Try to finish
            TestContext.Current.TestOutputHelper?.WriteLine("[STEP 8] Attempting to finish order...");

            await TakeScreenshotAsync("ErrorUser_BeforeFinish");

            var finishSuccess = await checkoutPage.TryClickFinishAsync();

            if (!finishSuccess)
            {
                issues.Add("ISSUE: Finish button did not complete the order");

                // Check for any error messages on the page
                var pageContent = await Page.ContentAsync();
                if (pageContent.Contains("error", StringComparison.OrdinalIgnoreCase))
                {
                    // Try to find error elements
                    var errorElements = Page.Locator("[class*='error'], [data-test='error']");
                    var errorCount = await errorElements.CountAsync();

                    for (int i = 0; i < errorCount; i++)
                    {
                        var errorText = await errorElements.Nth(i).TextContentAsync();
                        if (!string.IsNullOrWhiteSpace(errorText))
                        {
                            issues.Add($"PAGE ERROR: {errorText}");
                        }
                    }
                }
            }

            await TakeScreenshotAsync("ErrorUser_AfterFinishAttempt");
        }
        else
        {
            TestContext.Current.TestOutputHelper?.WriteLine("[INFO] Could not proceed to step two");
            await TakeScreenshotAsync("ErrorUser_StuckOnStepOne");
        }

        // Report all issues
        ReportIssues(issues);

        // We expect issues with error_user
        Assert.True(issues.Count > 0, "Expected to find issues with error_user, but none were found");
    }

    private void ReportIssues(List<string> issues)
    {
        TestContext.Current.TestOutputHelper?.WriteLine("ERROR USER TEST REPORT");
        TestContext.Current.TestOutputHelper?.WriteLine($"Total issues found: {issues.Count}");

        if (issues.Count == 0)
        {
            TestContext.Current.TestOutputHelper?.WriteLine("  No issues found (unexpected for error_user)");
        }
        else
        {
            TestContext.Current.TestOutputHelper?.WriteLine("Issues preventing checkout completion:");
            foreach (var issue in issues)
            {
                TestContext.Current.TestOutputHelper?.WriteLine($"  • {issue}");
            }
        }
    }
}