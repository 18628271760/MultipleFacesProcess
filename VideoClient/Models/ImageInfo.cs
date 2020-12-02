using System;

namespace VideoClient.Models
{
    public class ImageInfo
    {
        public IntPtr imgData { get; set; }

        /// <summary>
        /// 图片像素宽
        /// </summary>
        public int width { get; set; }

        /// <summary>
        /// 图片像素高
        /// </summary>
        public int height { get; set; }

        /// <summary>
        /// 图片格式
        /// </summary>
        public int format { get; set; }
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
}
