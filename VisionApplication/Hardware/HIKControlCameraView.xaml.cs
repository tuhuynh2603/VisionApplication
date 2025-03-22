﻿using MvCamCtrl.NET;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using VisionApplication.Define;
using VisionApplication.Helper;
using VisionApplication.Model;
using VisionApplication.MVVM.View;
using VisionApplication.MVVM.ViewModel;

namespace VisionApplication.Hardware
{
    /// <summary>
    /// Interaction logic for HIKControlCameraView.xaml
    /// </summary>

    public partial class HIKControlCameraView : System.Windows.Controls.UserControl
    {

        private static CameraParameter cameraParameter = new CameraParameter();


        MyCamera.MV_CC_DEVICE_INFO_LIST m_stDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
        public MyCamera m_MyCamera = new MyCamera();
        public bool m_bGrabbing = false;
        //Thread m_hReceiveThread = null;
        public string m_strCameraSerial;
        public int m_nTrack;
        public bool m_bIsConnected = false;
        public HIKControlCameraView(string strCameraID, int nTrack)
        {
            InitializeComponent();
            //this.Closing += Window_Closing;

            DeviceListAcq();

            CameraHelper.LoadCamSetting(nTrack, cameraParameter);
            MainWindowVM.updateCameraConnectionStatusDelegate?.Invoke(nTrack, InitializeCamera(strCameraID));
            m_strCameraSerial = strCameraID;
            m_nTrack = nTrack;
            // ch:设置采集连续模式 | en:Set Continues Aquisition Mode

        }
        public HIKControlCameraView()
        {
            InitializeComponent();
            //this.Closing += Window_Closing;
            //Todo: Add later 
            //DeviceListAcq();



            // ch:设置采集连续模式 | en:Set Continues Aquisition Mode

        }

        public void InitCameraSetting(string strCameraID, int nTrack)
        {
            CameraHelper.LoadCamSetting(nTrack, cameraParameter);
            MainWindowVM.updateCameraConnectionStatusDelegate?.Invoke(nTrack, InitializeCamera(strCameraID));
            m_strCameraSerial = strCameraID;
            m_nTrack = nTrack;
        }

        // ch:显示错误信息 | en:Show error message
        private void ShowErrorMsg(string csMessage, int nErrorNum)
        {
            string errorMsg;
            if (nErrorNum == 0)
            {
                errorMsg = csMessage;
            }
            else
            {
                errorMsg = csMessage + ": Error =" + String.Format("{0:X}", nErrorNum);
            }

            switch (nErrorNum)
            {
                case MyCamera.MV_E_HANDLE: errorMsg += " Error or invalid handle "; break;
                case MyCamera.MV_E_SUPPORT: errorMsg += " Not supported function "; break;
                case MyCamera.MV_E_BUFOVER: errorMsg += " Cache is full "; break;
                case MyCamera.MV_E_CALLORDER: errorMsg += " Function calling order error "; break;
                case MyCamera.MV_E_PARAMETER: errorMsg += " Incorrect parameter "; break;
                case MyCamera.MV_E_RESOURCE: errorMsg += " Applying resource failed "; break;
                case MyCamera.MV_E_NODATA: errorMsg += " No data "; break;
                case MyCamera.MV_E_PRECONDITION: errorMsg += " Precondition error, or running environment changed "; break;
                case MyCamera.MV_E_VERSION: errorMsg += " Version mismatches "; break;
                case MyCamera.MV_E_NOENOUGH_BUF: errorMsg += " Insufficient memory "; break;
                case MyCamera.MV_E_UNKNOW: errorMsg += " Unknown error "; break;
                case MyCamera.MV_E_GC_GENERIC: errorMsg += " General error "; break;
                case MyCamera.MV_E_GC_ACCESS: errorMsg += " Node accessing condition error "; break;
                case MyCamera.MV_E_ACCESS_DENIED: errorMsg += " No permission "; break;
                case MyCamera.MV_E_BUSY: errorMsg += " Device is busy, or network disconnected "; break;
                case MyCamera.MV_E_NETER: errorMsg += " Network error "; break;
            }

            System.Windows.MessageBox.Show(errorMsg, "PROMPT");
        }

