   M S B A M L   `   `   `  ���� L  HVisionApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullS.clr-namespace:VisionApplication.MVVM.ViewModel VisionApplication.MVVM.ViewModel  S.clr-namespace:VisionApplication.MVVM.Behaviors VisionApplication.MVVM.Behaviors  5    /VisionApplication.MVVM.View.LotBarcodeDataTable   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     D	viewmodel.clr-namespace:VisionApplication.MVVM.ViewModel     _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors  =behavior.clr-namespace:VisionApplication.MVVM.Behaviors  $��Center=�$��Center=�.��� $��10q�  � ColumnDefinitions  �� $��* ��� $��2* �.+��� $��0��
�������$��0,0,3,0q�
���   �._��� $��	0,100,0,0q�.+�(  "System.Windows.Controls.DatePicker  -   $��DatePicker_Date��$��0,50,0,0q�   SelectedDate �� *selectedDate+*  $Microsoft.Xaml.Behaviors.Interaction   	Behaviors =   7VisionApplication.MVVM.Behaviors.EventToCommandBehavior     	EventName$ SelectedDateChanged��   Command �� *DateSelectionChangedCmd+� $��0,10,0,0q�  �� $��45 ��� .+��� $��0��$��Lot ID:��
�������
�����p��� -    ��Name$ ccb_LotSelected_ComboBox��$��1�� �� SelectedValue �� *lotSelected+���� *m_ListStrLotFullPath+   $ SelectionChanged�� �� *SelectionChangedCmd+�� -    ��Name$ btn_Load_Lot��$��70��$��	0,10,10,0q�$��Right=�$��Load Lot������ *btn_Load_Lot_ClickCmd+�� -   $ btn_Send_To_Server��$��70��$��	0,10,10,0q�$��Right=�$��Send Lot��${�False��$y�Hidden=����� *btn_Send_To_Server_ClickCmd+�� $��1�� �� CornerRadius$ 0��$��0,0,0,1q����	#FF86D2F9���	#FF242526._��� .+��� 	 �� Orientation$	 
Horizontal=�$��0,0,0,0q�.+��� $	��NO.��$��17(�$��50��
�������$��Center=�$��Bottom=�$
��Bold"�$��1q�
��������� $��	Date Scan��$��Center=�$��17(�$	��200��
�������$
��Bold"�$��1q�
��������� $��
Barcode ID��$��Center=�$��17(�$	��150��
�������$
��Bold"�$��1q�
��������� $��Result��$��Center=�$��17(�$��80��
�������$
��Bold"�$��1q�
��������� -   
 ��Name$'
 !scv_LotBarcodeDataTableScrollView��$	��550��$��1��$
��Auto=�$
��Auto=�$��0,35,0,0q�.���� -    ��Name$ lvLotBarCodeData��$��15(�$
��Auto��$	��550��
�����w����� *m_ListLotBarcodeDataTable+���� .R��� $	 
Horizontal=�.+��� 
�������$��50�� �� TextAlignment$ Center=�$��15(����� *str_NO+�� 
�������$	��200��$ Center=�$��15(����� *str_DateScan+�� 
�������$	��150��$ Center=�$��15(����� *str_BarcodeID+�� 
�������$��50��$ Center=�$��15(����� *
str_Result+