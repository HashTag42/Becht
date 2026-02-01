using SauceDemo.Pages;
using Xunit.v3;

namespace SauceDemo.Tests;

/// <summary>
/// BONUS 1: Parameterized Tests
/// Uses Happy Path as the standard test and passes different usernames.
/// Aborts if something in the UI blocks the test from continuing.
/// </summary>
public class ParameterizedTests : TestBase
{
    public static IEnumerable<object[]> UserCredentials => new List<object[]>
    {
        new object[] { TestData.Credentials.StandardUser, TestData.Credentials.Password, true, "Standard user - should complete" },
        new object[] { TestData.Credentials.LockedOutUser, TestData.Credentials.Password, true, "Locked out user - should fail login" },
        new object[] { TestData.Credentials.ProblemUser, TestData.Credentials.Password, true, "Problem user - may have issues" },
        new object[] { TestData.Credentials.PerformanceGlitchUser, TestData.Credentials.Password, true, "Glitchy user - slow but should complete" },
        new object[] { TestData.Credentials.ErrorUser, TestData.Credentials.Password, true, "Error user - should have checkout issues" },
        new object[] { TestData.Credentials.VisualUser, TestData.Credentials.Password, true, "Visual user - should complete (visual issues only)" },
    };

    [Theory]
    [MemberData(nameof(UserCredentials))]
    public async Task HappyPath_WithDifferentUsers(
        string username,
        string password,
        bool expectedToComplete,
        string description)
    {
        Log($"\n[PARAMETERIZED TEST] {username}");
        Log($"Description: {description}");
        Log($"Expected to complete: {expectedToComplete}");

        //
        // ARRANGE
        //
        var issues = new List<string>();
        var loginPage = new LoginPage(Page);
        var inventoryPage = new InventoryPage(Page);
        var cartPage = new CartPage(Page);
        var checkoutPage = new CheckoutPage(Page);

        //
        // ACT & ASSERT
        //
        try
        {
            // Step 1: Login
            Log("\n[STEP 1] Attempting login...");
            await loginPage.NavigateAsync();
            await loginPage.LoginAsync(username, password);

            // Check for login error
            if (await loginPage.IsErrorDisplayedAsync())
            {
                var errorMsg = await loginPage.GetErrorMessageAsync();
                issues.Add($"LOGIN BLOCKED: {errorMsg}");
                await TakeScreenshotAsync($"Param_{username}_LoginFailed");

                if (!expectedToComplete)
                {
                    Log($"[EXPECTED] Login failed as expected: {errorMsg}");
                    return; // Test passes - expected failure
                }

                AbortWithReport(username, issues, "Login failed");
                return;
            }

            // Wait for inventory page
            await Page.WaitForURLAsync("**/inventory.html", new() { Timeout = 15000 });

            if (!await inventoryPage.IsOnPageAsync())
            {
                issues.Add("BLOCKED: Could not reach inventory page");
                AbortWithReport(username, issues, "Navigation failed");
                return;
            }

            Log("[OK] Login successful");

            // Step 2: Add 3 items
            Log("[STEP 2] Adding 3 items to cart...");
            for (int i = 0; i < 3; i++)
            {
                var success = await inventoryPage.TryAddItemToCartByIndexAsync(i);
                if (!success)
                {
                    issues.Add($"Could not add item {i} to cart");
                }
            }

            // Step 3: Remove 1 item
            Log("[STEP 3] Removing 1 item...");
            await inventoryPage.TryRemoveItemFromCartByIndexAsync(0);

            var cartCount = await inventoryPage.GetCartItemCountAsync();
            if (cartCount < 1)
            {
                issues.Add($"BLOCKED: No items in cart (expected 3, got {cartCount})");
                await TakeScreenshotAsync($"Param_{username}_NoItemsInCart");
                AbortWithReport(username, issues, "Cart is empty");
                return;
            }
            Log($"[OK] Cart has {cartCount} items");

            // Step 4: Go to cart
            Log("[STEP 4] Going to cart...");
            await inventoryPage.ClickShoppingCartAsync();
            await Page.WaitForURLAsync("**/cart.html", new() { Timeout = 10000 });

            if (!await cartPage.IsOnPageAsync())
            {
                issues.Add("BLOCKED: Could not navigate to cart");
                AbortWithReport(username, issues, "Cart navigation failed");
                return;
            }
            Log("[OK] On cart page");

            // Step 5: Checkout
            Log("[STEP 5] Starting checkout...");
            await cartPage.ClickCheckoutAsync();
            await Page.WaitForURLAsync("**/checkout-step-one.html", new() { Timeout = 10000 });

            if (!await checkoutPage.IsOnStepOneAsync())
            {
                issues.Add("BLOCKED: Could not start checkout");
                AbortWithReport(username, issues, "Checkout failed");
                return;
            }
            Log("[OK] On checkout step one");

            // Step 6: Fill form
            Log("[STEP 6] Filling shipping info...");
            var firstNameOk = await checkoutPage.TryFillFirstNameAsync("John");
            var lastNameOk = await checkoutPage.TryFillLastNameAsync("Doe");
            var postalOk = await checkoutPage.TryFillPostalCodeAsync("12345");

            if (!firstNameOk) issues.Add("Could not fill First Name");
            if (!lastNameOk) issues.Add("Could not fill Last Name");
            if (!postalOk) issues.Add("Could not fill Postal Code");

            await checkoutPage.ClickContinueAsync();
            await Page.WaitForTimeoutAsync(2000);

            // Check for form errors
            if (await checkoutPage.IsErrorDisplayedAsync())
            {
                var errorMsg = await checkoutPage.GetErrorMessageAsync();
                issues.Add($"Form error: {errorMsg}");
            }

            if (!await checkoutPage.IsOnStepTwoAsync())
            {
                issues.Add("BLOCKED: Could not proceed to checkout step two");
                await TakeScreenshotAsync($"Param_{username}_CheckoutBlocked");

                if (!expectedToComplete)
                {
                    Log("[EXPECTED] Checkout blocked as expected");
                    PrintReport(username, issues, completed: false);
                    return;
                }

                AbortWithReport(username, issues, "Checkout step two failed");
                return;
            }
            Log("[OK] On checkout step two");

            // Step 7: Modify total and finish
            Log("[STEP 7] Modifying total and finishing...");
            await checkoutPage.ModifyTotalAsync("Total: $500.00");
            await checkoutPage.ClickFinishAsync();

            await Page.WaitForTimeoutAsync(2000);

            if (!await checkoutPage.IsOnCompletePageAsync())
            {
                issues.Add("BLOCKED: Could not complete order");
                await TakeScreenshotAsync($"Param_{username}_FinishBlocked");

                if (!expectedToComplete)
                {
                    Log("[EXPECTED] Finish blocked as expected");
                    PrintReport(username, issues, completed: false);
                    return;
                }

                AbortWithReport(username, issues, "Order completion failed");
                return;
            }
            Log("[OK] Order complete!");

            // Step 8: Back home
            Log("[STEP 8] Returning home...");
            await checkoutPage.ClickBackHomeAsync();
            await Page.WaitForURLAsync("**/inventory.html", new() { Timeout = 10000 });

            await TakeScreenshotAsync($"Param_{username}_Complete");
            Log("[OK] Test completed successfully!");

            PrintReport(username, issues, completed: true);

            if (!expectedToComplete && issues.Count == 0)
            {
                Log($"[WARNING] User {username} was expected to fail but completed successfully");
            }
        }
        catch (TimeoutException ex)
        {
            issues.Add($"TIMEOUT: {ex.Message}");
            await TakeScreenshotAsync($"Param_{username}_Timeout");

            if (expectedToComplete)
            {
                AbortWithReport(username, issues, "Timeout occurred");
            }
            else
            {
                PrintReport(username, issues, completed: false);
            }
        }
        catch (Exception ex)
        {
            issues.Add($"ERROR: {ex.Message}");
            await TakeScreenshotAsync($"Param_{username}_Error");

            if (expectedToComplete)
            {
                AbortWithReport(username, issues, "Exception occurred");
            }
            else
            {
                PrintReport(username, issues, completed: false);
            }
        }
    }

    private void AbortWithReport(string username, List<string> issues, string reason)
    {
        PrintReport(username, issues, completed: false);
        if (username == TestData.Credentials.LockedOutUser ||
            username == TestData.Credentials.ErrorUser ||
            username == TestData.Credentials.ProblemUser)
        {
            // We expect issues with certain users
            Assert.True(issues.Count > 0, $"Expected to find issues with {username} but none were found");
        }
        else
        {
            Assert.Fail($"Test aborted for {username}: {reason}");
        }
    }

    private void PrintReport(string username, List<string> issues, bool completed)
    {
        Log($"\n--- Report for {username} ---");
        Log($"Completed: {completed}");
        Log($"Issues: {issues.Count}");
        foreach (var issue in issues)
        {
            Log($"  • {issue}");
        }
    }
}