   M S B A M L   `   `   `  ���� G  CTachyControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;-clr-namespace:TSControl;assembly=TachyControl	TSControl  V RAutomatedMicroTriangulation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:AutomatedMicroTriangulationAutomatedMicroTriangulation 1   +AutomatedMicroTriangulation.StationsMeasure   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     >link-clr-namespace:TSControl;assembly=TachyControl     ;local)clr-namespace:AutomatedMicroTriangulation       � Title$  StationsMeasure��$��409.199��$	��300�� � WindowStartupLocation$ CenterScreen=� � 	Resources    dataFromCOMs S� 
ObjectType   TSControl.TSControl S� 
MethodName��%&   &	        S� "  $ getCOMPorts��.��� .+��� .+���  �� Orientation$ 
Horizontal=�$��5q�.+��� $��70��.��	Target 1:�� -    ��Name$ cmbStationMeasureTarget1�� �� 
IsReadOnly .   �� DisplayMemberPath$
 Name��	 �� SelectedValuePath$	 ID��$	��100��
 �� SelectedValue
 �� *ID+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ 	LostFocus=��� $ 
Horizontal=�$	��5,0q�.+��� $��70��.��	Target 2:�� -   $ cmbStationMeasureTarget2�� .  $
 Name��$	 ID��$	��100��
 �� *ID+$ TwoWay=�$ 	LostFocus=��� $ 
Horizontal=�$��5,5,0,0q�.+��� $��70��.��	Target 3:�� -   $ cmbStationMeasureTarget3�� .  $
 Name��$	 ID��$	��100��
 �� *ID+$ TwoWay=�$ 	LostFocus=��� -    ��Name$ stpStationMeasureConPanel��$ Vertical=�$	��5,5q�$
��Left=�.+��� $ 
Horizontal=�.+��� $��70��.��COM:�� -   $ cmbStationMeasureCOM��$	��100������  �� Source# [  �� $ 
Horizontal=�$��5q�.+��� -    ��Name$ btnStationMeasureMeasure��.��
Do measure