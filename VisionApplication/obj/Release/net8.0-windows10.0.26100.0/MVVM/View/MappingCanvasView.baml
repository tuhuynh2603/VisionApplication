   M S B A M L   `   `   `  ���� L  HVisionApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:VisionApplication.MVVM.ViewVisionApplication.MVVM.View  S.clr-namespace:VisionApplication.MVVM.ViewModel VisionApplication.MVVM.ViewModel  3    -VisionApplication.MVVM.View.MappingCanvasView   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     ;local)clr-namespace:VisionApplication.MVVM.View     0cmd http://www.galasoft.ch/mvvmlight     D	viewmodel.clr-namespace:VisionApplication.MVVM.ViewModel     Hi:http://schemas.microsoft.com/expression/2010/interactivity     -   .��� -     �Name$  Grid_CanVas_Mapping�� � RowDefinitions �� �� $��30 �.+��� $
��Auto=�$
��Auto=�.���� -    ��Name$ canvas_Mapping��$
��Left=����� *MappingCanvasHeight+ �� Mode$ TwoWay=����� *MappingCanvasWidth+.+��� ���� *mappingRectangles+$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=����� .R��� ���� .R�5   /VisionApplication.MVVM.View.MapingRectangleView  ���� .���  �� Property �@��Left �� Value �� *	imageLeft+��  �@��Top �� *
imageTop+� $��1�� � ColumnDefinitions �� �� .+��� $��0�� �� Orientation$ 
Horizontal=�$��Center=�
�������.+��� -   	 ��Name$	 btn_Previous_Page��$��-��$��30��$��18(�
������ ���� *btnPageClickCommand+�� -   
 ��Name$
 text_Current_Page��$��20(�$��Center=�$��Center=�$��50��
�������
����������� *
txtPage1+�� -   $	 btn_Next_Page��$��+��$��30��$��18(�
������ ���� *btnNextPageCommand+�� $��1��$ 
Horizontal=�$��Center=�
�������.+��� -   $	 btn_Previous_Page2��$��-��$��30��$��18(�
������ ���� *btnPage2ClickCommand+�� -   $
 text_Current_Page2��$��20(�$��Center=�$��Center=�$��50��
�������
����������� *
txtPage2+�� -	   $	 btn_Next_Page2��$��+��$��30��$��18(�
������ ���� *btnPage2NextCommand+