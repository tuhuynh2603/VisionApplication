   M S B A M L   `   `   `  ���� L  HVisionApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:VisionApplication.MVVM.ViewVisionApplication.MVVM.View  S.clr-namespace:VisionApplication.MVVM.ViewModel VisionApplication.MVVM.ViewModel  S.clr-namespace:VisionApplication.MVVM.Behaviors VisionApplication.MVVM.Behaviors  /    )VisionApplication.MVVM.View.OutputLogView   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     ;local)clr-namespace:VisionApplication.MVVM.View     D	viewmodel.clr-namespace:VisionApplication.MVVM.ViewModel     _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors  =behavior.clr-namespace:VisionApplication.MVVM.Behaviors  
������ 
������   � DataContext  2   ,VisionApplication.MVVM.ViewModel.OutputLogVM   � 	Resources    BorderGapMaskConverter  GroupBoxStyle1 �� 
TargetType �� Property �� Value �� 
TargetType J� SnapsToDevicePixels � ColumnDefinitions � RowDefinitions	 �� CornerRadius
 J� OpacityMask `� ConverterParameter `� 	Converter �� ElementName �� Path �� RelativeSource ��Name �� Padding��%#  &	        &	      7   �� �� " ��.���  � ��
 ���� ��  � ��$ .VnLucida sans)���  � ��$ 17(���  � ��
 ������  � ��$ 3q���  � �� �� " ��.R�� $
 true�� �� $��6 ��� $
��Auto ��� $��* ��� $��6 � �� $
��Auto ��� $
��Auto ��� $��* ��� $��6 �.+��� 
������ #��z��#��z��$��4��$��0��$	 10��$��1��$��3���� 
������ #��z��$��4��$	 4��$��1��$��3��
 `� $ 7��8   .1��� $ Header��$ ActualWidth��� $ ActualWidth� �� *Self+�� $ ActualHeight� �� *Self+._��� #��z��#��z��$	 3��._��� 
������ #��z��$	 2���� $ Header��$��1��$ 0q�$��0��$��2��._��� $��Header��$
��True��# z �� $��2��$��1��#��z��$��2��# z .��� .+��� $��0 0 0 0q�#���  .��� .+�,� -    ,�Name$ flowDocumentSVOutputlog��
������  ,� SelectionBrush �	#FF4092D3
������ ���� *	OutputLog+*  $Microsoft.Xaml.Behaviors.Interaction   	Behaviors :   4VisionApplication.MVVM.Behaviors.ScrollToEndBehavior  