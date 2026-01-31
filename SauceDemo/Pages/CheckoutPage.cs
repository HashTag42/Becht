using Microsoft.Playwright;

namespace SauceDemo.Pages;

/// <summary>
/// Page Object Model for the Sauce Demo Checkout Pages"
/// Step One, Step Two, Complete
/// </summary>
public class CheckoutPage
{
    //
    // URLs
    //
    public const string StepOneUrl = "https://www.saucedemo.com/checkout-step-one.html";
    public const string StepTwoUrl = "https://www.saucedemo.com/checkout-step-two.html";
    public const string CompleteUrl = "https://www.saucedemo.com/checkout-complete.html";

    //
    // CSS SELECTORS
    //
    private const string BackHomeButton = "#back-to-products";
    private const string CompleteHeader = ".complete-header";
    private const string ContinueButton = "#continue";
    private const string FinishButton = "#finish";
    private const string FirstNameInput = "#first-name";
    private const string LastNameInput = "#last-name";
    private const string PostalCodeInput = "#postal-code";
    private const string SummaryTotal = ".summary_total_label";

    //
    // PRIVATE FIELDS
    //
    private readonly IPage _page;

    //
    // CONSTRUCTOR
    //
    public CheckoutPage(IPage page) => _page = page;

    //
    // METHODS
    //
    public async Task ClickBackHomeAsync()
    {
        await _page.ClickAsync(BackHomeButton);
    }

        public async Task ClickContinueAsync()
    {
        await _page.ClickAsync(ContinueButton);
    }

    public async Task ClickFinishAsync()
    {
        await _page.ClickAsync(FinishButton);
    }

    public async Task FillFirstNameAsync(string firstName)
    {
        await _page.FillAsync(FirstNameInput, firstName);
    }

    public async Task FillLastNameAsync(string lastName)
    {
        await _page.FillAsync(LastNameInput, lastName);
    }

    public async Task FillPostalCodeAsync(string postalCode)
    {
        await _page.FillAsync(PostalCodeInput, postalCode);
    }

    public async Task FillShippingInfoAsync(string firstName, string lastName, string postalCode)
    {
        await FillFirstNameAsync(firstName);
        await FillLastNameAsync(lastName);
        await FillPostalCodeAsync(postalCode);
    }

    public async Task<string> GetCompleteHeaderAsync()
    {
        return await _page.Locator(CompleteHeader).TextContentAsync() ?? string.Empty;
    }

    public async Task<string> GetTotalAsync()
    {
        return await _page.Locator(SummaryTotal).TextContentAsync() ?? string.Empty;
    }

    public async Task<bool> IsOnCompletePageAsync()
    {
        return _page.Url.Contains("checkout-complete.html");
    }

    public async Task<bool> IsOnStepOneAsync()
    {
        return _page.Url.Contains("checkout-step-one.html");
    }

    public async Task<bool> IsOnStepTwoAsync()
    {
        return _page.Url.Contains("checkout-step-two.html");
    }

    public async Task ModifyTotalAsync(string newTotal)
    {
        // Use JavaScript to modify the total display
        await _page.EvaluateAsync($@"
            const totalElement = document.querySelector('{SummaryTotal}');
            if (totalElement) {{
                totalElement.textContent = '{newTotal}';
            }}
        ");
    }
}