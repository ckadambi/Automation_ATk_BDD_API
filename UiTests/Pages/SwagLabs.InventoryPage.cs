using OpenQA.Selenium;

namespace UiTests.Pages.SwagLabs;

public sealed class InventoryPage : UiTests.Pages.BasePage
{
    private static readonly By InventoryContainer = By.Id("inventory_container");
    private static readonly By CartBadge = By.CssSelector(".shopping_cart_badge");
    private static readonly By CartLink = By.CssSelector("a.shopping_cart_link");
    public InventoryPage(IWebDriver driver) : base(driver) { }
    public bool IsLoaded() => Find(InventoryContainer).Displayed;
    public void AddItemToCartByName(string itemName)
    {
        string slug = itemName.ToLowerInvariant().Replace(" ", "-");
        var addBtn = By.Id($"add-to-cart-{slug}");
        Find(addBtn).Click();
        Thread.Sleep(2000);
    }
    public int CartCount()
    {        
        var badges = FindAll(CartBadge);
        if (badges.Count == 0) return 0;
        return int.TryParse(badges.First().Text, out var n) ? n : 0;
    }
    public void OpenCart()
    {
        Thread.Sleep(2000);
        Find(CartLink).Click();
    }
}