        private void DeviceListAcq()
        {
            // ch:创建设备列表 | en:Create Device List
            System.GC.Collect();
            cbDeviceList.Items.Clear();
            m_stDeviceList.nDeviceNum = 0;
            int nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_stDeviceList);
            if (0 != nRet)
            {
                ShowErrorMsg("Enumerate devices fail!", 0);
                return;
            }

            // ch:在窗体列表中显示设备名 | en:Display device name in the form list
            for (int i = 0; i < m_stDeviceList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));

                    if (gigeInfo.chUserDefinedName != "")
                    {
                        cbDeviceList.Items.Add("GEV: " + gigeInfo.chUserDefinedName + " (" + gigeInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        cbDeviceList.Items.Add("GEV: " + gigeInfo.chManufacturerName + " " + gigeInfo.chModelName + " (" + gigeInfo.chSerialNumber + ")");
                    }
                }
                else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                {
                    MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stUsb3VInfo, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                    if (usbInfo.chUserDefinedName != "")
                    {
                        cbDeviceList.Items.Add("U3V: " + usbInfo.chUserDefinedName + " (" + usbInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        cbDeviceList.Items.Add("U3V: " + usbInfo.chManufacturerName + " " + usbInfo.chModelName + " (" + usbInfo.chSerialNumber + ")");
                    }
                }
            }

