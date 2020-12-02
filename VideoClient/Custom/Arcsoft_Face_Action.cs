using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using VideoClient.Models;
using VideoClient.Utils;

namespace VideoClient.Custom
{
    public class Arcsoft_Face_Action : Arcsoft_Face_3_0
    {
        public string AppID { get; }
        public string AppKey { get; }
        public int FaceEngineNums { get; set; }
        public int IDEngineNums { get; set; }
        public int AIEngineNums { get; set; }
        public ConcurrentQueue<IntPtr> FaceEnginePoor { get; set; }
        public ConcurrentQueue<IntPtr> IDEnginePoor { get; set; }
        public ConcurrentQueue<IntPtr> AIEnginePoor { get; set; }

        public Arcsoft_Face_Action()
        {

        }

        public Arcsoft_Face_Action(string appId, string appKey)
        {
            int retCode = -1;
            try
            {
                retCode = ASFOnlineActivation(appId, appKey);
                if (retCode == 0)
                {
                    LogHelper.LogInfo("SDK激活成功！");
                }
                else if (retCode == 90114)
                {
                    LogHelper.LogInfo("SDK已激活！");
                }
                else
                {
                    throw new Exception("SDK激活失败，错误码：" + retCode);
                }
                AppID = appId;
                AppKey = appKey;
            }
            catch (Exception ex)
            {
                throw new Exception($"Arcsoft_Face_Action 初始化失败，异常：{ex.Message}");
            }
        }

        public IntPtr InitASFEnginePtr(int faceMask, bool isImageMode = true)
        {
            IntPtr pEngines = IntPtr.Zero;
            int retCode = -1;
            try
            {
                if (isImageMode)
                {
                    retCode = ASFInitEngine(ASF_DetectMode.ASF_DETECT_MODE_IMAGE, ArcSoftFace_OrientPriority.ASF_OP_0_HIGHER_EXT, ParmsBestPractice.detectFaceScaleVal_Image, ParmsBestPractice.detectFaceMaxNum, faceMask, ref pEngines);
                }
                else
                {
                    retCode = ASFInitEngine(ASF_DetectMode.ASF_DETECT_MODE_VIDEO, ArcSoftFace_OrientPriority.ASF_OP_0_HIGHER_EXT, ParmsBestPractice.detectFaceScaleVal_Video, ParmsBestPractice.detectFaceMaxNum, faceMask, ref pEngines);
                }
                if (retCode == 0)
                {
                    LogHelper.LogInfo("SDK初始化成功！pEngines=" + pEngines);
                }
                else
                {
                    throw new Exception("SDK初始化失败，错误码：" + retCode);
                }
                return pEngines;
            }
            catch (Exception ex)
            {
                throw new Exception("ASFFunctions->ASFFunctions, generate exception as: " + ex);
            }
        }

        public static ASF_MultiFaceInfo DetectMultipleFace(IntPtr pEngine, ImageInfo imageInfo)
        {
            ASF_MultiFaceInfo multiFaceInfo = new ASF_MultiFaceInfo();
            IntPtr pMultiFaceInfo = Marshal.AllocHGlobal(Marshal.SizeOf<ASF_MultiFaceInfo>());
            try
            {
                int retCode = ASFDetectFaces(pEngine, imageInfo.width, imageInfo.height, imageInfo.format, imageInfo.imgData, pMultiFaceInfo, ASF_DetectModel.ASF_DETECT_MODEL_RGB);
                multiFaceInfo = Marshal.PtrToStructure<ASF_MultiFaceInfo>(pMultiFaceInfo);
                return multiFaceInfo;
            }
            catch
            {
                return multiFaceInfo;
            }
            finally
            {
                Marshal.FreeHGlobal(pMultiFaceInfo);
            }
        }

