using Microsoft.Playwright;
using SauceDemo.Pages;

namespace SauceDemo.Tests;

/// <summary>
/// Scenario 3: Problem User
/// Login with problem_user and report issues with adding/removing items and checkout.
/// </summary>
public class ProblemUserTests : TestBase
{
    [Fact]
    public async Task ProblemUser_ReportAllIssues()
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

        Log("[SCENARIO] Problem User");

        // Login
        Log("[STEP 1] Attempt to Login");
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync(TestData.Credentials.ProblemUser, TestData.Credentials.Password);

        if (!await inventoryPage.IsOnPageAsync())
        {
            issues.Add("[CRITICAL] Could not login with problem_user");
            ReportIssues(issues);
            Assert.Fail("Login failed");
        }

        // Check initial cart state (should have 2 items per requirements)
        Log("[STEP 2] Check initial cart state (should have 2 items per requirements)");
        var initialCartCount = await inventoryPage.GetCartItemCountAsync();
        Log($"[INFO] Initial cart count: {initialCartCount}");
        if (initialCartCount == 2)
        {
            Log("[INFO] Confirmed: 2 items already in cart on login");
        }

        await TakeScreenshotAsync("ProblemUser_InitialState");

        // Try to add items to cart
        Log("[STEP 3] Attempting to add items to cart...");
        var itemCount = await inventoryPage.GetInventoryItemCountAsync();

        for (int i = 0; i < Math.Min(itemCount, 6); i++)
        {
            var cartBefore = await inventoryPage.GetCartItemCountAsync();
            var addSuccess = await inventoryPage.TryAddItemToCartByIndexAsync(i);
            var cartAfter = await inventoryPage.GetCartItemCountAsync();

            if (!addSuccess || cartAfter == cartBefore)
            {
                issues.Add($"ISSUE: Could not add item at index {i} to cart. Cart count remained at {cartAfter}");
            }
            else
            {
                Log($"[INFO] Successfully added item at index {i}");
            }
        }

        await TakeScreenshotAsync("ProblemUser_AfterAddAttempts");

        // Try to remove items from cart
        Log("[STEP 4] Attempting to remove items from cart...");
        var currentCartCount = await inventoryPage.GetCartItemCountAsync();

        for (int i = 0; i < Math.Min(itemCount, 3); i++)
        {
            var cartBefore = await inventoryPage.GetCartItemCountAsync();
            var removeSuccess = await inventoryPage.TryRemoveItemFromCartByIndexAsync(i);
            var cartAfter = await inventoryPage.GetCartItemCountAsync();

            if (!removeSuccess && cartBefore > 0)
            {
                issues.Add($"ISSUE: Could not remove item at index {i} from cart. Cart count: {cartAfter}");
            }
            else if (removeSuccess)
            {
                Log($"[INFO] Successfully removed item at index {i}");
            }
        }

        await TakeScreenshotAsync("ProblemUser_AfterRemoveAttempts");

        // Go to cart and checkout
        Log("[STEP 5] Click Shopping Cart icon");
        await inventoryPage.ClickShoppingCartAsync();

        if (!await cartPage.IsOnPageAsync())
        {
            issues.Add("ISSUE: Could not navigate to cart page");
        }

        await cartPage.ClickCheckoutAsync();

        if (!await checkoutPage.IsOnStepOneAsync())
        {
            issues.Add("ISSUE: Could not navigate to checkout page");
            ReportIssues(issues);
            return;
        }

        // Try to fill checkout form
        Log("[STEP 6] Attempting to fill checkout form...");

        var firstNameSuccess = await checkoutPage.TryFillFirstNameAsync(TestData.Shipping.FirstName);
        if (!firstNameSuccess)
        {
            issues.Add("ISSUE: Could not enter First Name in checkout form");
        }

        var lastNameSuccess = await checkoutPage.TryFillLastNameAsync(TestData.Shipping.LastName);
        if (!lastNameSuccess)
        {
            issues.Add("ISSUE: Could not enter Last Name in checkout form");
        }

        var postalCodeSuccess = await checkoutPage.TryFillPostalCodeAsync(TestData.Shipping.PostalCode);
        if (!postalCodeSuccess)
        {
            issues.Add("ISSUE: Could not enter Postal Code in checkout form");
        }

        await TakeScreenshotAsync("ProblemUser_CheckoutForm");

        // Try to continue
        Log("[STEP 7] Attempt to continue");
        await checkoutPage.ClickContinueAsync();
        await Page.WaitForTimeoutAsync(1000);

        if (await checkoutPage.IsErrorDisplayedAsync())
        {
            var errorMsg = await checkoutPage.GetErrorMessageAsync();
            issues.Add($"ISSUE: Checkout form error - {errorMsg}");
        }

        if (await checkoutPage.IsOnStepOneAsync())
        {
            issues.Add("ISSUE: Could not proceed past checkout step one - form validation failed or button not working");
        }

        await TakeScreenshotAsync("ProblemUser_AfterContinueAttempt");

        // Report all issues
        ReportIssues(issues);

        // Assert that we found issues (this is expected for problem_user)
        Assert.True(issues.Count > 0, "Expected to find issues with problem_user, but none were found");
    }

    private void ReportIssues(List<string> issues)
    {
        Log("PROBLEM USER TEST REPORT");

        if (issues.Count == 0)
        {
            Log("No issues found (unexpected for problem_user)");
        }
        else
        {
            Log($"Total issues found: {issues.Count}");
            foreach (var issue in issues)
            {
                Log($"   * {issue}");
            }
        }
    }
}