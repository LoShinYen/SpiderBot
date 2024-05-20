using Microsoft.Extensions.Configuration;
using NLog;
using OTTSpiderBotHtmlDocument.Helpers;
using OTTSpiderBotHtmlDocument.Models;

class Program
{ 
    internal static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    static async Task Main(string[] args)
    {
        Console.WriteLine("Application start!");
        IConfiguration configuration = InitializeConfiguration();
        
        var crawlSettings = configuration.GetSection("CrawlSettings").Get<CrawlSettings>() ?? new CrawlSettings();

        try
        {
            Console.WriteLine("Start crawling...");
            foreach (var platform in crawlSettings.Platforms)
            {
                Console.WriteLine("Foreach platform...");
                using (var selenium = new SeleniumHelper())
                {
                    Console.WriteLine("Using selenium...");
                    try
                    {
                        selenium.NavigateTo(platform.Url);

                        await selenium.WaitForPageLoadCompleteAsync(platform.RequiresExtendedLoading);

                        var pageSource = selenium.GetPageSource();

                        FileHelper.SaveToFile(platform.SubFolder, platform.Name, pageSource);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Logger.Error(e.Message);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            await Task.Delay(60000);
            Logger.Error(e.Message);
        }
        finally
        {
            Console.WriteLine("Application end!");
            await Task.Delay(60000);
        }
    }

    private static IConfiguration InitializeConfiguration()
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var builder = new ConfigurationBuilder()
           .SetBasePath(basePath)
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        return builder.Build();
    }
}