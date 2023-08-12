using BilibiliDM_PluginFramework;
using DGJv3.API;
using DGJv3.InternalModule;
using MicrosoftTTS_DGJ_Plugin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace DGJv3
{
    public class DanmuHandler : INotifyPropertyChanged
    {
        private bool _biliComentSpeech;
        public bool BiliComentSpeech
        {
            get => _biliComentSpeech;
            set => SetField(ref _biliComentSpeech, value, nameof(BiliComentSpeech));
        }

        private Dispatcher dispatcher;

        private MicrosoftTTS MicrosoftTTS;

        private static string loker = Guid.NewGuid().ToString();

        /// <summary>
        /// 最多点歌数量
        /// </summary>
        public uint MaxTotalSongNum { get => _maxTotalSongCount; set => SetField(ref _maxTotalSongCount, value, nameof(MaxTotalSongNum)); }
        private uint _maxTotalSongCount;

        public bool AdminCmdEnable { get => _adminCmdEnable; set => SetField(ref _adminCmdEnable, value, nameof(AdminCmdEnable)); }
        private bool _adminCmdEnable;

        internal DanmuHandler(MicrosoftTTS microsoftTTS)
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            MicrosoftTTS = microsoftTTS;
        }



        private void Speaking(string text)
        {
            MicrosoftTTS?.Speaking(text);
        }

        public void ProcessCmdResult(string msg)
        {
            if (BiliComentSpeech)
            {
                Log(msg);
                Speaking(msg);
            }
        }

        /// <summary>
        /// 处理弹幕消息
        /// <para>
        /// 注：调用侧可能会在任意线程
        /// </para>
        /// </summary>
        /// <param name="danmakuModel"></param>
        internal void ProcessDanmu(DanmakuModel danmakuModel)
        {
            ProcessCmdResult(danmakuModel.CommentText);
        }

        private readonly static char[] SPLIT_CHAR = { ' ' };

        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        public event LogEvent LogEvent;
        private void Log(string message, Exception exception = null) => LogEvent?.Invoke(this, new LogEventArgs() { Message = message, Exception = exception });
    }
}
