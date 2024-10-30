﻿using Emgu.CV;
using VisionApplication.Hardware.SDKHrobot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static VisionApplication.Hardware.SDKHrobot.HiWinRobotInterface;
using VisionApplication.MVVM.View;
using VisionApplication.Helper;
using VisionApplication.MVVM.ViewModel;

namespace VisionApplication.Hardware
{
    /// <summary>
    /// Interaction logic for HiWinRobotUserControl.xaml
    /// </summary>
    public partial class HiWinRobotUserControl : System.Windows.Controls.UserControl
    {
        public enum MOVETYPES
        {
            AbsoluteMove = 0,
            RelativeMove = 1
        }

        public static string m_strAlarmMessage = "";
        public HiWinRobotUserControl()
        {
            InitializeComponent();
            //m_txtRobotIPAddress = HiWinRobotInterface.m_strRobotIPAddress;

            combo_JogType.Items.Clear();
            combo_JogType.Items.Add("XYZ");
            combo_JogType.Items.Add("Joint");
            combo_JogType.SelectedIndex = 1;
            SetMovingButtonLabel();

            combo_MoveTypes.Items.Add("Absolute");
            combo_MoveTypes.Items.Add("Relative");
            combo_MoveTypes.SelectedIndex = 1;

            int bServoOnOff = HWinRobot.get_motor_state(HiWinRobotInterface.m_RobotConnectID);
            toggle_ServoOnOff.IsChecked = bServoOnOff == 0 ? false : true;
            if (HWinRobot.get_operation_mode(HiWinRobotInterface.m_RobotConnectID) == 0)
            {
                check_Manual.IsChecked = true;
                check_Auto.IsChecked = false;
            }
            else
            {
                check_Manual.IsChecked = false;
                check_Auto.IsChecked = true;
            }

            //m_nAccRatioPercentValue = 10;
            //m_PTPSpeedPercentValue = 10;
            //m_nLinearSpeedValue = 10;
            //m_nOverridePercent = 10;
            //m_nStepRelativeValue = 1000;

            //slider_AccRatioPercent.Value = 10;
            //slider_LinearSpeed.Value = 100;
            //slider_PTPSpeedPercent.Value = 10;
            //slider_OverridePercent.Value = 10;
            //slider_StepRelative.Value = 1000;

            slider_AccRatioPercentShow.Text = "10";
            slider_LinearSpeedShow.Text = "100";
            slider_PTPSpeedPercentShow.Text = "10";
            slider_OverridePercentShow.Text = "10";
            slider_StepRelativeShow.Text = "1000";
            HIKRobotVM.m_List_sequencePointData = new List<SequencePointData>();
            HIKRobotVM.m_List_sequencePointData.Add(new SequencePointData() { m_PointComment = SequencePointData.HOME_POSITION });
            HIKRobotVM.m_List_sequencePointData.Add(new SequencePointData() { m_PointComment = SequencePointData.READY_POSITION });
            HIKRobotVM.m_List_sequencePointData.Add(new SequencePointData() { m_PointComment = SequencePointData.PRE_PICK_POSITION });
            HIKRobotVM.m_List_sequencePointData.Add(new SequencePointData() { m_PointComment = SequencePointData.PICK_POSITION });
            HIKRobotVM.m_List_sequencePointData.Add(new SequencePointData() { m_PointComment = SequencePointData.PRE_PASS_PLACE_POSITION });
            HIKRobotVM.m_List_sequencePointData.Add(new SequencePointData() { m_PointComment = SequencePointData.PASS_PLACE_POSITION });
            HIKRobotVM.m_List_sequencePointData.Add(new SequencePointData() { m_PointComment = SequencePointData.PRE_FAILED_BLACK_PLACE_POSITION });
            HIKRobotVM.m_List_sequencePointData.Add(new SequencePointData() { m_PointComment = SequencePointData.PRE_FAILED_PLACE_POSITION });


            HIKRobotVM.m_List_sequencePointData.Add(new SequencePointData() { m_PointComment = SequencePointData.CALIB_ROBOT_POSITION_1 });
            HIKRobotVM.m_List_sequencePointData.Add(new SequencePointData() { m_PointComment = SequencePointData.CALIB_ROBOT_POSITION_2 });
            HIKRobotVM.m_List_sequencePointData.Add(new SequencePointData() { m_PointComment = SequencePointData.CALIB_ROBOT_POSITION_3 });
            HIKRobotVM.m_List_sequencePointData.Add(new SequencePointData() { m_PointComment = SequencePointData.CALIB_Vision_POSITION_1 });
            HIKRobotVM.m_List_sequencePointData.Add(new SequencePointData() { m_PointComment = SequencePointData.CALIB_Vision_POSITION_2 });
            HIKRobotVM.m_List_sequencePointData.Add(new SequencePointData() { m_PointComment = SequencePointData.CALIB_Vision_POSITION_3 });

            HiWinRobotInterface.SequencePointData.ReadRobotPointsFromExcel(ref HIKRobotVM.m_List_sequencePointData);
            dataGrid_all_robot_Positions.ItemsSource = null;
            dataGrid_all_robot_Positions.ItemsSource = HIKRobotVM.m_List_sequencePointData;
            HIKRobotVM.Docalibration(HIKRobotVM.m_List_sequencePointData);
            //this.DataContext = this;
        }


        private void slider_AccRatioPercent_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            HIKRobotVM? vm = this.DataContext as HIKRobotVM;
            if (vm == null)
                return;
            if (slider_AccRatioPercent.Value < slider_AccRatioPercent.Minimum)
            {
                vm.m_nAccRatioPercentValue = (int)slider_AccRatioPercent.Minimum;
            }
            else if (slider_AccRatioPercent.Value <= slider_AccRatioPercent.Maximum)
            {
                vm.m_nAccRatioPercentValue = (int)slider_AccRatioPercent.Maximum;
            }
        }

        private void slider_AccRatioPercentShow_TextChanged(object sender, TextChangedEventArgs e)
        {
            double dvalue = 0.0;
            bool error = Double.TryParse(slider_AccRatioPercentShow.Text, out dvalue);
            slider_AccRatioPercentShow.Text = Math.Round(dvalue).ToString();

            Int16 val = Convert.ToInt16(slider_AccRatioPercentShow.Text);
            HWinRobot.set_acc_dec_ratio(HiWinRobotInterface.m_RobotConnectID, val);
        }

