   M S B A M L   `   `   `  ����L  HVisionApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:VisionApplication.MVVM.ViewVisionApplication.MVVM.View  5      S.clr-namespace:VisionApplication.MVVM.ViewModel VisionApplication.MVVM.ViewModel  5      /    )VisionApplication.MVVM.View.StatisticView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;local)clr-namespace:VisionApplication.MVVM.View     5      D	viewmodel.clr-namespace:VisionApplication.MVVM.ViewModel     5      .��5      �   � ColumnDefinitions  5   
   �� 5      $
��6.1* �6   6.   �� 5      $
��3.1* �6    6/   .+�5   
   3   -VisionApplication.MVVM.View.MappingCanvasView  $��0��6K    � DataContext 6"   �� *mMappingCanvasVM+6K   � 5   
   $��1��6   .+�5      �� $
��Auto��6    �� CornerRadius$ 8��5      $��3q�6.   
������  J� Opacity$	 0.8Q�5   6   $��0.2,0,0.4,0.2q�5      ._�5      ]� $
��True��6   .+�5      ]� $
��True��6    $	��Top=�65   .+�5      �� $��5q�6#   $��
Statistics��6.   
�������$��15(�6Z   ��6h   Z� .��5   "   �  � RowDefinitions 5    &   �� 5!   *   $��50 �68   �� 5"   *   .+�5$   &   � $��0��6+   $��5,5,20,5q�68     5%   *   �� 5&   .   $��1* �6?   �� 5'   .   $
��1.5* �6?   �� 5(   .   $
��1.5* �6?   .+�5+   *   �� $��0��5,   1   $��16(�5-   1   $��Microsoft New Tai Lue)�5.   1   $
��Left=�5/   1   $�� ��50   1   $
��Wrap=�51   1   
�������53   +   �� 54   *   $��1��55   1   $��16(�56   1   $��Microsoft New Tai Lue)�57   1   $��Right=�58   1    �� TextAlignment$ Right=�59   1   $��Center=�5:   1   $��Camera 1��5;   1   $
��Wrap=�5<   1   
�������5>   +   �� 5?   *   $��2��5@   1   $��16(�5A   1   $��Microsoft New Tai Lue)�5B   1   $��Right=�5C   1   $ Right=�5D   1   $��Center=�5E   1   $��Barcode��5F   1   $
��Wrap=�5G   1   
�������5I   +   5J   '   �� 5M   &   -   5        $��lboxStatistic��5O   .   $��1��5M   .   $��5,5,20,5q�6;   
������ $��0q�5O   C   ��5P   -   �� *listSummary+��5R   *   �� 5S   .    �� 
TargetType" ��64   .�5T   2   ��  �� Property � �� �� Value 5U   6   �� 5V   :   	 �� 
TargetType"	 ��6J   .R�5W   >   �� 6Q   5X   ;   5Z   3   5[   /   ��5^   &   �� 5_   *   .R�5`   .   5   /VisionApplication.MVVM.View.SummaryTemplateView  5a   +   5c   #   5e   #   5g      5h      5i      5j      5k      5l      5m      