using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MicrosoftTTS_DGJ_Plugin
{
    internal class Utilities
    {
        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        internal static readonly string DataDirectoryPath = Path.Combine(AssemblyDirectory, "MicrosoftTTS");
        internal static readonly string BinDirectoryPath = Path.Combine(DataDirectoryPath, "bin");
        internal static readonly string ConfigFilePath = Path.Combine(DataDirectoryPath, "config.json");
        internal static readonly string RolesFilePath = Path.Combine(DataDirectoryPath, "AI_Role.json");
        internal static readonly string PluginName = "微软TTS模块AI版插件For点歌姬";
        internal static readonly string PluginAuth = "Simon";
        internal static readonly string PluginCont = "请在github提issue";
        internal static readonly string PluginDesc = "点歌姬AI发音用";
    }
}
