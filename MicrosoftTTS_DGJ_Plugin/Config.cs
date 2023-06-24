using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MicrosoftTTS_DGJ_Plugin
{
    public class Config
    {
       
        [JsonProperty("MsKey")]
        public string subscriptionKey { get; set; }="asaa";
        [JsonProperty("MsRegion")]
        public string subscriptionRegion { get; set; }
        [JsonProperty("VoiceName")]
        public string VoiceName { get; set; }
        [JsonProperty("VoiceStyle")]
        public string VoiceStyle { get; set; } = "neutral";

        internal static Config Load(bool reset = false)
        {
            Config config = new Config();
            if (!reset)
            {
                try
                {
                    var str = File.ReadAllText(Utilities.ConfigFilePath, Encoding.UTF8);
                    config = JsonConvert.DeserializeObject<Config>(str);
                }

                catch (Exception ex)
                {
                }
            }
            return config;
        }
        internal static void Write(Config config)
        {
            try
            {
                Formatting fmt = Formatting.Indented;
                File.WriteAllText(Utilities.ConfigFilePath, JsonConvert.SerializeObject(config, fmt), Encoding.UTF8);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
