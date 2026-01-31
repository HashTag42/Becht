using Microsoft.Playwright;
using Microsoft.Playwright.Xunit.v3;

namespace PlaywrightTests;

public class UnitTest1: PageTest
{
    [Fact]
    public async Task MainNavigation()
    {
        // Assertions use the expect API.
        await Expect(Page).ToHaveURLAsync("https://playwright.dev/");
    }

    override public async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await Page.GotoAsync("https://playwright.dev");
    }

    public override async Task DisposeAsync()
    {
        Console.WriteLine("After each test cleanup");
        await base.DisposeAsync();
    }
}