            // ch:选择第一项 | en:Select the first item
            if (m_stDeviceList.nDeviceNum != 0)
            {
                cbDeviceList.SelectedIndex = 0;
            }
        }

        private void bnEnum_Click(object sender, RoutedEventArgs e)
        {
            //DeviceListAcq();
        }

        private void bnOpen_Click(object sender, RoutedEventArgs e)
        {
            if (m_stDeviceList.nDeviceNum == 0 || cbDeviceList.SelectedIndex == -1)
            {
                ShowErrorMsg("No device, please select", 0);
                return;
            }

            MainWindowVM.updateCameraConnectionStatusDelegate?.Invoke(m_nTrack, InitializeCamera(m_strCameraSerial));
            // ch:设置采集连续模式 | en:Set Continues Aquisition Mode
            //bnGetParam_Click(null, null);

            // ch:控件操作 | en:Control operation
            bnOpen.IsEnabled = false;

            bnClose.IsEnabled = true;
            //bnStartGrab.IsEnabled = true;
            //bnStopGrab.IsEnabled = false;
            //bnContinuesMode.IsEnabled = true;
            //bnContinuesMode.IsChecked = true;
            //bnTriggerMode.IsEnabled = true;
            //cbSoftTrigger.IsEnabled = false;
            //bnSoftTriggerOnce.IsEnabled = false;

            tbExposure.IsEnabled = true;
            tbGain.IsEnabled = true;
            tbFrameRate.IsEnabled = true;
            bnGetParam.IsEnabled = true;
            bnSetParam.IsEnabled = true;
        }

        public bool InitializeCamera(string strCameraID)
        {
            m_bIsConnected = false;
            int nCameraIndex = -1;
            MyCamera.MV_CC_DEVICE_INFO devices;
            if (strCameraID != "")
            {
                // ch:选择第一项 | en:Select the first item
                //if (m_stDeviceList.nDeviceNum != 0)
                //{
                //    cbDeviceList.SelectedIndex = 0;
                //}

                for (int n = 0; n < m_stDeviceList.nDeviceNum; n++)
                {
                    devices = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[n], typeof(MyCamera.MV_CC_DEVICE_INFO));
                    if (devices.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                    {
                        MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(devices.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));

                        if (gigeInfo.chSerialNumber == strCameraID)
                        {
                            nCameraIndex = n;
                            break;
                        }
                    }
                    else if (devices.nTLayerType == MyCamera.MV_USB_DEVICE)
                    {
                        MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)MyCamera.ByteToStruct(devices.SpecialInfo.stUsb3VInfo, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                        if (usbInfo.chSerialNumber == strCameraID)
                        {
                            nCameraIndex = n;
                            break;
                        }
                    }

                }
                // ch:获取选择的设备信息 | en:Get selected device information
            }
            else
                nCameraIndex = cbDeviceList.SelectedIndex;


            if ( nCameraIndex < 0)
                return false;

           /* MyCamera.MV_CC_DEVICE_INFO*/ devices =
                (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[nCameraIndex],
                                                                typeof(MyCamera.MV_CC_DEVICE_INFO));

            // ch:打开设备 | en:Open device
            if (null == m_MyCamera)
            {
                m_MyCamera = new MyCamera();
                if (null == m_MyCamera)
                {
                    return false;
                }
            }

            int nRet = m_MyCamera.MV_CC_CreateDevice_NET(ref devices);
            if (MyCamera.MV_OK != nRet)
            {
                return false;
            }

            if (m_MyCamera.MV_CC_IsDeviceConnected_NET())
                m_MyCamera.MV_CC_CloseDevice_NET();

            if (m_MyCamera.MV_CC_IsDeviceConnected_NET())
                m_MyCamera.MV_CC_DestroyDevice_NET();

            nRet = m_MyCamera.MV_CC_OpenDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                m_MyCamera.MV_CC_DestroyDevice_NET();
                ShowErrorMsg("Device open fail!", nRet);
                return false;
            }

            // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
            if (devices.nTLayerType == MyCamera.MV_GIGE_DEVICE)
            {
                int nPacketSize = m_MyCamera.MV_CC_GetOptimalPacketSize_NET();
                if (nPacketSize > 0)
                {
                    nRet = m_MyCamera.MV_CC_SetIntValueEx_NET("GevSCPSPacketSize", nPacketSize);
                    if (nRet != MyCamera.MV_OK)
                    {
                        ShowErrorMsg("Set Packet Size failed!", nRet);
                    }
                }
                else
                {
                    ShowErrorMsg("Get Packet Size failed!", nPacketSize);
                }
            }


            m_MyCamera.MV_CC_SetEnumValue_NET("AcquisitionMode", (uint)MyCamera.MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
            m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON);
            m_MyCamera.MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE);
            if(!SetParameterToCamera(cameraParameter.exposureTime, cameraParameter.gain, cameraParameter.frameRate))
            {
                float expose = 0;
                float gain = 0;
                float frameRate = 0;
                GetCameraParameter(ref expose, ref gain, ref frameRate);
                cameraParameter.gain = gain;
                cameraParameter.exposureTime = expose;
                cameraParameter.frameRate = frameRate;
                ShowErrorMsg($"Failed! gain{gain} expose {expose} frameRate {frameRate}", (int)MyCamera.MV_ALG_E_PARAM_VALUE);

                //Application.WriteCamSetting(m_nTrack);
            }
            m_bIsConnected = true;
            return true;
        }

        private void bnClose_Click(object sender, RoutedEventArgs e)
        {

            // ch:取流标志位清零 | en:Reset flow flag bit
            if (m_bGrabbing == true)
            {
                m_bGrabbing = false;
            }

            // ch:关闭设备 | en:Close Device
            m_MyCamera.MV_CC_CloseDevice_NET();
            m_MyCamera.MV_CC_DestroyDevice_NET();

            bnOpen.IsEnabled = true;

            bnClose.IsEnabled = false;

            tbExposure.IsEnabled = false;
            tbGain.IsEnabled = false;
            tbFrameRate.IsEnabled = false;
            bnGetParam.IsEnabled = false;
            bnSetParam.IsEnabled = false;

        }
        public void CaptureAndGetImageBuffer (ref byte[] pGrabbedImgBuf, ref int nWidthImage, ref int nHeightImage)
        {
            //IGrabResult grabResult = camera.StreamGrabber.RetrieveResult(grabTimeOut, TimeoutHandling.Return);
            Array.Clear(pGrabbedImgBuf, 0, pGrabbedImgBuf.Length);

            MyCamera.MV_FRAME_OUT stFrameInfo = new MyCamera.MV_FRAME_OUT();
            //MyCamera.MV_DISPLAY_FRAME_INFO stDisplayInfo = new MyCamera.MV_DISPLAY_FRAME_INFO();
            int nRet = MyCamera.MV_OK;
            nRet = m_MyCamera.MV_CC_GetImageBuffer_NET(ref stFrameInfo, 1000);
            if (nRet == MyCamera.MV_OK)
            {
                if (stFrameInfo.pBufAddr != null)
                {
                    Marshal.Copy(stFrameInfo.pBufAddr, pGrabbedImgBuf, 0, (int)stFrameInfo.stFrameInfo.nFrameLen);

                    m_MyCamera.MV_CC_FreeImageBuffer_NET(ref stFrameInfo);
                    nWidthImage = stFrameInfo.stFrameInfo.nWidth;
                    nHeightImage = stFrameInfo.stFrameInfo.nHeight;
                }
            }
        }
        private void bnGetParam_Click(object sender, RoutedEventArgs e)
        {
            float expose = 0;
            float gain = 0;
            float frameRate = 0;
            GetCameraParameter(ref expose, ref gain, ref frameRate);
            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            int nRet = m_MyCamera.MV_CC_GetFloatValue_NET("ExposureTime", ref stParam);
            tbExposure.Text = expose.ToString("F1");
            tbGain.Text = gain.ToString("F1");
            tbFrameRate.Text = frameRate.ToString("F1");
        }
        public void GetCameraParameter(ref float expose, ref float gain, ref float frameRate)
        {
            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            int nRet = m_MyCamera.MV_CC_GetFloatValue_NET("ExposureTime", ref stParam);
            if (MyCamera.MV_OK == nRet)
            {
                expose= stParam.fCurValue;
            }

            nRet = m_MyCamera.MV_CC_GetFloatValue_NET("Gain", ref stParam);
            if (MyCamera.MV_OK == nRet)
            {
                gain = stParam.fCurValue;
            }

            nRet = m_MyCamera.MV_CC_GetFloatValue_NET("ResultingFrameRate", ref stParam);
            if (MyCamera.MV_OK == nRet)
            {
                frameRate = stParam.fCurValue;
            }
        }

        private void bnSetParam_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                float.Parse(tbExposure.Text);
                float.Parse(tbGain.Text);
                float.Parse(tbFrameRate.Text);
            }
            catch
            {
                ShowErrorMsg("Please enter correct type!", 0);
                return;
            }

            bool bSetOK = SetParameterToCamera(float.Parse(tbExposure.Text), float.Parse(tbGain.Text), float.Parse(tbFrameRate.Text));
            if (!bSetOK)
            {
                System.Windows.MessageBox.Show("Set param to Camera failed. Please check again!");

                return;
            }

            var result = System.Windows.MessageBox.Show("Would you like to save camera parameters ?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                cameraParameter.gain = float.Parse(tbGain.Text);
                cameraParameter.exposureTime = float.Parse(tbExposure.Text);
                cameraParameter.frameRate = float.Parse(tbFrameRate.Text);
                CameraHelper.WriteCamSetting(m_nTrack, cameraParameter);
            }
        }

        public bool SetParameterToCamera(float fExpose, float fGain, float fFrameRate)
        {
            m_MyCamera.MV_CC_SetEnumValue_NET("ExposureAuto", 0);
            int nRet = m_MyCamera.MV_CC_SetFloatValue_NET("ExposureTime", fExpose);
            if (nRet != MyCamera.MV_OK)
            {
                ShowErrorMsg("Set Exposure Time Fail!", nRet);
                return false;
            }

            m_MyCamera.MV_CC_SetEnumValue_NET("GainAuto", 0);
            nRet = m_MyCamera.MV_CC_SetFloatValue_NET("Gain", fGain);
            if (nRet != MyCamera.MV_OK)
            {
                ShowErrorMsg("Set Gain Fail!", nRet);
                return false;
            }

            nRet = m_MyCamera.MV_CC_SetFloatValue_NET("AcquisitionFrameRate", fFrameRate);
            if (nRet != MyCamera.MV_OK)
            {
                ShowErrorMsg("Set Frame Rate Fail!", nRet);
                return false;
            }
            return true;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {


            bnClose_Click(null, null);
        }
    }
}
