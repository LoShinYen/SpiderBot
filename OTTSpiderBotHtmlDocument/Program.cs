using Microsoft.Extensions.Configuration;
using NLog;
using OTTSpiderBotHtmlDocument.Helpers;
using OTTSpiderBotHtmlDocument.Models;

class Program
{ 
    internal static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    static async Task Main(string[] args)
    {
        IConfiguration configuration = InitializeConfiguration();
        
        var crawlSettings = configuration.GetSection("CrawlSettings").Get<CrawlSettings>() ?? new CrawlSettings();

        foreach (var platform in crawlSettings.Platforms)
        {
            using (var selenium = new SeleniumHelper())
            {
                try
                {
                    selenium.NavigateTo(platform.Url);

                    await selenium.WaitForPageLoadCompleteAsync();

                    var pageSource = selenium.GetPageSource();

                    FileHelper.SaveToFile(platform.SubFolder, platform.Name, pageSource);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Error in Main");
                }
            }
        }
    }

    private static IConfiguration InitializeConfiguration()
    {
        var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        return builder.Build();
    }
}