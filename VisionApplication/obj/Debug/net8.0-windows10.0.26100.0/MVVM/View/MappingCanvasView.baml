   M S B A M L   `   `   `  ����L  HVisionApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:VisionApplication.MVVM.ViewVisionApplication.MVVM.View  5      S.clr-namespace:VisionApplication.MVVM.ViewModel VisionApplication.MVVM.ViewModel  5	      3    -VisionApplication.MVVM.View.MappingCanvasView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;local)clr-namespace:VisionApplication.MVVM.View     5      0cmd http://www.galasoft.ch/mvvmlight     5      D	viewmodel.clr-namespace:VisionApplication.MVVM.ViewModel     5	      Hi:http://schemas.microsoft.com/expression/2010/interactivity     5
      -   5        .��5      � -   5          �Name$  Grid_CanVas_Mapping��5       � RowDefinitions 5   
   �� 5      �� 5      $��30 �6   .+�5   
   �� $
��Auto=�6   $
��Auto=�6<   .��5       �� -   5         ��Name$ canvas_Mapping��5       $
��Left=�6�   ��6-   �� *MappingCanvasHeight+ �� Mode$ TwoWay=���6a   �� *MappingCanvasWidth+.+�5"      �� ��6   �� *mappingRectangles+$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=���5#      �� 5$      .R�5%      �� 5&      ��5)      �� 5*      .R�5+      5   /VisionApplication.MVVM.View.MapingRectangleView  5,      ��5/      �� 50      .�51      ��  �� Property �@��Left �� Value 6<   �� *	imageLeft+�� 52       �@��Top 6;   �� *
imageTop+53      56      58      59      � 5;   
   $��1��6    � ColumnDefinitions 5<      �� 5=      �� 5>      .+�5@      �� $��0��6    �� Orientation$ 
Horizontal=�6)   $��Center=�6B   
�������.+�5A      �� -   5        	 ��Name$	 btn_Previous_Page��5A      $��-��6\   $��30��6h   $��18(�6s   
������ ��64   �� *btnPageClickCommand+6�   �� 5B      -   5        
 ��Name$
 text_Current_Page��5B      $��20(�6P   $��Center=�5C   4   $��Center=�6Q   $��50��6l   
�������
���������5B   3   �� *
txtPage1+5C   �   �� 5D      -   5        $	 btn_Next_Page��5D      $��+��6W   $��30��6c   $��18(�6n   
������ ��60   �� *btnNextPageCommand+6�   5E      �� 5G      $��1��6   $ 
Horizontal=�6)   $��Center=�6B   
�������.+�5H      �� -   5        $	 btn_Previous_Page2��5H      $��-��6^   $��30��6j   $��18(�6u   
������ ��65   �� *btnPage2ClickCommand+6�   �� 5I      -   5        $
 text_Current_Page2��5I      $��20(�6Q   $��Center=�5J   4   $��Center=�6Q   $��50��6l   
�������
���������5I   4   �� *
txtPage2+5J   �   �� 5K      -	   5        $	 btn_Next_Page2��5K      $��+��6Z   $��30��6f   $��18(�6q   
������ ��61   �� *btnPage2NextCommand+6�   5L      5M      5O      5P      