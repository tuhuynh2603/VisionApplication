   M S B A M L   `   `   `  ���� L  HVisionApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:VisionApplication.MVVM.ViewVisionApplication.MVVM.View  +    %VisionApplication.MVVM.View.ImageView   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     X TXceed.Wpf.Toolkit, Version=4.6.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f47xceedtk)http://schemas.xceed.com/wpf/xaml/toolkit  5local)clr-namespace:VisionApplication.MVVM.View    � 	Resources   �� Source�� $7 1/VisionApplication;component/ResourceControl.xaml?�%    .��� -    �Name$ grd_Image_View��.+�� $��Stretch=�$��0,0,0,30q�$��Stretch=����#1a1a1a$
}�True��.+�]� -    ]�Name$ panelSliderGray��$��20��$	��Top=� K� ZIndex$ 2��$y�	Collapsed=�$
��True�����	#FF1B1B27 J� Opacity$ 1Q�.+��� -    ��Name$ 	sldBinary��$��2 0 0 0q�$	��250��$��20��
������ $	��Top=�$
��Left=� �� TickFrequency$ 1Q� � Maximum$	 255Q�	 � SmallChange$	 1Q����	#FFBD3F24$��1q�
 �� IsSnapToTickEnabled$

 True�� � Value ��  �� Path$
 Text� �� ElementName$ sldValueShow�� �� Mode$ TwoWay=��� -    ��Name$ sldValueShow��$��30��
������ 
��� ��� �� TextAlignment$ Center=�$��10(����� $ Value�$ 	sldBinary��$ TwoWay=�� -   $ grd_Dock��$	��Top=�$
��Left=�
�����w�.+��� -    ��Name$ image��$��0 0 0 0q�$��Uniform=�$	��Top=�$
��Left=� �� BitmapScalingMode$ NearestNeighbor=�$ 1��� -   $ GridCoordinate��$	��Top=�$
��Left=�$ 4������ $ image��$ ActualHeight�$ OneWay=� �� IsAsync$
 True������ $ image��$ ActualWidth�$ OneWay=�$
 True���� -    ��Name$ GridOverlay��$	��Top=�$
��Left=�$ 2������ $ image��$ ActualHeight�$ OneWay=�$
 True������ $ image��$ ActualWidth�$ OneWay=�$
 True���� -	   $ 	gridteach��$	��Top=�$
��Left=�$ 5������ $ 	imageView��$ ActualWidth�$ OneWay=����� $ 	imageView��$ ActualHeight�$ OneWay=�.+��� -
    ��Name$ 
controlWin��$y�	Collapsed=�$��80��$ 5��$��10��$��80��$��10�� �� Left$	 200��
 �� Top$	 220��   DesignerItemTemplate#��[  .��� .+��� 
���\\��$��-3q�$��3�� J� IsHitTestVisible$ False���� -   $��	rotaPoint��
���\\��$��	-6 -6 0 0q�$	��Top=�$
��Left=�$��6��$ False���� -   $��GridTeachOverlay������ $ 	imageView��$ ActualWidth�$ OneWay=����� $ 	imageView��$ ActualHeight�$ OneWay=��� -   $ 
GridResult��$	��Top=�$
��Left=�$ 3������ $ image��$ ActualHeight�$ OneWay=�$
 True������ $ image��$ ActualWidth�$ OneWay=�$
 True���� -   $��resultTeach��$��	10,30,0,0q� �� Orientation$ Vertical=�$ 2��.+��� -    ��Name$ tbl_Status1�����	#FF1A46FF$��35(�$
��Bold"��� -   $ tbl_Status2�����	#FFFF0621$��25(�$
��Bold"�� $��20�����#212020$	 0.9Q�$��Bottom=� � ColumnDefinitions �� $��3* ��� $��1* �.+��� $��0��$ 
Horizontal=�$��Center=�$
��Left=�.+��� $ 
Horizontal=�$	��250��$��0,0,0,0q�$��Center=�.+��� $
��Pos:��$��0,0,8,0q�$
��Left=�$��Center=� ( $styleTextBoxTaskBarImageView_Content#���  �� -   $ tbl_Pos��$��[0,0] Robot [0,0]��$
��Auto��$��0,0,2,0q�$
��Left=� �� TextAlignment$
 Left=�$��Center=� & "styleTextBoxTaskBarImageView_Title#���  �� $ 
Horizontal=�$	��110��$��Center=�.+��� $��Gray: ��$��0,0,8,0q�$��Right=�$��Center=�#���  �� -   $ tbl_Value_gray��$	��[0]��$
��auto��$��0,0,2,0q�$
��Left=�$
 Left=�$��Center=�#���  �� $��1��$ 
Horizontal=�$��Center=�$��Right=�.+��� $
��Time��$��10,0,8,0q�$��Right=�$��Center=�#���  �� -   $ tbl_InspectTime��$��0��$��30��$��0,0,5,0q�$
��Left=�$ Right=�$��Center=�#���  �� $��ms��$��0,0,10,0q�$��20��$��Right=�$��Center=�#���  