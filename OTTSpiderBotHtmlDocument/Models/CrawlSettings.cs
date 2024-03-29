namespace OTTSpiderBotHtmlDocument.Models
{
    public class CrawlSettings
    {
        public int WaitSeconds { get; set; }
        public bool Headless { get; set; }
        public List<Platform> Platforms { get; set; } = new List<Platform>();
    }

    public class Platform
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string SubFolder { get; set; } = string.Empty;
    }
}