        private void slider_PTPSpeedPercent_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            HIKRobotVM? vm = this.DataContext as HIKRobotVM;
            if (vm == null)
                return;
            if (slider_PTPSpeedPercent.Value < slider_PTPSpeedPercent.Minimum)
            {
                vm.m_PTPSpeedPercentValue = (int)slider_PTPSpeedPercent.Minimum;
            }
            else if (slider_PTPSpeedPercent.Value <= slider_PTPSpeedPercent.Maximum)
            {
                vm.m_PTPSpeedPercentValue = (int)slider_PTPSpeedPercent.Maximum;
            }
        }

        private void slider_PTPSpeedPercentShow_TextChanged(object sender, TextChangedEventArgs e)
        {
            double dvalue = 0.0;
            bool error = Double.TryParse(slider_PTPSpeedPercentShow.Text, out dvalue);
            slider_PTPSpeedPercentShow.Text = Math.Round(dvalue).ToString();
            Int16 val = Convert.ToInt16(slider_PTPSpeedPercentShow.Text);
            HWinRobot.set_ptp_speed(HiWinRobotInterface.m_RobotConnectID, val);
        }

        private void slider_LinearSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            HIKRobotVM? vm = this.DataContext as HIKRobotVM;
            if (vm == null)
                return;

            if (slider_LinearSpeed.Value < slider_LinearSpeed.Minimum)
            {
                vm.m_nLinearSpeedValue = (int)slider_LinearSpeed.Minimum;
            }
            else if (slider_LinearSpeed.Value <= slider_LinearSpeed.Maximum)
            {
                vm.m_nLinearSpeedValue = (int)slider_LinearSpeed.Maximum;
            }
        }

        private void slider_LinearSpeedShow_TextChanged(object sender, TextChangedEventArgs e)
        {
            double dvalue = 0.0;
            bool error = Double.TryParse(slider_LinearSpeedShow.Text, out dvalue);
            slider_LinearSpeedShow.Text = Math.Round(dvalue).ToString();
            Int16 val = Convert.ToInt16(slider_LinearSpeedShow.Text);
            HWinRobot.set_lin_speed(HiWinRobotInterface.m_RobotConnectID, val);
        }

        private void slider_OverridePercent_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            HIKRobotVM? vm = this.DataContext as HIKRobotVM;
            if (vm == null)
                return;

            if (slider_OverridePercent.Value < slider_OverridePercent.Minimum)
            {
                vm.m_nOverridePercent = (int)slider_OverridePercent.Minimum;
            }
            else if (slider_OverridePercent.Value <= slider_OverridePercent.Maximum)
            {
                vm.m_nOverridePercent = (int)slider_OverridePercent.Maximum;
            }
        }

        private void slider_OverridePercentShow_TextChanged(object sender, TextChangedEventArgs e)
        {
            double dvalue = 0.0;
            bool error = Double.TryParse(slider_OverridePercentShow.Text, out dvalue);
            slider_OverridePercentShow.Text = Math.Round(dvalue).ToString();
            Int16 val = Convert.ToInt16(slider_OverridePercentShow.Text);
            HWinRobot.set_override_ratio(HiWinRobotInterface.m_RobotConnectID, val);
        }

        private void toggle_ServoOnOff_Click(object sender, RoutedEventArgs e)
        {
            MainWindowVM.master.m_hiWinRobotInterface.StopMotor();
            LogMessage.WriteToDebugViewer(2, "Stop Move");

            int bServoOnOff = HWinRobot.get_motor_state(HiWinRobotInterface.m_RobotConnectID);
            if (bServoOnOff == 0)
            {
                HWinRobot.set_motor_state(HiWinRobotInterface.m_RobotConnectID, 1);
                toggle_ServoOnOff.Background = new SolidColorBrush(Colors.Green);
                toggle_ServoOnOff.Content = "Servo ON";

            }
            else
            {
                HWinRobot.set_motor_state(HiWinRobotInterface.m_RobotConnectID, 0);
                toggle_ServoOnOff.Background = new SolidColorBrush(Colors.Gray);
                toggle_ServoOnOff.Content = "Servo OFF";

            }
            //toggle_ServoOnOff.IsChecked = bServoOnOff == 0 ? false : true;
        }

        public void SetMotorSpeed()
        {
            HWinRobot.set_acc_dec_ratio(HiWinRobotInterface.m_RobotConnectID, Convert.ToInt16(slider_AccRatioPercent.Value));
            HWinRobot.set_ptp_speed(HiWinRobotInterface.m_RobotConnectID, Convert.ToInt16(slider_PTPSpeedPercent.Value));
            HWinRobot.set_lin_speed(HiWinRobotInterface.m_RobotConnectID, Convert.ToInt16(slider_LinearSpeed.Value));
            HWinRobot.set_override_ratio(HiWinRobotInterface.m_RobotConnectID, Convert.ToInt16(slider_OverridePercent.Value));
        }


        private void button_Add_Point_To_Sequence_Click(object sender, RoutedEventArgs e)
        {
            SetMotorSpeed();
            double[] d_XYZvalue = new double[6];
            HWinRobot.get_current_position(HiWinRobotInterface.m_RobotConnectID, d_XYZvalue);
            double[] d_Jointvalue = new double[6];
            HWinRobot.get_current_joint(HiWinRobotInterface.m_RobotConnectID, d_Jointvalue);

            HIKRobotVM.m_List_sequencePointData.Add(HiWinRobotInterface.AddSequencePointInfo(HiWinRobotInterface.m_RobotConnectID, d_XYZvalue, d_Jointvalue, HIKRobotVM.m_List_sequencePointData.Count, ""));
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                if (MainWindowVM.master == null)
                    return;
                dataGrid_all_robot_Positions.ItemsSource = null;
                dataGrid_all_robot_Positions.ItemsSource = HIKRobotVM.m_List_sequencePointData;
            }));
        }
        private void button_Delete_Point_To_Sequence_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid_all_robot_Positions.SelectedIndex >= HIKRobotVM.m_List_sequencePointData.Count || dataGrid_all_robot_Positions.SelectedIndex < 0)
                return;

            //string[] strListCommentCannotDelete = { "Lower Soft Limit", "Home Position", "Higher Soft Limit" };

            //if (m_List_sequencePointData[dataGrid_all_robot_Positions.SelectedIndex].m_PointComment == strListCommentCannotDelete[0] ||
            //   m_List_sequencePointData[dataGrid_all_robot_Positions.SelectedIndex].m_PointComment == strListCommentCannotDelete[1] ||
            //   m_List_sequencePointData[dataGrid_all_robot_Positions.SelectedIndex].m_PointComment == strListCommentCannotDelete[2])
            //    return;

            HIKRobotVM.m_List_sequencePointData.RemoveAt(dataGrid_all_robot_Positions.SelectedIndex);
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                if (MainWindowVM.master == null)
                    return;
                dataGrid_all_robot_Positions.ItemsSource = null;
                dataGrid_all_robot_Positions.ItemsSource = HIKRobotVM.m_List_sequencePointData;
            }));
        }


        private void button_setTo_ChoosenPos_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid_all_robot_Positions.SelectedIndex >= HIKRobotVM.m_List_sequencePointData.Count || dataGrid_all_robot_Positions.SelectedIndex < 0)
                return;

            SetMotorSpeed();

            double[] d_XYZvalue = new double[6];
            HWinRobot.get_current_position(HiWinRobotInterface.m_RobotConnectID, d_XYZvalue);
            double[] d_Jointvalue = new double[6];
            HWinRobot.get_current_joint(HiWinRobotInterface.m_RobotConnectID, d_Jointvalue);

            HIKRobotVM.m_List_sequencePointData[dataGrid_all_robot_Positions.SelectedIndex] = HiWinRobotInterface.AddSequencePointInfo(HiWinRobotInterface.m_RobotConnectID, d_XYZvalue, d_Jointvalue, HIKRobotVM.m_List_sequencePointData.Count, HIKRobotVM.m_List_sequencePointData[dataGrid_all_robot_Positions.SelectedIndex].m_PointComment);
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                if (MainWindowVM.master == null)
                    return;
                dataGrid_all_robot_Positions.ItemsSource = null;
                dataGrid_all_robot_Positions.ItemsSource = HIKRobotVM.m_List_sequencePointData;
            }));
        }

        private void button_Save_Sequence_Click(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.MessageBox.Show("Would you like to save all points to Excel file ?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                HiWinRobotInterface.SequencePointData.SaveRobotPointsToExcel(HIKRobotVM.m_List_sequencePointData);
                HiWinRobotInterface.SequencePointData.ReadRobotPointsFromExcel(ref HIKRobotVM.m_List_sequencePointData);
                HIKRobotVM.Docalibration(HIKRobotVM.m_List_sequencePointData);
            }
        }

        int mComboSelectedItem_Backup = 0;
        string[] m_strXYZMovingButtonLabel = { "X", "Y", "Z", "RTZ" };
        string[] m_strJointMovingButtonLabe = { "A1", "A2", "A3", "A4" };

        private void combo_JogType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (combo_JogType.SelectedIndex != mComboSelectedItem_Backup)
            {
                mComboSelectedItem_Backup = combo_JogType.SelectedIndex;
                SetMovingButtonLabel();
            }
        }

        public void SetMovingButtonLabel()
        {
            if (combo_JogType == null)
                return;

            if (combo_JogType.Items.Count == 0)
                return;

            if (combo_JogType.SelectedIndex < 0)
            {
                combo_JogType.SelectedIndex = 0;
            }

            if (combo_JogType.SelectedItem.ToString() == "XYZ")
            {
                label_move_Motor1.Content = m_strXYZMovingButtonLabel[0];
                label_move_Motor2.Content = m_strXYZMovingButtonLabel[1];
                label_move_Motor3.Content = m_strXYZMovingButtonLabel[2];
                label_move_Motor4.Content = m_strXYZMovingButtonLabel[3];


            }
            else
            {
                label_move_Motor1.Content = m_strJointMovingButtonLabe[0];
                label_move_Motor2.Content = m_strJointMovingButtonLabe[1];
                label_move_Motor3.Content = m_strJointMovingButtonLabe[2];
                label_move_Motor4.Content = m_strJointMovingButtonLabe[3];

            }

        }

        private void combo_MoveType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void slider_StepRelativeShow_TextChanged(object sender, TextChangedEventArgs e)
        {
            HIKRobotVM vm = this.DataContext as HIKRobotVM;
            if (vm == null)
                return;

            double dvalue = 0.0;
            Double.TryParse(slider_StepRelativeShow.Text, out dvalue);
            vm.m_nStepRelativeValue = (int)Math.Round(dvalue);
            slider_StepRelativeShow.Text = vm.m_nStepRelativeValue.ToString();
        }

        private void dataGrid_all_robot_Positions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dataGrid_all_robot_Positions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGrid_all_robot_Positions.CurrentColumn == null)
                return;
            if (dataGrid_all_robot_Positions.CurrentColumn.DisplayIndex > 1)
                return;

            if (dataGrid_all_robot_Positions.SelectedIndex >= 0 && dataGrid_all_robot_Positions.SelectedIndex < HIKRobotVM.m_List_sequencePointData.Count)
            {
                //int nIndex = combo_JogType.SelectedIndex;

                if (combo_JogType.SelectedIndex == (int)JOG_TYPE.JOG_XYZ)
                {
                    double[] dpos = new double[6];
                    HIKRobotVM.m_List_sequencePointData[dataGrid_all_robot_Positions.SelectedIndex].GetXYZPoint(ref dpos);
                    m_Thread = new System.Threading.Thread(new System.Threading.ThreadStart(() => HWinRobot.ptp_pos(HiWinRobotInterface.m_RobotConnectID, 0, dpos)));
                    m_Thread.IsBackground = true;
                    m_Thread.Start();
                }

                else if (combo_JogType.SelectedIndex == (int)JOG_TYPE.JOG_JOINT)
                {
                    double[] dpos = new double[6];
                    HIKRobotVM.m_List_sequencePointData[dataGrid_all_robot_Positions.SelectedIndex].GetJointPoint(ref dpos);
                    m_Thread = new System.Threading.Thread(new System.Threading.ThreadStart(() => HWinRobot.ptp_axis(HiWinRobotInterface.m_RobotConnectID, 0, dpos)));
                    m_Thread.IsBackground = true;
                    m_Thread.Start();
                }

            }
        }

        private void button_Stop_Moving_Click(object sender, RoutedEventArgs e)
        {
            MainWindowVM.master.m_hiWinRobotInterface.StopMotor();
            LogMessage.WriteToDebugViewer(2, "Stop Move Clicked");
        }

        Thread m_Thread;
        private void button_negative_Move1_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            int nIndex = combo_MoveTypes.SelectedIndex;
            m_Thread = new System.Threading.Thread(new System.Threading.ThreadStart(() => MoveMotor(0, nIndex, -1)));
            m_Thread.IsBackground = true;
            m_Thread.Start();
        }

        private void button_positive_Move1_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            int nIndex = combo_MoveTypes.SelectedIndex;
            m_Thread = new System.Threading.Thread(new System.Threading.ThreadStart(() => MoveMotor(0, nIndex, 1)));
            m_Thread.IsBackground = true;
            m_Thread.Start();
        }

        private void button_negative_Move2_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            int nIndex = combo_MoveTypes.SelectedIndex;
            m_Thread = new System.Threading.Thread(new System.Threading.ThreadStart(() => MoveMotor(1, nIndex, -1)));
            m_Thread.IsBackground = true;
            m_Thread.Start();
        }

        private void button_positive_Move2_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            int nIndex = combo_MoveTypes.SelectedIndex;
            m_Thread = new System.Threading.Thread(new System.Threading.ThreadStart(() => MoveMotor(1, nIndex, 1)));
            m_Thread.IsBackground = true;
            m_Thread.Start();
        }


        private void button_negative_Move3_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            int nIndex = combo_MoveTypes.SelectedIndex;
            m_Thread = new System.Threading.Thread(new System.Threading.ThreadStart(() => MoveMotor(2, nIndex, -1)));
            m_Thread.IsBackground = true;
            m_Thread.Start();
        }

        private void button_positive_Move3_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            int nIndex = combo_MoveTypes.SelectedIndex;
            m_Thread = new System.Threading.Thread(new System.Threading.ThreadStart(() => MoveMotor(2, nIndex, 1)));
            m_Thread.IsBackground = true;
            m_Thread.Start();
        }

        private void button_negative_Move4_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            int nIndex = combo_MoveTypes.SelectedIndex;
            m_Thread = new System.Threading.Thread(new System.Threading.ThreadStart(() => MoveMotor(5, nIndex, -1)));
            m_Thread.IsBackground = true;
            m_Thread.Start();
        }

        private void button_positive_Move4_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            int nIndex = combo_MoveTypes.SelectedIndex;
            m_Thread = new System.Threading.Thread(new System.Threading.ThreadStart(() => MoveMotor(5, nIndex, 1)));
            m_Thread.IsBackground = true;
            m_Thread.Start();
        }

        public void MoveZMotorUp(int nValue)
        {
            double[] dValue = { 0, 0, 0, 0, 0, 0 };
            dValue[2] = nValue;
            HWinRobot.ptp_rel_pos(HiWinRobotInterface.m_RobotConnectID, 0, dValue);

        }

        public void MoveMotor(int nMotorID, int nType, int ndirection)
        {
            HIKRobotVM vm = this.DataContext as HIKRobotVM;
            if (vm == null)
                return;

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (HWinRobot.get_motion_state(HiWinRobotInterface.m_RobotConnectID) != 1)
                    return;

                //MainWindowVM.master.m_hiWinRobotInterface.wait_for_stop_motion(HiWinRobotInterface.m_DeviceID);
                if (combo_MoveTypes.SelectedIndex == (int)MOVETYPES.AbsoluteMove)
                {
                    if (combo_JogType.SelectedIndex == (int)JOG_TYPE.JOG_XYZ)
                    {
                        double[] dValue = new double[6];
                        HWinRobot.get_current_position(HiWinRobotInterface.m_RobotConnectID, dValue);
                        dValue[nMotorID] = (Double)(vm.m_nStepRelativeValue / 1000.0);
                        HWinRobot.ptp_pos(HiWinRobotInterface.m_RobotConnectID, 0, dValue);
                    }
                    else
                    {
                        //Note: ONLY correct in Scara Robot with maximum motor = 4
                        if (nMotorID >= HiWinRobotInterface.NUMBER_AXIS)
                            nMotorID = HiWinRobotInterface.NUMBER_AXIS - 1;

                        double[] dValue = new double[6];
                        HWinRobot.get_current_joint(HiWinRobotInterface.m_RobotConnectID, dValue);
                        dValue[nMotorID] = (Double)(vm.m_nStepRelativeValue / 1000.0);
                        HWinRobot.ptp_axis(HiWinRobotInterface.m_RobotConnectID, 0, dValue);
                    }
                }
                else
                {


                    if (combo_JogType.SelectedIndex == (int)JOG_TYPE.JOG_XYZ)
                    {
                        double[] dValue = { 0, 0, 0, 0, 0, 0 };
                        dValue[nMotorID] = (Double)(Math.Abs(vm.m_nStepRelativeValue) / 1000.0) * ndirection;
                        HWinRobot.ptp_rel_pos(HiWinRobotInterface.m_RobotConnectID, 0, dValue);
                    }
                    else
                    {
                        //Noted: ONLY correct in Scara Robot with maximum motor = 4
                        if (nMotorID >= HiWinRobotInterface.NUMBER_AXIS)
                            nMotorID = HiWinRobotInterface.NUMBER_AXIS - 1;
                        double[] dValue = { 0, 0, 0, 0, 0, 0 };
                        dValue[nMotorID] = (Double)(Math.Abs(   vm.m_nStepRelativeValue) / 1000.0) * ndirection;
                        HWinRobot.ptp_rel_axis(HiWinRobotInterface.m_RobotConnectID, 0, dValue);
                    }
                }
                //dValue[nMotorID] += (Double)(Math.Abs(m_nStepRelativeValue) / 1000.0) * ndirection;

                //MainWindowVM.master.m_hiWinRobotInterface.wait_for_stop_motion(HiWinRobotInterface.m_DeviceID);
                //double[] dValue2 = { 2, 0, 0, 0, 0, 1};
                //HWinRobot.ptp_rel_pos(HiWinRobotInterface.m_DeviceID, 0, dValue2);
            });
        }

        public static SequencePointData GetPointData(string strComment)
        {
            SequencePointData data = new SequencePointData();
            for (int nIndex = 0; nIndex < HIKRobotVM.m_List_sequencePointData.Count; nIndex++)
            {
                if (HIKRobotVM.m_List_sequencePointData[nIndex].m_PointComment == strComment)
                {
                    return HIKRobotVM.m_List_sequencePointData[nIndex];
                }
            }

            return data;
        }

        public static int CheckPointExistOnList(ref List<SequencePointData> list_data, string strComment)
        {
            SequencePointData data = new SequencePointData();
            for (int nIndex = 0; nIndex < list_data.Count; nIndex++)
            {
                if (list_data[nIndex].m_PointComment == strComment)
                {
                    return nIndex;
                }
            }

            return -1;
        }




        public static int MoveToPrePickPosition(int ndeviceid, System.Drawing.PointF pRobotPos, int nSequenceStep)
        {
            for (int nIndex = 0; nIndex < HIKRobotVM.m_List_sequencePointData.Count; nIndex++)
            {
                if (HIKRobotVM.m_List_sequencePointData[nIndex].m_PointComment == SequencePointData.PRE_PICK_POSITION)
                {
                    double[] dPos = new double[6];
                    HIKRobotVM.m_List_sequencePointData[nIndex].GetXYZPoint(ref dPos);
                    dPos[0] = pRobotPos.X;
                    dPos[1] = pRobotPos.Y;
                    return HWinRobot.ptp_pos(HiWinRobotInterface.m_RobotConnectID, 0, dPos);
                }
            }
            //HiWinRobotInterface.HomeMove();
            return -99;
        }



        private void button_Home_Move_Click(object sender, RoutedEventArgs e)
        {
            //int nIndex = combo_MoveTypes.SelectedIndex;
            m_Thread = new System.Threading.Thread(new System.Threading.ThreadStart(() => HiWinRobotInterface.HomeMove()));
            m_Thread.IsBackground = true;
            m_Thread.Start();

        }

        private void button_ResetAlarm_Click(object sender, RoutedEventArgs e)
        {
            HWinRobot.clear_alarm(HiWinRobotInterface.m_RobotConnectID);
            label_Alarm.Text = "";

        }

        public bool b_button_RobotConnect = true;
        private void button_RobotConnect_Checked(object sender, RoutedEventArgs e)
        {
            b_button_RobotConnect = (bool)button_RobotConnect.IsChecked;
            if (MainWindowVM.master == null)
                return;

            HiWinRobotInterface.m_strRobotIPAddress = FileHelper.GetCommInfo("Robot Comm::IpAddress", HiWinRobotInterface.m_strRobotIPAddress, AppMagnus.pathRegistry);
            HIKRobotVM vm = this.DataContext as HIKRobotVM;
            if (vm == null)
                return;

            vm.txtRobotIPAddress = HiWinRobotInterface.m_strRobotIPAddress;
        }

        private void button_RobotConnect_Unchecked(object sender, RoutedEventArgs e)
        {
            b_button_RobotConnect = (bool)button_RobotConnect.IsChecked;

        }

        public void GetHomePosition()
        {
            //SetMotorSpeed();
            double[] dHomePos = new double[6];
            HWinRobot.get_home_point(m_RobotConnectID, dHomePos);

            double[] dXYZPos = new double[6];
            int nHomePointIndex = -1;
            for (int nIndex = 0; nIndex < HIKRobotVM.m_List_sequencePointData.Count; nIndex++)
            {
                if (HIKRobotVM.m_List_sequencePointData[nIndex].m_PointComment == SequencePointData.HOME_POSITION)
                    nHomePointIndex = nIndex;
            }
            if (nHomePointIndex >= 0)
            {
                HIKRobotVM.m_List_sequencePointData[nHomePointIndex].GetXYZPoint(ref dXYZPos);
                HIKRobotVM.m_List_sequencePointData[nHomePointIndex] = HiWinRobotInterface.AddSequencePointInfo(HiWinRobotInterface.m_RobotConnectID, dXYZPos, dHomePos, HIKRobotVM.m_List_sequencePointData.Count, "Home Position");
            }
            else
                HIKRobotVM.m_List_sequencePointData.Add(HiWinRobotInterface.AddSequencePointInfo(HiWinRobotInterface.m_RobotConnectID, dXYZPos, dHomePos, HIKRobotVM.m_List_sequencePointData.Count, "Home Position"));

            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                if (MainWindowVM.master == null)
                    return;
                dataGrid_all_robot_Positions.ItemsSource = null;
                dataGrid_all_robot_Positions.ItemsSource = HIKRobotVM.m_List_sequencePointData;
            }));
        }
        private void button_GetHomePosition_Click(object sender, RoutedEventArgs e)
        {
            GetHomePosition();
        }

        public void SetHomePosition()
        {
            if (dataGrid_all_robot_Positions.SelectedIndex < HIKRobotVM.m_List_sequencePointData.Count && dataGrid_all_robot_Positions.SelectedIndex >= 0)
            {
                double[] dPos = new double[6];
                HIKRobotVM.m_List_sequencePointData[dataGrid_all_robot_Positions.SelectedIndex].GetXYZPoint(ref dPos);

                double[] dJointPos = new double[6];

                HIKRobotVM.m_List_sequencePointData[dataGrid_all_robot_Positions.SelectedIndex].GetJointPoint(ref dJointPos);

                int nHomePointIndex = -1;
                for (int nIndex = 0; nIndex < HIKRobotVM.m_List_sequencePointData.Count; nIndex++)
                {
                    if (HIKRobotVM.m_List_sequencePointData[nIndex].m_PointComment == "Home Position")
                        nHomePointIndex = nIndex;
                }
                if (nHomePointIndex >= 0)
                {
                    HIKRobotVM.m_List_sequencePointData[nHomePointIndex].SetXYZPoint(dPos);
                    HIKRobotVM.m_List_sequencePointData[nHomePointIndex].SetJointPoint(dJointPos);

                }
                else
                    HIKRobotVM.m_List_sequencePointData[dataGrid_all_robot_Positions.SelectedIndex].m_PointComment = "Home Position";

                HWinRobot.set_home_point(HiWinRobotInterface.m_RobotConnectID, dJointPos);
                //HiWinRobotInterface.SetHome(m_DeviceID, dPos, combo_JogType.SelectedIndex);

            }

            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                if (MainWindowVM.master == null)
                    return;
                dataGrid_all_robot_Positions.ItemsSource = null;
                dataGrid_all_robot_Positions.ItemsSource = HIKRobotVM.m_List_sequencePointData;
            }));
        }

        private void button_SetHomePosition_Click(object sender, RoutedEventArgs e)
        {
            SetHomePosition();
        }

        public void GetSoftLimit()
        {
            //double[] dlowSoftLimit = new double[6];
            //double[] dhighSoftLimit = new double[6];
            //bool b_rel = false;
            //HiWinRobotInterface.GetSoftLimit(HiWinRobotInterface.m_RobotConnectID, (int)JOG_TYPE.JOG_XYZ, ref b_rel, ref dlowSoftLimit, ref dhighSoftLimit);

            //double[] dlowJointSoftLimit = new double[6];
            //double[] dhighJointSoftLimit = new double[6];
            //HiWinRobotInterface.GetSoftLimit(HiWinRobotInterface.m_RobotConnectID, (int)JOG_TYPE.JOG_JOINT, ref b_rel, ref dlowJointSoftLimit, ref dhighJointSoftLimit);

            //int nLowIndex = -1;
            //int nHighIndex = -1;
            //for (int nIndex = 0; nIndex < m_List_sequencePointData.Count; nIndex++)
            //{
            //    if (m_List_sequencePointData[nIndex].m_PointComment == "Lower Soft Limit")
            //        nLowIndex = nIndex;

            //    if (m_List_sequencePointData[nIndex].m_PointComment == "Higher Soft Limit")
            //        nHighIndex = nIndex;
            //}
            //if (nLowIndex >= 0)
            //    m_List_sequencePointData[nLowIndex] = HiWinRobotInterface.AddSequencePointInfo(HiWinRobotInterface.m_RobotConnectID, dlowSoftLimit, dlowJointSoftLimit, nLowIndex + 1, "Lower Soft Limit");
            //else
            //    m_List_sequencePointData.Add(HiWinRobotInterface.AddSequencePointInfo(HiWinRobotInterface.m_RobotConnectID, dlowSoftLimit, dlowJointSoftLimit, m_List_sequencePointData.Count, "Lower Soft Limit"));

            //if (nHighIndex >= 0)
            //    m_List_sequencePointData[nHighIndex] = HiWinRobotInterface.AddSequencePointInfo(HiWinRobotInterface.m_RobotConnectID, dhighSoftLimit, dlowJointSoftLimit, nHighIndex + 1, "Higher Soft Limit");
            //else
            //    m_List_sequencePointData.Add(HiWinRobotInterface.AddSequencePointInfo(HiWinRobotInterface.m_RobotConnectID, dhighSoftLimit, dhighJointSoftLimit, m_List_sequencePointData.Count, "Higher Soft Limit"));

            //System.Windows.Application.Current.Dispatcher.Invoke(() =>
            //{
            //    if (MainWindowVM.master == null)
            //        return;
            //    dataGrid_all_robot_Positions.ItemsSource = null;
            //    dataGrid_all_robot_Positions.ItemsSource = m_List_sequencePointData;
            //});
        }
        private void button_GetSoftLimit_Click(object sender, RoutedEventArgs e)
        {
            GetSoftLimit();
        }

        public void SetSoftLimit(int nLimit = 0)
        {
            //if (dataGrid_all_robot_Positions.SelectedIndex < m_List_sequencePointData.Count && dataGrid_all_robot_Positions.SelectedIndex >= 0)
            //{

            //    double[] dPos = new double[6];
            //    double[] dJoint = new double[6];
            //    double[] dLowPos = new double[6];
            //    double[] dHighPos = new double[6];
            //    bool bRe = false;
            //    HiWinRobotInterface.GetSoftLimit(m_RobotConnectID, combo_JogType.SelectedIndex, ref bRe, ref dLowPos, ref dHighPos);

            //    if (combo_JogType.SelectedIndex == (int)JOG_TYPE.JOG_JOINT)
            //    {
            //        m_List_sequencePointData[dataGrid_all_robot_Positions.SelectedIndex].GetJointPoint(ref dJoint);
            //    }
            //    else
            //        m_List_sequencePointData[dataGrid_all_robot_Positions.SelectedIndex].GetXYZPoint(ref dPos);

            //    int nSoftPointIndex = -1;
            //    string[] strComment = { "Lower Soft Limit", "Higher Soft Limit" };
            //    for (int nIndex = 0; nIndex < m_List_sequencePointData.Count; nIndex++)
            //    {
            //        if (m_List_sequencePointData[nIndex].m_PointComment == strComment[nLimit])
            //            nSoftPointIndex = nIndex;
            //    }

            //    if (nSoftPointIndex >= 0)
            //    {
            //        if (combo_JogType.SelectedIndex == (int)JOG_TYPE.JOG_JOINT)
            //            m_List_sequencePointData[nSoftPointIndex].SetXYZPoint(dPos);//
            //        else
            //            m_List_sequencePointData[nSoftPointIndex].SetJointPoint(dPos);//

            //    }
            //    else
            //        m_List_sequencePointData[dataGrid_all_robot_Positions.SelectedIndex].m_PointComment = strComment[nLimit];


            //    if (nLimit == 0)
            //        HiWinRobotInterface.SetSoftLimit(m_RobotConnectID, dPos, dHighPos, combo_JogType.SelectedIndex);
            //    else
            //        HiWinRobotInterface.SetSoftLimit(m_RobotConnectID, dLowPos, dPos, combo_JogType.SelectedIndex);
            //}

            //System.Windows.Application.Current.Dispatcher.Invoke(() =>
            //{
            //    if (MainWindowVM.master == null)
            //        return;
            //    dataGrid_all_robot_Positions.ItemsSource = null;
            //    dataGrid_all_robot_Positions.ItemsSource = m_List_sequencePointData;
            //});

        }


        private void button_SetLowerSoftLimit_Click(object sender, RoutedEventArgs e)
        {
            //SetSoftLimit(0);
        }

        private void button_SetHigherSoftLimit_Click(object sender, RoutedEventArgs e)
        {
            //SetSoftLimit(1);
        }

        private void button_Import_Task_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_Start_Task_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_Abort_Task_Click(object sender, RoutedEventArgs e)
        {

        }

        private void check_Manual_Checked(object sender, RoutedEventArgs e)
        {
            if (HiWinRobotInterface.m_RobotConnectID >= 0)
                HWinRobot.set_operation_mode(HiWinRobotInterface.m_RobotConnectID, (int)ROBOT_OPERATION_MODE.MODE_MANUAL);
            //HWinRobot.disconnect(HiWinRobotInterface.m_DeviceID);
            //if(MainWindowVM.master !=null)
            //    MainWindowVM.master.m_hiWinRobotInterface.ReconnectToHIKRobot();
        }

        private void check_Auto_Checked(object sender, RoutedEventArgs e)
        {
            if (HiWinRobotInterface.m_RobotConnectID >= 0)
                HWinRobot.set_operation_mode(HiWinRobotInterface.m_RobotConnectID, (int)ROBOT_OPERATION_MODE.MODE_AUTO);
            //HWinRobot.disconnect(HiWinRobotInterface.m_DeviceID);
            //if (MainWindowVM.master != null)
            //    MainWindowVM.master.m_hiWinRobotInterface.ReconnectToHIKRobot();
        }

        private void dataGrid_robot_Output_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (HWinRobot.get_connection_level(HiWinRobotInterface.m_RobotConnectID) == 0)
                return;

            if (dataGrid_robot_Output.SelectedIndex < 0)
                return;

            int nvalue = HWinRobot.get_digital_output(HiWinRobotInterface.m_RobotConnectID, dataGrid_robot_Output.SelectedIndex + 1);
            bool bIOStatus = nvalue == 1 ? true : false;
            HWinRobot.set_digital_output(HiWinRobotInterface.m_RobotConnectID, dataGrid_robot_Output.SelectedIndex + 1, !bIOStatus);
        }





        public int WaitForNextStepCalibrationEvent(string strDebugMessage = "")
        {


            lock (label_Alarm)
            {

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    label_Alarm.Text = strDebugMessage;
                });
            }

            while (!m_NextStepCalibration.WaitOne(10))
            {
                //if (MainWindow.mainWindow == null)
                //    return -1;
            }
            return 0;
        }



        public static AutoResetEvent m_NextStepCalibration = new AutoResetEvent(false);
        private void button_Next_Calibration_Click(object sender, RoutedEventArgs e)
        {
            m_NextStepCalibration.Set();
        }


        private void button_CameraRobotCalibration_Click(object sender, RoutedEventArgs e)
        {
            //if (m_CalibrationThread == null)
            //    m_CalibrationThread = new System.Threading.Thread(new System.Threading.ThreadStart(() => CalibrationSequence()));
            //else if (!m_CalibrationThread.IsAlive)
            //    m_CalibrationThread = new System.Threading.Thread(new System.Threading.ThreadStart(() => CalibrationSequence()));
            //else
            //    return;

            //label_Alarm.Text = "Start Calibration";
            //m_NextStepCalibration.Reset();
            //m_CalibrationThread.IsBackground = true;
            //m_CalibrationThread.Start();
        }

        //public void CalibrationSequence()
        //{
        //    //int nAlarmCount = 0;
        //    //HWinRobot.get_alarm_log_count(HiWinRobotInterface.m_RobotConnectID, ref nAlarmCount);
        //    HWinRobot.clear_alarm(HiWinRobotInterface.m_RobotConnectID);
        //    System.Windows.Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        button_Next_Calibration.IsEnabled = true;
        //    });
        //    if (WaitForNextStepCalibrationEvent("Calibration sequence!. Press OK to Home Move and move to ready position") < 0)
        //        return;
        //    System.Windows.Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        button_Next_Calibration.IsEnabled = false;
        //    });
        //    HomeMove();
        //    MainWindowVM.master.m_hiWinRobotInterface.MoveTo_STATIC_POSITION(SequencePointData.READY_POSITION);
        //    // Please put calibration jig to the workplace 


        //    System.Windows.Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        button_Next_Calibration.IsEnabled = true;
        //    });
        //    if (WaitForNextStepCalibrationEvent("Please put calibration jig to the workplace then press Next to trigger camera 1 to get calibration position") < 0)
        //        return;

        //    System.Windows.Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        button_Next_Calibration.IsEnabled = false;
        //    });
        //    if (MainWindowVM.master.m_Tracks[0].SingleSnap_HIKCamera() < 0)
        //    {
        //        MessageBox.Show("Cannot open camera 1. Please Check camera connection again! Stop calibration...", "", MessageBoxButton.OK);
        //        return;
        //    }

        //    System.Drawing.PointF[] vision_points;
        //    System.Drawing.PointF[] robot_points = new System.Drawing.PointF[3];
        //    if(MainWindowVM.master.m_Tracks[0].CalibrationGet3Points(out vision_points) < 0)
        //    {
        //        MessageBox.Show("Calibration sequence failed to get vision points, please check the lower and higher threshold or the lighting...", "", MessageBoxButton.OKCancel);
        //        return;
        //    }

        //    System.Windows.Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        button_Next_Calibration.IsEnabled = true;
        //    });
        //    if (WaitForNextStepCalibrationEvent("Done. Please press Next to move the robot to 1st Point.") < 0)
        //        return;

        //    System.Windows.Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        button_Next_Calibration.IsEnabled = false;
        //    });
        //    if (MainWindowVM.master.m_hiWinRobotInterface.MoveTo_STATIC_POSITION(SequencePointData.CALIB_ROBOT_POSITION_1) < 0)
        //    {
        //        if (MessageBox.Show("Move Failed, please click OK then reset alarm and manually move the robot to position 1", "", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
        //        {
        //            MessageBox.Show("Calibration sequence cancel...", "", MessageBoxButton.OKCancel);

        //            return;
        //        }

        //    }

        //    MainWindowVM.master.m_hiWinRobotInterface.wait_for_stop_motion();
        //    System.Windows.Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        button_Next_Calibration.IsEnabled = true;
        //    });
        //    if (WaitForNextStepCalibrationEvent(" Done. Please press Next to save and move the robot to 2nd Point.") < 0)
        //        return;

        //    int nPointIndex = 0;
        //    double[] drobotPoint = new double[6];
        //    //SequencePointData pData = MainWindowVM.master.m_hiWinRobotInterface.m_hiWinRobotUserControl.GetPointData(SequencePointData.CALIB_ROBOT_POSITION_1);
        //    //pData.GetXYZPoint(ref drobotPoint);
        //    HWinRobot.get_current_position(HiWinRobotInterface.m_RobotConnectID, drobotPoint);
        //    robot_points[nPointIndex++] = new System.Drawing.PointF((float)drobotPoint[0], (float)drobotPoint[1]);
        //    System.Windows.Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        button_Next_Calibration.IsEnabled = false;
        //    });
        //    if (MainWindowVM.master.m_hiWinRobotInterface.MoveTo_STATIC_POSITION(SequencePointData.CALIB_ROBOT_POSITION_2) < 0)
        //    {
        //        if (MessageBox.Show("Move Failed, please click OK then reset alarm and manually move the robot to position 2", "", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
        //        {
        //            MessageBox.Show("Calibration sequence cancel...", "", MessageBoxButton.OKCancel);

        //            return;
        //        }

        //    }
        //    MainWindowVM.master.m_hiWinRobotInterface.wait_for_stop_motion();
        //    System.Windows.Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        button_Next_Calibration.IsEnabled = true;
        //    });
        //    if (WaitForNextStepCalibrationEvent(" Done. Please press Next to save and move the robot to 3rd Point.") < 0)
        //        return;

        //    System.Windows.Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        button_Next_Calibration.IsEnabled = false;
        //    });
        //    HWinRobot.get_current_position(HiWinRobotInterface.m_RobotConnectID, drobotPoint);
        //    robot_points[nPointIndex++] = new System.Drawing.PointF((float)drobotPoint[0], (float)drobotPoint[1]);

        //    if (MainWindowVM.master.m_hiWinRobotInterface.MoveTo_STATIC_POSITION(SequencePointData.CALIB_ROBOT_POSITION_3) < 0)
        //    {
        //        if (MessageBox.Show("Move Failed, please click OK then reset alarm and manually move the robot to position 3", "", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
        //        {
        //            MessageBox.Show("Calibration sequence cancel...", "", MessageBoxButton.OKCancel);

        //            return;
        //        }

        //    }
        //    MainWindowVM.master.m_hiWinRobotInterface.wait_for_stop_motion();
        //    System.Windows.Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        button_Next_Calibration.IsEnabled = true;
        //    });
        //    if (WaitForNextStepCalibrationEvent(" Done. Please press Next to save the 3rd Point.") < 0)
        //        return;

        //    HWinRobot.get_current_position(HiWinRobotInterface.m_RobotConnectID, drobotPoint);
        //    robot_points[nPointIndex++] = new System.Drawing.PointF((float)drobotPoint[0], (float)drobotPoint[1]);

        //    if (MessageBox.Show("Would you like to save the calibration result ?", "", MessageBoxButton.YesNo) == MessageBoxResult.No)
        //        return;

        //    //Docalibration();
        //    Mat mat_CameraRobotTransform = Application.Track.MagnusMatrix.CalculateTransformMatrix(vision_points, robot_points);
        //    System.Windows.Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        button_Next_Calibration.IsEnabled = true;
        //    });
        //    if (WaitForNextStepCalibrationEvent("Calibration Done. Please press Next complete the sequence.") < 0)
        //        return;

        //}

        //public int SaveCalibrationData(System.Drawing.PointF[] vision_points, System.Drawing.PointF[] robot_points)
        //{
        //    if (vision_points.Length < 3 || robot_points.Length < 3)
        //        return -1;

        //    for (int nIndex = 0; nIndex < m_List_sequencePointData.Count; nIndex++)
        //    {
        //        if (m_List_sequencePointData[nIndex].m_PointComment == SequencePointData.CALIB_ROBOT_POSITION_1)
        //        {
        //           m_List_sequencePointData[nIndex].m_X = robot_points[0].X ;
        //           m_List_sequencePointData[nIndex].m_Y = robot_points[0].Y ;
        //        }
        //        if (m_List_sequencePointData[nIndex].m_PointComment == SequencePointData.CALIB_ROBOT_POSITION_2)
        //        {
        //            m_List_sequencePointData[nIndex].m_X = robot_points[1].X;
        //            m_List_sequencePointData[nIndex].m_Y = robot_points[1].Y;
        //        }

        //        if (m_List_sequencePointData[nIndex].m_PointComment == SequencePointData.CALIB_ROBOT_POSITION_3)
        //        {
        //            m_List_sequencePointData[nIndex].m_X = robot_points[2].X;
        //            m_List_sequencePointData[nIndex].m_Y = robot_points[2].Y;
        //        }

        //        if (m_List_sequencePointData[nIndex].m_PointComment == SequencePointData.CALIB_Vision_POSITION_1)
        //        {
        //            m_List_sequencePointData[nIndex].m_X = vision_points[0].X;
        //            m_List_sequencePointData[nIndex].m_Y = vision_points[0].Y;
        //        }
        //        if (m_List_sequencePointData[nIndex].m_PointComment == SequencePointData.CALIB_Vision_POSITION_2)
        //        {
        //            m_List_sequencePointData[nIndex].m_X = vision_points[1].X;
        //            m_List_sequencePointData[nIndex].m_Y = vision_points[1].Y;
        //        }

        //        if (m_List_sequencePointData[nIndex].m_PointComment == SequencePointData.CALIB_Vision_POSITION_3)
        //        {
        //            m_List_sequencePointData[nIndex].m_X = vision_points[2].X;
        //            m_List_sequencePointData[nIndex].m_Y = vision_points[2].Y;
        //        }
        //    }

        //    m_MatCameraRobotTransform = Application.Track.MagnusMatrix.CalculateTransformMatrix(vision_points, robot_points);
        //    SequencePointData.SaveRobotPointsToExcel(m_List_sequencePointData);

        //    return 0;
        //}


    }
}
