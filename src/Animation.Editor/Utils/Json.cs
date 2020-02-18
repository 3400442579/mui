using System.IO;
using System.Text;
using System.Text.Json;

namespace Animation.Editor.Utils
{
    public class Json
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string s, JsonSerializerOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(s))
                return default;

            JsonSerializerOptions o = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };

            if (options != null)
                o = options;

            return JsonSerializer.Deserialize<T>(s, o);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static T DeserializeByFile<T>(string file, JsonSerializerOptions options = null)
        {
            string s= File.ReadAllText(file, Encoding.UTF8);
            if (string.IsNullOrWhiteSpace(s))
                return default;

            if (options != null)
                return JsonSerializer.Deserialize<T>(s, options);
            else
                return JsonSerializer.Deserialize<T>(s);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string Serialize(object v, JsonSerializerOptions options = null)
        {
            if (v == null)
                return null;

            if (options == null)
                return JsonSerializer.Serialize(v);
            else
                return JsonSerializer.Serialize(v, options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="save"></param>
        /// <param name="options"></param>
        public static void SerializeToFile(object v, string save, JsonSerializerOptions options = null)
        {
            string json;
            if (v == null)
                json = "";
            else
            {
                if (options == null)
                    json = JsonSerializer.Serialize(v);
                else
                    json = JsonSerializer.Serialize(v, options);
            }

            FileInfo fileInfo = new FileInfo(save);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();
            File.WriteAllText(save, json, Encoding.UTF8);
        }
    }


}
