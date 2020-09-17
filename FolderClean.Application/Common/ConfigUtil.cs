using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace FolderClean.Application.Common
{
    /// <summary>
    /// Stores App
    /// </summary>
    public static class ConfigUtil
    {
        public static ConfigApp Config { get; set; }

        /// <summary>
        /// Reading Config file from Json
        /// </summary>
        /// <returns></returns>
        public static (bool,string) ReadConfigFile()
        {
            try
            {
                // Get Current Folder Location
                var folder = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory?.FullName;
                var json = File.ReadAllText(folder + "/config.json");
                Config = JsonSerializer.Deserialize<ConfigApp>(json);
                if (string.IsNullOrWhiteSpace(Config.DestinationFolder) || string.IsNullOrWhiteSpace(Config.SourceFolder))
                {
                    return (false, "Source Folder and Destination Folder cannot be Empty");
                }
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
            return (true, "");
        }
    }
}