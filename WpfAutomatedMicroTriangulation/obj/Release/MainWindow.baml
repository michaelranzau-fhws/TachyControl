   M S B A M L   `   `   `  ���� V  RAutomatedMicroTriangulation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:AutomatedMicroTriangulationAutomatedMicroTriangulation  G CTachyControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;-clr-namespace:TSControl;assembly=TachyControl	TSControl O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem M ISystem, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089A-clr-namespace:System.IO.Ports;assembly=SystemSystem.IO.Ports ,    &AutomatedMicroTriangulation.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ;local)clr-namespace:AutomatedMicroTriangulation     >link-clr-namespace:TSControl;assembly=TachyControl     9System&clr-namespace:System;assembly=mscorlib     <io-clr-namespace:System.IO.Ports;assembly=System     -     �Name$  winMain�� � Title$# Automated Micro Triangulation��$��684.6��$
��1048�� � 
ResizeMode$ NoResize=� � Icon$ TitleIcon.ico�� � WindowStartupLocation$ CenterScreen=� � 	Resources    WindowColor  ContentAreaColorLight  ContentAreaColorDark  DisabledControlLightColor  DisabledControlDarkColor  DisabledForegroundColor  SelectedBackgroundColor  SelectedUnfocusedColor  ControlLightColor 	 ControlMediumColor 
 ControlDarkColor  ControlMouseOverColor  ControlPressedColor  
GlyphColor  GlyphMouseOver  BorderLightColor  BorderMediumColor  BorderDarkColor  PressedBorderLightColor  PressedBorderDarkColor  DisabledBorderLightColor  DisabledBorderDarkColor  DefaultBorderBrushDarkColor  HeaderTopColor " DatagridCurrentCellBorderColor  SliderTrackDarkColor  NavButtonFrameColor  MenuPopupBrush �� EndPoint �� 
StartPoint � Color	 � Offset $  ProgressBarIndicatorAnimatedFill  ButtonFocusVisual
 �� Property �� Value �� StrokeDashArray �� 
TargetType �� 
TargetType ��Name �� CornerRadius'  !System.Windows.VisualStateManager   VisualStateGroups%  System.Windows.VisualStateGroup   Transitions%  System.Windows.VisualTransition   GeneratedDuration	   To   States   System.Windows.VisualState   
Storyboard �� TargetProperty �� 
TargetName8  2System.Windows.Media.Animation.EasingColorKeyFrame �� KeyTime �� Value �� Triggers P� Property P� Value �� 
TargetName   DefaultBorderBrushLightBrush  Button_TreeViewItem   Button_TreeViewItem_green ! Button_TreeViewItem_orange " Button_TreeViewItem_gray # BTN_Add �� Padding $ RedBrush  �� Color % 
WhiteBrush & SolidBorderBrush ' 
GreenBrush ( DisabledBackgroundBrush ) DisabledBorderBrush * DisabledForegroundBrush! ��Name + TreeViewItemFocusVisual ., *TreeViewItem.TreeArrow.Static.Checked.Fill 0- ,TreeViewItem.TreeArrow.Static.Checked.Stroke +. 'TreeViewItem.TreeArrow.MouseOver.Stroke )/ %TreeViewItem.TreeArrow.MouseOver.Fill 30 /TreeViewItem.TreeArrow.MouseOver.Checked.Stroke 11 -TreeViewItem.TreeArrow.MouseOver.Checked.Fill 2 	TreeArrow &3 "TreeViewItem.TreeArrow.Static.Fill (4 $TreeViewItem.TreeArrow.Static.Stroke 5 ExpandCollapseToggleStyle" �� Padding# F�Name$ �� Angle% �� CenterY& �� CenterX' ]� 
Conditions( �� Property) �� Value* �� RelativeSource+ �� AncestorType, J� SnapsToDevicePixels- � ColumnDefinitions. � RowDefinitions/ d�Name0 �� 	ClickMode1 d� 	IsChecked2 ��Name3 �� Triggers 6 TVI_Add&   System.Windows.Controls.DataGrid)  #System.Windows.Controls.DataGridRow4 �� 	Resources*  $System.Windows.Controls.DataGridCell 7 dataFromStations5 S� 
ObjectType3	   -AutomatedMicroTriangulation.ProvidedFunctions6 S� 
MethodName 8 numberOftargetsNotNull 9 dataFromCOMs
  TSControl.TSControl : dataFromParityEnum  System.Enum7 S� MethodParameters8 M� TypeName ; dataFromStopBitsEnum < SetTSSetCOMTemplate9 ��Name: �� 
IsReadOnly; �� SelectedValuePath< �� SelectedValue= �� Mode> �� UpdateSourceTrigger? �� Source<   6AutomatedMicroTriangulation.EnumBindingSourceExtension@ �� DisplayMemberPathA �� Orientation��%OF  &	        &	      &	 &     &	 9     &	 L     &	 _     &	 r     &	 �     &	 �     &		 �     &	
 �     &	 �     &	 �     &	 �     &	     &	 ;    &	 N    &	 a    &	 t    &	 �    &	 �    &	 �    &	 �    &	 �    &	 �    &	 �    &	     &	     &	 u    &	     '�� h    7  7  7  7  7  7  7  7  &	 #
    7  7  7  7  7  &	  �    7  7  7  7  &	! �    7  7  7  7  7  &	" �    7  7  7  7  7  &	# -#    7  7  7  7  7  7  7  7  &	$ ]*    &	% l*    &	& }*    &	' �*    &	( �*    &	) �*    &	* �*    '�� �*    7 ( 7 & 7 % 7 ( 7 ) 7 * &	+ q,    &	, �,    &	- �,    &	. �,    &	/ �,    &	0 �,    &	1 -    &	2 -    &	3 3-    &	4 J-    &	5 _-    7 2 7 3 7 4 7 , 7 - 7 . 7 / 7 0 7 1 'Q� �/    7 + 7 5 &	6 4    7 + 7 5 '  �9    '  :    '  �:    &	7 &;    &	8 B;    &	9 l;    &	: �;    &	; �;    &	< �;    7 9 7 7 7 : 7 ; 7  7   ��	#FFE8EDF9����	#FFC5CBF9����	#FF7381F9����	#FFE8EDF9����	#FFC5C5C5����	#FF888888����	#FFC5CBF9����	#FFDDDDDD����	White����	#FFCCCCCC����	#FF211AA9����	#FFAAAAAA����	#FF888888����	#FF444444����/+sc#1, 0.004391443, 0.002428215, 0.242281124����	#FFCCCCCC����	#FFAAAAAA����	#FF888888����	#FFAAAAAA����	#FF888888����	#FFAAAAAA����	#FF888888����	Black����	#FFC5CBF9����	Black����	#FFC5CBF9����	#FF3843C4���� $ 0.5,1&�$ 0.5,0&�.��� # �  $	 0Q�� # � 	 $		 0.5Q�� # �  $	 1Q��� $	 0,0&�$	 1,0&���� � $ 	#000000FF��$	 0Q�� $ 	#600000FF��$		 0.4Q�� $ 	#600000FF��$		 0.6Q�� $ 	#000000FF��$	 1Q��� .��� 
 � �� �� .R��� ._��� $��2q�$��1�����	#60000000$	 1 2R��� " ��.��� 
 �@J�SnapsToDevicePixels . �� 
 �@�OverridesDefaultStyle . �� 
 �@�FocusVisualStyle8   �� 
 � ��$ 23���� 
 � ��$ 75���� 
 � �� �� " ��.R��� $ Border��#��z��$ 2��$��1q����� $	 0,0&�$	 0,1&���� � # �  $		 0.0Q�� # �  $		 1.0Q����� $ 0.5,1&�$ 0.5,0&�.��� # �  $	 0Q�� # � 	 $	 1Q�   CommonStates��   $ 0:0:0.5D�  $ 0D�$ Pressed��.   
Normal��  	MouseOver��. �� .���� $e _(Panel.Background).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8    Pressed��. �� .���� $e _(Panel.Background).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8  �� $g a(Border.BorderBrush).                       (GradientBrush.GradientStops)[0].(GradientStop.Color)�$ Border��.g�  $ 0��8  �� $g a(Border.BorderBrush).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8    Disabled��. �� .���� $e _(Panel.Background).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8  �� $4 .(TextBlock.Foreground).(SolidColorBrush.Color)�$ Border��.g�  $ 0��8  �� $g a(Border.BorderBrush).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8  ._��� $��2q�$��Center=�$��Center=���.  P�  �@��	IsDefault . . ��� $ Border��
 � �� �� $	 0,0&�$	 0,1&���� � # �  $		 0.0Q�� # �  $		 1.0Q��� " ��.��� 
 �@J�SnapsToDevicePixels . �� 
 �@�OverridesDefaultStyle . �� 
 �@�FocusVisualStyle8   �� 
 � ��$ 20���� 
 � ��$ 25���� 
 � ��$ 10(��� 
 � ��$ Normal"��� 
 � ��$ 1q��� 
 � �� �� " ��.R��� $ Border��#��z��$ 2��$��1q����� $	 0,0&�$	 0,1&���� � # �  $		 0.0Q�� # �  $		 1.0Q����� $ 0.5,1&�$ 0.5,0&�.��� $ 	#FFFF7B7B��$	 0Q�� $ 	#FFFFC2C2��$	 1Q�   CommonStates��   $ 0:0:0.5D�  $ 0D�$ Pressed��.   
Normal��  	MouseOver��. �� .���� $e _(Panel.Background).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��$ 	#FFFF7B7B��  Pressed��. �� .���� $e _(Panel.Background).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��$ 	#FFFF5555���� $g a(Border.BorderBrush).                       (GradientBrush.GradientStops)[0].(GradientStop.Color)�$ Border��.g�  $ 0��8  �� $g a(Border.BorderBrush).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8    Disabled��. �� .���� $4 .(TextBlock.Foreground).(SolidColorBrush.Color)�$ Border��.g�  $ 0��8  �� $� �(Border.BorderBrush).                                                                           (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8  ._��� $��2q�$��Center=�$��Center=���.  P�  �@��	IsDefault . . ��� $ Border��
 � �� �� $	 0,0&�$	 0,1&���� � # �  $		 0.0Q�� # �  $		 1.0Q��� " ��.��� 
 � �� �� " ��.R��� $ Border��#��z��$ 2��$��1q����� $	 0,0&�$	 0,1&���� � # �  $		 0.0Q�� # �  $		 1.0Q����� $ 0.5,1&�$ 0.5,0&�.��� $ 	#FF7ED158��$	 0Q�� $ 	#FFC2FFC2��$	 1Q�   CommonStates��   $ 0:0:0.5D�  $ 0D�$ Pressed��.   
Normal��  	MouseOver��. �� .���� $e _(Panel.Background).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��$ 	#FF7BFF7B��  Pressed��. �� .���� $e _(Panel.Background).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��$ 	#FF55FF55���� $g a(Border.BorderBrush).                       (GradientBrush.GradientStops)[0].(GradientStop.Color)�$ Border��.g�  $ 0��8   �� $g a(Border.BorderBrush).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8    Disabled��. �� .���� $4 .(TextBlock.Foreground).(SolidColorBrush.Color)�$ Border��.g�  $ 0��8  �� $g a(Border.BorderBrush).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8  ._��� $��2q�$��Center=�$��Center=���.  P�  �@��	IsDefault . . ��� $ Border��
 � �� �� $	 0,0&�$	 0,1&���� � # �  $		 0.0Q�� # �  $		 1.0Q��� " ��.��� 
 � �� �� " ��.R��� $ Border��#��z��$ 2��$��1q����� $	 0,0&�$	 0,1&���� � # �  $		 0.0Q�� # �  $		 1.0Q����� $ 0.5,1&�$ 0.5,0&�.��� $ 	#FFFFC963��$	 0Q�� $ 	#FFFFF0D4��$	 1Q�   CommonStates��   $ 0:0:0.5D�  $ 0D�$ Pressed��.   
Normal��  	MouseOver��. �� .���� $e _(Panel.Background).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��$ 	#FFFFC963��  Pressed��. �� .���� $e _(Panel.Background).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��$ 	#FFFFBB39���� $g a(Border.BorderBrush).                       (GradientBrush.GradientStops)[0].(GradientStop.Color)�$ Border��.g�  $ 0��8   �� $g a(Border.BorderBrush).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8    Disabled��. �� .���� $e _(Panel.Background).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8  �� $4 .(TextBlock.Foreground).(SolidColorBrush.Color)�$ Border��.g�  $ 0��8  �� $g a(Border.BorderBrush).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8  ._��� $��2q�$��Center=�$��Center=���.  P�  �@��	IsDefault . . ��� $ Border��
 � �� �� $	 0,0&�$	 0,1&���� � # �  $		 0.0Q�� # �  $		 1.0Q��� " ��.��� 
 � �� �� " ��.R��� $ Border��#��z��$ 2��$��1q����� $	 0,0&�$	 0,1&���� � # �  $		 0.0Q�� # �  $		 1.0Q����� $ 0.5,1&�$ 0.5,0&�.��� $ 	#FFCDCDCD��$	 0Q�� $ 	#FFDDDDDD��$	 1Q�   CommonStates��   $ 0:0:0.5D�  $ 0D�$ Pressed��.   
Normal��  	MouseOver��. �� .���� $e _(Panel.Background).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��$ 	#FFBDBDBD��  Pressed��. �� .���� $e _(Panel.Background).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��$ 	#FFBBBBBB���� $g a(Border.BorderBrush).                       (GradientBrush.GradientStops)[0].(GradientStop.Color)�$ Border��.g�  $ 0��8   �� $g a(Border.BorderBrush).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8    Disabled��. �� .���� $e _(Panel.Background).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8  �� $4 .(TextBlock.Foreground).(SolidColorBrush.Color)�$ Border��.g�  $ 0��8  �� $g a(Border.BorderBrush).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8  ._��� $��2q�$��Center=�$��Center=���.  P�  �@��	IsDefault . . ��� $ Border��
 � �� �� $	 0,0&�$	 0,1&���� � # �  $		 0.0Q�� # �  $		 1.0Q��� " ��.��� 
 �@J�SnapsToDevicePixels . �� 
 �@�OverridesDefaultStyle . �� 
 �@�FocusVisualStyle8   �� 
 � ��$ 17���� 
 � ��$
 Auto���� 
 � ��$ 10(��� 
 � ��$ Normal"��� 
 � ��$ 0q��� 
 � ��$ 2,3,2,2q��� 
 � �� �� " ��.R��� $ Border��#��z��$ 2��$��1q����� $	 0,0&�$	 0,1&���� � # �  $		 0.0Q�� # �  $		 1.0Q����� $ 0.5,1&�$ 0.5,0&�.��� # �  $	 0Q�� # � 	 $	 1Q�   CommonStates��   $ 0:0:0.5D�  $ 0D�$ Pressed��.   
Normal��  	MouseOver��. �� .���� $e _(Panel.Background).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8    Pressed��. �� .���� $e _(Panel.Background).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8  �� $g a(Border.BorderBrush).                       (GradientBrush.GradientStops)[0].(GradientStop.Color)�$ Border��.g�  $ 0��8  �� $g a(Border.BorderBrush).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8    Disabled��. �� .���� $e _(Panel.Background).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8  �� $4 .(TextBlock.Foreground).(SolidColorBrush.Color)�$ Border��.g�  $ 0��8  �� $g a(Border.BorderBrush).                       (GradientBrush.GradientStops)[1].(GradientStop.Color)�$ Border��.g�  $ 0��8  ._��� $��0,0,0,0q�$ 2q��� $��Center=�$��Center=���.  P�  �@��	IsDefault . . ��� $ Border��
 � �� �� $	 0,0&�$	 0,1&���� � # �  $		 0.0Q�� # �  $		 1.0Q��� $	  Red���� $  White���� $  	#FF8C8E94���� $  Green���� $  	#FFF3F3F3���� $  	#FF8C8E94���� $  Black���� " ��.��� 
 � �� �� " ��.R�� .+��� $��Border��8��  8�� $��0.5,0.5,0.5,0q�$ 0,3,0,0��._��� $! ContentSite��$��Center=�$��Center=�$��Header��$��	12,2,12,2q���.  P�  �@��
IsSelected . . ��� 
 �@K�ZIndex$	 100���� $ Border��8  
 � ��P�  � {� .  . ��� $ Border��8  
 � ���� $ Border��8  
 � ���� 8  
 � ���� .��� 
 � �� �� .R��� �� $  	#FF595959���� $  	#FF262626���� $  	#FF1BBBFA���� $  Transparent���� $  	#FF262626���� $  	#FF595959��B� $��M0,0 L0,6 L6,0 zC��� $  Transparent���� $  	#FF989898���� " d�.��� 
 � |� .  �� 
 � ��$ 16���� 
 � ��$ 16���� 
 � �� �� " d�.R��� 
������ $��16��$" 5,5,5,5q�$��16��._�F� $# 
ExpandPath��8��  8�� 8�� z��� $	$ 135Q�$% 3Q�$& 3Q� P�  �@d�	IsChecked$
 TrueY�. ��� $ 
ExpandPath��
 � z� �� $	$ 180Q�$% 3Q�$& 3Q��� $ 
ExpandPath��8  
 � ���� $ 
ExpandPath��8  
 � ��P�  �@J�IsMouseOver . . ��� $ 
ExpandPath��8  
 � ���� $ 
ExpandPath��8  
 � ��]� ' �� ( �@J�IsMouseOver) . �� ( �@d�	IsChecked$
) TrueY�./��� $ 
ExpandPath��8  
 � ���� $ 
ExpandPath��8  
 � ���� " Q�.��� 
 � �� �� *HorizontalContentAlignment+* �� "+ ���� 
 � �� �� *VerticalContentAlignment+* �� "+ ���� 
 � ��$ 0q��� 
 � ��$ 0q��� 
 � ��# � ���� 
 � �� �	#007ED158�� 
 �@�FocusVisualStyle8   �� 
 � �� �� " Q�.R��� #" z��#��z��$ 15��#��z��#��z��$��2q�, . ._�� - �� $��19��$
��Auto ��� $
��Auto ��� $��* �. �� $
��Auto ��� .+�d� $/ Expander��$0 Press=�8�� 1 �� *
IsExpanded+* �� *TemplatedParent+�� $ Bd��$��1��._��� $! PART_Header��$��Header��#��z��#, z, �� $2 	ItemsHost��$��2��$��1��$��1�� P�  �@Q�
IsExpanded .  . ��� $ 	ItemsHost��
 � y�$ 	Collapsed=�P�  �@��HasItems .  . ��� $ Expander��
 � y�$ Hidden=�]� ' �� ( �@Q�
IsSelected) . �� ( �@Q�IsSelectionActive) .  ./��� $ Bd��# � /�
 � ���� # � .�
 � ��P�  � {� .  . ��� # � ��
 � ��3 P�  �@%�IsVirtualizing . . ��� 
 � �� �� .R�$� �� " Q�.��� 
 � �� �� *HorizontalContentAlignment+* �� "+ ���� 
 � �� �� *VerticalContentAlignment+* �� "+ ���� 
 � ��$ 2,0,2,1q��� 
 � ��$ 20,0,0,0q��� 
 � ��$ 24���� 
 � ��$ 80���� 
 � ��$
 Left=��� 
 � ��# � ���� 
 �@�FocusVisualStyle8   �� 
 � �� �� " Q�.R��� #" z�����	#FFDDDDDD$ 15��#��z��#��z��$��2q�, . ._�� - �� $��19��$
��Auto ��� $
��Auto ��� $��* �. �� $
��Auto ��� .+�d� $/ Expander��$0 Press=�8�� 1 �� *
IsExpanded+* �� *TemplatedParent+�� $��1��$��20(�$
��Bold"�$��
0,-6,0,-10q�$��0q�.��+�� $ Bd��$��1��$��	20,-2,0,0q�._��� $! PART_Header��$y�	Collapsed=�$��Header��#��z��#, z, �� $2 	ItemsHost��$��2��$��1��$��1�� P�  �@Q�
IsExpanded .  . ��� $ 	ItemsHost��
 � y�$ 	Collapsed=�P�  �@��HasItems .  . ��� $ Expander��
 � y�$ Hidden=�P�  �@J�IsMouseOver . . ��� 
 � ��$ 20,0,0,0q��� 
 � ��$	 245���� $ PART_Header��
 � y�$ Visible=�P�  �@Q�
IsSelected . . ��� $ Bd��
 � �� �	#FFDDDDDD�� 
 � ��
 �   ��� 
 � ��$
 Bold"�]� ' �� ( �@Q�
IsSelected) . �� ( �@Q�IsSelectionActive) .  ./��� $ Bd��# � /�
 � ���� # � .�
 � ��P�  � {� .  . ��� # � ��
 � ��3 P�  �@%�IsVirtualizing . . ��� 
 � �� �� .R�$� �� "  .��� 
 � ��$ Normal"��� "  4 ���� $  Transparent��(��       *SystemColors.ControlBrushKey+)�� $  Black��(��       *$"SystemColors.HighlightTextBrushKey+)�� $  Black��(��       *" SystemColors.ControlTextBrushKey+)�� "  .��� 
 � ��$ 0q��� 
 �@�FocusVisualStyle Z� S� "5 	 $6 getStations��S� "5 	 $6 getNumberOftargetsNotNull��S� "5 
 $6 getCOMPorts��S� $6 	GetValues��"5  7 M� $8 	io:Parity��S� $6 	GetValues��"5  7 M� $8 io:StopBits���� " ��.��� 
 � �� �� " ��.R��� $	��Top=�$	��200��$	��320��$��5q����	#FFF7F7F7$ 4��$��1q�
���   �._�]� .+��� $9 SetTSSetCOM��$��5,5,5,5q�$	��Top=�: .  $; ID��< �� *COMID+$= TwoWay=�$> 	LostFocus=����� 8?   �� $9 SetTSSetTSType��$��5,0,5,5q�$	��Top=�: .  < �� *TSType+$= TwoWay=�$> 	LostFocus=����� ?   *M� *link:TotalStationType++�� $9 SetTSSetStation��$��5,0,5,5q�$	��Top=�: .  $
@ Name��$; ID��< �� *	StationID+$= TwoWay=�$> 	LostFocus=����� 8?  $> PropertyChanged=��� $��5,0,5,5q�$
��Wrap=�$	��Top=�Total station:�� �� ���� *
DeviceType+�� 
Camera: �� ���� *Camera+�� Serial number: �� ���� *SerialNumber+� $	��Top=�- �� $��80 ��� $��* �.+��� $��0��$��80��.��	BaudRate:�� $��1��$��5q�: .  < �� *	_BaudRate+$= TwoWay=�$> 	LostFocus=����� ?   *M� *link:BaudRate++� $	��Top=�- �� $��80 ��� $��* �.+��� $��0��$��80��.��	DataBits:�� $��1��$��5q�: .  < �� *	_DataBits+$= TwoWay=�$> 	LostFocus=����� ?   *M� *link:DataBits++� $	��Top=�- �� $��80 ��� $��* �.+��� $��0��$��80��.��	Parity:�� $��1��$��5q�: .  < �� *	_Parity+$= TwoWay=�$> 	LostFocus=����� 8?  � $	��Top=�- �� $��80 ��� $��* �.+��� $��0��$��80��.��	StopBits:�� $��1��$��5q�: .  < �� *	_StopBits+$= TwoWay=�$> 	LostFocus=����� 8?  �� $A 
Horizontal=�$��Bottom=�$��Center=�.+��� $��5q�$��50��$��20��$
��Test���� $��5q�$��40��$��20��$��Delete��8�� �� $��5q�$��50��$��20��$��Default��8�� $��10(�� .��� .+�]� ��. $��0,0,-0.4,-0.2q�$��2��.+��� $A 
Horizontal=�$	��Top=�.+��� $&�� Automated Micro Triangulation - ��$��16(�$
��Bold"��� -   B ��Name$B lblPHead��$�� ��$��16(�$
��Bold"��� $��Bottom=�$��
2016, FHWS��$��12(�$
��Bold"�$��25���� -   C ��Name$C tabControl_Main��$��Stretch=�$��Stretch=�
�������.@��� $��Project management��.���� .+��� $A 
Horizontal=�.+��� -   $B lblPMNumberStations��$��Number of stations:��$��10,5,0,0q��� -   $B lblPMNumberStations2��$�� ��$��0,5,10,0q��� $A 
Horizontal=�.+��� -   $B lblPMNumberTargets��$��Number of targets:��$��10,5,0,0q��� -   $B lblPMNumberTargets2��$�� ��$��0,5,10,0q��� $A 
Horizontal=�.+��� -   $B lblPMNumberObs��$#��Total number of observations:��$��10,5,0,0q��� -	   $B lblPMNumberObs2��$�� ��$��0,5,10,0q��� $A 
Horizontal=�.+��� -
   $B lblPMLastObs��$��Date of last observation:��$��10,5,0,0q��� -   $B lblPMLastObs2��$�� ��$��0,5,10,0q��� � $��Stretch=�$��
10,10,10,5q�- �� $��* ��� $��* �.+��� -   D ��Name$D 	btnPMOpen��$��0��$
��Open��$��0,0,5,0q��� -   $D btnPMNew��$��1��$	��New��$��	5.4,0,0,0q�$��23��$	��Top=��� $A 
Horizontal=�$��	10,5,10,5q�.+��� $	��100��$��Name: ���� -   E ��Name$E 	txtPMName��$	��200���� $A 
Horizontal=�$��	10,5,10,5q�.+��� $	��100��$��Description: ���� -   $E txtPMDescription��$	��400��$	��100��F �� AcceptsReturnF . G �� TextWrapping$
G Wrap=�"H �� VerticalScrollBarVisibility$
H Auto=��� $A 
Horizontal=�$��	10,5,10,5q�.+��� $	��100��$��	Creator: ���� -   $E txtPMCreator��$	��200���� $��Targets��.���� -   $C tabControl_targets��
�������$��Stretch=�$��Stretch=�.@��� $��Targets��.��]� ��. .+�� $��Stretch=�$��
10,10,10,5q�$	��Top=�- �� $��* ��� $��* �.+��� -   $D 
btnTTLoadT��$��0��$��Load Targets��$��0,0,5,0q��� -   $D btnNewTarget��$��1��$��
New Target��$��5.2,0,0.4,0.4q�$��23��$��Bottom=��� $��Bottom=�$��Stretch=�$��Stretch=�.+��� �� $��Number of targets: ��$��	10,5,10,0q�  -   I  Name$I 
dgrTargets��$��Stretch=�$��Stretch=�$��0,5,0,10q�
�������J   AutoGenerateColumnsJ .  K   
IsReadOnlyK .  $L �� IsSynchronizedWithCurrentItem$
L TrueY�M   CanUserAddRowsM .  N   ColumnsN 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumnO   Width5  /System.Windows.Controls.DataGridLengthConverter$O 1* P   Header$P ID��Q   SortMemberPath$Q ID��R   
IsReadOnlyR . S   CellTemplateS �� .R��� $��0q����� *ID+$= TwoWay=�$> 	LostFocus=�  $O 2* $
P Name��$
Q Name��R .  S �� .R��� $��0q����� *Name+$= TwoWay=�$> 	LostFocus=�  $O 4* $P Method��$Q Method��R .  S �� .R��� ���� ?   *M� *link:TargetMethod++< �� *Method+$= TwoWay=�$> 	LostFocus=�  $O 2* $Q PointLocal.X��R .  P �� .�.��� .�X �� $��9(�.�objectS �� .R��� $��0q����� *PointLocal.X+$= TwoWay=�$> 	LostFocus=�  $O 2* $Q PointLocal.Y��R .  P �� .�.��� .�Y �� $��9(�.�objectS �� .R��� $��0q����� *PointLocal.Y+$= TwoWay=�$> 	LostFocus=�  $O 2* $Q PointLocal.Z��R .  P �� .�.��� .�Z �� $��9(�.�objectS �� .R��� $��0q����� *PointLocal.Z+$= TwoWay=�$> 	LostFocus=�  $O 2* $P X��$Q Point.X��R .  S �� .R��� $��0q����� *	Point.X+$= TwoWay=�$> 	LostFocus=�  $O 2* $P Y��$Q Point.Y��R .  S �� .R��� $��0q����� *	Point.Y+$= TwoWay=�$> 	LostFocus=�  $O 2* $P Z��$Q Point.Z��R .  S �� .R��� $��0q����� *	Point.Z+$= TwoWay=�$> 	LostFocus=�  $O 5* $P Description��$Q Description��R .  S �� .R��� $��0q����� *Description+$= TwoWay=�$> 	LostFocus=�  $
O 2.5* $P  ��S �� .R�� $��Stretch=�
�������.+��� $A 
Horizontal=�$��Center=�.+��� $��18��$��measure��#��[  $��50��$��10(��� $D btnTargetDel��-   $��18��$	��del��#��[ �� $��Control points��.��]� ��. .+�� $��Stretch=�$��
10,10,10,5q�$	��Top=�- �� $��* ��� $��* �.+��� $��0��$��Load control points��$��0,0,5,0q��� -   $D btnNewCP��$��1��$��New control point��$��5,0,0,0q��� $��Bottom=�$��Stretch=�$��Stretch=�.+��� �� $��Number of targets: ��$��	10,5,10,0q�  -   $I dgrControlPoints��$��Stretch=�$��Stretch=�$��0,5,0,10q�
�������J .  K .  $
L TrueY�M .  N   $O 1* $P ID��$Q ID��R . S �� .R��� $��0q����� *ID+$= TwoWay=�$> 	LostFocus=�  $O 2* $
P Name��$
Q Name��R .  S �� .R��� $��0q����� *Name+$= TwoWay=�$> 	LostFocus=�  $O 4* $P Method��$Q Method��R .  S �� .R��� ���� ?   *M� *link:TargetMethod++< �� *Method+$= TwoWay=�$> 	LostFocus=�  $O 2* $P X��$Q Point.X��R .  S �� .R��� $��0q����� *	Point.X+$= TwoWay=�$> 	LostFocus=�  $O 2* $P Y��$Q Point.Y��R .  S �� .R��� $��0q����� *	Point.Y+$= TwoWay=�$> 	LostFocus=�  $O 2* $P Z��$Q Point.Z��R .  S �� .R��� $��0q����� *	Point.Z+$= TwoWay=�$> 	LostFocus=�  $O 5* $P Description��$Q Description��R .  S �� .R��� $��0q����� *Description+$= TwoWay=�$> 	LostFocus=�  $
O 2.5* $P  ��S �� .R�� $��Stretch=�
�������.+��� $A 
Horizontal=�$��Center=�.+��� $��18��$��measure��#��[  $��50��$��10(��� $D btnTargetsCPDel��-   $��18��$	��del��#��[ �� $��Stations��.��]� ��. .+�� $��Stretch=�$��
10,10,10,5q�$	��Top=�- �� $��* ��� $��* �.+��� -   $D btnStationsLS��$��0��$��Load Stations��$��0,0,5,0q��� -   $D btnNewStation��$��1��$��New Station��$��5,0,0,0q��� $��Bottom=�$��Stretch=�$��Stretch=�.+��� �� $��Number of targets: ��$��	10,5,10,0q�  -   $I dgrStations��$��Stretch=�$��Stretch=�$��0,5,0,10q�
�������J .  K .  $
L TrueY�M .  N   $O 1* $P ID��$Q ID��R . S �� .R��� $��0q����� *ID+$= TwoWay=�$> 	LostFocus=�  $O 2* $
P Name��$
Q Name��R .  S �� .R��� $��0q����� *Name+$= TwoWay=�$> 	LostFocus=�  $O 2* $P X��$Q Point.X��R .  S �� .R��� $��0q����� *	Point.X+$= TwoWay=�$> 	LostFocus=�  $O 2* $P Y��$Q Point.Y��R .  S �� .R��� $��0q����� *	Point.Y+$= TwoWay=�$> 	LostFocus=�  $O 2* $P Z��$Q Point.Z��R .  S �� .R��� $��0q����� *	Point.Z+$= TwoWay=�$> 	LostFocus=�  $O 5* $P Description��$Q Description��R .  S �� .R��� $��0q����� *Description+$= TwoWay=�$> 	LostFocus=�  $
O 2.5* $P  ��S �� .R�� $��Stretch=�
�������.+��� $A 
Horizontal=�$��Center=�.+�� {��� #? [8 $> PropertyChanged=�.+��� $D btnStationsMeasure��-   $��18��$��measure��#��[  $��50��$��10(�{��� *
IsCOMSet+$> 	LostFocus=��� $D btnStationsDel��-   $��18��$	��del��#��[ �� $��Measuring concept��.���� -   T ��Name$T stpMConceptObservations��.+��� #��[ $	��Top=�$��50��$��30��$��Right=�$��5q�U K� ZIndex$U 1��.��	Clear allR� -   V R�Name$V trvMConcept��$��0q�$	��525��$��Disabled=�$
��Auto=�$��	0,-25,0,0q��� $��Measuring mode��.���� -    $C tabControl_measuringMode��
�������$��Stretch=�$��Stretch=�.@��� $��Connect��.��� -!   W �Name$W wrpMModeConnect��X � Orientation$X 
Horizontal=��� $��Observations��.��]� .+�]� $	��Top=�.+��� -"   $D btnMModeObsStart��$
��Left=�#��[  $��80��$��30��.��Start�� -#   $D btnMModeObsStop��$
��Left=�$��12(�#��[ $��80��$��30��$��5,0,0,0q�{�.  .��Stop / Pause�� -$   $ D btnMModeObsClearMeasurings��$��12(�$��Right=�$��30��$	��100��#��[ $��5,0,0,0q�.��Clear measurings�� -%   $D btnMModeObsClearConcept��$��12(�$��Right=�$��30��$��80��#��[ $��5,0,0,0q�.��Clear concept�� $��Right=�$��5,0,0,0q�.��Check Targets�� -&   $D btnMModeObsFromMConcept��$��Right=�$��5,0,0,0q�.��Load from measurement concept� �� $	��Top=�R� -'   $V trvMMode��$��0q�$	��500��$��Disabled=�$
��Auto=�$��0q�$	��Top=�� �� $��Results��.��]� .+��� -(   $C tabControl_results��$	��Top=�
�������$��Stretch=�$��Stretch=�.@��� $��Measuring data��.��]� .+�]� $	��Top=�.+��� $��Right=�$��5,0,0,0q�.��Export� �� $	��Top=�� �� $��Calculations��.��]� .+�]� $	��Top=�.+��� $A 
Horizontal=�.+��� $��0,0,5,0q�.��Calculate targets�� $��0,0,5,0q�$��Center=�$
1 TrueY�.��Fix stations�� $��0,0,5,0q�$��Center=�$
1 TrueY�.��Fix control points�� $��Right=�$��0,0,0,0q�.��Export� �� $	��Top=�� �� $��Differences��.��]� .+�]� $	��Top=�.+��� $��Right=�$��5,0,0,0q�.��Export� �� $	��Top=�� �� $��Protocol��.��]� .+�]� $	��Top=�.+��� $A 
Horizontal=�.+��� $��0,0,5,0q�$��Center=�$
1 TrueY�.��Full protocol�� $��Right=�$��5,0,0,0q�.��Export� �� $	��Top=�� �� $��Bottom=�#��[ $��80��$��30��$��Right=�$��5q�$U 1��.��Clear all results� �� -)   Y ��Name$Y tabMainSettings��$��Settings��.���� -*   $C tabControl_settings��
�������$��Stretch=�$��Stretch=�.@��� -+   $Y tabSetTSSet��$��Total station settings��.���� $
��Auto=�.��� -,   $W wrpSetTSSetSP��$X 
Horizontal=��� $��Notification settings��.��]� .+��� $A 
Horizontal=�$��5,5,0,0q�$	��Top=�.+��� $��80��.��
Send to:�� $	��180��$��20��$��Center=�.@��� Z �� 
IsSelectedZ . .��michael.ranzau@fhws.de�� $	��Top=�$��10,5,0,0q�.��.,Send notification on total stations behavior�� $	��Top=�$��29,5,0,0q�.��Power status�� $	��Top=�$��29,5,0,0q�.��Position is moving�� $	��Top=�$��29,5,0,0q�.��Orientiation is moving�� $	��Top=�$��29,5,0,0q�.��Fatal errors�� $	��Top=�$��10,5,0,0q�.��Connection error�� $	��Top=�$��10,5,0,0q�.��Attachments�� $	��Top=�$��29,5,0,0q�.��Last protocol�� $	��Top=�$��29,5,0,0q�.��Last measurements� $	��Top=��� $��Contacts��.��]� ��. .+�� $��Stretch=�$��
10,10,10,5q�$	��Top=�- �� $��* ��� $��* �.+��� --   $D btnNewContact��$��0��$��New contact��$��0,0,5,0q��� $��Bottom=�$��Stretch=�$��Stretch=�.+��� �� $��Number of contacts: ��$��	10,5,10,0q�  $��Stretch=�$��Stretch=�$��0,5,0,10q�
�������N   $O 1* $
P Name��$
Q Name��S �� .R��� $��0q����� *Name+  $O 1* $P EMail��$Q EMail��S �� .R��� $��0q����� *EMail+  $O 1* $P Phone��$Q Phone��S �� .R��� $��0q����� *Phone+  $
O 0.2* $P  ��$Q delete��S �� .R��� $	��del��#��[ 