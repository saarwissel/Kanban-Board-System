   M S B A M L   `   `   `  ����   IntroSE.Kanban.Frontend,    &IntroSE.Kanban.Frontend.View.BoardView   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  Board��$	��600��$	��800�� � WindowStartupLocation$ CenterScreen=�.��]� $��10q�.+��� -   $
��Back��$��Bottom=�$	��100��$��	0,0,20,20q�$��Right=��� $	��Top=�$��0,0,0,10q�.+��� $��24(�$
��Bold"����� *	BoardName+�� $��14(�$��Italic$�$��0,5,0,0q����� *
OwnerEmail+ �� StringFormat$ 
Owner: {0}���� $��Members:��$��SemiBold"�$��0,10,0,5q��� $
��10,0q����� *	Members+���� .R��� $��13(����� �  � ColumnDefinitions �� $��* ��� $��* ��� $��* �.+��� $��0��$��5q�.+��� $��Backlog��$
��Bold"�$��16(�$��0,0,0,10q��� ���� *BacklogTasks+���� .R��� $��13(����� �� $��1��$��5q�.+��� $��In Progress��$
��Bold"�$��16(�$��0,0,0,10q��� ���� *InProgressTasks+���� .R��� $��13(����� �� $��2��$��5q�.+��� $
��Done��$
��Bold"�$��16(�$��0,0,0,10q��� ���� *	DoneTasks+���� .R��� $��13(����� 