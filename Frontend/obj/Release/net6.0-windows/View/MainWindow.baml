   M S B A M L   `   `   `  ����   IntroSE.Kanban.Frontend-    'IntroSE.Kanban.Frontend.View.MainWindow   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  Boards��$	��500��$	��700�� � WindowStartupLocation$ CenterScreen=�.��]� $��20q�.+��� $	��Top=�$��0,0,0,20q�.+��� $��20(�$
��Bold"����� *Email+ �� StringFormat$ 	Hello {0}���� $��
My Boards:��$��16(�$��SemiBold"�$��0,5,0,10q��� $��	Add Board��$	��100������ *AddBoardCommand+�� $��Stretch=����� *Boards+���� .R��� 
�������$��1q� �� CornerRadius$ 8��$��4q� �� Padding$ 8q�._�]� .+��� $
��Left=�$	��400��.+��� $
��Bold"�$��16(����� *Name+�� $��Italic$����� *Owner+$ 
Owner: {0}����  �� Orientation$ 
Horizontal=�$��Right=�.+��� $
��Open��$	��5,0q����� *DataContext.OpenBoardCommand+ �� RelativeSource ��  �� AncestorType
 Window���� �� $��30��$��30��
������ $��0q����� * DataContext.DeleteBoardCommand+ �� 
 Window���� .���� $	��✖��$��14(�
���  ��