   M S B A M L   `   `   `  ����L  HVisionApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:VisionApplication.MVVM.ViewVisionApplication.MVVM.View  5      S.clr-namespace:VisionApplication.MVVM.ViewModel VisionApplication.MVVM.ViewModel  5      S.clr-namespace:VisionApplication.MVVM.Behaviors VisionApplication.MVVM.Behaviors  5	      /    )VisionApplication.MVVM.View.OutputLogView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;local)clr-namespace:VisionApplication.MVVM.View     5      D	viewmodel.clr-namespace:VisionApplication.MVVM.ViewModel     5      _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors  5      =behavior.clr-namespace:VisionApplication.MVVM.Behaviors  5	      
������ 
������   � DataContext  5      2   ,VisionApplication.MVVM.ViewModel.OutputLogVM  5   
    � 	Resources 5         BorderGapMaskConverter  GroupBoxStyle1 �� 
TargetType �� Property �� Value �� 
TargetType J� SnapsToDevicePixels � ColumnDefinitions � RowDefinitions	 �� CornerRadius
 J� OpacityMask `� ConverterParameter `� 	Converter �� ElementName �� Path �� RelativeSource ��Name �� Padding��%�  &	        &	      7   �� 5   
   6!   �� 5   
   " ��6'   .�5      ��  � ��
 ���� 6,   �� 5       � ��$ .VnLucida sans)�6+   6D   �� 5       � ��$ 17(�6)   �� 5       � ��
 ����6+   �� 5       � ��$ 3q�60   �� 5       � �� 5      �� 5      " ��6&   .R�5      � $
 true��6    5      �� 5   "   $��6 �63   �� 5   "   $
��Auto �63   �� 5    "   $��* �63   �� 5!   "   $��6 �63    5#      �� 5$   "   $
��Auto �60   �� 5%   "   $
��Auto �60   �� 5&   "   $��* �60   �� 5'   "   $��6 �60   .+�5)      �� 
������ #��z��6?   #��z��5*   %   $��4��5+   %   $��0��69   $	 10��6I   $��1��6[   $��3��6h   �� 5,      
������ #��z��6?   $��4��6s   $	 4��6�   $��1��6�   $��3��6�   
 5-   "   `� 5.   &   $ 7��63   8   .1�5/   *   �� $ Header��62   $ ActualWidth�6G   �� 50   *   $ ActualWidth�62    6E   �� *Self+�� 51   *   $ ActualHeight�62    6F   �� *Self+52   '   ._�54   "   �� #��z��6)   #��z��6U   $	 3��6�   ._�55   &   �� 
������ #��z��6G   $	 2��6{   56   #   57      �� 58      $ Header��6%   $��1��65   $ 0q�6E   $��0��6Q   $��2��6^   ._�59   "   �� $��Header��63   $
��True��6J   # z 6e   5:      �� 5;      $��2��6/   $��1��6C   #��z��6S   $��2��6v   # z 6�   5<      5=      5?      5@      5A      .��5C      � .+�5D   
   �� $��0 0 0 0q�6   #���  5E      .��5F      � .+�5G      ,� -   5         ,�Name$ flowDocumentSVOutputlog��5G   '   
������  ,� SelectionBrush �	#FF4092D3
������ ��5J   3   �� *	OutputLog+*  $Microsoft.Xaml.Behaviors.Interaction   	Behaviors 5K      :   4VisionApplication.MVVM.Behaviors.ScrollToEndBehavior  5L      5O      5P      5Q      5R      5S      