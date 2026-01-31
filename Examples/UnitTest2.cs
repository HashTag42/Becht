using Microsoft.Playwright;

namespace PlaywrightTests;

public class ExampleTest : IAsyncLifetime
{
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;
    private IPage _page = null!;

    public async ValueTask InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync();
        _page = await _browser.NewPageAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _browser.DisposeAsync();
        _playwright.Dispose();
    }

    [Fact]
    public async Task HomepageHasTitle()
    {
        await _page.GotoAsync("https://playwright.dev");
        var title = await _page.TitleAsync();
        Assert.Contains("Playwright", title);
    }
}