   M S B A M L   `   `   `  ����  IntroSE.Kanban.Frontend-    'IntroSE.Kanban.Frontend.View.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Boards��5   	   $	��500��6   $	��700��6%    � WindowStartupLocation$ CenterScreen=�5   	   .��5      ]� $��20q�6   .+�5   
   �� $	��Top=�6   $��0,0,0,20q�6*   .+�5      �� $��20(�5	      $
��Bold"�6&   ��5      �� *Email+ �� StringFormat$ 	Hello {0}��5	   &   �� 5
      $��
My Boards:��6   $��16(�6*   $��SemiBold"�68   $��0,5,0,10q�6N   �� 5      $��	Add Board��6   $	��100��6M   ��6)   �� *AddBoardCommand+6M   5      �� 5   
   $��Stretch=�62   ��6   �� *Boards+��5      �� 5      .R�5      �� 
�������$��1q�60    �� CornerRadius$ 8��6D   $��4q�6U    �� Padding$ 8q�6`   ._�5      ]� .+�5      �� $
��Left=�6)   $	��400��6?   .+�5   "   �� $
��Bold"�6B   $��16(�6T   ��6,   �� *Name+6T   �� 5   "   $��Italic$�6\   ��6,   �� *Owner+$ 
Owner: {0}��6\   5      �� 5       �� Orientation$ 
Horizontal=�6)   $��Right=�6B   .+�5   "   �� $
��Open��6)   $	��5,0q�5   )   ��5   )   �� *DataContext.OpenBoardCommand+ �� RelativeSource ��  �� AncestorType
 Window��5   )   �� 5   )   �� 5   "   $��30��5   )   $��30��64   
������ $��0q�5    )   ��5   )   �� * DataContext.DeleteBoardCommand+ �� 
 Window��5   )   �� .��5!   &   �� $	��✖��60   $��14(�69   
���  ��6G   5"   #   5#      5$      5%      5&      5(      5)      5*      