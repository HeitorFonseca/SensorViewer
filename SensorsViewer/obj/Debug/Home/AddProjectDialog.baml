   M S B A M L   `   `   `  ����H  DSensorsViewer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:SensorsViewer.HomeSensorsViewer.Home  5      )    #SensorsViewer.Home.AddProjectDialog   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      U QMahApps.Metro, Version=1.5.0.23, Culture=neutral, PublicKeyToken=f4fb5a3c4d1e5b4f;controls,http://metro.mahapps.com/winfx/xaml/controls  5      ,local clr-namespace:SensorsViewer.Home  5      $��1q�5	      ���	#CC119EDA$	��150��5      $	��400��6   .��5      �   � ColumnDefinitions  5   
   �� 5      $
��Auto �6    z� SharedSizeGroup$ LabelCol��6,   �� 5      $��* �6   �� 5      $��30 �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��0��6!   $��Project Name:��61   $��Center=�6F   $��Right=�6a   $��10,0,0,0q�6}   �� 5   
   -   5         ��Name$ FirstNameTextBox��5      $��0��6,   $��1��69   $��8 8 0 0q�6I   ��5      �� *ProjectName+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�.��6a    J� InputBindings 5      �� 5      
 �� Key$ Return��6E    �� Command 6   �� *EnterKeyDownCommand+6E   .��5   %   5       �� 5"   
   $��1��6   $��0��6!   $��0 8 0 0q�61   $��Model:��6B   $��Center=�6P   $��Right=�6k   �� 5$   
   $��1��6   $��1��6   $��8 8 0 0q�6/   ��6@   �� *	ModelPath+$ TwoWay=�$ PropertyChanged=��� 5&   
   -   5        $��1��5&      $��2��6   $��5��6.   $��0 8 0 0q�69   $��Center=�6K   $��Center=�6f   $	��...��6�   �� 5(   
   $��2��6   $��0��6"   $��2��62   	 �� Orientation$	 
Horizontal=�6G   $��Right=�6`   .+�5)      �� -   5        $��80��5)      $��8 8 0 8q�6$   $��Right=�65   $��OK��6i   �� 5*      -   5        $��80��5*      $��8 8 0 8q�6$   $��Right=�65   $��Cancel��6h   5+      5-      5.      