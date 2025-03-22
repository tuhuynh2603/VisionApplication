﻿using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using VisionApplication.Algorithm;
using VisionApplication.Define;
using VisionApplication.Hardware;
using VisionApplication.MVVM.View;
using MvCamCtrl.NET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Rectangle = System.Drawing.Rectangle;

namespace VisionApplication
{
    using VisionApplication.MVVM.ViewModel;
    using System.Collections.ObjectModel;
    using VisionApplication.Helper.UIImage;
    using static VisionApplication.Algorithm.VisionAlgorithmInterface;

    public class Track
    {
        private MainWindow mainWindow;
        public InspectionCore m_InspectionCore;
        public int m_Width = 3840;
        public int m_Height = 2748;
        public ImageView[] m_imageViews;

        public VisionResultData[] m_VisionResultDatas;
        public VisionResultData[] m_VisionResultDatas_Total;

        public VisionResultData m_InspectionOnlineThreadVisionResult = new VisionResultData();
        public VisionResultData m_SequenceThreadVisionResult = new VisionResultData();


        public int m_nTrackID;
        public HIKControlCameraView m_hIKControlCameraView;
        public string m_strSeriCamera = "";
        Mat m_frame = new Mat();
        public VideoCapture m_cap;
        public Thread threadInspectOnline;

        public ObservableCollection<DebugInfors> m_StepDebugInfors;
        public List<ArrayOverLay> m_ArrayOverLay;
        public Track(int indexTrack, int numdoc, string serieCam, int width = 3840, int height = 2748)
        {
            m_StepDebugInfors = new ObservableCollection<DebugInfors>();
            m_ArrayOverLay = new List<ArrayOverLay>();
            m_nTrackID = indexTrack;
            m_imageViews = new ImageView[numdoc];
            m_VisionResultDatas = new VisionResultData[100000];
            m_VisionResultDatas_Total = new VisionResultData[100000];

            for (int n = 0; n < m_VisionResultDatas.Length; n++)
            {
                m_VisionResultDatas[n] = new VisionResultData();
                m_VisionResultDatas_Total[n] = new VisionResultData();
            }

            //VisionResultData.ReadLotResultFromExcel(Application.m_strCurrentLot, indexTrack, ref m_VisionResultDatas, ref m_CurrentSequenceDeviceID);

            m_Width = width;
            m_Height = height;
            m_strSeriCamera = serieCam;
            if (serieCam != "none" && serieCam != "")
                m_hIKControlCameraView = new HIKControlCameraView(serieCam, indexTrack);
            //m_cap = new VideoCapture(0);
            //m_cap.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth, m_Width);
            //m_cap.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight, m_Height);
            //m_cap.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps, 10);
            //m_cap.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Autofocus, 1);
            //m_cap.SetCaptureProperty(CapProp.Focus, 65); // set the focus to the specified value

            int dpi = 96;
            m_imageViews[0] = new ImageView(m_Width, m_Height, dpi, indexTrack, 1);
            m_imageViews[0].bufferImage = new byte[m_Width * m_Height * 3];
            m_imageViews[0]._imageWidth = m_Width;
            m_imageViews[0]._imageHeight = m_Height;
            m_imageViews[0]._dpi = dpi;
            m_imageViews[0].SetBackgroundDoc(indexTrack);

            TransformImage transformImage = new TransformImage(m_imageViews[0].grd_Dock);
            m_imageViews[0].transform = transformImage;
            m_imageViews[0].docID = 0;
            m_imageViews[0].trackID = indexTrack;
            m_imageViews[0].dockPaneID = 0;
            m_imageViews[0].visibleRGB = /*indexTrack == 0 ?*/ Visibility.Visible /*: Visibility.Collapsed*/;
            System.Drawing.Size size = new System.Drawing.Size(m_Width, m_Height);
            m_InspectionCore = new InspectionCore(ref size);
            //InspectionCore.Initialize();

            //CheckInspectionOnlineThread();
            //m_CurrentSequenceDeviceID = Application.GetIntRegistry(Application.m_strCurrentDeviceID_Registry[indexTrack], 0);

        }

        //private void Video_ImageGrabbed(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Array.Clear(m_imageViews[0].bufferImage, 0, m_imageViews[0].bufferImage.Length);

        //        m_cap.SetCaptureProperty(CapProp.Autofocus, 0);
        //        //m_cap.SetCaptureProperty(CapProp.Focus, InspectionCore.DeviceLocationParameter.m_nStepTemplate);
        //        m_cap.Retrieve(m_frame);
        //        Image<Bgr, byte> imgg = m_frame.ToImage<Bgr, byte>();
        //        m_imageViews[0].bufferImage = BitmapToByteArray(imgg.ToBitmap());
        //        m_imageViews[0].UpdateNewImageColor(m_imageViews[0].bufferImage, imgg.ToBitmap().Width, imgg.ToBitmap().Height, 96);
        //        CvInvoke.WaitKey(10);
        //    }
        //    catch
        //    {

        //    }
        //}

        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {

            BitmapData bmpdata = null;

            try
            {
                bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int numbytes = bmpdata.Stride * bitmap.Height;
                byte[] bytedata = new byte[numbytes];
                IntPtr ptr = bmpdata.Scan0;

                Marshal.Copy(ptr, bytedata, 0, numbytes);

                return bytedata;
            }
            finally
            {
                if (bmpdata != null)
                    bitmap.UnlockBits(bmpdata);
            }

        }

        //public int Snap()
        //{
        //    m_cap.ImageGrabbed += Video_ImageGrabbed;

        //    m_cap.Start();
        //    if (MainWindow.mainWindow == null)
        //        return -1;

        //    while (MainWindow.mainWindow.bEnableGrabCycle)
        //    {
        //        if (MainWindow.mainWindow == null)
        //            return -1;
        //    }
        //    m_cap.Stop();
        //    m_cap.ImageGrabbed -= Video_ImageGrabbed;

        //    //m_cap.Dispose();
        //    return 0;
        //}

