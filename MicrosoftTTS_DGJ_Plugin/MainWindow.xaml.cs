using DGJv3;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace MicrosoftTTS_DGJ_Plugin
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        internal UniversalCommand TestSpeakingCommand { get; private set; }

        public MicrosoftTTS MicrosoftTTS { get; set; }
        public string TestText { get; set; } = "语音测试，今天天气还不错哦~";
        public ObservableCollection<AiRoles> AiRoleList { get; set; }


        public MainWindow()
        {
            
            InitializeComponent();

            AiRoleList = new ObservableCollection<AiRoles>();
            MicrosoftTTS = new MicrosoftTTS(Utilities.PluginName, Utilities.PluginAuth, Utilities.PluginCont, AiRoleList);
            ApplyConfig(Config.Load());
            InitAiRoles();

            TestSpeakingCommand = new UniversalCommand(TestSpeakingClick);
            DataContext = this;

            this.Closing += MainWindow_Closing;
        }
       
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            DeInit();
        }

        static MainWindow()
        {
            try
            {
                Directory.CreateDirectory(Utilities.DataDirectoryPath);
            }
            catch (Exception) { }
        }
        private void ApplyConfig(Config config)
        {
            MicrosoftTTS.subscriptionKey = config.subscriptionKey;
            MicrosoftTTS.subscriptionRegion = config.subscriptionRegion;
            MicrosoftTTS.VoiceName = config.VoiceName;
            MicrosoftTTS.VoiceStyle = config.VoiceStyle;
            MicrosoftTTS.EnableProxy = config.enableProxy;
            MicrosoftTTS.ProxyServer = config.proxyServer;
            MicrosoftTTS.ProxyServerPort = config.proxyServerPort;
            MicrosoftTTS.ProxyServerUser = config.proxyServerUser;
            MicrosoftTTS.ProxyServerPassword = config.proxyServerPassword;
            MicrosoftTTS.Volume = config.volume;
        }
        private void TestSpeakingClick(object parameter)
        {
            MicrosoftTTS.Speaking(TestText);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MicrosoftTTS.Speaking(TestText);
        }

        /// <summary>
        /// 收集设置
        /// </summary>
        /// <returns></returns>
        private Config GatherConfig() => new Config()
        {
            subscriptionKey = MicrosoftTTS.subscriptionKey,
            subscriptionRegion = MicrosoftTTS.subscriptionRegion,
            VoiceName = MicrosoftTTS.VoiceName,
            VoiceStyle = MicrosoftTTS.VoiceStyle,
            enableProxy = MicrosoftTTS.EnableProxy,
            proxyServer = MicrosoftTTS.ProxyServer,
            proxyServerPort = MicrosoftTTS.ProxyServerPort,
            proxyServerUser = MicrosoftTTS.ProxyServerUser,
            proxyServerPassword = MicrosoftTTS.ProxyServerPassword,
            volume = MicrosoftTTS.Volume,
        };

        internal void DeInit()
        {
            Config.Write(GatherConfig());

        }

        private AiRoles[] InitAiRoles()
        {
            AiRoleList.Clear();
            var roles = AiRoles.Load();
            roles= roles.OrderBy(p=>p.Name).ToArray();
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    //ComboBoxItem newItem1 = new ComboBoxItem();
                    //newItem1.Content = $"{role.LocalName}({(role.Gender == "Female" ? "女" : "男")})";
                    //newItem1.Tag = role.ShortName;
                    //cbAiRoles.Items.Add(newItem1);
                    //AiRoleComboBoxList.Add(newItem1);
                    AiRoleList.Add(role);
                }
            }
            return roles;
        }


    }
}
