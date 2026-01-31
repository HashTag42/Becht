using Microsoft.Playwright;

namespace SauceDemo.Pages;

/// <summary>
/// Page Object Model for the Sauce Demo Inventory Page.
/// </summary>
public class InventoryPage
{
    // URL
    public const string Url = "https://www.saucedemo.com/inventory.html";

    // CSS Selectors
    private const string AddToCartButton = "button[data-test^='add-to-cart']";
    private const string InventoryItem = ".inventory_item";
    private const string ShoppingCartBadge = ".shopping_cart_badge";

    // Private field
    private readonly IPage _page;

    // Constructor
    public InventoryPage(IPage page) => _page = page;

    public async Task AddItemToCartByIndexAsync(int index)
    {
        var items = _page.Locator(InventoryItem);
        var item = items.Nth(index);
        await item.Locator(AddToCartButton).ClickAsync();
    }

    public async Task<int> GetCartItemCountAsync()
    {
        var badge = _page.Locator(ShoppingCartBadge);
        if (!await badge.IsVisibleAsync())
            return 0;

        var text = await badge.TextContentAsync();
        return int.TryParse(text, out var count) ? count : 0;
    }
}