   M S B A M L   `   `   `  ����L  HVisionApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullS.clr-namespace:VisionApplication.MVVM.Behaviors VisionApplication.MVVM.Behaviors  5      S.clr-namespace:VisionApplication.MVVM.ViewModel VisionApplication.MVVM.ViewModel  5      I)clr-namespace:VisionApplication.MVVM.ViewVisionApplication.MVVM.View  5	      /    )VisionApplication.MVVM.View.StepDebugView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors  5      ?
behaviors1.clr-namespace:VisionApplication.MVVM.Behaviors  5      >	viewmodel.clr-namespace:VisionApplication.MVVM.ViewModel  5      4view)clr-namespace:VisionApplication.MVVM.View  5	      .��5      � .+�5   
   ��   �� CornerRadius$  0��6   $��1q�6"   ���	#FF86D2F9���	#FF242526._�5      �� .+�5      ��  �� Orientation$ 
Horizontal=�6   $��5,10,0,5q�67   .+�5      �� $
��Step��6   $��17(�5      $��50��5      
�������$��Center=�5      $
��Bold"�5      5      �� 5      $��Information��6   $��Center=�5       $��17(�5!      $	��450��5"      
�������$
��Bold"�5$      5%      5&      5'      5(      �� 5)   
   -   5         ��Name$ scv_StepDebugScrollView��5)      $
��Auto=�68   $
��Auto=�6]   $��0,40,0,0q�6�   .��5/      �� -   5        $��15(�5/      
���   � �� SelectedItem 50      �� *selectedItem+ �� Mode$ TwoWay=���6{   �� *listStepDebugInfors+*  $Microsoft.Xaml.Behaviors.Interaction   	Behaviors 51      =   7VisionApplication.MVVM.Behaviors.EventToCommandBehavior  52         	EventName$ SelectionChanged��68      Command 53   )   �� *SelectionChangedCommand+��56      �� 57      .R�58      �� $ 
Horizontal=�6%   .+�59      �� 
�������$��50��5:   E   $��15(�5;   E   ��59   (   �� *
str_Step+5<      �� 5=      
�������$
��Auto��5>   E   $��15(�5?   E   ��5=   (   �� *str_Message+5@      5A      5B      5D      5F      5G      5H      