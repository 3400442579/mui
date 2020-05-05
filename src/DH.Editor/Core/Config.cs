using System;
using System.IO;
using System.Reflection;

namespace DH.Editor.Core
{
    public class Config
    {
        private static readonly string appData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.json");

        public static Config Instance { get; private set; } = new Config();

        static Config()
        {
            //Loads AppData settings.
            if (File.Exists(appData))
            {
                string s=  File.ReadAllText(appData);
                if (!string.IsNullOrWhiteSpace(s))
                {
                    try
                    {
                        Instance = System.Text.Json.JsonSerializer.Deserialize<Config>(s);
                    }
                    catch { }
                }
            }

            // if (Instance == null)
            //     Instance = new Config();
        }

        public static void Save()
        {
            #region Create folder


            File.WriteAllText(appData, System.Text.Json.JsonSerializer.Serialize(Instance,
                new System.Text.Json.JsonSerializerOptions { IgnoreNullValues = true, AllowTrailingCommas = true }
                ));

            #endregion
        }

        //[JsonIgnore]
        public static string Version => Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "1.0.0.0";



        /// <summary>
        /// 
        /// </summary>
        public string Theme { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccentColor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Lang { get; set; }

    }
}
