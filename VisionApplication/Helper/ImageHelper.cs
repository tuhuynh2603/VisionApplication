
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows;

namespace VisionApplication.Helper
{
    class ImageHelper
    {
        public static byte[] LoadBufferSingleImage(string pathImage, ref int iwI, ref int ihI, ref int stride)
        {
            byte[] bufferCamera = new byte[0];
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Stream stream = new FileStream(pathImage, FileMode.Open, FileAccess.Read);
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(stream);

            
            LogMessage.WriteToDebugViewer(6, "Load Bitmap   " + stopwatch.ElapsedMilliseconds.ToString());
            stopwatch.Reset(); stopwatch.Start();
            // Lock the bitmap's bits.  
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                bmp.PixelFormat);
            LogMessage.WriteToDebugViewer(6, "Get BitmapData   " + stopwatch.ElapsedMilliseconds.ToString());
            stopwatch.Reset(); stopwatch.Start();
            int channels = bmpData.Stride / bmpData.Width;
            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;
            iwI = bmp.Width;
            ihI = bmp.Height;
            stride = bmp.Width;
            bufferCamera = new byte[iwI * ihI];
            if (channels > 1)
            {
                IntPtr ptr1 = Marshal.AllocHGlobal(bufferCamera.Length);
                Parallel.For(0, iwI * ihI - 1, i =>
                {
                    Marshal.WriteByte(ptr1, i, Marshal.ReadByte(ptr, i * channels));
                });
                LogMessage.WriteToDebugViewer(6, "ReadBuffer   " + stopwatch.ElapsedMilliseconds.ToString());
                stopwatch.Reset(); stopwatch.Start();
                Marshal.Copy(ptr1, bufferCamera, 0, bufferCamera.Length);
            }
            else
            {
                int x = bmpData.Stride - bmpData.Width;
                if (x != 0)
                {
                    Parallel.For(0, ihI, i =>
                    {
                        Marshal.Copy(ptr, bufferCamera, i * bmpData.Stride, bmpData.Width);
                    });
                }
                else
                    Marshal.Copy(ptr, bufferCamera, 0, bufferCamera.Length);
            }
            bmp.UnlockBits(bmpData);
            bmp.Dispose();
            stream.Dispose();
            LogMessage.WriteToDebugViewer(6, "Get Buffer   " + stopwatch.ElapsedMilliseconds.ToString());
            return bufferCamera;
        }
        public static byte[] LoadBufferColorImage(string pathImage, ref int iwI, ref int ihI, ref int stride)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Stream stream = new FileStream(pathImage, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(stream);

            // Lock the bitmap's bits.  
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                bmp.PixelFormat);
            int channels = bmpData.Stride / bmpData.Width;
            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;
            iwI = bmp.Width;
            stride = iwI * channels;
            ihI = bmp.Height;
            byte[] bufferCamera = new byte[stride * ihI];
            int x = bmpData.Stride - bmpData.Width * channels;
            if (x != 0)
            {
                int bmpStride = bmpData.Stride;
                int bmpWidth = bmpData.Width;
                int bmpHeight = bmpData.Height;
                byte[] temp = new byte[bmpData.Stride * bmpData.Height];
                Marshal.Copy(ptr, temp, 0, temp.Length);
                Parallel.For(0, ihI, i =>
                {
                    Array.Copy(temp, i * bmpStride, bufferCamera, i * bmpWidth * channels, bmpWidth * channels);
                });
            }
            else
            {
                Marshal.Copy(ptr, bufferCamera, 0, bufferCamera.Length);
            }
            bmp.UnlockBits(bmpData);
            bmp.Dispose();
            stream.Dispose();
            LogMessage.WriteToDebugViewer(6, "Get Buffer   " + stopwatch.ElapsedMilliseconds.ToString());
            return bufferCamera;
        }
        public static BitmapImage NotMVSDGetBitmapSource(string fileImage)
        {
            byte[] imageData = File.ReadAllBytes(fileImage);
            BitmapImage image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = mem;
                image.EndInit();
            }
            return image;
        }

        public static void SaveImage(string filePath, Bitmap image, long quality = 100L)
        {
            using (EncoderParameters encoderParameters = new EncoderParameters(1))
            using (EncoderParameter encoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality))
            {
                ImageCodecInfo codecInfo = ImageCodecInfo.GetImageDecoders().FirstOrDefault(codec => codec.FormatID == ImageFormat.Png.Guid);
                encoderParameters.Param[0] = encoderParameter;
                image.Save(filePath, codecInfo, encoderParameters);
            }
        }
        public static Bitmap GetBitmapFromBytes(byte[] imagebytes, int width, int height, int chanels)
        {
            Bitmap bm = null;
            if (chanels == 1)
            {
                bm = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                var BoundsRect = new Rectangle(0, 0, width, height);
                BitmapData bmpData = bm.LockBits(BoundsRect,
                                                ImageLockMode.WriteOnly,
                                                bm.PixelFormat);
                try
                {
                    IntPtr ptr = bmpData.Scan0;
                    ColorPalette cp = bm.Palette;
                    for (int i = 0; i < 256; i++)
                    {
                        cp.Entries[i] = Color.FromArgb(i, i, i);
                    }
                    // set palette back
                    bm.Palette = cp;
                    Parallel.For(0, height, i =>
                    {
                        Marshal.Copy(imagebytes, i * width, ptr + i * bmpData.Stride, width);
                    });
                }
                finally
                {
                    bm.UnlockBits(bmpData);
                }

            }
            else if (chanels == 3)
            {
                bm = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                var BoundsRect = new Rectangle(0, 0, width, height);
                BitmapData bmpData = bm.LockBits(BoundsRect,
                                                ImageLockMode.WriteOnly,
                                                bm.PixelFormat);
                try
                {
                    IntPtr ptr = bmpData.Scan0;
                    int skipByte = bmpData.Stride - width * 3;
                    byte[] newBuff = new byte[imagebytes.Length + skipByte * height];
                    for (int j = 0; j < height; j++)
                    {
                        Buffer.BlockCopy(imagebytes, j * width * 3, newBuff, j * (width * 3 + skipByte), width * 3);
                    }

                    ////convert RGB to BGR
                    //int length = Math.Abs(bmpData.Stride) * height;
                    //for (int i = 0; i < length; i += 3)
                    //{
                    //    byte dummy = newBuff[i];
                    //    newBuff[i] = newBuff[i + 2];
                    //    newBuff[i + 2] = dummy;
                    //}

                    Marshal.Copy(newBuff, 0, ptr, newBuff.Length);
                }
                finally
                {
                    bm.UnlockBits(bmpData);
                }
            }
            return bm;
        }
        public static BitmapSource CreateBlackBitmapSource(int width, int height)
        {
            int stride = width;//8bitperpixel
            byte[] pixels = new byte[height * stride];
            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
            colors.Add(System.Windows.Media.Color.FromRgb(0, 0, 0));
            BitmapPalette myPalette = new BitmapPalette(colors);
            BitmapSource image = BitmapSource.Create(width, height, 96, 96, System.Windows.Media.PixelFormats.Indexed8, BitmapPalettes.Gray256, pixels, stride);
            return image;
        }
    }
}
