   M S B A M L   `   `   `  ���� L  HVisionApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:VisionApplication.HardwareVisionApplication.Hardware  6    0VisionApplication.Hardware.HiWinRobotUserControl   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     :local(clr-namespace:VisionApplication.Hardware     .��� 
�������  � RowDefinitions  �� $��1* ��� $
��3.4* ��� $��2* �.+�� $��0��$
��Auto��  �� $��25 ��� $��5* �.+��� $��0��$��Robot Control��
���iii�
���   ��� $��1��$
��Auto��$��0,1,0,1q�
���   �$
��Auto��._��  � ColumnDefinitions �� $��3* ��� $��2* ��� $
��0.7* �.+�� $��0�� �� $��4* ��� .+�� $��0�� �� $��50 ��� $��1* �.+��� $��0��$��Status��$��1q�
���   �$��Center=�
�������$��Center=��� -    ��Name$ label_Alarm��$��1��
�������$��WrapWithOverflow=��� -   $��1��$��Reset Alarm��� $��1��  �� $��1* ��� $��1* �.+��� $��0��$��1,0,1,1q�
�������._�� .+��� $��Robot IP Address: ��$
��Auto��$
��Left=��� -   $ text_RobotIPAddress��
�������$	��110��$��Right=�
���   �����  �� Path$ txtRobotIPAddress� �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=��� $��1��$��1q�
�������._��  �� $��1* ��� $��2* ��� $��2* �.+��� $��0��$��Model: ��$��Right=�	� -   $��check_Manual��$��1��$��Manual��$
��Left=�$��Center=�	� -   $��
check_Auto��$��2��$��10,0,0,0q�$
��Auto��$
��Left=�$��Center=�d� -    d�Name$ button_RobotConnect��$��2��$	��N/A��$
��Auto��$��Center=��� $��1��
�������$��2q�._��  �� $
��1.3* ��� $
��1.5* ��� $
��0.7* ��� $
��0.7* ��� $
��1.5* �.+�� $��0��$��0,5,5,0q�  �� $��1* ��� $��1* ��� $��1* ��� $��1* �.+��� $��0��$��Acc Ratio (0-100 %)��$��1q�
���   �$��Stretch=�$��Stretch=�.��� $��0,5,0,5q� �� $��1* ��� $��3* �.+��� -    ��Name$ slider_AccRatioPercent��$��1�� � Minimum$ 10Q�	 � Maximum$		 100Q�
 � SmallChange$
 1Q�$��Center=� � Value �� $
 Text� �� ElementName$  slider_AccRatioPercentShow��$ TwoWay=��� -    ��Name$  slider_AccRatioPercentShow��
������ 
���   � �� TextAlignment$ Center=�$��Center=�$��15(����� $ Value�$ m_nAccRatioPercentValue��$ TwoWay=�$ PropertyChanged=��� $��1��$��PTP Speed (0-100 %)��$��1q�
���   �$��Stretch=�$��Stretch=�.��� $��0,5,0,5q� �� $��1* ��� $��3* �.+��� -	   $ slider_PTPSpeedPercent��$��1��$ 10Q�$		 100Q�$
 1Q�$��Center=� �� $
 Text�$  slider_PTPSpeedPercentShow��$ TwoWay=��� -
   $  slider_PTPSpeedPercentShow��
������ 
���   �$ Center=�$��Center=�$��15(����� $ Value�$ m_PTPSpeedPercentValue��$ TwoWay=�$ PropertyChanged=��� $��2��$��Lin Speed (1-2000 mm/s)��$��1q�
���   �$��Stretch=�$��Stretch=�.��� $��0,5,0,5q� �� $��1* ��� $��3* �.+��� -   $ slider_LinearSpeed��$��1��$ 1Q�$
	 2000Q�$
 10Q�$��Center=� �� $
 Text�$ slider_LinearSpeedShow��$ TwoWay=��� -   $ slider_LinearSpeedShow��
������ 
���   �$ Center=�$��Center=�$��12(����� $ Value�$ m_nLinearSpeedValue��$ TwoWay=�$ PropertyChanged=��� $��3��$��Override (0-100 %)��$��1q�
���   �$��Stretch=�$��Stretch=�.��� $��0,5,0,5q� �� $��1* ��� $��3* �.+��� -   $ slider_OverridePercent��$��1��$ 10Q�$		 100Q�$
 1Q�$��Center=� �� $
 Text�$  slider_OverridePercentShow��$ TwoWay=��� -   $  slider_OverridePercentShow��
������ 
���   �$ Center=�$��Center=�$��12(����� $ Value�$ m_nOverridePercent��$ TwoWay=�$ PropertyChanged=�� $��1��  �� $��8* ��� $��2* �.+�&   System.Windows.Controls.DataGrid  -   $��dataGrid_robot_Position��$��0��   CanUserAddRows$ False��   AutoGenerateColumns$ False��   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ Param��   Width5  /System.Windows.Controls.DataGridLengthConverter$ 1*    
IsReadOnly$
 True��   CanUserResize$ False��   CanUserSort$ False��   CanUserReorder$ False��   
Foreground
 �   �1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *	m_field+$ TwoWay=�  $ Value��$ 2* $
 True��$ False��$ False��$ False��
 �   � �� *	m_value+$ TwoWay=�  $
 Unit��$ 1* $
 True��$ False��$ False��$ False��
 �   � �� *m_unit+$ TwoWay=�� $��1��  �� �� .+�� $��0�� �� �� .+��� -    ��Name$ button_GetHomePosition��$��0��$��Get Home���� -   $ button_SetHomePosition��$��1��$��Set Home��� $��1�� �� �� .+��� -   $ button_Stop_Moving��$��0��$
��STOP��$��Stretch=��� -   $ button_Home_Move��$��1��$��	Home Move��  -   $��dataGrid_robot_Input��$��2��$ False��$ False��   $ Input��$ 1* $
 True��$ False��$ False��$ False��
 �   � �� *	m_field+$ TwoWay=�  $ Value��$ 1* $
 True��$ False��$ False��$ False��
 �   � �� *	m_value+$ TwoWay=�  -   $��dataGrid_robot_Output��$��3��$ False��$ False��   $ Output��$ 1* $
 True��$ False��$ False��$ False��
 �   � �� *	m_field+$ TwoWay=�  $ Value��$ 1* $
 True��$ False��$ False��$ False��
 �   � �� *	m_value+$ TwoWay=�� $��4��$��0,5,0,0q�  �� $��0.95* ��� $��1* ��� $��1* ��� $��1.75* ��� $��1* ��� $��1* ��� $��1* ��� $��1* ��� $��1* �.+�� $��0��.+��  �� $��1* ��� $��2* �.+��� $��0��$��Jog Type��$
��Left=��� -    ��Name$ combo_JogType��$��1��$��Stretch=�� $��1��$��2,2,2,2q�.+�d� -   $ toggle_ServoOnOff��$��Servo On/Off��� $��2��.+�� .+��� $��	Move Type��$��Center=��� -   $ combo_MoveTypes��$��Stretch=�$��65,0,0,0q��� $��3��$��	Step (um)��$��15(�.���  �� $��1* ��� $��2* �.+��� -   $ slider_StepRelative��$
��Auto��$��1��$ -500000Q�$	 500000Q�$
 1Q�$��Center=� �� $
 Text�$ slider_StepRelativeShow��$ TwoWay=��� -   $ slider_StepRelativeShow��
������ 
���   �$ Center=�$��Center=�$��10(����� $ Value�$ m_nStepRelativeValue��$ TwoWay=�$ PropertyChanged=�� $��4��$��2,2,2,2q� �� �� �� .+��� -    ��Name$ label_move_Motor1��$��0��$��A1��
���   �$��Right=��� -   $ button_negative_Move1��$��-��$��1��
���   �$��15(�$��Center=��� -   $ button_positive_Move1��$��+��$��2��
���   �$��15(�$��Center=�� $��5��$��2,2,2,2q� �� �� �� .+��� -   $ label_move_Motor2��$��0��$��A2��
���   �$��Right=��� -   $ button_negative_Move2��$��-��$��1��
���   �$��15(�$��Center=��� -    $ button_positive_Move2��$��+��$��2��
���   �$��15(�$��Center=�� $��6��$��2,2,2,2q� �� �� �� .+��� -!   $ label_move_Motor3��$��0��$��A3��
���   �$��Right=��� -"   $ button_negative_Move3��$��-��$��1��
���   �$��15(�$��Center=��� -#   $ button_positive_Move3��$��+��$��2��
���   �$��15(�$��Center=�� $��7��$��2,2,2,2q� �� �� �� .+��� -$   $ label_move_Motor4��$��0��$��A4��
���   �$��Right=��� -%   $ button_negative_Move4��$��-��$��1��
���   �$��15(�$��Center=��� -&   $ button_positive_Move4��$��+��$��2��
���   �$��15(�$��Center=�� $��8�� �� �� �� .+��� -'   $��0��$��	Add Point��$��0,0,2,0q�$��10(��� -(   $ button_setTo_ChoosenPos��$��1��$��	Set Point���� -)   $��2��$��Delete��$��2,0,0,0q�$��10(�� $��2�� �� $��1* ��� $��8* ��� $
��1.5* �.+�� $��0��  �� �� �� �� �� �� .+��� -*   $# button_CameraRobotCalibration��$�� Do Calibration��$��1��$��28��$��Bottom=��� -+   $ button_Next_Calibration��$
��Next��$��2��  -,   $"��dataGrid_all_robot_Positions��$��1��$   HorizontalScrollBarVisibility$
 Auto=�"   VerticalScrollBarVisibility$
 Auto=�$ False��$ False��$
��Left=�   $ PS No.��$
 auto $
 True��$ False��$ False��$ False��
 �   � �� *m_PointIndex+$ TwoWay=�  $ Comment��$
 auto $
 True��$ False��$ False��$ False��
 �   � �� *m_PointComment+$ TwoWay=�  $ X (mm)��$
 auto $ False��$ False��$ False��$ False��
 �   � �� *m_X+$ TwoWay=�  $ Y (mm)��$
 auto $ False��$ False��$ False��$ False��
 �   � �� *m_Y+$ TwoWay=�  $ Z (mm)��$
 auto $ False��$ False��$ False��$ False��
 �   � �� *m_Z+$ TwoWay=�  $ Rz (De)��$
 auto $ False��$ False��$ False��$ False��
 �   � �� *m_Rz+$ TwoWay=�  $ M1 (deg)��$
 auto $ False��$ False��$ False��$ False��
 �   � �� *
m_Joint1+$ TwoWay=�  $ M2 (deg)��$
 auto $ False��$ False��$ False��$ False��
 �   � �� *
m_Joint2+$ TwoWay=�  $ M3 (deg)��$
 auto $ False��$ False��$ False��$ False��
 �   � �� *
m_Joint3+$ TwoWay=�  $ M4 (deg)��$
 auto $ False��$ False��$ False��$ False��
 �   � �� *
m_Joint4+$ TwoWay=�  $
 Base��$
 auto $ False��$ False��$ False��$ False��
 �   � �� *m_Base+$ TwoWay=�  $
 Tool��$
 auto $ False��$ False��$ False��$ False��
 �   � �� *m_Tool+$ TwoWay=�� $��2��.+��� --   $ button_Save_Sequence��$��Center=�$
��Save��$��50��