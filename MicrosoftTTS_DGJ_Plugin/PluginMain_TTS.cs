using BilibiliDM_PluginFramework;
using DGJv3;
using DGJv3.API;
using DGJv3.InternalModule;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MicrosoftTTS_DGJ_Plugin
{
    public class PluginMain_TTS : DMPlugin
    {

        private MainWindow _mainWindow;

        private VersionChecker versionChecker;

        public string DownloadUpdateUrl = "";
        public PluginMain_TTS()
        {
            try
            {
                var info = Directory.CreateDirectory(Utilities.BinDirectoryPath);
                info.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            catch (Exception) { }

            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
            this.PluginName = Utilities.PluginName;
            this.PluginAuth = Utilities.PluginAuth;
            this.PluginCont = Utilities.PluginCont;
            this.PluginDesc = Utilities.PluginDesc;
            this.PluginVer = Assembly.GetExecutingAssembly().GetName().Version.ToString(4);
            base.Start();
            LoadAllReferencedAssemblies();
            versionChecker = new VersionChecker("MicrosoftTTS_DGJ_Plugin");
            Task.Run(() =>
            {
                if (versionChecker.FetchInfoFromGithub())
                {
                    Version current = null;

                    try
                    {
                        current = new Version(Assembly.GetExecutingAssembly().GetName().Version.ToString(4));
                    }
                    catch (Exception)
                    {

                    }

                    if (versionChecker.HasNewVersion(current))
                    {
                        Log("插件有新版本" + Environment.NewLine +
                            $"当前版本：{Assembly.GetExecutingAssembly().GetName().Version.ToString(4)}" + Environment.NewLine +
                            $"最新版本：{versionChecker.Version.ToString()} 更新时间：{versionChecker.UpdateDateTime.ToShortDateString()}" + Environment.NewLine +
                            $"更新包下载地址： ↘↘↘↘↘");
                        Log(versionChecker.DownloadUrl.AbsoluteUri);
                        Log(versionChecker.UpdateDescription);
                        DownloadUpdateUrl = versionChecker.DownloadUrl.AbsoluteUri;
                    }
                }
                else
                {
                    Log("版本检查出错：" + versionChecker?.LastException?.Message);
                }
            });
        }

        public override void Inited()
        {
            try
            {
                _mainWindow = new MainWindow(this);
                InjectDGJ();
            }
            catch (DllNotFoundException)
            {
                MessageBox.Show("你还没有安装点歌姬v3插件, 请先安装再使用本插件, 本插件加载将被取消。", "微软TTS插件For点歌姬", 0, MessageBoxImage.Error);
                throw;
            }
            catch (Exception e)
            {
                MessageBox.Show($"插件初始化失败了喵,请将桌面上的错误报告发送给作者（/TДT)/\n{e}", "微软TTS插件For点歌姬", 0, MessageBoxImage.Error);
                throw;
            }

        }
        public override void DeInit() => _mainWindow.DeInit();

        public override void Admin()
        {
            _mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _mainWindow.Show();
            _mainWindow.Topmost = true;
            _mainWindow.Topmost = false;
            _mainWindow.Activate();
            if (string.IsNullOrEmpty(DownloadUpdateUrl) == false)
            {
                //if (MessageBox.Show("微软TTS插件For点歌姬发现新版本，是否前往下载", "版本更新", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                //{
                //    Process.Start(versionChecker.UpdatePage.AbsoluteUri);
                //}
                DownloadUpdateUrl = "";
                Update update = new Update(versionChecker);
                update.Title = "版本更新";
                update.ShowDialog();
            }
        }

        public override void Start()
        {
            Log("启用此插件可以在点歌姬中开启此功能");
        }
        public override void Stop()
        {
            Log("禁用此插件可以在点歌姬中关闭此功能");
        }

        private void InjectDGJ()
        {
            try
            {
                Assembly dgjAssembly = Assembly.GetAssembly(typeof(WindowsTTS)); //如果没有点歌姬插件，插件的构造方法会抛出异常，无需考虑这里的assembly == null的情况
                DMPlugin dgjPlugin = Bililive_dm.App.Plugins.FirstOrDefault(p => p is DGJMain);
                if (dgjPlugin == null) // 没有点歌姬
                {
                    throw new DllNotFoundException();
                }
                object dgjWindow = null;
                try
                {
                    dgjWindow = dgjAssembly.DefinedTypes.FirstOrDefault(p => p.Name == "DGJMain").GetField("window", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(dgjPlugin);
                }
                catch (ReflectionTypeLoadException e) // 缺少登录中心时
                {
                    dgjWindow = e.Types.FirstOrDefault(p => p.Name == "DGJMain").GetField("window", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(dgjPlugin);
                }
                object TTSPlugin = dgjWindow.GetType().GetProperty("TTSPlugin", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public).GetValue(dgjWindow);

                var wtts = (WindowsTTS)dgjWindow.GetType().GetProperty("WindowsTTS", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public).GetValue(dgjWindow);
                if (wtts != null)
                {
                    Type type = wtts.GetType();  // 获取包含方法的类型
                    MethodInfo methodInfo = type.GetMethod("Log", BindingFlags.NonPublic | BindingFlags.Instance);  // 获取方法的 MethodInfo 对象

                    _mainWindow.MicrosoftTTS.LogEvent += (object sender, LogEventArgs e) =>
                    {
                        methodInfo?.Invoke(wtts, new object[] { $"{Utilities.PluginName}:{e.Message}", e.Exception });
                    };
                }
                MethodInfo methodInfo2 = TTSPlugin.GetType().GetMethod("AddModule", BindingFlags.Instance | BindingFlags.Public);
                if (methodInfo2 != null)
                {
                    //单线程加入插件模块
                    methodInfo2.Invoke(TTSPlugin, new object[] { _mainWindow.MicrosoftTTS });
                }
                else
                {
                    //兼容旧的方法
                ObservableCollection<TTS> TTSlist = (ObservableCollection<TTS>)dgjWindow.GetType().GetProperty("TTSlist", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public).GetValue(dgjWindow);
                    TTSlist.Insert(TTSlist.Count - 1 > -1 ? TTSlist.Count - 1 : 0, _mainWindow.MicrosoftTTS);
                }
            }
            catch (DllNotFoundException ex)
            {
                throw;
            }
            catch (Exception e)
            {
                MessageBox.Show($"注入到点歌姬失败了\n{e}", "微软TTS插件For点歌姬", 0, MessageBoxImage.Error);
                throw;
            }
        }

        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = new AssemblyName(args.Name);

            var path = assemblyName.Name + ".dll";
            string filepath = Path.Combine(Utilities.BinDirectoryPath, path);

            if (assemblyName.CultureInfo?.Equals(CultureInfo.InvariantCulture) == false)
            {
                path = string.Format(@"{0}\{1}", assemblyName.CultureInfo, path);
            }

            using (Stream stream = executingAssembly.GetManifestResourceStream(path))
            {
                if (stream == null)
                {
                    if (File.Exists(filepath))
                    {
                        return Assembly.LoadFrom(filepath);
                    }
                    return null;
                }

                var assemblyRawBytes = new byte[stream.Length];
                stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                try
                {
                    File.WriteAllBytes(filepath, assemblyRawBytes);
                }
                catch (Exception) { }
            }

            return Assembly.LoadFrom(filepath);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool SetDllDirectory(string path);

        private static void LoadAllReferencedAssemblies()
        {
            string defaultSearchPath = $"{Environment.SystemDirectory};{Environment.CurrentDirectory}";
            string specifiedSearchPath = Utilities.BinDirectoryPath;
            string combinedSearchPath = $"{specifiedSearchPath};{defaultSearchPath}";

            // 设置加载路径为合并后的搜索路径
            SetDllDirectory(combinedSearchPath);

            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames();

            var executingAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;

            foreach (string resourceName in resourceNames)
            {
                using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
                {

                    if (resourceStream != null && resourceName.EndsWith(".dll"))
                    {
                        var prefix = $"{executingAssemblyName}.Reference.";
                        int index = resourceName.IndexOf(prefix);
                        string fileName = resourceName;
                        if (index > -1)
                        {
                            fileName = Path.GetFileName(resourceName.Remove(index, prefix.Length));
                            fileName = fileName.TrimStart('.');
                        }
                        string filePath = Path.Combine(Utilities.BinDirectoryPath, fileName);

                        using (FileStream fileStream = File.Create(filePath))
                        {
                            resourceStream.CopyTo(fileStream);
                        }
                    }
                }
            }

        }
    }
}
