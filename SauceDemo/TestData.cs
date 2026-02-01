namespace SauceDemo;

/// <summary>
/// Static test data constants for use across all tests.
/// </summary>
public static class TestData
{
    public static class Settings
    {
        public const int DelaySeconds = 1;
        public const bool EnableDebugDelay = true;
    }

    public static class Shipping
    {
        public const string FirstName = "Cesar";
        public const string LastName = "Garcia";
        public const string PostalCode = "12345";
    }

    /// <summary>
    /// Delays test execution for debugging purposes when enabled.
    /// </summary>
    public static async Task DebugDelayAsync(CancellationToken cancellationToken = default)
    {
        if (Settings.EnableDebugDelay)
        {
            await Task.Delay(TimeSpan.FromSeconds(Settings.DelaySeconds), cancellationToken);
        }
    }
}
