   M S B A M L   `   `   `  ���� L  HVisionApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullS.clr-namespace:VisionApplication.MVVM.Behaviors VisionApplication.MVVM.Behaviors  S.clr-namespace:VisionApplication.MVVM.ViewModel VisionApplication.MVVM.ViewModel  3    -VisionApplication.MVVM.View.WarningMessageBox   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     Cbehavior.clr-namespace:VisionApplication.MVVM.Behaviors     D	viewmodel.clr-namespace:VisionApplication.MVVM.ViewModel     Hi:http://schemas.microsoft.com/expression/2010/interactivity     .���   � 	Resources     continueImage � RowDefinitions �� TextAlignment  
AbortImage  	NextImage  PreviousImage  
RetryImage��%2  &	        &	 �     &	 >    &	 �    &	 n    �  �� $��3* ��� $��1* �.+��� $��0��$+��%/Resources/ContinueSequenceButton.png��$��3q��� $��1��$��Continue��
���   �$ Center=�$��20(��  �� $��3* ��� $��1* �.+��� $��0��$(��"/Resources/AbortSequenceButton.png���� $��1��$��Abort��
���   �$ Center=�$��20(��  �� $��3* ��� $��1* �.+��� $��0��$'��!/Resources/SequenceNextButton.png���� $��1��$
��Next��
���   �$ Center=�$��20(��  �� $��3* ��� $��1* �.+��� $��0��$+��%/Resources/SequencePreviousButton.png���� $��1��$
��Back��
���   �$ Center=�$��20(��  �� $��3* ��� $��1* �.+��� $��0��$"��/Resources/SequenceRetry.png���� $��1��$��Retry��
���   �$ Center=�$��20(� �� $
��0.5* ��� $��2* ��� $
��1.5* �.+��� $��0��
��� ���
����� �$��1q�._��� $��Warning Message��$��Center=�$��Center=�$��Center=�
���  ��$��20(��� $��5q�$��1��$
��Wrap=�$��Center=�$��Center=�$ Center=�$��20(�
���   ����� *strWarningMessage+� $��2��
������� � ColumnDefinitions �� $��2* ��� .+��� $��1�� �� Orientation$ 
Horizontal=�$��Right=�$��10,5,5,5q�.+��� -    ��Name$ btn_Sequence_Continue��
������ $��0q�#��[  $��5,0,5,0q����� * btn_Sequence_Continue_ClickCmd+y��� *btn_Sequence_ContinueVisible+�� -   $ btn_Sequence_Abort��
������ $��0q�#��[ $��5,0,5,0q����� *btn_Sequence_Abort_ClickCmd+y��� *btn_Sequence_AbortVisible+�� $��0��$ 
Horizontal=�$��Right=�$��10,5,5,5q�.+��� -   $ btn_Sequence_Previous��
������ $��0q�#��[ $��5,0,5,0q����� * btn_Sequence_Previous_ClickCmd+y��� *btn_Sequence_PreviousVisible+�� -   $ btn_Sequence_Next��
������ $��0q�#��[ $��5,0,5,0q����� *btn_Sequence_Next_ClickCmd+y��� *btn_Sequence_NextVisible+�� -   $ btn_Retry_Current_Step��
������ $��0q�#��[ $��5,0,5,0q����� *!btn_Retry_Current_Step_ClickCmd+y��� *btn_Retry_Current_StepVisible+