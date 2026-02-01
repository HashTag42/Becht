using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Playwright;
using SauceDemo.Pages;

namespace SauceDemo.Tests;

/// <summary>
/// Bonus Scenario #4: Simple Load Testing
/// Simulates multiple concurrent users running the happy path.
/// </summary>
public class LoadTests : IAsyncLifetime
{
    private IPlaywright _playwright = null!;

    private const int ConcurrentUsers = 10;

    public async ValueTask InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        _playwright?.Dispose();
    }

    private static void Log(string message)
    {
        TestContext.Current.TestOutputHelper?.WriteLine(message);
    }

    [Fact]
    public async Task LoadTest_SimulateConcurrentUsers()
    {
        Log($"\n[LOAD TEST] Starting with {ConcurrentUsers} concurrent users");

        var results = new ConcurrentBag<UserResult>();
        var overallStopwatch = Stopwatch.StartNew();

        // Create tasks for each simulated user
        var userTasks = Enumerable.Range(1, ConcurrentUsers)
            .Select(userId => SimulateUserAsync(userId, results))
            .ToArray();

        // Wait for all users to complete
        await Task.WhenAll(userTasks);

        overallStopwatch.Stop();

        // Report results
        LogResults(results, overallStopwatch.ElapsedMilliseconds);

        // Assert all users succeeded
        var failures = results.Where(r => !r.Success).ToList();
        Assert.True(failures.Count == 0, $"{failures.Count} users failed: {string.Join(", ", failures.Select(f => $"User {f.UserId}: {f.Error}"))}");
    }

    private async Task SimulateUserAsync(int userId, ConcurrentBag<UserResult> results)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new UserResult { UserId = userId };

        IBrowser? browser = null;
        IBrowserContext? context = null;

        try
        {
            // Each user gets their own browser instance
            browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true // Always headless for load tests
            });

            context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            // Run the happy path for this user
            await RunHappyPathAsync(page, userId);

            result.Success = true;
            Log($"[User {userId}] ✅ Completed successfully");
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Error = ex.Message;
            Log($"[User {userId}] ❌ Failed: {ex.Message}");
        }
        finally
        {
            stopwatch.Stop();
            result.ElapsedMs = stopwatch.ElapsedMilliseconds;
            results.Add(result);

            if (context != null) await context.DisposeAsync();
            if (browser != null) await browser.DisposeAsync();
        }
    }

    private static async Task RunHappyPathAsync(IPage page, int userId)
    {
        var cartPage = new CartPage(page);
        var checkoutPage = new CheckoutPage(page);
        var inventoryPage = new InventoryPage(page);
        var loginPage = new LoginPage(page);

        // Login
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync(TestData.Credentials.StandardUser, TestData.Credentials.Password);
        await Assertions.Expect(page).ToHaveURLAsync(InventoryPage.Url);

        // Add items to cart
        await inventoryPage.AddItemToCartByIndexAsync(0);
        await inventoryPage.AddItemToCartByIndexAsync(1);

        // Go to cart
        await inventoryPage.ClickShoppingCartAsync();
        await Assertions.Expect(cartPage.CartItems).ToHaveCountAsync(2);

        // Checkout
        await cartPage.ClickCheckoutAsync();

        // Fill shipping info (unique per user to simulate real load)
        await checkoutPage.FillShippingInfoAsync(
            $"User{userId}",
            "LoadTest",
            "12345");

        await checkoutPage.ClickContinueAsync();

        // Finish order
        await checkoutPage.ClickFinishAsync();
        await Assertions.Expect(checkoutPage.CompleteHeaderText).ToHaveTextAsync("Thank you for your order!");
    }

    private static void LogResults(ConcurrentBag<UserResult> results, long totalElapsedMs)
    {
        var sortedResults = results.OrderBy(r => r.UserId).ToList();
        var successCount = sortedResults.Count(r => r.Success);
        var failCount = sortedResults.Count(r => !r.Success);
        var avgTime = sortedResults.Average(r => r.ElapsedMs);
        var minTime = sortedResults.Min(r => r.ElapsedMs);
        var maxTime = sortedResults.Max(r => r.ElapsedMs);

        Log("\n========================================");
        Log("LOAD TEST RESULTS");
        Log("========================================");
        Log($"Total Users:     {ConcurrentUsers}");
        Log($"Passed:          {successCount}");
        Log($"Failed:          {failCount}");
        Log($"Total Time:      {totalElapsedMs}ms");
        Log($"Avg User Time:   {avgTime:F0}ms");
        Log($"Min User Time:   {minTime}ms");
        Log($"Max User Time:   {maxTime}ms");
        Log("========================================");

        Log("\nPer-User Results:");
        foreach (var r in sortedResults)
        {
            var status = r.Success ? "PASS" : "FAIL";
            Log($"  User {r.UserId,2}: {status} ({r.ElapsedMs}ms){(r.Error != null ? $" - {r.Error}" : "")}");
        }
    }

    private class UserResult
    {
        public int UserId { get; set; }
        public bool Success { get; set; }
        public long ElapsedMs { get; set; }
        public string? Error { get; set; }
    }
}
