   M S B A M L   `   `   `  ����  VisionApplication2    ,VisionApplication.Hardware.BarCodeReaderView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .��5      � 
�������  � RowDefinitions  5   
   �� 5	      $��* �6   6)   �� 5
      $��* �6   6)   �� 5      $��* �6   6)   �� 5      $��* �6   6)   .+�5   
   �� $��0��6   $��Barcode Reader Setting��6   
�������
���   �$��1q�6h   6~   � 5   
   $��1��6    � ColumnDefinitions 5      �� 5      $��1* �6#   60   �� 5      $��2* �6#   60   �� 5      $��1* �6#   60   .+�5      �� $��0��6   $��	Reader IP��6$   
���   �$��1q�6L   6b   �� 5      -   5         ��Name$ label_ReaderIP_Address��5   $   $��1��6   
�������6Y   �� 5      -   5         ��Name$
 Save��5   %   $��2��6   $
��Save��63   6W   5      � 5   
   $��2��6    5      �� 5      $��1* �6#   60   �� 5      $��2* �6#   60   �� 5      $��1* �6#   60   .+�5"      �� $��Command��6   
���   �$��1q�6:   6P   �� 5#      -   5         ��Name$ combo_BarcodeBrank��5#      $��1��63    �� SelectedIndex 6z   �� *	brankID+6�   �� 5$      -   5        $ button_Trigger��5$      $��2��6.   $��Trigger��6>   6o   5'      � 5)   
   $��3��6   .+�5*      �  5+      �� 5,      $��1* �6'   64   �� 5-      $��2* �6'   64   �� 5.      $��1* �6'   64   .+�51      �� $��0��6   $��Data Received��6(   
���   �$��1q�6T   6j   �� 52      -   5        $ label_DataReceived��52   )   $��1��6   
�������6Z   �� 53      -   5        $ button_Clear��53   *   $��2��6   $��Clear��6@   6m   55      5:      5<      5>      