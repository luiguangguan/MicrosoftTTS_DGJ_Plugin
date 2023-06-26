﻿using DGJv3.API;
using DGJv3.InternalModule;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MicrosoftTTS_DGJ_Plugin
{
    public class MicrosoftTTS : TTS, INotifyPropertyChanged
    {
        private string _subscriptionKey;
        public string subscriptionKey
        {
            get => _subscriptionKey;
            set => SetField(ref _subscriptionKey, value, nameof(subscriptionKey));
        }

        private string _subscriptionRegion;
        public string subscriptionRegion
        {
            get => _subscriptionRegion;
            set => SetField(ref _subscriptionRegion, value, nameof(subscriptionRegion));
        }

        private string _VoiceName;
        public string VoiceName
        {
            get => _VoiceName;
            set => SetField(ref _VoiceName, value, nameof(VoiceName));
        }

        private string _VoiceStyle;
        public string VoiceStyle
        {
            get => _VoiceStyle;
            set => SetField(ref _VoiceStyle, value, nameof(VoiceStyle));
        }

        private bool _enableProxy = false;
        public bool EnableProxy
        {
            get => _enableProxy;
            set => SetField(ref _enableProxy, value, nameof(EnableProxy));
        }

        private string _proxyServer = "http://example.com";
        public string ProxyServer
        {
            get => _proxyServer;
            set => SetField(ref _proxyServer, value, nameof(ProxyServer));
        }

        private int _proxyServerPort = 7070;
        public int ProxyServerPort
        {
            get => _proxyServerPort;
            set => SetField(ref _proxyServerPort, value, nameof(ProxyServerPort));
        }

        private string _proxyServerUser;
        public string ProxyServerUser
        {
            get => _proxyServerUser;
            set => SetField(ref _proxyServerUser, value, nameof(ProxyServerUser));
        }

        private string _proxyServerPassword;
        public string ProxyServerPassword
        {
            get => _proxyServerPassword;
            set => SetField(ref _proxyServerPassword, value, nameof(ProxyServerPassword));
        }

        private string _volume;
        public string Volume
        {
            get => _volume;
            set => SetField(ref _volume, value, nameof(Volume));
        }

        private ObservableCollection<AiRoles> RolesList { get; set; }
        public ObservableCollection<ComboBoxItem> AiVoiceStyleComboBoxList { get; set; }
        public ObservableCollection<ComboBoxItem> AiRoleComboBoxList { get; set; }

        public string TestText { get; set; } = "今天天气还不错哦~";

        public MicrosoftTTS(string Name, string Author, string Contact, ObservableCollection<AiRoles> rolesList) : base(Name, Author, Contact)
        {
            RolesList = rolesList;
            AiRoleComboBoxList = new ObservableCollection<ComboBoxItem>();
            AiVoiceStyleComboBoxList = new ObservableCollection<ComboBoxItem>();

            PropertyChanged += This_PropertyChanged;
            RolesList.CollectionChanged += RolesList_CollectionChanged;
            //style.CollectionChanged += RolesList_CollectionChanged;
            AiRoleComboBoxList.CollectionChanged += AiRoleComboBoxList_CollectionChanged;
        }

        private void AiRoleComboBoxList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //var tn = VoiceStyle;
            //VoiceStyle = "";
            //VoiceStyle = tn;
        }
        private void RolesList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            AiRoleComboBoxList.Clear();
            foreach (var role in RolesList)
            {
                ComboBoxItem newItem1 = new ComboBoxItem();
                newItem1.Content = $"{role.LocalName}({(role.Gender == "Female" ? "女" : "男")}{role.LocaleName})";
                newItem1.Tag = role.ShortName;
                AiRoleComboBoxList.Add(newItem1);
                if (role.ShortName == VoiceName)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VoiceName)));
                }
            }
        }


        public override async void Speaking(string text)
        {
            try
            {

                Log($"收到语音文字：{text}");
                var config = SpeechConfig.FromSubscription(subscriptionKey, subscriptionRegion);
                // Note: the voice setting will not overwrite the voice element in input SSML.
                //config.SpeechSynthesisVoiceName = "zh-CN-XiaoxiaoNeural";
                // use the default speaker as audio output.

                if (EnableProxy)
                {
                    if (Regex.IsMatch(ProxyServer, "^[\\w.-]+(:\\d+)?$") == false)
                    {
                        Log("代理服务器地址错误");
                        return;
                    }
                    if (ProxyServerPort > 65535 || ProxyServerPort < 0)
                    {
                        Log("代理服务器端口范围错误，应在（0-65535）内");
                        return;
                    }
                    if (string.IsNullOrEmpty(ProxyServerUser) && string.IsNullOrEmpty(ProxyServerPassword))
                    { config.SetProxy(ProxyServer, ProxyServerPort); }
                    else
                    {
                        config.SetProxy(ProxyServer, ProxyServerPort, ProxyServerUser, ProxyServerPassword);
                    }
                }

                // 创建自定义的 AudioOutputFormat 实例
                ////var outputFormat = Microsoft.CognitiveServices.Speech.Audio.AudioOutputFormat.CreateNonPCM();

                //// 创建 SpeechSynthesizer 实例，并设置 AudioConfig

                //audioConfig.AudioProcessingOptions.
                using (var synthesizer = new Microsoft.CognitiveServices.Speech.SpeechSynthesizer(config, null))
                {
                    //synthesizer.
                    string mstts = $@"<speak version=""1.0"" xmlns=""http://www.w3.org/2001/10/synthesis""
       xmlns:mstts=""https://www.w3.org/2001/mstts"" xml:lang=""zh-CN"">
    <voice name=""{VoiceName}"">
        <mstts:express-as style=""{VoiceStyle}"" styledegree=""2"">
        <prosody volume='{(string.IsNullOrEmpty(Volume) ? "medium" :Volume)}'>{text}</prosody>
        </mstts:express-as>
    </voice>
</speak>";
                    //synthesizer.c
                    //Log(mstts);
                    synthesizer.SynthesisStarted += Synthesizer_SynthesisStarted;
                    synthesizer.SynthesisCompleted += Synthesizer_SynthesisCompleted;
                    synthesizer.SynthesisCanceled += Synthesizer_SynthesisCanceled;
                    using (var result = await synthesizer.SpeakSsmlAsync(mstts))
                    {
                        //if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                        //{
                        //    //using (var stream = new System.IO.MemoryStream(result.AudioData))
                        //    //{
                        //    //    // 创建一个 SoundPlayer 实例，并将音频数据流作为输入
                        //    //    using (var player = new SoundPlayer(stream))
                        //    //    {
                        //    //        // 播放声音
                        //    //        player.Play();
                        //    //        Log($"已播放语音：{text}");
                        //    //    }
                        //    //}
                        //    Log($"已播放语音：{text}");
                        //    Log($"Speech synthesized for text [{text}]");
                        //}
                        //else if (result.Reason == ResultReason.Canceled)
                        //{
                        //    var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                        //    Log($"CANCELED: Reason={cancellation.Reason}");

                        //    if (cancellation.Reason == CancellationReason.Error)
                        //    {
                        //        Log($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        //        Log($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                        //        Log($"CANCELED: Did you update the subscription info?");
                        //    }
                        //}
                        //else
                        //{
                        //    Log($"调用语音接口返回代码：[{result.Reason}]，{text}");
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Log("出错了", ex);
            }
        }

        private void Synthesizer_SynthesisCanceled(object sender, SpeechSynthesisEventArgs e)
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(e.Result);
            Log($"CANCELED: Reason={cancellation.Reason}");

            if (cancellation.Reason == CancellationReason.Error)
            {
                Log($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                Log($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                Log($"CANCELED: Did you update the subscription info?");
            }
        }

        private void Synthesizer_SynthesisCompleted(object sender, SpeechSynthesisEventArgs e)
        {
            Log($"发音完成：{e.Result.ResultId}");
            using (var stream = new System.IO.MemoryStream(e.Result.AudioData))
            {
                // 创建一个 SoundPlayer 实例，并将音频数据流作为输入
                using (var player = new SoundPlayer(stream))
                {
                    // 播放声音
                    player.Play();
                    Log($"已播放语音{e.Result.ResultId}");
                }
            }
        }

        private void Synthesizer_SynthesisStarted(object sender, SpeechSynthesisEventArgs e)
        {
            Log($"发音开始：{e.Result.ResultId}");
        }

        private void This_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VoiceName))
            {
                if (RolesList != null)
                {
                    var roles = RolesList.FirstOrDefault(p => p.ShortName == VoiceName);
                    if (roles != null && roles.StyleList?.Length > 0)
                    {
                        AiVoiceStyleComboBoxList.Clear();
                        foreach (var style in roles.StyleList)
                        {
                            System.Windows.Controls.ComboBoxItem newItem1 = new System.Windows.Controls.ComboBoxItem();
                            newItem1.Content = AiVoiceStyleIndex.GetName(style);
                            newItem1.Tag = style;
                            AiVoiceStyleComboBoxList.Add(newItem1);
                        }
                    }
                    else
                    {
                        this.VoiceStyle = "";
                        AiVoiceStyleComboBoxList.Clear();
                    }
                }
            }
            else if (e.PropertyName == nameof(RolesList))
            {

            }

        }

        public void SetLogHandler(Action<string> logHandler)
        {
            this.GetType().GetProperty("_log", BindingFlags.SetProperty | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, logHandler);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
