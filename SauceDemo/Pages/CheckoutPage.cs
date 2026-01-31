using Microsoft.Playwright;

namespace SauceDemo.Pages;

/// <summary>
/// Page Object Model for the Sauce Demo Checkout Pages"
/// Step One, Step Two, Complete
/// </summary>
public class CheckoutPage
{
    // URLs
    public const string StepOneUrl = "https://www.saucedemo.com/checkout-step-one.html";
    public const string StepTwoUrl = "https://www.saucedemo.com/checkout-step-two.html";
    public const string CompleteUrl = "https://www.saucedemo.com/checkout-complete.html";

    // STEP 1 CSS SELECTORS

    // PRIVATE FIELDS
    private readonly IPage _page;

    // CONSTRUCTOR
    public CheckoutPage(IPage page) => _page = page;

    // METHODS

    public async Task<bool> IsOnStepOneAsync()
    {
        return _page.Url.Contains("checkout-step-one.html");
    }


}