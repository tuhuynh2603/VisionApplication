﻿using Emgu.CV;
using VisionApplication.Define;
using VisionApplication.Hardware.SDKHrobot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Media.Imaging;

namespace VisionApplication
{
    using VisionApplication.Hardware;
    using System.Diagnostics;
    using System.Windows;
    using static HiWinRobotInterface;
    using static VisionApplication.Track;
    using VisionApplication.MVVM.ViewModel;
    using VisionApplication.Model;
    using static VisionApplication.MVVM.ViewModel.MappingCanvasVM;
    using MessageBox = System.Windows.Forms.MessageBox;
    using System.Collections.ObjectModel;
    using VisionApplication.Comm;

    public class Master
    {
        //private int width, height, dpi;
        public Track[] m_Tracks { set; get; }
        public int m_nActiveTrack { set; get; }
        public BarCodeReaderInterface m_BarcodeReader { set; get; }
        //public TeachParametersUC teachParameterUC { set; get; }
        //public VisionParameterUC visionParametersUC { set; get; } = new VisionParameterUC();
        //public MappingSetingUC mappingParameter { set; get; } = new MappingSetingUC();


        public static bool m_bIsTeaching { set; get; }
        public static AutoResetEvent m_NextStepTeachEvent { set; get; }
        //public CommLog commLog = new CommLog();

        public static ManualResetEvent[] InspectEvent { set; get; }
        public static ManualResetEvent[] InspectDoneEvent { set; get; }
        public static ManualResetEvent[] m_hardwareTriggerSnapEvent { set; get; }
        public static AutoResetEvent[] m_OfflineTriggerSnapEvent { set; get; }
        public int m_bNextStepSequence { set; get; } = 0;

        public static AutoResetEvent m_NextStepSequenceEvent { set; get; }
        public static AutoResetEvent m_EmergencyStopSequenceEvent { set; get; }

        public static ManualResetEvent[] m_EventInspectionOnlineThreadDone { set; get; }
        public static AutoResetEvent[] StartWaitPLCToTriggerCameraEvent { set; get; }
        public Thread thread_RobotSequence { set; get; }
        public Thread thread_BarcodeReaderSequence { set; get; }
        public Thread[] thread_InspectSequence { set; get; }
        public Thread threadInspecOffline { set; get; }
        public Thread[] thread_StreamCamera { set; get; }
        public Thread m_TeachThread { set; get; }
        public Thread[] m_SaveInspectImageThread { set; get; }
        public Thread[] m_UpdateResultThread { set; get; }

        public Thread[] m_StartWaitPLCReadyToTriggerCameraThread { set; get; }
        public Thread m_UpdateMappingUIThread { set; get; }
        public Thread m_IOStatusThread { set; get; }

        public static List<ArrayOverLay>[] list_arrayOverlay { set; get; }
        public static Queue<ImageSaveData>[] m_SaveInspectImageQueue { set; get; } = new Queue<ImageSaveData>[2]; // create a queue to hold messages
        public static Queue<VisionResultData>[] m_UpdateResultQueue { set; get; } = new Queue<VisionResultData>[2]; // create a queue to hold messages

        public HiWinRobotInterface m_hiWinRobotInterface { set; get; }
        public PLCCOMM m_plcComm { set; get; }
        #region Contructor Master

        private MainWindowVM m_MainWindowVM { set; get; }
        public Master(MainWindowVM mainVM)
        {
            m_MainWindowVM = mainVM;
            ContructorDocComponent();
            LoadRecipe();

            for (int nTrack = 0; nTrack < AppMagnus.m_nTrack; nTrack++)
                loadTeachImageToUI(nTrack);
            m_NextStepTeachEvent = new AutoResetEvent(false);
            m_bIsTeaching = false;
            m_nActiveTrack = 0;
            InitThread();
        }


        public void InitThread()
        {
            //string[] nSeriCam = { "02C89933333", "none" };
            if (m_IOStatusThread == null)
            {
                m_IOStatusThread = new System.Threading.Thread(new System.Threading.ThreadStart(() => func_IOStatusThread()));
                //m_IOStatusThread.IsBackground = true;
                m_IOStatusThread.Start();
            }

            for (int index_track = 0; index_track < AppMagnus.m_nTrack; index_track++)
            {
                int n = index_track;
                if (m_SaveInspectImageThread[n] == null)
                {
                    m_SaveInspectImageThread[n] = new System.Threading.Thread(new System.Threading.ThreadStart(() => func_RunSaveInspectImageThread(n)));
                    m_SaveInspectImageThread[n].IsBackground = true;
                    m_SaveInspectImageThread[n].Start();
                }


                if (m_UpdateResultThread[n] == null)
                {
                    m_UpdateResultThread[n] = new System.Threading.Thread(new System.Threading.ThreadStart(() => func_UpdateResultThread(n)));
                    m_UpdateResultThread[n].IsBackground = true;
                    m_UpdateResultThread[n].Start();
                }

                RunOnlineSequenceThread(n);
            }


            if (!m_bBarcodeReaderSequenceStatus)
            {
                m_bBarcodeReaderSequenceStatus = true;
                BarcodeReaderSequenceThread();
            }
        }



        public void DeleteMaster()
        {
            for (int nTrack = 0; nTrack < AppMagnus.m_nTrack; nTrack++)
            {
                //m_Tracks[nTrack].m_cap.Dispose();
                InspectEvent[nTrack].Set();
                InspectEvent[nTrack].Dispose();
                InspectDoneEvent[nTrack].Set();
                InspectDoneEvent[nTrack].Dispose();
                m_EventInspectionOnlineThreadDone[nTrack].Set();
                m_EventInspectionOnlineThreadDone[nTrack].Dispose();
            }
        }
        public void LoadRecipe(string strRecipe = "")
        {
            if (strRecipe != "")
            {
                AppMagnus.setRecipeToRegister(strRecipe);
            }

            //System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
            //{
            //    MainWindow.mainWindow.btn_LoadRecipe.Content = Application.currentRecipe;
            //});



            Dictionary<string, string> dictParam = MappingSetingUCVM.LoadMappingParamFromFile();
            ((MappingSetingUCVM)m_MainWindowVM.mMappingSettingUCVM.CurrentViewModel).UpdateMappingParamFromDictToUI(dictParam);

            #region Load Teach Paramter
            for (int nTrack = 0; nTrack < AppMagnus.m_nTrack; nTrack++)
            {

                CategoryTeachParameter categoryTeachParameter = TeachParameterVM.ReloadTeachParameterUI(nTrack);
                m_Tracks[nTrack].m_InspectionCore.UpdateTeachParamFromUIToInspectionCore(categoryTeachParameter, nTrack);


                for (int nArea = 0; nArea < AppMagnus.TOTAL_AREA; nArea++)
                {

                    CategoryVisionParameter visionParam =   VisionParameterVM.ReloadAreaParameterUI(nTrack, nArea); 
                    m_Tracks[nTrack].m_InspectionCore.UpdateAreaParameterFromUIToInspectionCore(visionParam, nArea);
                }


                m_Tracks[nTrack].m_InspectionCore.LoadTeachImageToInspectionCore(nTrack);
                m_Tracks[nTrack].AutoTeach(ref m_Tracks[nTrack]);
            }
            #endregion

        }
        #endregion

        #region ContructorDocComponent
        private void ContructorDocComponent()
        {
            m_Tracks = new Track[AppMagnus.m_nTrack];
            InspectEvent = new ManualResetEvent[AppMagnus.m_nTrack];
            InspectDoneEvent = new ManualResetEvent[AppMagnus.m_nTrack];
            m_OfflineTriggerSnapEvent = new AutoResetEvent[AppMagnus.m_nTrack];
            m_EventInspectionOnlineThreadDone = new ManualResetEvent[AppMagnus.m_nTrack];
            //StartWaitPLCToTriggerCameraEvent = new AutoResetEvent[Application.m_nTrack];
            m_hardwareTriggerSnapEvent = new ManualResetEvent[AppMagnus.m_nTrack];
            m_NextStepSequenceEvent = new AutoResetEvent(false);
            m_EmergencyStopSequenceEvent = new AutoResetEvent(false);
            m_SaveInspectImageThread = new Thread[AppMagnus.m_nTrack];
            m_UpdateResultThread = new Thread[AppMagnus.m_nTrack];
            m_StartWaitPLCReadyToTriggerCameraThread = new Thread[AppMagnus.m_nTrack];
            thread_InspectSequence = new Thread[AppMagnus.m_nTrack];
            thread_StreamCamera = new Thread[AppMagnus.m_nTrack];
            list_arrayOverlay = new List<ArrayOverLay>[AppMagnus.m_nTrack];


            m_hiWinRobotInterface = new HiWinRobotInterface();
            m_plcComm = new PLCCOMM();
            m_BarcodeReader = new BarCodeReaderInterface();
            for (int index_track = 0; index_track < AppMagnus.m_nTrack; index_track++)
            {
                m_SaveInspectImageQueue[index_track] = new Queue<ImageSaveData>();
                m_UpdateResultQueue[index_track] = new Queue<VisionResultData>();
                //for(int index_doc = 0; index_doc < Application.m_nDoc; index_doc++)
                InspectEvent[index_track] = new ManualResetEvent(false);
                InspectDoneEvent[index_track] = new ManualResetEvent(false);
                m_hardwareTriggerSnapEvent[index_track] = new ManualResetEvent(false);
                m_OfflineTriggerSnapEvent[index_track] = new AutoResetEvent(false);

                m_EventInspectionOnlineThreadDone[index_track] = new ManualResetEvent(false);
                //StartWaitPLCToTriggerCameraEvent[index_track] = new AutoResetEvent(false);

                list_arrayOverlay[index_track] = new List<ArrayOverLay>();

                m_Tracks[index_track] = new Track(index_track, 1, AppMagnus.m_strCameraSerial[index_track], AppMagnus.m_Width[index_track], AppMagnus.m_Height[index_track]);
            }
        }
        #endregion

        public BitmapImage LoadBitmap(string pathFile)
        {
            BitmapImage bitmap = new BitmapImage();
            try
            {
                if (!File.Exists(pathFile))
                    return bitmap;
                bitmap.BeginInit();
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(pathFile);
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Image Invalid !!!", "Load Image", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            return bitmap;
        }

        public void loadTeachImageToUI(int nTrack)
        {

            string strImageFilePath = System.IO.Path.Combine(AppMagnus.pathRecipe, AppMagnus.currentRecipe, "teachImage_Track" + (nTrack + 1).ToString() + ".bmp");
            if (!File.Exists(strImageFilePath))
                return;
            else
            {
                string[] nameImageArray = strImageFilePath.Split('\\');
                int leght = nameImageArray.Count();
                string _nameImage = nameImageArray[leght - 1];
                m_Tracks[nTrack].m_imageViews[0].nameImage = _nameImage;

                //Color Image
                //BitmapImage bitmap = new BitmapImage();
                //bitmap = LoadBitmap(strImageFilePath);
                //m_Tracks[nTrack].m_imageViews[0].UpdateNewImage(bitmap);
                // Mono Image
                m_Tracks[nTrack].m_imageViews[0].UpdateNewImageMono(strImageFilePath);
                m_Tracks[nTrack].m_imageViews[0].GridOverlay.Children.Clear();
                //m_Tracks[nTrack].m_imageViews[0].UpdateTextOverlay("", "", DefautTeachingSequence.ColorContentTeached, DefautTeachingSequence.ColorExplaintionTeahing);
                //mainWindow.UpdateTitleDoc(0, "teachImage.bmp", true);
            }
        }

        public void loadImageFromFileToUI(int nTrack, string strFileName)
        {

            if (!File.Exists(strFileName))
                return;
            else
            {
                string[] nameImageArray = strFileName.Split('\\');
                int leght = nameImageArray.Count();
                string _nameImage = nameImageArray[leght - 1];
                m_Tracks[nTrack].m_imageViews[0].nameImage = _nameImage;

                //Color Image
                //BitmapImage bitmap = new BitmapImage();
                //bitmap = LoadBitmap(strImageFilePath);
                //m_Tracks[nTrack].m_imageViews[0].UpdateNewImage(bitmap);
                // Mono Image
                m_Tracks[nTrack].m_imageViews[0].UpdateNewImageMono(strFileName);
                m_Tracks[nTrack].m_InspectionCore.LoadImageToInspection(m_Tracks[nTrack].m_imageViews[0].btmSource);

                m_Tracks[nTrack].m_imageViews[0].GridOverlay.Children.Clear();
                //m_Tracks[nTrack].m_imageViews[0].UpdateTextOverlay("", "", DefautTeachingSequence.ColorContentTeached, DefautTeachingSequence.ColorExplaintionTeahing);
                //mainWindow.UpdateTitleDoc(0, "teachImage.bmp", true);
            }
        }


        public void SaveUIImage(int nTrack)
        {
            string strDateTime = string.Format("({0}.{1}.{2}_{3}.{4}.{5})", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"), DateTime.Now.ToString("HH"), DateTime.Now.ToString("mm"), DateTime.Now.ToString("ss"));
            string strPath = System.IO.Path.Combine(AppMagnus.pathImageSave, "UI Image");
            if (!Directory.Exists(strPath))
                Directory.CreateDirectory(strPath);
            strPath = System.IO.Path.Combine(strPath, $"Track {nTrack + 1}");

            if (!Directory.Exists(strPath))
                Directory.CreateDirectory(strPath);

            string strImageFilePath = System.IO.Path.Combine(strPath, $"{strDateTime}.bmp");
            try
            {

                m_Tracks[nTrack].m_InspectionCore.LoadImageToInspection(m_Tracks[nTrack].m_imageViews[0].btmSource);
                m_Tracks[nTrack].m_InspectionCore.SaveCurrentSourceImage(strImageFilePath, 0);

                m_Tracks[nTrack].m_InspectionCore.SaveCurrentSourceImage(strImageFilePath, 1);
                /*BitmapSource _bitmapImage = m_Tracks[nTrack].m_imageViews[0].btmSource;
                string path_image = strImageFilePath;
                using (FileStream stream = new FileStream(path_image, FileMode.CreateNew))
                {
                    BitmapEncoder encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)_bitmapImage));
                    encoder.Save(stream);
                    _bitmapImage.Freeze();
                    stream.Dispose();
                    stream.Close();
                }*/
            }
            catch (Exception)
            {
            }
        }

        public void SaveUITeachImage(int nTrack)
        {
            string strImageFilePath = System.IO.Path.Combine(AppMagnus.pathRecipe, AppMagnus.currentRecipe, "teachImage_Track" + (nTrack + 1).ToString() + ".bmp");
            try
            {
                if (File.Exists(strImageFilePath))
                    File.Delete(strImageFilePath);
                BitmapSource _bitmapImage = m_Tracks[nTrack].m_imageViews[0].btmSource;
                string path_image = strImageFilePath;
                using (FileStream stream = new FileStream(path_image, FileMode.CreateNew))
                {
                    BitmapEncoder encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)_bitmapImage));
                    encoder.Save(stream);
                    _bitmapImage.Freeze();
                    stream.Dispose();
                    stream.Close();
                }
            }
            catch (Exception)
            {
            }
        }

