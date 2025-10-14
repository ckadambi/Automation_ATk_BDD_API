using FluentAssertions;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using UiTests.Pages.SwagLabs;

namespace UiTests.Steps;

[Binding]
public class SwagLabsSteps
{
    private readonly IWebDriver _driver;
    private readonly LoginPage _login;
    private readonly InventoryPage _inventory;
    private readonly CartPage _cart;

    public SwagLabsSteps(IWebDriver driver)
    {
        _driver = driver;
        _login = new LoginPage(driver);
        _inventory = new InventoryPage(driver);
        _cart = new CartPage(driver);
    }

    [Given(@"I am on the Swag Labs login page")]
    public void GivenIAmOnTheSwagLabsLoginPage()
    {
        _login.GoTo();
        _login.Title.Should().Contain("Swag Labs");
    }

    [When(@"I log in with username ""(.*)"" and password ""(.*)""")]
    public void WhenILogInWith(string user, string pass) => _login.Login(user, pass);

    [Then(@"I should see the inventory page")]
    public void ThenIShouldSeeTheInventoryPage() => _inventory.IsLoaded().Should().BeTrue();

    [When(@"I add ""(.*)"" to the cart")]
    public void WhenIAddToTheCart(string itemName) => _inventory.AddItemToCartByName(itemName);

    [Then(@"the cart count should be (.*)")]
    public void ThenTheCartCountShouldBe(int expected) => _inventory.CartCount().Should().Be(expected);

    [When(@"I open the cart")]
    public void WhenIOpenTheCart() => _inventory.OpenCart();

    [Then(@"I should see ""(.*)"" in the cart")]
    public void ThenIShouldSeeInTheCart(string itemName) => _cart.HasItem(itemName).Should().BeTrue();

    [Then(@"I should see a login error")]
    public void ThenIShouldSeeALoginError()
    {
        _login.ErrorText().Should().NotBeNullOrWhiteSpace();
    }
}
