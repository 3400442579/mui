using Animation.Editor.Utils;
using DH.MUI.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;

namespace Animation.Editor.Core
{
    public class Config: NotifyPropertyChanged
    {
        public static Config Exa { get; } = new Config();

        public static string AppName = "Animation";
        static Config()
        {
            string appData = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GifMake"), "Config.json");

            //Loads AppData settings.
            if (File.Exists(appData))
            {
                try
                {
                    Exa = Json.DeserializeByFile<Config>(appData);
                }
                catch { }
            }

            // if (Instance == null)
            //     Instance = new Config();
        }

        public static void Save()
        {
            try
            {
                string appData1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GifMake", "Config.json");
                #region Create folder

                var folder1 = Path.GetDirectoryName(appData1);

                if (!string.IsNullOrWhiteSpace(folder1) && !Directory.Exists(folder1))
                    Directory.CreateDirectory(folder1);

                Json.SerializeToFile(Exa, appData1);
                 
            }
            catch (Exception e)
            {
                //LogWriter.Log(e, "Saving settings");
            }

            #endregion
        }

        #region Options • Temporary Files

       

        public string TemporaryFolder { get; set; }

        [JsonIgnore]
        public string TemporaryFolderResolved
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TemporaryFolder))
                    TemporaryFolder = "%temp%";

                return Environment.ExpandEnvironmentVariables(TemporaryFolder);
            }
        }

        #endregion



    }
}
