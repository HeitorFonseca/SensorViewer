   M S B A M L   `   `   `  ����H  DSensorsViewer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:SensorsViewer.ResultSensorsViewer.Result  5      c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity 5	      f bGalaSoft.MvvmLight.Platform, Version=5.3.0.19032, Culture=neutral, PublicKeyToken=5f873c45e98af8a1lMclr-namespace:GalaSoft.MvvmLight.Command;assembly=GalaSoft.MvvmLight.PlatformGalaSoft.MvvmLight.Command 5
      R NSharpGL.WPF, Version=2.4.0.0, Culture=neutral, PublicKeyToken=27fc851303210b27>.clr-namespace:SharpGL.WPF;assembly=SharpGL.WPFSharpGL.WPF 5      %    SensorsViewer.Result.ResultView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      4local"clr-namespace:SensorsViewer.Result     5      W SHelixToolkit.Wpf, Version=1.1.0.0, Culture=neutral, PublicKeyToken=52aa3500039caf0d(helixhttp://helix-toolkit.org/wpf  5      U	 QMahApps.Metro, Version=1.5.0.23, Culture=neutral, PublicKeyToken=f4fb5a3c4d1e5b4f;controls,http://metro.mahapps.com/winfx/xaml/controls 	 5      XiPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity 	 5	      WcmdMclr-namespace:GalaSoft.MvvmLight.Command;assembly=GalaSoft.MvvmLight.Platform 	 5
      <sharpGL.clr-namespace:SharpGL.WPF;assembly=SharpGL.WPF 	 5      .��5      � .+�5   
   &   HelixToolkit.Wpf.HelixViewport3D  -   5           Name$  
viewPort3d��5       y�64   �� *SensorsVisibility+   Camera 5      =� 5       � LookDirection$ 0,0,-15�6$   5         Children 5      $  HelixToolkit.Wpf.DefaultLights  5      g� 5       g� Content 6    �� *
GroupModel+6B   5        SharpGL.WPF.OpenGLControl  5   
   -   5        y�5   T   �� *InterpVisibility+.  (System.Windows.Interactivity.Interaction   Triggers 5       /  )System.Windows.Interactivity.EventTrigger  5!         	EventName$ OpenGLInitialized��6!   .  (System.Windows.Interactivity.TriggerBase   Actions. 5"      /  )GalaSoft.MvvmLight.Command.EventToCommand     PassEventArgsToCommand . 	   Command	 6)   �� *OpenGLInitializedCommand+6V   5#        5$      $ 
OpenGLDraw��6!   . 5%         . 	 6)   �� *
OpenGLDraw+6H   5&        5'      $ Resized��6!   . 5(         . 	 6)   �� *OpenGLResized+6K   5)      5,      ) 	 #MahApps.Metro.Controls.ToggleSwitch  5.   
   -   5        
  Name$
 enabledSwitch��5.       $��201,0,0,275q�67      OffLabel$ Sensors��6M      OnLabel$
 Ramp��6`    +  'MahApps.Metro.Styles.ToggleSwitch.Win10#��[  50       $	��Top=�6a   $��Right=�6y      	IsChecked 5/       �� *
ViewMode+ 52        53      $ Checked��6!   . 54         . 	 6)   �� * OnCheckedModeViewButtonCommand+6\   55        56      $ 	Unchecked��6!   . 57         . 	 6)   �� *" OnUnCheckedModeViewButtonCommand+6^   58      5:      �� 5<   
   $��Model Dimensions��6   $
��Left=�6+   $	��Top=�6F   $��	20,-3,0,0q�6^   �� 5>   
    �� Orientation$ 
Horizontal=�6   .+�5?      �� $��X:��6   $
��Left=�6!   $	��Top=�6<   $��0,17,0,0q�6T   �� 5@      $��
0,18,0,260q�63   $��44��6G   ��6   �� *
ModelXSize+6G   �� 5A      $��Y:��6   $
��Left=�6!   $	��Top=�6<   $��0,17,0,0q�6T   �� 5B      $��
0,18,0,260q�63   $��44��6G   ��6   �� *
ModelYSize+6G   �� 5C      $��Z:��6   $
��Left=�6!   $	��Top=�6<   $��0,17,0,0q�6T   �� 5D      $��
0,17,0,261q�63   $��44��6G   ��6   �� *
ModelZSize+6G   5E      �� 5G   
    �� AutoToolTipPlacement$ BottomRight=�6    � Minimum$ 0Q�5I      $��Bottom=�5K       � Maximum 5H      �� *	MaxSlider+ � Value 5J      �� *Slider+ �� Mode$ TwoWay=� 5M        5N      $ ValueChanged��6!   . 5O         . 	 6)   �� *OnSliderValueChanged+6R   5P      5S      5U      5V      