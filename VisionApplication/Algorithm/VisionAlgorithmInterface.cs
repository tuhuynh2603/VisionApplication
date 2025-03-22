using Emgu.CV.CvEnum;
using Emgu.CV;
using System;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace VisionApplication.Algorithm
{
    public static class VisionAlgorithmInterface
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Point2d
        {
            public double x, y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct s_SingleTargetMatch
        {
            public Point2d ptLT, ptRT, ptRB, ptLB, ptCenter;
            public double dMatchedAngle;
            public double dMatchScore;
        }

        public static class StringUntil
        {
            public static List<s_SingleTargetMatch> ParseMatches(string data)
            {
                var matches = new List<s_SingleTargetMatch>();

                string[] entries = data.Split('|', StringSplitOptions.RemoveEmptyEntries);
                foreach (string entry in entries)
                {
                    string[] values = entry.Split(';');

                    if (values.Length != 7) continue;

                    s_SingleTargetMatch match = new s_SingleTargetMatch
                    {
                        ptLT = ParsePoint(values[0]),
                        ptRT = ParsePoint(values[1]),
                        ptRB = ParsePoint(values[2]),
                        ptLB = ParsePoint(values[3]),
                        ptCenter = ParsePoint(values[4]),
                        dMatchedAngle = double.Parse(values[5]),
                        dMatchScore = double.Parse(values[6])
                    };

                    matches.Add(match);
                }
                return matches;
            }

            private static Point2d ParsePoint(string data)
            {
                string[] xy = data.Split(',');
                return new Point2d { x = double.Parse(xy[0]), y = double.Parse(xy[1]) };
            }
        }

        public static class VisionAlgorithm
        {
            [DllImport("VisionAlgoritiomMagnus.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern void InitializeTemplate(IntPtr buffer, int width, int height, int channels, int minReduceArea);

            [DllImport("VisionAlgoritiomMagnus.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr DoInspect(IntPtr buffer, int width, int height, int channels,
                int minReduceArea, bool bitwiseNot, bool toleranceRange,
                double tolerance1, double tolerance2, double tolerance3, double tolerance4, double toleranceAngle,
                double score, int maxPos, double maxOverlap,
                bool debugMode, bool subPixel, bool stopLayer1);

            public static List<s_SingleTargetMatch> FindTemplate(byte[] imageBuffer, int width, int height, int channels)
            {
                GCHandle handle = GCHandle.Alloc(imageBuffer, GCHandleType.Pinned);
                IntPtr bufferPtr = handle.AddrOfPinnedObject();
                int size;
                var results = new List<s_SingleTargetMatch>();
                try
                {
                    var ptr = DoInspect(bufferPtr, width, height, channels,
                        256, false, false,
                        40, 60, -110, -100, 30,
                        0.5, 70, 0,
                        false, false, false);

                    if (ptr == IntPtr.Zero) return new List<s_SingleTargetMatch>();

                    byte[] buffer = new byte[10000];
                    Marshal.Copy(ptr, buffer, 0, buffer.Length);
                    string parseResult = Encoding.ASCII.GetString(buffer).Split('\0')[0];

                    if (string.IsNullOrEmpty(parseResult))
                        return new List<s_SingleTargetMatch>();

                    return StringUntil.ParseMatches(parseResult);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return new List<s_SingleTargetMatch>();
                }
                finally
                {
                    handle.Free();
                }

            }

            public static byte[] LoadImage(string path, ref int imageWidth, ref int imageHeight, ref int channels)
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"File not found: {path}");
                }

                using (Bitmap bitmap = new Bitmap(path))
                {
                    imageWidth = bitmap.Width;
                    imageHeight = bitmap.Height;
                    channels = 1; // Grayscale

                    Bitmap grayscaleBitmap = new Bitmap(imageWidth, imageHeight, PixelFormat.Format8bppIndexed);

                    // Chỉnh bảng màu thành grayscale
                    ColorPalette palette = grayscaleBitmap.Palette;
                    for (int i = 0; i < 256; i++)
                        palette.Entries[i] = Color.FromArgb(i, i, i);
                    grayscaleBitmap.Palette = palette;

                    // Convert ảnh thành grayscale nhanh hơn
                    Rectangle rect = new Rectangle(0, 0, imageWidth, imageHeight);
                    BitmapData bmpData = grayscaleBitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

                    int stride = bmpData.Stride;
                    byte[] pixelData = new byte[stride * imageHeight];

                    for (int y = 0; y < imageHeight; y++)
                    {
                        for (int x = 0; x < imageWidth; x++)
                        {
                            Color color = bitmap.GetPixel(x, y);
                            byte gray = (byte)(0.299 * color.R + 0.587 * color.G + 0.114 * color.B);
                            pixelData[y * stride + x] = gray;
                        }
                    }

                    Marshal.Copy(pixelData, 0, bmpData.Scan0, pixelData.Length);
                    grayscaleBitmap.UnlockBits(bmpData);

                    return pixelData;
                }
            }

            public static byte[] templateData;
            public static byte[] MatToByteArray(Mat mat)
            {
                if (mat == null || mat.IsEmpty)
                    throw new ArgumentException("Mat is empty or null");

                int size = mat.Rows * mat.Cols * mat.ElementSize; // Tổng số byte trong Mat
                byte[] buffer = new byte[size];

                mat.CopyTo(buffer); // Sao chép dữ liệu vào buffer

                return buffer;
            }

            public static void SendTemplateImage(byte[] templateData, int width, int height, int channels, int minReduceArea = 256)
            {
                GCHandle handle = GCHandle.Alloc(templateData, GCHandleType.Pinned);
                IntPtr bufferPtr = handle.AddrOfPinnedObject();

                try
                {
                    InitializeTemplate(bufferPtr, width, height, channels, minReduceArea);
                }
                finally
                {
                    handle.Free();
                }
            }
        }

        //public static VisionAlgorithmInterface()
        //{
            //int imgWidth = 0, imgHeight = 0, channels = 1;

            //// Load template image
            //byte[] templateData = VisionAlgorithm.LoadImage("C:\\Wisely\\C++\\Fastest_Image_Pattern_Matching\\PatternMatching\\Test Images\\Dst2.bmp", ref imgWidth, ref imgHeight, ref channels);
            //VisionAlgorithm.SendTemplateImage(templateData, imgWidth, imgHeight, channels);

            //// Load image for inspection
            //byte[] inspectImage = VisionAlgorithm.LoadImage("C:\\Wisely\\C++\\Fastest_Image_Pattern_Matching\\PatternMatching\\Test Images\\Src2.bmp", ref imgWidth, ref imgHeight, ref channels);

            //var results = VisionAlgorithm.InspectImage(inspectImage, imgWidth, imgHeight, channels);
       // }



    }
}
