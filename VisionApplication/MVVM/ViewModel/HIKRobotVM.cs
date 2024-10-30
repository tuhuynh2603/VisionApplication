using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static VisionApplication.Hardware.SDKHrobot.HiWinRobotInterface;

namespace VisionApplication.MVVM.ViewModel
{
    public class HIKRobotVM:BaseVM, ICustomUserControl
    {
        public static Mat m_MatCameraRobotTransform { set; get; } = new Mat();
        public static List<SequencePointData> m_List_sequencePointData = new List<SequencePointData>();

        private string _m_txtRobotIPAddress = "127.0.0.1";

        public string txtRobotIPAddress
        {
            get { return _m_txtRobotIPAddress; }
            set
            {
                _m_txtRobotIPAddress = value;
                OnPropertyChanged("txtRobotIPAddress");
            }
        }

        private int _m_nAccRatioPercentValue = 1;
        public int m_nAccRatioPercentValue
        {
            get { return _m_nAccRatioPercentValue; }
            set
            {
                _m_nAccRatioPercentValue = value;
                OnPropertyChanged("m_nAccRatioPercentValue");
            }
        }

        private int _m_PTPSpeedPercentValue = 1;
        public int m_PTPSpeedPercentValue
        {
            get { return _m_PTPSpeedPercentValue; }
            set
            {
                _m_PTPSpeedPercentValue = value;
                OnPropertyChanged("m_PTPSpeedPercentValue");
            }
        }


        private int _m_nLinearSpeedValue = 1;
        public int m_nLinearSpeedValue
        {
            get { return _m_nLinearSpeedValue; }
            set
            {
                _m_nLinearSpeedValue = value;
                OnPropertyChanged("m_nLinearSpeedValue");
            }
        }





        private int _m_nOverridePercent = 1;
        public int m_nOverridePercent
        {
            get { return _m_nOverridePercent; }
            set
            {
                _m_nOverridePercent = value;
                OnPropertyChanged("m_nOverridePercent");
            }
        }

        private int _m_nStepRelativeValue = 0;
        public int m_nStepRelativeValue
        {
            get { return _m_nStepRelativeValue; }
            set
            {
                _m_nStepRelativeValue = value;
                OnPropertyChanged("m_nStepRelativeValue");
            }
        }



        private DragDropUserControlVM _dragDropVM { set; get; }
        public void RegisterUserControl()
        {
            _dragDropVM.RegisterMoveGrid();
            _dragDropVM.RegisterResizeGrid();
        }

        public HIKRobotVM(DragDropUserControlVM dragDropVM)
        {
            _dragDropVM = dragDropVM;
            RegisterUserControl();
        }

        public static void Docalibration(List<SequencePointData> data)
        {

            System.Drawing.PointF[] m_CalibPoints = new System.Drawing.PointF[3];
            System.Drawing.PointF[] m_CalibVisionPoints = new System.Drawing.PointF[3];

            for (int nIndex = 0; nIndex < data.Count; nIndex++)
            {
                if (data[nIndex].m_PointComment == SequencePointData.CALIB_ROBOT_POSITION_1)
                {
                    m_CalibPoints[0].X = (float)data[nIndex].m_X;
                    m_CalibPoints[0].Y = (float)data[nIndex].m_Y;
                }
                if (data[nIndex].m_PointComment == SequencePointData.CALIB_ROBOT_POSITION_2)
                {
                    m_CalibPoints[1].X = (float)data[nIndex].m_X;
                    m_CalibPoints[1].Y = (float)data[nIndex].m_Y;
                }

                if (data[nIndex].m_PointComment == SequencePointData.CALIB_ROBOT_POSITION_3)
                {
                    m_CalibPoints[2].X = (float)data[nIndex].m_X;
                    m_CalibPoints[2].Y = (float)data[nIndex].m_Y;
                }

                if (data[nIndex].m_PointComment == SequencePointData.CALIB_Vision_POSITION_1)
                {
                    m_CalibVisionPoints[0].X = (float)data[nIndex].m_X;
                    m_CalibVisionPoints[0].Y = (float)data[nIndex].m_Y;
                }
                if (data[nIndex].m_PointComment == SequencePointData.CALIB_Vision_POSITION_2)
                {
                    m_CalibVisionPoints[1].X = (float)data[nIndex].m_X;
                    m_CalibVisionPoints[1].Y = (float)data[nIndex].m_Y;
                }

                if (data[nIndex].m_PointComment == SequencePointData.CALIB_Vision_POSITION_3)
                {
                    m_CalibVisionPoints[2].X = (float)data[nIndex].m_X;
                    m_CalibVisionPoints[2].Y = (float)data[nIndex].m_Y;
                }
            }

            HIKRobotVM.m_MatCameraRobotTransform = Track.MagnusMatrix.CalculateTransformMatrix(m_CalibVisionPoints, m_CalibPoints);

        }


    }
}
