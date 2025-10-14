using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace UiTests.Pages;

public abstract class BasePage
{
    protected readonly IWebDriver Driver;
    protected readonly WebDriverWait Wait;

    protected BasePage(IWebDriver driver)
    {
        Driver = driver;
        Wait = new WebDriverWait(new SystemClock(), driver, TimeSpan.FromSeconds(15), TimeSpan.FromMilliseconds(250));
    }

    protected IWebElement Find(By by) => Wait.Until(d => d.FindElement(by));
    protected IReadOnlyCollection<IWebElement> FindAll(By by) => Driver.FindElements(by);
    public string Title => Driver.Title;
}
