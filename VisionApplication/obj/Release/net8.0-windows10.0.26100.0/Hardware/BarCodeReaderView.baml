   M S B A M L   `   `   `  ����   VisionApplication2    ,VisionApplication.Hardware.BarCodeReaderView   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     .��� 
�������  � RowDefinitions  �� $��* ��� $��* ��� $��* ��� $��* �.+��� $��0��$��Barcode Reader Setting��
�������
���   �$��1q�� $��1�� � ColumnDefinitions �� $��1* ��� $��2* ��� $��1* �.+��� $��0��$��	Reader IP��
���   �$��1q��� -    ��Name$ label_ReaderIP_Address��$��1��
��������� -    ��Name$
 Save��$��2��$
��Save��� $��2�� �� $��1* ��� $��2* ��� $��1* �.+��� $��Command��
���   �$��1q��� -    ��Name$ combo_BarcodeBrank��$��1�� �� SelectedIndex �� *	brankID+�� -   $ button_Trigger��$��2��$��Trigger��� $��3��.+��  �� $��1* ��� $��2* ��� $��1* �.+��� $��0��$��Data Received��
���   �$��1q��� -   $ label_DataReceived��$��1��
��������� -   $ button_Clear��$��2��$��Clear��