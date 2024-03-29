using System.Reflection;

namespace OTTSpiderBotHtmlDocument.Helpers
{
    public static class FileHelper
    {
        private static readonly string _baseFolderName = "OTTHtmlLog";

        /// <summary>
        /// 储存文件到指定路径，按年份和月份分类文件。
        /// </summary>
        /// <param name="subfolder">子文件夹名</param>
        /// <param name="filename">文件名，不包含日期后缀</param>
        /// <param name="content">要保存的内容</param>
        public static void SaveToFile(string subfolder, string filename, string content)
        {
            var filePath = BuildFilePath(subfolder, filename);
            File.WriteAllText(filePath, content);
            Console.WriteLine($"内容已保存到：{filePath}");
            Program.Logger.Info($"内容已保存到：{filePath}");
        }

        private static string BuildFilePath(string subfolder, string filename)
        {
            var dateSuffix = DateTime.Now.ToString("yyyyMMdd");
            var yearFolder = DateTime.Now.ToString("yyyy");
            var monthFolder = DateTime.Now.ToString("MM");
            filename += $"_{dateSuffix}.txt";

            var baseFolderPath = GetBaseFolderPath();
            var targetFolderPath = Path.Combine(baseFolderPath, subfolder, yearFolder, monthFolder);

            EnsureDirectoryExists(targetFolderPath);

            return Path.Combine(targetFolderPath, filename);
        }

        private static string GetBaseFolderPath()
        {
            var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory;
            return Path.Combine(exePath, _baseFolderName);
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