        public static List<MarkFaceInfor> DetectMultipleFaceAllInformation(IntPtr pEngine, ImageInfo imageInfo, bool extractFaceData = false)
        {
            List<MarkFaceInfor> infors = new List<MarkFaceInfor>();
            ASF_MultiFaceInfo multiFaceInfo = new ASF_MultiFaceInfo();
            IntPtr pMultiFaceInfo = Marshal.AllocHGlobal(Marshal.SizeOf<ASF_MultiFaceInfo>());
            try
            {
                int retCode = ASFDetectFaces(pEngine, imageInfo.width, imageInfo.height, imageInfo.format, imageInfo.imgData, pMultiFaceInfo, ASF_DetectModel.ASF_DETECT_MODEL_RGB);
                multiFaceInfo = Marshal.PtrToStructure<ASF_MultiFaceInfo>(pMultiFaceInfo);
                for (int faceIndex = 0; faceIndex < multiFaceInfo.faceNum; faceIndex++)
                {
                    ASF_SingleFaceInfo singleFaceInfo = new ASF_SingleFaceInfo();
                    singleFaceInfo.faceRect = Marshal.PtrToStructure<MRECT>(multiFaceInfo.faceRects + Marshal.SizeOf<MRECT>() * faceIndex);
                    singleFaceInfo.faceOrient = Marshal.PtrToStructure<int>(multiFaceInfo.faceOrients + Marshal.SizeOf<int>() * faceIndex);
                    MarkFaceInfor markFaceInfor = new MarkFaceInfor(singleFaceInfo.faceRect.left, singleFaceInfo.faceRect.top, singleFaceInfo.faceRect.right - singleFaceInfo.faceRect.left, singleFaceInfo.faceRect.bottom - singleFaceInfo.faceRect.top);
                    markFaceInfor.faceID = Marshal.PtrToStructure<int>(multiFaceInfo.faceID + Marshal.SizeOf<int>() * faceIndex);
                    if (extractFaceData)
                    {
                        markFaceInfor.faceFeatureData = ExtractSingleFaceFeature(pEngine, imageInfo, singleFaceInfo.faceRect, singleFaceInfo.faceOrient);
                    }
                    infors.Add(markFaceInfor);
                }
                return infors;
            }
            catch (Exception ex)
            {
                return null;
                //throw new Exception($"Arcsoft_Face_Action-->DetectMultipleFaceAllInformation 异常，异常信息：{ex.Message}");
            }
            finally
            {
                Marshal.FreeHGlobal(pMultiFaceInfo);
            }
        }

        public static bool ExtractFeaturesFromMemoryStream(Stream ms, IntPtr engine, out List<byte[]> facesFeature, out string errorString)
        {
            facesFeature = new List<byte[]>();
            errorString = null;
            try
            {
                ImageInfo imageInfo = new ImageInfo();
                ASF_MultiFaceInfo facesInfo = new ASF_MultiFaceInfo();
                imageInfo = ImageHelper.ReadBMPFormStream(ms);
                facesInfo = DetectMultipleFace(engine, imageInfo);
                if (facesInfo.faceNum == 0)
                {
                    errorString = "检测到人脸数量为0，请免冠正对镜头重新识别！";
                    return false;
                }
                if (facesInfo.faceNum > 1)
                {
                    errorString = "检测到多张人脸，请多余人员退出识别区，再重新识别！";
                    return false;
                }
                facesFeature = ExtractAllFeatures(engine, imageInfo, facesInfo);
                return true;
            }
            catch
            {
                errorString = "算法错误，请检查输入后重试！";
                return false;
            }
            finally
            {
                GC.Collect();
            }
        }

