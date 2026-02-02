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

        /// <summary>
        /// Runs headless when CI environment variable is set (e.g., GitHub Actions).
        /// </summary>
        public static bool Headless => Environment.GetEnvironmentVariable("CI") == "true";

        /// <summary>
        /// Browser to use: "chromium", "chrome", "msedge", "firefox", "webkit".
        /// </summary>
        public const string Browser = "chromium";
    }

    public static class Urls
    {
        public const string BaseUrl = "https://www.saucedemo.com";
        public const string Inventory = BaseUrl + "/inventory.html";
        public const string Cart = BaseUrl + "/cart.html";
        public const string CheckoutStepOne = BaseUrl + "/checkout-step-one.html";
        public const string CheckoutStepTwo = BaseUrl + "/checkout-step-two.html";
        public const string CheckoutComplete = BaseUrl + "/checkout-complete.html";
    }

    public static class Credentials
    {
        public const string StandardUser = "standard_user";
        public const string LockedOutUser = "locked_out_user";
        public const string ProblemUser = "problem_user";
        public const string PerformanceGlitchUser = "performance_glitch_user";
        public const string ErrorUser = "error_user";
        public const string VisualUser = "visual_user";
        public const string Password = "secret_sauce";
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
