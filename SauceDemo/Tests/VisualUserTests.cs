using Microsoft.Playwright;
using SauceDemo.Pages;

namespace SauceDemo.Tests;

/// <summary>
/// Scenario 6: Detect Visual Error
/// Login with visual_user and report UI elements that are outside their containers
/// when compared to standard_user.
/// </summary>
public class VisualUserTests : TestBase
{
    [Fact]
    public async Task VisualUser_DetectUIElementsOutsideBoxes()
    {
        //
        // ARRANGE
        //
        var visualIssues = new List<string>();

        //
        // ACT & ASSERT
        //

        Log("[SCENARIO] Detech Visual Error");

        // First, get baseline from standard_user
        Log("[STEP 1] Getting baseline from standard_user...");
        var loginPage = new LoginPage(Page);
        var inventoryPage = new InventoryPage(Page);

        await loginPage.NavigateAsync();
        await loginPage.LoginAsync(TestData.Credentials.StandardUser, TestData.Credentials.Password);

        if (!await inventoryPage.IsOnPageAsync())
        {
            Assert.Fail("Could not login with standard_user for baseline");
        }

        var standardImages = await inventoryPage.GetImageInfoAsync();
        await TakeScreenshotAsync("Visual_StandardUser_Baseline");

        // Logout and login with visual_user
        Log("[STEP 2] Logging in with visual_user...");
        await Page.GotoAsync(LoginPage.Url);
        await loginPage.LoginAsync(TestData.Credentials.VisualUser, TestData.Credentials.Password);

        if (!await inventoryPage.IsOnPageAsync())
        {
            Assert.Fail("Could not login with visual_user");
        }

        await TakeScreenshotAsync("Visual_VisualUser");

        // Get visual_user's layout
        var visualImages = await inventoryPage.GetImageInfoAsync();

        // Compare and report issues
        Log("[STEP 3] Analyzing visual differences...");

        foreach (var img in visualImages)
        {
            if (img.IsImageOutsideContainer)
            {
                visualIssues.Add(
                    $"VISUAL ISSUE at item index {img.Index}: " +
                    $"Image extends outside container. " +
                    $"Image bounds: ({img.ImageBounds.X:F0}, {img.ImageBounds.Y:F0}, " +
                    $"{img.ImageBounds.Width:F0}x{img.ImageBounds.Height:F0}), " +
                    $"Container bounds: ({img.ItemBounds.X:F0}, {img.ItemBounds.Y:F0}, " +
                    $"{img.ItemBounds.Width:F0}x{img.ItemBounds.Height:F0})");
            }
        }

        // Also check for different image sources (mismatched images)
        for (int i = 0; i < Math.Min(standardImages.Count, visualImages.Count); i++)
        {
            if (standardImages[i].ImageSource != visualImages[i].ImageSource)
            {
                visualIssues.Add(
                    $"VISUAL ISSUE at item index {i}: " +
                    $"Different image displayed. " +
                    $"Expected: {standardImages[i].ImageSource}, " +
                    $"Actual: {visualImages[i].ImageSource}");
            }
        }

        // Additional visual checks using Playwright's built-in capabilities
        Log("[STEP 4] Checking for additional visual anomalies...");

        // Check cart icon position
        var cartIcon = Page.Locator(".shopping_cart_link");
        var cartIconBox = await cartIcon.BoundingBoxAsync();
        var headerBox = await Page.Locator(".header_secondary_container").BoundingBoxAsync();

        if (cartIconBox != null && headerBox != null)
        {
            if (cartIconBox.Y < headerBox.Y ||
                cartIconBox.Y + cartIconBox.Height > headerBox.Y + headerBox.Height + 10)
            {
                visualIssues.Add(
                    $"VISUAL ISSUE: Shopping cart icon may be misaligned. " +
                    $"Cart Y: {cartIconBox.Y:F0}, Header Y range: {headerBox.Y:F0}-{headerBox.Y + headerBox.Height:F0}");
            }
        }

        // Check if any prices look abnormal
        var prices = Page.Locator(".inventory_item_price");
        var priceCount = await prices.CountAsync();

        for (int i = 0; i < priceCount; i++)
        {
            var priceBox = await prices.Nth(i).BoundingBoxAsync();
            var itemBox = await Page.Locator(".inventory_item").Nth(i).BoundingBoxAsync();

            if (priceBox != null && itemBox != null)
            {
                if (priceBox.X < itemBox.X - 5 ||
                    priceBox.X + priceBox.Width > itemBox.X + itemBox.Width + 5)
                {
                    visualIssues.Add(
                        $"VISUAL ISSUE at item index {i}: Price element outside container bounds");
                }
            }
        }

        // Report all visual issues
        ReportVisualIssues(visualIssues);

        // Per requirements, there should be at least 2 visual issues
        Assert.True(visualIssues.Count >= 2,
            $"Expected at least 2 visual issues with visual_user, but found {visualIssues.Count}");
    }

    [Fact]
    public async Task VisualUser_CompareScreenshots()
    {
        // This test creates comparison screenshots for manual review
        var loginPage = new LoginPage(Page);
        var inventoryPage = new InventoryPage(Page);

        // Standard user screenshot
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync(TestData.Credentials.StandardUser, TestData.Credentials.Password);
        await Page.WaitForLoadStateAsync();
        await TakeScreenshotAsync("Comparison_StandardUser");

        // Visual user screenshot
        await Page.GotoAsync(LoginPage.Url);
        await loginPage.LoginAsync(TestData.Credentials.VisualUser, TestData.Credentials.Password);
        await Page.WaitForLoadStateAsync();
        await TakeScreenshotAsync("Comparison_VisualUser");

        Log("[INFO] Screenshots saved for manual comparison:");
        Log("  - Comparison_StandardUser_*.png");
        Log("  - Comparison_VisualUser_*.png");
        Log("Compare these screenshots to identify visual differences.");
    }

    private void ReportVisualIssues(List<string> issues)
    {
        Log("[VISUAL USER TEST REPORT]");
        Log($"Visual issues detected: {issues.Count}");

        if (issues.Count == 0)
        {
            Log("  No visual issues detected (unexpected for visual_user)");
        }
        else
        {
            Log("UI Elements outside their containers:");
            foreach (var issue in issues)
            {
                Log($"  • {issue}");
            }
        }
    }
}