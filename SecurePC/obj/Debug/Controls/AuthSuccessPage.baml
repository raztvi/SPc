   M S B A M L   `   `   `  ����  SecurePC'    !SecurePC.Controls.AuthSuccessPage   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      -   5          N�Name$  FaceVerificationPane��5   0   $	��360��6N   $	��710��6^    N� FontSize$ 14(�6j    N� Title$ Auth Success��5      .��5      � $��Center=�6    � ColumnDefinitions 5   
   �� 5      $	��300 �6   �� 5      $	��110 �6   �� 5      $	��300 �6    � RowDefinitions 5   
   �� 5      $��35 �6   �� 5      $��20 �6   �� 5      $��40 �6   �� 5      $��10 �6   �� 5      $��4* �6   �� 5      $��22 �6   .+�5   
   �� $��0��6   $��0��6$   $��10,0,0,0q�61   $
��Left=�6C   $	��Top=�6^   $ ��Successful Authentication!��6v   $
��Wrap=�6�   ���	#FF1B5106$��20(�6�   �� 5   
   $��0��6   $��1��6$   $��10,0,0,0q�61   $
��Left=�6C   $	��Top=�6^   $��Results:��6v   $
��Wrap=�6�   �� 5   
   $��0��6   $��2��6$   $��10,0,0,0q�61   $
��Left=�6C   $	��Top=�6^   $
��Wrap=�6�   ��6v   �� *FaceVerifyResult+ �� ElementName$ FaceVerificationPane��6�   �� 5   
   -   5        $��GoBack��5   N   $��1��6   $��2��6!   $��Go back��6.   $��10(�6@   ���#00b294$
��Left=�6q   $��20��6�   $	��Top=�6�   $��75��6�   6�   � 5   
   $��0��6   $��4��6   .+�5       ��  K� ZIndex$	 999��6j   ��6   �� *LeftResultCollection+$ FaceVerificationPane����5!      �� 5"      .R�5#      �� 
������ $��Center=�6:   $��Center=�6W   ��6r   �� *MaxImageSize+$ FaceVerificationPane����6�   �� *MaxImageSize+$ FaceVerificationPane��5$      ��5&      �� 5'       �� 
TargetType" ��6   .�5(      ��  �� Property �@��Left	 �� Value	 68   �� *Left+�� 5)       �@��Top	 67   �� *Top+5*      ��5,      �� 5-      .R�5.      �� ���	#FF26B8F4$��1��6i   ��6$   �� *Width+��6<   �� *Height+6i   5/      51      �� 52      -   5        
 ��Name$
 LeftImageDisplay��52      $��Center=�6�   $��Center=�6�   $��0,0,0,0q�6�   $,��&/SecurePC;component/Assets/default.jpg��6�   ��6.   �� *MaxImageSize+$ FaceVerificationPane����6p   �� *MaxImageSize+$ FaceVerificationPane��6�   53      � 54   
   $��2��6   $��4��6   .+�55      �� $	 999��6k   ��6   �� *RightResultCollection+$ FaceVerificationPane����56      �� 57      .R�58      �� 
������ $��Center=�6:   $��Center=�6W   ��6r   �� *MaxImageSize+$ FaceVerificationPane����6�   �� *MaxImageSize+$ FaceVerificationPane��59      ��5;      �� 5<      " ��6   .�5=      ��  �@��Left	 68   �� *Left+�� 5>       �@��Top	 67   �� *Top+5?      ��5A      �� 5B      .R�5C      �� ���	#FF26B8F4$��1��6i   ��6$   �� *Width+��6<   �� *Height+6i   5D      5F      �� 5G      -   5        $
 RightImageDisplay��5G      $��Center=�6�   $��Center=�6�   $��0,0,0,0q�6�   $,��&/SecurePC;component/Assets/default.jpg��6�   ��6/   �� *MaxImageSize+$ FaceVerificationPane����6q   �� *MaxImageSize+$ FaceVerificationPane��6�   5H      5J      5L      