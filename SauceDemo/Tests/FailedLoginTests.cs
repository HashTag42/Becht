using Microsoft.Playwright;
using SauceDemo.Pages;

namespace SauceDemo.Tests;

/// <summary>
/// Scenario 2: Failed Login
/// Attempt to login with locked_out_user and verify error message.
/// </summary>
public class FailedLoginTests : TestBase
{
    [Fact]
    public async Task LockedOutUser_ShouldSeeErrorMessage()
    {
        //
        // ARRANGE
        //
        var loginPage = new LoginPage(Page);

        Log("[SCENARIO] Failed Login");

        //
        // ACT
        //
        Log("[STEP 1] Attempt to login");
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync(TestData.Credentials.LockedOutUser, TestData.Credentials.Password);

        //
        // ASSERT
        //
        Log("[STEP 2] Verify there is an error message");
        await Assertions.Expect(loginPage.ErrorMessage).ToBeVisibleAsync();
        var errorMessage = await loginPage.GetErrorMessageAsync();
        await Assertions.Expect(loginPage.ErrorMessage).ToContainTextAsync("locked out", new() { IgnoreCase = true });

        // Report the error message
        TestContext.Current.TestOutputHelper?.WriteLine($"[REPORT] Locked out user error message: {errorMessage}");

        // For debugging purposes
        await TestData.DebugDelayAsync(TestContext.Current.CancellationToken);

        await TakeScreenshotAsync("FailedLogin_LockedOutUser");
    }
}