using OpenQA.Selenium;

namespace UiTests.Pages.SwagLabs;

public sealed class CartPage : UiTests.Pages.BasePage
{
    private static readonly By CartItems = By.CssSelector(".cart_item");
    public CartPage(IWebDriver driver) : base(driver) { }
    public bool HasItem(string itemName) =>
        FindAll(CartItems).Any(i => i.Text.Contains(itemName, StringComparison.OrdinalIgnoreCase));
}
