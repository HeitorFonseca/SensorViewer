   M S B A M L   `   `   `  ����H  DSensorsViewer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:SensorsViewer.HomeSensorsViewer.Home  5   	   c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity 5	   	   f bGalaSoft.MvvmLight.Platform, Version=5.3.0.19032, Culture=neutral, PublicKeyToken=5f873c45e98af8a1lMclr-namespace:GalaSoft.MvvmLight.Command;assembly=GalaSoft.MvvmLight.PlatformGalaSoft.MvvmLight.Command 5
   	   U QMahApps.Metro, Version=1.5.0.23, Culture=neutral, PublicKeyToken=f4fb5a3c4d1e5b4ffCclr-namespace:MahApps.Metro.Controls.Dialogs;assembly=MahApps.MetroMahApps.Metro.Controls.Dialogs 5   	   R NAttachedCommandBehavior, Version=2.0.0.0, Culture=neutral, PublicKeyToken=nullbFclr-namespace:AttachedCommandBehavior;assembly=AttachedCommandBehaviorAttachedCommandBehavior 5   	   !    SensorsViewer.Home.HomeView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ;controls,http://metro.mahapps.com/winfx/xaml/controls  5   	   ,local clr-namespace:SensorsViewer.Home  5   	   XiPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity  5	   	   WcmdMclr-namespace:GalaSoft.MvvmLight.Command;assembly=GalaSoft.MvvmLight.Platform  5
   	   QdialogsCclr-namespace:MahApps.Metro.Controls.Dialogs;assembly=MahApps.Metro  5   	   NcFclr-namespace:AttachedCommandBehavior;assembly=AttachedCommandBehavior  5   	   Z	 VFontAwesome.WPF, Version=4.7.0.37774, Culture=neutral, PublicKeyToken=0758b07a11a4f466-fa$http://schemas.fontawesome.io/icons/ 	 5   	   -   5        $��Window��5   	   $	��650��5      $
��1100��6'      WindowStyle#��[  5   	     � WindowStartupLocation$  CenterScreen=�5   	    � WindowState$ Normal=�5   	   (  "MahApps.Metro.Controls.MetroWindow   ShowIconOnTitleBar . $	��900��5   	   
������� � Title$ HomeView��5   	   8  2MahApps.Metro.Controls.Dialogs.DialogParticipation   Register 5   	   �� .  (System.Windows.Interactivity.Interaction   Triggers 5      /  )System.Windows.Interactivity.EventTrigger  5   
      	EventName$ Closing��6   .  (System.Windows.Interactivity.TriggerBase   Actions. 5      /  )GalaSoft.MvvmLight.Command.EventToCommand     PassEventArgsToCommand . 	   Command	 6!   �� 
 �� Path$
 CloseWindowCommand�6M   5        5    
   $ Loaded��6   . 5!         . 	 6!   �� $
 LoadedWindowCommand�6N   5"         IconTemplate 5%      �� 5&   
   .R�5'      �� $��../Resources/gmLogo.png��6   $
��None=�66   6G   5(       � DataContext 5+      &    SensorsViewer.Home.HomeViewModel  5,   
   6    .��5/      � .+�50   
   �  � ColumnDefinitions 51      �� 52      $	��204 �6#   �� 53      $	��264 �6#   �� 54      $��* �6#   .+�58      �� $��0��6     
CardShadow J� Effect# [ 6%   ���#37509F$
��Auto��6_   $��0,0,0.4,0.2q�6m   ._�59      �  � RowDefinitions 5:      �� 5;      $��70 �6(   �� 5<      $
��Auto �6(   �� 5=      $
��Auto �6(   �� 5>      $��* �6(   .+�5A      �� $��0��6     MenuItem#��[ 6*   $��0,0,0.333,0q�6L   ���#1BB9F0 5B        5C      $ 	MouseDown��6-   . 5D   "      . 	 65   �� $
 CreateProjectCommand�6c   5E      ._�5H      ��  �� Orientation$ 
Horizontal=�6%   $��
-10,0,10,0q�6>   .+�5K      ! 	 FontAwesome.WPF.FontAwesome     Icon$ 
PlusCircle=�6-   $��30(�6?    �� Padding$ 0,0,5,0q�6N   $��0,8,0,0q�6`   
�������6q   �� 5L        H4#��[ 6)   
�������$
��Auto��6Y   $��0,10,0,0.333q�6f   �� 6~   $�New Project��6�   6�   5M      5N      � 5P      $��1��6   .+�5Q      �� $��Projects��6$     H3#��[ 65   ���#BEC5DB$
��Left=�6f   $��15,0,0,0q�6�   $ 10q�6�   6�   �� 5R      -   5         ��Name$ lbProjectItems��5R   "   $��0,52,0.333,-52q�6�   ���#37509F��6:   �� *ProjectItems+ �� SelectedItem 6_   �� *SelectedProjectItem+ �� Mode$ TwoWay=���5S      �� 5T   "    �� 
TargetType" ��6(   .�5U   &   ��  �� Property � �� �� Value$ 0,0,0,0q�6@   �� 5V   &    � �� 5W   *   �� 5X   .    �� 
TargetType" ��6>   .R�5Y   2   ��  ��Name$ Bd��69   ���#EBEBEB$��0q�6[    J� Opacity# z 6o   #��z��6�    �� Padding# z��6�    J� SnapsToDevicePixels . ._�5Z   6   �� #��z��6G   # z 6�   #��z��6�   5[   3    �� Triggers 5\   2   P� 5]   6    P� Property �@��
IsSelected  P� Value  . . �5^   :   �� ! �� 
TargetName$! Bd��6W    � �� 5_   >   �� 5`   B   .��5a   F   � " � Color$" #4D64AC��6S   # � Offset$# 0Q�6c   � 5b   F   $" #37509F��6S   $# 1Q�6c   5c   C   5e   ;   5f   7   P� 5g   6    �@J�IsMouseOver  . . �5h   :   �� $! Bd��6A    � �� 5i   >   �� 5j   B   .��5k   F   � $" #4D64AC��6S   $# 0Q�6c   � 5l   F   $" #37509F��6S   $# 1Q�6c   5m   C   5o   ;   5p   7   5r   /   5t   '   5u   #   $ � ContextMenu$ 5x      �� 5y   "   .@�5z   &   m� $��Rename��6/   % m� Command% 6?   �� *ClickInRenameContextMenu+& m� CommandParameter& 5{   ?   �� ' �� RelativeSource' �� ( �� AncestorType"( ��$"
 PlacementTarget.SelectedItem�m� 5~   &   $��Delete��6/   % 6?   �� *ClickInDeleteContextMenu+& 5   ?   �� ' �� "( ��$"
 PlacementTarget.SelectedItem�5�   #   ��5�      �� 5�   "   .R�5�   &   ��   	MenuItem2#��[ 6.   
������ $��0q�6k    5�   *     5�   .   $  PreviewMouseLeftButtonDown��6=   . 5�   2      . 	 6E   �� ' �� *FindAncestor+"( �$&
  DataContext.SelectProjectCommand�6�   5�   /     5�   .   $! PreviewMouseRightButtonDown��6=   . 5�   2      . 	 6E   �� ' �� *FindAncestor+"( �$&
  DataContext.SelectProjectCommand�6�   5�   /   ._�5�   *   �� ) ��Name$) tbProjectItem��64   $��2��6K   $��1��6X     H5#��[ 6h   $
��Left=�6�   $��0,0,0,0q�6�   ��6�   �� *Name+* �� UpdateSourceTrigger$* PropertyChanged=�6�   5�   '   5�   #   5�      5�      5�      5�      �� 5�      $��1��6   # [ 6%   
�������$
��Auto��6]   $��2 1 0 0q�6k   ._�5�      � .+�5�      �� -   5        + ��Name$+ TabCategoryTabItems��5�   "   , �� SelectedIndex$, 0��6k   ��6?   �� *SelectedTabCategory+ 6}   �� *SelectedTab+$* PropertyChanged=���5�      �� 5�      .R�5�   "   �� $) tbTabCategory��6,   $	��110��6C   $��16(�6R   $��Center=�6a   - �� TextAlignment$- Center=�6~   ��6�   �� *Name+ 5�   &     5�   *   $ 	MouseDown��69   . 5�   .      . 	 6A   �� ' �� *FindAncestor+"( �$+
 %DataContext.ClickInTabCategoryCommand�6�   5�   +   5�   #   5�      . �� ContentTemplate. 5�      �� 5�      .R�5�   "   �� $��10(�6-   , 6;   �� ' �� *FindAncestor+"( �$
 DataContext.SubTabIndex� 5�   &     5�   *   $ SelectionChanged��69   . 5�   .      . 	 6A   �� ' �� *FindAncestor+"( �$&
  DataContext.ClickInSubTabCommand�6�   5�   +   .@�5�   &   �� / ��Name$/ tabItemSensor��6.   $��Sensors��6E   $��5(�6V   $��25, 0, -25 , 0q�6c     MainTabItem#��[ 6{   .��5�   *   �  5�   .   �� 5�   2   $��30 �6@   6N   �� 5�   2   $��* �6@   6M   �� 5�   2   $��30 �6@   6N   .+�5�   .   �� 0 ��Name$0 tbUploadSensorFile��66   *	  $MahApps.Metro.Controls.TextBoxHelper1 	  	Watermark$1 Upload sensor file��5�   6     ButtonCommandMetroTextBox#���  5�   6   2 �� 
IsReadOnly2 . ��5�   6   �� ' �� *FindAncestor+"( �$T
 NDataContext.SelectedProjectChartContent.OpticalSensorViewModel.SensorsFilePath�3 	  ButtonCommand3 5�   6   �� ' �� *FindAncestor+"( �$#
 DataContext.BrowseFileCommand�{�5�   6   �� ' �� *FindAncestor+"( �$+
 %DataContext.SelectedTab.EnableSensors�$ OneWay=�.��5�   7   4 	  ButtonContent4 5�   2   �� 5�   6   $'��!../Resources/open-in-browser2.png��6<   .��5�   X   5�   /   �� 5�   .   $ SensorListBox��6C   $��1��66   $��Hidden=�5�   6   ��5�   6   �� ' �� *FindAncestor+"( �$%
 DataContext.SelectedTab.Sensors�$* PropertyChanged=� 5�   6   �� ' �� *FindAncestor+"( �$ 
 DataContext.SelectedSensor�$ TwoWay=�{�5�   6   �� ' �� *FindAncestor+"( �$+
 %DataContext.SelectedTab.EnableSensors�$ OneWay=���5�   2   �� 5�   6   " ��6<   .�5�   :   ��  � ��
 ���� 6W   �� 5�   :    � �� 6g   �� *HorizontalContentAlignment+' �� "( ���� 5�   :    � �� 6e   �� *VerticalContentAlignment+' �� "( ���� 5�   :    � ��$ 0,0,0,0q�6T   �� 5�   :    � �� 5�   >   �� 5�   B   " ��6R   .R�5�   F   �� $ Bd��6M   #��z��6Y   #��z��6�   #��z��6�   # z��6�    . ._�5�   J   �� #��z��6[   # z 6�   #��z��6�   5�   G    5�   F   P� 5�   J    �@��
IsSelected  . . �5�   N   �� $! Bd��6k    � ��
 ���� 6{   5�   K   5�   C   5�   ;   5 �� Triggers5 5�   :   �� 5�   >   6 �� Binding6 6J   �� *
Visibility+$* PropertyChanged=�7 �� Value	7 false6�   .a�5�   B   ��  � y�$ 	Collapsed=�6_   5�   ?   5�   7   ��5�   2   �� 5�   6   .R�5�   :   �� $��0 0 0 2q�6A   ���#EBEBEB._�5�   >   �  5�   B   �� 5�   F   $��30 �6T   6b   �� 5�   F   $��30 �6T   6b   .+�5�   B   �� $0 tbSensorData��6J   $��0��5�   J   8 	  SelectAllOnFocus8 . $1 Sensor Name��6�   $	��100��5�   s   $��
-135,0,0,0q�6    	 sensorDataText#��[	 6�   ��6J   �� *
SensorName+$ TwoWay=�.��6�    5�   F     5�   J   $ 	LostFocus��6Y   . 5�   N      . 	 6a   �� ' �� *FindAncestor+"( �$'
 !DataContext.EditSensorDataCommand�6�   5�   K   .��5�   ^   5�   C   �� 5�   B   $��1��6M   $ 
Horizontal=�6Z   .+�5�   F   �� 9 ��Name$	9 lbX��6L   $��X:��6Y   $��
0,0,0,-0.2q�6f   $��12(�6z   6�   �� 5�   F   $	0 tbX��6N   8 . $1 X��6�   #��[	 5�   n   $��-5,1,0,0q�6�   ��6N   �� *X+$ TwoWay=�.��6�    5�   J     5�   N   $ 	LostFocus��6]   . 5   R      . 	 6e   �� ' �� *FindAncestor+"( �$'
 !DataContext.EditSensorDataCommand�6�   5  O   .��5  b   5  G   �� 5  F   $	9 lbY��6L   $��Y:��6Y   $��
0,0,0,-0.2q�6f   $��12(�6z   6�   �� 5  F   $	0 tbY��6N   8 . $1 Y��6�   #��[	 5  n   $��-5,1,0,0q�6�   ��6N   �� *Y+$ TwoWay=�.��6�    5  J     5  N   $ 	LostFocus��6]   . 5	  R      . 	 6e   �� ' �� *FindAncestor+"( �$'
 !DataContext.EditSensorDataCommand�6�   5
  O   .��5  b   5  G   �� 5  F   $	9 lbZ��6L   $��Z:��6Y   $��
0,0,0,-0.2q�6f   $��12(�6z   6�   �� 5  F   $	0 tbZ��6N   8 . $1 Z��6�   #��[	 5  n   $��-5,1,0,0q�6�   ��6N   �� *Z+$ TwoWay=�.��6�    5  J     5  N   $ 	LostFocus��6]   . 5  R      . 	 6e   �� ' �� *FindAncestor+"( �$'
 !DataContext.EditSensorDataCommand�6�   5  O   .��5  b   5  G   �� 5  F   $9 lbSensorSize��6L   $��Size:��6b   $��
0,0,0,-0.2q�6r   $��12(�6�   6�   �� 5  F   $0 tbSensorSize��6N   8 . $1 10��6�   #��[	 5  q   $��-5,1,0,0q�6�   ��6N   �� *Size+$ TwoWay=�.��6�    5  J     5  N   $ 	LostFocus��6]   . 5  R      . 	 6e   �� ' �� *FindAncestor+"( �$'
 !DataContext.EditSensorDataCommand�6�   5  O   .��5  b   5  G   5   C   �� 5"  B   : ��Name$: btnDelSensor��6I   $��0��5%  I   $��0q�6V   $��0,0,0,0q�6j   $��
210,6,23,8q�6|    
 DeleteButton#��[
 6�   ��5#  I   �� ' �� *FindAncestor+"( �$%
 DataContext.DeleteSensorCommand���5$  I   �� .��5&  F   �� $$��../Resources/deleteSensor3.png��6L   $
��None=�6t   5'  C   5(  ?   5)  ;   5*  7   5,  /   �� 5.  .   $: btnAddSensor��65   $��2��5/  5   $��0,0,0,0q�6C   $��0q�6U   $	��124��6i     AddSensorButton#��[ 6u   ��50  5   �� ' �� *FindAncestor+"( �$%
 DataContext.AddNewSensorCommand���51  5   �� {�52  5   �� ' �� *FindAncestor+"( �$+
 %DataContext.SelectedTab.EnableSensors�.��54  2   �� $#��../Resources/AddSensorBtn.png��68   $
��None=�6_   55  /   56  +   57  '   �� 59  &   $/ tabItemAnalysis��6.   $��Analysis��6G   $��
80,0,-80,0q�6Y   #��[ 6m   .��5:  *   �� $ 
lbAnalysis��62   ��6F   �� *
Analysis+ 5;  2   �� ' �� *FindAncestor+"( �$"
 DataContext.SelectedAnalysis�
; � Tag; 5<  2   �� *DataContext+< �� ElementName$< Window��$ 5>  .   �� 5?  2   .@�5@  6   m� $��Export to TXT��6?   % 6V   �� */-PlacementTarget.Tag.ClickInExportToTxtCommand+' �� $( ContextMenuL�& 5A  ?   �� ' �� "( ��$"
 PlacementTarget.SelectedItem�5C  3   ��5F  .   �� 5G  2   .R�5H  6   �� $��0 0 0 2q�6=   ���#EBEBEB 5I  :     5J  >   $ 	MouseDown��6M   . 5K  B      . 	 6U   �� ' �� *FindAncestor+"( �$%
 DataContext.ClickInAnalysisItem�6�   5L  ?   ._�5N  :   �  5O  >   �� 5P  B   $��30 �6P   6^   �� 5Q  B   $��35 �6P   6^   .+�5T  >   �� $��0��6H   $��14(�6k   $��5,8,0,0q�6y   ��6U   �� *Name+6�   �� 5V  >   $��1��6I   $ Vertical=�6V   .+�5W  B   �� $��5, 0 , 0 , 0q�6b     AnalysisData#��[ 6x   ��6L   �� *Date+6�   �� 5X  B   $��5, 0 , 0 , 0q�6c   #��[ 6y   ��6L   �� *Time+6�   5Y  ?   �� 5[  >   $: btnDelAnalysis��6F   $��0��5^  F   $��0q�6S   $��0,0,0,0q�6g   $��212,6,17.6,8.4q�6y   #��[
 6�   ��5\  F   �� ' �� *FindAncestor+"( �$'
 !DataContext.DeleteAnalysisCommand���5]  F   �� .��5_  B   �� $$��../Resources/deleteSensor3.png��6H   $
��None=�6p   5`  ?   5a  ;   5b  7   5c  3   5e  +   5f  '   5g  #   5i     5k     5l     5m     �� 5p     $��2��6   $��2 1 0 0q�6%   ���#E6E6E6._�5r     �� 
�������.@�5t     �� $��Result��6     ContentTabItem#��[ 6.   $��30,8,-30,-8q�6V   $��0,0,0,0q�6k   .��5v     �  5w     �� 5x  "   $
��Auto �60   6@   �� 5y  "   $
��Auto �60   6@   �� 5z  "   $��* �60   6=   .+�5}     �� $��2��6%   $��18q�62   
�������= �� CornerRadius$= 3��6Q   # [ 6c   $ 8q�6�   $��1q�6�   ���#EBEBEB._�5~  "   �� ��61   �� *SelectedProjectResultContent+5     5�     5�     �� 5�     $��Chart��6   #��[ 6-   $��60,8,-60,-8q�6U   $��0,0,0,0q�6j   .��5�     �  5�     �� 5�  "   $
��Auto �60   6@   �� 5�  "   $
��Auto �60   6@   �� 5�  "   $��* �60   6=   .+�5�     �� $��2��6%   $��18q�62   
�������$= 3��6Q   # [ 6c   $ 8q�6�   $��1q�6�   ���#EBEBEB._�5�  "   �� ��61   �� *SelectedProjectChartContent+$* PropertyChanged=�5�     5�     5�     �� 5�     $��Sensors Log��6   #��[ 63   $��90,8,-90,-8q�6\   $��0,0,0,0q�6q   .��5�     � .+�5�     �� $��2��6%   $��25q�62   
�������$= 3��6Q   # [ 6c   $ 8q�6�   $��1q�6�   ���#EBEBEB._�5�  "   �� $��1q�6+   $��1q�66   $, 0��5�  +     VirtualisedMetroListView#��[ 6=   ��5�  J   �� *?=SelectedProjectChartContent.OpticalSensorViewModel.SensorsLog+> �� View> 5�  &   �� 5�  *   .P�5�  .   �� $��	Sensor Id��6i   ? �� DisplayMemberBinding? 6=   �� *
SensorName+6i   �� 5�  .   $��	Timestamp��6r   ? 6=   �� *Values[0].Timestamp+6r   �� 5�  .   $��Value��6n   ? 6=   �� *Values[0].Value+6n   �� 5�  .   $��	Parameter��6r   ? 6=   �� *Values[0].Parameter+6r   5�  +   5�  #   5�     5�     5�     �� 5�     $��Sensors Analysis��6   #��[ 68   $��120,8,-120,-8q�6a   $��0,0,0,0q�6x   .��5�     � .+�5�     �� $��2��6%   $��25q�62   
�������$= 3��6Q   # [ 6c   $ 8q�6�   $��1q�6�   ���#EBEBEB._�5�  "   �� $��1q�6+   $��1q�66   $, 0��5�  +   #��[ 6=   ��5�  J   �� *?=SelectedProjectChartContent.OpticalSensorViewModel.SensorList+> 5�  &   �� 5�  *   .P�5�  .   �� $��	Sensor Id��6i   ? 6=   �� *
SensorName+6i   �� 5�  .   $��	Max Value��6b   ? 6=   �� *Max+6b   �� 5�  .   $��	Min Value��6b   ? 6=   �� *Min+6b   �� 5�  .   $��Integral��6g   ? 6=   �� *
Integral+6g   5�  +   5�  #   5�     �� 5�     $��- No data received��6$   $
��Left=�6A   $��
34,563,0,0q�6\   $	��Top=�6p   5�     5�     5�     5�     5�     5�     5�     