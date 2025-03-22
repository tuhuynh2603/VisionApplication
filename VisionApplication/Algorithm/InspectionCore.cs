﻿using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionApplication.Define;
using CvContourArray = Emgu.CV.Util.VectorOfVectorOfPoint;
using CvImage = Emgu.CV.Mat;
using CvPointArray = Emgu.CV.Util.VectorOfPoint;
namespace VisionApplication.Algorithm
{
    using Emgu.CV.Util;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Forms.VisualStyles;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Media3D;
    using VisionApplication.Helper.UIImage;
    using VisionApplication.Model;
    using static VisionApplication.Algorithm.VisionAlgorithmInterface;

    public class InspectionCore
    {


        //public VisionAlgorithmInterface Algorithm { get; set; }

        public Size globalImageSize;
        TemplateMatchingModel m_TemplateMatchingModel = new TemplateMatchingModel();
        public struct ImageTarget
        {
            public CvImage Gray;
            public CvImage Bgr;
        }

        public class DeviceLocationParameter
        {
            public/* static*/ Rectangles m_L_DeviceLocationRoi = new Rectangles();
            public bool m_L_LocationEnable = true;

            public THRESHOLD_TYPE m_L_ThresholdType = THRESHOLD_TYPE.BINARY_THRESHOLD;
            public OBJECT_COLOR m_L_ObjectColor = OBJECT_COLOR.BLACK;
            public/* static*/ int m_L_lowerThreshold = 0;
            public/* static*/ int m_L_upperThreshold = 255;

            public/* static*/ int m_L_lowerThresholdInnerChip = 0;
            public/* static*/ int m_L_upperThresholdInnerChip = 255;

            public/* static*/ int m_L_OpeningMask = 11;
            public/* static*/ int m_L_MinWidthDevice = 50;
            public/* static*/ int m_L_MinHeightDevice = 50;
            public/* static*/ int m_L_DilationMask = 30;

            public/* static*/ Rectangles m_L_TemplateRoi = new Rectangles();
            public/* static*/ int m_L_StepTemplate = 4;
            public/* static*/ double m_L_ScaleImageRatio = 0.1;
            public/* static*/ double m_L_MinScore = 50.0;
            public/* static*/ int m_L_CornerIndex = 0;


            public int m_DR_NumberROILocation = 1;
            public int m_DR_DefectROIIndex = 0;
        }

        public class DeviceLocationResult
        {
            public PointF m_dCenterDevicePoint = new PointF(0, 0);
            public PointF m_dCornerDevicePoint = new PointF(0, 0);
            public double m_dAngleOxDevice = 0.0;
        }


        public class BlackChipParameter
        {
            public/* static*/ bool m_OC_EnableCheck = true;
            public/* static*/ int m_OC_lowerThreshold = 0;
            public/* static*/ int m_OC_upperThreshold = 50;
            public/* static*/ int m_OC_DilationMask = 0;
            public/* static*/ int m_OC_OpeningMask = 0;
            public int m_OC_MinWidthDevice = 200;
            public int m_OC_MinHeightDevice = 200;

        }
        public class SurfaceDefectParameter
        {

            public Rectangles m_DR_DefectROILocations = new Rectangles();
            public/* static*/ bool m_DR_AreaEnable = false;


            public/* static*/ int m_LD_lowerThreshold = 0;
            public/* static*/ int m_LD_upperThreshold = 50;
            public/* static*/ int m_LD_DilationMask = 0;
            public/* static*/ int m_LD_OpeningMask = 0;
            public int m_LD_ObjectCoverPercent = 50;
        }

        public /*static*/ ImageTarget m_SourceImage;
        public /*static*/ ImageTarget m_TeachImage;
        public /*static*/ ImageTarget m_TemplateImage;

        public /*static*/ DeviceLocationParameter m_DeviceLocationParameter;
        public /*static*/ BlackChipParameter m_blackChipParameter;

        public /*static*/ SurfaceDefectParameter[] m_SurfaceDefectParameter = new SurfaceDefectParameter[AppMagnus.TOTAL_AREA];
        public DeviceLocationResult m_DeviceLocationResult;
        public InspectionCore(ref Size mImageSize)
        {
            //Algorithm = new VisionAlgorithmInterface();
            globalImageSize = new Size(mImageSize.Width, mImageSize.Height);
            m_SourceImage = new ImageTarget();
            m_TeachImage = new ImageTarget();
            m_TemplateImage = new ImageTarget();
            m_DeviceLocationParameter = new DeviceLocationParameter();
            m_blackChipParameter = new BlackChipParameter();
            for (int nArea = 0; nArea < AppMagnus.TOTAL_AREA; nArea++)
                m_SurfaceDefectParameter[nArea] = new SurfaceDefectParameter();

            m_DeviceLocationResult = new DeviceLocationResult();
        }

