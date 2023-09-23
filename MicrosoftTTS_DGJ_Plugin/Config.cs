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
        public string subscriptionKey { get; set; }
        [JsonProperty("MsRegion")]
        public string subscriptionRegion { get; set; }
        [JsonProperty("VoiceName")]
        public string VoiceName { get; set; }
        [JsonProperty("VoiceStyle")]
        public string VoiceStyle { get; set; } = "neutral";

        [JsonProperty("enableProxy")]
        public bool enableProxy { get; set; } = false;

        [JsonProperty("proxyServer")]
        public string proxyServer { get; set; } = "example.com";

        [JsonProperty("proxyServerPort")]
        public int proxyServerPort { get; set; } = 7070;

        [JsonProperty("proxyServerUser")]
        public string proxyServerUser { get; set; }

        [JsonProperty("proxyServerPassword")]
        public string proxyServerPassword { get; set; }

        [JsonProperty("volume")]
        public string volume { get; set; } = "medium";

        [JsonProperty("biliComentSpeech")]
        public bool BiliComentSpeech { get; set; } = false;

        [JsonProperty("UseWinTts")]
        public bool UseWinTts { get; set; } = false;

        [JsonProperty("UseSampleMode")]
        public bool UseSampleMode { get; set; } = true;

        [JsonProperty("CharacterCount")]
        public long CharacterCount { get; set; } = 0;

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
