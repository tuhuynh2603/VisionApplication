   M S B A M L   `   `   `  ���� L  HVisionApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:VisionApplication.MVVM.ViewVisionApplication.MVVM.View  I)clr-namespace:VisionApplication.MVVM.ViewVisionApplication.MVVM.View  S.clr-namespace:VisionApplication.MVVM.ViewModel VisionApplication.MVVM.ViewModel  ?$clr-namespace:VisionApplication.MVVMVisionApplication.MVVM  G(clr-namespace:VisionApplication.HardwareVisionApplication.Hardware  [2clr-namespace:VisionApplication.MVVM.View.Template$VisionApplication.MVVM.View.Template  S.clr-namespace:VisionApplication.MVVM.Behaviors VisionApplication.MVVM.Behaviors  ,    &VisionApplication.MVVM.View.MainWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     [ WXceed.Wpf.AvalonDock, Version=4.6.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4g cXceed.Wpf.AvalonDock.Themes.Aero, Version=4.6.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4h	 dXceed.Wpf.AvalonDock.Themes.Metro, Version=4.6.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4i
 eXceed.Wpf.AvalonDock.Themes.VS2010, Version=4.6.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4C
avalonDock,http://schemas.xceed.com/wpf/xaml/avalondock   	 
 Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006   	 
 _ [MaterialDesignThemes.Wpf, Version=5.1.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5local)clr-namespace:VisionApplication.MVVM.View  _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors  4view)clr-namespace:VisionApplication.MVVM.View  >	viewmodel.clr-namespace:VisionApplication.MVVM.ViewModel  3template$clr-namespace:VisionApplication.MVVM  7hardware(clr-namespace:VisionApplication.Hardware  Gribbontabitems2clr-namespace:VisionApplication.MVVM.View.Template  >	behaviors.clr-namespace:VisionApplication.MVVM.Behaviors  -   $	��900��$
��1600��  � WindowStyle$
  None=� J� 	AllowDrop$
 True�� � Title$ 
MainWindow��$��10q� � WindowState �� *windowState+ J� InputBindings �� -   KeyBinding_b��
 �� Key$ b�� �� Command �� *btn_Binarize_ClickCmd+�� -   KeyBinding_p��$ p�� �� *btn_Pixel_Ruler_ClickCmd+�� -   KeyBinding_X_Shift��$ X�� �� 	Modifiers$ Shiftf� �� *btn_Clear_CommCmd+�� -   KeyBinding_O��$ O�� �� *btn_ShowOverlay_ClickCmd+ � DataContext 3   -VisionApplication.MVVM.ViewModel.MainWindowVM  .��� ���	#FF322F2E$��0,0,0,0q�.+�� 	 � RowDefinitions	 �� $��30 ��� $	��140 ��� $��1* �.+��� 
�����F�$��1q����� 
 �� EndPoint$
 0.1,1&� � MappingMode$ RelativeToBoundingBox=� �� 
StartPoint$ 0.5,1&�.���  � Color$ Black�� � Offset$ 0.1139Q�� $ 	#FF352F2E��$ 0.8939Q�._�� $��0�� � 	Resources  �� DataType1   +VisionApplication.MVVM.ViewModel.TitleBarVM*   $VisionApplication.MVVM.View.TitleBar��%"   (��       *, +)�� "  .R�   � ColumnDefinitions �� $
��Auto ��� .+���  �� Orientation$ 
Horizontal=�$��0��.+��� $ ��..\..\Resources\ICONHD.png��$��30��$��30���� $"��Tap and Reel Packing Machine��$��15(�$��Microsoft New Tai Lue)�
��������� $��1��$ 
Horizontal=�$��Right=�.+��� $��Right=�$��Center=�$ 
Horizontal=�$��30��.+��� $ 
Horizontal=�$��Center=�.+��� $��Lot ID: ��$��0 0 0 0q�$
��Auto�����#00BFFF$��20(�$
��Bold"�$��Center=��� -    ��Name$ 
text_LotID��$
��Auto��$	��150��
�������
���OO/�$��20(�$
��Bold"�$��Center=�����  �� Path$ m_strCurrentLotID� �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�*  $Microsoft.Xaml.Behaviors.Interaction   Triggers +  %Microsoft.Xaml.Behaviors.EventTrigger     	EventName$ MouseLeftButtonDown��*  $Microsoft.Xaml.Behaviors.TriggerBase   Actions. 2  ,Microsoft.Xaml.Behaviors.InvokeCommandAction     Command �� * text_LotID_MouseLeftButtonDown+�� $��Recipe: ��$��18,0,0,0q�$
��Auto�����#00BFFF$��20(�$
��Bold"�$��Center=�7  1System.Windows.Controls.Ribbon.RibbonToggleButton  -     Name$ btn_LoadRecipe��   ToggleButtonStyle1#���   $
��None��$��80��$��Microsoft New Tai Lue)�$��Center=�$��20(�$
��auto��
�������  BgButton   MouseOverBackground# [    CheckedBackground# [ ���� *btn_LoadRecipe_ClickCmd+�� $��Access Level:�����#00BFFF$��Center=�$��Center=�$��18(�$��18 0 0 0q�$��Microsoft New Tai Lue)��� -   $ 
acessLevel��$
��None��$��50��$��Microsoft New Tai Lue)�$��Center=�$��Center=� �� TextAlignment$ Center=�$��20(�
���OO/�$
��auto��
��������� $��User:�����#00BFFF$��Center=�$��20(�$��18 0 0 0q�$��Microsoft New Tai Lue)�  -	   $ btnLogIn��#���   $
��None��$��80��$��Microsoft New Tai Lue)�$��Center=�$��20(�$
��auto��
�������# [ # [    $ 	Unchecked��.    �� *btn_LogIn_UncheckedCmd+  $ Checked��.    �� *btn_LogIn_CheckedCmd+�� $��1������ *mTitleBarVM+� -
    �Name$ controllTabPanel��$��1��$	��Top=� �� $��1* ��� $��1* �.+��� $��0��$ 
Horizontal=�.+��� -     ��Name$  tab_controls�����	#FF322F2E���� $
 0.5,0.03&�$ RelativeToBoundingBox=�$ 0.5,0&�.��� $ 	#FF342F2E��$ 0.997Q�� $ Transparent��$ 0.919Q�.@��� -   $��tab_Hardware_view��$��HARDWARE��.���� $ 
Horizontal=�$
{�True��.+�0	  *System.Windows.Controls.Ribbon.RibbonGroup	  -   $��hardware_group������ ! �� 
TargetType"! 	 .��� " �� Property" � ��# �� Value$# 12(�$ �� Triggers$ �� % �� Binding% �� $ IsUseSmallIcon�$ PropertyChanged=�$ TwoWay=�& �� Value& True.a��� " � ��$# 11(����� -   $��groupHardware��$�� ��$��Microsoft New Tai Lue)����	#FFD6F9FF$��13(�$
��Bold"�' J� Effect' -
  'System.Windows.Media.Effects.BlurEffect
  ( 
  Radius$	( 0.1Q�.@�1  +System.Windows.Controls.Ribbon.RibbonButton  $��1��$��0��$��50��� $��	0,0,0,-30q�	 �� $��5* ��� $��3* �.+�� $��0�� �� �� .+�  -   $��btn_stream_camera��$��0��)   Label$) Stream Camera��*   LargeImageSource$'* !..\..\Resources\camera-custom.png��
�������  CheckedButtonStyle# [   MouseOverButtonStyle# [ $
��auto��   $ 	Unchecked��.    �� * btn_stream_camera_UnCheckedCmd+  $ Checked��.    �� *btn_stream_camera_CheckedCmd+  -   $��btn_camera_setting��$��1��$) Camera Setting��$,* &..\..\Resources\camera-iris-custom.png��
�������# [ # [ $
��auto��   $ 	Unchecked��.    �� *!btn_Camera_Setting_UncheckedCmd+  $ Checked��.    �� *btn_Camera_Setting_CheckedCmd+�� -   + ��Name$+ label_Camera_Status��$��15(�$��1��$	��Top=�$��Center=����� *labelCameraStatus+���� *labelCameraStatusBackground+� $��	0,0,0,-30q�	 �� $��5* ��� $��3* �.+�  -   $ btn_Robot_Controller��$��0��$) ROBOT Controller��$1* +..\..\Resources/robot-industrial-custom.png��
�������# [ # [ $
��Auto��   $ 	Unchecked��.    �� *#!btn_Robot_Controller_UncheckedCmd+  $ Checked��.    �� *!btn_Robot_Controller_CheckedCmd+�� -   $+ label_Robot_Status��$��0,0,0,0q�$��15(�$��1��$	��Top=����� *labelRobotStatus+���� , �� Color, �� *color_RobotStatus+� $��	0,0,0,-30q�	 �� $��5* ��� $��3* �.+�  -   $ btn_BarCodeReader_Setting��$��0��$) Barcode Reader Setting��$,* &..\..\Resources/qrcode-scan-custom.png��
�������$
{�true��- d� 	IsChecked$- FalseY�# [ # [ $
��Auto��   $ 	Unchecked��.    �� *(&btn_BarCodeReader_Setting_UncheckedCmd+  $ Checked��.    �� *&$btn_BarCodeReader_Setting_CheckedCmd+�� -   $+ label_Barcode_Status��$��0,0,0,0q�$��15(�$��1��$��Disconnected��$	��Top=�� $��	0,0,0,-30q�	 �� $��5* ��� $��3* �.+�  -   $ btn_PLCCOMM_Setting��$��0��$) PLC COMM Setting��$!* ..\..\Resources/network.png��
�������$
{�true��$- FalseY�# [ # [ $
��Auto��   $ 	Unchecked��.    �� *" btn_PLCCOMM_Setting_UncheckedCmd+  $ Checked��.    �� * btn_PLCCOMM_Setting_CheckedCmd+�� -   $+ label_PLCCOMM_Status��$��15(�$��1��$��Disconnected��$	��Top=�$��Center=�� $��	0,0,0,-30q�	 �� $��5* ��� $��3* �.+�  -   $ btn_Serial_COMM��$��0��$) Serial COMM��$%* ..\..\Resources\serial-comm.png��
�������$
{�true��$- FalseY�# [ # [ $
��Auto��   $ 	Unchecked��.    �� *btn_Serial_COMM_UncheckedCmd+  $ Checked��.    �� *btn_Serial_COMM_CheckedCmd+�� -   $+ label_Serial_Comm��$��15(�$��1��$��Disconnected��$	��Top=�$��Center=��� -   $��tab_vision_view��$��
INSPECTION��.��8   2VisionApplication.MVVM.View.Template.InspectionTab   �� *inspectionTabVM+�� -   $��tabTool_View��$��	TOOL VIEW��.���� $ 
Horizontal=�$
{�True��.+�	  -   $��btn_Binarize_Group������ $
��View��$��Microsoft New Tai Lue)����	#FFD6F9FF$��13(�$
��Bold"�' 
  $	( 0.1Q�.@�  $��1��$��0��$��50��  -   $��btn_Binarize��$) Binarize��$ * ..\..\Resources\binary.png��
�������# [ # [ $��68������ *btn_Binarize_ClickCmd+- �� *isbtn_BinarizeChecked+   $ 
MouseLeave��.    �� *Binarize_MouseLeaveCmd+  $ 
MouseEnter��.    �� *Binarize_MouseEnterCmd+  -   $��btn_ShowOverlay��$) Show Overlay��$!* ..\..\Resources\overlay.png��
�������# [ # [ $��68������ *btn_ShowOverlay_ClickCmd+  -   $��btn_Clear_Comm��.   Label$. Clear Output Log��/   LargeImageSource$#/ ..\..\Resources\clear-log.png��
�������0   MouseOverBackground#0 [ $��68������ *btn_Clear_CommCmd+  -    $��btn_Pixel_Ruler��$- FalseY�$) Pixel Ruler��$%* ..\..\Resources\pixel-ruler.png��
�������$��13(�# [ # [ $��68������ *btn_Pixel_Ruler_ClickCmd+  -!   $��btn_Lot_DataExcel��$) Lot Overview��$** $..\..\Resources\book-open-custom.png��# [ 
�������# [ $
��auto��   $ 	Unchecked��.    �� * btn_Lot_DataExcel_UncheckedCmd+  $ Checked��.    �� *btn_Lot_DataExcel_CheckedCmd+  -"   $��btn_mapping_parameters��$) Mapping Setting��$#* ..\..\Resources\blue-chip.png��# [ 
�������# [ $
��auto��   $ 	Unchecked��.    �� *%#btn_mapping_parameters_UncheckedCmd+  $ Checked��.    �� *#!btn_mapping_parameters_CheckedCmd+� $��1��.+��� $
��Auto��$ 
Horizontal=�$��Right=�.+��� 
�����F�$��1��  -#   $ btn_run_sequence��  runSequenceImage#��[   ToggleButtonStyle2#���  # [ # [    $ 	Unchecked��.    �� *btn_run_sequence_UncheckedCmd+  $ Checked��.    �� *btn_run_sequence_CheckedCmd+  -$   $ btn_Debug_sequence��  debugSequenceImage#��[ #���  # [ # [ $- FalseY�   $ 	Unchecked��.    �� *!btn_Debug_sequence_UncheckedCmd+  $ Checked��.    �� *btn_Debug_sequence_CheckedCmd+  -%   $ btn_Imidiate_Stop��  ImidiateStopImage#��[ #���  # [ # [ - �� *Isbtn_Imidiate_StopChecked+���� *btn_Imidiate_Stop_ClickCmd+  -&   $ btn_Emergency_Stop��  EmergencyStopImage#��[ #���  # [ # [ - �� *Isbtn_Emergency_StopChecked+���� *btn_Emergency_Stop_ClickCmd+  -'   $ btn_Reset_Machine�� 	 
ResetImage#��[	 #���  # [ # [ ���� *btn_Reset_Machine_ClickCmd+� -(   $ Layout1��$��2��$��Stretch=�$��Stretch=�
���   �	 �� $��2* �.+�� .+�)  #Xceed.Wpf.AvalonDock.DockingManager  -)   1  Name$1 dockManager��2   AllowMixedOrientation$
2 True��3   GridSplitterWidth$3 10Q�4   GridSplitterHeight$4 10Q�5   Layout5 �� *layoutVision+$ PropertyChanged=�$ TwoWay=�6   Theme6 - 
 'Xceed.Wpf.AvalonDock.Themes.VS2010Theme  7   DocumentHeaderTemplate7 �� .R��� $ 
Horizontal=�.+��� $��CharacterEllipsis=����� *Title+�� $��0 0 0 0q�$��10(�$
��Bold"����� *	ContentId+�� $��50,200,50,50q� :   4VisionApplication.MVVM.ViewModel.WarningMessageBoxVM3   -VisionApplication.MVVM.View.WarningMessageBox8   2VisionApplication.MVVM.ViewModel.VisionParameterVM3   -VisionApplication.MVVM.View.VisionParameterUC7   1VisionApplication.MVVM.ViewModel.TeachParameterVM3   -VisionApplication.MVVM.View.TeachParametersUC2   ,VisionApplication.MVVM.ViewModel.StepDebugVM/   )VisionApplication.MVVM.View.StepDebugView<   6VisionApplication.MVVM.ViewModel.SerialCommunicationVM9   3VisionApplication.MVVM.View.SerialCommunicationView3   -VisionApplication.MVVM.ViewModel.PixelRulerVM,   &VisionApplication.MVVM.View.PixelRuler0   *VisionApplication.MVVM.ViewModel.PLCCOMMVM(   "VisionApplication.Hardware.PLCCOMM9   3VisionApplication.MVVM.ViewModel.HIKControlCameraVM5   /VisionApplication.Hardware.HIKControlCameraView8   2VisionApplication.MVVM.ViewModel.MappingSetingUCVM1    +VisionApplication.MVVM.View.MappingSetingUC2!   ,VisionApplication.MVVM.ViewModel.LoginUserVM+"   %VisionApplication.MVVM.View.LoginUser6#   0VisionApplication.MVVM.ViewModel.BarcodeReaderVM2$   ,VisionApplication.Hardware.BarCodeReaderView<%   6VisionApplication.MVVM.ViewModel.LotBarcodeDatatableVM5&   /VisionApplication.MVVM.View.LotBarcodeDataTable5'   /VisionApplication.MVVM.ViewModel.RecipeManageVM2(   ,VisionApplication.MVVM.View.RecipeManageView��%�  (��       *, +)(��      *, +)(�� $     *, +)(�� 6     *, +)(�� H     *, +)(�� Z     *, +)(�� l     *, +)(�� ~     *, +)(�� �     *, +)(�� �     *,! +)(�� �     *,# +)(�� �     *,% +)(�� �     *,' +)�� "  .R�  �� "  .R�  �� "  .R�  �� "  .R�  �� "  .R�  �� "  .R�  �� "  .R�  �� "  .R�  �� "  .R�   �� " ! .R�"  �� " # .R�$  �� " % .R�&  �� " ' .R�(  .+�0)   *VisionApplication.MVVM.DragDropUserControl)   �� *mSerialCommunicationVM+)   �� *mPixelRulerVM+)   �� *
mPLCCOMMVM+)   �� *mHiwinRobotVM+)   �� *mHIKControlCameraVM+)   �� *mMappingSettingUCVM+)   �� *mRecipeManageVM+)   �� *mLoginUserVM+)   �� *mBarCodeReaderVM+)   �� *mLotBarcodeDataTableVM+)   �� *mWarningMessageBoxVM+)   �� *mVisionParameterVM+)   �� *mTeachParameterVM+)   �� *mStepDebugVM+