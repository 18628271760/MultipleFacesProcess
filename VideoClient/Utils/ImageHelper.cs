using System;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using VideoClient.Models;

namespace VideoClient.Utils
{
    public static class ImageHelper
    {
        public static ImageInfo ReadBMPFromImage(Image image, ImageInfo imageInfo)
        {
            //将Image转换为Format24bppRgb格式的BMP
            Bitmap bm = new Bitmap(image);
            BitmapData data = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {
                Marshal.FreeHGlobal(imageInfo.imgData);
                //位图中第一个像素数据的地址。它也可以看成是位图中的第一个扫描行
                IntPtr ptr = data.Scan0;
                //定义数组长度
                int soureBitArrayLength = data.Height * Math.Abs(data.Stride);
                byte[] sourceBitArray = new byte[soureBitArrayLength];

                //将bitmap中的内容拷贝到ptr_bgr数组中
                Marshal.Copy(ptr, sourceBitArray, 0, soureBitArrayLength);

                //填充引用对象字段值
                imageInfo.width = data.Width;
                imageInfo.height = data.Height;
                imageInfo.format = ASF_ImagePixelFormat.ASVL_PAF_RGB24_B8G8R8;
                //获取去除对齐位后度图像数据
                int line = imageInfo.width * 3;
                int pitch = Math.Abs(data.Stride);
                int bgr_len = line * imageInfo.height;
                byte[] destBitArray = new byte[bgr_len];
                for (int i = 0; i < imageInfo.height; ++i)
                {
                    Array.Copy(sourceBitArray, i * pitch, destBitArray, i * line, line);
                }
                imageInfo.imgData = Marshal.AllocHGlobal(destBitArray.Length);
                Marshal.Copy(destBitArray, 0, imageInfo.imgData, destBitArray.Length);
                return imageInfo;
            }
            catch
            {
                imageInfo.imgData = IntPtr.Zero;
                imageInfo.format = 0;
                imageInfo.height = 0;
                imageInfo.width = 0;
                return imageInfo;
            }
            finally
            {
                bm.UnlockBits(data);
            }
        }

        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Png);
                    stream.Position = 0;
                    BitmapImage result = new BitmapImage();
                    result.BeginInit();
                    result.CacheOption = BitmapCacheOption.OnLoad;
                    result.StreamSource = stream;
                    result.EndInit();
                    result.Freeze();
                    return result;
                }
            }
            else
            {
                return null;
            }
        }

        public static ImageInfo ReadBMPFormStream(Stream stream)
        {
            ImageInfo imageInfo = new ImageInfo();
            Image image = Image.FromStream(stream);
            if (image.Width % 4 != 0)
            {
                image = ScaleImage(image, image.Width - (image.Width % 4), image.Height);
            }
            Bitmap bitmapImage = new Bitmap(image);
            BitmapData data = bitmapImage.LockBits(new Rectangle(0, 0, bitmapImage.Width, bitmapImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {
                //位图中第一个像素数据的地址。它也可以看成是位图中的第一个扫描行
                IntPtr ptr = data.Scan0;

                //定义数组长度
                int soureBitArrayLength = data.Height * Math.Abs(data.Stride);
                byte[] sourceBitArray = new byte[soureBitArrayLength];

                //将bitmap中的内容拷贝到ptr_bgr数组中
                Marshal.Copy(ptr, sourceBitArray, 0, soureBitArrayLength);

                //填充引用对象字段值
                imageInfo.width = data.Width;
                imageInfo.height = data.Height;
                imageInfo.format = ASF_ImagePixelFormat.ASVL_PAF_RGB24_B8G8R8;

                //获取去除对齐位后度图像数据
                int line = imageInfo.width * 3;
                int pitch = Math.Abs(data.Stride);
                int bgr_len = line * imageInfo.height;
                byte[] destBitArray = new byte[bgr_len];

                /*
                 * 图片像素数据在内存中是按行存储，一般图像库都会有一个内存对齐，在每行像素的末尾位置
                 * 每行的对齐位会使每行多出一个像素空间（三通道如RGB会多出3个字节，四通道RGBA会多出4个字节）
                 * 以下循环目的是去除每行末尾的对齐位，将有效的像素拷贝到新的数组
                 */
                for (int i = 0; i < imageInfo.height; ++i)
                {
                    Array.Copy(sourceBitArray, i * pitch, destBitArray, i * line, line);
                }
                imageInfo.imgData = Marshal.AllocHGlobal(destBitArray.Length);
                Marshal.Copy(destBitArray, 0, imageInfo.imgData, destBitArray.Length);
                return imageInfo;
            }
            finally
            {
                bitmapImage.UnlockBits(data);
            }
        }

        public static Image ScaleImage(Image image, int dstWidth, int dstHeight)
        {
            Graphics g = null;
            try
            {
                //按比例缩放           
                float scaleRate = GetWidthAndHeight(image.Width, image.Height, dstWidth, dstHeight);
                int width = (int)(image.Width * scaleRate);
                int height = (int)(image.Height * scaleRate);

                //将宽度调整为4的整数倍
                if (width % 4 != 0)
                {
                    width = width - width % 4;
                }

                Bitmap destBitmap = new Bitmap(width, height);
                g = Graphics.FromImage(destBitmap);
                g.Clear(Color.Transparent);

                //设置画布的描绘质量         
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, new Rectangle((width - width) / 2, (height - height) / 2, width, height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);

                //设置压缩质量     
                EncoderParameters encoderParams = new EncoderParameters();
                long[] quality = new long[1];
                quality[0] = 100;
                EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                encoderParams.Param[0] = encoderParam;

                return destBitmap;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (g != null)
                {
                    g.Dispose();
                }
            }
            return null;
        }

        public static float GetWidthAndHeight(int oldWidth, int oldHeigt, int newWidth, int newHeight)
        {
            //按比例缩放           
            float scaleRate = 0.0f;
            if (oldWidth >= newWidth && oldHeigt >= newHeight)
            {
                int widthDis = oldWidth - newWidth;
                int heightDis = oldHeigt - newHeight;
                if (widthDis > heightDis)
                {
                    scaleRate = newWidth * 1f / oldWidth;
                }
                else
                {
                    scaleRate = newHeight * 1f / oldHeigt;
                }
            }
            else if (oldWidth >= newWidth && oldHeigt < newHeight)
            {
                scaleRate = newWidth * 1f / oldWidth;
            }
            else if (oldWidth < newWidth && oldHeigt >= newHeight)
            {
                scaleRate = newHeight * 1f / oldHeigt;
            }
            else
            {
                int widthDis = newWidth - oldWidth;
                int heightDis = newHeight - oldHeigt;
                if (widthDis > heightDis)
                {
                    scaleRate = newHeight * 1f / oldHeigt;
                }
                else
                {
                    scaleRate = newWidth * 1f / oldWidth;
                }
            }
            return scaleRate;
        }
    }
}
