   M S B A M L   `   `   `  ���� L  HVisionApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullS.clr-namespace:VisionApplication.MVVM.Behaviors VisionApplication.MVVM.Behaviors  S.clr-namespace:VisionApplication.MVVM.ViewModel VisionApplication.MVVM.ViewModel  I)clr-namespace:VisionApplication.MVVM.ViewVisionApplication.MVVM.View  /    )VisionApplication.MVVM.View.StepDebugView   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors  ?
behaviors1.clr-namespace:VisionApplication.MVVM.Behaviors  >	viewmodel.clr-namespace:VisionApplication.MVVM.ViewModel  4view)clr-namespace:VisionApplication.MVVM.View  .��� .+���   �� CornerRadius$  0��$��1q����	#FF86D2F9���	#FF242526._��� .+���  �� Orientation$ 
Horizontal=�$��5,10,0,5q�.+��� $
��Step��$��17(�$��50��
�������$��Center=�$
��Bold"��� $��Information��$��Center=�$��17(�$	��450��
�������$
��Bold"��� -    ��Name$ scv_StepDebugScrollView��$
��Auto=�$
��Auto=�$��0,40,0,0q�.���� -   $��15(�
���   � �� SelectedItem �� *selectedItem+ �� Mode$ TwoWay=����� *listStepDebugInfors+*  $Microsoft.Xaml.Behaviors.Interaction   	Behaviors =   7VisionApplication.MVVM.Behaviors.EventToCommandBehavior     	EventName$ SelectionChanged��   Command �� *SelectionChangedCommand+���� .R��� $ 
Horizontal=�.+��� 
�������$��50��$��15(����� *
str_Step+�� 
�������$
��Auto��$��15(����� *str_Message+