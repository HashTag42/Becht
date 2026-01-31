using Microsoft.Playwright;

namespace SauceDemo.Pages;

/// <summary>
/// Page Object Model for the Sauce Demo Inventory Page.
/// </summary>
public class InventoryPage
{
    //
    // URL
    //
    public const string Url = "https://www.saucedemo.com/inventory.html";

    //
    // CSS SELECTORS
    //

    // CSS Attribute selector: Matches all button elements where the `data-test` attribute starts with `add-to-cart`
    private const string AddToCartButton = "button[data-test^='add-to-cart']";
    // CSS Class selector: Matches all elements with `class="inventory_item"`
    private const string InventoryItem = ".inventory_item";
    private const string RemoveButton = "button[data-test^='remove']";
    private const string ShoppingCartBadge = ".shopping_cart_badge";
    private const string ShoppingCartLink = ".shopping_cart_link";

    //
    // PRIVATE FIELDS
    //
    private readonly IPage _page;

    //
    // CONSTRUCTOR
    //
    public InventoryPage(IPage page) => _page = page;

    //
    // METHODS
    //
    public async Task AddItemToCartByIndexAsync(int index)
    {
        var items = _page.Locator(InventoryItem);
        var item = items.Nth(index);
        await item.Locator(AddToCartButton).ClickAsync();
    }

    public async Task ClickShoppingCartAsync()
    {
        await _page.ClickAsync(ShoppingCartLink);
    }

    public async Task<int> GetCartItemCountAsync()
    {
        var badge = _page.Locator(ShoppingCartBadge);
        if (!await badge.IsVisibleAsync())
            return 0;

        var text = await badge.TextContentAsync();
        return int.TryParse(text, out var count) ? count : 0;
    }

    public async Task<bool> IsOnPageAsync()
    {
        return _page.Url.Contains("inventory.html");
    }

    public async Task RemoveItemFromCartByIndexAsync(int index)
    {
        var items = _page.Locator(InventoryItem);
        var item = items.Nth(index);
        await item.Locator(RemoveButton).ClickAsync();
    }
}