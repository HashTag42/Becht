using Microsoft.Playwright;
using Xunit;

namespace SauceDemo;

public abstract class TestBase : IAsyncLifetime
{
    protected IPlaywright Playwright { get; private set; } = null!;
    protected IBrowser Browser { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;
    protected IBrowserContext Context { get; private set; } = null!;

    protected virtual string BrowserType => "chromium";

    // Set to false for debugging
    protected virtual bool Headless => false;

    public async ValueTask InitializeAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        var browser = Playwright.Chromium;

        Browser = await browser.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = Headless
        });

        Context = await Browser.NewContextAsync();

        Page = await Context.NewPageAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        await Browser.DisposeAsync();
        Playwright.Dispose();
    }
}