        public void SaveTemplateImage(int nTrackID)
        {
            string pathFileImage = System.IO.Path.Combine(AppMagnus.pathRecipe, AppMagnus.currentRecipe, "templateImage_Track" + (nTrackID + 1).ToString() + ".bmp");
            CvInvoke.Imwrite(pathFileImage, m_Tracks[nTrackID].m_InspectionCore.m_TemplateImage.Gray);
        }


        internal void func_GrabImageThread(bool bSingleSnap = false)
        {
            //for (int index_track = 0; index_track < Application.m_nTrack; index_track++)
            //{
            if (bSingleSnap)
                //m_Tracks[0].SingleSnap();
                m_Tracks[0].SingleSnap_HIKCamera();
            else
                m_Tracks[0].Stream_HIKCamera();
            //m_Tracks[0].Snap();
            // }
        }
        string m_folderPath = @"C:\";
        internal void RunOfflineSequenceThread(int nTrackID)
        {
            if (m_folderPath == @"C:\" || m_folderPath == "")
                m_folderPath = AppMagnus.pathImageSave;
            // Set the initial directory for the dialog box
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

            folderBrowserDialog.SelectedPath = m_folderPath;

            // Display the dialog box and wait for the user's response
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();

            // If the user clicked the OK button, open the selected folder
            if ((int)result == 1)
            {
                // Get the path of the selected folder
                m_folderPath = folderBrowserDialog.SelectedPath;

                // Open the folder using a DirectoryInfo or other appropriate method
                // ...
            }
            else
                return;

            //Master.InspectEvent[0].Set();

            if (threadInspecOffline == null)
            {
                threadInspecOffline = new System.Threading.Thread(new System.Threading.ThreadStart(() => m_Tracks[nTrackID].func_InspectOfflineThread(m_folderPath)));
                threadInspecOffline.IsBackground = true;
                threadInspecOffline.Start();
            }
            else if (!threadInspecOffline.IsAlive)
            {
                threadInspecOffline = new System.Threading.Thread(new System.Threading.ThreadStart(() => m_Tracks[nTrackID].func_InspectOfflineThread(m_folderPath)));
                threadInspecOffline.IsBackground = true;
                threadInspecOffline.Start();
            }
        }
        internal void RunOnlineSequenceThread(int nTrack)
        {
            //InspectDoneEvent[nTrack].Reset();
            //InspectEvent[nTrack].Reset();
            //m_hardwareTriggerSnapEvent[nTrack].Reset();
            if (thread_InspectSequence[nTrack] == null)
            {

                thread_InspectSequence[nTrack] = new System.Threading.Thread(new System.Threading.ThreadStart(() => m_Tracks[nTrack].func_InspectOnlineThread()));
                //thread_InspectSequence[nTrack].IsBackground = true;
                thread_InspectSequence[nTrack].Start();
            }
            else if (!thread_InspectSequence[nTrack].IsAlive)
            {
                thread_InspectSequence[nTrack] = new System.Threading.Thread(new System.Threading.ThreadStart(() => m_Tracks[nTrack].func_InspectOnlineThread()));
                //thread_InspectSequence[nTrack].IsBackground = true;
                thread_InspectSequence[nTrack].Start();
            }

        }


        public void func_ResetSequenceThread()
        {
            if (ResetSequence() < 0)
            {
                lock (this)
                {
                    m_bMachineNotReadyNeedToReset = true;
                }
                WaitForNextStepSequenceEvent(" Reset Machine Failed. Please try again!");
            }
        }
        public int ResetSequence(bool bEnableStepMode = true)
        {
            lock (this)
            {
                m_bMachineNotReadyNeedToReset = true;
            }

            m_hiWinRobotInterface.StopMotor();
            if (RobotIOStatus.m_EmergencyStatus + RobotIOStatus.m_ImidiateStatus == 0)
                HWinRobot.set_motor_state(HiWinRobotInterface.m_RobotConnectID, 1);

        ResetSequence_Step1:
            int nError = (int)SEQUENCE_OPTION.SEQUENCE_CONTINUE;
            Master.m_EventInspectionOnlineThreadDone[0].Reset();
            Master.m_EventInspectionOnlineThreadDone[1].Reset();
            if (HiWinRobotInterface.m_RobotConnectID < 0)
                return -1;
            HWinRobot.clear_alarm(HiWinRobotInterface.m_RobotConnectID);

            // Stop Motor
            m_hiWinRobotInterface.StopMotor();
            if (MainWindowVM.m_bEnableDebugSequence)
                HWinRobot.set_operation_mode(m_RobotConnectID, (int)ROBOT_OPERATION_MODE.MODE_MANUAL);
            else
                HWinRobot.set_operation_mode(m_RobotConnectID, (int)ROBOT_OPERATION_MODE.MODE_AUTO);

           MainWindowVM.master.m_hiWinRobotInterface.wait_for_stop_motion();
            nError = WaitForNextStepSequenceEvent("Reset Sequence: Stop motor Done. Press Next to Move to place position");
            if (nError == (int)SEQUENCE_OPTION.SEQUENCE_ABORT) return -1;

            else if (nError == (int)SEQUENCE_OPTION.SEQUENCE_IMIDIATE_BUTTON_CONTINUE)
            {
                goto ResetSequence_Step1;
            }
        //else if (nError == (int)SEQUENCE_OPTION.SEQUENCE_GOBACK)
        //{
        //    goto ResetSequence_Step1;
        //}
        //else if (nError == (int)SEQUENCE_OPTION.SEQUENCE_RETRY)
        //{
        //    goto ResetSequence_Step1;

        //}

        ResetSequence_Step2:
            // Move Z up 20mm before move to home
           MainWindowVM.master.m_hiWinRobotInterface.m_hiWinRobotUserControl.MoveZMotorUp(20);
           MainWindowVM.master.m_hiWinRobotInterface.wait_for_stop_motion();
           Thread.Sleep(500);

           MainWindowVM.master.m_hiWinRobotInterface.MoveTo_STATIC_POSITION(HiWinRobotInterface.SequencePointData.PRE_FAILED_BLACK_PLACE_POSITION, true);
           MainWindowVM.master.m_hiWinRobotInterface.wait_for_stop_motion();
            nError = WaitForNextStepSequenceEvent("Reset Sequence: Moving done. Press Next to put the device to position");
            if (nError == (int)SEQUENCE_OPTION.SEQUENCE_ABORT) return -1;
            else if (nError == (int)SEQUENCE_OPTION.SEQUENCE_IMIDIATE_BUTTON_CONTINUE)
            {
                goto ResetSequence_Step2;
            }
        //else if (nError == (int)SEQUENCE_OPTION.SEQUENCE_GOBACK)
        //{
        //    goto ResetSequence_Step1;
        //}
        //else if (nError == (int)SEQUENCE_OPTION.SEQUENCE_RETRY)
        //{
        //    goto ResetSequence_Step1;

        //}

        ResetSequence_Step3:
            HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_AIR_ON, false);
            HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_AIR_OFF, true);
            Thread.Sleep(1000);
            nError = WaitForNextStepSequenceEvent("Reset Sequence: Put device done. Press Next to turn off all the airs");
            if (nError == (int)SEQUENCE_OPTION.SEQUENCE_ABORT) return -1;
            else if (nError == (int)SEQUENCE_OPTION.SEQUENCE_IMIDIATE_BUTTON_CONTINUE)
            {
                goto ResetSequence_Step3;
            }
        //else if (nError == (int)SEQUENCE_OPTION.SEQUENCE_GOBACK)
        //{
        //    goto ResetSequence_Step2;
        //}
        //else if (nError == (int)SEQUENCE_OPTION.SEQUENCE_RETRY)
        //{
        //    goto ResetSequence_Step1;

        //}