        public /*static*/ bool LoadImageToInspection(BitmapSource btmSource)
        {
            if (btmSource == null)
                return false;
            try
            {
                CvImage imgBgr = BitmapSourceConvert.ToMat(btmSource);
                BitmapSource btmSourceGray = new FormatConvertedBitmap(btmSource, PixelFormats.Gray8, null, 0);
                CvImage imgGray = BitmapSourceConvert.ToMat(btmSourceGray);
                m_SourceImage.Gray = imgGray.Clone();
                m_SourceImage.Bgr = imgBgr.Clone();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public /*static*/ bool LoadOfflineImage(string strPath)
        {
            try
            {
                m_SourceImage.Gray = CvInvoke.Imread(strPath, Emgu.CV.CvEnum.ImreadModes.Grayscale);
                m_SourceImage.Bgr = CvInvoke.Imread(strPath, Emgu.CV.CvEnum.ImreadModes.Color);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public /*static*/ bool UpdateTeachParamFromUIToInspectionCore(CategoryTeachParameter categoriesTeachParam, int nTrack)
        {

            m_DeviceLocationParameter.m_L_ThresholdType = categoriesTeachParam.L_ThresholdType;
            m_DeviceLocationParameter.m_L_LocationEnable = categoriesTeachParam.L_LocationEnable;
            m_DeviceLocationParameter.m_L_ObjectColor = categoriesTeachParam.L_ObjectColor;


            m_DeviceLocationParameter.m_L_lowerThreshold = categoriesTeachParam.L_lowerThreshold;
            m_DeviceLocationParameter.m_L_upperThreshold = categoriesTeachParam.L_upperThreshold;

            m_DeviceLocationParameter.m_L_lowerThresholdInnerChip = categoriesTeachParam.L_lowerThresholdInnerChip;
            m_DeviceLocationParameter.m_L_upperThresholdInnerChip = categoriesTeachParam.L_upperThresholdInnerChip;


            if (categoriesTeachParam.L_DeviceLocationRoi.Width > globalImageSize.Width
                || categoriesTeachParam.L_DeviceLocationRoi.GetRectangle().TopLeft.X >= globalImageSize.Width
                || categoriesTeachParam.L_DeviceLocationRoi.GetRectangle().TopLeft.Y >= globalImageSize.Height)
                categoriesTeachParam.L_DeviceLocationRoi.SetRectangle(new VisionApplication.Define.Rectangles(new System.Windows.Point(globalImageSize.Width / 2, globalImageSize.Height / 2), 300, 300));

            m_DeviceLocationParameter.m_L_DeviceLocationRoi = categoriesTeachParam.L_DeviceLocationRoi.GetRectangle();
            m_DeviceLocationParameter.m_L_OpeningMask = categoriesTeachParam.L_OpeningMask;
            m_DeviceLocationParameter.m_L_DilationMask = categoriesTeachParam.L_DilationMask;
            m_DeviceLocationParameter.m_L_MinWidthDevice = categoriesTeachParam.L_MinWidthDevice;
            m_DeviceLocationParameter.m_L_MinHeightDevice = categoriesTeachParam.L_MinHeightDevice;

            if (categoriesTeachParam.L_TemplateRoi.Width > globalImageSize.Width ||
                categoriesTeachParam.L_TemplateRoi.GetRectangle().TopLeft.X >= globalImageSize.Width
                || categoriesTeachParam.L_TemplateRoi.GetRectangle().TopLeft.Y >= globalImageSize.Height)
                categoriesTeachParam.L_TemplateRoi.SetRectangle(new VisionApplication.Define.Rectangles(new System.Windows.Point(globalImageSize.Width / 2, globalImageSize.Height / 2), 300, 300));

            m_DeviceLocationParameter.m_L_TemplateRoi = categoriesTeachParam.L_TemplateRoi.GetRectangle();
            m_DeviceLocationParameter.m_L_StepTemplate = categoriesTeachParam.L_NumberSide;
            m_DeviceLocationParameter.m_L_ScaleImageRatio = categoriesTeachParam.L_ScaleImageRatio;
            m_DeviceLocationParameter.m_L_MinScore = categoriesTeachParam.L_MinScore;
            m_DeviceLocationParameter.m_L_CornerIndex = categoriesTeachParam.L_CornerIndex;

            m_blackChipParameter.m_OC_EnableCheck = categoriesTeachParam.OC_EnableCheck;
            m_blackChipParameter.m_OC_lowerThreshold = categoriesTeachParam.OC_lowerThreshold;
            m_blackChipParameter.m_OC_upperThreshold = categoriesTeachParam.OC_upperThreshold;
            m_blackChipParameter.m_OC_OpeningMask = categoriesTeachParam.OC_OpeningMask;
            m_blackChipParameter.m_OC_DilationMask = categoriesTeachParam.OC_DilationMask;
            m_blackChipParameter.m_OC_MinWidthDevice = categoriesTeachParam.OC_MinWidthDevice;
            m_blackChipParameter.m_OC_MinHeightDevice = categoriesTeachParam.OC_MinHeightDevice;

            m_DeviceLocationParameter.m_DR_NumberROILocation = categoriesTeachParam.L_NumberROILocation;

            return true;
        }
        public bool UpdateAreaParameterFromUIToInspectionCore(CategoryVisionParameter areaParam, int nArea)
        {


            if (areaParam.LD_DefectROILocation.Width > globalImageSize.Width ||
            areaParam.LD_DefectROILocation.GetRectangle().TopLeft.X >= globalImageSize.Width
                || areaParam.LD_DefectROILocation.GetRectangle().TopLeft.Y >= globalImageSize.Height)
                areaParam.LD_DefectROILocation.SetRectangle(new Rectangles(new System.Windows.Point(globalImageSize.Width / 2, globalImageSize.Height / 2), 300, 300));

            m_SurfaceDefectParameter[nArea].m_DR_DefectROILocations = areaParam.LD_DefectROILocation.GetRectangle();
            m_SurfaceDefectParameter[nArea].m_DR_AreaEnable = areaParam.LD_AreaEnable;
            m_SurfaceDefectParameter[nArea].m_LD_lowerThreshold = areaParam.LD_lowerThreshold;
            m_SurfaceDefectParameter[nArea].m_LD_upperThreshold = areaParam.LD_upperThreshold;
            m_SurfaceDefectParameter[nArea].m_LD_OpeningMask = areaParam.LD_OpeningMask;
            m_SurfaceDefectParameter[nArea].m_LD_DilationMask = areaParam.LD_DilationMask;
            m_SurfaceDefectParameter[nArea].m_LD_ObjectCoverPercent = areaParam.LD_ObjectCoverPercent;

            return true;
        }

        public /*static*/ bool LoadTeachImageToInspectionCore(int nTrack)
        {
            try
            {
                m_SourceImage.Gray = CvInvoke.Imread(System.IO.Path.Combine(AppMagnus.pathRecipe, AppMagnus.currentRecipe, "teachImage_Track" + (nTrack + 1).ToString() + ".bmp"), Emgu.CV.CvEnum.ImreadModes.Grayscale);
                m_TeachImage.Gray = CvInvoke.Imread(System.IO.Path.Combine(AppMagnus.pathRecipe, AppMagnus.currentRecipe, "teachImage_Track" + (nTrack + 1).ToString() + ".bmp"), Emgu.CV.CvEnum.ImreadModes.Grayscale);
                m_TemplateImage.Gray = CvInvoke.Imread(System.IO.Path.Combine(AppMagnus.pathRecipe, AppMagnus.currentRecipe, "templateImage_Track" + (nTrack + 1).ToString() + ".bmp"), Emgu.CV.CvEnum.ImreadModes.Grayscale);
                var nWidth = m_TemplateImage.Gray.Width;
                var nHeight = m_TemplateImage.Gray.Height;
                var nChanels = 1;
                var buff = VisionAlgorithmInterface.VisionAlgorithm.MatToByteArray(m_TemplateImage.Gray);
                VisionAlgorithmInterface.VisionAlgorithm.SendTemplateImage(buff, nWidth, nHeight, nChanels);
            }
            catch { }

            return true;
        }

        public /*static*/ void PushBackDebugInfors(CvImage debugImage, CvImage debugRegion, string debugMessage, bool bEnableDebug, ObservableCollection<DebugInfors> debugInfors)
        {
            if (!bEnableDebug)
            {
                return;
            }

            if (debugRegion.Width == 0)
                debugRegion = CvImage.Zeros(m_SourceImage.Gray.Height, m_SourceImage.Gray.Width, DepthType.Cv8U, 1);

            CvImage zoomedImage = new CvImage(m_SourceImage.Gray.Height, m_SourceImage.Gray.Width, DepthType.Cv8U, 3);
            CvImage zoomedRegion = new CvImage(m_SourceImage.Gray.Height, m_SourceImage.Gray.Width, DepthType.Cv8U, 1);
            CvInvoke.Resize(debugImage, zoomedImage, new System.Drawing.Size(zoomedImage.Width, zoomedImage.Height));
            CvInvoke.Resize(debugRegion, zoomedRegion, new System.Drawing.Size(zoomedImage.Width, zoomedImage.Height));
            debugInfors.Add(new DebugInfors() { mat_Image = zoomedImage, mat_Region = zoomedRegion, str_Step = (debugInfors.Count() + 1).ToString(), str_Message = debugMessage });

        }

        public /*static*/ void AddRegionOverlay(ref List<ArrayOverLay> list_arrayOverlay, Mat mat_region, System.Windows.Media.Color color)
        {

            if (mat_region.Width == 0)
                mat_region = CvImage.Zeros(m_SourceImage.Gray.Height, m_SourceImage.Gray.Width, DepthType.Cv8U, 1);

            CvImage zoomedRegion = new CvImage(m_SourceImage.Gray.Height, m_SourceImage.Gray.Width, DepthType.Cv8U, 1);
            CvInvoke.Resize(mat_region, zoomedRegion, new System.Drawing.Size(zoomedRegion.Width, zoomedRegion.Height));
            list_arrayOverlay.Add(new ArrayOverLay() { mat_Region = zoomedRegion, _color = color });

        }

        public /*static*/ int Inspect(ref ImageTarget InspectImage, ref List<ArrayOverLay> list_arrayOverlay, ref List<Point2d> pCenter, ref List<Point2d> pCorner, ObservableCollection<DebugInfors> debugInfors, bool bEnableDebug = false)
        {
            int nError;
            Stopwatch timeIns = new Stopwatch();
            timeIns.Start();
            //double dScale = 3;

            var w = InspectImage.Gray.Width;
            var h = InspectImage.Gray.Height;
            var c = 1;

            ////Step 1: Find Device location////////
            ///// Filter ??
            //             + Threshold  1
            //             + Adaptive Threshold   1
            //             + Template Matching    1
            //             + Cross Point          0

            // Algorithm
            // Find pvi device location ?? 
            // Filter ?? 
            // 
            //             + Line
            //             + General
            //             + overlap
            //             + custom algorithm threshold => mopho (dilation, erosion, opening, closing)
            //                                             + Priority
            //                                             + Mark size
            //                                              



            var buffer = VisionAlgorithmInterface.VisionAlgorithm.MatToByteArray(InspectImage.Gray);
            var matchingResults = VisionAlgorithmInterface.VisionAlgorithm.FindTemplate(buffer, w, h, c);

            //nError = FindDeviceLocation_Zoom(ref InspectImage.Gray,
            //ref list_arrayOverlay, ref pCenter, ref pCorner,debugInfors, bEnableDebug);

            if (matchingResults.Count == 0)
            {
                var a = new Point2d();
                a.x = 0; a.y = 0;
                pCenter.Add(a);
                pCorner.Add(a);
                return -(int)ERROR_CODE.NO_PATTERN_FOUND;
            }

            pCenter = matchingResults.OrderBy(s => s.dMatchScore).Select(s=> s.ptCenter).ToList();
            pCorner = matchingResults.OrderBy(s => s.dMatchScore).Select(s => s.ptLT).ToList();

            CvImage mat_point = new CvImage();
            mat_point = CvImage.Zeros(InspectImage.Gray.Height, InspectImage.Gray.Width, DepthType.Cv8U, 1);
            for (var i = 0; i < pCenter.Count; i++)
            {

                Point pCenterOverLay = new Point((int)pCenter[i].x, (int)pCenter[i].y);
                CvInvoke.Circle(mat_point, pCenterOverLay, 11, new MCvScalar(255), 3);
                AddRegionOverlay(ref list_arrayOverlay, mat_point, Colors.Blue);
            }

            return -(int)ERROR_CODE.PASS;
        }

        public /*static*/ void SetTemplateImage()
        {
            m_TeachImage.Gray.ToImage<Gray, byte>();
            Image<Gray, Byte> imgCropped = m_TeachImage.Gray.ToImage<Gray, byte>().Clone();
            System.Drawing.Rectangle rec = new System.Drawing.Rectangle();
            rec.X = (int)m_DeviceLocationParameter.m_L_TemplateRoi.TopLeft.X;
            rec.Y = (int)m_DeviceLocationParameter.m_L_TemplateRoi.TopLeft.Y;
            rec.Width = (int)m_DeviceLocationParameter.m_L_TemplateRoi.Width;
            rec.Height = (int)m_DeviceLocationParameter.m_L_TemplateRoi.Height;
            imgCropped.ROI = rec;
            m_TemplateImage.Gray = imgCropped.Mat.Clone();
        }

        public void SaveCurrentSourceImage(string strFullFileName, int nType = 0)
        {
            if (nType == 0)
            {
                CvInvoke.Imwrite(strFullFileName, m_SourceImage.Gray);
            }
            else
            {
                //m_SourceImage.Gray.Save(strFullFileName);
                string strtemp = strFullFileName.Replace(".bmp", ".jpg");
                CvInvoke.Imwrite(strtemp, m_SourceImage.Gray, new KeyValuePair<ImwriteFlags, int>(ImwriteFlags.JpegQuality, 20));


            }
        }
        public enum MophoType
        {
            OpeningCircle,
            ClosingCircle,
            DilationCircle,
            ErodeCircle,
            OpeningRectangle,
            ClosingRectangle,
            DilationRectangle,
            ErodeRectangle
        }

        public enum OffsetType
        {
            Circle,
            Rectangle
        }

        //public int CustomInspection()
        //{
        
        //}
        public CvImage GetThresholdImage(ref CvImage imgSource, THRESHOLD_TYPE type, OBJECT_COLOR colorType, List<int> param, ref CvImage imgSourceForHighlight, ref List<ArrayOverLay> list_arrayOverlay, ObservableCollection<DebugInfors> debugInfors, bool bEnableDebug = false)
        {
            CvImage outputMat = new CvImage();
            if (type == THRESHOLD_TYPE.BINARY_THRESHOLD)
            {
                MagnusOpenCVLib.Threshold2(ref imgSource, ref outputMat, param[0], param[1]);

            }
            else//if (m_DeviceLocationParameter.m_L_ThresholdType.GetHashCode() == (int)THRESHOLD_TYPE.VAR_THRESHOLD)
            {
                MagnusOpenCVLib.VarThresholding(ref imgSource, ref outputMat, (int)colorType, 2 * (param[1]) + 3, 2 * (param[2]) + 3);
            }

            if (bEnableDebug)
            {
                Stopwatch timeIns = new Stopwatch();
                timeIns.Start();
                PushBackDebugInfors(imgSourceForHighlight, outputMat, $"Get Threshold Image {type.ToString()}. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug, debugInfors);
            }

            return outputMat;
        }

        public CvImage GetOffsetRegion(ref CvImage imgSource, OffsetType type, int offsetSize, ref CvImage imgSourceForHighlight, ref List<ArrayOverLay> list_arrayOverlay, ObservableCollection<DebugInfors> debugInfors, bool bEnableDebug = false)
        {
            CvImage outputMat = new CvImage();
            var mophoType = MophoType.OpeningCircle;
            var param = new List<int>();
            if (type == OffsetType.Circle)
            {
                if (offsetSize < 0)
                    mophoType = MophoType.ErodeCircle;
                else
                    mophoType = MophoType.DilationCircle;
                param.Add(offsetSize);
                param.Add(1);

            }
            else
            {
                if (offsetSize < 0)
                    mophoType = MophoType.ErodeRectangle;
                else
                    mophoType = MophoType.DilationRectangle;

                param.Add(offsetSize);
                param.Add(offsetSize);
                param.Add(1);
            }

            outputMat = MophoAlgorithm(ref imgSource, mophoType, param, ref imgSourceForHighlight, ref list_arrayOverlay, debugInfors, bEnableDebug);

            if (bEnableDebug)
            {
                Stopwatch timeIns = new Stopwatch();
                timeIns.Start();
                PushBackDebugInfors(imgSourceForHighlight, outputMat, $"Get Offset Region {type.ToString()}. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug, debugInfors);
            }

            return outputMat;
        }

        public CvImage MophoAlgorithm(ref CvImage imgSource, MophoType type, List<int> param, ref CvImage imgSourceForHighlight, ref List<ArrayOverLay> list_arrayOverlay, ObservableCollection<DebugInfors> debugInfors, bool bEnableDebug = false)
        {
            CvImage outputMat = new CvImage();
            switch (type)
            {
                case MophoType.OpeningCircle:
                    {
                        MagnusOpenCVLib.OpenningCircle_Magnus(ref imgSource, ref outputMat, param[0], param[1]);
                        break;
                    }
                case MophoType.ClosingCircle:
                    {
                        MagnusOpenCVLib.ClosingCircle_Magnus(ref imgSource, ref outputMat, param[0], param[1]);
                        break;
                    }
                case MophoType.DilationCircle:
                    {
                        MagnusOpenCVLib.DilationCircle_Magnus(ref imgSource, ref outputMat, param[0], param[1]);
                        break;
                    }
                case MophoType.ErodeCircle:
                    {
                        MagnusOpenCVLib.ErodeCircle_Magnus(ref imgSource, ref outputMat, param[0], param[1]);
                        break;
                    }
                case MophoType.OpeningRectangle:
                    {
                        MagnusOpenCVLib.OpeningRectangle(ref imgSource, ref outputMat, param[0], param[1], param[2]);
                        break;
                    }
                case MophoType.ClosingRectangle:
                    {
                        MagnusOpenCVLib.ClosingRectangle(ref imgSource, ref outputMat, param[0], param[1], param[2]);
                        break;
                    }
                case MophoType.DilationRectangle:
                    {
                        MagnusOpenCVLib.DilationRectangle(ref imgSource, ref outputMat, param[0], param[1], param[2]);
                        break;
                    }
                case MophoType.ErodeRectangle:
                    {
                        MagnusOpenCVLib.ErodeRectangle(ref imgSource, ref outputMat, param[0], param[1], param[2]);
                        break;
                    }

                default:
                    break;
            }

            if (bEnableDebug)
            {
                Stopwatch timeIns = new Stopwatch();
                timeIns.Start();
                PushBackDebugInfors(imgSourceForHighlight, outputMat, $"{type.ToString()}. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug, debugInfors);
            }

            return outputMat;
        }

        public int FindBlackChip(ref CvImage imgSource, ref CvImage mat_RegionInspectionROI, ref PointF pCenter, ref List<ArrayOverLay> list_arrayOverlay, ObservableCollection<DebugInfors> debugInfors, bool bEnableDebug = false)
        {
            Stopwatch timeIns = new Stopwatch();

            //if (m_TemplateImage.Gray == null)
            //    return -99;
            timeIns.Start();
            int m_nLowerBlackChipThreshold = m_blackChipParameter.m_OC_lowerThreshold;
            int m_nUpperBlackChipThreshold = m_blackChipParameter.m_OC_upperThreshold;
            int m_nOpeningBlackChipThreshold = m_blackChipParameter.m_OC_OpeningMask;
            int m_nOpeningBlackChipThreshold2 = m_blackChipParameter.m_OC_DilationMask;
            int m_OC_MinWidthDevice = m_blackChipParameter.m_OC_MinWidthDevice;
            int m_OC_MinHeightDevice = m_blackChipParameter.m_OC_MinHeightDevice;


            CvImage img_thresholdRegion = new CvImage();

            CvImage blurredImage = new CvImage();
            CvInvoke.GaussianBlur(imgSource, blurredImage, new Size(9, 9), 0);
            PushBackDebugInfors(blurredImage, img_thresholdRegion, "Check Black Chip Region: Blur Image. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();

            MagnusOpenCVLib.Threshold2(ref blurredImage, ref img_thresholdRegion, m_nLowerBlackChipThreshold, m_nUpperBlackChipThreshold);
            CvInvoke.BitwiseAnd(mat_RegionInspectionROI, img_thresholdRegion, img_thresholdRegion);
            PushBackDebugInfors(imgSource, img_thresholdRegion, "Check Black Chip Region: img_thresholdRegion. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();

            CvImage img_OpeningRegion = new CvImage();
            MagnusOpenCVLib.OpenningCircle_Magnus(ref img_thresholdRegion, ref img_OpeningRegion, m_nOpeningBlackChipThreshold);
            PushBackDebugInfors(imgSource, img_OpeningRegion, "Check Black Chip Region: After Openning Mask. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();
            CvImage img_FillUpRegion = new CvImage();

            MagnusOpenCVLib.FillUp(ref img_OpeningRegion, ref img_FillUpRegion);
            PushBackDebugInfors(imgSource, img_FillUpRegion, "Check Black Chip Region: FillUp. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();

            CvImage img_OpeningRegion2 = new CvImage();
            MagnusOpenCVLib.DilationCircle_Magnus(ref img_FillUpRegion, ref img_OpeningRegion2, m_nOpeningBlackChipThreshold2);
            PushBackDebugInfors(imgSource, img_OpeningRegion2, "Check Black Chip Region: After Dilation. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();


            CvImage img_BiggestRegion = new CvImage();
            MagnusOpenCVLib.SelectBiggestRegion(ref img_OpeningRegion2, ref img_BiggestRegion);
            PushBackDebugInfors(imgSource, img_BiggestRegion, "Check Black Chip Region: img_BiggestRegion. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();
            CvPointArray point_regions = new CvPointArray();

            CvInvoke.FindNonZero(img_BiggestRegion, point_regions);
            if (point_regions.Size == 0)
                return -(int)ERROR_CODE.PASS;

            CvContourArray contours2 = new CvContourArray();
            MagnusOpenCVLib.GenContourRegion(ref img_BiggestRegion, ref contours2, RetrType.External);
            RotatedRect rotateRect_Device = new RotatedRect();
            rotateRect_Device = CvInvoke.MinAreaRect(contours2[0]);
            if (rotateRect_Device.Size.Width < (int)(m_OC_MinWidthDevice * m_DeviceLocationParameter.m_L_ScaleImageRatio)
                || rotateRect_Device.Size.Height < (int)(m_OC_MinHeightDevice * m_DeviceLocationParameter.m_L_ScaleImageRatio))
                return -(int)ERROR_CODE.PASS;

            AddRegionOverlay(ref list_arrayOverlay, img_BiggestRegion, Colors.Red);

            CvImage distanceTransform = new CvImage();
            CvInvoke.DistanceTransform(img_BiggestRegion, distanceTransform, null, DistType.L2, 5);
            PushBackDebugInfors(distanceTransform, distanceTransform, "Check Black Chip Region: distanceTransform. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();

            CvImage normalizedDistanceTransform = new CvImage();
            CvInvoke.Normalize(distanceTransform, normalizedDistanceTransform, 0, 255, NormType.MinMax, DepthType.Cv8U);

            PushBackDebugInfors(normalizedDistanceTransform, normalizedDistanceTransform, "Check Black Chip Region: Normalize. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();

            CvImage img_thresholdTransformRegion = new CvImage();
            MagnusOpenCVLib.Threshold2(ref normalizedDistanceTransform, ref img_thresholdTransformRegion, 245, 255);
            PushBackDebugInfors(imgSource, img_thresholdTransformRegion, "Check Black Chip Region: img_thresholdTransformRegion. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();


            CvInvoke.FindNonZero(img_thresholdTransformRegion, point_regions);
            if (point_regions.Size == 0)
                return -(int)ERROR_CODE.PASS;


            List<Rectangle> corner = new List<Rectangle>();
            CvImage result = new CvImage();
            CvContourArray contourArray = new CvContourArray();

            MagnusOpenCVLib.ExtractExternalContours(ref img_thresholdTransformRegion, ref result, ref contourArray);
            PushBackDebugInfors(imgSource, result, "Check Black Chip Region: ConvexHull. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();
            List<PointF> listContourPoints = new List<PointF>();
            for (int n = 0; n < contourArray.Size; n++)
            {
                CvPointArray PointArrayTemp = contourArray[n];
                for (int m = 0; m < PointArrayTemp.Size; m++)
                {
                    listContourPoints.Add(PointArrayTemp[m]);

                }
            }

            PointF pTemp = new PointF(0, 0);
            MagnusOpenCVLib.SelectPointBased_Top_Left_Bottom_Right(ref listContourPoints, ref pTemp, (int)POSITION._TOP);

            CvImage mat_point = CvImage.Zeros(imgSource.Height, imgSource.Width, DepthType.Cv8U, 1);
            System.Drawing.Rectangle rect_center = new System.Drawing.Rectangle((int)(pTemp.X),
                                                                            (int)(pTemp.Y),
                                                                            (int)(4),
                                                                           (int)(4));

            CvInvoke.Rectangle(mat_point, rect_center, new MCvScalar(255), -1);
            PushBackDebugInfors(imgSource, mat_point, "Center Chip Region . (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();
            AddRegionOverlay(ref list_arrayOverlay, mat_point, Colors.Yellow);

            pCenter = new Point((int)(pTemp.X / m_DeviceLocationParameter.m_L_ScaleImageRatio), (int)(pTemp.Y / m_DeviceLocationParameter.m_L_ScaleImageRatio));

            return -(int)ERROR_CODE.OPPOSITE_CHIP;
        }

        public /*static*/ int FindDeviceLocation_Zoom(ref CvImage imgSource, ref List<ArrayOverLay> list_arrayOverlay, ref PointF pCenter, ref PointF pCorner, ObservableCollection<DebugInfors> debugInfors, bool bEnableDebug = false)
        {
            Stopwatch timeIns = new Stopwatch();
            int nErrorTemp = -(int)ERROR_CODE.NO_PATTERN_FOUND;

            //if (m_TemplateImage.Gray == null)
            //    return -99;
            timeIns.Start();

            CvImage zoomedInImage = new CvImage((int)(imgSource.Height * m_DeviceLocationParameter.m_L_ScaleImageRatio), (int)(imgSource.Width * m_DeviceLocationParameter.m_L_ScaleImageRatio), DepthType.Cv8U, 3);
            CvInvoke.Resize(imgSource, zoomedInImage, new System.Drawing.Size(zoomedInImage.Width, zoomedInImage.Height));

            CvImage blurredImage = new CvImage();
            CvInvoke.GaussianBlur(zoomedInImage, blurredImage, new Size(5, 5), 0);

            CvImage zoomedInImage_Scale = new CvImage((int)(imgSource.Height * m_DeviceLocationParameter.m_L_ScaleImageRatio), (int)(imgSource.Width * m_DeviceLocationParameter.m_L_ScaleImageRatio), DepthType.Cv8U, 3);
            CvImage img_thresholdRegion = new CvImage();
            CvImage img_BiggestRegion = new CvImage();
            CvImage img_SelectRegion = new CvImage();
            CvImage img_DilationRegion = new CvImage();
            CvPointArray point_regions = new CvPointArray();

            CvImage region_Crop = new CvImage();
            //mat_DeviceLocationRegion = new CvImage();
            System.Drawing.Rectangle rectDeviceLocation = new System.Drawing.Rectangle((int)(m_DeviceLocationParameter.m_L_DeviceLocationRoi.TopLeft.X * m_DeviceLocationParameter.m_L_ScaleImageRatio),
                                                                                        (int)(m_DeviceLocationParameter.m_L_DeviceLocationRoi.TopLeft.Y * m_DeviceLocationParameter.m_L_ScaleImageRatio),
                                                                                        (int)(m_DeviceLocationParameter.m_L_DeviceLocationRoi.Width * m_DeviceLocationParameter.m_L_ScaleImageRatio),
                                                                                       (int)(m_DeviceLocationParameter.m_L_DeviceLocationRoi.Height * m_DeviceLocationParameter.m_L_ScaleImageRatio));
            CvImage region_SearchDeviceLocation = new CvImage();
            region_SearchDeviceLocation = CvImage.Zeros(zoomedInImage.Height, zoomedInImage.Width, DepthType.Cv8U, 1);
            CvInvoke.Rectangle(region_SearchDeviceLocation, rectDeviceLocation, new MCvScalar(255), -1);

            PushBackDebugInfors(zoomedInImage, region_SearchDeviceLocation, "Region_SearchDeviceLocationt. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();

            if (m_DeviceLocationParameter.m_L_ThresholdType == THRESHOLD_TYPE.BINARY_THRESHOLD)
            {

                MagnusOpenCVLib.Threshold2(ref blurredImage, ref img_thresholdRegion, m_DeviceLocationParameter.m_L_lowerThreshold, m_DeviceLocationParameter.m_L_upperThreshold);
                PushBackDebugInfors(imgSource, img_thresholdRegion, "img_thresholdRegion. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
                timeIns.Restart();

                CvInvoke.BitwiseAnd(img_thresholdRegion, region_SearchDeviceLocation, img_thresholdRegion);
                PushBackDebugInfors(imgSource, img_thresholdRegion, "region after Intersection with Device Location ROI (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
                timeIns.Restart();
            }
            else//if (m_DeviceLocationParameter.m_L_ThresholdType.GetHashCode() == (int)THRESHOLD_TYPE.VAR_THRESHOLD)
            {
                MagnusOpenCVLib.VarThresholding(ref blurredImage, ref img_thresholdRegion, (int)m_DeviceLocationParameter.m_L_ObjectColor, 2 * ((int)(m_DeviceLocationParameter.m_L_MinWidthDevice / 10)) + 3, ref region_SearchDeviceLocation, m_DeviceLocationParameter.m_L_upperThreshold);
                PushBackDebugInfors(imgSource, img_thresholdRegion, "VarThresholding. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
                timeIns.Restart();
            }


            PointF p_CornerPoint_Temp = new PointF();
            PointF p_CenterPoint_Temp = new PointF();
            RotatedRect rotateRect_Device = new RotatedRect();

            bool bIsChipFound = false;
            bool bIsChipNoCornerFound = false;

            if (m_DeviceLocationParameter.m_L_LocationEnable)
            {

                CvImage img_MophoRegion = new CvImage();
                MagnusOpenCVLib.OpeningCircle(ref img_thresholdRegion, ref img_MophoRegion, 2);
                PushBackDebugInfors(imgSource, img_MophoRegion, "Outer region after Opening. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
                timeIns.Restart();

                CvImage img_MophoRegionEnd = new CvImage();

                MagnusOpenCVLib.ClosingCircle_Magnus(ref img_MophoRegion, ref img_MophoRegionEnd, m_DeviceLocationParameter.m_L_DilationMask);
                PushBackDebugInfors(imgSource, img_MophoRegionEnd, "Outer region after ClosingCircle. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
                timeIns.Restart();

                List<Rectangle> rect_SelectRectangle = new List<Rectangle>();
                CvImage mat_selectedtRegions = new CvImage();

                MagnusOpenCVLib.SelectRegion(ref img_MophoRegionEnd, ref mat_selectedtRegions, ref rect_SelectRectangle, (int)(m_DeviceLocationParameter.m_L_MinWidthDevice * m_DeviceLocationParameter.m_L_ScaleImageRatio), (int)(m_DeviceLocationParameter.m_L_MinHeightDevice * m_DeviceLocationParameter.m_L_ScaleImageRatio));
                PushBackDebugInfors(imgSource, mat_selectedtRegions, "Outer region after SelectRegion with Width and Height. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
                timeIns.Restart();

                CvInvoke.FindNonZero(mat_selectedtRegions, point_regions);
                if (point_regions.Size == 0)
                    return -(int)ERROR_CODE.NO_PATTERN_FOUND;

                CvImage mat_FillUpBlackRegion = new CvImage();
                MagnusOpenCVLib.FillUp(ref mat_selectedtRegions, ref mat_FillUpBlackRegion);
                PushBackDebugInfors(imgSource, mat_FillUpBlackRegion, "Outer region after FillUp Select Region. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
                timeIns.Restart();

                // Add offset Outer = 3
                int nInnerOffset = 5;
                CvImage mat_OffsetOuterRegion = new CvImage();
                MagnusOpenCVLib.ErosionCircle(ref mat_FillUpBlackRegion, ref mat_OffsetOuterRegion, nInnerOffset);
                PushBackDebugInfors(imgSource, mat_OffsetOuterRegion, "Outer region after offset. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
                timeIns.Restart();


                //////////////////////////////////////////////////
                ////  segment single chip region
                CvImage img_thresholdInnerRegion = new CvImage();
                MagnusOpenCVLib.Threshold2(ref zoomedInImage, ref img_thresholdInnerRegion, m_DeviceLocationParameter.m_L_lowerThresholdInnerChip, m_DeviceLocationParameter.m_L_upperThresholdInnerChip);
                PushBackDebugInfors(imgSource, img_thresholdInnerRegion, "Inner threshold Region. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
                timeIns.Restart();
                CvImage mat_InnerChipRegion = new CvImage();
                CvInvoke.BitwiseAnd(img_thresholdInnerRegion, mat_OffsetOuterRegion, mat_InnerChipRegion);
                PushBackDebugInfors(imgSource, mat_InnerChipRegion, "Inner threshold region after Intersection with Black FillUp Region (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
                timeIns.Restart();

                CvImage mat_InnerChipFillUpRegion = new CvImage();
                MagnusOpenCVLib.FillUp(ref mat_InnerChipRegion, ref mat_InnerChipFillUpRegion);
                PushBackDebugInfors(imgSource, mat_InnerChipFillUpRegion, "Inner Chip Region after FillUp. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
                timeIns.Restart();

                CvImage mat_InnerChipOpeningRegion = new CvImage();
                MagnusOpenCVLib.OpeningCircle(ref mat_InnerChipFillUpRegion, ref mat_InnerChipOpeningRegion, m_DeviceLocationParameter.m_L_OpeningMask);
                PushBackDebugInfors(imgSource, mat_InnerChipOpeningRegion, "Inner Chip Region after Opening. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
                timeIns.Restart();

                CvImage mat_BiggestInnerChipRegion;

                List<PointF> listCornerPoints = new List<PointF>();
                List<PointF> listCenterPoints = new List<PointF>();

                List<PointF> listCornerPoints_Fail = new List<PointF>();
                List<PointF> listCenterPoints_Fail = new List<PointF>();



                PointF cornerPointTemp = new PointF();
                while (true)
                {
                    timeIns.Restart();
                    CvImage mat_BiggestInnerChipRegion_temp = new CvImage();
                    MagnusOpenCVLib.SelectBiggestRegion(ref mat_InnerChipOpeningRegion, ref mat_BiggestInnerChipRegion_temp);
                    PushBackDebugInfors(imgSource, mat_BiggestInnerChipRegion_temp, "Inner Chip Region after select BiggestRegion. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);


                    // Find none zero first, if zero => return
                    CvInvoke.FindNonZero(mat_BiggestInnerChipRegion_temp, point_regions);
                    if (point_regions.Size == 0)
                        break;
                    /////////////
                    CvContourArray contours_temp = new CvContourArray();
                    MagnusOpenCVLib.GenContourRegion(ref mat_BiggestInnerChipRegion_temp, ref contours_temp, RetrType.External);
                    rotateRect_Device = CvInvoke.MinAreaRect(contours_temp[0]);

                    // Check Size of Region, if small => return because biggest region small => all smaller will small :V
                    if (rotateRect_Device.Size.Width < (int)(m_DeviceLocationParameter.m_L_MinWidthDevice * m_DeviceLocationParameter.m_L_ScaleImageRatio)
                        || rotateRect_Device.Size.Height < (int)(m_DeviceLocationParameter.m_L_MinHeightDevice * m_DeviceLocationParameter.m_L_ScaleImageRatio))
                        break;


                    // Openning to remove the noise on conveyor
                    timeIns.Restart();
                    mat_BiggestInnerChipRegion = new CvImage();
                    MagnusOpenCVLib.OpenningCircle_Magnus(ref mat_BiggestInnerChipRegion_temp, ref mat_BiggestInnerChipRegion, (int)(m_DeviceLocationParameter.m_L_MinWidthDevice * m_DeviceLocationParameter.m_L_ScaleImageRatio / 4));
                    PushBackDebugInfors(imgSource, mat_BiggestInnerChipRegion, "biggest Chip Region after opening. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
                    timeIns.Restart();


                    CvInvoke.BitwiseXor(mat_InnerChipOpeningRegion, mat_BiggestInnerChipRegion_temp, mat_InnerChipOpeningRegion);
                    PushBackDebugInfors(imgSource, mat_InnerChipOpeningRegion, "Inner Chip Region after Remove Biggest region. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
                    timeIns.Restart();


                    CvInvoke.FindNonZero(mat_BiggestInnerChipRegion, point_regions);
                    if (point_regions.Size == 0)
                        continue;

                    // Get smallest 
                    CvContourArray contours = new CvContourArray();
                    MagnusOpenCVLib.GenContourRegion(ref mat_BiggestInnerChipRegion, ref contours, RetrType.External);
                    rotateRect_Device = CvInvoke.MinAreaRect(contours[0]);



                    if (Math.Abs(rotateRect_Device.Size.Width / rotateRect_Device.Size.Height - m_DeviceLocationParameter.m_L_TemplateRoi.Width / m_DeviceLocationParameter.m_L_TemplateRoi.Height) > 0.2)
                        continue;

                    int nError = FindNearestPoints_Debug(zoomedInImage, rotateRect_Device, ref cornerPointTemp, ref list_arrayOverlay,debugInfors, bEnableDebug);
                    Point pCenterTemp = new Point((int)rotateRect_Device.Center.X, (int)rotateRect_Device.Center.Y);
                    if (nError < 0)
                    {
                        bIsChipNoCornerFound = true;
                        listCenterPoints_Fail.Add(pCenterTemp);
                        listCornerPoints_Fail.Add(cornerPointTemp);
                        continue;
                    }
                    else
                    {
                        bIsChipFound = true;
                        listCenterPoints.Add(pCenterTemp);
                        listCornerPoints.Add(cornerPointTemp);
                    }

                }

                //if (bIsChipFound | bIsChipNoCornerFound == false)
                //    return -(int)ERROR_CODE.NO_PATTERN_FOUND;

                if (bIsChipFound)
                {
                    int nIndexOut = MagnusOpenCVLib.SelectPointBased_Top_Left_Bottom_Right(ref listCenterPoints, ref pCenter, (int)POSITION._BOTTOM);
                    p_CenterPoint_Temp = pCenter;
                    p_CornerPoint_Temp = listCornerPoints[nIndexOut];
                }
                else if (bIsChipNoCornerFound)
                {
                    int nIndexOut = MagnusOpenCVLib.SelectPointBased_Top_Left_Bottom_Right(ref listCenterPoints_Fail, ref pCenter, (int)POSITION._BOTTOM);
                    p_CenterPoint_Temp = pCenter;
                    p_CornerPoint_Temp = listCornerPoints_Fail[nIndexOut];
                }

            }
            else
            {
                CvImage img_MophoRegion = new CvImage();
                MagnusOpenCVLib.OpeningCircle(ref img_thresholdRegion, ref img_MophoRegion, m_DeviceLocationParameter.m_L_OpeningMask);
                PushBackDebugInfors(imgSource, img_MophoRegion, "Outer region after Opening. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
                timeIns.Restart();


                CvImage img_MophoRegion2 = new CvImage();
                MagnusOpenCVLib.ClosingCircle(ref img_MophoRegion, ref img_MophoRegion2, m_DeviceLocationParameter.m_L_DilationMask);
                PushBackDebugInfors(imgSource, img_MophoRegion2, "Outer region after ClosingCircle. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
                timeIns.Restart();

                List<Rectangle> rect_SelectRectangle = new List<Rectangle>();
                CvImage mat_BiggestInnerChipRegion = new CvImage();

                MagnusOpenCVLib.SelectBiggestRegion(ref img_MophoRegion2, ref mat_BiggestInnerChipRegion);
                PushBackDebugInfors(imgSource, mat_BiggestInnerChipRegion, "Inner Chip Region after select BiggestRegion. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
                timeIns.Restart();

                CvInvoke.FindNonZero(mat_BiggestInnerChipRegion, point_regions);
                if (point_regions.Size == 0)
                    return -(int)ERROR_CODE.NO_PATTERN_FOUND;
                /////////////
                CvContourArray contours = new CvContourArray();
                MagnusOpenCVLib.GenContourRegion(ref mat_BiggestInnerChipRegion, ref contours, RetrType.External);
                rotateRect_Device = CvInvoke.MinAreaRect(contours[0]);

                if (rotateRect_Device.Size.Width < (int)(m_DeviceLocationParameter.m_L_MinWidthDevice * m_DeviceLocationParameter.m_L_ScaleImageRatio)
                    || rotateRect_Device.Size.Height < (int)(m_DeviceLocationParameter.m_L_MinHeightDevice * m_DeviceLocationParameter.m_L_ScaleImageRatio))
                    return -(int)ERROR_CODE.NO_PATTERN_FOUND;


                PointF[] points = rotateRect_Device.GetVertices();

                int nLeft = (int)rotateRect_Device.Center.X;
                int nTop = (int)rotateRect_Device.Center.Y;
                for (int n = 0; n < points.Length; n++)
                {
                    if (points[n].X < nLeft && points[n].Y < nTop)
                    {
                        p_CornerPoint_Temp = points[n];
                        break;
                    }
                }
                //p_CornerPoint_Temp = FindNearestPoints_Debug(zoomedInImage, rotateRect_Device, ref list_arrayOverlay,debugInfors, bEnableDebug);
                //if (p_CornerPoint_Temp.X + p_CornerPoint_Temp.Y == 0)
                //    return -(int)ERROR_CODE.LABEL_FAIL;

                p_CenterPoint_Temp = new Point((int)rotateRect_Device.Center.X, (int)rotateRect_Device.Center.Y);
                bIsChipFound = true;
            }

            //////////////////////
            if (bIsChipFound)
            {
                nErrorTemp = -(int)ERROR_CODE.PASS;
                int[] nResult = new int[m_DeviceLocationParameter.m_DR_NumberROILocation];
                for (int nPVIAreaIndex = 0; nPVIAreaIndex < m_DeviceLocationParameter.m_DR_NumberROILocation; nPVIAreaIndex++)
                {
                    nResult[nPVIAreaIndex] = 0;
                    LabelMarking_Inspection(zoomedInImage, nPVIAreaIndex, p_CenterPoint_Temp, p_CornerPoint_Temp, ref nResult[nPVIAreaIndex], ref list_arrayOverlay,debugInfors, bEnableDebug);
                    if (nResult[nPVIAreaIndex] < 0)
                    {
                        nErrorTemp = nResult[nPVIAreaIndex];
                        break;
                    }
                }

                pCenter.X = (int)(p_CenterPoint_Temp.X / m_DeviceLocationParameter.m_L_ScaleImageRatio);
                pCenter.Y = (int)(p_CenterPoint_Temp.Y / m_DeviceLocationParameter.m_L_ScaleImageRatio);
                pCorner.X = (int)(p_CornerPoint_Temp.X / m_DeviceLocationParameter.m_L_ScaleImageRatio);
                pCorner.Y = (int)(p_CornerPoint_Temp.Y / m_DeviceLocationParameter.m_L_ScaleImageRatio);
            }
            else if (bIsChipNoCornerFound)
            {
                nErrorTemp = -(int)ERROR_CODE.LABEL_FAIL;
                pCenter.X = (int)(p_CenterPoint_Temp.X / m_DeviceLocationParameter.m_L_ScaleImageRatio);
                pCenter.Y = (int)(p_CenterPoint_Temp.Y / m_DeviceLocationParameter.m_L_ScaleImageRatio);
                pCorner.X = (int)(p_CornerPoint_Temp.X / m_DeviceLocationParameter.m_L_ScaleImageRatio);
                pCorner.Y = (int)(p_CornerPoint_Temp.Y / m_DeviceLocationParameter.m_L_ScaleImageRatio);
            }

            if ((nErrorTemp != -(int)ERROR_CODE.PASS) && m_blackChipParameter.m_OC_EnableCheck)
            {
                // Black Chip
                PointF pCenterBlackChip = new PointF(0, 0);
                int nErrorBlackChip = -(int)ERROR_CODE.PASS;
                nErrorBlackChip = FindBlackChip(ref zoomedInImage, ref region_SearchDeviceLocation, ref pCenterBlackChip, ref list_arrayOverlay,debugInfors, bEnableDebug);

                //CvInvoke.WaitKey(0);


                if (nErrorBlackChip == -(int)ERROR_CODE.PASS && nErrorTemp == -(int)ERROR_CODE.NO_PATTERN_FOUND)
                    return -(int)ERROR_CODE.NO_PATTERN_FOUND;


                //Check whether need to pick the black chip or the normal chip
                if (nErrorBlackChip != -(int)ERROR_CODE.PASS && nErrorTemp != -(int)ERROR_CODE.NO_PATTERN_FOUND)
                {
                    if (MagnusOpenCVLib.Check2PointIndex(ref pCenter, ref pCenterBlackChip, (int)POSITION._BOTTOM))
                    {
                        pCenter = pCenterBlackChip;
                        nErrorTemp = -(int)ERROR_CODE.OPPOSITE_CHIP;
                    }
                }
                else if (nErrorBlackChip == -(int)ERROR_CODE.OPPOSITE_CHIP)
                {
                    pCenter = pCenterBlackChip;
                    nErrorTemp = -(int)ERROR_CODE.OPPOSITE_CHIP;

                }
            }

            timeIns.Restart();

            CvImage mat_point = new CvImage();
            mat_point = CvImage.Zeros(zoomedInImage.Height, zoomedInImage.Width, DepthType.Cv8U, 1);
            //System.Drawing.Rectangle rect_center = new System.Drawing.Rectangle((int)(pCenter.X * m_DeviceLocationParameter.m_L_ScaleImageRatio),
            //                                                    (int)(pCenter.Y * m_DeviceLocationParameter.m_L_ScaleImageRatio),
            //                                                    (int)(6),
            //                                                   (int)(6));

            Point pCenterOverLay = new Point((int)(pCenter.X * m_DeviceLocationParameter.m_L_ScaleImageRatio), (int)(pCenter.Y * m_DeviceLocationParameter.m_L_ScaleImageRatio));

            CvInvoke.Circle(mat_point, pCenterOverLay, 11, new MCvScalar(255), 3);
            PushBackDebugInfors(imgSource, mat_point, "Center Chip Region . (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();
            AddRegionOverlay(ref list_arrayOverlay, mat_point, Colors.Blue);

            return nErrorTemp;
        }

        public int FindNearestPoints_Debug(CvImage imgSourceInput, RotatedRect rotateRect_Device, ref PointF pCornerOut, ref List<ArrayOverLay> list_arrayOverlay, ObservableCollection<DebugInfors> debugInfors, bool bEnableDebug)
        {
            Stopwatch timeIns = new Stopwatch();
            CvPointArray regionPoints = new CvPointArray();

            PointF[] points = rotateRect_Device.GetVertices();
            pCornerOut = points[0];

            //RotatedRect rotateRect = new RotatedRect(polygonInput[polygonInput.Count() - 1], new SizeF(rectMatchingPosition.Width - 30, rectMatchingPosition.Height - 30), -fAngleInput);
            CvImage rec_region2 = new CvImage();
            rec_region2 = CvImage.Zeros(imgSourceInput.Height, imgSourceInput.Width, DepthType.Cv8U, 1);
            MagnusOpenCVLib.GenRectangle2(rec_region2, rotateRect_Device, new MCvScalar(255), 1);


            PushBackDebugInfors(imgSourceInput, rec_region2, "Inner region after GenRectangle2. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            AddRegionOverlay(ref list_arrayOverlay, rec_region2, Colors.Yellow);

            CvImage rec_regionChipFillup = new CvImage();
            MagnusOpenCVLib.FillUp(ref rec_region2, ref rec_regionChipFillup);
            PushBackDebugInfors(imgSourceInput, rec_regionChipFillup, "Inner region after FillUp. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);

            ////////////////////////////////
            // Begin Attract corner of chip///

            CvImage fillup_Region = new CvImage();
            CvImage mat_Thresholdregion = new CvImage();
            timeIns.Restart();
            MagnusOpenCVLib.Threshold2(ref imgSourceInput, ref mat_Thresholdregion, m_DeviceLocationParameter.m_L_lowerThresholdInnerChip, m_DeviceLocationParameter.m_L_upperThresholdInnerChip);
            CvInvoke.BitwiseAnd(mat_Thresholdregion, rec_regionChipFillup, mat_Thresholdregion);
            PushBackDebugInfors(imgSourceInput, mat_Thresholdregion, "Inner rect Threshold region . (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();

            CvImage mat_ThresholdFillupRegion = new CvImage();
            MagnusOpenCVLib.FillUp(ref mat_Thresholdregion, ref mat_ThresholdFillupRegion);
            PushBackDebugInfors(imgSourceInput, mat_ThresholdFillupRegion, "Inner Threshold Region after FillUp. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();

            CvImage mat_openingRegionTemp = new CvImage();
            MagnusOpenCVLib.OpeningCircle(ref mat_ThresholdFillupRegion, ref mat_openingRegionTemp, (int)(m_DeviceLocationParameter.m_L_OpeningMask * m_DeviceLocationParameter.m_L_ScaleImageRatio) + 1);
            PushBackDebugInfors(imgSourceInput, mat_openingRegionTemp, "Inner Region after OpeningCircle. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();

            CvImage mat_BiggestRegion = new CvImage();
            MagnusOpenCVLib.SelectBiggestRegion(ref mat_openingRegionTemp, ref mat_BiggestRegion);
            PushBackDebugInfors(imgSourceInput, mat_BiggestRegion, "Inner Region after SelectBiggestRegion. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();

            CvInvoke.FindNonZero(mat_BiggestRegion, regionPoints);
            if (regionPoints.Size == 0)
                return -1;

            AddRegionOverlay(ref list_arrayOverlay, mat_BiggestRegion, Colors.Cyan);



            //Detect Special Chip corner
            CvImage mat_CornerChipRegion = new CvImage();
            CvInvoke.BitwiseXor(rec_regionChipFillup, mat_BiggestRegion, mat_CornerChipRegion);
            PushBackDebugInfors(imgSourceInput, mat_CornerChipRegion, "Corner Chip Region after sub inner region and inner rect. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();

            // Need add Offset Inner
            int nInnerOffset = -3;
            CvImage rec_regionChipFillup_Offset = new CvImage();
            if (nInnerOffset > 0)
                MagnusOpenCVLib.ErodeRectangle(ref rec_regionChipFillup, ref rec_regionChipFillup_Offset, Math.Abs(nInnerOffset), Math.Abs(nInnerOffset));
            else
                MagnusOpenCVLib.DilationRectangle(ref rec_regionChipFillup, ref rec_regionChipFillup_Offset, Math.Abs(nInnerOffset), Math.Abs(nInnerOffset));

            PushBackDebugInfors(imgSourceInput, rec_regionChipFillup_Offset, "Rectangle Chip after offset 5 pixel. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();
            CvInvoke.BitwiseAnd(rec_regionChipFillup_Offset, mat_CornerChipRegion, mat_CornerChipRegion);
            PushBackDebugInfors(imgSourceInput, mat_CornerChipRegion, "Corner Chip Region after intersection with offset rectangle. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();

            CvImage mat_openingCornerRegion = new CvImage();
            MagnusOpenCVLib.OpeningCircle(ref mat_CornerChipRegion, ref mat_openingCornerRegion, (int)(m_DeviceLocationParameter.m_L_OpeningMask * m_DeviceLocationParameter.m_L_ScaleImageRatio / 2 + 1));
            PushBackDebugInfors(imgSourceInput, mat_openingCornerRegion, "Corner Chip Region after OpeningCircle. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();

            CvImage mat_CornerBiggestRegion = new CvImage();
            MagnusOpenCVLib.SelectBiggestRegion(ref mat_openingCornerRegion, ref mat_CornerBiggestRegion);
            PushBackDebugInfors(imgSourceInput, mat_CornerBiggestRegion, "Corner Chip Region after SelectBiggestRegion. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();



            CvInvoke.FindNonZero(mat_CornerBiggestRegion, regionPoints);
            if (regionPoints.Size == 0)
                return -1;
            AddRegionOverlay(ref list_arrayOverlay, mat_CornerBiggestRegion, Colors.Blue);

            System.Drawing.Rectangle rect_temp = CvInvoke.BoundingRectangle(regionPoints);
            Point Center_Point = new Point(rect_temp.Left + rect_temp.Width / 2, rect_temp.Top + rect_temp.Height / 2);
            double minDistance = 9999999;
            int nminIndex = -1;
            for (int n = 0; n < rotateRect_Device.GetVertices().Count(); n++)
            {
                double distance_Square = (points[n].X - Center_Point.X) * (points[n].X - Center_Point.X) + (points[n].Y - Center_Point.Y) * (points[n].Y - Center_Point.Y);
                if (distance_Square < minDistance)
                {
                    nminIndex = n;
                    minDistance = distance_Square;
                }
            }
            PushBackDebugInfors(imgSourceInput, mat_CornerBiggestRegion, $"Corner Chip {pCornerOut.X}, {pCornerOut.Y} Region after SelectBiggestRegion. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();

            pCornerOut = points[nminIndex];
            return 0;
        }


        public void LabelMarking_Inspection(CvImage imgSourceInput, int nPVIAreaIndex, PointF pCenter, PointF pCorner, ref int nResultOutput, ref List<ArrayOverLay> list_arrayOverlay, ObservableCollection<DebugInfors> debugInfors, bool bEnableDebug)
        {
            if (m_SurfaceDefectParameter[nPVIAreaIndex].m_DR_AreaEnable == false)
            {
                nResultOutput = -(int)ERROR_CODE.PASS;
                return;
            }

            nResultOutput = -(int)ERROR_CODE.LABEL_FAIL;
            double dScale = m_DeviceLocationParameter.m_L_ScaleImageRatio;

            System.Drawing.Rectangle rectPVIArea = new System.Drawing.Rectangle((int)(m_SurfaceDefectParameter[nPVIAreaIndex].m_DR_DefectROILocations.TopLeft.X * dScale),
                                                                                (int)(m_SurfaceDefectParameter[nPVIAreaIndex].m_DR_DefectROILocations.TopLeft.Y * dScale),
                                                                                (int)(m_SurfaceDefectParameter[nPVIAreaIndex].m_DR_DefectROILocations.Width * dScale),
                                                                                (int)(m_SurfaceDefectParameter[nPVIAreaIndex].m_DR_DefectROILocations.Height * dScale));
            CvImage region_PVIArea = new CvImage();
            region_PVIArea = CvImage.Zeros(imgSourceInput.Height, imgSourceInput.Width, DepthType.Cv8U, 1);
            CvInvoke.Rectangle(region_PVIArea, rectPVIArea, new MCvScalar(255), -1);

            Stopwatch timeIns = new Stopwatch();

            PointF pCenterTeach = new PointF((float)(m_DeviceLocationResult.m_dCenterDevicePoint.X * dScale), (float)(m_DeviceLocationResult.m_dCenterDevicePoint.Y * dScale));
            PointF pCornerTeach = new PointF((float)(m_DeviceLocationResult.m_dCornerDevicePoint.X * dScale), (float)(m_DeviceLocationResult.m_dCornerDevicePoint.Y * dScale));

            double dDeltaAngle = Track.MagnusMatrix.CalculateShiftXYAngle(pCenter, pCorner, pCenterTeach, pCornerTeach);

            PushBackDebugInfors(imgSourceInput, imgSourceInput, $"pCorner {pCorner.X}, {pCorner.Y}, pCornerTeach {pCornerTeach.X}, {pCornerTeach.Y}", bEnableDebug,debugInfors);
            PushBackDebugInfors(imgSourceInput, imgSourceInput, $"pCenter {pCenter.X}, {pCenter.Y}, pCenterTeach {pCenterTeach.X},  {pCenterTeach.Y}", bEnableDebug,debugInfors);

            double fShiftX = pCenter.X - m_DeviceLocationResult.m_dCenterDevicePoint.X * dScale;
            double fShiftY = pCenter.Y - m_DeviceLocationResult.m_dCenterDevicePoint.Y * dScale;

            CvImage shiftImage = MagnusOpenCVLib.RotateShiftImage(ref imgSourceInput, pCenter, (float)dDeltaAngle, -(float)fShiftX, -(float)fShiftY);
            PushBackDebugInfors(shiftImage, region_PVIArea, $"Area {nPVIAreaIndex + 1} shift Image. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();

            //dDeltaAngle = Track.MagnusMatrix.CalculateShiftXYAngle(pCenterTeach, pCornerTeach, pCenter, pCorner);
            //fShiftX = pCenter.X - m_DeviceLocationResult.m_dCenterDevicePoint.X * dScale;
            //fShiftY = pCenter.Y - m_DeviceLocationResult.m_dCenterDevicePoint.Y * dScale;

            PointF pCenterTemp = new PointF(rectPVIArea.X + rectPVIArea.Width / 2, rectPVIArea.Y + rectPVIArea.Height / 2);
            CvImage shiftRegion = MagnusOpenCVLib.RotateShiftImage(ref region_PVIArea, pCenterTeach, -(float)dDeltaAngle, (float)fShiftX, (float)fShiftY);
            PushBackDebugInfors(imgSourceInput, shiftRegion, $"Area {nPVIAreaIndex + 1} shift Image. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();

            //CvImage AbsDiffImage = new CvImage();
            //CvInvoke.AbsDiff(m_TeachImage.Gray, shiftImage, AbsDiffImage);
            //PushBackDebugInfors(AbsDiffImage, region_PVIArea, $"Area {nPVIAreaIndex + 1} Subtract teach and inspection image. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            //timeIns.Restart();

            // Image Processing
            CvImage img_thresholdRegion = new CvImage();
            MagnusOpenCVLib.Threshold2(ref imgSourceInput, ref img_thresholdRegion, m_SurfaceDefectParameter[nPVIAreaIndex].m_LD_lowerThreshold, m_SurfaceDefectParameter[nPVIAreaIndex].m_LD_upperThreshold);
            PushBackDebugInfors(imgSourceInput, img_thresholdRegion, $"Area {nPVIAreaIndex + 1} img_thresholdRegion. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();

            //CvImage mat_IntersectionRegion = new CvImage();
            CvInvoke.BitwiseAnd(img_thresholdRegion, shiftRegion, img_thresholdRegion);
            PushBackDebugInfors(imgSourceInput, img_thresholdRegion, $"Area {nPVIAreaIndex + 1} region after Intersection with teach region (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();


            CvImage img_MophoRegion = new CvImage();
            MagnusOpenCVLib.ClosingCircle(ref img_thresholdRegion, ref img_MophoRegion, m_SurfaceDefectParameter[nPVIAreaIndex].m_LD_DilationMask);
            PushBackDebugInfors(imgSourceInput, img_MophoRegion, $"Area {nPVIAreaIndex + 1}  region after ClosingCircle. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();

            CvImage img_MophoRegion2 = new CvImage();
            MagnusOpenCVLib.OpeningCircle(ref img_MophoRegion, ref img_MophoRegion2, m_SurfaceDefectParameter[nPVIAreaIndex].m_LD_OpeningMask);
            List<int> Rows = new List<int>();
            List<int> Cols = new List<int>();
            int nArea = MagnusOpenCVLib.GetArea(ref img_MophoRegion2);
            //MagnusOpenCVLib.GetRegionPoints(ref img_MophoRegion2, Rows, Cols);
            PushBackDebugInfors(imgSourceInput, img_MophoRegion2, $"Area {nPVIAreaIndex + 1}  region after Opening. (" + timeIns.ElapsedMilliseconds.ToString() + " ms)", bEnableDebug,debugInfors);
            timeIns.Restart();






            timeIns.Stop();
            int nBoxSize = (int)(m_SurfaceDefectParameter[nPVIAreaIndex].m_DR_DefectROILocations.Width * dScale * m_SurfaceDefectParameter[nPVIAreaIndex].m_DR_DefectROILocations.Height * dScale);
            if (nArea < nBoxSize * m_SurfaceDefectParameter[nPVIAreaIndex].m_LD_ObjectCoverPercent / 100.0)
            {
                nResultOutput = -(int)ERROR_CODE.LABEL_FAIL;
                AddRegionOverlay(ref list_arrayOverlay, shiftRegion, Colors.Red);

            }
            else
            {
                nResultOutput = -(int)ERROR_CODE.PASS;
                AddRegionOverlay(ref list_arrayOverlay, shiftRegion, Colors.Green);
            }


        }
        public int Calibration_Get3Points(CvImage imgSourceInput, out PointF[] points, ref List<ArrayOverLay> list_arrayOverlay)
        {
            points = new PointF[4];
            CvImage thresholdImage = new CvImage();
            MagnusOpenCVLib.Threshold2(ref imgSourceInput, ref thresholdImage, m_DeviceLocationParameter.m_L_lowerThreshold, m_DeviceLocationParameter.m_L_upperThreshold);

            CvImage OpeningImage = new CvImage();

            MagnusOpenCVLib.OpeningCircle(ref thresholdImage, ref OpeningImage, 1);
            CvImage BiggestRegion = new CvImage();

            MagnusOpenCVLib.SelectBiggestRegion(ref OpeningImage, ref BiggestRegion);
            CvPointArray point_regions = new CvPointArray();

            CvInvoke.FindNonZero(BiggestRegion, point_regions);
            if (point_regions.Size == 0)
                return -1;

            CvContourArray contours = new CvContourArray();
            MagnusOpenCVLib.GenContourRegion(ref BiggestRegion, ref contours, RetrType.External);
            RotatedRect rotateRect_Device = CvInvoke.MinAreaRect(contours[0]);
            //if (rotateRect_Device.Size.Width < (int)(m_DeviceLocationParameter.m_nMinWidthDevice)
            //    || rotateRect_Device.Size.Height < (int)(m_DeviceLocationParameter.m_nMinHeightDevice))
            //    return -(int)ERROR_CODE.NO_PATTERN_FOUND;

            points = rotateRect_Device.GetVertices();



            CvImage rec_region2 = new CvImage();
            rec_region2 = CvImage.Zeros(imgSourceInput.Height, imgSourceInput.Width, DepthType.Cv8U, 1);

            MagnusOpenCVLib.GenRectangle2(rec_region2, rotateRect_Device, new MCvScalar(255), 1);
            CvImage rec_regionFillup = new CvImage();

            MagnusOpenCVLib.FillUp(ref rec_region2, ref rec_regionFillup);

            AddRegionOverlay(ref list_arrayOverlay, rec_regionFillup, Colors.YellowGreen);
            return 0;
        }
        //public static int FindNearestPoints(CvImage imgSourceInput, ref CvImage deviceLocationThresholdRegion, System.Drawing.Rectangle rectMatchingPosition, List<Point> polygonInput, float fAngleInput)
        //{

        //    RotatedRect rotateRect = new RotatedRect(polygonInput[polygonInput.Count() - 1], new SizeF(rectMatchingPosition.Width - 30, rectMatchingPosition.Height - 30), -fAngleInput);
        //    CvImage rec_region2 = new CvImage();
        //    rec_region2 = CvImage.Zeros(imgSourceInput.Height, imgSourceInput.Width, DepthType.Cv8U, 1);
        //    MagnusOpenCVLib.GenRectangle2(rec_region2, rotateRect, new MCvScalar(255), 1);

        //    CvImage rec_regionFillup = new CvImage();
        //    MagnusOpenCVLib.FillUp(ref rec_region2, ref rec_regionFillup);

        //    CvImage fillup_Region = new CvImage();
        //    MagnusOpenCVLib.FillUp(ref deviceLocationThresholdRegion, ref fillup_Region);

        //    CvImage And_Region = new CvImage();

        //    //MagnusOpenCVLib.Different(rec_regionFillup, fillup_Region, ref Difference_Region);
        //    CvImage XOR_Region = new CvImage();
        //    CvInvoke.BitwiseXor(rec_regionFillup, fillup_Region, XOR_Region);
        //    CvInvoke.BitwiseAnd(XOR_Region, rec_regionFillup, And_Region);

        //    //CvInvoke.BitwiseXor(rec_regionFillup, fillup_Region, XOR_Region);
        //    //CvInvoke.BitwiseAnd(XOR_Region, rec_regionFillup, Difference_Region);



        //    CvImage opening_Region2 = new CvImage();
        //    MagnusOpenCVLib.OpeningCircle(ref And_Region, ref opening_Region2, 5);

        //    MagnusOpenCVLib.SelectBiggestRegion(ref opening_Region2, ref deviceLocationThresholdRegion);
        //    CvPointArray regionPoints = new CvPointArray();
        //    CvInvoke.FindNonZero(deviceLocationThresholdRegion, regionPoints);
        //    System.Drawing.Rectangle rect_temp = CvInvoke.BoundingRectangle(regionPoints);
        //    Point Center_Point = new Point(rect_temp.Left + rect_temp.Width / 2, rect_temp.Top + rect_temp.Height / 2);
        //    double minDistance = 9999999;
        //    int nminIndex = -1;
        //    for (int n = 0; n < polygonInput.Count() - 1; n++)
        //    {
        //        double distance_Square = (polygonInput[n].X - Center_Point.X) * (polygonInput[n].X - Center_Point.X) + (polygonInput[n].Y - Center_Point.Y) * (polygonInput[n].Y - Center_Point.Y);
        //        if (distance_Square < minDistance)
        //        {
        //            nminIndex = n;
        //            minDistance = distance_Square;
        //        }
        //    }

        //    return nminIndex;
        //}


        public int AutoTeachDatumLocation(ref List<Point> p_Regionpolygon, Rectangles rectDeviceLocationInput, Rectangles rectTemplateInput, ref Mat mat_DeviceLocationRegion, ref System.Drawing.Rectangle rectMatchingPosition, ref double nAngleOutput, ref double dScoreOutput, ref int nCornerIndex)
        {

            //if (m_TeachImage.Gray == null)
            //    return -99;

            //CvImage img_thresholdRegion = new CvImage();
            //CvImage img_openingRegionRegion = new CvImage();
            //CvImage img_BiggestRegion = new CvImage();
            //CvImage img_SelectRegion = new CvImage();
            //CvImage img_DilationRegion = new CvImage();

            //CvImage img_Crop = new CvImage();
            //CvImage region_Crop = new CvImage();
            //mat_DeviceLocationRegion = new CvImage();


            //Image<Gray, Byte> imgCropped = new Image<Gray, byte>(m_TeachImage.Gray.Bitmap);
            //System.Drawing.Rectangle rec = new System.Drawing.Rectangle();
            //CvImage templateImageCropped = new CvImage();
            //rec.X = (int)rectTemplateInput.TopLeft.X;
            //rec.Y = (int)rectTemplateInput.TopLeft.Y;
            //rec.Width = (int)rectTemplateInput.Width;
            //rec.Height = (int)rectTemplateInput.Height;
            //imgCropped.ROI = rec;
            //templateImageCropped = imgCropped.Mat.Clone();
            ////CvInvoke.Imshow("Template Image", templateImageCropped);
            ////CvInvoke.WaitKey();


            //System.Drawing.Rectangle rectDeviceLocation = new System.Drawing.Rectangle((int)rectDeviceLocationInput.TopLeft.X,
            //                                                                            (int)rectDeviceLocationInput.TopLeft.Y,
            //                                                                            (int)rectDeviceLocationInput.Width,
            //                                                                           (int)rectDeviceLocationInput.Height);
            ////Image<Gray, Byte> Image_Source_Crop_Temp = new Image<Gray, Byte>(imgSource.Bitmap);
            //CvImage region_SearchDeviceLocation = new CvImage();
            //region_SearchDeviceLocation = CvImage.Zeros(m_TeachImage.Gray.Height, m_TeachImage.Gray.Width, DepthType.Cv8U, 1);
            //CvInvoke.Rectangle(region_SearchDeviceLocation, rectDeviceLocation, new MCvScalar(255), -1);
            //MagnusOpenCVLib.Threshold2(ref m_TeachImage.Gray, ref img_thresholdRegion, m_DeviceLocationParameter.m_L_lowerThreshold, m_DeviceLocationParameter.m_L_upperThreshold);
            //CvInvoke.BitwiseAnd(img_thresholdRegion, region_SearchDeviceLocation, img_thresholdRegion);
            //MagnusOpenCVLib.OpeningRectangle(ref img_thresholdRegion, ref img_openingRegionRegion, m_DeviceLocationParameter.m_nOpeningMask, m_DeviceLocationParameter.m_nOpeningMask);
            //MagnusOpenCVLib.SelectBiggestRegion(ref img_openingRegionRegion, ref mat_DeviceLocationRegion);
            //List<System.Drawing.Rectangle> rectLabel = new List<System.Drawing.Rectangle>();

            //MagnusOpenCVLib.SelectRegion(ref mat_DeviceLocationRegion, ref img_SelectRegion, ref rectLabel, m_DeviceLocationParameter.m_nMinWidthDevice, m_DeviceLocationParameter.m_nMinHeightDevice);
            //if (rectLabel == null)
            //    return -99;

            //MagnusOpenCVLib.DilationRectangle(ref img_SelectRegion, ref img_DilationRegion, m_DeviceLocationParameter.m_nDilationMask, m_DeviceLocationParameter.m_nDilationMask);
            //System.Drawing.Rectangle rectangleRoi = new System.Drawing.Rectangle();
            //Image<Gray, Byte> ImageAfterDilationCrop = new Image<Gray, Byte>(m_TeachImage.Gray.Bitmap);
            //Image<Gray, Byte> Img = new Image<Gray, Byte>(m_TeachImage.Gray.Bitmap);


            //int nWidth = 0, nHeight = 0;
            //MagnusOpenCVLib.GetWidthHeightRegion(ref img_DilationRegion, ref nWidth, ref nHeight);
            //if (nWidth >= m_TemplateImage.Gray.Width && nHeight >= m_TemplateImage.Gray.Height)
            //    MagnusOpenCVLib.CropImage(ref ImageAfterDilationCrop, ref Img, img_DilationRegion, ref rectangleRoi);
            //else
            //    MagnusOpenCVLib.CropImage(ref ImageAfterDilationCrop, ref Img, region_SearchDeviceLocation, ref rectangleRoi);

            //System.Drawing.Rectangle rectMatchingPosition = new System.Drawing.Rectangle();
            //bool bIsTemplateFounded = false;
            //if (ImageAfterDilationCrop.Width <= m_TemplateImage.Gray.Width || ImageAfterDilationCrop.Height <= m_TemplateImage.Gray.Height)
            //{
            //    if (DeviceLocationParameter.m_dAngleResolutionTemplate >= 30)
            //        bIsTemplateFounded = m_TemplateMatchingModel.MAgnus_TemplateMatching(imgSource, m_TemplateImage.Gray, DeviceLocationParameter.m_dMinScoreTemplate, DeviceLocationParameter.m_nStepTemplate, DeviceLocationParameter.m_dAngleResolutionTemplate, ref rectMatchingPosition, ref nAngleOutput, ref dScoreOutput);
            //    else
            //    {
            //        bIsTemplateFounded = m_TemplateMatchingModel.MAgnus_TemplateMatching(imgSource, m_TemplateImage.Gray, 0, 24, 15, ref rectMatchingPosition, ref nAngleOutput, ref dScoreOutput);

            //        bIsTemplateFounded = m_TemplateMatchingModel.MAgnus_KdTreeTemplateMatching(imgSource, m_TemplateImage.Gray, DeviceLocationParameter.m_dMinScoreTemplate, DeviceLocationParameter.m_dAngleResolutionTemplate, ref rectMatchingPosition, ref nAngleOutput, ref dScoreOutput);

            //    }
            //}
            //else
            //{
            //if (DeviceLocationParameter.m_dAngleResolutionTemplate >= 30)
            //    bIsTemplateFounded = m_TemplateMatchingModel.MAgnus_TemplateMatching(ImageAfterDilationCrop.Mat, m_TemplateImage.Gray, DeviceLocationParameter.m_dMinScoreTemplate, DeviceLocationParameter.m_nStepTemplate, DeviceLocationParameter.m_dAngleResolutionTemplate, ref rectMatchingPosition, ref nAngleOutput, ref dScoreOutput);
            //else
            //{
            ////bIsTemplateFounded = m_TemplateMatchingModel.MAgnus_TemplateMatching(ImageAfterDilationCrop.Mat, m_TemplateImage.Gray, 0, 24, 15, ref rectMatchingPosition, ref nAngleOutput, ref dScoreOutput);

            ////bIsTemplateFounded = m_TemplateMatchingModel.MAgnus_KdTreeTemplateMatching(ImageAfterDilationCrop.Mat, m_TemplateImage.Gray, DeviceLocationParameter.m_dMinScoreTemplate, DeviceLocationParameter.m_dScaleImageRatio, ref rectMatchingPosition, ref nAngleOutput, ref dScoreOutput);
            //}
            //}

            //MagnusOpenCVLib.CropImage(ref ImageAfterDilationCrop, ref Img, img_DilationRegion, ref rectangleRoi);

            //bool bIsTemplateFounded = false;
            //if (ImageAfterDilationCrop.Width <= templateImageCropped.Width || ImageAfterDilationCrop.Height <= templateImageCropped.Height)
            //{
            //    if (DeviceLocationParameter.m_dAngleResolutionTemplate >= 30)
            //        bIsTemplateFounded = m_TemplateMatchingModel.MAgnus_TemplateMatching(m_TeachImage.Gray, templateImageCropped, DeviceLocationParameter.m_dMinScoreTemplate, DeviceLocationParameter.m_nStepTemplate, DeviceLocationParameter.m_dAngleResolutionTemplate, ref rectMatchingPosition, ref nAngleOutput, ref dScoreOutput);
            //    else
            //    {
            //        bIsTemplateFounded = m_TemplateMatchingModel.MAgnus_TemplateMatching(m_TeachImage.Gray, templateImageCropped, 0, 12, 30, ref rectMatchingPosition, ref nAngleOutput, ref dScoreOutput);

            //        bIsTemplateFounded = m_TemplateMatchingModel.MAgnus_KdTreeTemplateMatching(m_TeachImage.Gray, templateImageCropped, DeviceLocationParameter.m_dMinScoreTemplate, DeviceLocationParameter.m_dAngleResolutionTemplate, ref rectMatchingPosition, ref nAngleOutput, ref dScoreOutput);

            //    }
            //}
            //else
            //{
            //    if (DeviceLocationParameter.m_dAngleResolutionTemplate >= 30)
            //        bIsTemplateFounded = m_TemplateMatchingModel.MAgnus_TemplateMatching(ImageAfterDilationCrop.Mat, templateImageCropped, DeviceLocationParameter.m_dMinScoreTemplate, DeviceLocationParameter.m_nStepTemplate, DeviceLocationParameter.m_dAngleResolutionTemplate, ref rectMatchingPosition, ref nAngleOutput, ref dScoreOutput);
            //    else
            //    {
            //        bIsTemplateFounded = m_TemplateMatchingModel.MAgnus_TemplateMatching(ImageAfterDilationCrop.Mat, templateImageCropped, 0, 12, 30, ref rectMatchingPosition, ref nAngleOutput, ref dScoreOutput);

            //        bIsTemplateFounded = m_TemplateMatchingModel.MAgnus_KdTreeTemplateMatching(ImageAfterDilationCrop.Mat, templateImageCropped, DeviceLocationParameter.m_dMinScoreTemplate, DeviceLocationParameter.m_dAngleResolutionTemplate, ref rectMatchingPosition, ref nAngleOutput, ref dScoreOutput);
            //    }
            //}

            ////List<Point> pPolygon = new List<Point>();
            ////Point pCenter = new Point((rectMatchingPosition.Left + rectMatchingPosition.Right) / 2 + rectangleRoi.Left,
            ////              (rectMatchingPosition.Top + rectMatchingPosition.Bottom) / 2 + rectangleRoi.Top);

            //////top bottom left right  x1 x2 y1 y2
            ////Point po1 = new Point(rectMatchingPosition.Left + rectangleRoi.Left, rectMatchingPosition.Top + rectangleRoi.Top);
            ////Point po2 = new Point(rectMatchingPosition.Right + rectangleRoi.Left, rectMatchingPosition.Top + rectangleRoi.Top);
            ////Point po3 = new Point(rectMatchingPosition.Right + rectangleRoi.Left, rectMatchingPosition.Bottom + rectangleRoi.Top);
            ////Point po4 = new Point(rectMatchingPosition.Left + rectangleRoi.Left, rectMatchingPosition.Bottom + rectangleRoi.Top);

            ////pPolygon.Add(po1);
            ////pPolygon.Add(po2);
            ////pPolygon.Add(po3);
            ////pPolygon.Add(po4);
            ////pPolygon.Add(pCenter);
            ////List<Point> p_Regionpolygon_temp = RotatePolygon(pPolygon, -nAngleOutput, pCenter.X, pCenter.Y);
            //FindNearestPoints_Debug(m_TeachImage.Gray, ref mat_DeviceLocationRegion, rotateRect_Device,debugInfors, bEnableDebug);

            //nCornerIndex = FindNearestPoints_Debug(m_TeachImage.Gray, ref mat_DeviceLocationRegion, rectMatchingPosition, p_Regionpolygon_temp, -(float)nAngleOutput, null, false);
            return 0;
        }
        public static List<Point> RotatePolygon(List<Point> polygon, double angle, double midx, double midy)
        {
            List<Point> newPoints = new List<Point>();

            foreach (Point p in polygon)
            {
                Point newP = RotatePoint(p, angle, midx, midy);
                newPoints.Add(newP);
            }

            return new List<Point>(newPoints);
        }

        private static Point RotatePoint(Point p, double angle, double midx, double midy)
        {
            //if (angle >= 180)
            //    angle = 180 - angle;
            //if (angle <= -180)
            //    angle = 180 + angle;
            double radians = angle * Math.PI / 180.0;
            double cosRadians = Math.Cos(radians);
            double sinRadians = Math.Sin(radians);

            double x = p.X - midx;
            double y = p.Y - midy;

            double newX = x * cosRadians - y * sinRadians + midx;
            double newY = x * sinRadians + y * cosRadians + midy;

            return new Point((int)newX, (int)newY);
        }
    }
}
