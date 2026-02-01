using Microsoft.Playwright;
using SauceDemo.Pages;

namespace SauceDemo.Tests;

/// <summary>
/// Scenario 4: Glitchy User
/// Login with performance_glitch_user and attempt Happy Path.
/// Abort and report if anything doesn't work correctly.
/// </summary>
public class GlitchyUserTests : TestBase
{
    private const int TimeoutMs = 10000; // Extended timeout for slow responses

    [Fact]
    public async Task GlitchyUser_AttemptHappyPath_ReportIssues()
    {
        var issues = new List<string>();
        var startTime = DateTime.Now;

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

        Log("[SCENARIO] Glitchy User");

        try
        {
            // Step 1: Login
            Log("[STEP 1] Navigating to login page...");
            var stepStart = DateTime.Now;
            await loginPage.NavigateAsync();
            var loginLoadTime = (DateTime.Now - stepStart).TotalMilliseconds;
            Log($"[INFO] Login page loaded in {loginLoadTime}ms");

            if (loginLoadTime > 3000)
            {
                issues.Add($"PERFORMANCE: Login page took {loginLoadTime}ms to load (expected < 3000ms)");
            }

            stepStart = DateTime.Now;
            await loginPage.LoginAsync(TestData.Credentials.PerformanceGlitchUser, TestData.Credentials.Password);

            // Wait for navigation with extended timeout
            await Page.WaitForURLAsync(InventoryPage.Url, new() { Timeout = TimeoutMs });
            var loginTime = (DateTime.Now - stepStart).TotalMilliseconds;
            Log($"[INFO] Login completed in {loginTime}ms");

            if (loginTime > 5000)
            {
                issues.Add($"PERFORMANCE: Login took {loginTime}ms (expected < 5000ms)");
            }

            if (!await inventoryPage.IsOnPageAsync())
            {
                issues.Add("CRITICAL: Failed to navigate to inventory page after login");
                AbortTest(issues);
                return;
            }

            // Step 2: Add 3 items to cart
            Log("[STEP 2] Adding 3 items to cart...");
            for (int i = 0; i < 3; i++)
            {
                stepStart = DateTime.Now;
                await inventoryPage.AddItemToCartByIndexAsync(i);
                var addTime = (DateTime.Now - stepStart).TotalMilliseconds;

                if (addTime > 2000)
                {
                    issues.Add($"PERFORMANCE: Adding item {i} took {addTime}ms");
                }
            }

            var cartCount = await inventoryPage.GetCartItemCountAsync();
            if (cartCount != 3)
            {
                issues.Add($"ISSUE: Expected 3 items in cart, but found {cartCount}");
                AbortTest(issues);
                return;
            }

            await TakeScreenshotAsync("GlitchyUser_AfterLogin");

            // Step 3: Remove 1 item
            Log("[STEP 3] Removing 1 item from cart...");
            stepStart = DateTime.Now;
            await inventoryPage.RemoveItemFromCartByIndexAsync(0);
            var removeTime = (DateTime.Now - stepStart).TotalMilliseconds;

            if (removeTime > 2000)
            {
                issues.Add($"PERFORMANCE: Removing item took {removeTime}ms");
            }

            cartCount = await inventoryPage.GetCartItemCountAsync();
            if (cartCount != 2)
            {
                issues.Add($"ISSUE: Expected 2 items in cart after removal, but found {cartCount}");
                AbortTest(issues);
                return;
            }

            // Step 4: Go to cart
            Log("[STEP 4] Navigating to cart...");
            stepStart = DateTime.Now;
            await inventoryPage.ClickShoppingCartAsync();
            await Page.WaitForURLAsync(CartPage.Url, new() { Timeout = TimeoutMs });
            var cartNavTime = (DateTime.Now - stepStart).TotalMilliseconds;

            if (cartNavTime > 3000)
            {
                issues.Add($"PERFORMANCE: Cart navigation took {cartNavTime}ms");
            }

            if (!await cartPage.IsOnPageAsync())
            {
                issues.Add("ISSUE: Failed to navigate to cart page");
                AbortTest(issues);
                return;
            }

            // Step 5: Checkout
            Log("[STEP 5] Starting checkout..");
            stepStart = DateTime.Now;
            await cartPage.ClickCheckoutAsync();
            await Page.WaitForURLAsync(CheckoutPage.StepOneUrl, new() { Timeout = TimeoutMs });
            var checkoutNavTime = (DateTime.Now - stepStart).TotalMilliseconds;

            if (checkoutNavTime > 3000)
            {
                issues.Add($"PERFORMANCE: Checkout navigation took {checkoutNavTime}ms");
            }

            if (!await checkoutPage.IsOnStepOneAsync())
            {
                issues.Add("ISSUE: Failed to navigate to checkout step one");
                AbortTest(issues);
                return;
            }

            // Step 6: Fill form and continue
            Log("[STEP 6] Filling shipping information...");
            await checkoutPage.FillShippingInfoAsync(
                TestData.Shipping.FirstName,
                TestData.Shipping.LastName,
                TestData.Shipping.PostalCode);

            stepStart = DateTime.Now;
            await checkoutPage.ClickContinueAsync();
            await Page.WaitForURLAsync(CheckoutPage.StepTwoUrl, new() { Timeout = TimeoutMs });
            var continueTime = (DateTime.Now - stepStart).TotalMilliseconds;

            if (continueTime > 3000)
            {
                issues.Add($"PERFORMANCE: Continue to step two took {continueTime}ms");
            }

            if (!await checkoutPage.IsOnStepTwoAsync())
            {
                issues.Add("ISSUE: Failed to navigate to checkout step two");
                AbortTest(issues);
                return;
            }

            // Step 7: Modify total and finish
            Log("[STEP 7] Modifying total and finishing...");
            await checkoutPage.ModifyTotalAsync("Total: $500.00");

            stepStart = DateTime.Now;
            await checkoutPage.ClickFinishAsync();
            await Page.WaitForURLAsync(CheckoutPage.CompleteUrl, new() { Timeout = TimeoutMs });
            var finishTime = (DateTime.Now - stepStart).TotalMilliseconds;

            if (finishTime > 3000)
            {
                issues.Add($"PERFORMANCE: Finish took {finishTime}ms");
            }

            if (!await checkoutPage.IsOnCompletePageAsync())
            {
                issues.Add("ISSUE: Failed to navigate to checkout complete page");
                AbortTest(issues);
                return;
            }

            // Step 8: Back home
            Log("[STEP 8] Returning home...");
            stepStart = DateTime.Now;
            await checkoutPage.ClickBackHomeAsync();
            await Page.WaitForURLAsync(InventoryPage.Url, new() { Timeout = TimeoutMs });
            var homeTime = (DateTime.Now - stepStart).TotalMilliseconds;

            if (homeTime > 3000)
            {
                issues.Add($"PERFORMANCE: Back home took {homeTime}ms");
            }

            await TakeScreenshotAsync("GlitchyUser_Complete");

            // Report results
            var totalTime = (DateTime.Now - startTime).TotalMilliseconds;
            Log($"[INFO] Total test time: {totalTime}ms");

            ReportIssues(issues, completed: true);
        }
        catch (TimeoutException ex)
        {
            issues.Add($"TIMEOUT: {ex.Message}");
            await TakeScreenshotAsync("GlitchyUser_Timeout");
            AbortTest(issues);
        }
        catch (Exception ex)
        {
            issues.Add($"ERROR: {ex.Message}");
            await TakeScreenshotAsync("GlitchyUser_Error");
            AbortTest(issues);
        }

        // For debugging purposes
        await TestData.DebugDelayAsync(TestContext.Current.CancellationToken);
    }

    private void AbortTest(List<string> issues)
    {
        ReportIssues(issues, completed: false);
        Assert.Fail("Test aborted due to issues. See report above.");
    }

    private void ReportIssues(List<string> issues, bool completed)
    {
        Log("GLITCHY USER TEST REPORT");
        Log($"Test completed: {(completed ? "YES" : "NO - ABORTED")}");
        Log($"Issues found: {issues.Count}");

        if (issues.Count == 0)
        {
            Log("No issues found");
        }
        else
        {
            foreach (var issue in issues)
            {
                Log($"   * {issue}");
            }
        }
    }
}