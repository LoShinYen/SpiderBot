using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace OTTSpiderBotHtmlDocument.Helpers
{
    public class SeleniumHelper : IDisposable
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;
        private readonly int _waitSeconds;

        public SeleniumHelper(int waitSeconds = 10, bool headless = false)
        {
            _waitSeconds = waitSeconds;
            //var chromeDriverPath = AppDomain.CurrentDomain.BaseDirectory;
            //string driverPath = Path.Combine(chromeDriverPath, "chromedriver.exe");
            //var driverService = ChromeDriverService.CreateDefaultService(chromeDriverPath);
            var options = new ChromeOptions();
            options.AddUserProfilePreference("profile.default_content_setting_values.notifications", 2);
            options.AddUserProfilePreference("profile.default_content_setting_values.geolocation", 2); // 禁用地理位置請求
            options.AddUserProfilePreference("profile.default_content_setting_values.media_stream_mic", 2); // 禁用麥克風存取請求
            options.AddUserProfilePreference("profile.default_content_setting_values.media_stream_camera", 2); // 禁用攝像頭存取請求
            options.PageLoadStrategy = PageLoadStrategy.Eager;
            if (headless)
            {
                options.AddArguments("--headless");
            }
            //_driver = new ChromeDriver(driverService, options);
            _driver = new ChromeDriver(options);
            _driver.Manage().Window.Size = new System.Drawing.Size(1024, 824);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(_waitSeconds));
        }

        public void NavigateTo(string url)
        {
            try
            {
                _driver.Navigate().GoToUrl(url);
            }
            catch (WebDriverException e)
            {
                Program.Logger.Error($"Error navigating to {url}: {e.Message}");
                Console.WriteLine($"Error navigating to {url}: {e.Message}");
            }
        }

        public bool WaitForElementVisible(By by)
        {
            try
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(_waitSeconds));
                return wait.Until(d => d.FindElement(by).Displayed);
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public async Task WaitForPageLoadCompleteAsync(bool requiresExtendedLoading)
        {
            new WebDriverWait(_driver, TimeSpan.FromSeconds(_waitSeconds)).Until(
                d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
            if (requiresExtendedLoading)
            {
                await ScrollDownByPortionsAsync(1000, 20);
            }
            else
            { 
                await ScrollDownByPortionsAsync(700, 10);
            }
        }

        public string GetPageSource()
        {
            return _driver.PageSource;
        }

        /// <summary>
        /// 指定像素滾動
        /// </summary>
        /// <param name="pixelsToScroll">像素</param>
        /// <param name="numberOfPortions">次數</param>
        public async Task ScrollDownByPortionsAsync(int pixelsToScroll, int numberOfPortions)
        {
            try
            {
                long totalScrollHeight = (long)((IJavaScriptExecutor)_driver).ExecuteScript("return document.body.scrollHeight");
                long portionHeight = totalScrollHeight / numberOfPortions;
                long currentScrollPosition = 0;

                for (int i = 0; i < numberOfPortions; i++)
                {
                    currentScrollPosition += pixelsToScroll;
                    ((IJavaScriptExecutor)_driver).ExecuteScript($"window.scrollBy(0, {pixelsToScroll});");
                    await Task.Delay(1000);
                    if (currentScrollPosition >= portionHeight * (i + 1))
                    {
                        currentScrollPosition = portionHeight * (i + 1);
                    }
                }

                ((IJavaScriptExecutor)_driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            }
            catch (WebDriverException e)
            {
                Console.WriteLine($"Error scrolling down by portions: {e.Message}");
                Program.Logger.Error($"Error scrolling down by portions: {e.Message}");
            }
        }


        public void Dispose()
        {
            _driver.Quit();
        }
    }
}
