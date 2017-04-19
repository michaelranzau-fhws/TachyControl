using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Collections;

namespace AutomatedMicroTriangulation
{
    /// <summary>
    /// File format for project data to save as XML data.
    /// </summary>

    [XmlRoot("Project")]
    public class XMLProject
    {
        private double _globalStdMax = 0.1;

        [XmlAttribute("GlobalStdMax")]
        public double GlobalStdMax
        {
            get { return _globalStdMax; }
            set { _globalStdMax = value; }
        }

        private bool _isSaved = false;

        [XmlIgnore]
        public bool IsSaved
        {
            get { return _isSaved; }
            set { _isSaved = value; }
        }

        private bool _isProjectLoaded = false;
        [XmlIgnore]
        public bool IsProjectLoaded
        {
            get { return _isProjectLoaded; }
            set { _isProjectLoaded = value; }
        }

        private string _name;
        [XmlAttribute("Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        internal List<TSControl.TSControl_Calibration> getCaliListFromSerialNo(string serialNumber)
        {
            List<TSControl.TSControl_Calibration> caliList = new List<TSControl.TSControl_Calibration>();
            caliList = (from calis in Calibrations
                        where calis.SerialNumber == serialNumber
                        select calis).ToList();
            return caliList;
        }

        private string _author;
        [XmlAttribute("Author")]
        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }
        private string _description;

        [XmlAttribute("Description")]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private ObservableCollection<Target> _targets = new ObservableCollection<Target>();
        [XmlElement("Target")]
        public ObservableCollection<Target> Targets
        {
            get { return _targets; }
            set {
                _targets = value;
            }
        }
        private ObservableCollection<Connection> _connections = new ObservableCollection<Connection>();

        [XmlElement("Connection")]
        public ObservableCollection<Connection> Connections
        {
            get { return _connections; }
            set { _connections = value; }
        }
        private ObservableCollection<ControlPoint> _controlPoints = new ObservableCollection<ControlPoint>();
        [XmlElement("ControlPoint")]
        public ObservableCollection<ControlPoint> ControlPoints
        {
            get { return _controlPoints; }
            set { _controlPoints = value; }
        }

        private ObservableCollection<Station> _stations = new ObservableCollection<Station>();
        [XmlElement("Station")]
        public ObservableCollection<Station> Stations
        {
            get { return _stations; }
            set { _stations = value; }
        }

        private ObservableCollection<TSControl.TSControl_Calibration> _calibrations = new ObservableCollection<TSControl.TSControl_Calibration>();
        [XmlElement("Calibration")]
        public ObservableCollection<TSControl.TSControl_Calibration> Calibrations
        {
            get { return _calibrations; }
            set { _calibrations = value; }
        }

        private MeasuringConcept _mConcept = new MeasuringConcept();

        [XmlElement("MeasuringConcept")]
        public MeasuringConcept MConcept
        {
            get { return _mConcept; }
            set { _mConcept = value; }
        }
        private MeasuringMode _mMode = new MeasuringMode();

        [XmlElement("MeasuringMode")]
        public MeasuringMode MMode
        {
            get { return _mMode; }
            set { _mMode = value; }
        }
        private ProtocolSetting _protocolSettings = new ProtocolSetting();

        [XmlElement("ProtocolSetting")]
        public ProtocolSetting ProtocolSettings
        {
            get { return _protocolSettings; }
            set { _protocolSettings = value; }
        }

        private string _projectPath = "";

        [XmlIgnore]
        public string ProjectPath
        {
            get { return _projectPath; }
            set {
                _projectPath = value;
                TSControl.TSControl.ProjectPath = _projectPath;
                ProtocolSetting.ProtocolPath = _projectPath;
            }
        }


        public XMLProject() {
        }

    }

    /// <summary>
    /// File format for app data to save as XML data.
    /// </summary>

    [XmlRoot("AppSettings")]
    public class XMLAppSettings
    {

        private bool _isLoaded = false;
        [XmlIgnore]
        public bool IsLoaded
        {
            get { return _isLoaded; }
            set { _isLoaded = value; }
        }
        private ObservableCollection<Contact> _contacts = new ObservableCollection<Contact>();
        [XmlElement("Contact")]
        public ObservableCollection<Contact> Contacts
        {
            get { return _contacts; }
            set { _contacts = value; }
        }
        
    }
}
