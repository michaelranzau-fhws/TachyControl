using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using TSControl;
using Features;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace AutomatedMicroTriangulation
{
    /// <summary>
    /// Saving Bluetooth connection settings.
    /// </summary>
    public class Connection : INotifyPropertyChanged
    {
        // ##### Properties #####

        private string _com = "";
        /// <summary>
        /// Keyword to connect with specific COM port.
        /// </summary>
        [XmlAttribute("COM")]
        public string COM
        {
            get { return _com; }
            set {
                _com = value;
                OnPropertyChanged("COM");
            }
        }
        private BaudRate _baudRate;
        [XmlAttribute("BaudRate")]
        public BaudRate _BaudRate
        {
            get { return _baudRate; }
            set {
                _baudRate = value;
                OnPropertyChanged("_BaudRate");
            }
        }
        private DataBits _dataBits;

        [XmlAttribute("DataBits")]
        public DataBits _DataBits
        {
            get { return _dataBits; }
            set {
                _dataBits = value;
                OnPropertyChanged("_DataBits");
            }
        }
        private System.IO.Ports.Parity _parity;

        [XmlAttribute("Parity")]
        public System.IO.Ports.Parity _Parity
        {
            get { return _parity; }
            set {
                _parity = value;
                OnPropertyChanged("_Parity");
            }
        }
        private System.IO.Ports.StopBits _stopBits;


        [XmlAttribute("StopBits")]
        public System.IO.Ports.StopBits _StopBits
        {
            get { return _stopBits; }
            set {
                _stopBits = value;
                OnPropertyChanged("_StopBits");
            }
        }

        private TotalStationType _deviceType = TotalStationType.Leica;

        [XmlAttribute("DeviceType")]
        public TotalStationType DeviceType
        {
            get { return _deviceType; }
            set
            {
                _deviceType = value;
                OnPropertyChanged("DeviceType");
            }
        }

        // ##### Constructor #####

        public Connection()
        {
            resetToDefault();
        }

        // ##### Methods #####


        /// <summary>
        /// Get SerialPort object with current serial port settings.
        /// </summary>
        /// <returns>SerialPort obeject</returns>
        public System.IO.Ports.SerialPort getSerialPort()
        {
            System.IO.Ports.SerialPort sp = new System.IO.Ports.SerialPort();
            sp.BaudRate = (int)_BaudRate;
            sp.DataBits = (int)_DataBits;
            sp.Parity = _Parity;
            sp.StopBits = _StopBits;
            sp.PortName = COM;
            return sp;
        }

        /// <summary>
        /// Set connection settings to default.
        /// BaudRate = BaudRate.BD115200
        /// DataBits = DataBits.DB8
        /// Parity = System.IO.Ports.Parity.None
        /// StopBits = System.IO.Ports.StopBits.One
        /// DeviceType = TotalStationType.Leica
        /// </summary>
        public void resetToDefault()
        {
            _BaudRate = BaudRate.BD115200;
            _DataBits = DataBits.DB8;
            _Parity = System.IO.Ports.Parity.None;
            _StopBits = System.IO.Ports.StopBits.One;
            DeviceType = TotalStationType.Leica;
        }

        // ##### Events #####

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
    /// <summary>
    /// Holds contact information for notifications.
    /// </summary>
    public class Contact
    {
        // ##### Properties #####
        private static int _count = 0;

        public static int Count
        {
            get { return _count; }
            set { _count = value; }
        }
        private int _id;

        [XmlAttribute("ID")]
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _email = "";

        [XmlAttribute("EMail")]
        public string EMail
        {
            get { return _email; }
            set { _email = value; }
        }
        private string _name = "";

        [XmlAttribute("Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private string _phone = "";

        [XmlAttribute("Phone")]
        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        // ##### Constructor #####
        public Contact()
        {
            ID = Count++;
        }

    }
    /// <summary>
    /// Represents a control point
    /// </summary>
    public class ControlPoint
    {
        // ##### Properties #####

        private static int _count = 0;

        public static int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        private static int _newID = 0;

        public static int NewID
        {
            get { return _newID; }
            set { _newID = value; }
        }
        private int _id;

        [XmlAttribute("ID")]
        public int ID
        {
            get { return _id; }
            set
            {
                _id = value;
                if (_id >= NewID)
                    NewID = _id + 1;
            }
        }
        private TargetMethod _method;

        [XmlAttribute("Method")]
        public TargetMethod Method
        {
            get { return _method; }
            set
            {
                _method = value;
            }
        }
        private string _name = "";

        [XmlAttribute("Name")]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
            }
        }

        private Point3D _point = new Point3D();
        [XmlElement("Point")]
        public Point3D Point
        {
            get { return _point; }
            set
            {
                _point = value;
            }
        }

        private string _description;

        [XmlAttribute("Description")]
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
            }
        }

        // ##### Constructor #####

        public ControlPoint()
        {
            ID = NewID;
            Count++;
        }
    }
    
    /// <summary>
    /// Represents the measuring concept
    /// </summary>
    public class MeasuringConcept
    {
        // ##### Properties #####

        private TreeView _tv;
        /// <summary>
        /// Treeview to fill with the concept.
        /// </summary>
        [XmlIgnore]
        public TreeView TV
        {
            get { return _tv; }
            set { _tv = value; }
        }

        private ObservableCollection<ObservationGroup> _observationGroups = new ObservableCollection<ObservationGroup>();
        [XmlElement("ObservationGroup")]
        public ObservableCollection<ObservationGroup> ObservationGroups
        {
            get { return _observationGroups; }
            set { _observationGroups = value; }
        }
        private ObservableCollection<Notification> _notifications = new ObservableCollection<Notification>();
        [XmlElement("Notification")]
        public ObservableCollection<Notification> Notifications
        {
            get { return _notifications; }
            set { _notifications = value; }
        }
        private ObservableCollection<Pause> _pauses = new ObservableCollection<Pause>();
        [XmlElement("Pause")]
        public ObservableCollection<Pause> Pauses
        {
            get { return _pauses; }
            set { _pauses = value; }
        }

        private ObservableCollection<Operation> _operations = new ObservableCollection<Operation>();
        [XmlElement("Operation")]
        public ObservableCollection<Operation> Operations
        {
            get { return _operations; }
            set { _operations = value; }
        }

        // ##### Constructor #####

        public MeasuringConcept()
        {
            TV = MainWindow.TVMConcept;
            TV.Items.Clear();
            TV.Items.Add(new MConceptAdd().TVI);
            Operations.CollectionChanged += this.OnOperationsCollectionChanged;
        }

        // ##### Methods #####

        /// <summary>
        /// Handle changes in measuring concept.
        /// </summary>
        private void OnOperationsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (MainWindow.Project.IsProjectLoaded)
            // Handles only changes if project is already loaded.
            {
                NotifyCollectionChangedAction action = e.Action;
                ObservableCollection<Operation> ops = sender as ObservableCollection<Operation>;

                switch (action)
                {
                    // Elements / operations added to concept
                    case NotifyCollectionChangedAction.Add:

                        foreach (Operation opNew in e.NewItems)
                        {
                            switch (opNew.Type)
                                // Create new operations like observation groups, notifications or pauses.
                            {
                                case OperationType.ObservationGroup:

                                    ObservationGroup og = new ObservationGroup();
                                    opNew.IDOfType = og.ID;
                                    MainWindow.Project.MConcept.ObservationGroups.Add(og);

                                    MConceptObservations o = new MConceptObservations();
                                    o.ObsGroup = og;
                                    TV.Items.Add(o.TVI);
                                    TV.Items.Add(new MConceptAdd().TVI);

                                    break;
                                case OperationType.Notification:
                                    Notification n = new Notification();
                                    opNew.IDOfType = n.ID;
                                    MainWindow.Project.MConcept.Notifications.Add(n);

                                    MConceptNotification mcn = new MConceptNotification();
                                    mcn.NotificationGroup = n;
                                    TV.Items.Add(mcn.TVI);
                                    TV.Items.Add(new MConceptAdd().TVI);

                                    break;
                                case OperationType.Pause:
                                    Pause p = new Pause();
                                    opNew.IDOfType = p.ID;
                                    MainWindow.Project.MConcept.Pauses.Add(p);

                                    MConceptPause mcp = new MConceptPause();
                                    mcp.PauseGroup = p;
                                    TV.Items.Add(mcp.TVI);
                                    TV.Items.Add(new MConceptAdd().TVI);

                                    break;
                            }
                            // Move new operation to the right position.
                            Operations.Move(Operations.IndexOf(opNew), opNew.ID);
                        }

                        break;
                    // Elements / operations removed from concept
                    case NotifyCollectionChangedAction.Remove:
                        // There is nothing to do
                        break;
                    // Elements / operations moved in the concept
                    case NotifyCollectionChangedAction.Move:
                        foreach (Operation opOld in e.OldItems)
                        {
                            int addIndex = (e.OldStartingIndex * 2 + 1) + 1;
                            int itemIndex = e.OldStartingIndex * 2 + 1;
                            int addIndexNew = (e.NewStartingIndex * 2 + 1) + 1;
                            int itemIndexNew = e.NewStartingIndex * 2 + 1;
                            TV.Items.MoveCurrentToPosition(itemIndex);
                            TreeViewItem tviItem = (TreeViewItem)TV.Items.CurrentItem;
                            TV.Items.MoveCurrentToPosition(addIndex);
                            TreeViewItem tviAdd = (TreeViewItem)TV.Items.CurrentItem;

                            TV.Items.Remove(tviItem);
                            TV.Items.Remove(tviAdd);

                            TV.Items.Insert(itemIndexNew, tviItem);
                            TV.Items.Insert(addIndexNew, tviAdd);

                            int i = 0;
                            foreach(Operation opAll in ops)
                            {
                                opAll.ID = i++;
                            }
                        }
                        break;
                }
            }
        }
    }
    /// <summary>
    /// Represents the measuring mode
    /// </summary>
    public class MeasuringMode
    {
        // ##### Properties #####
        private TreeView _tv;
        /// <summary>
        /// Treeview to fill with the measurings.
        /// </summary>
        [XmlIgnore]
        public TreeView TV
        {
            get { return _tv; }
            set { _tv = value; }
        }
        private ObservableCollection<ObservationGroup> _observationGroups = new ObservableCollection<ObservationGroup>();
        [XmlElement("ObservationGroup")]
        public ObservableCollection<ObservationGroup> ObservationGroups
        {
            get { return _observationGroups; }
            set { _observationGroups = value; }
        }
        private ObservableCollection<Notification> _notifications = new ObservableCollection<Notification>();
        [XmlElement("Notification")]
        public ObservableCollection<Notification> Notifications
        {
            get { return _notifications; }
            set { _notifications = value; }
        }
        private ObservableCollection<Pause> _pauses = new ObservableCollection<Pause>();
        [XmlElement("Pause")]
        public ObservableCollection<Pause> Pauses
        {
            get { return _pauses; }
            set { _pauses = value; }
        }

        private ObservableCollection<Operation> _operations = new ObservableCollection<Operation>();
        [XmlElement("Operation")]
        public ObservableCollection<Operation> Operations
        {
            get { return _operations; }
            set { _operations = value; }
        }

        // ##### Constructor #####

        public MeasuringMode()
        {
            TV = MainWindow.TVMMode;
            TV.Items.Clear();
            TV.Items.Add(new MModeAdd().TVI);
            Operations.CollectionChanged += this.OnOperationsCollectionChanged;
        }

        // ##### Methods #####
        private void OnOperationsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (MainWindow.Project.IsProjectLoaded)
            // Handles only changes if project is already loaded.
            {
                NotifyCollectionChangedAction action = e.Action;
                ObservableCollection<Operation> ops = sender as ObservableCollection<Operation>;

                switch (action)
                {
                    case NotifyCollectionChangedAction.Add:
                        // Elements / operations added to measuring mode
                        foreach (Operation opNew in e.NewItems)
                        {
                            switch (opNew.Type)
                            // Create new operations like observation groups, notifications or pauses.
                            {
                                case OperationType.ObservationGroup:
                                    ObservationGroup og = new ObservationGroup();
                                    opNew.IDOfType = og.ID;
                                    MainWindow.Project.MMode.ObservationGroups.Add(og);

                                    MModeObservations o = new MModeObservations();
                                    o.ObsGroup = og;
                                    TV.Items.Add(o.TVI);
                                    TV.Items.Add(new MModeAdd().TVI);

                                    break;
                                case OperationType.Notification:
                                    Notification n = new Notification();
                                    opNew.IDOfType = n.ID;
                                    MainWindow.Project.MMode.Notifications.Add(n);

                                    MModeNotification mcn = new MModeNotification();
                                    mcn.NotificationGroup = n;
                                    TV.Items.Add(mcn.TVI);
                                    TV.Items.Add(new MModeAdd().TVI);

                                    break;
                                case OperationType.Pause:
                                    Pause p = new Pause();
                                    opNew.IDOfType = p.ID;
                                    MainWindow.Project.MMode.Pauses.Add(p);

                                    MModePause mcp = new MModePause();
                                    mcp.PauseGroup = p;
                                    TV.Items.Add(mcp.TVI);
                                    TV.Items.Add(new MModeAdd().TVI);

                                    break;
                            }
                            // Move new operation to the right position.
                            Operations.Move(Operations.IndexOf(opNew), opNew.ID);
                        }

                        break;
                    // Elements / operations removed from measuring mode
                    case NotifyCollectionChangedAction.Remove:
                        break;
                    // Elements / operations moved in the measuring mode
                    case NotifyCollectionChangedAction.Move:
                        foreach (Operation opOld in e.OldItems)
                        {
                            int addIndex = (e.OldStartingIndex * 2 + 1) + 1;
                            int itemIndex = e.OldStartingIndex * 2 + 1;
                            int addIndexNew = (e.NewStartingIndex * 2 + 1) + 1;
                            int itemIndexNew = e.NewStartingIndex * 2 + 1;
                            TV.Items.MoveCurrentToPosition(itemIndex);
                            TreeViewItem tviItem = (TreeViewItem)TV.Items.CurrentItem;
                            TV.Items.MoveCurrentToPosition(addIndex);
                            TreeViewItem tviAdd = (TreeViewItem)TV.Items.CurrentItem;

                            TV.Items.Remove(tviItem);
                            TV.Items.Remove(tviAdd);

                            TV.Items.Insert(itemIndexNew, tviItem);
                            TV.Items.Insert(addIndexNew, tviAdd);

                            int i = 0;
                            foreach (Operation opAll in ops)
                            {
                                opAll.ID = i++;
                            }
                        }
                        break;
                }
            }
        }
    }
    /// <summary>
    /// Represents a notification
    /// </summary>
    public class Notification
    {
        // ##### Properties #####

        private static int _newID = 0;

        public static int NewID
        {
            get { return _newID; }
            set { _newID = value; }
        }
        private int _id;

        [XmlAttribute("ID")]
        public int ID
        {
            get { return _id; }
            set
            {
                _id = value;
                if (_id >= NewID)
                    NewID = _id + 1;
            }
        }
        private int _contactID;

        [XmlAttribute("ContactID")]
        public int ContactID
        {
            get { return _contactID; }
            set { _contactID = value; }
        }
        private string _content;

        [XmlAttribute("Content")]
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }
        private string _attachments;

        [XmlAttribute("Attachments")]
        public string Attachments
        {
            get { return _attachments; }
            set { _attachments = value; }
        }

        // ##### Constructor #####

        public Notification()
        {
            ID = NewID;
        }
    }
    /// <summary>
    /// Represents an observation.
    /// </summary>
    public class Observation : INotifyPropertyChanged
    {
        // ##### Static properties #####
        private static int _count;

        public static int Count
        {
            get { return _count; }
            set { _count = value; }
        }
        private static int _newID = 0;

        public static int NewID
        {
            get { return _newID; }
            set { _newID = value; }
        }


        // ##### Properties #####

        private int _id;
        [XmlAttribute("ID")]
        public int ID
        {
            get { return _id; }
            set {
                _id = value;
                if (_id >= NewID)
                    NewID = _id + 1;
            }
        }

        private string _serialNumber = "";

        [XmlAttribute("SerialNumber")]
        public string SerialNumber
        {
            get { return _serialNumber; }
            set {
                _serialNumber = value;
                OnPropertyChanged("SerialNumber");
            }
        }

        private int _stationID;

        [XmlAttribute("StationID")]
        public int StationID
        {
            get { return _stationID; }
            set {
                _stationID = value;
                OnPropertyChanged("StationID");
            }
        }
        private int _targetID;

        [XmlAttribute("TargetID")]
        public int TargetID
        {
            get { return _targetID; }
            set {
                _targetID = value;
                OnPropertyChanged("TargetID");
            }
        }
        private TargetMethod _method = TargetMethod.Ellipse;

        [XmlAttribute("Method")]
        public TargetMethod Method
        {
            get { return _method; }
            set {
                _method = value;
                OnPropertyChanged("Method");
            }
        }
        private int _shutter;

        [XmlAttribute("Shutter")]
        public int Shutter
        {
            get { return _shutter; }
            set {
                _shutter = value;
                OnPropertyChanged("Shutter");
            }
        }
        private int _gain;

        [XmlAttribute("Gain")]
        public int Gain
        {
            get { return _gain; }
            set {
                _gain = value;
                OnPropertyChanged("Gain");
            }
        }
        private float _std;

        [XmlAttribute("Std")]
        public float Std
        {
            get { return _std; }
            set {
                _std = value;
                OnPropertyChanged("Std");
            }
        }
        private Angle _angleObs = new Angle();

        [XmlElement("AngleObs")]
        public Angle AngleObs
        {
            get { return _angleObs; }
            set {
                _angleObs = value;
                OnPropertyChanged("AngleObs");
            }
        }
        private Angle _angleObsFace2 = new Angle();

        [XmlElement("AngleObsFace2")]
        public Angle AngleObsFace2
        {
            get { return _angleObsFace2; }
            set
            {
                _angleObsFace2 = value;
                OnPropertyChanged("AngleObsFace2");
            }
        }
        private Angle _angleApprox = new Angle();

        [XmlElement("AngleApprox")]
        public Angle AngleApprox
        {
            get { return _angleApprox; }
            set {
                _angleApprox = value;
                OnPropertyChanged("AngleApprox");
            }
        }

        private Point2D _point = new Point2D();

        [XmlElement("Point")]
        public Point2D Point
        {
            get { return _point; }
            set {
                _point = value;
                OnPropertyChanged("Point");
            }
        }
        private Point2D _pointFace2 = new Point2D();

        [XmlElement("PointFace2")]
        public Point2D PointFace2
        {
            get { return _pointFace2; }
            set
            {
                _pointFace2 = value;
                OnPropertyChanged("PointFace2");
            }
        }
        private DateTime _timestamp;

        [XmlAttribute("Timestamp")]
        public DateTime Timestamp
        {
            get { return _timestamp; }
            set {
                _timestamp = value;
                OnPropertyChanged("Timestamp");
            }
        }
        private MState _mState = MState.Waiting;

        [XmlAttribute("MState")]
        public MState _MState
        {
            get { return _mState; }
            set {
                _mState = value;
                OnPropertyChanged("Timestamp");
            }
        }
        private int _calibrationID = -1;


        [XmlAttribute("CalibrationID")]
        public int CalibrationID
        {
            get { return _calibrationID; }
            set { _calibrationID = value; }
        }

        private int _fucosMotorPosition = -1;
        [XmlElement("FocusMotorPosition")]
        public int FocusMotorPosition
        {
            get { return _fucosMotorPosition; }
            set { _fucosMotorPosition = value; }
        }

        // ##### Events #####

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));

            }
        }
        // ##### Constructor #####

        public Observation()
        {
            Count++;
            ID = NewID;
            try
            {
                Std = (float)MainWindow.Project.GlobalStdMax;
            }
            catch
            {
                Std = 0.1f;
            }
        }
    }
    /// <summary>
    /// An observation group handles many observations with addidional information about 
    /// sorting, repetitions, shots and faces to measure.
    /// </summary>
    public class ObservationGroup
    {
        // ##### Properties #####
        private SynchMeasMode _synch;

        [XmlAttribute("SynchronousMeasurement")]
        public SynchMeasMode SynchronousMeasurement
        {
            get { return _synch; }
            set { _synch = value; }
        }
        private TargetsSortBy _sortBy;

        [XmlAttribute("SortBy")]
        public TargetsSortBy SortBy
        {
            get { return _sortBy; }
            set { _sortBy = value; }
        }
        private Face _face;

        [XmlAttribute("Faces")]
        public Face Faces
        {
            get { return _face; }
            set {
                _face = value;

            }
        }
        private int _repetitions;

        [XmlAttribute("Repetitions")]
        public int Repetitions
        {
            get { return _repetitions; }
            set { _repetitions = value; }
        }
        private int _shots;

        [XmlAttribute("Shots")]
        public int Shots
        {
            get { return _shots; }
            set { _shots = value; }
        }
        private static int _newID = 0;

        public static int NewID
        {
            get { return _newID; }
            set { _newID = value; }
        }
        private int _id;

        [XmlAttribute("ID")]
        public int ID
        {
            get { return _id; }
            set
            {
                _id = value;
                if (_id >= NewID)
                    NewID = _id + 1;
            }
        }

        private string _name;

        [XmlAttribute("Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private ItemsChangeObservableCollection<Observation> _observations = new ItemsChangeObservableCollection<Observation>();
        [XmlElement("Observations")]
        public ItemsChangeObservableCollection<Observation> Observations
        {
            get { return _observations; }
            set {
                _observations = value;
            }
        }
        private ICollectionView _obsView;
        [XmlIgnore]
        public ICollectionView ObsView
        {
            get { return _obsView; }
            set { _obsView = value; }
        }
        private ObservableCollection<ObservationGroupStation> _observationGroupStations = new ObservableCollection<ObservationGroupStation>();
        [XmlElement("ObservationGroupStations")]
        public ObservableCollection<ObservationGroupStation> _ObservationGroupStations
        {
            get { return _observationGroupStations; }
            set { _observationGroupStations = value; }
        }

        private SortDescriptionCollection _obsSortDescription = new SortDescriptionCollection();
        [XmlElement("ObsSortDescription")]
        public SortDescriptionCollection ObsSortDescription
        {
            get { return _obsSortDescription; }
            set {
                _obsSortDescription = value;
            }
        }

        // ##### Constructor #####
        public ObservationGroup()
        {
            ID = NewID;
            ObsView = CollectionViewSource.GetDefaultView(Observations);
            Observations.CollectionChanged += Observations_CollectionChanged;

        }

        // ##### Methods #####

        /// <summary>
        /// Handels observation list changes.
        /// </summary>
        private void Observations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedAction action = e.Action;
            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        setObservationGroupStations();
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    setObservationGroupStations();
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void setObservationGroupStations()
        {
            if (MainWindow.Project.IsProjectLoaded)
            {
                if (Observations.Count > 0)
                {
                    var obsBySID = from o in Observations
                                   orderby o.StationID
                                   group o by o.StationID into groupO
                                   select groupO.OrderBy(p => p.StationID).First();

                    foreach (var o in obsBySID)
                    {
                        if (_ObservationGroupStations.Any(p => p.StationID == o.StationID))
                        {

                        }
                        else
                            // If station is not present in current observation group,
                            // add new station handler (ObservationGroupStation) to observation group
                        {
                            ObservationGroupStation oGrS = new ObservationGroupStation();
                            oGrS.StationID = o.StationID;
                            _ObservationGroupStations.Add(oGrS);
                        }
                    }
                }
                List<ObservationGroupStation> removeListOGrS = new List<ObservationGroupStation>();
                foreach (ObservationGroupStation oGrS in _ObservationGroupStations)
                {
                    if (Observations.Any(p => p.StationID == oGrS.StationID))
                    {

                    }
                    else
                    // If station is present in current observation group but not in any observation
                    // delete station handler (ObservationGroupStation) from observation group
                    {
                        removeListOGrS.Add(oGrS);
                    }
                }
                foreach (ObservationGroupStation oGrS in removeListOGrS)
                {
                    _ObservationGroupStations.Remove(oGrS);
                }
                removeListOGrS.Clear();
            }
        }
        /// <summary>
        /// Update sort descriptions.
        /// </summary>
        public void ObsView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ObsSortDescription = ObsView.SortDescriptions;
        }
    }
    /// <summary>
    /// Saves settings for all Observation from the same station in one observation group.
    /// </summary>
    public class ObservationGroupStation : INotifyPropertyChanged
    {
        // ##### Properties #####

        private static int _newID = 0;

        public static int NewID
        {
            get { return _newID; }
            set { _newID = value; }
        }
        private int _id;

        [XmlAttribute("ID")]
        public int ID
        {
            get { return _id; }
            set
            {
                _id = value;
                if (_id >= NewID)
                    NewID = _id + 1;
            }
        }

        private int _calibrationID = -1;

        [XmlAttribute("CalibrationID")]
        public int CalibrationID
        {
            get { return _calibrationID; }
            set { _calibrationID = value; }
        }
        private CalibrationType _caliType = CalibrationType.OnlyOneCalibration;
        /// <summary>
        /// Set how many calibrations will be done.
        /// </summary>
        [XmlAttribute("CaliType")]
        public CalibrationType CaliType
        {
            get { return _caliType; }
            set { _caliType = value; }
        }

        private string _com = "";

        [XmlAttribute("COM")]
        public string COM
        {
            get { return _com; }
            set
            {
                _com = value;
                if (_com != "")
                {
                    IsCOMSet = true;
                    if (MainWindow.Project.IsProjectLoaded)
                        getConnection();
                }
                else
                {
                    IsCOMSet = false;
                }
                OnPropertyChanged("COM");
            }
        }
        private bool _isCOMSet = false;
        [XmlIgnore]
        public bool IsCOMSet
        {
            get { return _isCOMSet; }
            set { _isCOMSet = value; }
        }

        private int _stationID = -1;

        [XmlAttribute("StationID")]
        public int StationID
        {
            get { return _stationID; }
            set {
                _stationID = value;
                try
                {
                    StationName = (from s in MainWindow.Project.Stations
                                   where s.ID == _stationID
                                   select s.Name).First();
                }
                catch
                {
                    StationName = "";
                }
            }
        }
        private string _stationName = "";
        [XmlIgnore]
        public string StationName
        {
            get { return _stationName; }
            set { _stationName = value; }
        }

        // ##### Events #####
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        // ##### Constructor #####
        public ObservationGroupStation()
        {
            ID = NewID;
        }

        // ##### Methods #####
        public Connection getConnection()
        {
            Connection con;
            if (COM == "")
            {
                if(MainWindow.Project.Connections.Count > 0)
                {
                    con = MainWindow.Project.Connections[0];
                }
                else
                {
                    con = new Connection();
                    string[] coms = TSControl.TSControl.getCOMPorts();
                    if(coms.Length > 0)
                    {
                        con.COM = coms[0];
                    }
                    else
                    {
                        con.COM = "COM1";
                    }
                }
            }
            else
            {
                try
                {
                    con = MainWindow.Project.Connections.First(
                        delegate (Connection conFind)
                        {
                            return conFind.COM == COM;
                        }
                        );
                }
                catch
                {
                    con = new Connection();
                    con.COM = COM;
                    MainWindow.Project.Connections.Add(con);
                }
            }
            return con;
        }
    }
    /// <summary>
    /// Represents an operation like observation groups, notifications and pauses for sorting.
    /// </summary>
    public class Operation
    {
        // ##### Properties #####
        private string _tvi_ContentHeader;
        [XmlIgnore]
        public string TVI_ContentHeader
        {
            get { return _tvi_ContentHeader; }
            set { _tvi_ContentHeader = value; }
        }

        private static int _count = 0;

        public static int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        private int _id;
        [XmlAttribute("ID")]
        public int ID
        {
            get { return _id; }
            set {
                _id = value;
                TVI_ContentHeader = "ID: " +  _id.ToString();
            }
        }
        private int _idOfType = -1;

        [XmlAttribute("IDOfType")]
        public int IDOfType
        {
            get { return _idOfType; }
            set {
                _idOfType = value;
            }
        }
        private OperationType _type = OperationType.ObservationGroup;

        [XmlAttribute("Type")]
        public OperationType Type
        {
            get { return _type; }
            set {
                _type = value;
            }
        }

        // ##### Constructor #####
        public Operation()
        {
            ID = Count++;
        }
    }
    /// <summary>
    /// Saves properties for a pause during measuring mode.
    /// </summary>
    public class Pause
    {
        // ##### Properties #####

        private static int _newID = 0;

        public static int NewID
        {
            get { return _newID; }
            set { _newID = value; }
        }
        private int _id;

        [XmlAttribute("ID")]
        public int ID
        {
            get { return _id; }
            set
            {
                _id = value;
                if (_id >= NewID)
                    NewID = _id + 1;
            }
        }
        private double _delay = 10;

        [XmlAttribute("Delay")]
        /// <summary>
        /// Pause in seconds.
        /// </summary>
        public double Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }
        private DateTime _startAt;

        [XmlAttribute("StartAt")]
        public DateTime StartAt
        {
            get { return _startAt; }
            set { _startAt = value; }
        }
        private PauseType _type = PauseType.Delay;

        [XmlAttribute("Type")]
        public PauseType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        // ##### Constructor #####
        public Pause()
        {
            ID = NewID;
        }
    }
    /// <summary>
    /// Savs protocoll settings. Currently only the protocol path is available.
    /// </summary>
    public class ProtocolSetting
    {
        // ##### Properties #####
        private static string _protocolPath = @"\Protocol_" + DateTime.Today.ToString("yyyyMMdd") + ".txt";
        [XmlAttribute("ProtocolPath")]
        public static string ProtocolPath
        {
            get { return _protocolPath; }
            set { _protocolPath = value; }
        }

        // ##### Constructor #####
        public ProtocolSetting() { }
    }
    /// <summary>
    /// Represents a station.
    /// </summary>
    public class Station : INotifyPropertyChanged
    {
        // ##### Static properties #####
        private static int _count = 0;

        public static int Count
        {
            get { return _count; }
            set { _count = value; }
        }
        private static int _newID = 0;

        public static int NewID
        {
            get { return _newID; }
            set { _newID = value; }
        }

        // ##### Properties #####

        private int _id;

        [XmlAttribute("ID")]
        public int ID
        {
            get { return _id; }
            set {
                _id = value;
                if (_id >= NewID)
                    NewID = _id + 1;
                OnPropertyChanged("ID");
            }
        }

        private string _name;

        [XmlAttribute("Name")]
        public string Name
        {
            get { return _name; }
            set {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private Point3D _point = new Point3D();

        [XmlElement("Point")]
        public Point3D Point
        {
            get { return _point; }
            set {
                _point = value;
                OnPropertyChanged("Point");
            }
        }

        private string _description;

        [XmlAttribute("Description")]
        public string Description
        {
            get { return _description; }
            set {
                _description = value;
                OnPropertyChanged("Description");
            }
        }
        private bool _isOrientated = false;

        [XmlAttribute("IsOrientated")]
        public bool IsOrientated
        {
            get { return _isOrientated; }
            set { _isOrientated = value; }
        }
        private Observation _orientedObservation = new Observation();

        [XmlElement("OrientedObservation")]
        public Observation OrientedObservation
        {
            get { return _orientedObservation; }
            set {
                _orientedObservation = value;
                IsOrientated = true;
            }
        }

        // ##### Constructor #####
        public Station()
        {
            ID = NewID;
            Count++;

        }

        // ##### Events #####

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

    }
    /// <summary>
    /// Represents target.
    /// </summary>
    public class Target
    {
        // ##### Properties #####

        private static int _count = 0;

        public static int Count
        {
            get { return _count; }
            set { _count = value; }
        }
        private static int _newID = 0;

        public static int NewID
        {
            get { return _newID; }
            set { _newID = value; }
        }

        private int _id;

        [XmlAttribute("ID")]
        public int ID
        {
            get { return _id; }
            set {
                _id = value;
                if (_id >= NewID)
                    NewID = _id + 1;
            }
        }
        private TargetMethod _method;

        [XmlAttribute("Method")]
        public TargetMethod Method
        {
            get { return _method; }
            set {
                _method = value;
            }
        }
        private string _name = "";

        [XmlAttribute("Name")]
        public string Name
        {
            get { return _name; }
            set {
                _name = value;
            }
        }

        private Point3D _pointLocal = new Point3D();

        [XmlElement("PointLocal")]
        public Point3D PointLocal
        {
            get { return _pointLocal; }
            set {
                _pointLocal = value;
            }
        }
        private Point3D _point = new Point3D();
        [XmlElement("Point")]
        public Point3D Point
        {
            get { return _point; }
            set {
                _point = value;
            }
        }

        private string _description;

        [XmlAttribute("Description")]
        public string Description
        {
            get { return _description; }
            set {
                _description = value;
            }
        }

        // ##### Constructor #####
        public Target()
        {
            ID = NewID;
            Count++;
        }
    }
    public enum Face
    {
        [XmlEnum(Name = "I+II")]
        I_II = 0,
        [XmlEnum(Name = "I")]
        I = 1,
        [XmlEnum(Name = "II")]
        II = 2
    }
    public enum MState
    {
        [XmlEnum(Name = "Waiting")]
        Waiting = 0,
        [XmlEnum(Name = "Successful")]
        Successful = 1,
        [XmlEnum(Name = "Failure")]
        Failure = 2
    }
    public enum OperationType
    {
        [XmlEnum(Name = "ObservationGroup")]
        ObservationGroup = 0,
        [XmlEnum(Name = "Notification")]
        Notification = 2,
        [XmlEnum(Name = "Pause")]
        Pause = 3
    }
    public enum PauseType
    {
        [XmlEnum(Name = "Delay")]
        Delay = 0,
        [XmlEnum(Name = "StartAt")]
        StartAt = 1
    }
    public enum SynchMeasMode
    {
        [XmlEnum(Name = "Simultaneously")]
        Simultaneously = 0,
        [XmlEnum(Name = "Successively")]
        Successively = 1
    }
    public enum TargetsSortBy
    {
        [XmlEnum(Name = "SortByTarget")]
        SortByTarget = 0,
        [XmlEnum(Name = "SortByHz")]
        SortByHz = 1
    }


}
