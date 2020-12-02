using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using Emgu.CV;
using VideoClient.Utils;
using System.Drawing;
using System.Runtime.InteropServices;
using VideoClient.Models;
using VideoClient.Custom;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Grpc.Core;
using FaceClient;
using System.Collections.Generic;
using System.Timers;

namespace VideoClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _showVideoTimer = new DispatcherTimer();
        private Timer _detectVideoTimer = new Timer();
        private Timer _checkConnectTimer = new Timer();
        private VideoCapture _videCaptureShow;
        private Mat mat;
        private string _playFile;
        private bool _playStatus;
        private bool _detectStatus;
        private bool _checkStatus;
        Arcsoft_Face_Action _enginePool;
        private string _appID;
        private string _faceKey;
        private int _engineNums;
        private GrpcChannel _grpcChannel;
        private IConfiguration _config;
        private FaceRecongnise.FaceRecongniseClient _client;
        private bool _complete;
        private static object _obj = new object();

        public MainWindow()
        {
            InitializeComponent();

            _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .Build();

            InitializeEngine(_config);
            InitializeGRPC(_config);
            InitializeTimers();

            _playStatus = false;
            _detectStatus = false;
            _checkStatus = false;
            _complete = true;

        }

        private void InitializeEngine(IConfiguration conf)
        {
            IConfiguration config = conf;
            _appID = config["AppID"];
            _faceKey = config["AppKey"];
            _engineNums = int.Parse(config["EngineNums"]);

            _enginePool = new Arcsoft_Face_Action(_appID, _faceKey);
            _enginePool.Arcsoft_EnginePool(_engineNums, 0, 0);
        }

        private void InitializeTimers()
        {
            //Show Video
            _showVideoTimer.Tick += ShowVideoTick;
            _showVideoTimer.Interval = TimeSpan.FromSeconds(0.02);

            //Detect Feature
            _detectVideoTimer.Elapsed += DetectFaceTick;
            _detectVideoTimer.Interval = 300;

            //Check Connect
            _checkConnectTimer.Elapsed += CheckConnectTick;
            _checkConnectTimer.Interval = 100;
        }

        private void InitializeGRPC(IConfiguration conf)
        {
            IConfiguration config = conf;
            _grpcChannel = GrpcChannel.ForAddress(config["gRPC_Server"]);
            _client = new FaceRecongnise.FaceRecongniseClient(_grpcChannel);
        }

        private void ShowVideoTick(object sender, EventArgs e)
        {
            try
            {
                Mat originalMat = _videCaptureShow.QueryFrame();
                lock (_obj)
                {
                    mat = originalMat;
                }               
                if (originalMat != null)
                {
                    Video_Show.Source = ImageHelper.BitmapToBitmapImage(originalMat.ToBitmap());
                }
            }
            catch
            {

            }
        }

        private async void DetectFaceTick(object sender, ElapsedEventArgs e)
        {
            Mat currentMat;
            lock (_obj)
            {
                currentMat = mat;
            }
            List<MarkFaceInfor> markFaceInfors = ExtractFaceData(currentMat, _enginePool);
            if (markFaceInfors == null)
            {
                return;
            }
            if (markFaceInfors.Count==0)
            {
                return;
            }
            while(!_complete)
            {
                Task.Delay(10).Wait();
            }
            _complete = false;
            var regFace = _client.RecongnizationByFace();

            //定义接收响应逻辑                       
            var regFaceResponseTask = Task.Run(async () =>
            {
                WriteReceiveMsgAsync(string.Format("当前接收时间{0}", DateTime.Now.ToString("HH:mm:ss:fff")));
                await foreach (var resp in regFace.ResponseStream.ReadAllAsync())
                {                  
                    WriteReceiveMsgAsync($"姓名：{resp.PersonName}，相似度：{resp.ConfidenceLevel}");
                }
            });

            //开始调用           
            WriteSendMsgAsync(string.Format("开始发送时间{0}", DateTime.Now.ToString("HH:mm:ss:fff")));
            for (int index = 0; index < markFaceInfors.Count; index++)
            {
                WriteSendMsgAsync($"发送编号：{index}");
                await regFace.RequestStream.WriteAsync(new FaceRequest()
                {
                    FaceFeature = Google.Protobuf.ByteString.CopyFrom(markFaceInfors[index].faceFeatureData)
                });
            }           
            await regFace.RequestStream.CompleteAsync();
            
            //等待结果          
            await regFaceResponseTask;
            _complete = true;
        }

        private List<MarkFaceInfor> ExtractFaceData(Mat mat, Arcsoft_Face_Action arcsoft)
        {
            if (mat != null)
            {
                Image image = mat.ToBitmap();
                ImageInfo imageInfo = new ImageInfo();
                imageInfo = ImageHelper.ReadBMPFromImage(image, imageInfo);
                List<MarkFaceInfor> markFaceInfors = new List<MarkFaceInfor>();

                if (imageInfo.imgData != IntPtr.Zero)
                {
                    var engine = arcsoft.GetEngine(arcsoft.FaceEnginePoor);
                    while (engine == IntPtr.Zero)
                    {
                        Task.Delay(10).Wait();
                        engine = arcsoft.GetEngine(arcsoft.FaceEnginePoor);
                    }
                    markFaceInfors = Arcsoft_Face_Action.DetectMultipleFaceAllInformation(engine, imageInfo, true);
                    arcsoft.PutEngine(arcsoft.FaceEnginePoor, engine);
                }
                else 
                {
                    markFaceInfors = null;
                }
                image.Dispose();
                Marshal.FreeHGlobal(imageInfo.imgData);
                return markFaceInfors;
            }
            else
            {
                return null;
            }
        }

        private async void WriteSendMsgAsync(string msg)
        {
            await Dispatcher.BeginInvoke(() =>
            {
                if (SendMsg.LineCount > 100)
                {
                    SendMsg.Clear();
                }
                SendMsg.AppendText(msg + "\n");
                SendMsg.ScrollToEnd();
            });
        }
            
        private async void WriteReceiveMsgAsync(string msg)
        {
            await Dispatcher.BeginInvoke(() =>
            {
                //大于100行清除记录，可选
                if (ReceiveMsg.LineCount > 100)
                {
                    ReceiveMsg.Clear();
                }
                ReceiveMsg.AppendText(msg + "\n");
                ReceiveMsg.ScrollToEnd();
            });
        }

        private async void CheckConnectTick(object sender, EventArgs e)
        {
            WriteSendMsgAsync(string.Format("开始发送心跳时间{0:HH:MM:ss:fff}\n", DateTime.Now));
            var reply = await _client.CheckAliveAsync(new AliveRequest 
            {
                AliveTest="Alive test"
            });
            WriteReceiveMsgAsync(string.Format("开始接收心跳时间{0:HH:MM:ss:fff}\n {1}", DateTime.Now,reply.AliveMessage));
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (_checkStatus)
            {
                _checkConnectTimer.Stop();
                Connect.Content = "检测";
                _checkStatus = false;
            }
            else
            {
                _checkConnectTimer.Start();
                Connect.Content = "暂停";
                _checkStatus = true;
            }
        }

        private void Choose_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                Play_Name.Content = openFileDialog.FileName;
                _playFile = openFileDialog.FileName;
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_playFile))
            {
                _videCaptureShow = new VideoCapture();
            }
            else
            {
                _videCaptureShow = new VideoCapture(_playFile);
            }

            if (_playStatus)
            {
                _showVideoTimer.Stop();
                Play.Content = "播放";
                _playStatus = false;
            }
            else 
            {
                _showVideoTimer.Start();
                Play.Content = "暂停";
                _playStatus = true;
            }           
        }

        private void Detect_Click(object sender, RoutedEventArgs e)
        {
            if (_detectStatus)
            {
                _detectVideoTimer.Stop();
                Detect.Content = "检测";
                _detectStatus = false;
            }
            else
            {
                _detectVideoTimer.Start();
                Detect.Content = "暂停";
                _detectStatus = true;
            }                              
        }
    }
}
