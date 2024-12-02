using Emgu.CV.CvEnum;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VisionApplication.Algorithm
{
    public class VisionAlgorithmInterface
    {

        [DllImport("C:\\Wisely\\C#\\TestMatching\\TestMatching\\VisionAlgoritiomMagnus.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void InitializeTemplate(IntPtr buffer, int width, int height, int channels, int minReduceArea);


        [DllImport("C:\\Wisely\\C#\\TestMatching\\TestMatching\\VisionAlgoritiomMagnus.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void DoInspect(IntPtr buffer, int width, int height, int channels,
            int minReduceArea, bool bitwiseNot, bool toleranceRange,
            double tolerance1, double tolerance2, double tolerance3, double tolerance4, double toleranceAngle,
            double score, int maxPos, double maxOverlap,
            bool debugMode, bool subPixel, bool stopLayer1);

        void InspectImage(byte[] imageBuffer, int width, int height, int channels)
        {
            GCHandle handle = GCHandle.Alloc(imageBuffer, GCHandleType.Pinned);
            IntPtr bufferPtr = handle.AddrOfPinnedObject();

            try
            {
                DoInspect(bufferPtr, width, height, channels,
                    256, false, true,
                    40, 60, -110, -100, 30,
                    0.5, 70, 0,
                    false, false, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                handle.Free();
            }
        }

        static void SendTemplateImage(byte[] templateData, int width, int height, int channels, int minReduceArea = 256)
        {
            GCHandle handle = GCHandle.Alloc(templateData, GCHandleType.Pinned);
            IntPtr bufferPtr = handle.AddrOfPinnedObject();

            Mat mat = new Mat(height, width, channels == 1 ? DepthType.Cv8U : DepthType.Cv8U, channels);
            Marshal.Copy(templateData, 0, mat.DataPointer, templateData.Length);
            CvInvoke.Imwrite("template Image.bmp", mat);
            try
            {
                InitializeTemplate(bufferPtr, width, height, channels, minReduceArea);
            }
            finally
            {
                handle.Free();
            }
        }

        public VisionAlgorithmInterface()
        {
            byte[] templateData = File.ReadAllBytes("C:\\Wisely\\C#\\TestMatching\\TestMatching\\Dst2.bmp");


            SendTemplateImage(templateData, 529, 133, 1); // Gửi template image dưới dạng grayscale

            byte[] inspectImage = File.ReadAllBytes("C:\\Wisely\\C#\\TestMatching\\TestMatching\\Src2.bmp");

            InspectImage(inspectImage, 2592, 1944, 1);
        }



    }
}
