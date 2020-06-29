using System;
using System.IO;
using System.Reflection;

namespace An.Editor.Util
{
    public class Config
    {
        private static readonly string appData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.json");

        public static Config All { get; private set; } = new Config();

        static Config()
        {
           
            //Loads AppData settings.
            if (File.Exists(appData))
            {
                string s = File.ReadAllText(appData);
                if (!string.IsNullOrWhiteSpace(s))
                {
                    try
                    {
                        All = System.Text.Json.JsonSerializer.Deserialize<Config>(s);
                    }
                    catch { }
                }
            }

      
        }

        public static void Save()
        {

            var folder = Path.GetDirectoryName(appData);
            if (!string.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder))
                Directory.CreateDirectory(folder);



            #region Create folder


            File.WriteAllText(appData,
                System.Text.Json.JsonSerializer.Serialize(All,
                new System.Text.Json.JsonSerializerOptions
                {
                    IgnoreNullValues = true,
                    AllowTrailingCommas = true
                })
             );

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


        /// <summary>
        /// 撤销操作记录限制
        /// </summary>
        public int HistoryLimit { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public string TemporaryFolder { get; set; }
         

        public string TemporaryFolderResolved
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TemporaryFolder))
                    TemporaryFolder = "%temp%";

                return Environment.ExpandEnvironmentVariables(TemporaryFolder);
            }
        }

    }
}