        ResetSequence_Step4:
            HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_AIR_ON, false);
            HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_AIR_OFF, false);
            Thread.Sleep(1000);
            nError = WaitForNextStepSequenceEvent("Reset Sequence: Press Next to Move to Ready position");
            if (nError == (int)SEQUENCE_OPTION.SEQUENCE_ABORT) return -1;
            else if (nError == (int)SEQUENCE_OPTION.SEQUENCE_IMIDIATE_BUTTON_CONTINUE)
            {
                goto ResetSequence_Step4;
            }
        //else if (nError == (int)SEQUENCE_OPTION.SEQUENCE_GOBACK)
        //{
        //    goto ResetSequence_Step3;
        //}
        //else if (nError == (int)SEQUENCE_OPTION.SEQUENCE_RETRY)
        //{
        //    goto ResetSequence_Step1;

        //}

        ResetSequence_Step5:
           MainWindowVM.master.m_hiWinRobotInterface.MoveTo_STATIC_POSITION(HiWinRobotInterface.SequencePointData.READY_POSITION, false);
           MainWindowVM.master.m_hiWinRobotInterface.wait_for_stop_motion();
           MainWindowVM.master.m_hiWinRobotInterface.MoveTo_STATIC_POSITION(HiWinRobotInterface.SequencePointData.READY_POSITION, true);
           MainWindowVM.master.m_hiWinRobotInterface.wait_for_stop_motion();
            nError = WaitForNextStepSequenceEvent("Reset Sequence: Press Next to Complete Reset Sequence");
            if (nError == (int)SEQUENCE_OPTION.SEQUENCE_ABORT) return -1;
            else if (nError == (int)SEQUENCE_OPTION.SEQUENCE_IMIDIATE_BUTTON_CONTINUE)
            {
                goto ResetSequence_Step5;
            }
            //else if (nError == (int)SEQUENCE_OPTION.SEQUENCE_GOBACK)
            //{
            //    goto ResetSequence_Step4;
            //}
            //else if (nError == (int)SEQUENCE_OPTION.SEQUENCE_RETRY)
            //{
            //    goto ResetSequence_Step1;

            //}



            if (m_bMachineNotReadyNeedToReset == true)
            {
                lock (this)
                {
                    m_bMachineNotReadyNeedToReset = false;
                }
            }

            return 0;
        }

        public enum SEQUENCE_OPTION
        {
            SEQUENCE_ABORT = -1,
            SEQUENCE_CONTINUE = 0,
            SEQUENCE_IMIDIATE_BUTTON_CONTINUE = 1,
            SEQUENCE_GOBACK = 2,
            SEQUENCE_RETRY = 3
        }

        public static class RobotIOStatus
        {
            public static int m_PLC_Ready_Status = 0;
            public static int m_EmergencyStatus = 0;
            public static int m_ImidiateStatus = 0;
            public static int m_ResetMachineStatus = 0;
            public static int m_RunMachineStatus = 0;
            public static int m_DoorOpennedStatus = 0;
            public static int m_EndLotStatus = 0;
            public static int m_CreateNewLotStatus = 0;
            public static int m_IsLastChipStatus = 0;
            public static int m_EmergencyStatus_Simulate = 0;
            public static int m_ImidiateStatus_Simulate = 0;
            public static int m_ResetMachineStatus_Simulate = 0;
        }




        public static bool m_bMachineNotReadyNeedToReset = true;
        public static bool m_bNeedToImidiateStop = false;
        public static bool m_IsDoorOpennedAction = false;
        public static bool m_bIsReleasePopupMessage = false;
        public static int m_SequenceMode = (int)SEQUENCE_MODE.MODE_AUTO;
        public static int m_SequenceMode_With_Or_No_Robot = 0;
        public static DateTime startLot_dateTime;
        public static int m_nTotalCompletedLot = 0;

        private void func_IOStatusThread()
        {
            int bEmergencyStatus_Backup = -1;
            int bImidiateStatus_Backup = -1;
            int bResetStatus_Backup = -1;
            int bDoorStatus_Backup = RobotIOStatus.m_DoorOpennedStatus;// HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.PLC_DOOR_STATUS);
            int bEndLotStatus_Backup = RobotIOStatus.m_EndLotStatus;// HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.PLC_END_LOT);
            int bCreateNewLotStatus_Backup = RobotIOStatus.m_CreateNewLotStatus;// HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.PLC_CREATE_NEW_LOT);
            m_SequenceMode = m_plcComm.ReadPLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_MANUAL_BARCODE_ENABLE);

            while (true)
            {
                //if (MainWindow.mainWindow == null || !MainWindow.m_IsWindowOpen)
                //    break;

                Thread.Sleep(50);

                if (m_SequenceMode == (int)SEQUENCE_MODE.MODE_MANUAL)
                {
                    if (HiWinRobotInterface.m_RobotConnectID >= 0)
                    {
                        HWinRobot.disconnect(HiWinRobotInterface.m_RobotConnectID);
                    }
                }
                else
                {
                    if (HiWinRobotInterface.m_RobotConnectID < 0 || m_hiWinRobotInterface == null)
                    {
                        RobotIOStatus.m_EmergencyStatus = RobotIOStatus.m_EmergencyStatus_Simulate;
                        RobotIOStatus.m_ImidiateStatus = RobotIOStatus.m_ImidiateStatus_Simulate;
                        RobotIOStatus.m_ResetMachineStatus = RobotIOStatus.m_ResetMachineStatus_Simulate;
                    }
                    else
                    {
                        RobotIOStatus.m_PLC_Ready_Status = HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.PLC_PACKING_PROCESS_READY);
                        RobotIOStatus.m_EmergencyStatus = HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.EMERGENCY_STATUS) | RobotIOStatus.m_EmergencyStatus_Simulate;
                        RobotIOStatus.m_ImidiateStatus = HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.IMIDIATE_STOP_STATUS) | RobotIOStatus.m_ImidiateStatus_Simulate;
                        RobotIOStatus.m_ResetMachineStatus = HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.RESET_STATUS) /*| m_ResetMachineStatus_Simulate*/;
                        RobotIOStatus.m_RunMachineStatus = HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.RUNSEQUENCE_STATUS);
                        RobotIOStatus.m_DoorOpennedStatus = HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.PLC_DOOR_STATUS);
                        m_SequenceMode_With_Or_No_Robot = HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.PLC_RUN_NO_ROBOT);


                        //m_CreateNewLotStatus = HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.PLC_CREATE_NEW_LOT);
                        RobotIOStatus.m_EndLotStatus = HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.PLC_END_LOT);

                        if (RobotIOStatus.m_EmergencyStatus > 1 && m_bRobotSequenceStatus)
                        {
                            m_bMachineNotReadyNeedToReset = true;
                        reconnect:
                            m_hiWinRobotInterface.ReconnectToHIKRobot();
                            System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                OutputLogVM.AddLineOutputLog($"Robot Disconnected! Reconnecting... = {RobotIOStatus.m_EmergencyStatus}!", (int)ERROR_CODE.LABEL_FAIL);
                            });
                            Thread.Sleep(1000);

                            if (m_RobotConnectID >= 0)
                            {
                                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    OutputLogVM.AddLineOutputLog($"Robot Connected.. = {RobotIOStatus.m_EmergencyStatus}!", (int)ERROR_CODE.LABEL_FAIL);
                                });
                                continue;
                            }
                            else
                                goto reconnect;

                        }

                    }

                    if (RobotIOStatus.m_ImidiateStatus == 1)
                    {
                        m_bNeedToImidiateStop = true;
                        string strMess = "Imidiate Stop Button clicked!";
                        m_hiWinRobotInterface.StopMotor();
                        HiWinRobotInterface.SetMotorState(0);


                            WarningMessageBoxVM.popupWarningMessageBoxDelegate?.Invoke(strMess, WARNINGMESSAGE.MESSAGE_IMIDIATESTOP);
                            //Thread.Sleep(100);
                    }

                    else if (!MainWindowVM.m_bEnableDebugSequence)
                    {
                        if (RobotIOStatus.m_DoorOpennedStatus == 1)
                        {
                            //m_IsDoorOpennedAction = true;
                            if (MainWindowVM.m_bSequenceRunning)
                            {
                                m_bNeedToImidiateStop = true;
                                m_hiWinRobotInterface.StopMotor();
                                HiWinRobotInterface.SetMotorState(0);
                                //MainWindowVM.master.m_hiWinRobotInterface.wait_for_stop_motion();
                                string strMess = "Door is openned!";
                               WarningMessageBoxVM.popupWarningMessageBoxDelegate?.Invoke(strMess, WARNINGMESSAGE.MESSAGE_IMIDIATESTOP);
                            }
                        }
                    }

                    if (bDoorStatus_Backup != RobotIOStatus.m_DoorOpennedStatus)
                    {
                        //m_plcComm.WritePLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_IMIDIATE_STATUS, RobotIOStatus.m_ImidiateStatus);
                        bDoorStatus_Backup = RobotIOStatus.m_DoorOpennedStatus;

                        LogMessage.WriteToDebugViewer(9, $"Door Signal Status changed Status = {bDoorStatus_Backup}!");

                        System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            OutputLogVM.AddLineOutputLog($"Door Signal Status changed Status = {bDoorStatus_Backup}!", (int)ERROR_CODE.LABEL_FAIL);
                        });
                    }
                    bDoorStatus_Backup = RobotIOStatus.m_DoorOpennedStatus;


                    if (RobotIOStatus.m_EmergencyStatus == 1)
                    {
                        m_bMachineNotReadyNeedToReset = true;
                        m_hiWinRobotInterface.StopMotor();
                        HiWinRobotInterface.SetMotorState(0);
                        string strMess = "Emergency Button clicked, please release them  then reset the sequence!";
                       WarningMessageBoxVM.popupWarningMessageBoxDelegate?.Invoke(strMess, WARNINGMESSAGE.MESSAGE_EMERGENCY);

                    }

                    if (RobotIOStatus.m_RunMachineStatus == 1)
                    {

                        RobotIOStatus.m_EmergencyStatus = HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.EMERGENCY_STATUS) | RobotIOStatus.m_EmergencyStatus_Simulate;

                        if (RobotIOStatus.m_EmergencyStatus == 0)
                           WarningMessageBoxVM.popupWarningMessageBoxDelegate?.Invoke("", WARNINGMESSAGE.MESSAGE_EMERGENCY, false);


                        System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            m_MainWindowVM.Run_Sequence();
                        });
                        //}
                        Thread.Sleep(2000);

                    }


                    if (m_plcComm == null)
                        continue;

                    if (!m_plcComm.m_modbusClient.Connected)
                        continue;

                    if (bEmergencyStatus_Backup != RobotIOStatus.m_EmergencyStatus)
                    {
                        bEmergencyStatus_Backup = RobotIOStatus.m_EmergencyStatus;
                        LogMessage.WriteToDebugViewer(9, $"Emergency Status changed Status = {RobotIOStatus.m_EmergencyStatus}!");

                        System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            OutputLogVM.AddLineOutputLog($"Emergency Status changed Status = {RobotIOStatus.m_EmergencyStatus}!", (int)ERROR_CODE.LABEL_FAIL);
                        });
                    }

                    if (bImidiateStatus_Backup != RobotIOStatus.m_ImidiateStatus)
                    {
                        bImidiateStatus_Backup = RobotIOStatus.m_ImidiateStatus;

                        LogMessage.WriteToDebugViewer(9, $"Imidiate button Status changed Status = {RobotIOStatus.m_ImidiateStatus}!");

                        System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            OutputLogVM.AddLineOutputLog($"Imidiate button Status changed Status = {RobotIOStatus.m_ImidiateStatus}!", (int)ERROR_CODE.LABEL_FAIL);
                        });
                    }

                    bool bIsShow = m_MainWindowVM.mWarningMessageBoxVM.isVisible == Visibility.Collapsed? false:true;

                    if (bResetStatus_Backup != RobotIOStatus.m_ResetMachineStatus || bIsShow)
                    {

                        RobotIOStatus.m_EmergencyStatus = HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.EMERGENCY_STATUS) | RobotIOStatus.m_EmergencyStatus_Simulate;




                        if (RobotIOStatus.m_ResetMachineStatus == 1)
                        {
                          WarningMessageBoxVM.continueSequenceButtonClickedDelegate?.Invoke(WARNINGMESSAGE.MESSAGE_IMIDIATESTOP);

                        }

                        if (bResetStatus_Backup != RobotIOStatus.m_ResetMachineStatus)
                        {
                            LogMessage.WriteToDebugViewer(9, $"Reset button Status changed Status = {RobotIOStatus.m_ResetMachineStatus}!");

                            System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                               OutputLogVM.AddLineOutputLog($"Reset button Status changed Status = {RobotIOStatus.m_ResetMachineStatus}!", (int)ERROR_CODE.LABEL_FAIL);
                            });
                        }
                    }
                    bResetStatus_Backup = RobotIOStatus.m_ResetMachineStatus;

                }

                //m_CreateNewLotStatus = m_plcComm.ReadPLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_RESET_LOT); //  HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.PLC_CREATE_NEW_LOT);


                RobotIOStatus.m_CreateNewLotStatus = m_plcComm.ReadPLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_RESET_LOT);
                if (RobotIOStatus.m_CreateNewLotStatus == 1 && bCreateNewLotStatus_Backup != RobotIOStatus.m_CreateNewLotStatus)  /*(m_plcComm.ReadPLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_RESET_LOT) > 0)*/
                {

                    m_plcComm.WritePLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_RESET_LOT, 0);

                    bCreateNewLotStatus_Backup = RobotIOStatus.m_CreateNewLotStatus;


                    if (m_IsPIDSentStatus)
                    {

                        System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            OutputLogVM.AddLineOutputLog("Busy when sending lot data to PID machine, cannot create lot!!!", (int)ERROR_CODE.LABEL_FAIL);
                        });
                        continue;
                    }

                    SerialCommunication.WriteData("STATUS_PID");
                    SerialCommunication.m_SerialDataReceivedEvent.Reset();
                    if (SerialCommunication.m_SerialDataReceivedEvent.WaitOne(5000) == false)
                    {

                        System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            OutputLogVM.AddLineOutputLog("TimeOut when reading data from PID! Please check the PID machine status before running sequence", (int)ERROR_CODE.LABEL_FAIL);
                        });

                        WaitForNextStepSequenceEvent("TimeOut when reading data from PID! Please check the PID machine status before running sequence", true);
                        continue;
                    }

                    string strReceived_ACK = SerialCommunication.ReadData();
                    if (strReceived_ACK.Contains("STATUS_ERROR"))
                    {


                        System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            OutputLogVM.AddLineOutputLog($"{strReceived_ACK}. Please check the PID machine status before running sequence", (int)ERROR_CODE.LABEL_FAIL);
                        });

                        WaitForNextStepSequenceEvent("STATUS_ERROR. Please check the PID machine status before running sequence", true);
                        continue;
                    }





                    RobotIOStatus.m_IsLastChipStatus = 0;
                    LogMessage.WriteToDebugViewer(9, $"Reset Lot");
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        OutputLogVM.AddLineOutputLog($"Reset Lot", (int)ERROR_CODE.LABEL_FAIL);
                        startLot_dateTime = DateTime.Now;
                        AppMagnus.m_strCurrentLot = string.Format("{0}{1}{2}_{3}{4}{5}", startLot_dateTime.ToString("yyyy"), startLot_dateTime.ToString("MM"), startLot_dateTime.ToString("dd"), startLot_dateTime.ToString("HH"), startLot_dateTime.ToString("mm"), startLot_dateTime.ToString("ss"));
                        if (AppMagnus.m_strStartLotDay != AppMagnus.m_strCurrentLot.Split('_')[0])
                            StatisticVM.clearStatisticDelegate?.Invoke();
                        AppMagnus.m_strStartLotDay = AppMagnus.m_strCurrentLot.Split('_')[0];
                        AppMagnus.SetStringRegistry(AppMagnus.m_strCurrentLot_Registry, AppMagnus.m_strCurrentLot);
                        //MainWindow.mainWindow.m_staticView.ClearStatistic();
                        Thread.Sleep(250);
                        for (int nT = 0; nT < 2; nT++)
                        {
                            for (int n = 0; n < ((MappingSetingUCVM)MainWindowVM.mainWindowVM.mMappingSettingUCVM.CurrentViewModel).categoriesMappingParam.M_NumberDevicePerLot; n++)
                            {
                                m_Tracks[nT].m_VisionResultDatas[n] = new VisionResultData();
                                m_Tracks[nT].m_VisionResultDatas_Total[n] = new VisionResultData();

                            }
                            //m_Tracks[nT].m_CurrentSequenceDeviceID = 0;
                            VisionResultData.SaveSequenceResultToExcel(AppMagnus.m_strCurrentLot, nT, new VisionResultData());
                            //VisionResultData.SaveSequenceResultToExcel(Application.m_strCurrentLot, nT, new VisionResultData(), true);

                            MappingCanvasVM.setMappingPageDelegate?.Invoke(nT, 0);
                            m_MainWindowVM.LoadStatistic(nT, false);
                            //InspectEvent[nT].Reset();
                            //InspectDoneEvent[nT].Reset();
                            //m_hardwareTriggerSnapEvent[nT].Reset();
                            m_Tracks[nT].m_CurrentSequenceDeviceID = 0;
                            while (m_SaveInspectImageQueue[nT].Count > 0)
                                m_SaveInspectImageQueue[nT].Dequeue();

                        }

                        //lock (MainWindowVM.master.m_Tracks[0])
                        //{
                        //   MainWindow.mainWindow.LoadStatistic(0, true);
                        //}
                        //Thread.Sleep(250);

                        //lock (MainWindowVM.master.m_Tracks[1])
                        //{
                        //   MainWindow.mainWindow.LoadStatistic(1, true);
                        //}
                        //Thread.Sleep(250);

                        OutputLogVM.AddLineOutputLog($"Create New Lot ID: {AppMagnus.m_strCurrentLot} ", (int)ERROR_CODE.LABEL_FAIL);

                        if (true)
                        {
                            //MainWindowVM mainvm = (MainWindowVM)MainWindow.mainWindow.DataContext;
                            m_MainWindowVM.m_strCurrentLotID = AppMagnus.m_strCurrentLot;
                        }

                        else
                        {

                            System.Windows.Forms.MessageBox.Show("Please KeyIn the Lot ID (on the top) then press Continue");
                        }
                    });
                    m_plcComm.WritePLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_RESET_LOT_ACK, 1);

                    Thread.Sleep(250);

                    //m_CreateNewLotStatus = 0;
                }
                bCreateNewLotStatus_Backup = RobotIOStatus.m_CreateNewLotStatus;
            }
        }

        public int WaitForNextStepSequenceEvent(string strDebugMessage = "", bool bPopUpInformation = false, int nTimeout = 0)
        {
            m_NextStepSequenceEvent.Reset();
            m_bNextStepSequence = (int)SEQUENCE_OPTION.SEQUENCE_CONTINUE;
            if (RobotIOStatus.m_EmergencyStatus == 1)
                return (int)SEQUENCE_OPTION.SEQUENCE_ABORT;

            if (m_bNeedToImidiateStop)
            {
                //WarningMessageBoxVM.popupWarningMessageBoxDelegate?.Invoke(strDebugMessage, WARNINGMESSAGE.MESSAGE_IMIDIATESTOP);
                while (!m_NextStepSequenceEvent.WaitOne(10))
                {
                    //if (MainWindow.mainWindow == null)
                    //    return (int)SEQUENCE_OPTION.SEQUENCE_ABORT;

                    if (RobotIOStatus.m_EmergencyStatus == 1)
                        return (int)SEQUENCE_OPTION.SEQUENCE_ABORT;
                }
                //Show Dialog
            }
            else if (MainWindowVM.m_bEnableDebugSequence)
            {
               WarningMessageBoxVM.popupWarningMessageBoxDelegate?.Invoke(strDebugMessage, WARNINGMESSAGE.MESSAGE_STEPDEBUG);
                while (!m_NextStepSequenceEvent.WaitOne(10))
                {
                    //if (MainWindow.mainWindow == null)
                    //    return (int)SEQUENCE_OPTION.SEQUENCE_ABORT;

                    if (RobotIOStatus.m_EmergencyStatus == 1 || RobotIOStatus.m_ImidiateStatus == 1)
                        return (int)SEQUENCE_OPTION.SEQUENCE_ABORT;
                }
            }
            else if (bPopUpInformation)
            {
               WarningMessageBoxVM.popupWarningMessageBoxDelegate?.Invoke(strDebugMessage, WARNINGMESSAGE.MESSAGE_INFORMATION);
                while (!m_NextStepSequenceEvent.WaitOne(10))
                {
                    //if (MainWindow.mainWindow == null)
                    //    return (int)SEQUENCE_OPTION.SEQUENCE_ABORT;

                    if (RobotIOStatus.m_EmergencyStatus == 1 || RobotIOStatus.m_ImidiateStatus == 1)
                        return (int)SEQUENCE_OPTION.SEQUENCE_ABORT;
                }
            }

            return m_bNextStepSequence;
        }


        public bool m_bRobotSequenceStatus = false;

        internal bool RobotSequenceThread()
        {

            if (thread_RobotSequence == null)
            {

                thread_RobotSequence = new System.Threading.Thread(new System.Threading.ThreadStart(() => func_RobotSequence()));
                //thread_RobotSequence.IsBackground = true;
                thread_RobotSequence.Start();
                return false;
            }
            else if (!thread_RobotSequence.IsAlive)
            {
                thread_RobotSequence = new System.Threading.Thread(new System.Threading.ThreadStart(() => func_RobotSequence()));
                //thread_RobotSequence.IsBackground = true;
                thread_RobotSequence.Start();
                return false;
            }
            return true;
        }

        void func_RobotSequence()
        {
            LogMessage.WriteToDebugViewer(9, "Run Robot Sequence Thread....");

            System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
            {
                OutputLogVM.AddLineOutputLog("Run Sequence Thead...", (int)ERROR_CODE.LABEL_FAIL);
                //MainWindow.mainWindow.LoadStatistic(0,true);
            });
            //m_SequenceMode = (int)SEQUENCE_MODE.MODE_AUTO;
            if (m_SequenceMode == (int)SEQUENCE_MODE.MODE_AUTO)
            {
                LogMessage.WriteToDebugViewer(0, $"Begin sequence... AUTO MODE");
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {

                    OutputLogVM.AddLineOutputLog($"Begin sequence... AUTO MODE");
                });

                RobotSequence();

                PrinterSerialCommunicationSequence();

                m_plcComm.WritePLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_ROBOT_RUNNING_STATUS, -1);

            }
            else
            {
                LogMessage.WriteToDebugViewer(0, $"Begin sequence... MANUAL MODE");
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {

                    OutputLogVM.AddLineOutputLog($"Begin sequence... MANUAL MODE");
                });

                while (true)
                {
                    if (!MainWindowVM.m_bSequenceRunning || m_bMachineNotReadyNeedToReset || RobotIOStatus.m_EndLotStatus == 1)
                    {
                        break;
                    }
                }
            }


            if (m_SequenceMode_With_Or_No_Robot == 1)
            {
                LogMessage.WriteToDebugViewer(0, $"Begin sequence... MANUAL MODE");
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {

                    OutputLogVM.AddLineOutputLog($"Begin sequence... MANUAL MODE");
                });

                while (true)
                {
                    if (!MainWindowVM.m_bSequenceRunning || m_bMachineNotReadyNeedToReset || RobotIOStatus.m_EndLotStatus == 1)
                    {
                        break;
                    }
                }
            }


        }


        void PrinterSerialCommunicationSequence()
        {
            // Waiting for Complete Reel from PLC Signal
            int m_CompletedReelStatus = HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.PLC_COMPLETED_LOT);
            //m_CompletedReelStatus = 1;
            while (m_CompletedReelStatus != 1)
            {
                Thread.Sleep(500);
                m_CompletedReelStatus = HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.PLC_COMPLETED_LOT);

                if (!MainWindowVM.m_bSequenceRunning || m_bMachineNotReadyNeedToReset || RobotIOStatus.m_EndLotStatus == 1)
                {
                    m_bRobotSequenceStatus = false;
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                    {



                       MainWindowVM.btn_runChecked = false;
                        //MainWindowVM mainvm = (MainWindowVM)MainWindow.mainWindow.DataContext;
                        MainWindowVM.m_bSequenceRunning = false;
                    });
                    return;
                }
            }

            // Waiting for data received from Client
            //int nRepeat = 0;
            if (m_CompletedReelStatus == 1)
            {

                sendLastLotDataToPID();
            }
        }

        bool m_IsPIDSentStatus = false;
        public void sendLastLotDataToPID()
        {
            m_IsPIDSentStatus = true;
            ObservableCollection<VisionResultDataExcel> list_BarcodeResult = new ObservableCollection<VisionResultDataExcel>();
            LotBarcodeDatatableVM.ReadLotResultFromExcel(AppMagnus.m_strCurrentLot, list_BarcodeResult);
            string strDataSend = LotBarcodeDatatableVM.CombineReelIDStringSentToClient(list_BarcodeResult, AppMagnus.m_strCurrentLot);
            int nError = SendAndReceiveAndCheckDataFromPID(strDataSend);
            if (nError == -1)
            {
                m_IsPIDSentStatus = false;
                return;
            }

            int nTotalDevice = 0;
            string strLastLot = "";
            VisionResultData.ReadTotalLotFromExcel(AppMagnus.m_strCurrentLot, 1, ref m_nTotalCompletedLot, ref nTotalDevice, ref strLastLot);

            if (strLastLot == AppMagnus.m_strCurrentLot)
            {
                m_IsPIDSentStatus = false;
                return;
            }

            m_nTotalCompletedLot++;
            VisionResultData.SaveTotalLotToExcel(AppMagnus.m_strCurrentLot, 1, m_nTotalCompletedLot, startLot_dateTime.ToString(), list_BarcodeResult.Count);
            m_IsPIDSentStatus = false;
        }

        public int SendAndReceiveAndCheckDataFromPID(string strDataSend)
        {

            SerialCommunication.m_SerialDataReceivedEvent.Reset();
        SendDataToClient:

            SerialCommunication.WriteData(strDataSend);

            int nTimeout = 0;
            while (SerialCommunication.m_SerialDataReceivedEvent.WaitOne(100) == false)
            {
                if (m_bMachineNotReadyNeedToReset)
                {
                    m_bRobotSequenceStatus = false;
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        MainWindowVM.btn_runChecked = false;
                       MainWindowVM.m_bSequenceRunning = false;
                    });
                    return -1;
                }

                nTimeout += 100;
               WarningMessageBoxVM.popupWarningMessageBoxDelegate?.Invoke("Waiting 'ReelData,N,LotID_OK//Error' from PID...", WARNINGMESSAGE.MESSAGE_INFORMATION);
                if (nTimeout >= 5000)
                {
                    nTimeout = 0;
                    string strString = "Timeout when Reading Data from PID. Press 'Continue' to resend or 'Abort' to end sequence.";
                    int nSelect = WaitForNextStepSequenceEvent(strString, true);
                    if (nSelect == (int)SEQUENCE_OPTION.SEQUENCE_IMIDIATE_BUTTON_CONTINUE)
                        goto SendDataToClient;
                    else
                        return -1;
                }

            }
           WarningMessageBoxVM.popupWarningMessageBoxDelegate?.Invoke("Waiting 'ReelData,N,LotID_OK//Error' from PID...", WARNINGMESSAGE.MESSAGE_INFORMATION, false);

            SerialCommunication.m_SerialDataReceivedEvent.Reset();
            // Check whether data sent and received are same or not
            string[] strReceived = SerialCommunication.ReadData().Split('_');
            if (strReceived.Length < 1)
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {

                    OutputLogVM.AddLineOutputLog("Data received from PID is wrong format. ", (int)ERROR_CODE.LABEL_FAIL);
                });
                SerialCommunication.WriteData("DATA_ERROR");
                string strString = "Data received from PID is wrong format.  Press 'Continue' to resend or 'Abort' to end sequence.";
                int nSelect = WaitForNextStepSequenceEvent(strString, true);
                if (nSelect == (int)SEQUENCE_OPTION.SEQUENCE_IMIDIATE_BUTTON_CONTINUE)
                    goto SendDataToClient;
                else
                    return -1;
            }


            string[] strRe = strReceived[0].Split(',');
            string[] strSend = strDataSend.Split(',');

            for (int n = 0; n < strDataSend.Split(',').Length; n++)
            {
                if (strRe.Length <= n)
                    break;
                LogMessage.WriteToDebugViewer(1, $"Send - Received: {strSend[n]}     {strRe[n]}");
            }

            if (strReceived[0] != strDataSend)
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {

                    OutputLogVM.AddLineOutputLog("Mismatching Data Between Client And TR Machine Failed! Please Check the Connection.", (int)ERROR_CODE.LABEL_FAIL);
                });
                SerialCommunication.WriteData("DATA_ERROR");

                string strString = "Mismatching Data Between PID and T&R. Press 'Continue' to resend or 'Abort' to end sequence.";
                int nSelect = WaitForNextStepSequenceEvent(strString, true);

                if (nSelect == (int)SEQUENCE_OPTION.SEQUENCE_IMIDIATE_BUTTON_CONTINUE)
                    goto SendDataToClient;

                return -1;
            }

            if (strReceived[1].Contains("OK") == false)
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {

                    OutputLogVM.AddLineOutputLog("Received Error From PID.", (int)ERROR_CODE.LABEL_FAIL);
                });

                string strString = "Timeout when Reading Data from PID. Press 'Continue' to resend or 'Abort' to end sequence.";
                int nSelect = WaitForNextStepSequenceEvent(strString, true);

                if (nSelect == (int)SEQUENCE_OPTION.SEQUENCE_IMIDIATE_BUTTON_CONTINUE)
                    goto SendDataToClient;
                return -1;
            }


            SerialCommunication.WriteData("DATA_OK");

            SerialCommunication.m_SerialDataReceivedEvent.Reset();
            nTimeout = 0;
            while (SerialCommunication.m_SerialDataReceivedEvent.WaitOne(100) == false)
            {
                if (m_bMachineNotReadyNeedToReset)
                {
                    m_bRobotSequenceStatus = false;
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                       MainWindowVM.btn_runChecked = false;
                       MainWindowVM.m_bSequenceRunning = false;
                    });
                    return -1;
                }

               WarningMessageBoxVM.popupWarningMessageBoxDelegate?.Invoke("Waiting 'ACK' from PID...", WARNINGMESSAGE.MESSAGE_INFORMATION);

                nTimeout += 100;
                if (nTimeout >= 50000)
                {
                    nTimeout = 0;

                    string strString = "Timeout when Reading Data from PID. Press 'Continue' to resend or 'Abort' to end sequence.";
                    int nSelect = WaitForNextStepSequenceEvent(strString, true);

                    if (nSelect == (int)SEQUENCE_OPTION.SEQUENCE_IMIDIATE_BUTTON_CONTINUE)
                        goto SendDataToClient;
                    else
                        return -1;
                }
            }
           WarningMessageBoxVM.popupWarningMessageBoxDelegate?.Invoke("Waiting 'ACK' from PID...", WARNINGMESSAGE.MESSAGE_INFORMATION, false);

            SerialCommunication.m_SerialDataReceivedEvent.Reset();
            string strReceived_ACK = SerialCommunication.ReadData();

            if (strReceived_ACK.Contains("ACK") == false)
            {
                string strString = "Received ACK_Error From Client. End Sequence. Please Check the PID machine then open Serial Communication Dialog to manually send lot data to PID.";
                WaitForNextStepSequenceEvent(strString, true);
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {

                    OutputLogVM.AddLineOutputLog("Received ACK_Error From Client. End Sequence.", (int)ERROR_CODE.LABEL_FAIL);
                });

                return -1;
            }

            //SerialCommunication.WriteData("ACK_OK");
            return 0;
        }


        void RobotSequence()
        {

        _Start_Lot:
            if (HiWinRobotInterface.m_RobotConnectID < 0)
            {
                MessageBox.Show("ROBOT is not available. Please check the connection!");
                return;
            }

            int nCamera1InspectionResult = -(int)ERROR_CODE.NOT_INSPECTED;
            VisionResultData visionResultDataTemp = new VisionResultData();
            int nCurrentSequenceStep = 0;
            string strLotID = AppMagnus.m_strCurrentLot;
            System.Drawing.PointF robotPoint = new System.Drawing.PointF(0, 0);
            // feedback running to plc


            int nError = 0;

            // Home Move // init OutPut
            if (ResetSequence() < 0)
            {
                WaitForNextStepSequenceEvent(" Reset Machine Failed. End Sequence...", true);
                return;
            }

            while (RobotIOStatus.m_RunMachineStatus == 1)
            {
                LogMessage.WriteToDebugViewer(9, "Waiting for PLC Running Sequence Signal Off...");
                if (!MainWindowVM.m_bSequenceRunning && !InspectionTabVM.bEnableOfflineInspection || RobotIOStatus.m_EmergencyStatus == 1 || RobotIOStatus.m_EndLotStatus == 1)
                    return;

                Thread.Sleep(500);
            }



            while (HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.PLC_PACKING_PROCESS_READY) != 1)
            {
                // Need popup Dialog if waiting too long 
                if (!MainWindowVM.m_bSequenceRunning && !InspectionTabVM.bEnableOfflineInspection || RobotIOStatus.m_EmergencyStatus == 1 || RobotIOStatus.m_EndLotStatus == 1)
                    return;

                LogMessage.WriteToDebugViewer(9, "Waiting for PLC ready Signal....");
                Thread.Sleep(500);
            }

            m_plcComm.WritePLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_ROBOT_RUNNING_STATUS, 1);

            int nRetry = 0;
        StartSequence:
            m_plcComm.WritePLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_ROBOT_RESULT, 0);

            HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_READY_CONVEYOR_ON, true);
            //Wait for Signal from PLC when they ready for new Lot so we can create new lot ID
            //HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_READY_CONVEYOR_ON, false);

            nError = WaitForNextStepSequenceEvent("Begin Sequence: Press Next to trigger camera 1");
            if (nError != (int)SEQUENCE_OPTION.SEQUENCE_CONTINUE || RobotIOStatus.m_EndLotStatus == 1 )
                return;

            //Application.SetIntRegistry(Application.m_strCurrentDeviceID_Registry[0], m_Tracks[0].m_CurrentSequenceDeviceID);

            m_nWaitEventInspectionOnlineThreadStatus = 0;
            if (func_CameraTriggerThread() < 0 || RobotIOStatus.m_EndLotStatus == 1 || m_SequenceMode_With_Or_No_Robot == 1)
            {
                return;
            }
            //Thread.Sleep(500);
            //m_hardwareTriggerSnapEvent[0].Set();

            Stopwatch timeIns = new Stopwatch();
            Stopwatch timeIns_fullSequence = new Stopwatch();
            timeIns_fullSequence.Start();
            timeIns.Start();

            //int nChipCount = 0;
            while (MainWindowVM.m_bSequenceRunning || InspectionTabVM.bEnableOfflineInspection || m_bMachineNotReadyNeedToReset)
            {

                if (RobotIOStatus.m_EndLotStatus == 1 || m_SequenceMode_With_Or_No_Robot == 1)
                    return;

                LogMessage.WriteToDebugViewer(9, "Begin Sequence");
                // Step 1: wait for station 1 inspection
                timeIns.Restart();
                nCamera1InspectionResult = -(int)ERROR_CODE.NOT_INSPECTED;

                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Waiting for vision result...");

                while (m_nWaitEventInspectionOnlineThreadStatus == 0)
                {

                    if (!MainWindowVM.m_bSequenceRunning || m_bMachineNotReadyNeedToReset || RobotIOStatus.m_EndLotStatus == 1 || m_SequenceMode_With_Or_No_Robot == 1)
                    {
                        return;
                    }

                    if (RobotIOStatus.m_RunMachineStatus == 1)
                    {

                        System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                        {

                            OutputLogVM.AddLineOutputLog("PLC Button Run is pressed. Restarting the Robot sequence....", (int)ERROR_CODE.LABEL_FAIL);
                        });
                        Thread.Sleep(3000);
                        goto _Start_Lot;
                    }
                }

                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Waiting for vision result Done!");


                if (RobotIOStatus.m_RunMachineStatus == 1)
                {

                    System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                    {

                        OutputLogVM.AddLineOutputLog("PLC Button Run is pressed. Restarting the Robot sequence....", (int)ERROR_CODE.LABEL_FAIL);
                    });
                    Thread.Sleep(3000);
                    goto _Start_Lot;
                }

                Thread.Sleep(10);
                if (m_nWaitEventInspectionOnlineThreadStatus > 0)
                    nCamera1InspectionResult = m_Tracks[0].m_InspectionOnlineThreadVisionResult.m_nResult;
                else
                    nCamera1InspectionResult = -(int)ERROR_CODE.NO_PATTERN_FOUND;


                if (nCamera1InspectionResult == -(int)ERROR_CODE.NO_PATTERN_FOUND) // If no pattern found, popup Error 
                {
                    nRetry++;

                    LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Station 1 inspection No Device Found ({timeIns.ElapsedMilliseconds} ms)"); timeIns.Restart();
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                    {

                        OutputLogVM.AddLineOutputLog("Device not Found. Image processing Failed!.", (int)ERROR_CODE.LABEL_FAIL);
                    });


                    if (nRetry < 1)
                        goto StartSequence;

                    nRetry = 0;
                    m_plcComm.WritePLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_ROBOT_RESULT, -1);
                    HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_AIR_ON, false);
                    HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_AIR_OFF, false);

                    nError = WaitForNextStepSequenceEvent("Inspection Failed (Device Not found)! Please press 'RUN' on UI or PLC  to restart the sequence!", true);
                    if (nError == (int)SEQUENCE_OPTION.SEQUENCE_ABORT || RobotIOStatus.m_EndLotStatus == 1 )
                        return;
                    goto StartSequence;
                    //return;
                }

                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Station 1 Inspection done  ({timeIns.ElapsedMilliseconds} ms)"); timeIns.Restart();
                nError = WaitForNextStepSequenceEvent("Inspection done. Press Next to Move to  pre pick position");
                if (nError != (int)SEQUENCE_OPTION.SEQUENCE_CONTINUE)
                    goto Check_StepSequence;



                _Step_1:
                nCurrentSequenceStep = 1;

            _Step_2:
                nCurrentSequenceStep = 2;
            //LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Move to Pre Pick position");

            _Step_3:
                nCurrentSequenceStep = 3;
                //LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Move to Pre Pick position");

                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Move to Pre Pick position");

                if (m_bMachineNotReadyNeedToReset || RobotIOStatus.m_EndLotStatus == 1)
                    return;

                visionResultDataTemp.m_nDeviceIndexOnReel = m_Tracks[0].m_InspectionOnlineThreadVisionResult.m_nDeviceIndexOnReel;
                visionResultDataTemp.m_strDeviceID = m_Tracks[0].m_InspectionOnlineThreadVisionResult.m_strDeviceID;
                visionResultDataTemp.m_nResult = nCamera1InspectionResult;
                visionResultDataTemp.m_strFullImagePath = m_Tracks[0].m_InspectionOnlineThreadVisionResult.m_strFullImagePath;
                visionResultDataTemp.m_strDatetime = m_Tracks[0].m_InspectionOnlineThreadVisionResult.m_strDatetime;

                robotPoint = MagnusMatrix.ApplyTransformation(HIKRobotVM.m_MatCameraRobotTransform, m_Tracks[0].m_Center_Vision);
               MainWindowVM.master.m_hiWinRobotInterface.MoveTo_PRE_PICK_POSITION(robotPoint, -m_Tracks[0].m_dDeltaAngleInspection);
               MainWindowVM.master.m_hiWinRobotInterface.wait_for_stop_motion();
                nError = WaitForNextStepSequenceEvent("Move to Pre Pick position done. Press Next to turn on vaccum");
                if (nError != (int)SEQUENCE_OPTION.SEQUENCE_CONTINUE)
                    goto Check_StepSequence;

                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Move to Pre Pick position Done ({timeIns.ElapsedMilliseconds} ms)"); timeIns.Restart();

            _Step_4:
                nCurrentSequenceStep = 4;
                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Turn on vaccum");


                if (m_bMachineNotReadyNeedToReset || RobotIOStatus.m_EndLotStatus == 1)
                    return;
                // Turn on vaccum

                HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_AIR_ON, true);
                HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_AIR_OFF, false);

                nError = WaitForNextStepSequenceEvent("Press Next to Pick the device");

                if (nError != (int)SEQUENCE_OPTION.SEQUENCE_CONTINUE)
                    goto Check_StepSequence;

                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Turn on vaccum Done ({timeIns.ElapsedMilliseconds} ms)"); timeIns.Restart();

            _Step_5:
                nCurrentSequenceStep = 5;
                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Move to Pick position (move Down Z motor)");

                //System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                //{
                //    OutputLogVM.AddLineOutputLog($"Current Step {nCurrentSequenceStep} ");
                //});

                if (m_bMachineNotReadyNeedToReset || RobotIOStatus.m_EndLotStatus == 1)
                    return;
                // Move to Pick position (move Down Z motor)

               MainWindowVM.master.m_hiWinRobotInterface.MoveTo_PICK_POSITION(robotPoint, -m_Tracks[0].m_dDeltaAngleInspection);
               MainWindowVM.master.m_hiWinRobotInterface.wait_for_stop_motion();

                // Move to  Pre pick  position again (move Up Z motor)
                // From now, if Air PressureStatus signal is 0, we consider it as the chip has been through, so we arlamp it
                nError = WaitForNextStepSequenceEvent("Press Next to move up to pre pick position");
                if (nError != (int)SEQUENCE_OPTION.SEQUENCE_CONTINUE)
                    goto Check_StepSequence;

                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Move to Pick position (move Down Z motor) Done ({timeIns.ElapsedMilliseconds} ms)"); timeIns.Restart();

            _Step_6:
                nCurrentSequenceStep = 6;
                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Move To Pre Pick Pos");

                if (m_bMachineNotReadyNeedToReset || RobotIOStatus.m_EndLotStatus == 1)
                    return;
               MainWindowVM.master.m_hiWinRobotInterface.MoveTo_PRE_PICK_POSITION(robotPoint, -m_Tracks[0].m_dDeltaAngleInspection);
               MainWindowVM.master.m_hiWinRobotInterface.wait_for_stop_motion();

                nError = WaitForNextStepSequenceEvent("Move to prepick position done. Press Next to turn on conveyor and waiting for Air Pressure Signal");
                if (nError != (int)SEQUENCE_OPTION.SEQUENCE_CONTINUE)
                    goto Check_StepSequence;
                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Move to prepick position done. ");

            _Step_7:
                nCurrentSequenceStep = 7;
                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Set ConveyOr On, Waiting for Air Pressure");

                //Trigger conveyor on
                HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_READY_CONVEYOR_ON, true);
                //
                int nCountSleep = 0;
                m_plcComm.WritePLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_AIR_PRESS_RESULT, 0);
                int nStatus = HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.AIR_PRESSURESTATUS);

                while (nStatus == 0)
                {
                    if (!MainWindowVM.m_bSequenceRunning && !InspectionTabVM.bEnableOfflineInspection || RobotIOStatus.m_EndLotStatus == 1)
                    {
                        return;
                    }

                    nCountSleep++;
                    if (nCountSleep > 300)
                    {

                        m_plcComm.WritePLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_AIR_PRESS_RESULT, -1);
                        HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_AIR_ON, false);
                        HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_AIR_OFF, false);

                        System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                        {

                            OutputLogVM.AddLineOutputLog($"Air Pressure got problem! {nStatus}", (int)ERROR_CODE.LABEL_FAIL);
                        });

                        //WaitForNextStepSequenceEvent("Air Pressure got problem!",true);
                        return;
                    }
                    Thread.Sleep(10);
                    nStatus = HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.AIR_PRESSURESTATUS);
                }

                nError = WaitForNextStepSequenceEvent("Move to prepick position done. Press Next to move up to pre place position");
                if (nError != (int)SEQUENCE_OPTION.SEQUENCE_CONTINUE)
                    goto Check_StepSequence;

                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Move to  Pre pick  position Done ({timeIns.ElapsedMilliseconds} ms)");

            _Step_8:
                nCurrentSequenceStep = 8;
                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Move To Pass fail position");

                if (m_bMachineNotReadyNeedToReset || RobotIOStatus.m_EndLotStatus == 1)
                    return;
                // Move To Pass fail position
                //nCamera1InspectionResult = (int)ERROR_CODE.PASS;
                string strPrePosition;
                if (nCamera1InspectionResult == -(int)ERROR_CODE.PASS)
                    strPrePosition = SequencePointData.PRE_PASS_PLACE_POSITION;
                else if (nCamera1InspectionResult == -(int)ERROR_CODE.OPPOSITE_CHIP)
                    strPrePosition = SequencePointData.PRE_FAILED_BLACK_PLACE_POSITION;
                else
                    strPrePosition = SequencePointData.PRE_FAILED_PLACE_POSITION;

               MainWindowVM.master.m_hiWinRobotInterface.MoveTo_STATIC_POSITION(strPrePosition);
               MainWindowVM.master.m_hiWinRobotInterface.wait_for_stop_motion();
                // Trigger camera  1 

                nError = WaitForNextStepSequenceEvent("Move To Pass fail position Done. Press Next to trigger camera 1");
                if (nError != (int)SEQUENCE_OPTION.SEQUENCE_CONTINUE)
                    goto Check_StepSequence;

                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Move To Pass fail position Done ({timeIns.ElapsedMilliseconds} ms)"); timeIns.Restart();


            _Step_9:
                nCurrentSequenceStep = 9;
                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Trigger Camera 1");

                if (m_bMachineNotReadyNeedToReset || RobotIOStatus.m_EndLotStatus == 1)
                    return;

                // If not yet trigger camera 1, trigger again
                if (m_StartWaitPLCReadyToTriggerCameraThread[0] == null || !m_StartWaitPLCReadyToTriggerCameraThread[0].IsAlive)
                {
                    m_nWaitEventInspectionOnlineThreadStatus = 0;
                    m_StartWaitPLCReadyToTriggerCameraThread[0] = new System.Threading.Thread(new System.Threading.ThreadStart(() => func_CameraTriggerThread(nCurrentSequenceStep, 0)));
                    //m_StartWaitPLCReadyToTriggerCameraThread[0].IsBackground = true;
                    m_StartWaitPLCReadyToTriggerCameraThread[0].Start();
                }

                nError = WaitForNextStepSequenceEvent("Set Trigger camera 1 done, Press Next to Wait for PLC Ready to Place");
                if (nError != (int)SEQUENCE_OPTION.SEQUENCE_CONTINUE)
                    goto Check_StepSequence;

                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Trigger Camera 1 Done");


            _Step_10:
                nCurrentSequenceStep = 10;
                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Wait For PLC Ready ");
                if (m_bMachineNotReadyNeedToReset || RobotIOStatus.m_EndLotStatus == 1)
                    return;
                // Wait For PLC 2 Ready
                //m_plcComm.WritePLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_ROBOT_RESULT, nCamera1InspectionResult);


                if (nCamera1InspectionResult == -(int)ERROR_CODE.PASS)
                {
                    HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_PLACE_DONE, false);
                    LogMessage.WriteToDebugViewer(9, $"{AppMagnus.LineNumber()}: {AppMagnus.PrintCallerName()}");
                    while (HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.PLC_ALLOW_TO_PLACE) != 1)
                    {
                        if (!MainWindowVM.m_bSequenceRunning && !InspectionTabVM.bEnableOfflineInspection || RobotIOStatus.m_EndLotStatus == 1)
                        {
                            return;
                        }

                        if (RobotIOStatus.m_RunMachineStatus == 1)
                        {

                            System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                            {

                                OutputLogVM.AddLineOutputLog("PLC Button Run is pressed. Restarting the Robot sequence....", (int)ERROR_CODE.LABEL_FAIL);
                            });
                            Thread.Sleep(3000);
                            goto _Start_Lot;
                        }
                        Thread.Sleep(10);
                    }
                }

                nError = WaitForNextStepSequenceEvent("wait for plc ready signal done. Press Next to Put device to the tray");
                if (nError != (int)SEQUENCE_OPTION.SEQUENCE_CONTINUE)
                    goto Check_StepSequence;

                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Wait For PLC Ready  Done ({timeIns.ElapsedMilliseconds} ms)"); timeIns.Restart();


            _Step_11:
                nCurrentSequenceStep = 11;
                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Put device to the tray");

                if (m_bMachineNotReadyNeedToReset || RobotIOStatus.m_EndLotStatus == 1)
                    return;
                /////////////////Step 9: put device to the tray (turn off vaccum)
                HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_AIR_ON, false);
                HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_AIR_OFF, true);


                if (nCamera1InspectionResult == -(int)ERROR_CODE.PASS)
                    Thread.Sleep(50);
                else
                    Thread.Sleep(150);

                HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_AIR_ON, false);
                HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_AIR_OFF, false);
                //End 30 End sequencd if pass

                int nDeviceID = m_Tracks[0].m_CurrentSequenceDeviceID;
                m_Tracks[0].m_VisionResultDatas[nDeviceID].m_nDeviceIndexOnReel = nDeviceID;
                m_Tracks[0].m_VisionResultDatas[nDeviceID].m_strDeviceID = (nDeviceID + 1).ToString();
                m_Tracks[0].m_VisionResultDatas[nDeviceID].m_nResult = nCamera1InspectionResult;
                m_Tracks[0].m_VisionResultDatas[nDeviceID].m_strFullImagePath = visionResultDataTemp.m_strFullImagePath;
                m_Tracks[0].m_VisionResultDatas[nDeviceID].m_strDatetime = visionResultDataTemp.m_strDatetime;


                if (nCamera1InspectionResult == -(int)ERROR_CODE.PASS)
                {
                    if (RobotIOStatus.m_IsLastChipStatus > 0)
                    {

                        if (MainWindowVM.m_bSequenceRunning)
                        {

                            lock (m_UpdateResultQueue[0])
                            {
                                m_UpdateResultQueue[0].Enqueue(m_Tracks[0].m_VisionResultDatas[nDeviceID]);
                            }
                        }


                        LogMessage.WriteToDebugViewer(9, $" Check Last Chip Status to Move or wait: {RobotIOStatus.m_IsLastChipStatus}");
                        HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_PLACE_DONE, true);
                        RobotIOStatus.m_IsLastChipStatus = 0;
                        goto _Start_Lot;
                    }

                    //get Chip signal at end 29
                    RobotIOStatus.m_IsLastChipStatus = HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.PLC_LAST_CHIP);
                    HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_PLACE_DONE, true);

                    //PLC start trigger last chip at begin 29
                }




                if (MainWindowVM.m_bSequenceRunning)
                {

                    lock (m_UpdateResultQueue[0])
                    {
                        m_UpdateResultQueue[0].Enqueue(m_Tracks[0].m_VisionResultDatas[nDeviceID]);
                    }

                    if (m_Tracks[0].m_VisionResultDatas[nDeviceID].m_nResult == -(int)ERROR_CODE.PASS)
                    {
                        m_Tracks[0].m_CurrentSequenceDeviceID++;
                    }
                }





                //if(nEmptyTrayHoles >= 5)
                //    m_hardwareTriggerSnapEvent[1].Set();

                nError = WaitForNextStepSequenceEvent("Put device to the tray Done. Press next to Move to Ready position");
                if (nError != (int)SEQUENCE_OPTION.SEQUENCE_CONTINUE)
                    goto Check_StepSequence;

                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Put device to the tray Done ({timeIns.ElapsedMilliseconds} ms)"); timeIns.Restart();


            _Step_12:
                nCurrentSequenceStep = 12;

                if (m_bMachineNotReadyNeedToReset || RobotIOStatus.m_EndLotStatus == 1)
                    return;

                _Step_13:
                nCurrentSequenceStep = 13;
                if (m_bMachineNotReadyNeedToReset || RobotIOStatus.m_EndLotStatus == 1)
                    return;

                _Step_14:
                nCurrentSequenceStep = 14;

                LogMessage.WriteToDebugViewer(9, $"Step {nCurrentSequenceStep} : Turn of Air. End Sequence");

                if (m_bMachineNotReadyNeedToReset || RobotIOStatus.m_EndLotStatus == 1)
                    return;
                // Move to ready position and turn off vaccum

                if (HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.PLC_CHIPFOUND) != 1)
                {
                   MainWindowVM.master.m_hiWinRobotInterface.MoveTo_STATIC_POSITION(HiWinRobotInterface.SequencePointData.READY_POSITION);
                }

                HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_AIR_ON, false);
                HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_AIR_OFF, false);
               MainWindowVM.master.m_hiWinRobotInterface.wait_for_stop_motion();

                nError = WaitForNextStepSequenceEvent("Sequence completed! Press Next to continue sequence...");
                if (nError != (int)SEQUENCE_OPTION.SEQUENCE_CONTINUE)
                    goto Check_StepSequence;



                //if (nCamera1InspectionResult == -(int)ERROR_CODE.PASS)
                //    m_Tracks[0].m_CurrentSequenceDeviceID++;

                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    OutputLogVM.AddLineOutputLog($"Total Sequence time {timeIns_fullSequence.ElapsedMilliseconds} (ms): ");
                });
                LogMessage.WriteToDebugViewer(9, $"End sequence. Total time: ({timeIns_fullSequence.ElapsedMilliseconds} ms)");
                timeIns_fullSequence.Restart();

            Check_StepSequence:
                if (nError != (int)SEQUENCE_OPTION.SEQUENCE_CONTINUE)
                {
                    switch (nError)
                    {
                        case (int)SEQUENCE_OPTION.SEQUENCE_ABORT:
                            System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                OutputLogVM.AddLineOutputLog($"Current Step {nCurrentSequenceStep} ");
                            });
                            return;

                        //case (int)SEQUENCE_OPTION.SEQUENCE_IMIDIATE_BUTTON_CONTINUE:
                        //    break;

                        //case (int)SEQUENCE_OPTION.SEQUENCE_GOBACK:
                        //    nCurrentSequenceStep--;
                        //    break;

                        //case (int)SEQUENCE_OPTION.SEQUENCE_RETRY:
                        //    nCurrentSequenceStep = 0;
                        //    break;

                        //case (int)SEQUENCE_OPTION.SEQUENCE_CONTINUE:
                        //    break;

                        default:
                            break;
                    }

                    switch (nCurrentSequenceStep)
                    {
                        case 0:
                            goto _Step_14;
                        case 1:
                            goto _Step_1;
                        case 2:
                            goto _Step_2;
                        case 3:
                            goto _Step_3;
                        case 4:
                            goto _Step_4;
                        case 5:
                            goto _Step_5;
                        case 6:
                            goto _Step_6;
                        case 7:
                            goto _Step_7;
                        case 8:
                            goto _Step_8;
                        case 9:
                            goto _Step_9;
                        case 10:
                            goto _Step_10;
                        case 11:
                            goto _Step_11;
                        case 12:
                            goto _Step_12;
                        case 13:
                            goto _Step_13;
                        case 14:
                            goto _Step_14;

                    }
                }

            }

        }

        int m_nWaitEventInspectionOnlineThreadStatus = 0;
        public int func_CameraTriggerThread(int nCurrentSequenceStep = -1, int nTrack = 0)
        {
            //int nError;
            LogMessage.WriteToDebugViewer(9, $"Waiting for Chip found Signal from PLC");
            while (HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.PLC_CHIPFOUND) != 1)
            {

                if (!MainWindowVM.m_bSequenceRunning && !InspectionTabVM.bEnableOfflineInspection || RobotIOStatus.m_EmergencyStatus == 1)
                {
                    return -1;
                }
                Thread.Sleep(10);
            }
            LogMessage.WriteToDebugViewer(9, $"Set PLC Conveyor ({(int)OUTPUT_IOROBOT.ROBOT_READY_CONVEYOR_ON} ON");
            HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.ROBOT_READY_CONVEYOR_ON, false);
            LogMessage.WriteToDebugViewer(9, $"Set Camera Trigger Event ");
            m_EventInspectionOnlineThreadDone[0].Reset();
            m_hardwareTriggerSnapEvent[0].Set();
            Stopwatch timeIns_fullSequence = new Stopwatch();
            int m_nTimeOut = 0;
            while (!m_EventInspectionOnlineThreadDone[0].WaitOne(10))
            {
                m_nTimeOut++;
                if (m_nTimeOut > 500)
                {
                    break;
                }
            }
            m_EventInspectionOnlineThreadDone[0].Reset();

            if (m_nTimeOut > 500)
                m_nWaitEventInspectionOnlineThreadStatus = -1;
            else
                m_nWaitEventInspectionOnlineThreadStatus = 1;
            return 0;
        }
        internal void BarcodeReaderSequenceThread()
        {


            if (thread_BarcodeReaderSequence == null)
            {
                thread_BarcodeReaderSequence = new System.Threading.Thread(new System.Threading.ThreadStart(() => func_BarcodeReaderSequence()));
                //thread_BarcodeReaderSequence.IsBackground = true;
                thread_BarcodeReaderSequence.Start();
            }
            else if (!thread_BarcodeReaderSequence.IsAlive)
            {
                thread_BarcodeReaderSequence = new System.Threading.Thread(new System.Threading.ThreadStart(() => func_BarcodeReaderSequence()));
                //thread_BarcodeReaderSequence.IsBackground = true;
                thread_BarcodeReaderSequence.Start();
            }
        }

        public bool m_bBarcodeReaderSequenceStatus = false;
        void func_BarcodeReaderSequence()
        {

            //while (HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.PLC_PACKING_PROCESS_READY) == 0)
            //{
            //    // Need popup Dialog if waiting too long 
            //    if (!MainWindow.mainWindow.m_bSequenceRunning && !MainWindow.mainWindow.bEnableOfflineInspection || RobotIOStatus.m_EmergencyStatus > 0)
            //        return;
            //    LogMessage.WriteToDebugViewer(8, "Waiting for PLC ready Signal....");
            //    Thread.Sleep(500);
            //}

            LogMessage.WriteToDebugViewer(9, "Run Barcode Sequence Thread....");

            BarcodeReaderSequence_New();
            m_bBarcodeReaderSequenceStatus = false;
        }

        public bool m_bBarcodeBusy = false;
        void BarcodeReaderSequence_New()
        {
        startBarcodeSequence:
            if (m_SequenceMode == (int)SEQUENCE_MODE.MODE_AUTO)
            {
                HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.BARCODE_RESULT_PASS, false);
                HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.BARCODE_RESULT_FAIL, false);
            }
            else
            {
                m_plcComm.WritePLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_MANUAL_BARCODE_RESULT_PASS, 0);
                m_plcComm.WritePLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_MANUAL_BARCODE_RESULT_FAIL, 0);
            }

            //int nTimeout = 0;
            if (m_plcComm == null)
                goto EndThread;

            m_plcComm.WritePLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_BARCODE_READY, 1);

            while (true/*MainWindow.mainWindow.m_bSequenceRunning*/)
            {
                if (m_SequenceMode == (int)SEQUENCE_MODE.MODE_AUTO)
                {
                    LogMessage.WriteToDebugViewer(8, $"Barcode Reader: Wait PLC Barcode Trigger {(int)INPUT_IOROBOT.PLC_BARCODE_TRIGGER}!   Device {m_Tracks[1].m_CurrentSequenceDeviceID}");
                    //LogMessage.WriteToDebugViewer(8, $"Barcode Reader: Scan Done!  Device {m_Tracks[1].m_CurrentSequenceDeviceID}");

                    HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.BARCODE_CAPTURE_BUSY, false);
                    m_bBarcodeBusy = false;
                    int nTimeOut = 0;
                    while (HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.PLC_BARCODE_TRIGGER) != 1)
                    {
                        if (m_SequenceMode == (int)SEQUENCE_MODE.MODE_MANUAL)
                            goto startBarcodeSequence;

                        //if (MainWindow.mainWindow == null || !MainWindow.m_IsWindowOpen)
                        //    goto EndThread;
                        Thread.Sleep(30);
                        nTimeOut++;
                        if (nTimeOut > 10)
                        {
                            nTimeOut = 0;
                            HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.BARCODE_CAPTURE_BUSY, false);
                        }
                        //if (m_RunMachineStatus == 1)
                        //{

                        //    System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                        //    {

                        //        OutputLogVM.AddLineOutputLog("PLC Button Run is pressed. Restarting the Barcode sequence....", (int)ERROR_CODE.LABEL_FAIL);
                        //    });
                        //    Thread.Sleep(3000);
                        //    goto startBarcodeSequence;
                        //}

                    }
                }
                else
                {
                    LogMessage.WriteToDebugViewer(8, $"Barcode Reader: Wait PLC Barcode Trigger {(int)PLCCOMM.PLC_ADDRESS.PLC_MANUAL_BARCODE_TRIGGER}!   Device {m_Tracks[1].m_CurrentSequenceDeviceID}");
                    while (m_plcComm.ReadPLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_MANUAL_BARCODE_TRIGGER) != 1)
                    {
                        //if (MainWindow.mainWindow == null || !MainWindow.m_IsWindowOpen)
                        //    goto EndThread;
                        Thread.Sleep(100);
                    }
                    m_plcComm.WritePLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_MANUAL_BARCODE_TRIGGER, 0);
                }




                if (m_SequenceMode == (int)SEQUENCE_MODE.MODE_AUTO)
                {
                    HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.BARCODE_RESULT_PASS, false);
                    HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.BARCODE_RESULT_FAIL, false);
                }
                else
                {
                    int[] nvalue = { 0, 0 };
                    m_plcComm.WritePLCMultiRegister((int)PLCCOMM.PLC_ADDRESS.PLC_MANUAL_BARCODE_RESULT_PASS, nvalue);
                }


                int nDeviceTemp = m_plcComm.ReadPLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_CURRENT_BARCODE_CHIP_COUNT);
                LogMessage.WriteToDebugViewer(8, $"Barcode Reader: Read Barcode  Device {nDeviceTemp}");

                if (nDeviceTemp > 0)
                    m_Tracks[1].m_CurrentSequenceDeviceID = nDeviceTemp;

                LogMessage.WriteToDebugViewer(8, $"Barcode Reader: Read Barcode  Device {m_Tracks[1].m_CurrentSequenceDeviceID}");

                int nDeviceID = m_Tracks[1].m_CurrentSequenceDeviceID;

                LogMessage.WriteToDebugViewer(8, $"Barcode Reader: Set BARCODE_CAPTURE_BUSY {(int)OUTPUT_IOROBOT.BARCODE_CAPTURE_BUSY} ON   Device {m_Tracks[1].m_CurrentSequenceDeviceID}");
                m_bBarcodeBusy = true;

                if (m_SequenceMode == (int)SEQUENCE_MODE.MODE_AUTO)
                    HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.BARCODE_CAPTURE_BUSY, true);
                else
                    m_plcComm.WritePLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_MANUAL_BARCODE_CAPTURE_BUSY, 1);



                LogMessage.WriteToDebugViewer(7 + 1, $"Get barcode  Device {m_Tracks[1].m_CurrentSequenceDeviceID} ");
                string strFullPathImageOut;
                string strBarcodeResult = m_BarcodeReader.GetBarCodeStringAndImage(out strFullPathImageOut, nDeviceID, AppMagnus.m_strCurrentLot);
                LogMessage.WriteToDebugViewer(7 + 1, "Get barcode Done ");


                LogMessage.WriteToDebugViewer(8, $"Barcode Reader: Waiting for vision done....  Device {m_Tracks[1].m_CurrentSequenceDeviceID} ");
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {

                    m_Tracks[1].m_InspectionCore.LoadImageToInspection(m_Tracks[1].m_imageViews[0].btmSource);

                });

                System.Drawing.PointF pCenter = new System.Drawing.PointF(0, 0);
                System.Drawing.PointF pCorner = new System.Drawing.PointF(0, 0);
                m_Tracks[1].m_VisionResultDatas[nDeviceID].m_nResult = m_Tracks[1].Inspect(out pCenter, out pCorner, false);


                if (strBarcodeResult.Contains("Dummy") || strBarcodeResult.Length < 1)
                {
                    m_Tracks[1].m_VisionResultDatas[nDeviceID].m_nResult = -(int)ERROR_CODE.LABEL_FAIL;
                }

                m_Tracks[1].m_VisionResultDatas[nDeviceID].m_nDeviceIndexOnReel = nDeviceID;
                m_Tracks[1].m_VisionResultDatas[nDeviceID].m_strDeviceID = strBarcodeResult;
                m_Tracks[1].m_VisionResultDatas[nDeviceID].m_strFullImagePath = strFullPathImageOut;
                string strDateTime = string.Format("{0}:{1}:{2}_{3}:{4}:{5}", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"), DateTime.Now.ToString("HH"), DateTime.Now.ToString("mm"), DateTime.Now.ToString("ss"));
                m_Tracks[1].m_VisionResultDatas[nDeviceID].m_strDatetime = strDateTime;
                //if (RobotIOStatus.m_EmergencyStatus == 1)
                //{
                //    System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                //    {
                //        OutputLogVM.AddLineOutputLog("Barcode Reader Sequence Ended!", (int)ERROR_CODE.LABEL_FAIL);

                //    });
                //    continue;

                //}

                LogMessage.WriteToDebugViewer(8, $"Barcode Reader: Vision Done!  Device {m_Tracks[1].m_CurrentSequenceDeviceID}");
                if (nDeviceID > 0)
                    if (m_Tracks[1].m_VisionResultDatas[nDeviceID].m_strDeviceID == m_Tracks[1].m_VisionResultDatas[nDeviceID - 1].m_strDeviceID
                        && m_Tracks[1].m_VisionResultDatas[nDeviceID - 1].m_nResult == m_Tracks[1].m_VisionResultDatas[nDeviceID].m_nResult)
                    {
                        m_Tracks[1].m_VisionResultDatas[nDeviceID].m_nResult = -(int)ERROR_CODE.PROCESS_ERROR;
                        string strFullPathImageOutTemp = m_Tracks[1].m_VisionResultDatas[nDeviceID].m_strFullImagePath;
                        string strFailImage = strFullPathImageOutTemp.Replace("PASS IMAGE", "FAIL IMAGE");
                        if (File.Exists(strFullPathImageOutTemp) && !File.Exists(strFailImage))
                            File.Move(strFullPathImageOutTemp, strFailImage);
                        m_Tracks[1].m_VisionResultDatas[nDeviceID].m_strFullImagePath = strFailImage;
                        System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            OutputLogVM.AddLineOutputLog($"Barcode Reader: Scanned same device. Result FAILED!  Device {m_Tracks[1].m_CurrentSequenceDeviceID}", (int)ERROR_CODE.LABEL_FAIL);

                        });
                    }

                bool bFailSendToPLC = false;
                if (m_Tracks[1].m_VisionResultDatas[nDeviceID].m_nResult != -(int)ERROR_CODE.PASS)
                {

                    bFailSendToPLC = true;
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        OutputLogVM.AddLineOutputLog($"Barcode Reader: Result FAILED!  Device {m_Tracks[1].m_CurrentSequenceDeviceID}", (int)ERROR_CODE.LABEL_FAIL);

                    });
                }

                LogMessage.WriteToDebugViewer(8, $"Barcode Reader: Send result to PLC {(int)PLCCOMM.PLC_ADDRESS.PLC_BARCODE_RESULT}  . Result: {bFailSendToPLC}    Device {m_Tracks[1].m_CurrentSequenceDeviceID}");
                m_plcComm.WritePLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_BARCODE_RESULT, m_Tracks[1].m_VisionResultDatas[nDeviceID].m_nResult);

                if (m_SequenceMode == (int)SEQUENCE_MODE.MODE_AUTO)
                {
                    int nSttusss = HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.PLC_BARCODE_TRIGGER);
                    if (nSttusss > 1)
                    {
                        LogMessage.WriteToDebugViewer(8, $"Barcode Reader: Robot Disconnect. Input {(int)INPUT_IOROBOT.PLC_BARCODE_TRIGGER}  status = {nSttusss}  Device {m_Tracks[1].m_CurrentSequenceDeviceID}");
                        Thread.Sleep(1000);
                        goto startBarcodeSequence;
                    }
                    HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.BARCODE_RESULT_PASS, !bFailSendToPLC);
                    HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.BARCODE_RESULT_FAIL, bFailSendToPLC);
                    HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, (int)OUTPUT_IOROBOT.BARCODE_CAPTURE_BUSY, false);
                }
                else
                {
                    m_plcComm.WritePLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_MANUAL_BARCODE_RESULT_PASS, bFailSendToPLC == false ? 1 : 0);
                    m_plcComm.WritePLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_MANUAL_BARCODE_RESULT_FAIL, bFailSendToPLC == false ? 0 : 1);
                    m_plcComm.WritePLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_MANUAL_BARCODE_CAPTURE_BUSY, 0);
                }
                LogMessage.WriteToDebugViewer(8, $"Barcode Reader: Send result to PLC Done {(int)PLCCOMM.PLC_ADDRESS.PLC_BARCODE_RESULT}  . Result: {bFailSendToPLC}  Device {m_Tracks[1].m_CurrentSequenceDeviceID}");

                //LogMessage.WriteToDebugViewer(8, $"{ Application.LineNumber()}: {Application.PrintCallerName()}");
                if (MainWindowVM.m_bSequenceRunning || m_SequenceMode == (int)SEQUENCE_MODE.MODE_MANUAL)
                {

                    if (m_Tracks[1].m_VisionResultDatas[nDeviceID].m_nResult == -(int)ERROR_CODE.PASS)
                    {

                        lock (m_UpdateResultQueue)
                        {
                            m_UpdateResultQueue[1].Enqueue(m_Tracks[1].m_VisionResultDatas[nDeviceID]);
                        }



                        int nAbleCout;
                        if (m_SequenceMode == (int)SEQUENCE_MODE.MODE_MANUAL)
                        {
                            nAbleCout = m_plcComm.ReadPLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_MANUAL_BARCODE_COUNT);
                        }
                        else
                        {
                            nAbleCout = HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.PLC_BARCODE_COUNT);
                        }
                        int nTimeOut = 0;
                        while (nAbleCout != 1)
                        {
                            if (m_SequenceMode == (int)SEQUENCE_MODE.MODE_MANUAL)
                            {
                                nAbleCout = m_plcComm.ReadPLCRegister((int)PLCCOMM.PLC_ADDRESS.PLC_MANUAL_BARCODE_COUNT);
                            }
                            else
                            {
                                nAbleCout = HWinRobot.get_digital_input(HiWinRobotInterface.m_RobotConnectID, (int)INPUT_IOROBOT.PLC_BARCODE_COUNT);
                            }

                            Thread.Sleep(50);
                            nTimeOut++;
                            if (nTimeOut > 50)
                            {
                                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    OutputLogVM.AddLineOutputLog($"Barcode Reader: Get Count Signal Failed !  Device {m_Tracks[1].m_CurrentSequenceDeviceID}", (int)ERROR_CODE.LABEL_FAIL);

                                });

                                m_Tracks[1].m_VisionResultDatas[nDeviceID].m_nResult = -(int)ERROR_CODE.NO_PATTERN_FOUND;
                                break;
                            }
                        }
                    }
                    LogMessage.WriteToDebugViewer(8, $"Barcode Reader: Get count PLC Done. Result: {bFailSendToPLC}  Device {m_Tracks[1].m_CurrentSequenceDeviceID}");

                    // If disconnect during save data, set at fail
                    if (m_Tracks[1].m_VisionResultDatas[nDeviceID].m_nResult != -(int)ERROR_CODE.PASS)
                    {
                        string strFullPathImageOutTemp = m_Tracks[1].m_VisionResultDatas[nDeviceID].m_strFullImagePath;
                        string strFailImage = strFullPathImageOutTemp.Replace("PASS IMAGE", "FAIL IMAGE");
                        if (File.Exists(strFullPathImageOutTemp) && !File.Exists(strFailImage))
                            File.Move(strFullPathImageOutTemp, strFailImage);
                        m_Tracks[1].m_VisionResultDatas[nDeviceID].m_strFullImagePath = strFailImage;


                    }

                    if (m_Tracks[1].m_VisionResultDatas[nDeviceID].m_nResult == -(int)ERROR_CODE.PASS)
                    {

                        //lock (m_UpdateResultQueue)
                        //{
                        //    m_UpdateResultQueue[1].Enqueue(m_Tracks[1].m_VisionResultDatas[nDeviceID]);
                        //}

                        double dDeltaAngle = 0;
                        System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            m_Tracks[1].DrawInspectionResult(ref m_Tracks[1].m_VisionResultDatas[nDeviceID].m_nResult, ref pCenter, ref dDeltaAngle);
                        });
                        m_Tracks[1].m_CurrentSequenceDeviceID++;
                    }
                }
                LogMessage.WriteToDebugViewer(8, $"Barcode Reader: Scan Done!");

            }

        EndThread:
            LogMessage.WriteToDebugViewer(1, $"BarCodeReaderSequence Thread Released");

        }



        string m_FailstrLotIDFolder = "";
        int nFailCount = 0;
        internal void func_RunSaveInspectImageThread(int nTrack)
        {
            while (true)
            {
                //if (MainWindow.mainWindow == null || !MainWindow.m_IsWindowOpen)
                //    break;

                Thread.Sleep(50);
                ImageSaveData data = new ImageSaveData();
                lock (m_SaveInspectImageQueue[nTrack])
                {
                    if (m_SaveInspectImageQueue[nTrack].Count == 0)
                        continue;

                    data = m_SaveInspectImageQueue[nTrack].Dequeue();
                }
                try
                {
                    if (data.imageSave == null)
                        continue;

                    CvInvoke.Imwrite(data.m_strPathImage, data.imageSave);
                    LogMessage.WriteToDebugViewer(4, $"{data.m_strPathImage}");

                }
                catch (Exception)
                {
                }
            }

            LogMessage.WriteToDebugViewer(1, $"Image Save Thread Released.");

        }

        public string createImageFilePathToSave(int nDeviceID, int nResult, string trackName, string strCurrentLot, string strDeviceID = "")
        {
            string strDeviceIDTemp = strDeviceID;
            if (strDeviceID == "")
                strDeviceIDTemp = string.Format("{0}{1}{2}+{3}{4}{5}", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"), DateTime.Now.ToString("HH"), DateTime.Now.ToString("mm"), DateTime.Now.ToString("ss"));
            //string strLotIDFolder = Path.Combine(Application.pathImageSave, data.strLotID + "\\");
            //string strDay = string.Format("{0}{1}{2}", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"));
            string strPassFolder = Path.Combine(AppMagnus.pathImageSave, trackName, AppMagnus.currentRecipe, AppMagnus.m_strStartLotDay, strCurrentLot, "PASS IMAGE");
            if (!Directory.Exists(strPassFolder))
                Directory.CreateDirectory(strPassFolder);
            string path_image = Path.Combine(strPassFolder, $"{strDeviceIDTemp}_{nDeviceID + 1}" + ".bmp");

            string strFailFolder = Path.Combine(AppMagnus.pathImageSave, trackName, AppMagnus.currentRecipe, AppMagnus.m_strStartLotDay, strCurrentLot, "FAIL IMAGE");
            if (!Directory.Exists(strFailFolder))
                Directory.CreateDirectory(strFailFolder);

            if (m_FailstrLotIDFolder != strCurrentLot)
            {
                m_FailstrLotIDFolder = strCurrentLot;
                nFailCount = 0;
            }


            //string strPassFail = "PASS";
            //m_FailstrLotIDFolder = strPassFolder;


            if (nResult != -(int)ERROR_CODE.PASS)
            {
                path_image = Path.Combine(strFailFolder, $"{strDeviceIDTemp}_{nFailCount + 1}" + ".bmp");
                nFailCount++;
            }
            return path_image;
        }


        internal void func_UpdateResultThread(int nTrack)
        {
            VisionResultData data = new VisionResultData();
            while (true)
            {
                //if (MainWindow.mainWindow == null || !MainWindow.m_IsWindowOpen)
                //    break;

                Thread.Sleep(50);
                lock (m_UpdateResultQueue[nTrack])
                {
                    if (m_UpdateResultQueue[nTrack].Count == 0)
                        continue;

                    data = m_UpdateResultQueue[nTrack].Dequeue();

                    //data.m_nDeviceIndexOnReel = m_UpdateResultQueue[nTrack].Dequeue().m_nDeviceIndexOnReel;
                    //data.m_nResult = m_UpdateResultQueue[nTrack].Dequeue().m_nResult;
                    //data.m_strDeviceID = m_UpdateResultQueue[nTrack].Dequeue().m_strDeviceID;
                    //data.m_strFullImagePath = m_UpdateResultQueue[nTrack].Dequeue().m_strFullImagePath;

                }
                try
                {
                    if (data.m_nResult == -(int)ERROR_CODE.NOT_INSPECTED)
                        continue;

                    saveExcel:
                    int nEror = VisionResultData.SaveSequenceResultToExcel(AppMagnus.m_strCurrentLot, nTrack, data, data.bIsTotal);
                    if (nEror == -1)
                    {
                        Thread.Sleep(500);
                        goto saveExcel;
                    }

                    System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        updateMappingResultDelegate?.Invoke(data, nTrack, data.m_nDeviceIndexOnReel);
                        StatisticVM.updateValueStatisticDelegate?.Invoke(data.m_nResult, nTrack);
                    });

                }
                catch (Exception e)
                {
                    LogMessage.WriteToDebugViewer(1, $"Track {nTrack + 1} Update Result UI Thread Failed. {e}");
                }
            }

            LogMessage.WriteToDebugViewer(1, $"Update Result UI Thread Released.");

        }


        public void TeachThread()
        {
            if (m_TeachThread == null)
            {
                m_TeachThread = new System.Threading.Thread(new System.Threading.ThreadStart(() => m_Tracks[MainWindowVM.activeImageDock.trackID].m_imageViews[0].func_TeachSequence(MainWindowVM.activeImageDock.trackID)));
                m_TeachThread.SetApartmentState(ApartmentState.STA);
            }
            else
            {
                m_TeachThread = null;
                m_TeachThread = new System.Threading.Thread(new System.Threading.ThreadStart(() => m_Tracks[MainWindowVM.activeImageDock.trackID].m_imageViews[0].func_TeachSequence(MainWindowVM.activeImageDock.trackID)));
                m_TeachThread.SetApartmentState(ApartmentState.STA);
            }
            m_TeachThread.IsBackground = true;
            m_TeachThread.Start();


        }

        public void UpdateResult()
        {
        }

        public void WriteTeachParam(int nTrack)
        {
            try
            {
                ((TeachParameterVM)m_MainWindowVM.mTeachParameterVM.CurrentViewModel).WriteTeachParam(nTrack);
            }
            catch (Exception)
            {
            }
        }

        internal void OpenHiwinRobotDialog(bool bIschecked = false)
        {
            //HiWinRobotInterface.m_hiWinRobotUserControl.Visibility = System.Windows.Visibility.Visible;
            //defectInfor.lvDefect.View = gridView;
            //if (HiWinRobotInterface.m_RobotConnectID >= 0)
            //    HWinRobot.set_operation_mode(HiWinRobotInterface.m_RobotConnectID, (int)ROBOT_OPERATION_MODE.MODE_MANUAL);
            //m_hiWinRobotInterface.m_hiWinRobotUserControl.check_Manual.IsChecked = true;
            //m_hiWinRobotInterface.m_hiWinRobotUserControl.check_Auto.IsChecked = false;

            //if (bIschecked)
            //{
            //   MainWindow.mainWindow.grd_Defect.Height = 650;// m_hiWinRobotInterface.m_hiWinRobotUserControl.Height;
            //   MainWindow.mainWindow.grd_Defect.Width = 800;// m_hiWinRobotInterface.m_hiWinRobotUserControl.Width;

            //   MainWindow.mainWindow.grd_Defect.Children.Clear();
            //   MainWindow.mainWindow.grd_Defect.Children.Add(m_hiWinRobotInterface.m_hiWinRobotUserControl);
            //    //defectInfor.SvDefect.CanContentScroll = true;
            //    //MainWindow.mainWindow.grd_Defect.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            //    //MainWindow.mainWindow.grd_Defect.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            //   MainWindow.mainWindow.grd_Defect.Visibility = Visibility.Visible;

            //   MainWindow.mainWindow.grd_Defect_Settings.Visibility = System.Windows.Visibility.Visible;
            //}
            //else
            //{
            //   MainWindow.mainWindow.grd_Defect.Children.Clear();
            //   MainWindow.mainWindow.grd_Defect_Settings.Visibility = System.Windows.Visibility.Collapsed;
            //}
        }

        public static void ReleaseEventAndThread()
        {
            for (int nTrack = 0; nTrack < 2; nTrack++)
            {
                InspectEvent[nTrack].Set();
                //InspectDoneEvent[nTrack].Set();
                m_hardwareTriggerSnapEvent[nTrack].Set();
                //m_OfflineTriggerSnapEvent[nTrack].Set();
                //m_NextStepSequenceEvent.Set();
                //m_EventInspectionOnlineThreadDone[nTrack].Set();
                //StartWaitPLCToTriggerCameraEvent[nTrack].Set();


                Thread.Sleep(500);

                InspectEvent[nTrack].Dispose();
                InspectDoneEvent[nTrack].Dispose();
                m_hardwareTriggerSnapEvent[nTrack].Dispose();
                m_OfflineTriggerSnapEvent[nTrack].Dispose();
                m_NextStepSequenceEvent.Dispose();
                m_EventInspectionOnlineThreadDone[nTrack].Dispose();
                //StartWaitPLCToTriggerCameraEvent[nTrack].Dispose();

            }
        }
    }




}
