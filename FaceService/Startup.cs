using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using ArcSoft;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;

namespace Proprietor.InformationService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private string appID;
        private string faceKey;
        private int faceEngineNums;
        private Arcsoft_Face_Action arcsoft_Face_Action;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //取配置参数
            appID = Configuration.GetSection("AppSettings:AppId").Value;
            faceKey = Configuration.GetSection("AppSettings:FaceKey").Value;
            faceEngineNums = 0;
            int.TryParse(Configuration.GetSection("AppSettings:FaceEngineNum").Value, out faceEngineNums);
            arcsoft_Face_Action = new Arcsoft_Face_Action(appID, faceKey);
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //配置grpc
            services.AddGrpc();

            //添加虹软“引擎池”服务
            Arcsoft_Face_Action enginePool = new Arcsoft_Face_Action(appID, faceKey);
            enginePool.Arcsoft_EnginePool(faceEngineNums, 0, 0);
            services.AddArcSoftFaceService(enginePool);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                //映射到Grpc
                endpoints.MapGrpcService<FaceService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });

            //测试模拟加载人脸数据
            LoadFaceInformationFromFolderJPG(@"D:\Test");
        }

        //从文件夹的jpg（人脸照片）读数据，测试使用，替换成数据库读取。
        private void LoadFaceInformationFromFolderJPG(string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath, "*.jpg");
            IntPtr pEngine_Load = IntPtr.Zero;
            FaceAction faceAction = new FaceAction(appID, faceKey, ref pEngine_Load, true);

            for (int index = 0; index < files.Length; index++)
            {
                ImageInfo imageInfor = ArcSoft.Utilities.ImageHelper.ReadBMPFormJPG(files[index]);
                ASF_MultiFaceInfo individualFaceInfor = new ASF_MultiFaceInfo();
                individualFaceInfor = faceAction.DetectMultipleFace(pEngine_Load, imageInfor);
                List<byte[]> data = faceAction.ExtractAllFeatures(pEngine_Load, imageInfor, individualFaceInfor);
                if (data.Count == 1)
                {
                    StaticDataForTestUse.dbFaceInfor.TryAdd(PutFeatureByteIntoFeatureIntPtr(data[0]), Path.GetFileNameWithoutExtension(files[index]));
                }
            }
            FaceAction.ASFUninitEngine(pEngine_Load);
        }

        //辅助函数
        private IntPtr PutFeatureByteIntoFeatureIntPtr(byte[] data)
        {
            try
            {
                if (data.Length > 0)
                {
                    ASF_FaceFeature localFeature = new ASF_FaceFeature();
                    localFeature.featureSize = data.Length;
                    localFeature.feature = Marshal.AllocHGlobal(localFeature.featureSize);
                    Marshal.Copy(data, 0, localFeature.feature, data.Length);
                    IntPtr intPtrFeature = Marshal.AllocHGlobal(Marshal.SizeOf<ASF_FaceFeature>());
                    Marshal.StructureToPtr(localFeature, intPtrFeature, false);
                    return intPtrFeature;
                }
                else
                {
                    return IntPtr.Zero;
                }
            }
            catch
            {
                return IntPtr.Zero;
            }
        }
    }
}
