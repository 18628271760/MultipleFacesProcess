using ArcSoft;
using FaceService;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;


namespace Proprietor.InformationService
{
    public class FaceService:FaceRecongnise.FaceRecongniseBase
    {
        private IConfiguration Configuration { get; }
        private IEnginePoor FaceProcess { get; }

        private float _faceMix = 0.8f;

        public FaceService(IConfiguration configuration, IEnginePoor process)
        {
            Configuration = configuration;
            FaceProcess = process;
            float.TryParse(Configuration.GetSection("AppSettings:FaceMixLevel").Value, out _faceMix);
        }

        public override async Task RecongnizationByFace(IAsyncStreamReader<FaceRequest> requestStream, IServerStreamWriter<FaceReply> responseStream, ServerCallContext context)
        {
            var faceQueue = new Queue<Google.Protobuf.ByteString>();
            IntPtr featurePoint = IntPtr.Zero;
            IntPtr engine = FaceProcess.GetEngine(FaceProcess.FaceEnginePoor);
            FaceReply faceReply = new FaceReply();

            /*
            //开始取数据
            while (await requestStream.MoveNext())
            {
                faceQueue.Enqueue(requestStream.Current.FaceFeature);
            }
            //开始业务处理
            while (faceQueue.TryDequeue(out var feature))
            {
                //识别业务
                byte[] featureByte = feature.ToByteArray();
                featurePoint = Arcsoft_Face_Action.PutFeatureByteIntoFeatureIntPtr(featureByte);
                float maxScore = 0f;

                while (engine == IntPtr.Zero)
                {
                    Task.Delay(10).Wait();
                    engine = FaceProcess.GetEngine(FaceProcess.IDEnginePoor);
                }
                foreach (var f in StaticDataForTestUse.dbFaceInfor)
                {
                    float result = 0;
                    int compareStatus = Arcsoft_Face_3_0.ASFFaceFeatureCompare(engine, featurePoint, f.Key, ref result, 1);
                    if (compareStatus == 0)
                    {
                        if (result >= maxScore)
                        {
                            maxScore = result;
                        }
                        if (result >= _faceMix && result>= maxScore)
                        {
                            faceReply.PersonName = f.Value;
                            faceReply.ConfidenceLevel = result;
                        } 
                    }
                    else
                    {
                        faceReply.PersonName = $"对比异常 error code={compareStatus}";
                        faceReply.ConfidenceLevel = result;
                    }
                }
                if (maxScore < _faceMix)
                {
                    faceReply.PersonName = $"未找到匹配者";
                    faceReply.ConfidenceLevel = maxScore;
                }
                Marshal.FreeHGlobal(featurePoint);
                FaceProcess.PutEngine(FaceProcess.FaceEnginePoor, engine);
                await responseStream.WriteAsync(faceReply) ;
            }
            */
            while (await requestStream.MoveNext())
            {
                //识别业务
                byte[] featureByte = requestStream.Current.FaceFeature.ToByteArray();
                if (featureByte.Length != 1032)
                {
                    continue;
                }
                featurePoint = Arcsoft_Face_Action.PutFeatureByteIntoFeatureIntPtr(featureByte);
                float maxScore = 0f;

                while (engine == IntPtr.Zero)
                {
                    Task.Delay(10).Wait();
                    engine = FaceProcess.GetEngine(FaceProcess.IDEnginePoor);
                }
                foreach (var f in StaticDataForTestUse.dbFaceInfor)
                {
                    float result = 0;
                    int compareStatus = Arcsoft_Face_3_0.ASFFaceFeatureCompare(engine, featurePoint, f.Key, ref result, 1);
                    if (compareStatus == 0)
                    {
                        if (result >= maxScore)
                        {
                            maxScore = result;
                        }
                        if (result >= _faceMix && result >= maxScore)
                        {
                            faceReply.PersonName = f.Value;
                            faceReply.ConfidenceLevel = result;
                        }
                    }
                    else
                    {
                        faceReply.PersonName = $"对比异常 error code={compareStatus}";
                        faceReply.ConfidenceLevel = result;
                    }
                }
                if (maxScore < _faceMix)
                {
                    faceReply.PersonName = $"未找到匹配者";
                    faceReply.ConfidenceLevel = maxScore;
                }
                Marshal.FreeHGlobal(featurePoint);
                await responseStream.WriteAsync(faceReply);
            }
            FaceProcess.PutEngine(FaceProcess.FaceEnginePoor, engine);
        }

        public override Task<AliveReply> CheckAlive(AliveRequest request, ServerCallContext context)
        {
            return Task.FromResult(new AliveReply
            {
                AliveMessage = request.AliveTest+" is alive!"
            }); 
        }
    }
}
