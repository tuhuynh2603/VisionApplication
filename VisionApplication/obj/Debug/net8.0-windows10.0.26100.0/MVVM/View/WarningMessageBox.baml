   M S B A M L   `   `   `  ����L  HVisionApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullS.clr-namespace:VisionApplication.MVVM.Behaviors VisionApplication.MVVM.Behaviors  5      S.clr-namespace:VisionApplication.MVVM.ViewModel VisionApplication.MVVM.ViewModel  5      3    -VisionApplication.MVVM.View.WarningMessageBox   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Cbehavior.clr-namespace:VisionApplication.MVVM.Behaviors     5      D	viewmodel.clr-namespace:VisionApplication.MVVM.ViewModel     5      Hi:http://schemas.microsoft.com/expression/2010/interactivity     5      .��5      �   � 	Resources  5   
      continueImage � RowDefinitions �� TextAlignment  
AbortImage  	NextImage  PreviousImage  
RetryImage��%�  &	        &	 &    &	 9    &	 J    &	 _    � 5       5      �� 5      $��3* �6$   62   �� 5      $��1* �6$   62   .+�5      �� $��0��6   $+��%/Resources/ContinueSequenceButton.png��6'   $��3q�6V   6c   �� 5      $��1��6   $��Continue��6)   
���   �$ Center=�6L   $��20(�6c   6s   5      � 5       5      �� 5      $��3* �6$   62   �� 5      $��1* �6$   62   .+�5      �� $��0��6   $(��"/Resources/AbortSequenceButton.png��6'   6U   �� 5      $��1��6   $��Abort��6)   
���   �$ Center=�6I   $��20(�6`   6p   5      � 5!       5"      �� 5#      $��3* �6$   62   �� 5$      $��1* �6$   62   .+�5&      �� $��0��6   $'��!/Resources/SequenceNextButton.png��6'   6T   �� 5'      $��1��6   $
��Next��6)   
���   �$ Center=�6H   $��20(�6_   6o   5(      � 5*       5+      �� 5,      $��3* �6$   62   �� 5-      $��1* �6$   62   .+�5/      �� $��0��6   $+��%/Resources/SequencePreviousButton.png��6'   6X   �� 50      $��1��6   $
��Back��6)   
���   �$ Center=�6H   $��20(�6_   6o   51      � 53       54      �� 55      $��3* �6$   62   �� 56      $��1* �6$   62   .+�58      �� $��0��6   $"��/Resources/SequenceRetry.png��6'   6O   �� 59      $��1��6   $��Retry��6)   
���   �$ Center=�6I   $��20(�6`   6p   5:      5>       5A   
   �� 5B      $
��0.5* �6   �� 5C      $��2* �6   �� 5D      $
��1.5* �6   .+�5G   
   �� $��0��6   
��� ���
����� �$��1q�6I   ._�5H      �� $��Warning Message��6   $��Center=�6.   $��Center=�6K   $��Center=�6f   
���  ��$��20(�6�   6�   5I      �� 5J   
   $��5q�6   $��1��6   $
��Wrap=�5K      $��Center=�6-   $��Center=�6J   $ Center=�6e   $��20(�6|   
���   ���5J   ,   �� *strWarningMessage+5K   �   � 5M   
   $��2��6   
������� � ColumnDefinitions 5N      �� 5O      $��2* �6#   �� 5P      .+�5R      �� $��1��6    �� Orientation$ 
Horizontal=�6*   $��Right=�6C   $��10,5,5,5q�6_   .+�5S      �� -   5         ��Name$ btn_Sequence_Continue��5S      
������ $��0q�6R   #��[  5T      $��5,0,5,0q�5W   !   ��5U   !   �� * btn_Sequence_Continue_ClickCmd+y�5V   !   �� *btn_Sequence_ContinueVisible+5W   4   �� 5X      -   5        $ btn_Sequence_Abort��5X      
������ $��0q�6N   #��[ 5Y      $��5,0,5,0q�5\      ��5Z      �� *btn_Sequence_Abort_ClickCmd+y�5[      �� *btn_Sequence_AbortVisible+5\   0   5]      �� 5_      $��0��6   $ 
Horizontal=�6*   $��Right=�6C   $��10,5,5,5q�6_   .+�5`      �� -   5        $ btn_Sequence_Previous��5`      
������ $��0q�6Q   #��[ 5a      $��5,0,5,0q�5c      ��5b      �� * btn_Sequence_Previous_ClickCmd+y�5d      �� *btn_Sequence_PreviousVisible+6S   �� 5f      -   5        $ btn_Sequence_Next��5f      
������ $��0q�6N   #��[ 5g      $��5,0,5,0q�5i      ��5h      �� *btn_Sequence_Next_ClickCmd+y�5j      �� *btn_Sequence_NextVisible+6O   �� 5l      -   5        $ btn_Retry_Current_Step��5l      
������ $��0q�6S   #��[ 5m      $��5,0,5,0q�5p   !   ��5n      �� *!btn_Retry_Current_Step_ClickCmd+y�5o      �� *btn_Retry_Current_StepVisible+5p   4   5r      5s      5t      5u      