# TachyControl
This is part of a control and measurement software for current total stations. For full functionality install the OpenCV wrapper EmguCV from https://sourceforge.net/projects/emgucv/ and add the library path to the path variable. In the Visual Studio project add Emgu.CV.UI.dll and Emgu.CV.World.dll to the referenced files.

Because of ristrictions in publishing the native GeoCOM commands from Leica Geosystems, the TSControl library can only be hand over on demand. Please contact michael.ranzau@fhws.de to get full access to all needed files and to the builded executeables.

More information will soon be available at geo.fhws.de/iats.html.


## WPF GUI
The main software *AutomatedMicroTriangulation* has a WPF-made graphical user interface. The backend is programmed in C#. The software includes a class for saving all needed project information called *XMLProject*. 