        public int Stream_HIKCamera()
        {
            if (m_hIKControlCameraView == null)
                return - 1;

            if (!m_hIKControlCameraView.m_MyCamera.MV_CC_IsDeviceConnected_NET())
            {
                MainWindowVM.updateCameraConnectionStatusDelegate?.Invoke(m_nTrackID, m_hIKControlCameraView.InitializeCamera(m_strSeriCamera));
            }

            int nRet = m_hIKControlCameraView.m_MyCamera.MV_CC_StartGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                m_hIKControlCameraView.m_bGrabbing = false;
                return 0;
            }
            while (MainWindowVM.bEnableGrabCycle)
            {
                //if (MainWindow.mainWindow == null)
                //    return -1;

                int nWidth = 0, nHeight = 0;

                nRet = m_hIKControlCameraView.m_MyCamera.MV_CC_SetCommandValue_NET("TriggerSoftware");
                if (MyCamera.MV_OK != nRet)
                {
                    //OutputDe("Trigger Software Fail!", nRet);
                    nRet = m_hIKControlCameraView.m_MyCamera.MV_CC_StopGrabbing_NET();
                    return 0;
                }

                m_hIKControlCameraView.CaptureAndGetImageBuffer(ref m_imageViews[0].bufferImage, ref nWidth, ref nHeight);
                m_imageViews[0].UpdateSourceImageMono(nWidth, nHeight);
                //m_imageViews[0].UpdateNewImageColor(m_imageViews[0].bufferImage, nWidth, nHeight, 96);
                if (MyCamera.MV_OK != nRet)
                {
                    m_hIKControlCameraView.m_bGrabbing = false;
                    nRet = m_hIKControlCameraView.m_MyCamera.MV_CC_StopGrabbing_NET();
                    return 0;
                }
            }

            //nRet = m_hIKControlCameraView.m_MyCamera.MV_CC_StopGrabbing_NET();

            //m_hIKControlCameraView.m_MyCamera.MV_CC_SetEnumValue_NET("AcquisitionMode", (uint)MyCamera.MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
            //m_hIKControlCameraView.m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON);
            //m_hIKControlCameraView.m_MyCamera.MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE);


            return 0;
        }


        public int SingleSnap_HIKCamera()
        {
            if (!m_hIKControlCameraView.m_MyCamera.MV_CC_IsDeviceConnected_NET())
                MainWindowVM.updateCameraConnectionStatusDelegate?.Invoke(m_nTrackID, m_hIKControlCameraView.InitializeCamera(m_strSeriCamera));

            int nRet = m_hIKControlCameraView.m_MyCamera.MV_CC_StartGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                m_hIKControlCameraView.m_bGrabbing = false;
                return -1;
            }

