   M S B A M L   `   `   `  ���� L  HVisionApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullS.clr-namespace:VisionApplication.MVVM.Behaviors VisionApplication.MVVM.Behaviors  S.clr-namespace:VisionApplication.MVVM.ViewModel VisionApplication.MVVM.ViewModel  +    %VisionApplication.MVVM.View.LoginUser   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     Cbehavior.clr-namespace:VisionApplication.MVVM.Behaviors     D	viewmodel.clr-namespace:VisionApplication.MVVM.ViewModel     Hi:http://schemas.microsoft.com/expression/2010/interactivity     $	��400��$	��400��  � 	Resources     ForegroundColor �� TargetProperty	 �� To g� Duration g� AutoReverse��%O   &	        �� .���� $ Foreground.Color� Red$ 0:0:0.9D�$
 True�� J� InputBindings �� 
 �� Key$ Enter�� �� Command �� *LoginCommand+.��� ����  �� EndPoint$ 0.5,1&�	 �� 
StartPoint$	 0.5,0&�.��� 
 � Color$
 White�� � Offset$ 0.439Q�� $
 	#FF17717E��$ 0.808Q�.+��� $ ��..\..\Resources\LOGOHD.png��$	��Top=��  � ColumnDefinitions �� �� .+��� $��0��$	��Top=�.+���  �� Orientation$ 
Horizontal=�.+��� $�� �����	#FFD16512$��Segoe Print)�$��28(�$��Center=�$
��Bold"�$��	10 20 0 0q��� $��0 0 0 0q��� $��0,0,0,0q�$��0 50 0 0q�._��� $ 
Horizontal=�.+��� $��User:��$��5,1,0,1q�$��Microsoft YaHei UI)����	#FFD16512$��12(��� -    ��Name$ currentUser��$��5,1,0,1q�$��61,0,0,0q�$��Microsoft YaHei UI)����	#FFD16512$
��None��$��12(�$��SemiBold"��� $��0,0,0,0q�
�������._��� $ 
Horizontal=�.+��� $��Access Level:��$��5,1,0,1q����	#FFD16512$��Microsoft YaHei UI)�$��12(��� -   $ currentAccesslevel��$
��None��$��5,1,0,1q����	#FFD16512$��15,0,0,0q�$��Microsoft YaHei UI)�$��12(�$
��Bold"��� $��Bottom=�$��5,0,0,20q�.+��� -   $	��tbx��$��Log In��$��2q�$
��Left=�$��	Underline{�$��14(�����  �� 
TargetType" ��.���  �� Property � �� �� Value �	#FFD16512 �� Triggers P�  P� Property �@J�IsMouseOver P� Value$
 True��. ���  � ��
 ��  �:�  :� RoutedEvent$ 	MouseDown��[��� #��[  �� -    ��Name$
 mncs��$��Create New User Account��$
��Left=�$��2q�$��	Underline{�$��14(����� " ��.���  � �� �	#FFD16512 P�  �@J�IsMouseOver$
 True��. ���  � ��
 ��  �:� $ 	MouseDown��[��� #��[  �� -   $��Change Password��$
��Left=�$��2q�$��	Underline{�$��14(����� " ��.���  � �� �	#FFD16512 P�  �@J�IsMouseOver$
 True��. ���  � ��
 ��  �:� $ 	MouseDown��[��� #��[  �� -   $!��Delete Current User Account��$
��Left=�$��2q�$��13(�$��Italic$����� " ��.���  � �� �	#FFD16512 P�  �@J�IsMouseOver$
 True��. ���  � ��
 ��  �:� $ 	MouseDown��[��� #��[  �� -   $��Log Out��$
��Left=�$��2q�$��Italic$�$��13(����� " ��.���  � �� �	#FFD16512 P�  �@J�IsMouseOver$
 True��. ���  � ��
 ��  �:� $ 	MouseDown��[��� #��[  �� -    ��Name$ 
panelLogIn��$��1��$y�Visible=� K� ZIndex$ 2��$��0q� J� RenderTransformOrigin$ 0.5,0.5&�._��� -	    ��Name$ 
PanelLogIn��$	��Top=�$��	0,130,0,0q�$��Center=�.+��� -
   $��lbLogin��$��Log In��$��25(����	#FFD16512$
��Bold"�$��Expanded&�$��Microsoft YaHei UI)��� -   $ NotifyLogin��
���  ��$��
11,-10,0,0q����� " ��.���  � y�$ Visible=� ��  �� Binding �� *Text+ �� Value  .a���  � y�$ 	Collapsed=��� $��0,5,0,0q� �� CornerRadius$ 10��$��3q����	#FFF0EEDB._��� -    ��Name$ userName��
�������$��USERNAME��$��0q� � ToolTip$ Enter Username��$��0����Z� ��Z� 4   .VisionApplication.MVVM.Behaviors.FocusBehavior    	IsFocused  �� *	IsFocused+�� $��0,15,0,0q�$ 10��$��3q����	#FFF0EEDB._�G� -   ! G�Name$! passWord��$��1��" G� Password$" 
1111111111��
�������$ Enter Password����Z� ��Z� �� $ 
Horizontal=�$��20q�.+��� $ 10��$��3q����� $ 0.5,1&�$	 0.5,0&�.��� $
 	#FFD16512��$ 0Q�� $
 	#FFE58B12��$ 0.996Q�._��� -   # ��Name$# loginOk��$��80��$��35��$��15(�$
��Bold"�
�������$��Login����Z� ��Z� ���� *LoginCommand+�� $ 10��$��	 10,0,0,0q�$��0q����� $ 0.5,1&�$	 0.5,0&�.��� $
 	#FFD16512��$ 0Q�� $
 	#FFE58B12��$ 0.996Q�._��� -   $# loginCancel��$��80��$��35��
�������$��15(�$
��Bold"�$��Cancel����Z� ��Z� �� -   $ panelCreateUser��$��1��$y�Visible=�$ 2��$��0,0,0,0q�._��� $	��Top=�$��	0,100,0,0q�$��Center=�.+��� $��Create New User��$��25(����	#FFD16512$
��Bold"�$��Expanded&�$��Center=�$��0,27,0,0q�$��Microsoft YaHei UI)��� -   $ NotifyNewUser�����	#FFD16512$��38,-20,0,10q����� " ��.���  � y�$ Visible=� ��  �� *Text+  .a���  � y�$ 	Collapsed=��� $��0,0,0,0q�$	��200��$ 10��$��3q����	#FFF0EEDB._��� -   $ userNameNew��
�������$��USERNAME��$ Enter Username����Z� ��Z� �� $��0,15,0,0q�$ 10��$	��200��$��3q����	#FFF0EEDB._�G� -   $! passWordNew��$" 
1111111111��
�������$ Enter Password����Z� ��Z� �� $��0,15,0,0q�$ 10��$	��200��$��3q����	#FFF0EEDB._�G� -   $! ConfirmPassWordNew��$" 
1111111111��
�������$ Confirm Password����Z� ��Z� �� $ 
Horizontal=�$��5,5,0,0q�$	��200��$��Center=�$��20��.+�	� -   $��engineerLevel��$��Engineer��$��Center=�$ d� 	IsChecked$
$ TrueY�$��0,0,7,0q�$��0q�$��70��$��SemiBold"�$��Center=����	#FFD16512	� -   $��operatorLevel��$��Operator��$��Center=�$��0q�$��70��$��SemiBold"�$��0,0,7,0q����	#FFD16512	� -   $��	userLevel��$
��User��$��0q�$��Center=�$��70��$��SemiBold"����	#FFD16512�� $ 
Horizontal=�$��Center=�$��20,10,20,20q�.+��� $ 10��$��0,0,40,0q�$��3q����� $ 0.5,1&�$	 0.5,0&�.��� $
 	#FFD16512��$ 0Q�� $
 	#FFE58B12��$ 0.996Q�._��� -   $# createOk��$��80��$��Create����Z� ��Z� �� $ 10��$��	 10,0,0,0q�$��Right=�$��3q����� $ 0.5,1&�$	 0.5,0&�.��� $
 	#FFD16512��$ 0Q�� $
 	#FFE58B12��$ 0.996Q�._��� -   $# createCancel��$��80��$��Cancel����Z� ��Z� �� -   $ panelChangePassword��$��1��$y�Visible=�$ 0��$��0,0,0,0q�._��� -   $ PanelChangePassword��$	��Top=�$��	0,130,0,0q�$��Center=�.+��� $��New Password��$��25(����	#FFD16512$
��Bold"�$��Expanded&�$��Microsoft YaHei UI)��� -   $ NotifyChangePw��
���  ��$��	10,20,0,0q����� " ��.���  � y�$ Visible=� ��  �� *Text+  .a���  � y�$ 	Collapsed=��� $��0,0,0,0q�$ 10��$��3q����	#FFF0EEDB._�G� -   $! NewPassWord��$" 
1111111111��
�������$ Enter New Password����Z� ��Z� �� $��0,15,0,0q�$ 10��$��3q����	#FFF0EEDB._�G� -   $! ConfirmNewPassWord��$" 
1111111111��
�������$ Confirm Password����Z� ��Z� �� $ 
Horizontal=�$��20q�.+��� $ 10��$��3q����� $ 0.5,1&�$	 0.5,0&�.��� $
 	#FFD16512��$ 0Q�� $
 	#FFE58B12��$ 0.996Q�._��� -   $# chagneOk��$��80��$��Change����Z� ��Z� �� $ 10��$��	 10,0,0,0q�$��3q����� $ 0.5,1&�$	 0.5,0&�.��� $
 	#FFD16512��$ 0Q�� $
 	#FFE58B12��$ 0.996Q�._��� -    $# changecancel��$��80��$��Cancel����Z� ��Z� 