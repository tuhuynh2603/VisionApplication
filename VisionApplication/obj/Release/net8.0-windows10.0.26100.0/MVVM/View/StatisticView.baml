   M S B A M L   `   `   `  ���� L  HVisionApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:VisionApplication.MVVM.ViewVisionApplication.MVVM.View  S.clr-namespace:VisionApplication.MVVM.ViewModel VisionApplication.MVVM.ViewModel  /    )VisionApplication.MVVM.View.StatisticView   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     ;local)clr-namespace:VisionApplication.MVVM.View     D	viewmodel.clr-namespace:VisionApplication.MVVM.ViewModel     .���   � ColumnDefinitions  �� $
��6.1* ��� $
��3.1* �.+�3   -VisionApplication.MVVM.View.MappingCanvasView  $��0�� � DataContext �� *mMappingCanvasVM+� $��1��.+��� $
��Auto�� �� CornerRadius$ 8��$��3q�
������  J� Opacity$	 0.8Q�$��0.2,0,0.4,0.2q�._�]� $
��True��.+�]� $
��True��$	��Top=�.+��� $��5q�$��
Statistics��
�������$��15(���Z� .���  � RowDefinitions �� $��50 ��� .+�� $��0��$��5,5,20,5q�  �� $��1* ��� $
��1.5* ��� $
��1.5* �.+��� $��0��$��16(�$��Microsoft New Tai Lue)�$
��Left=�$�� ��$
��Wrap=�
��������� $��1��$��16(�$��Microsoft New Tai Lue)�$��Right=� �� TextAlignment$ Right=�$��Center=�$��Camera 1��$
��Wrap=�
��������� $��2��$��16(�$��Microsoft New Tai Lue)�$��Right=�$ Right=�$��Center=�$��Barcode��$
��Wrap=�
��������� -   $��lboxStatistic��$��1��$��5,5,20,5q�
������ $��0q����� *listSummary+����  �� 
TargetType" ��.���  �� Property � �� �� Value �� 	 �� 
TargetType"	 ��.R��� ���� .R�5   /VisionApplication.MVVM.View.SummaryTemplateView  