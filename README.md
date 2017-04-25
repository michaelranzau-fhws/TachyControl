# TachyControl
This is part of a control and measurement software for current total stations. For full functionality install the OpenCV wrapper EmguCV from https://sourceforge.net/projects/emgucv/ and add the library path to the path variable. In the Visual Studio project add Emgu.CV.UI.dll and Emgu.CV.World.dll to the referenced files.

Because of ristrictions in publishing the native GeoCOM commands from Leica Geosystems, the TSControl library can only be hand over on demand. Please contact michael.ranzau@fhws.de to get full access to all needed files and to the builded executeables.

More information will soon be available at geo.fhws.de/iats.html.


## WPF GUI
The main software *AutomatedMicroTriangulation* has a WPF-made graphical user interface. The backend is programmed in C#. 

**Tab-wise GUI structure**

![alt text](https://github.com/michaelranzau-fhws/TachyControl/blob/master/pictures/ClassDiagram_WpfAutomatedMicroTriangulation_Controls.png "Diagram - GUI structure")

## Data types
The software includes a class for saving all needed project information called *XMLProject*. 

**Data types - overview**

![alt text](https://github.com/michaelranzau-fhws/TachyControl/blob/master/pictures/ClassDiagram_WpfAutomatedMicroTriangulation_DataTypes_Overview.png "Diagram - Data types - overview")

**Data types - detail**

![alt text](https://github.com/michaelranzau-fhws/TachyControl/blob/master/pictures/ClassDiagram_WpfAutomatedMicroTriangulation_DataTypes_Details.png "Diagram - Data types - detail")

## Enums
The software includes few enums to save some measurement settings.

![alt text](https://github.com/michaelranzau-fhws/TachyControl/blob/master/pictures/ClassDiagram_WpfAutomatedMicroTriangulation_Enums.png "Diagram - enums")

## Methods
Besides the methods provided by Features and TSControl there are some additional methods for the file management and for geometrical calculations.

![alt text](https://github.com/michaelranzau-fhws/TachyControl/blob/master/pictures/ClassDiagram_WpfAutomatedMicroTriangulation_Mehtods.png "Diagram - methods")

## Toolbox

### TSControl
Abstract class to represent and manage any kind of total stations. The class also includes the calibration methods.

![alt text](https://github.com/michaelranzau-fhws/TachyControl/blob/master/pictures/ClassDiagram_TSControl.png "Diagram - TSControl")

### Features
This class provides methods to find objects in an image.

![alt text](https://github.com/michaelranzau-fhws/TachyControl/blob/master/pictures/ClassDiagram_Features.png "Diagram - Features")
