   M S B A M L   `   `   `  ����N  JLightBuzz.Vitruvius, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullhGclr-namespace:LightBuzz.Vitruvius.Controls;assembly=LightBuzz.VitruviusLightBuzz.Vitruvius.Controls  5   	   T PCefSharp.Wpf, Version=51.0.0.0, Culture=neutral, PublicKeyToken=40c4b6fc221f4138A0clr-namespace:CefSharp.Wpf;assembly=CefSharp.WpfCefSharp.Wpf 5   	    OpenSenseMap   OpenSenseMap.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   2awe"http://schemas.awesomium.com/winfx     5   	   d `Microsoft.Kinect.Wpf.Controls, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e350k(http://schemas.microsoft.com/kinect/2014  5   	   J FWpfAnimatedGif, Version=1.4.14.0, Culture=neutral, PublicKeyToken=null,gif"http://wpfanimatedgif.codeplex.com  5   	   VcontrolsGclr-namespace:LightBuzz.Vitruvius.Controls;assembly=LightBuzz.Vitruvius  5   	   :wpf0clr-namespace:CefSharp.Wpf;assembly=CefSharp.Wpf  5   	     � Title$  
MainWindow��5   	   $	��350��6   $	��525��6)    � WindowStyle$
 None=�5	   	    � 
ResizeMode$ NoResize=�6    � WindowStartupLocation$ CenterScreen=�5   	    � WindowState$ 	Maximized=�6.   ��5
   	   Z� .��5      �  � RowDefinitions 5   
   �� 5      $��3* �6   �� 5      $��1* �6    � ColumnDefinitions 5   
   �� 5      $��1* �6   �� 5      $��2* �6   �� 5      $��1* �6   .+�5   
   %  CefSharp.Wpf.ChromiumWebBrowser  -   5          Name$ 
webControl��5   !   $��0��65   $��2��6B   $��0��6S   $��3��6c      Address$ http://139.59.128.198/��5      /   )LightBuzz.Vitruvius.Controls.KinectViewer  5   
   -   5        	  Name$	 viewer��5   !   $��2��61   $��0��6>   �� 5   
   -   5        
 ��Name$
 zoomAnimation��5      $��1��6'   $��2��65   "  WpfAnimatedGif.ImageBehavior   AnimatedSource$ Animations/Zoom_02.gif��6E   � 5"   
   $��0��6   $��1��6   .+�5#      �� -   5        $
 light_image��5#      $y�Hidden=�6)   6?   5$      � 5'   
   $��1��6   $��1��6    5(      �� 5)      $��1* �6#   60   �� 5*      $��1* �6#   60   �� 5+      $��1* �6#   60    5-      �� 5.      $��2* �6    6.   �� 5/      $��1* �6    6.   .+�51      �� -   5        $
 swipeAnimation��51      $y�Hidden=�6,   $��0��6@   $��1��6M   $" Animations/ChangePage_01.gif��6]   52      55      56      