            int nWidth = 0, nHeight = 0;
            nRet = m_hIKControlCameraView.m_MyCamera.MV_CC_SetCommandValue_NET("TriggerSoftware");
            if (MyCamera.MV_OK != nRet)
            {
                //OutputDe("Trigger Software Fail!", nRet);
                nRet = m_hIKControlCameraView.m_MyCamera.MV_CC_StopGrabbing_NET();
                return -1;
            }
            m_hIKControlCameraView.CaptureAndGetImageBuffer(ref m_imageViews[0].bufferImage, ref nWidth, ref nHeight);
            m_imageViews[0].UpdateSourceImageMono(nWidth, nHeight);
            // m_imageViews[0].UpdateNewImageColor(m_imageViews[0].bufferImage, nWidth, nHeight, 96);
            nRet = m_hIKControlCameraView.m_MyCamera.MV_CC_StopGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                m_hIKControlCameraView.m_bGrabbing = false;
                return -1;
            }
            return 0;

        }
        //public int SingleSnap()
        //{
        //    m_cap.ImageGrabbed += SingleOffline_ImageGrabbed;

        //    m_cap.Start();
        //    if (MainWindow.mainWindow == null)
        //        return -1;
        //    while (MainWindow.mainWindow.bEnableSingleSnapImages == false)
        //    {
        //        if (MainWindow.mainWindow == null)
        //            return -1;
        //    }
        //    m_cap.Stop();
        //    m_cap.ImageGrabbed -= SingleOffline_ImageGrabbed;

        //    return 0;
        //}


        #region Calculate XYZ Shift and Transform Matrix between robot and camera

        public class MagnusMatrix
        {
            private float[,] data;

            public MagnusMatrix(int rows, int cols)
            {
                data = new float[rows, cols];
            }

            public float this[int i, int j]
            {
                get { return data[i, j]; }
                set { data[i, j] = value; }
            }

            public int Rows => data.GetLength(0);
            public int Cols => data.GetLength(1);


            static PointF[] AddHomogeneousCoordinate(PointF[] points)
            {
                PointF[] homogeneousPoints = new PointF[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    homogeneousPoints[i] = new PointF(points[i].X, points[i].Y);
                }
                return homogeneousPoints;
            }

            public static Mat CalculateTransformMatrix(PointF[] srcPnt, PointF[] dstPnt)
            {
                //float x1c = pC1.X;
                //float y1c = pC1.Y;
                //float x2c = pC2.X;
                //float y2c = pC2.Y;
                //float x1r = pR1.X;
                //float y1r = pR1.Y;
                //float x2r = pR2.X;
                //float y2r = pR2.Y;

                //MagnusMatrix A = new MagnusMatrix(6, 6);
                //A[0, 0] = x1c; A[0, 1] = y1c; A[0, 2] = 1; A[1, 3] = x1c; A[1, 4] = y1c; A[1, 5] = 1;
                //A[2, 0] = x2c; A[2, 1] = y2c; A[3, 3] = x2c; A[3, 4] = y2c;

                //float[] B = { x1r, x2r, y1r, y2r, 0, 0 };

                //float[] x = GaussElimination(A, B);
                //PointF[] srcPnt = new PointF[3];
                //srcPnt[0] = pC1;
                //srcPnt[1] = pC2;
                //srcPnt[2] = pC3;
                PointF[] srcPointsHomogeneous = AddHomogeneousCoordinate(srcPnt);

                //PointF[] dstPnt = new PointF[3];
                //dstPnt[0] = pR1;
                //dstPnt[1] = pR2;
                //srcPnt[2] = pC3;
                PointF[] dstPointsHomogeneous = AddHomogeneousCoordinate(dstPnt);
                Mat mat = new Mat(3, 3, DepthType.Cv32F, 1);
                mat.SetTo(new MCvScalar(10.5f));
                mat = CvInvoke.GetAffineTransform(srcPointsHomogeneous, dstPointsHomogeneous);
                return mat;
            }

            public static PointF ApplyTransformation(Mat transformMatrix, PointF point)
            {
                //System.Array transf = transformMatrix.GetData();
                //object a = transf.GetValue(0, 0);

                //float value = (float)transf.GetValue(0, 0);
                //Matrix<float> transf = new Matrix<float>(3, 3);
                //transformMatrix.CopyTo(transf);
                float x = GetMatValue(transformMatrix, 0, 0) * point.X + GetMatValue(transformMatrix, 0, 1) * point.Y + GetMatValue(transformMatrix, 0, 2);
                float y = GetMatValue(transformMatrix, 1, 0) * point.X + GetMatValue(transformMatrix, 1, 1) * point.Y + GetMatValue(transformMatrix, 1, 2);

                return new PointF(x, y);
            }

            static float GetMatValue(Mat mat, int row, int col)
            {
                // Ensure the indices are within bounds
                if (row < 0 || row >= mat.Rows || col < 0 || col >= mat.Cols)
                {
                    throw new ArgumentOutOfRangeException("Invalid row or column indices");
                }

                // Access the value at the specified row and column
                double dva = mat.GetValue(row, col);
                float value = (float)dva;

                return value;
            }


            public static double CalculateShiftXYAngle(PointF pCenter1, PointF pCorner1, PointF pCenter2, PointF pCorner2)
            {
                double dX1 = pCorner1.X - pCenter1.X;
                double dY1 = pCorner1.Y - pCenter1.Y;
                double dX2 = pCorner2.X - pCenter2.X;
                double dY2 = pCorner2.Y - pCenter2.Y;

                double deltaangle = AngleBetweenVectors(dX1, dY1, dX2, dY2) * RotationDirection(dX1, dY1, dX2, dY2);
                if (Math.Abs(deltaangle) <= 0.5 || Math.Abs(Math.Abs(deltaangle) - 180) <= 0.5)
                {
                    double dot = (pCenter1.X - pCorner1.X) * (pCenter2.X - pCorner2.X) + (pCenter1.Y - pCorner1.Y) * (pCenter2.Y - pCorner2.Y);

                    if (dot < 0)
                        deltaangle = 180;
                }
                return deltaangle;
            }
            public static double AngleWithXAxis(double xa, double ya)
            {
                // Calculate the angle in radians
                double thetaRad = Math.Atan2(ya, xa);

                // Convert radians to degrees
                double thetaDeg = Math.Round((180 / Math.PI) * thetaRad, 2);

                return thetaDeg;
            }
            public static double AngleBetweenVectors(double xa, double ya, double xb, double yb)
            {
                // Calculate the dot product
                double dotProduct = xa * xb + ya * yb;

                // Calculate the magnitudes of the vectors
                double magnitudeA = Math.Sqrt(xa * xa + ya * ya);
                double magnitudeB = Math.Sqrt(xb * xb + yb * yb);

                // Calculate the angle in radians
                if (Math.Abs(dotProduct - magnitudeA * magnitudeB) < 0.0001)
                    if (Math.Abs(dotProduct) < 0.0001)
                        return 90.0;
                    else
                        return 0.0;

                // Convert radians to degrees
                double thetaRad = Math.Acos(dotProduct / (magnitudeA * magnitudeB));
                double thetaDeg = Math.Round((180 / Math.PI) * thetaRad, 2);

                return thetaDeg;
            }
            public static int RotationDirection(double xa, double ya, double xb, double yb)
            {
                // Calculate the cross product
                double crossProduct = xa * yb - xb * ya;

                // Determine the rotation direction
                if (crossProduct > 0)
                {
                    return -1;
                }
                else if (crossProduct < 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        #endregion
        public void DrawInspectionResult(ref int nResult, ref PointF pCenter, ref double dAngle)
        {
            //Stopwatch timeIns = new Stopwatch();

            //timeIns.Restart();
            SolidColorBrush color = new SolidColorBrush(Colors.Yellow);
            if (!MainWindowVM.m_bSequenceRunning)
            {
                foreach (ArrayOverLay overlay in m_ArrayOverLay)
                {
                    SolidColorBrush c = new SolidColorBrush(overlay._color);
                    m_imageViews[0].DrawRegionOverlay(overlay.mat_Region, c);
                }


            }
            else if (m_ArrayOverLay.Count > 0)
            {

                SolidColorBrush c = new SolidColorBrush(m_ArrayOverLay[m_ArrayOverLay.Count - 1]._color);
                m_imageViews[0].DrawRegionOverlay(m_ArrayOverLay[m_ArrayOverLay.Count - 1].mat_Region, c);
            }

            if (m_nTrackID == 0)
            {
                color = new SolidColorBrush(Colors.Blue);
                m_imageViews[0].DrawStringOverlay("(" + pCenter.X.ToString() + ", " + pCenter.Y.ToString() + ", " + ((int)dAngle).ToString() + ")", (int)pCenter.X, (int)pCenter.Y + 30, color, 7);
            }

            if (nResult == (int)ERROR_CODE.PASS)
            {
                color = new SolidColorBrush(Colors.Green);
                m_imageViews[0].DrawString("Good", 10, 10, color, 31);
                color = new SolidColorBrush(Colors.Yellow);

            }
            else
            {
                color = new SolidColorBrush(Colors.Red);

                switch (nResult)
                {

                    case -(int)ERROR_CODE.NO_PATTERN_FOUND:
                        m_imageViews[0].DrawString("Device not found! ", 10, 10, color, 31);

                        break;

                    case -(int)ERROR_CODE.OPPOSITE_CHIP:
                        m_imageViews[0].DrawString("Black Chip ", 10, 10, color, 31);

                        break;

                    case -(int)ERROR_CODE.LABEL_FAIL:
                        m_imageViews[0].DrawString("Label failed ", 10, 10, color, 31);

                        break;
                    case -(int)ERROR_CODE.PROCESS_ERROR:
                        m_imageViews[0].DrawString("Process Error", 10, 10, color, 31);

                        break;

                }
            }
        }

        public int AutoTeach(ref Track m_track, bool bEnableDisplay = false)
        {

            {
                //Track _track = m_track;
                //if (InspectionTabVM.m_bEnableDebug)
                //    m_StepDebugInfors.Clear();
                m_ArrayOverLay.Clear();
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    m_imageViews[0].ClearOverlay();
                    m_imageViews[0].ClearText();
                });
                int nResult;



                List<Point2d> pCenter = new();
                List<Point2d> pCorner = new();
                nResult = m_InspectionCore.Inspect(ref m_InspectionCore.m_TeachImage, ref m_ArrayOverLay, ref pCenter, ref pCorner,m_StepDebugInfors, false);
                //Draw Result
                //if (nResult == 0)
                //{
                m_InspectionCore.m_DeviceLocationResult.m_dCenterDevicePoint = new PointF( (float) pCenter.Last().x, (float) pCenter.Last().y);
                m_InspectionCore.m_DeviceLocationResult.m_dCornerDevicePoint = new PointF((float)pCorner.Last().x, (float)pCorner.Last().y);
                //m_InspectionCore.m_DeviceLocationResult.m_dAngleOxDevice = MagnusMatrix.AngleWithXAxis(pCorner.X - pCenter.X, pCorner.Y - pCenter.Y);
                //}



                if (bEnableDisplay)
                {
                    double dDeltaAngle = MagnusMatrix.CalculateShiftXYAngle(m_InspectionCore.m_DeviceLocationResult.m_dCenterDevicePoint, m_InspectionCore.m_DeviceLocationResult.m_dCornerDevicePoint, m_InspectionCore.m_DeviceLocationResult.m_dCenterDevicePoint, m_InspectionCore.m_DeviceLocationResult.m_dCornerDevicePoint);

                    System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        //LogMessage.WriteToDebugViewer(5 + m_nTrackID, $"{ Application.LineNumber()}: {Application.PrintCallerName()}");

                        DrawInspectionResult(ref nResult, ref m_InspectionCore.m_DeviceLocationResult.m_dCenterDevicePoint, ref dDeltaAngle);
                        //LogMessage.WriteToDebugViewer(5 + m_nTrackID, $"{ Application.LineNumber()}: {Application.PrintCallerName()}");

                    });
                }
                return nResult;
            }
        }


        public List<ArrayOverLay> m_CalibArrayOverLay = new List<ArrayOverLay>();
        public int CalibrationGet3Points(out PointF[] points)
        {
            m_CalibArrayOverLay.Clear();
            int nResult = m_InspectionCore.Calibration_Get3Points(m_InspectionCore.m_SourceImage.Gray, out points, ref m_CalibArrayOverLay);
            PointF[] pointsOverlay = points;

            System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
            {
                m_imageViews[0].ClearOverlay();
                foreach (ArrayOverLay overlay in m_CalibArrayOverLay)
                {
                    SolidColorBrush c = new SolidColorBrush(overlay._color);
                    m_imageViews[0].DrawRegionOverlay(overlay.mat_Region, c);
                }

                SolidColorBrush color = new SolidColorBrush(Colors.Yellow);
                for (int n = 0; n < pointsOverlay.Length; n++)
                {
                    m_imageViews[0].DrawStringOverlay((n + 1).ToString(), (int)pointsOverlay[n].X, (int)pointsOverlay[n].Y, color);
                }
            });


            return nResult;
        }
        public int DebugFunction()
        {
            PointF pCenter = new PointF();
            PointF pCorner = new PointF();
            int nResult = Inspect(out pCenter, out pCorner, true);
            double dDeltaAngle = MagnusMatrix.CalculateShiftXYAngle(pCenter, pCorner, m_InspectionCore.m_DeviceLocationResult.m_dCenterDevicePoint, m_InspectionCore.m_DeviceLocationResult.m_dCornerDevicePoint);
            //Todo need to later after adding the calib function to calculate the transform matrix
            if (MainWindowVM.master.m_hiWinRobotInterface != null && MainWindowVM.master.m_hiWinRobotInterface.m_hiWinRobotUserControl != null)
                MagnusMatrix.ApplyTransformation(HIKRobotVM.m_MatCameraRobotTransform, pCenter);
            //Draw Result
            System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
            {
                DrawInspectionResult(ref nResult, ref pCenter, ref dDeltaAngle);
            });


            return nResult;
        }

        public int Inspect( out PointF pCenterOut, out PointF pCornerOut, bool IsStepDebug)
        {
            //Track _track = m_track;

            if (IsStepDebug)
                m_StepDebugInfors.Clear();

            m_ArrayOverLay.Clear();
            System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
            {
                m_imageViews[0].ClearOverlay();
                m_imageViews[0].ClearText();
            });


            //nResult = m_InspectionCore.Inspect(ref m_InspectionCore.m_TeachImage, ref m_ArrayOverLay, ref pCenter, ref pCorner, m_StepDebugInfors, false);
            //Draw Result
            //if (nResult == 0)
            //{
            //m_InspectionCore.m_DeviceLocationResult.m_dAngleOxDevice = MagnusMatrix.AngleWithXAxis(pCorner.X - pCenter.X, pCorner.Y - pCenter.Y);
            //}

            //Console.WriteLine(matchingResults.Count);
            int nResult;
            //double nAngleOutput = 0;
            List<Point2d> pCenter = new();
            List<Point2d> pCorner = new();
            nResult = m_InspectionCore.Inspect(ref m_InspectionCore.m_SourceImage, ref m_ArrayOverLay, ref pCenter, ref pCorner, m_StepDebugInfors, IsStepDebug);
            pCenterOut = new PointF((float)pCenter.Last().x, (float)pCenter.Last().y);
            pCornerOut = new PointF((float)pCorner.Last().x, (float)pCorner.Last().y);

            return nResult;
        }

        public bool m_bInspecting = false;

        private void InspectThread()
        {
            string[] strCameraName = { "Camera", "BarCode" };
            LogMessage.WriteToDebugViewer(5 + m_nTrackID, $"Start Vision Inspect Thread {strCameraName[m_nTrackID]}");
            PointF pCenter, pCorner;
            Stopwatch timeIns = new Stopwatch();
            timeIns.Start();
            int nDeviceID = m_CurrentSequenceDeviceID;
            int nDeviceIDFail = 0;
            bool bAlreadySetEvent = false;
            while (true)
            {
                //if (!MainWindow.m_IsWindowOpen || MainWindow.mainWindow == null)
                //    break;

                try
                {
                    m_bInspecting = false;
                    pCenter = new PointF(0, 0);
                    pCorner = new PointF(0, 0);


                    //Master.InspectEvent[m_nTrackID].WaitOne();
                    //while (!Master.InspectEvent[m_nTrackID].WaitOne(10))
                    //{
                    //    if (MainWindow.mainWindow == null || !MainWindow.m_IsWindowOpen)
                    //        return;
                    //    Thread.Sleep(5);
                    //}
                    Master.InspectEvent[m_nTrackID].WaitOne();
                    Master.InspectEvent[m_nTrackID].Reset();
                    bAlreadySetEvent = false;
                    //if (MainWindow.mainWindow == null || !MainWindow.m_IsWindowOpen)
                    //    break;


                    nDeviceID = m_CurrentSequenceDeviceID;
                    timeIns.Restart();

                    if (m_imageViews[0].btmSource == null)
                    {

                        System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            OutputLogVM.AddLineOutputLog($" {strCameraName[m_nTrackID]}: Capture Failed!", (int)ERROR_CODE.LABEL_FAIL);

                        });
                        m_InspectionOnlineThreadVisionResult.m_nResult = -(int)ERROR_CODE.NO_PATTERN_FOUND;
                        Master.InspectDoneEvent[m_nTrackID].Set();
                        goto InspectionDone;
                    }

                    m_bInspecting = true;
                    //if (m_CurrentSequenceDeviceID < 0 || m_CurrentSequenceDeviceID >= m_VisionResultDatas.Length)
                    //    m_CurrentSequenceDeviceID = 0;


                    System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        //OutputLogVM.AddLineOutputLog($" {m_nTrackID} Load image to Inspection Core!");
                        m_InspectionCore.LoadImageToInspection(m_imageViews[0].btmSource);
                        //OutputLogVM.AddLineOutputLog($" {m_nTrackID}  Load image to Inspection Core Done!");

                    });

                    //System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                    //{
                    //    OutputLogVM.AddLineOutputLog($"  {m_nTrackID}  Inspect");

                    //});
                    m_SequenceVisionResult = Inspect( out pCenter, out pCorner, false);
                    m_Center_Vision = pCenter;
                    m_dDeltaAngleInspection = MagnusMatrix.CalculateShiftXYAngle(m_Center_Vision, pCorner, m_InspectionCore.m_DeviceLocationResult.m_dCenterDevicePoint, m_InspectionCore.m_DeviceLocationResult.m_dCornerDevicePoint);
                    m_InspectionOnlineThreadVisionResult.m_nResult = m_SequenceVisionResult;
                    if (m_SequenceVisionResult == -(int)ERROR_CODE.PASS)
                        m_InspectionOnlineThreadVisionResult.m_strFullImagePath = MainWindowVM.master.createImageFilePathToSave(nDeviceID, m_SequenceVisionResult, "Camera", AppMagnus.m_strCurrentLot);
                    else
                        m_InspectionOnlineThreadVisionResult.m_strFullImagePath = MainWindowVM.master.createImageFilePathToSave(nDeviceIDFail++, m_SequenceVisionResult, "Camera", AppMagnus.m_strCurrentLot);

                    Master.InspectDoneEvent[m_nTrackID].Set();
                    bAlreadySetEvent = true;
                    LogMessage.WriteToDebugViewer(5 + m_nTrackID, "Total inspection time: " + timeIns.ElapsedMilliseconds.ToString());
                    //System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                    //{
                    //    OutputLogVM.AddLineOutputLog($" {strCameraName[m_nTrackID]}:  Inspect Done. {timeIns.ElapsedMilliseconds}");

                    //});

                    if (AppMagnus.m_bEnableSavingOnlineImage && m_nTrackID == 0 && MainWindowVM.m_bSequenceRunning)
                    {

                        //LogMessage.WriteToDebugViewer(5 + m_nTrackID, $"{ Application.LineNumber()}: {Application.PrintCallerName()}");

                        System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            ImageSaveData imageSaveData = new ImageSaveData();
                            imageSaveData.nDeviceID = nDeviceID;
                            imageSaveData.strLotID = AppMagnus.m_strCurrentLot;
                            imageSaveData.nTrackID = m_nTrackID;
                            imageSaveData.nResult = m_InspectionOnlineThreadVisionResult.m_nResult;
                            imageSaveData.m_strPathImage = m_InspectionOnlineThreadVisionResult.m_strFullImagePath;
                            imageSaveData.imageSave = BitmapSourceConvert.ToMat(m_imageViews[0].btmSource).Clone();
                            lock (Master.m_SaveInspectImageQueue)
                            {
                                Master.m_SaveInspectImageQueue[m_nTrackID].Enqueue(imageSaveData);
                            }
                            //LogMessage.WriteToDebugViewer(5 + m_nTrackID, $"{ Application.LineNumber()}: {Application.PrintCallerName()}");

                        });
                        LogMessage.WriteToDebugViewer(4, $"{m_InspectionOnlineThreadVisionResult.m_strFullImagePath}");

                    }



                InspectionDone: 
                    //m_Center_V                          ision = pCenter;
                    //double dDeltaAngle = MagnusMatrix.CalculateShiftXYAngle(m_Center_Vision, pCorner, m_InspectionCore.m_DeviceLocationResult.m_dCenterDevicePoint, m_InspectionCore.m_DeviceLocationResult.m_dCornerDevicePoint);
                    //m_dDeltaAngleInspection = dDeltaAngle;
                    //Master.InspectDoneEvent[m_nTrackID].Set();

                    System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        double dDeltaAngle = m_dDeltaAngleInspection;
                        DrawInspectionResult(ref m_InspectionOnlineThreadVisionResult.m_nResult, ref pCenter, ref dDeltaAngle);

                        m_imageViews[0].tbl_InspectTime.Text = timeIns.ElapsedMilliseconds.ToString();
                    });

                }
                catch (Exception e)
                {
                    LogMessage.WriteToDebugViewer(5 + m_nTrackID, "Inspection Thread PROCESS ERROR: " + e.ToString());
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        OutputLogVM.AddLineOutputLog($" {strCameraName[m_nTrackID]}: Inspection Thread PROCESS ERROR", (int)ERROR_CODE.LABEL_FAIL);

                    });
                    if (!bAlreadySetEvent)
                    {
                        m_InspectionOnlineThreadVisionResult.m_nResult = -(int)ERROR_CODE.NO_PATTERN_FOUND;
                        Master.InspectDoneEvent[m_nTrackID].Set();
                    }
                    continue;
                }
            }

            LogMessage.WriteToDebugViewer(1, $"Track {m_nTrackID} InspecThread Thread Released.");

        }


        public int m_CurrentSequenceDeviceID = 0;
        public int m_CurrentSequenceDeviceID_Total = 0;
        public int m_CurrentPLCRegisterDeviceID = 0;
        public int m_nCurrentClickMappingID = 0;
        public PointF m_Center_Vision = new PointF();
        public double m_dDeltaAngleInspection = 0.0;
        public int m_SequenceVisionResult = -(int)ERROR_CODE.PASS;
        public int m_nVisionReady = 0;
        public void func_InspectOfflineThread(string strFolderPath)
        {

            if (!InspectionTabVM.bEnableOfflineInspection || MainWindowVM.m_bSequenceRunning)
                return;

            //CheckInspectionOnlineThread();

            DirectoryInfo folder = new DirectoryInfo(strFolderPath);

            // Get a list of items (files and directories) inside the folder
            FileSystemInfo[] items = folder.GetFileSystemInfos();

            // Loop through the items and print their names

            while (InspectionTabVM.bEnableOfflineInspection && !MainWindowVM.m_bSequenceRunning)
            {
                try
                {
                    while (!Master.m_OfflineTriggerSnapEvent[m_nTrackID].WaitOne(10))
                    {
                        //if (MainWindow.mainWindow == null)
                        //    return;

                        if (!InspectionTabVM.bEnableOfflineInspection || MainWindowVM.m_bSequenceRunning)
                            return;

                    }
                    bool bDeviceIDFound = false;
                    foreach (FileSystemInfo item in items)
                    {
                        string strDeviceID = item.Name.Split('.')[0];
                        strDeviceID = strDeviceID.Split('_')[1];
                        int nDeviceID = Int32.Parse(strDeviceID);
                        if (nDeviceID < 0 || nDeviceID >= m_VisionResultDatas.Length)
                            nDeviceID = 0;
                        if (m_nCurrentClickMappingID != nDeviceID - 1)
                            continue;
                        bDeviceIDFound = true;
                        if (m_imageViews[0].bufferImage == null)
                            continue;
                        Array.Clear(m_imageViews[0].bufferImage, 0, m_imageViews[0].bufferImage.Length);
                        // Mono Image
                        m_imageViews[0].UpdateNewImageMono(item.FullName);
                        break;
                    }
                    if (bDeviceIDFound == false)
                        continue;

                    Master.InspectEvent[m_nTrackID].Set();
                    //m_nResult[m_nCurrentClickMappingID] = Inspect();
                    Master.InspectDoneEvent[m_nTrackID].Reset();
                    while (!Master.InspectDoneEvent[m_nTrackID].WaitOne(10))
                    {
                        //if (MainWindow.mainWindow == null)
                        //    return;

                        if (!InspectionTabVM.bEnableOfflineInspection || MainWindowVM.m_bSequenceRunning)
                            return;
                    }
                    Master.InspectDoneEvent[m_nTrackID].Reset();
                    Master.m_EventInspectionOnlineThreadDone[m_nTrackID].Set();

                }
                catch (Exception e)
                {
                    LogMessage.WriteToDebugViewer(1, "PROCESS ERROR. Inspection Offline : " + e.ToString());
                }
            }
        }


        //public void InspectOfflineThread(string strFolderPath)
        //{

        //    //if (mainWindow.bEnableOfflineInspection)
        //    //        return;

        //    if (!MainWindow.mainWindow.bEnableOfflineInspection || MainWindow.mainWindow.bEnableRunSequence)
        //        return;

        //    CheckInspectionOnlineThread();

        //    //MainWindow.mainWindow.bEnableOfflineInspection = true;

        //    DirectoryInfo folder = new DirectoryInfo(strFolderPath);

        //    // Get a list of items (files and directories) inside the folder
        //    FileSystemInfo[] items = folder.GetFileSystemInfos();

        //    // Loop through the items and print their names
        //    Stopwatch timeIns = new Stopwatch();
        //    timeIns.Start();
        //    foreach (FileSystemInfo item in items)
        //    {
        //        try
        //        {

        //            while (!Master.m_hardwareTriggerSnapEvent[m_nTrackID].WaitOne(10))
        //            {
        //                if (MainWindow.mainWindow == null)
        //                    return;

        //                if (!MainWindow.mainWindow.bEnableOfflineInspection || MainWindow.mainWindow.bEnableRunSequence)
        //                    return;

        //            }
        //            timeIns.Restart();

        //            bool bDeviceIDFound = false;

        //                string strDeviceID = item.Name.Split('.')[0];
        //                strDeviceID = strDeviceID.Split('_')[1];
        //                int nDeviceID = Int32.Parse(strDeviceID) -1;
        //                if (nDeviceID < 0 || nDeviceID >= m_nResult.Length)
        //                    nDeviceID = 0;
        //            //if (m_nCurrentClickMappingID != nDeviceID - 1)
        //            //    continue;
        //            m_CurrentSequenceDeviceID = nDeviceID;
        //            bDeviceIDFound = true;
        //                Array.Clear(m_imageViews[0].bufferImage, 0, m_imageViews[0].bufferImage.Length);
        //                // Mono Image
        //                m_imageViews[0].UpdateNewImageMono(item.FullName);


        //            if (bDeviceIDFound == false)
        //                continue;

        //            LogMessage.WriteToDebugViewer(5, "UpdateNewImageMono Inspection Offline : " + timeIns.ElapsedMilliseconds.ToString());
        //            timeIns.Restart();

        //            Master.InspectEvent[m_nTrackID].Set();
        //            //m_nResult[m_nCurrentClickMappingID] = Inspect();
        //            Master.InspectDoneEvent[m_nTrackID].Reset();
        //            while (!Master.InspectDoneEvent[m_nTrackID].WaitOne(10))
        //            {
        //                if (MainWindow.mainWindow == null)
        //                    return;

        //                if (!MainWindow.mainWindow.bEnableOfflineInspection || MainWindow.mainWindow.bEnableRunSequence)
        //                    return;
        //            }
        //            LogMessage.WriteToDebugViewer(5, "Inspection Offline : " + timeIns.ElapsedMilliseconds.ToString());
        //            timeIns.Restart();
        //            Master.InspectDoneEvent[m_nTrackID].Reset();
        //            Master.VisionReadyEvent[m_nTrackID].Set();

        //        }
        //        catch (Exception e)
        //        {
        //            LogMessage.WriteToDebugViewer(5, "PROCESS ERROR. Inspection Offline : " + e.ToString());
        //        }
        //    }
        //}


        public void func_InspectOnlineThread()
        {
            string[] strCameraName = { "Camera", "BarCode" };
            bool bAlreadySetEvent = false;
        Start_InspectionOnlineThread:
            LogMessage.WriteToDebugViewer(7 + m_nTrackID, $"Start Inspection Online thread {strCameraName[m_nTrackID]}");

            CheckInspectionOnlineThread();
            int nWidth = 0, nHeight = 0;
            //Todo If Reset lot ID, need to create new lot ID and reset current Device ID to 0
            if (m_nTrackID == 0)
            {

                m_hIKControlCameraView.m_MyCamera.MV_CC_CloseDevice_NET();
                m_hIKControlCameraView.m_MyCamera.MV_CC_DestroyDevice_NET();

                //if (!m_hIKControlCameraView.m_MyCamera.MV_CC_IsDeviceConnected_NET())
                MainWindowVM.updateCameraConnectionStatusDelegate?.Invoke(m_nTrackID, m_hIKControlCameraView.InitializeCamera(m_strSeriCamera));
                int nRet = m_hIKControlCameraView.m_MyCamera.MV_CC_StartGrabbing_NET();
                //if (MyCamera.MV_OK != nRet)
                //{
                //    hIKControlCameraView.m_bGrabbing = false;
                //    return;
                //}
            }

            //m_cap.ImageGrabbed += Online_ImageGrabbed;
            //m_cap.Start();
            Stopwatch timeIns = new Stopwatch();
            timeIns.Start();
            int nDeviceID = m_CurrentSequenceDeviceID;
            while (true/*MainWindow.mainWindow.m_bSequenceRunning*/)
            {

                //if (MainWindow.mainWindow == null || !MainWindow.m_IsWindowOpen)
                //    break;

                LogMessage.WriteToDebugViewer(7 + m_nTrackID, "Get event HarTrigger ");
                Master.m_hardwareTriggerSnapEvent[m_nTrackID].WaitOne();
                Master.m_hardwareTriggerSnapEvent[m_nTrackID].Reset();
                bAlreadySetEvent = false;
                //if (MainWindow.mainWindow == null || !MainWindow.m_IsWindowOpen)
                //    break;

                LogMessage.WriteToDebugViewer(7 + m_nTrackID, "Get event HarTrigger done ");

                nDeviceID = m_CurrentSequenceDeviceID;
                timeIns.Restart();
                string strFullPathImageOut = "";
                string strBarcodeResult = "";

                try
                {
                    if (m_nTrackID == 0)
                    {
                        //Snap camera
                        int nRet = m_hIKControlCameraView.m_MyCamera.MV_CC_SetCommandValue_NET("TriggerSoftware");
                        if (MyCamera.MV_OK != nRet)
                        {
                            nRet = m_hIKControlCameraView.m_MyCamera.MV_CC_StopGrabbing_NET();
                            System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                OutputLogVM.AddLineOutputLog("Trigger Software Failed!.", (int)ERROR_CODE.LABEL_FAIL);

                            });
                            m_InspectionOnlineThreadVisionResult.m_nResult = -(int)ERROR_CODE.NO_PATTERN_FOUND;
                            Master.m_EventInspectionOnlineThreadDone[m_nTrackID].Set();
                            Master.InspectEvent[m_nTrackID].Reset();
                            goto Start_InspectionOnlineThread;
                        }

                        m_hIKControlCameraView.CaptureAndGetImageBuffer(ref m_imageViews[0].bufferImage, ref nWidth, ref nHeight);
                        if (MyCamera.MV_OK != nRet)
                        {
                            nRet = m_hIKControlCameraView.m_MyCamera.MV_CC_StopGrabbing_NET();
                            System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                OutputLogVM.AddLineOutputLog($" {strCameraName[m_nTrackID]}: Capture and Get Image buffer Failed!.", (int)ERROR_CODE.LABEL_FAIL);

                            });
                            m_InspectionOnlineThreadVisionResult.m_nResult = -(int)ERROR_CODE.NO_PATTERN_FOUND;
                            Master.m_EventInspectionOnlineThreadDone[m_nTrackID].Set();
                            Master.InspectEvent[m_nTrackID].Reset();
                            goto Start_InspectionOnlineThread;
                        }
                        m_imageViews[0].UpdateSourceImageMono(nWidth, nHeight);
                    }
                    else
                    {
                        //Scan Barcode
                        LogMessage.WriteToDebugViewer(7 + m_nTrackID, "Get barcode ");

                        strBarcodeResult = MainWindowVM.master.m_BarcodeReader.GetBarCodeStringAndImage(out strFullPathImageOut, nDeviceID, AppMagnus.m_strCurrentLot);
                        LogMessage.WriteToDebugViewer(7 + m_nTrackID, "Get barcode Done ");

                    }


                    LogMessage.WriteToDebugViewer(7 + m_nTrackID, "Capture and update Image time: " + timeIns.ElapsedMilliseconds.ToString());
                    // Do Inspection
                    Master.InspectDoneEvent[m_nTrackID].Reset();
                    Master.InspectEvent[m_nTrackID].Set();
                    if (!Master.InspectDoneEvent[m_nTrackID].WaitOne(2500))
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            OutputLogVM.AddLineOutputLog($"{strCameraName[m_nTrackID]}: Vision TIME OUT!", (int)ERROR_CODE.LABEL_FAIL);

                        });
                        m_InspectionOnlineThreadVisionResult.m_nResult = -(int)ERROR_CODE.NO_PATTERN_FOUND;
                        Master.m_EventInspectionOnlineThreadDone[m_nTrackID].Set();
                        Master.InspectEvent[m_nTrackID].Reset();
                        goto Start_InspectionOnlineThread;
                    }
                    Master.InspectDoneEvent[m_nTrackID].Reset();

                    // Camera
                    if (m_nTrackID == 0)
                    {
                        m_InspectionOnlineThreadVisionResult.m_nDeviceIndexOnReel = nDeviceID;
                        m_InspectionOnlineThreadVisionResult.m_strDeviceID = m_CurrentSequenceDeviceID.ToString();
                        string strDateTime = string.Format("{0}:{1}:{2}_{3}:{4}:{5}", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"), DateTime.Now.ToString("HH"), DateTime.Now.ToString("mm"), DateTime.Now.ToString("ss"));
                        m_InspectionOnlineThreadVisionResult.m_strDatetime = strDateTime;
                        bAlreadySetEvent = true;
                        Master.m_EventInspectionOnlineThreadDone[m_nTrackID].Set();

                        LogMessage.WriteToDebugViewer(7 + m_nTrackID, "Get Inspection Result Done. Total Inspection Time: " + timeIns.ElapsedMilliseconds.ToString());

                    }
                    //      Barcode
                    else if (m_nTrackID == 1)
                    {
                        LogMessage.WriteToDebugViewer(7 + m_nTrackID, "Barcode InspectionDone!");
                        //device id characters always > 5
                        if (strBarcodeResult.Contains("Dummy") || strBarcodeResult.Length < 1)
                        {
                            m_InspectionOnlineThreadVisionResult.m_nResult = -(int)ERROR_CODE.LABEL_FAIL;
                        }

                        m_InspectionOnlineThreadVisionResult.m_nDeviceIndexOnReel = nDeviceID;
                        m_InspectionOnlineThreadVisionResult.m_strDeviceID = strBarcodeResult;
                        m_InspectionOnlineThreadVisionResult.m_strFullImagePath = strFullPathImageOut;
                        string strDateTime = string.Format("{0}:{1}:{2}_{3}:{4}:{5}", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"), DateTime.Now.ToString("HH"), DateTime.Now.ToString("mm"), DateTime.Now.ToString("ss"));
                        m_InspectionOnlineThreadVisionResult.m_strDatetime = strDateTime;

                        bAlreadySetEvent = true;
                        Master.m_EventInspectionOnlineThreadDone[m_nTrackID].Set();
                        LogMessage.WriteToDebugViewer(7 + m_nTrackID, "Get Barcode Result Done. Total Inspection Time: " + timeIns.ElapsedMilliseconds.ToString());

                        //System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                        //{
                        //    OutputLogVM.AddLineOutputLog("Barcode Save Excel Done!");

                        //});

                    }


                    System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        OutputLogVM.AddLineOutputLog($"{strCameraName[m_nTrackID]}: Device {nDeviceID + 1}, Result = {m_InspectionOnlineThreadVisionResult.m_nResult}.  {timeIns.ElapsedMilliseconds} (ms)", m_InspectionOnlineThreadVisionResult.m_nResult);

                    });
                    timeIns.Restart();
                }
                catch (Exception e)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        OutputLogVM.AddLineOutputLog("Function_Inspection Online Thread FAILED!", (int)ERROR_CODE.LABEL_FAIL);
                    });

                    if (!bAlreadySetEvent)
                    {
                        m_InspectionOnlineThreadVisionResult.m_nResult = -(int)ERROR_CODE.NO_PATTERN_FOUND;
                        Master.m_EventInspectionOnlineThreadDone[m_nTrackID].Set();
                        Master.InspectEvent[m_nTrackID].Reset();
                    }

                    goto Start_InspectionOnlineThread;

                }

            }

            if (!bAlreadySetEvent)
            {
                m_InspectionOnlineThreadVisionResult.m_nResult = -(int)ERROR_CODE.NO_PATTERN_FOUND;
                Master.m_EventInspectionOnlineThreadDone[m_nTrackID].Set();
                Master.InspectEvent[m_nTrackID].Reset();
            }
            LogMessage.WriteToDebugViewer(1, $"Track {m_nTrackID}: func_InspectionOnlineThread Thread Released!");

        }

        private void SaveBtmSource(BitmapSource btm, string path)
        {
            Task.Factory.StartNew(() =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    BitmapSource _bitmapImage = btm;

                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        BitmapEncoder encoder = new BmpBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create((BitmapSource)_bitmapImage));
                        encoder.Save(stream);
                        _bitmapImage.Freeze();
                        stream.Dispose();
                        stream.Close();
                    }
                });
            });
        }

        public void ClearOverLay()
        {
            for (int index_doc = 0; index_doc < m_imageViews.Length; index_doc++)
            {
                m_imageViews[index_doc].ClearText();
                m_imageViews[index_doc].ClearOverlay();
            }
        }

        public void SetCameraResolution()
        {
        }


        public int CheckInspectionOnlineThread()
        {
            if (threadInspectOnline == null)
            {
                Master.InspectDoneEvent[m_nTrackID].Reset();
                Master.InspectEvent[m_nTrackID].Reset();
                threadInspectOnline = new System.Threading.Thread(new System.Threading.ThreadStart(() => InspectThread()));
                threadInspectOnline.Name = m_nTrackID.ToString();
                threadInspectOnline.SetApartmentState(ApartmentState.STA);
                ////threadInspectOnline.IsBackground = true;
                threadInspectOnline.Start();
                threadInspectOnline.Priority = ThreadPriority.Normal;
            }
            else if (!threadInspectOnline.IsAlive)
            {
                Master.InspectDoneEvent[m_nTrackID].Reset();
                Master.InspectEvent[m_nTrackID].Reset();
                threadInspectOnline = new System.Threading.Thread(new System.Threading.ThreadStart(() => InspectThread()));
                threadInspectOnline.Name = m_nTrackID.ToString();
                threadInspectOnline.SetApartmentState(ApartmentState.STA);
                //threadInspectOnline.IsBackground = true;
                threadInspectOnline.Start();
                threadInspectOnline.Priority = ThreadPriority.Normal;
            }

            return 0;
        }

    }


}
