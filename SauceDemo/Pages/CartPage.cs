using Microsoft.Playwright;

namespace SauceDemo.Pages;

/// <summary>
/// Page Object Model for the Sauce Demo Cart Page.
/// </summary>
public class CartPage
{
    //
    // URL
    //
    public const string Url = "https://www.saucedemo.com/cart.html";

    //
    // CSS SELECTORS
    //
    private const string CartItem = ".cart_item";
    private const string CheckoutButton = "#checkout";

    //
    // LOCATORS
    //
    public ILocator CartItems => _page.Locator(CartItem);

    //
    // PRIVATE FIELDS
    //
    private readonly IPage _page;

    //
    // CONSTRUCTOR
    //
    public CartPage(IPage page) => _page = page;

    //
    // METHODS
    //

    public async Task ClickCheckoutAsync()
    {
        await _page.ClickAsync(CheckoutButton);
    }

    public async Task<int> GetCartItemCountAsync()
    {
        return await _page.Locator(CartItem).CountAsync();
    }

    public async Task<bool> IsOnPageAsync()
    {
        return _page.Url.Contains("cart.html");
    }
}