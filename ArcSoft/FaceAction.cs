using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ArcSoft
{
    public class FaceAction : Arcsoft_Face_3_0
    {
        public FaceAction(string appId, string appKey, ref IntPtr pEngines, bool isImageMode = false, int faceMask = (FaceEngineMask.ASF_FACE_DETECT | FaceEngineMask.ASF_FACERECOGNITION))
        {
            int retCode = -1;
            try
            {
                retCode = ASFActivation(appId, appKey);
                if ((retCode == 0) || (retCode == 90114))
                {
                }
                else
                {
                    throw new Exception("SDK激活失败，错误码：" + retCode);
                }
                if (isImageMode)
                {
                    retCode = ASFInitEngine(ASF_DetectMode.ASF_DETECT_MODE_IMAGE, ArcSoftFace_OrientPriority.ASF_OP_0_ONLY, ParmsBestPractice.detectFaceScaleVal_Image, ParmsBestPractice.detectFaceMaxNum, faceMask, ref pEngines);
                }
                else
                {
                    retCode = ASFInitEngine(ASF_DetectMode.ASF_DETECT_MODE_VIDEO, ArcSoftFace_OrientPriority.ASF_OP_0_HIGHER_EXT, ParmsBestPractice.detectFaceScaleVal_Video, ParmsBestPractice.detectFaceMaxNum, faceMask, ref pEngines);
                }
                if ((retCode == 0))
                {
                }
                else
                {
                    throw new Exception("SDK初始化失败，错误码：" + retCode);
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public IntPtr InitASFEnginePtr(string appId, string appKey, bool isImageMode = false, int faceMask = (FaceEngineMask.ASF_FACE_DETECT | FaceEngineMask.ASF_FACERECOGNITION))
        {
            IntPtr pEngines = IntPtr.Zero;
            int retCode = -1;
            try
            {
                retCode = ASFOnlineActivation(appId, appKey);
                if ((retCode == 0) || (retCode == 90114))
                {
                }
                else
                {
                    throw new Exception("SDK激活失败，错误码：" + retCode);
                }
                if (isImageMode)
                {
                    retCode = ASFInitEngine(ASF_DetectMode.ASF_DETECT_MODE_IMAGE, ArcSoftFace_OrientPriority.ASF_OP_0_ONLY, ParmsBestPractice.detectFaceScaleVal_Image, ParmsBestPractice.detectFaceMaxNum, faceMask, ref pEngines);
                }
                else
                {
                    retCode = ASFInitEngine(ASF_DetectMode.ASF_DETECT_MODE_VIDEO, ArcSoftFace_OrientPriority.ASF_OP_0_HIGHER_EXT, ParmsBestPractice.detectFaceScaleVal_Video, ParmsBestPractice.detectFaceMaxNum, faceMask, ref pEngines);
                }
                if ((retCode == 0))
                {
                }
                else
                {
                    throw new Exception("SDK初始化失败，错误码：" + retCode);
                }
                return pEngines;
            }
            catch (Exception ex)
            {
                return pEngines;
            }
        }

        public ASF_MultiFaceInfo DetectMultipleFace(IntPtr pEngine, ImageInfo imageInfo)
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

        public List<MarkFaceInfor> DetectMultipleFaceAllInformation(IntPtr pEngine, ImageInfo imageInfo, bool extractFaceData = false)
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
            catch
            {
                return null;
            }
            finally
            {
                Marshal.FreeHGlobal(pMultiFaceInfo);
            }
        }

        private byte[] ExtractSingleFaceFeature(IntPtr pEngine, ImageInfo imageInfo, MRECT rect, int faceOrient)
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
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                Marshal.FreeHGlobal(pSingleFaceInfo);
                Marshal.FreeHGlobal(pFaceFeature);
            }
        }

        public List<byte[]> ExtractAllFeatures(IntPtr pEngine, ImageInfo imageInfo, ASF_MultiFaceInfo multiFaceInfo)
        {
            try
            {
                List<byte[]> results = new List<byte[]>();
                ASF_SingleFaceInfo singleFaceInfo = new ASF_SingleFaceInfo();
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
                throw new Exception("Arcsoft2-->ExtractAllFeatures exception " + ex);
            }
            finally
            {
                Marshal.FreeHGlobal(imageInfo.imgData);
            }
        }

        public IntPtr GetBMP_Ptr(Bitmap image, out int width, out int height, out int pitch)
        {
            width = -1;
            height = -1;
            pitch = -1;
            byte[] imageData = ReadBMP(image, ref width, ref height, ref pitch);
            IntPtr imageDataPtr = Marshal.AllocHGlobal(imageData.Length);
            Marshal.Copy(imageData, 0, imageDataPtr, imageData.Length);
            return imageDataPtr;
        }

        public byte[] ReadBMP(Bitmap image, ref int width, ref int height, ref int pitch)
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

        public ASVLOFFSCREEN ChangeMat2ASVLOFFSCREEN(Bitmap image)
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
    }
}
