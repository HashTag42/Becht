// Import the Playwright libraryy for browser automation types:
// IPlaywright, IBrowser, IPage, IBrowserContext
using Microsoft.Playwright;

namespace SauceDemo;

/// <summary>
/// Declare an abstract class (can't be instantiated directly, must be inherited).
/// Implements IAsyncLifetime which is xUnit's interface for async setup/teardown.
/// </summary>
public abstract class TestBase : IAsyncLifetime
{
    /// <summary>
    /// IPlaywright - Property for the Playwight engine
    /// protected - Child classes can access it
    /// private set -  Only this class can assign it
    /// null! - Tells the compiler this will be set before use
    /// </summary>
    protected IPlaywright Playwright { get; private set; } = null!;
    // Browser instance - the actual browser process
    protected IBrowser Browser { get; private set; } = null!;
    // Page - a single browser tab where tests run
    protected IPage Page { get; private set; } = null!;
    // Browser context - An isolated version with its own cookies/storage
    protected IBrowserContext Context { get; private set; } = null!;

    // Browser type - subclasses could override this
    // protected virtual string BrowserType => "chromium";

    // Set to false for debugging and observe browser behavior
    // protected virtual bool Headless => false;

    /// <summary>
    /// InitializeAsync() - xUnit calls this before each test
    /// </summary>
    /// <returns>ValueTask is returned by xUnit.v3 to improve performance</returns>
    public async ValueTask InitializeAsync()
    {
        // Create the Playwright instance
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        // Gets the browser type
        var browser = Playwright.Chromium;

        // Launches the browser with the headless setting
        Browser = await browser.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = TestData.Settings.Headless
        });

        // Creates a new  isolated browser context
        Context = await Browser.NewContextAsync();

        // Opens a new page (tab) in that context
        Page = await Context.NewPageAsync();
    }

    /// <summary>
    /// DisposeAsync() - xUnit calls this after each test
    /// </summary>
    /// <returns>ValueTask is returned by xUnit.v3 to improve performance</returns>
    public async ValueTask DisposeAsync()
    {
        // Close the context
        await Context.DisposeAsync();
        // Close the browser
        await Browser.DisposeAsync();
        // Dispose Playwright resources
        Playwright.Dispose();
    }

    /// <summary>
    /// Takes a screenshot and saves it to the screenshots folder.
    /// </summary>
    /// <param name="name">A descriptive name for the screenshot</param>
    protected async Task TakeScreenshotAsync(string name)
    {
        var screenshotsDir = Path.Combine(AppContext.BaseDirectory, "screenshots");
        Directory.CreateDirectory(screenshotsDir);

        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var path = Path.Combine(screenshotsDir, $"{name}_{timestamp}.png");

        await Page.ScreenshotAsync(new PageScreenshotOptions { Path = path });
    }
}