   M S B A M L   `   `   `  ����L  HVisionApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullS.clr-namespace:VisionApplication.MVVM.ViewModel VisionApplication.MVVM.ViewModel  5      S.clr-namespace:VisionApplication.MVVM.Behaviors VisionApplication.MVVM.Behaviors  5      5    /VisionApplication.MVVM.View.LotBarcodeDataTable   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      D	viewmodel.clr-namespace:VisionApplication.MVVM.ViewModel     5      _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors  5      =behavior.clr-namespace:VisionApplication.MVVM.Behaviors  5      $��Center=�5
      $��Center=�5      .��5      � $��10q�6     � ColumnDefinitions  5   
   �� 5      $��* �6   �� 5      $��2* �6   .+�5   
   �� $��0��6   
�������$��0,0,3,0q�65   
���   �._�5      �� $��	0,100,0,0q�6   .+�5      (  "System.Windows.Controls.DatePicker  -   5        $��DatePicker_Date��5   0   $��0,50,0,0q�6      SelectedDate 6G   �� *selectedDate+*  $Microsoft.Xaml.Behaviors.Interaction   	Behaviors 5      =   7VisionApplication.MVVM.Behaviors.EventToCommandBehavior  5         	EventName$ SelectedDateChanged��6:      Command 5   )   �� *DateSelectionChangedCmd+5      � 5      $��0,10,0,0q�6     5       �� 5!      $��45 �6+   �� 5"      .+�5$      �� $��0��6   $��Lot ID:��6,   
�������
�����p�6j   �� 5%      -   5         ��Name$ ccb_LotSelected_ComboBox��5%   /   $��1��6    �� SelectedValue 6Q   �� *lotSelected+��5&      �� *m_ListStrLotFullPath+ 5'        5(      $ SelectionChanged��6>    5)   )   �� *SelectionChangedCmd+5+      5,      �� 5-      -   5         ��Name$ btn_Load_Lot��5-   T   $��70��6   $��	0,10,10,0q�6%   $��Right=�68   $��Load Lot��6j   ��6}   �� *btn_Load_Lot_ClickCmd+6�   �� 5.      -   5        $ btn_Send_To_Server��5.   S   $��70��6   $��	0,10,10,0q�6$   $��Right=�67   $��Send Lot��6o   ${�False��6�   $y�Hidden=�6�   ��6�   �� *btn_Send_To_Server_ClickCmd+6�   50      52      �� 54   
   $��1��6    �� CornerRadius$ 0��6"   $��0,0,0,1q�63   ���	#FF86D2F9���	#FF242526._�55      �� .+�5=      �� 	 �� Orientation$	 
Horizontal=�6   $��0,0,0,0q�67   .+�5>      �� $	��NO.��6   $��17(�5?      $��50��5@      
�������$��Center=�5B      $��Bottom=�5C      $
��Bold"�5D      $��1q�6/   
�������5E      �� 5F      $��	Date Scan��6   $��Center=�5G      $��17(�5H      $	��200��5I      
�������$
��Bold"�5K      $��1q�6/   
�������5L      �� 5N      $��
Barcode ID��6   $��Center=�5O      $��17(�5P      $	��150��5Q      
�������$
��Bold"�5S      $��1q�6/   
�������5T      �� 5V      $��Result��6   $��Center=�5W      $��17(�5X      $��80��5Y      
�������$
��Bold"�5[      $��1q�6/   
�������5\      5^      5_      5`      �� 5a   
   -   5        
 ��Name$'
 !scv_LotBarcodeDataTableScrollView��5a   4   $	��550��6   $��1��6$   $
��Auto=�6_   $
��Auto=�6�   $��0,35,0,0q�6�   .��5c      �� -   5         ��Name$ lvLotBarCodeData��5c      $��15(�61   $
��Auto��6?   $	��550��6M   
�����w���5e   �   �� *m_ListLotBarcodeDataTable+��5g      �� 5h      .R�5i      �� $	 
Horizontal=�6%   .+�5j      �� 
�������$��50��5k   D    �� TextAlignment$ Center=�5l   D   $��15(�5m   D   ��5j   (   �� *str_NO+5n      �� 5o      
�������$	��200��5p   D   $ Center=�5q   E   $��15(�5r   D   ��5o   (   �� *str_DateScan+5s      �� 5u      
�������$	��150��5v   %   $ Center=�5w   (   $��15(�5x   %   ��5u   (   �� *str_BarcodeID+5y      �� 5{      
�������$��50��5|   !   $ Center=�5}   (   $��15(�5~   !   ��5{   (   �� *
str_Result+5      5�      5�      5�      5�      5�      5�      