        private static byte[] ExtractSingleFaceFeature(IntPtr pEngine, ImageInfo imageInfo, MRECT rect, int faceOrient)
        {
            var singleFaceInfo = new ASF_SingleFaceInfo();
            singleFaceInfo.faceRect = rect;
            singleFaceInfo.faceOrient = faceOrient;
            IntPtr pSingleFaceInfo = Marshal.AllocHGlobal(Marshal.SizeOf<ASF_SingleFaceInfo>());
            Marshal.StructureToPtr(singleFaceInfo, pSingleFaceInfo, false);
            IntPtr pFaceFeature = Marshal.AllocHGlobal(Marshal.SizeOf<ASF_FaceFeature>());
            try
            {
                int retCode = ASFFaceFeatureExtract(pEngine, imageInfo.width, imageInfo.height, imageInfo.format, imageInfo.imgData, pSingleFaceInfo, pFaceFeature);
                if (retCode == 0)
                {
                    ASF_FaceFeature faceFeature = Marshal.PtrToStructure<ASF_FaceFeature>(pFaceFeature);
                    byte[] feature = new byte[faceFeature.featureSize];
                    Marshal.Copy(faceFeature.feature, feature, 0, faceFeature.featureSize);
                    return feature;
                }
                if (retCode == 81925)
                {
                    throw new Exception("人脸特征检测结果置信度低!");
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Arcsoft_Face_Action-->ExtractSingleFaceFeature exception: {ex.Message}");
            }
            finally
            {
                Marshal.FreeHGlobal(pSingleFaceInfo);
                Marshal.FreeHGlobal(pFaceFeature);
            }
        }

        public static List<byte[]> ExtractAllFeatures(IntPtr pEngine, ImageInfo imageInfo, ASF_MultiFaceInfo multiFaceInfo)
        {
            try
            {
                ASF_SingleFaceInfo singleFaceInfo = new ASF_SingleFaceInfo();
                List<byte[]> results = new List<byte[]>();
                for (int index = 0; index < multiFaceInfo.faceNum; index++)
                {
                    singleFaceInfo.faceRect = Marshal.PtrToStructure<MRECT>(multiFaceInfo.faceRects + Marshal.SizeOf<MRECT>() * index);
                    singleFaceInfo.faceOrient = Marshal.PtrToStructure<int>(multiFaceInfo.faceOrients + Marshal.SizeOf<int>() * index);
                    byte[] singleFaceFeature = ExtractSingleFaceFeature(pEngine, imageInfo, singleFaceInfo.faceRect, singleFaceInfo.faceOrient);
                    if (singleFaceFeature != null)
                    {
                        results.Add(singleFaceFeature);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception("Arcsoft_Face_Action-->ExtractAllFeatures exception " + ex);
            }
            finally
            {
                Marshal.FreeHGlobal(imageInfo.imgData);
            }
        }

        public static IntPtr GetBMP_Ptr(Bitmap image, out int width, out int height, out int pitch)
        {
            IntPtr imageDataPtr = IntPtr.Zero;
            try
            {
                width = -1;
                height = -1;
                pitch = -1;
                byte[] imageData = ReadBMP(image, ref width, ref height, ref pitch);
                imageDataPtr = Marshal.AllocHGlobal(imageData.Length);
                Marshal.Copy(imageData, 0, imageDataPtr, imageData.Length);
                return imageDataPtr;
            }
            catch (Exception ex)
            {
                Marshal.FreeHGlobal(imageDataPtr);
                throw new Exception($"Arcsoft_Face_Action-->GetBMP_Ptr exception as:{ex.Message}");
            }
        }

        public static byte[] ReadBMP(Bitmap image, ref int width, ref int height, ref int pitch)
        {
            //将Bitmap锁定到系统内存中,获得BitmapData
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            //位图中第一个像素数据的地址。它也可以看成是位图中的第一个扫描行
            IntPtr ptr = data.Scan0;
            //定义数组长度
            int soureBitArrayLength = data.Height * Math.Abs(data.Stride);
            byte[] sourceBitArray = new byte[soureBitArrayLength];
            //将bitmap中的内容拷贝到ptr_bgr数组中
            Marshal.Copy(ptr, sourceBitArray, 0, soureBitArrayLength); width = data.Width;
            height = data.Height;
            pitch = Math.Abs(data.Stride);
            int line = width * 3;
            int bgr_len = line * height;
            byte[] destBitArray = new byte[bgr_len];
            for (int i = 0; i < height; ++i)
            {
                Array.Copy(sourceBitArray, i * pitch, destBitArray, i * line, line);
            }
            pitch = line;
            image.UnlockBits(data);
            return destBitArray;
        }

        public static ASVLOFFSCREEN ChangeMat2ASVLOFFSCREEN(Bitmap image)
        {
            int width = -1;
            int height = -1;
            int pitch = -1;
            IntPtr imagePtr = GetBMP_Ptr(image, out width, out height, out pitch);
            ASVLOFFSCREEN offInput = new ASVLOFFSCREEN();
            offInput.u32PixelArrayFormat = 513;
            offInput.ppu8Plane = new IntPtr[4];
            offInput.ppu8Plane[0] = imagePtr;
            offInput.i32Width = width;
            offInput.i32Height = height;
            offInput.pi32Pitch = new int[4];
            offInput.pi32Pitch[0] = pitch;
            return offInput;
        }

        public static IntPtr PutFeatureByteIntoFeatureIntPtr(byte[] data)
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

        private int InitEnginePool()
        {
            try
            {
                for (int index = 0; index < FaceEngineNums; index++)
                {
                    IntPtr enginePtr = IntPtr.Zero;
                    Arcsoft_Face_Action faceAction = new Arcsoft_Face_Action(AppID, AppKey);
                    enginePtr = faceAction.InitASFEnginePtr(ParmsBestPractice.faceBaseMask, false);
                    PutEngine(FaceEnginePoor, enginePtr);
                    Console.WriteLine($"FaceEnginePoor add {enginePtr}");
                }
                for (int index = 0; index < IDEngineNums; index++)
                {
                    IntPtr enginePtr = IntPtr.Zero;
                    Arcsoft_Face_Action faceAction = new Arcsoft_Face_Action(AppID, AppKey);
                    enginePtr = faceAction.InitASFEnginePtr(ParmsBestPractice.faceBaseMask);
                    PutEngine(IDEnginePoor, enginePtr);
                    Console.WriteLine($"IDEnginePoor add {enginePtr}");
                }
                for (int index = 0; index < AIEngineNums; index++)
                {
                    IntPtr enginePtr = IntPtr.Zero;
                    int aiMask = FaceEngineMask.ASF_AGE | FaceEngineMask.ASF_GENDER | FaceEngineMask.ASF_FACE3DANGLE | FaceEngineMask.ASF_LIVENESS;
                    Arcsoft_Face_Action faceAction = new Arcsoft_Face_Action(AppID, AppKey);
                    enginePtr = faceAction.InitASFEnginePtr(ParmsBestPractice.faceBaseMask | aiMask);
                    PutEngine(AIEnginePoor, enginePtr);
                    Console.WriteLine($"AIEnginePoor add {enginePtr}");
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"InitEnginePool--> exception {ex}");
            }
        }

        public IntPtr GetEngine(ConcurrentQueue<IntPtr> queue)
        {
            IntPtr item = IntPtr.Zero;
            if (queue.TryDequeue(out item))
            {
                return item;
            }
            else
            {
                return IntPtr.Zero;
            }
        }

        public void PutEngine(ConcurrentQueue<IntPtr> queue, IntPtr item)
        {
            if (item != IntPtr.Zero)
            {
                queue.Enqueue(item);
            }
        }

        public void Arcsoft_EnginePool(int faceEngineNums = 1, int idEngineNums = 0, int aiEngineNums = 0)
        {
            FaceEnginePoor = new ConcurrentQueue<IntPtr>();
            IDEnginePoor = new ConcurrentQueue<IntPtr>();
            AIEnginePoor = new ConcurrentQueue<IntPtr>();
            try
            {
                FaceEngineNums = faceEngineNums;
                IDEngineNums = idEngineNums;
                AIEngineNums = aiEngineNums;
                int status = InitEnginePool();
                if (status != 0)
                {
                    throw new Exception("引擎池初始化失败！");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"ArcSoft_EnginePool-->ArcSoft_EnginePool exception as: {ex}");
            }
        }
    }

    public struct ParmsBestPractice
    {
        //VIDEO模式取值范围[2,32]，推荐值为16
        public const int detectFaceScaleVal_Video = 16;

        //MAGE模式取值范围[2,32]，推荐值为30
        public const int detectFaceScaleVal_Image = 32;

        //最大需要检测的人脸个数，取值范围[1,50]
        public const int detectFaceMaxNum = 50;

        //人脸识别最基本功能。
        public const int faceBaseMask = FaceEngineMask.ASF_FACE_DETECT | FaceEngineMask.ASF_FACERECOGNITION;

        //RGB活体检测
        public const int faceLivingMask = FaceEngineMask.ASF_FACE_DETECT | FaceEngineMask.ASF_FACERECOGNITION | FaceEngineMask.ASF_LIVENESS;

        //process可传入属性组合只有ASF_AGE 、ASF_LIVENESS 、ASF_AGE 和 ASF_LIVENESS
        public const int processSupportMask = FaceEngineMask.ASF_AGE | FaceEngineMask.ASF_GENDER | FaceEngineMask.ASF_FACE3DANGLE | FaceEngineMask.ASF_LIVENESS;
    }
}
