   M S B A M L   `   `   `  ���� L  HVisionApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:VisionApplication.HardwareVisionApplication.Hardware  [ WSystem.Windows.Forms, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089Y@clr-namespace:System.Windows.Forms;assembly=System.Windows.FormsSystem.Windows.Forms ^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35tOclr-namespace:System.Windows.Forms.Integration;assembly=WindowsFormsIntegration System.Windows.Forms.Integration 5    /VisionApplication.Hardware.HIKControlCameraView   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     :local(clr-namespace:VisionApplication.Hardware     SwfCtrl@clr-namespace:System.Windows.Forms;assembly=System.Windows.Forms     bwfHostOclr-namespace:System.Windows.Forms.Integration;assembly=WindowsFormsIntegration     -   $��cameraControl��.��� -     �Name$  
cameraGrid��
������� � RowDefinitions �� $��25 ��� $��25 ��� $��1 ��� $��5* �.+��� $��0��
�������.��Camera Setting� $��1�� � ColumnDefinitions �� $��5* ��� $��2* �.+��� -   $��cbDeviceList��$��0���� -   $��bnEnum��$��1��.��Find device�� $��3��.+�� $��25�� �� $��1* ��� $��1* �.+��� -   $��bnOpen��$��0��$��Center=�$��25��.��Connect Camera�� -   $��bnClose��${�False��$��1��$��Center=�$��25��.��Disconnect Camera�  �� $��1* ��� $��1* ��� $��1* ��� $��1* �.+��� -   $��label1��$��0��$��75��$
��Left=�.��
Exposure�� -   $��
tbExposure��${�False��$��0��$��75,0,0,0q�
���   �$��1q��� -	   $��label2��$��1��$��75��$
��Left=�.��Gain�� -
   $��tbGain��${�False��$��1��$��75,0,0,0q�
���   �$��1q��� -   $��label3��$��2��$��75��$
��Left=�.��
Frame Rate�� -   $��tbFrameRate��${�False��$��2��$��75,0,0,0q�
���   �$��1q��� -   $��
bnGetParam��$��3��$	��100��$��40,2,0,0q�$
��Left=�${�False��.��Get Parameters�� -   $��
bnSetParam��$��3��$	��100��$��	142,2,0,0q�$
��Left=�${�False��.��Set Parameters