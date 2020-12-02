using System;
using System.Runtime.InteropServices;

namespace VideoClient.Custom
{
    public class Arcsoft_Face_3_0
    {
        public const string Dll_PATH = "libarcsoft_face_engine.dll";

        /// <summary>
        /// 获取激活文件信息。
        /// </summary>
        /// <param name="activeFileInfo">激活文件信息</param>
        /// <returns></returns>
        [DllImport(Dll_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFGetActiveFileInfo(IntPtr activeFileInfo);

        /// <summary>
        /// 用于在线激活SDK。
        /// </summary>
        /// <param name="appId">官网获取的APPID</param>
        /// <param name="sdkKey">官网获取的SDKKEY</param>
        /// <returns></returns>
        [DllImport(Dll_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFOnlineActivation(string appId, string sdkKey);

        /// <summary>
        /// 激活人脸识别SDK引擎函数,ASFActivation 接口与ASFOnlineActivation 功能一致，用于兼容老用户。
        /// </summary>
        /// <param name="appId">SDK对应的AppID</param>
        /// <param name="sdkKey">SDK对应的SDKKey</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFActivation(string appId, string sdkKey);

        /// <summary>
        /// 初始化引擎
        /// </summary>
        /// <param name="detectMode">AF_DETECT_MODE_VIDEO 视频模式 | AF_DETECT_MODE_IMAGE 图片模式</param>
        /// <param name="detectFaceOrientPriority">检测脸部的角度优先值，推荐：ASF_OrientPriority.ASF_OP_0_HIGHER_EXT</param>
        /// <param name="detectFaceScaleVal">用于数值化表示的最小人脸尺寸</param>
        /// <param name="detectFaceMaxNum">最大需要检测的人脸个数</param>
        /// <param name="combinedMask">用户选择需要检测的功能组合，可单个或多个</param>
        /// <param name="hEngine">初始化返回的引擎handle</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFInitEngine(uint detectMode, int detectFaceOrientPriority, int detectFaceScaleVal, int detectFaceMaxNum, int combinedMask, ref IntPtr hEngine);

        /// <summary>
        /// 人脸检测
        /// </summary>
        /// <param name="hEngine">引擎handle</param>
        /// <param name="width">图像宽度</param>
        /// <param name="height">图像高度</param>
        /// <param name="format">图像颜色空间</param>
        /// <param name="imgData">图像数据</param>
        /// <param name="detectedFaces">人脸检测结果</param>
        /// <param name="detectModel">预留字段，当前版本使用默认参数即可</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFDetectFaces(IntPtr hEngine, int width, int height, int format, IntPtr imgData, IntPtr detectedFaces, int detectModel);

        /// <summary>
        /// 检测人脸信息。
        /// </summary>
        /// <param name="hEngine">引擎句柄</param>
        /// <param name="ImgData">图像数据</param>
        /// <param name="detectedFaces">检测到的人脸信息</param>
        /// <param name="detectModel">预留字段，当前版本使用默认参数即可</param>
        /// <returns>人脸信息</returns>
        [DllImport(Dll_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFDetectFacesEx(IntPtr hEngine, IntPtr ImgData, out IntPtr detectedFaces, int detectModel);

        /// <summary>
        /// 单人脸特征提取
        /// </summary>
        /// <param name="hEngine">引擎handle</param>
        /// <param name="width">图像宽度，为4的倍数</param>
        /// <param name="height">图像高度，YUYV/I420/NV21/NV12格式为2的倍数；BGR24/GRAY/DEPTH_U16格式无限制</param>
        /// <param name="format">图像颜色空间</param>
        /// <param name="imgData">图像数据</param>
        /// <param name="faceInfo">单人脸信息（人脸框、人脸角度）</param>
        /// <param name="faceFeature">提取到的人脸特征信息</param>
        /// <returns>人脸特征信息</returns>
        [DllImport(Dll_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFFaceFeatureExtract(IntPtr hEngine, int width, int height, int format, IntPtr imgData, IntPtr faceInfo, IntPtr faceFeature);

        /// <summary>
        /// 单人特征提取。
        /// </summary>
        /// <param name="hEngine">引擎句柄</param>
        /// <param name="imgData">图像数据</param>
        /// <param name="faceInfo">单人脸信息（人脸框、人脸角度）</param>
        /// <param name="feature">提取到的人脸特征信息</param>
        /// <returns></returns>
        [DllImport(Dll_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFFaceFeatureExtractEx(IntPtr hEngine, IntPtr imgData, IntPtr faceInfo, IntPtr feature);

        /// <summary>
        /// 人脸特征比对，输出比对相似度。
        /// </summary>
        /// <param name="hEngine">引擎句柄</param>
        /// <param name="feature1">人脸特征</param>
        /// <param name="feature2">人脸特征</param>
        /// <param name="confidenceLevel">比对相似度</param>
        /// <param name="compareModel">选择人脸特征比对模型，默认为ASF_LIFE_PHOTO。
        /// 1. ASF_LIFE_PHOTO：用于生活照之间的特征比对，推荐阈值0.80；
        /// 2. ASF_ID_PHOTO：用于证件照或证件照和生活照之间的特征比对，推荐阈值0.82；</param>
        /// <returns>比对相似度</returns>
        [DllImport(Dll_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFFaceFeatureCompare(IntPtr hEngine, IntPtr feature1, IntPtr feature2, ref float confidenceLevel, int compareModel);

        /// <summary>
        /// 设置RGB/IR活体阈值，若不设置内部默认RGB：0.5 IR：0.7。
        /// </summary>
        /// <param name="hEngine">引擎句柄</param>
        /// <param name="threshold">活体阈值，推荐RGB:0.5 IR:0.7</param>
        /// <returns>设置状态</returns>
        [DllImport(Dll_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFSetLivenessParam(IntPtr hEngine, IntPtr threshold);

        /// <summary>
        /// 人脸属性检测
        /// </summary>
        /// <param name="hEngine">引擎句柄</param>
        /// <param name="width">图片宽度，为4的倍数</param>
        /// <param name="height">图片高度，YUYV/I420/NV21/NV12格式为2的倍数；BGR24格式无限制；</param>
        /// <param name="format">支持YUYV/I420/NV21/NV12/BGR24</param>
        /// <param name="imgData">图像数据</param>
        /// <param name="detectedFaces">多人脸信息</param>
        /// <param name="combinedMask">1.检测的属性（ASF_AGE、ASF_GENDER、 ASF_FACE3DANGLE、ASF_LIVENESS），支持多选
        /// 2.检测的属性须在引擎初始化接口的combinedMask参数中启用</param>
        /// <returns>检测状态</returns>
        [DllImport(Dll_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFProcess(IntPtr hEngine, int width, int height, int format, IntPtr imgData, IntPtr detectedFaces, int combinedMask);

        /// <summary>
        /// 人脸信息检测（年龄/性别/人脸3D角度），最多支持4张人脸信息检测，超过部分返回未知（活体仅支持单张人脸检测，超出返回未知）,接口不支持IR图像检测。
        /// </summary>
        /// <param name="hEngine">引擎句柄</param>
        /// <param name="imgData">图像数据</param>
        /// <param name="detectedFaces">多人脸信息</param>
        /// <param name="combinedMask">1.检测的属性（ASF_AGE、ASF_GENDER、 ASF_FACE3DANGLE、ASF_LIVENESS），支持多选
        /// 2.检测的属性须在引擎初始化接口的combinedMask参数中启用</param>
        /// <returns>检测状态</returns>
        [DllImport(Dll_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFProcessEx(IntPtr hEngine, IntPtr imgData, IntPtr detectedFaces, int combinedMask);

        /// <summary>
        /// 获取年龄信息
        /// </summary>
        /// <param name="hEngine">引擎handle</param>
        /// <param name="ageInfo">检测到的年龄信息</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFGetAge(IntPtr hEngine, IntPtr ageInfo);

        /// <summary>
        /// 获取性别信息
        /// </summary>
        /// <param name="hEngine">引擎handle</param>
        /// <param name="genderInfo">检测到的性别信息</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFGetGender(IntPtr hEngine, IntPtr genderInfo);

        /// <summary>
        /// 获取3D角度信息
        /// </summary>
        /// <param name="hEngine">引擎handle</param>
        /// <param name="p3DAngleInfo">检测到脸部3D角度信息</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFGetFace3DAngle(IntPtr hEngine, IntPtr p3DAngleInfo);

        /// <summary>
        /// 获取RGB活体信息。
        /// </summary>
        /// <param name="hEngine">引擎句柄</param>
        /// <param name="livenessInfo">检测到的活体信息</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFGetLivenessScore(IntPtr hEngine, IntPtr livenessInfo);

        /// <summary>
        /// 该接口仅支持单人脸IR 活体检测，超出返回未知。
        /// </summary>
        /// <param name="hEngine">引擎句柄</param>
        /// <param name="width">图片宽度，为4的倍数</param>
        /// <param name="height">图片高度</param>
        /// <param name="format">图像颜色格式</param>
        /// <param name="imgData">图像数据</param>
        /// <param name="detectedFaces">多人脸信息</param>
        /// <param name="combinedMask">目前仅支持ASF_IR_LIVENESS</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFProcess_IR(IntPtr hEngine, int width, int height, int format, IntPtr imgData, IntPtr detectedFaces, int combinedMask);

        /// <summary>
        /// 该接口仅支持单人脸IR 活体检测，超出返回未知。
        /// </summary>
        /// <param name="hEngine">引擎句柄</param>
        /// <param name="imgData">图像数据</param>
        /// <param name="detectedFaces">多人脸信息</param>
        /// <param name="combinedMask">目前仅支持ASF_IR_LIVENESS</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFProcessEx_IR(IntPtr hEngine, IntPtr imgData, IntPtr detectedFaces, int combinedMask);

        /// <summary>
        /// 获取IR活体信息。
        /// </summary>
        /// <param name="hEngine">引擎句柄</param>
        /// <param name="livenessInfo">检测到的IR活体信息</param>
        /// <returns></returns>
        [DllImport(Dll_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFGetLivenessScore_IR(IntPtr hEngine, IntPtr livenessInfo);

        /// <summary>
        /// 获取SDK版本信息。
        /// </summary>
        /// <returns>成功返回版本信息，失败返回Null。</returns>
        [DllImport(Dll_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern ASF_VERSION ASFGetVersion();

        /// <summary>
        /// 销毁SDK引擎。
        /// </summary>
        /// <param name="pEngine">引擎handle</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFUninitEngine(IntPtr pEngine);
    }

    /////////////////////////////////参数枚举/////////////////////////////////
    /// <summary>
    /// 检测模式
    /// </summary>
    public struct ASF_DetectMode
    {
        /// <summary>
        /// Video模式，一般用于多帧连续检测
        /// </summary>
        public const uint ASF_DETECT_MODE_VIDEO = 0x00000000;

        /// <summary>
        /// Image模式，一般用于静态图的单次检测
        /// </summary>
        public const uint ASF_DETECT_MODE_IMAGE = 0xFFFFFFFF;
    }

    /// <summary>
    /// 人脸检测方向
    /// </summary>
    public struct ArcSoftFace_OrientPriority
    {
        /// <summary>
        /// 常规预览下正方向
        /// </summary>
        public const int ASF_OP_0_ONLY = 0x1;

        /// <summary>
        /// 基于0°逆时针旋转90°的方向
        /// </summary>
        public const int ASF_OP_90_ONLY = 0x2;

        /// <summary>
        /// 基于0°逆时针旋转270°的方向
        /// </summary>
        public const int ASF_OP_270_ONLY = 0x3;

        /// <summary>
        /// 基于0°旋转180°的方向（逆时针、顺时针效果一样）
        /// </summary>
        public const int ASF_OP_180_ONLY = 0x4;

        /// <summary>
        /// 全角度
        /// </summary>
        public const int ASF_OP_0_HIGHER_EXT = 0x5;
    }

    /// <summary>
    /// 检测到的人脸角度
    /// </summary>
    public struct ArcSoftFace_OrientCode
    {
        public const int ASF_OC_0 = 0x1; // 0度
        public const int ASF_OC_90 = 0x2; // 90度
        public const int ASF_OC_270 = 0x3; // 270度
        public const int ASF_OC_180 = 0x4; // 180度
        public const int ASF_OC_30 = 0x5; // 30度
        public const int ASF_OC_60 = 0x6; // 60度
        public const int ASF_OC_120 = 0x7; // 120度
        public const int ASF_OC_150 = 0x8; // 150度
        public const int ASF_OC_210 = 0x9; // 210度
        public const int ASF_OC_240 = 0xa; // 240度
        public const int ASF_OC_300 = 0xb; // 300度
        public const int ASF_OC_330 = 0xc; // 330度
    }

    /// <summary>
    /// 检测模型
    /// </summary>
    public struct ASF_DetectModel
    {
        public const int ASF_DETECT_MODEL_RGB = 0x1; //RGB图像检测模型
        //预留扩展其他检测模型
    }

    /// <summary>
    /// 人脸比对可选的模型
    /// </summary>
    public struct ASF_CompareModel
    {
        public const int ASF_LIFE_PHOTO = 0x1;  //用于生活照之间的特征比对，推荐阈值0.80
        public const int ASF_ID_PHOTO = 0x2;    //用于证件照或生活照与证件照之间的特征比对，推荐阈值0.82
    }

    /// <summary>
    /// 支持的颜色空间颜色格式
    /// </summary>
    public struct ASF_ImagePixelFormat
    {
        //8-bit Y 通道，8-bit 2x2 采样 V 与 U 分量交织通道
        public const int ASVL_PAF_NV21 = 2050;
        //8-bit Y 通道，8-bit 2x2 采样 U 与 V 分量交织通道
        public const int ASVL_PAF_NV12 = 2049;
        //RGB 分量交织，按 B, G, R, B 字节序排布
        public const int ASVL_PAF_RGB24_B8G8R8 = 513;
        //8-bit Y 通道， 8-bit 2x2 采样 U 通道， 8-bit 2x2 采样 V 通道
        public const int ASVL_PAF_I420 = 1537;
        //YUV 分量交织， V 与 U 分量 2x1 采样，按 Y0, U0, Y1, V0 字节序排布
        public const int ASVL_PAF_YUYV = 1289;
        //8-bit IR图像
        public const int ASVL_PAF_GRAY = 1793;
        //16-bit IR图像,ASVL_PAF_DEPTH_U16 只是预留。
        public const int ASVL_PAF_DEPTH_U16 = 3074;
    }

    /// <summary>
    /// 算法功能常量值
    /// </summary>
    public struct FaceEngineMask
    {
        //人脸检测
        public const int ASF_FACE_DETECT = 0x00000001;
        //人脸特征
        public const int ASF_FACERECOGNITION = 0x00000004;
        //年龄
        public const int ASF_AGE = 0x00000008;
        //性别
        public const int ASF_GENDER = 0x00000010;
        //3D角度
        public const int ASF_FACE3DANGLE = 0x00000020;
        //RGB活体
        public const int ASF_LIVENESS = 0x00000080;
        //IR活体
        public const int ASF_IR_LIVENESS = 0x00000400;
    }

    /////////////////////////////////数据结构/////////////////////////////////
    /// <summary>
    /// SDK版本信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ASF_VERSION
    {
        //版本号
        public IntPtr Version;
        //构建日期
        public IntPtr BuildDate;
        //版权说明
        public IntPtr CopyRight;
    }

    /// <summary>
    /// 激活文件信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ASF_ActiveFileInfo
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public IntPtr startTime;

        /// <summary>
        /// 截止时间
        /// </summary>
        public IntPtr endTime;

        /// <summary>
        /// 平台
        /// </summary>
        public IntPtr platform;

        /// <summary>
        /// sdk类型
        /// </summary>
        public IntPtr sdkType;

        /// <summary>
        /// APPID
        /// </summary>
        public IntPtr appId;

        /// <summary>
        /// SDKKEY
        /// </summary>
        public IntPtr sdkKey;

        /// <summary>
        /// SDK版本号
        /// </summary>
        public IntPtr sdkVersion;

        /// <summary>
        /// 激活文件版本号
        /// </summary>
        public IntPtr fileVersion;
    }

    /// <summary>
    /// 人脸框信息。
    /// </summary>
    public struct MRECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    /// <summary>
    /// 单人脸信息。
    /// </summary>
    public struct ASF_SingleFaceInfo
    {
        // 人脸框
        public MRECT faceRect;
        //人脸角度
        public int faceOrient;
    }

    /// <summary>
    /// 多人脸信息。
    /// </summary>
    public struct ASF_MultiFaceInfo
    {
        // 人脸框数组
        public IntPtr faceRects;
        // 人脸角度数组
        public IntPtr faceOrients;
        // 检测到的人脸数
        public int faceNum;
        // 一张人脸从进入画面直到离开画面，faceID不变。在VIDEO模式下有效，IMAGE模式下为空。
        public IntPtr faceID;
    }

    /// <summary>
    /// 人脸特征。
    /// </summary>
    public struct ASF_FaceFeature
    {
        // 人脸特征
        public IntPtr feature;
        // 人脸特征长度
        public int featureSize;
    }

    /// <summary>
    /// 年龄信息。
    /// </summary>
    public struct ASF_AgeInfo
    {
        //0:未知; >0:年龄
        IntPtr ageArray;
        //检测的人脸数
        int num;
    }

    /// <summary>
    /// 性别信息。
    /// </summary>
    public struct ASF_GenderInfo
    {
        //0:男性; 1:女性; -1:未知
        IntPtr genderArray;
        //检测的人脸数
        int num;
    }

    /// <summary>
    /// 3D角度信息。
    /// </summary>
    public struct ASF_Face3DAngle
    {
        //横滚角
        public IntPtr roll;
        //偏航角
        public IntPtr yaw;
        //俯仰角
        public IntPtr pitch;
        //0:正常; 非0:异常
        public IntPtr status;
        //检测的人脸个数
        public IntPtr num;
    }

    /// <summary>
    /// 活体置信度。
    /// </summary>
    public struct ASF_LivenessThreshold
    {
        // BGR活体检测阈值设置，默认值0.5
        float thresholdmodel_BGR;
        // IR活体检测阈值设置，默认值0.7
        float thresholdmodel_IR;
    }

    /// <summary>
    /// 活体信息。
    /// </summary>
    public struct ASF_LivenessInfo
    {
        //0:非真人； 1:真人；-1：不确定； -2:传入人脸数 > 1；-3: 人脸过小；-4: 角度过大；-5: 人脸超出边界
        public IntPtr isLive;
        //检测的人脸个数
        public int num;
    }

    /// <summary>
    /// 图像数据信息。
    /// </summary>
    public struct ASVLOFFSCREEN
    {
        public uint u32PixelArrayFormat;
        public int i32Width;
        public int i32Height;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.SysUInt)]
        public IntPtr[] ppu8Plane;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I4)]
        public int[] pi32Pitch;
    }
}
