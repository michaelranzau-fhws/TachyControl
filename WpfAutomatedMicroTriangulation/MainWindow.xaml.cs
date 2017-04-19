using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TSControl;
using System.Collections.Specialized;
using Features;
using Emgu.CV.CvEnum;
using Emgu.CV;
using System.IO;

namespace AutomatedMicroTriangulation
{
    /// <summary>
    /// Logig for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // ##### Static properties #####
        private static TreeView _tvMConcept;
        /// <summary>
        /// Offers access to TreeView measuring concept control
        /// </summary>
        public static TreeView TVMConcept
        {
            get { return _tvMConcept; }
            set { _tvMConcept = value; }
        }
        private static TreeView _tvMMode;

        /// <summary>
        /// Offers access to TreeView measuring mode control
        /// </summary>
        public static TreeView TVMMode
        {
            get { return _tvMMode; }
            set { _tvMMode = value; }
        }
        private static WrapPanel _wrpSetTS;
        /// <summary>
        /// Offers access to WrapPanel total station settings
        /// </summary>
        public static WrapPanel WrpSetTS
        {
            get { return _wrpSetTS; }
            set { _wrpSetTS = value; }
        }
        private static TabControl _tabMain;

        /// <summary>
        /// Offers access to main TabControl
        /// </summary>
        public static TabControl TabMain
        {
            get { return _tabMain; }
            set { _tabMain = value; }
        }

        private CultureInfo ci = new CultureInfo("de-DE");

        private static XMLProject _project;
        /// <summary>
        /// Offers access to current loaded project
        /// </summary>
        public static XMLProject Project
        {
            get { return _project; }
            set { _project = value; }
        }

        // ##### Static methods #####

        // ##### Properties #####

        private XMLAppSettings _appSettings;

        /// <summary>
        /// Saves general app settings
        /// </summary>
        public XMLAppSettings AppSettings
        {
            get { return _appSettings; }
            set { _appSettings = value; }
        }
        private string _fullNewProtocol = "";

        /// <summary>
        /// Bind protocol content to user control.
        /// </summary>
        public string FullNewProtocol
        {
            get { return _fullNewProtocol; }
            set
            {
                _fullNewProtocol = value;
                RaisePropertyChanged("FullNewProtocol");
            }
        }

        // ##### Events #####
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }


        // ##### Constructor #####

        public MainWindow()
        {

            InitializeComponent();

            //readProcessExternData();

            TabMain = tabControl_Main;
            TVMConcept = trvMConcept;
            TVMMode = trvMMode;
            WrpSetTS = wrpSetTSSetSP;
            Project = new XMLProject();

            AppSettings = FileManager.loadAppSettings();
            if (AppSettings.IsLoaded)
            {
                //MessageBox.Show("App Settings loaded");
            }
            else
            {
                MessageBox.Show("App Settings not loaded");
            }
            content2GUI();
        }
        /// <summary>
        /// Experimental function to process extern data 
        /// </summary>
        private void readProcessExternData()
        {
            string path = @"\Messung_I\S1001\Targets";

            string[] lines = File.ReadAllLines(
                @"\Messung_I\S1001\Observations_edit.txt");

            string[] sep = { "," };
            foreach (string line in lines)
            {
                string[] paras = line.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                int tID = int.Parse(paras[2]);
                double hz = double.Parse(paras[4], CultureInfo.InvariantCulture);
                double v = double.Parse(paras[5], CultureInfo.InvariantCulture);
                int f = hz < 200 ? 0 : 1;

                // finde target
                string fileName = "f" + f + "s1re0" + tID.ToString("00000") + ".jpg";
                string imagePath = System.IO.Path.Combine(path, fileName);
                Ellipse2D point = FeatureDetection.findEllipse(new Mat(imagePath, ImreadModes.Unchanged), new Ellipse2D(320, 240));

                Angle measuredAngle = new Angle(hz, v, true);


                // Get calibrated / true angles
                LeicaTS_Calibration cali = new LeicaTS_Calibration();
                cali.Parameters.a11 = -000.0006115267 / 200 * Math.PI;
                cali.Parameters.a12 = -000.0000003626 / 200 * Math.PI;
                cali.Parameters.a21 = 000.0000003502 / 200 * Math.PI;
                cali.Parameters.a22 = -000.0006113362 / 200 * Math.PI;
                cali.Parameters.Hz = 157.4525607032 / 200 * Math.PI;
                cali.Parameters.V = 104.4740283453 / 200 * Math.PI;
                cali.Parameters.x = 325.4768;
                cali.Parameters.y = 247.6800;

                Angle trueAngle = TSControl.TSControl.getCorrectedAngle(measuredAngle,
                    new Point2D(point.X, point.Y), cali.Parameters);
                if (point.X < 0)
                {
                    trueAngle.Hz = 0;
                    trueAngle.V = 0;
                }

                // Write measured angles
                StreamWriter file = new StreamWriter(
                    @"\Messung_I\S1001\Measures.txt", true);
                file.WriteLine(
                    "1, " +
                    tID + ", " +
                    trueAngle.HzGon + ", " +
                    trueAngle.VGon);
                file.Close();
            }
        }

        // ##### Methods #####

        /// <summary>
        /// Fill GUI with current project data
        /// </summary>
        private void content2GUI()
        {
            // Project information
            lblPHead.Content = Project.Name + ", " + Project.Author;
            txtPMName.Text = Project.Name;
            txtPMDescription.Text = Project.Description;
            txtPMCreator.Text = Project.Author;
            lblPMNumberStations2.Content = Station.Count;
            lblPMNumberTargets2.Content = Target.Count;
            lblPMNumberObs2.Content = (from obsGr in Project.MMode.ObservationGroups
                                       from obs in obsGr.Observations
                                       where obs.AngleObs.IsMeasured == true
                                       select obs.ID).Count();

            var result = from obsGr in Project.MMode.ObservationGroups
                         from obs in obsGr.Observations
                         where obs.AngleObs.IsMeasured == true
                         select obs.Timestamp;

            if (result.Count() > 0)
            {
                DateTime dt = result.Max();

                if (dt == DateTime.MinValue)
                {
                    lblPMLastObs2.Content = "";
                }
                else
                {
                    lblPMLastObs2.Content = dt.ToString(ci);
                }
            }
            else
            {
                lblPMLastObs2.Content = "";
            }

            // Fill Targets
            dgrTargets.ItemsSource = Project.Targets;

            // Fill Control Points
            dgrControlPoints.ItemsSource = Project.ControlPoints;

            // Fill Stations
            dgrStations.ItemsSource = Project.Stations;

            // Fill Measuring Concept
            fillMeasuringConcept();

            // Fill Measuring Mode
            fillMeasuringMode();

            // Fill Settings TS Settings
            Project.Connections.CollectionChanged += updateConnectionPanels;
            updateConnectionPanels(new object(), new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            Project.IsProjectLoaded = true;

        }
        /// <summary>
        /// Fill GUI with measuring concept data
        /// </summary>
        private void fillMeasuringConcept()
        {
            trvMConcept.Items.Clear();
            trvMConcept.Items.Add(new MConceptAdd().TVI);
            int i = 0;
            foreach (Operation op in Project.MConcept.Operations)
            {
                op.ID = i++;
                TreeViewItem tvi = new TreeViewItem();
                switch (op.Type)
                {
                    case OperationType.ObservationGroup:
                        MConceptObservations mcog = new MConceptObservations();
                        ObservationGroup og = Project.MConcept.ObservationGroups.First(
                            delegate (ObservationGroup obsGroup)
                            {
                                return obsGroup.ID == op.IDOfType;
                            }
                            );
                        mcog.ObsGroup = og;
                        tvi = mcog.TVI;
                        break;
                    case OperationType.Notification:
                        MConceptNotification mcn = new MConceptNotification();
                        Notification n = Project.MConcept.Notifications.First(
                            delegate (Notification notificationGroup)
                            {
                                return notificationGroup.ID == op.IDOfType;
                            }
                            );
                        mcn.NotificationGroup = n;
                        tvi = mcn.TVI;

                        break;
                    case OperationType.Pause:
                        MConceptPause mcp = new MConceptPause();
                        Pause p = Project.MConcept.Pauses.First(
                            delegate (Pause pauseGroup)
                            {
                                return pauseGroup.ID == op.IDOfType;
                            }
                            );
                        mcp.PauseGroup = p;
                        tvi = mcp.TVI;

                        break;
                }
                trvMConcept.Items.Add(tvi);
                trvMConcept.Items.Add(new MConceptAdd().TVI);
            }
        }

        /// <summary>
        /// Fill GUI with measuring mode data
        /// </summary>
        private void fillMeasuringMode()
        {
            trvMMode.Items.Clear();
            trvMMode.Items.Add(new MModeAdd().TVI);
            int i = 0;
            foreach (Operation op in Project.MMode.Operations)
            {
                op.ID = i++;
                TreeViewItem tvi = new TreeViewItem();
                switch (op.Type)
                {
                    case OperationType.ObservationGroup:
                        MModeObservations mcog = new MModeObservations();
                        ObservationGroup og = Project.MMode.ObservationGroups.First(
                            delegate (ObservationGroup obsGroup)
                            {
                                return obsGroup.ID == op.IDOfType;
                            }
                            );
                        foreach(SortDescription sd in og.ObsSortDescription)
                        {
                            og.ObsView.SortDescriptions.Add(sd);
                        }
                        og.ObsView.CollectionChanged += og.ObsView_CollectionChanged;
                        mcog.ObsGroup = og;
                        tvi = mcog.TVI;
                        break;
                    case OperationType.Notification:
                        MModeNotification mcn = new MModeNotification();
                        Notification n = Project.MMode.Notifications.First(
                            delegate (Notification notificationGroup)
                            {
                                return notificationGroup.ID == op.IDOfType;
                            }
                            );
                        mcn.NotificationGroup = n;
                        tvi = mcn.TVI;

                        break;
                    case OperationType.Pause:
                        MModePause mcp = new MModePause();
                        Pause p = Project.MMode.Pauses.First(
                            delegate (Pause pauseGroup)
                            {
                                return pauseGroup.ID == op.IDOfType;
                            }
                            );
                        mcp.PauseGroup = p;
                        tvi = mcp.TVI;

                        break;
                }
                trvMMode.Items.Add(tvi);
                trvMMode.Items.Add(new MModeAdd().TVI);
            }
        }
        /// <summary>
        /// Fill connection setting panels
        /// </summary>
        /// <param name="sender">sender, example: new object()</param>
        /// <param name="e">Event Args, example: new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)</param>
        private void updateConnectionPanels(object sender, NotifyCollectionChangedEventArgs e)
        {
            // ### MMode connections
            // ### Settings TS Settings

            wrpSetTSSetSP.Children.Clear();
            wrpMModeConnect.Children.Clear();

            var connections = from cons in Project.Connections
                              orderby cons.COM
                              orderby cons.COM.Length
                              select cons;


            foreach (Connection con in connections)
            {
                SetTSConnectionPanel connectionPanel = new SetTSConnectionPanel(con);
                wrpSetTSSetSP.Children.Add(connectionPanel);

                MModeConnectionPanel mModeConnectionPanel = new MModeConnectionPanel(con);
                mModeConnectionPanel.BtnEdit.Click += BtnEdit_Click; // Switch to settings panel in TS settings
                wrpMModeConnect.Children.Add(mModeConnectionPanel);
            }
        }

        /// <summary>
        /// Switch to settings panel in TS settings
        /// </summary>
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            tabMainSettings.IsSelected = true;
            tabSetTSSet.IsSelected = true;
        }

        /// <summary>
        /// Save current project when app closes.
        /// </summary>
        private void winMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FileManager.saveProject(Project);
            App.Current.Shutdown();
        }

        /// <summary>
        /// Open and load project.
        /// </summary>
        private void btnPMOpen_Click(object sender, RoutedEventArgs e)
        {
            FileManager.openProject();
            if (Project.IsProjectLoaded)
            {
                content2GUI();
            }
            else
            {
                //MessageBox.Show("No valid project file was chosen.");
            }
        }
        /// <summary>
        /// Save as new project.
        /// </summary>
        private void btnPMNew_Click(object sender, RoutedEventArgs e)
        {
            if(txtPMName.Text != "" && txtPMCreator.Text != "")
            {
                try
                {
                    Project.Author = txtPMCreator.Text;
                    Project.Description = txtPMDescription.Text;
                    Project.Name = txtPMName.Text;
                    FileManager.saveProjectTo(Project);
                    content2GUI();
                    Project.IsProjectLoaded = true;
                }
                catch
                {
                    MessageBox.Show("Could not save Project.");
                }
            }
            else
            {
                MessageBox.Show("Name and Creator must not be empty.");
            }
        }
        /// <summary>
        /// Update project name.
        /// </summary>
        private void txtPMName_LostFocus(object sender, RoutedEventArgs e)
        {
            if(txtPMName.Text != "")
            {
                Project.Name = txtPMName.Text;
                lblPHead.Content = Project.Name + ", " + Project.Author;
            }
            else
            {
                MessageBox.Show("Name must not be empty.");
            }
        }
        /// <summary>
        /// Update creator name.
        /// </summary>
        private void txtPMCreator_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtPMCreator.Text != "")
            {
                Project.Author = txtPMCreator.Text;
            }
            else
            {
                MessageBox.Show("Creator must not be empty.");
            }
        }
        /// <summary>
        /// Update description.
        /// </summary>
        private void txtPMDescription_LostFocus(object sender, RoutedEventArgs e)
        {
            Project.Description = txtPMDescription.Text;
        }

        private void btnTTLoadT_Click(object sender, RoutedEventArgs e)
        {
            FileManager.loadTargets();
        }


        private void btnStationsLS_Click(object sender, RoutedEventArgs e)
        {
            FileManager.loadStations();
        }

        private void btnNewTarget_Click(object sender, RoutedEventArgs e)
        {
            Target t = new Target();
            
            Project.Targets.Add(t);
        }

        private void btnNewCP_Click(object sender, RoutedEventArgs e)
        {
            ControlPoint cp = new ControlPoint();

            Project.ControlPoints.Add(cp);

        }

        private void btnNewStation_Click(object sender, RoutedEventArgs e)
        {
            Station s = new Station();

            Project.Stations.Add(s);

        }

        private void btnTargetDel_Click(object sender, RoutedEventArgs e)
        {
            Project.Targets.Remove((Target)dgrTargets.SelectedItem);
            Target.NewID = Project.Targets.Select(mID => mID.ID).Max() + 1;
        }

        private void btnTargetsCPDel_Click(object sender, RoutedEventArgs e)
        {
            Project.ControlPoints.Remove((ControlPoint)dgrControlPoints.SelectedItem);
            ControlPoint.NewID = Project.ControlPoints.Select(mID => mID.ID).Max() + 1;
        }

        private void btnStationsDel_Click(object sender, RoutedEventArgs e)
        {
            Station s = (Station)dgrStations.SelectedItem;
            int stationsInUse = (from obsGr in Project.MMode.ObservationGroups
                                from obs in obsGr.Observations
                                where obs.StationID == s.ID
                                select obs).Count();

            if(stationsInUse == 0)
            {
                Project.Stations.Remove(s);
                if (Project.Stations.Count > 0)
                {
                    Station.NewID = Project.Stations.Select(mID => mID.ID).Max() + 1;
                }
                else
                {
                    Station.NewID = 0;
                }
            }
            else
            {
                MessageBox.Show(stationsInUse + " stations are in use in observations.");
            }
        }

        private void btnMModeObsClearConcept_Click(object sender, RoutedEventArgs e)
        {
            clearMMode();
        }
        /// <summary>
        /// Make a clean measuring mode.
        /// </summary>
        private void clearMMode()
        {
            Project.MMode.Operations.Clear();
            Project.MMode.Notifications.Clear();
            Project.MMode.Pauses.Clear();
            Project.MMode.ObservationGroups.Clear();
            trvMMode.Items.Clear();
            trvMMode.Items.Add(new MModeAdd().TVI);
        }

        private void btnMModeObsFromMConcept_Click(object sender, RoutedEventArgs e)
        {
            copyFromMConcept2MMode();
        }
        /// <summary>
        /// Copy all elements from measuring concept to measuring mode.
        /// </summary>
        private void copyFromMConcept2MMode()
        {
            clearMMode();
            Project.IsProjectLoaded = false;

            foreach (ObservationGroup ogC in Project.MConcept.ObservationGroups)
            {
                ObservationGroup og = new ObservationGroup();
                og.Faces = ogC.Faces;
                og.Name = ogC.Name;
                og.Repetitions = ogC.Repetitions;
                og.Shots = ogC.Shots;
                og.SortBy = ogC.SortBy;
                og.SynchronousMeasurement = ogC.SynchronousMeasurement;
                og.ID = ogC.ID;
                foreach (Observation oC in ogC.Observations)
                {
                    Observation o = new Observation();
                    o.AngleObs = oC.AngleObs;
                    o.Gain = oC.Gain;
                    o.Method = oC.Method;
                    o.Point = oC.Point;
                    o.SerialNumber = oC.SerialNumber;
                    o.Shutter = oC.Shutter;
                    o.StationID = oC.StationID;
                    o.Std = oC.Std;
                    o.TargetID = oC.TargetID;
                    o.Timestamp = oC.Timestamp;
                    o.ID = oC.ID;
                    o.CalibrationID = oC.CalibrationID;

                    og.Observations.Add(o);
                }

                Project.MMode.ObservationGroups.Add(og);
            }
            foreach(Pause pC in Project.MConcept.Pauses)
            {
                Pause p = new Pause();
                p.Delay = pC.Delay;
                p.ID = pC.ID;
                p.StartAt = pC.StartAt;
                p.Type = pC.Type;

                Project.MMode.Pauses.Add(p);
            }
            foreach(Notification nC in Project.MConcept.Notifications)
            {
                Notification n = new Notification();
                n.Attachments = nC.Attachments;
                n.ContactID = nC.ContactID;
                n.Content = nC.Content;
                n.ID = nC.ID;

                Project.MMode.Notifications.Add(n);
            }
            foreach (Operation opC in Project.MConcept.Operations)
            {
                Operation op = new Operation();
                op.ID = opC.ID;
                op.Type = opC.Type;
                op.IDOfType = opC.IDOfType;

                Project.MMode.Operations.Add(op);
            }

            Project.IsProjectLoaded = true;
            fillMeasuringMode();
        }

        private void btnMModeObsClearMeasurings_Click(object sender, RoutedEventArgs e)
        {
            clearMeasurings();
        }
        /// <summary>
        /// Set all measurings to clean observations.
        /// </summary>
        private void clearMeasurings()
        {
            foreach (ObservationGroup og in Project.MMode.ObservationGroups)
            {
                foreach (Observation o in og.Observations)
                {
                    o.AngleObs = new Angle();
                    o.Point = new Features.Point2D();
                }
            }
        }

        private void btnMModeObsStart_Click(object sender, RoutedEventArgs e)
        {
            btnMModeObsStop.IsEnabled = true;
            btnMModeObsStart.IsEnabled = false;

            // ### first: Station measure: exact position + orientation
            //          Measured before in Tab 'Stations'


            //  go for every operation
            //      go for every Observation Group
            //          go for every Observation 
            //              orderd by Observation Group Station
            //              get TS from Group Station
            //              open connection
            //              set TS to calibration mode settings
            //              Calibration routine
            //              Measure first face
            //              Protocoll measures
            //          go for every Observation
            //              Measure second face
            //              Protocoll measures

            Connection con = new Connection();
            ObservationGroupStation oGrS = new ObservationGroupStation();
            Station s = new Station();
            LeicaTS ts = new LeicaTS();
            LeicaTS_Calibration cali = new LeicaTS_Calibration();

            Operation[] ops = (from op in Project.MMode.Operations
                            select op).ToArray();
            foreach(Operation op in ops)
            {
                switch (op.Type)
                {
                    case OperationType.ObservationGroup:
                        try
                        {
                            ObservationGroup og = (from ogr in Project.MMode.ObservationGroups
                                                   where ogr.ID == op.IDOfType
                                                   select ogr).ToArray()[0];
                            ICollectionView obsView = og.ObsView;
                            Face f = og.Faces;
                            int faceCount = 0;
                            switch (f)
                            {
                                case Face.I:
                                    faceCount = 1;
                                    break;
                                case Face.II:
                                    faceCount = 1;
                                    break;
                                case Face.I_II:
                                    faceCount = 2;
                                    break;
                            }
                            for (int j = 0; j < og.Repetitions; j++)
                            {
                                for (int i = 0; i < faceCount; i++)
                                {
                                    obsView.MoveCurrentToFirst();
                                    for (int l = 0; l < og.Observations.Count(); l++)
                                    {
                                        Observation o = (Observation)obsView.CurrentItem;
                                        if (o.StationID != oGrS.StationID)
                                        {
                                            ts.setConnection(false);
                                            oGrS = (from oGr in og._ObservationGroupStations
                                                    where oGr.StationID == o.StationID
                                                    select oGr).ToArray()[0];
                                            con = (from cons in Project.Connections
                                                   where cons.COM == oGrS.COM
                                                   select cons).ToArray()[0];
                                            s = (from stations in Project.Stations
                                                 where stations.ID == oGrS.StationID
                                                 select stations).ToArray()[0];
                                            ts.SerialPortName = con.COM;
                                            ts._SerialPort = con.getSerialPort();
                                            ts.setConnection(true);
                                        }

                                        for (int k = 0; k < og.Shots; k++)
                                        {
                                            Angle stationToTarget = getAngleFromStationToTarget(o);

                                            // If face II
                                            if (i == 1)
                                            {
                                                stationToTarget.Hz += Math.PI;
                                                stationToTarget.V = 2 * Math.PI - stationToTarget.V;
                                            }

                                            // Do calibration
                                            if (o.CalibrationID < 0)
                                            {
                                                switch (oGrS.CaliType)
                                                {
                                                    case CalibrationType.OnlyOneCalibration:
                                                        // set calibration from oGrS to observation
                                                        if (oGrS.CalibrationID < 0)
                                                        // make new calibration an set it to oGrS
                                                        {
                                                            // select an observation in avarage distance
                                                            var obsStations = (from obs in og.Observations
                                                                               where obs.StationID == oGrS.StationID
                                                                               select obs).ToArray();
                                                            double[] sds = new double[og.Observations.Count()];
                                                            for (int n = 0; n < obsStations.Count(); n++)
                                                            {
                                                                Target t2 = (from targets in Project.Targets
                                                                             where targets.ID == obsStations[n].TargetID
                                                                             select targets).ToArray()[0];
                                                                sds[n] = Math.Sqrt(Math.Pow(t2.PointLocal.X - s.Point.X, 2) + Math.Pow(t2.PointLocal.Y - s.Point.Y, 2));
                                                            }
                                                            double sdAverage = sds.Sum() / og.Observations.Count();
                                                            double nearest = -1;
                                                            int selectedIndex = -1;
                                                            for (int n = 0; n < obsStations.Count(); n++)
                                                            {
                                                                if (Math.Abs(sds[n] - sdAverage) < nearest || nearest < 0)
                                                                {
                                                                    nearest = Math.Abs(sds[n] - sdAverage);
                                                                    selectedIndex = n;
                                                                }
                                                            }
                                                            // Calibrate on target n
                                                            Observation oCali = new Observation();
                                                            oCali.TargetID = obsStations[selectedIndex].TargetID;
                                                            oCali.StationID = oGrS.StationID;
                                                            cali = setCalibrationSettings(oCali);
                                                            cali.TachyLeica = ts;

                                                            if (cali.measureRefImage() == false)
                                                                throw new NotImplementedException();
                                                            if (cali.measureGrid() == false)
                                                                throw new NotImplementedException();
                                                            if (cali.calcCalibration() == false)
                                                                throw new NotImplementedException();
                                                            cali.SerialNumber = ts.SerialNumber;
                                                            Project.Calibrations.Add(cali);
                                                            oGrS.CalibrationID = cali.ID;
                                                        }
                                                        // set calibration from oGrS as current
                                                        o.CalibrationID = oGrS.CalibrationID;
                                                        break;
                                                    case CalibrationType.CalibrationForEveryTarget:
                                                        // Calibrate for every single observation
                                                        if (o.CalibrationID < 0)
                                                        {
                                                            // calibrate
                                                            cali = setCalibrationSettings(o);
                                                            cali.TachyLeica = ts;
                                                            if (cali.measureRefImage() == false)
                                                                throw new NotImplementedException();
                                                            if (cali.measureGrid() == false)
                                                                throw new NotImplementedException();
                                                            if (cali.calcCalibration() == false)
                                                                throw new NotImplementedException();
                                                            cali.SerialNumber = ts.SerialNumber;
                                                            o.CalibrationID = cali.ID;
                                                            Project.Calibrations.Add(cali);
                                                        }
                                                        break;
                                                    case CalibrationType.CalibrationEvery5cm:
                                                        // Check, if a calibration for current range is available and set
                                                        if (o.CalibrationID < 0)
                                                        {
                                                            // set calibrations to every observation
                                                            var obsStations = (from obs in og.Observations
                                                                               where obs.StationID == oGrS.StationID
                                                                               select obs).ToArray();
                                                            double[] sds = new double[og.Observations.Count()];
                                                            double min = 100000;
                                                            double max = -1;
                                                            for (int n = 0; n < obsStations.Count(); n++)
                                                            {
                                                                Target t2 = (from targets in Project.Targets
                                                                             where targets.ID == obsStations[n].TargetID
                                                                             select targets).ToArray()[0];
                                                                sds[n] = Math.Sqrt(Math.Pow(t2.PointLocal.X - s.Point.X, 2) + Math.Pow(t2.PointLocal.Y - s.Point.Y, 2));
                                                                if (sds[n] > max)
                                                                    max = sds[n];
                                                                if (sds[n] < min)
                                                                    min = sds[n];
                                                            }
                                                            double range = max - min;
                                                            int countRange = (int)Math.Ceiling(range / 0.05);
                                                            LeicaTS_Calibration[] calibrations = new LeicaTS_Calibration[countRange];
                                                            for(int n=0; n< countRange; n++)
                                                            {
                                                                calibrations[n] = new LeicaTS_Calibration();
                                                                Project.Calibrations.Add(calibrations[n]);
                                                            }
                                                            for (int n = 0; n < obsStations.Count(); n++)
                                                            {
                                                                int caliIndex = (int)Math.Floor((max - sds[n]) / 0.05);
                                                                obsStations[n].CalibrationID = calibrations[caliIndex].ID;
                                                            }
                                                        }
                                                        // Calibrate, if observation is not calibrated yet
                                                        cali = (LeicaTS_Calibration)(from calis in Project.Calibrations
                                                                where calis.ID == o.CalibrationID
                                                                select calis).ToArray()[0];
                                                        if(cali.Parameters.isCalibrated == false)
                                                        {
                                                            // Calibrate
                                                            cali = setCalibrationSettings(cali, o);
                                                            cali.TachyLeica = ts;

                                                            if (cali.measureRefImage() == false)
                                                                throw new NotImplementedException();
                                                            if (cali.measureGrid() == false)
                                                                throw new NotImplementedException();
                                                            if (cali.calcCalibration() == false)
                                                                throw new NotImplementedException();
                                                            cali.SerialNumber = ts.SerialNumber;
                                                            Project.Calibrations.Add(cali);

                                                        }
                                                        break;
                                                }
                                            }
                                            ts.setAngle(stationToTarget);

                                            // Set focus
                                            if (o.FocusMotorPosition < 0)
                                            {
                                                ts.CAM_AF_SingleShotAutofocus();
                                                var focus = int.Parse(ts.CAM_AF_GetMotorPosition().Parameter[0].ToString());
                                                o.FocusMotorPosition = (int)focus;
                                            }
                                            else
                                            {
                                                ts.CAM_AF_SetMotorPosition(o.FocusMotorPosition);
                                            }
                                            
                                            // save measures
                                            Angle measuredAngle = ts.getAngle();

                                            // make image
                                            // download image
                                            string imagePath = ts.takeImage(CAM_ID_TYPE.CAM_ID_OAC, CAM_RESOLUTION_TYPE.CAM_RES_640x480,
                                                CAM_COMPRESSION_TYPE.CAM_COMP_JPEG, CAM_JPEG_COMPR_QUALITY_TYPE.CAM_JPGQ_BEST,
                                                CAM_ZOOM_FACTOR_TYPE.CAM_ZOOM_4X, ON_OFF_TYPE.OFF, ("f" + i + "s" + s.ID + "re" + j),
                                                o.TargetID, CAM_WB_MANUAL_TYPE.WB_D40, CAM_ISO_SPEED_TYPE.CAM_ISO_A_220, 0.9);

                                            //// finde target
                                            //Ellipse2D point = FeatureDetection.findEllipse(new Mat(imagePath, LoadImageType.Unchanged), new Ellipse2D(320, 240));


                                            //// Save 2D Point
                                            //if (i == 1)
                                            //{
                                            //    o.AngleObsFace2 = measuredAngle;
                                            //    o.PointFace2 = new Point2D(point.X, point.Y, point.Sigma);
                                            //}
                                            //else
                                            //{
                                            //    o.AngleObs = measuredAngle;
                                            //    o.Point = new Point2D(point.X, point.Y, point.Sigma);
                                            //}

                                            //// Get calibrated / true angles
                                            //Angle trueAngle = TSControl.TSControl.getCorrectedAngle(measuredAngle,
                                            //    new Point2D(point.X, point.Y), cali.Parameters);
                                            //if (point.X < 0)
                                            //{
                                            //    trueAngle.Hz = 0;
                                            //    trueAngle.V = 0;
                                            //}
                                            // write Protocol
                                            o.Timestamp = DateTime.Now;
                                            StreamWriter file = new StreamWriter(
                                                System.IO.Path.GetDirectoryName(FileManager.CurrentProjectFileString) + @"\Observations.txt", true);
                                            file.WriteLine(
                                                o.ID + ", " +
                                                o.StationID + ", " +
                                                o.TargetID + ", " +
                                                o.CalibrationID + ", " +
                                                measuredAngle.HzGon + ", " +
                                                measuredAngle.VGon + ", " +
                                                //point.X + ", " +
                                                //point.Y + ", " +
                                                "-1" + ", " +
                                                "-1" + ", " +
                                                o.Timestamp.ToString("yyyyMMddHHmmssfff"));
                                            file.Close();
                                            //// Write measured angles
                                            //file = new StreamWriter(
                                            //    System.IO.Path.GetDirectoryName(FileManager.CurrentProjectFileString) + @"\Measures.txt", true);
                                            //file.WriteLine(
                                            //    o.StationID + ", " +
                                            //    o.TargetID + ", " +
                                            //    trueAngle.HzGon + ", " +
                                            //    trueAngle.VGon + ", ");
                                            //file.Close();
                                        }
                                        obsView.MoveCurrentToNext();
                                    }
                                }
                            }

                        }
                        catch
                        {
                            MessageBox.Show("Something went wrong.");
                        }
                        break;

                }
            }

        }
        /// <summary>
        /// Get Hz and V for station to tartget for currently orienteted total station.
        /// </summary>
        /// <param name="o">Observation</param>
        /// <returns>Angle</returns>
        private Angle getAngleFromStationToTarget(Observation o)
        {
            Angle stationToTarget = new Angle();
            Station s = (from stations in Project.Stations
                         where stations.ID == o.StationID
                         select stations).ToArray()[0];
            Target tOffset = (from targets in Project.Targets
                              where targets.ID == s.OrientedObservation.TargetID
                              select targets).ToArray()[0];
            Target t = (from targets in Project.Targets
                        where targets.ID == o.TargetID
                        select targets).ToArray()[0];

            double offset = s.OrientedObservation.AngleObs.Hz -
                Math.Atan((tOffset.PointLocal.X - s.Point.X) / (tOffset.PointLocal.Y - s.Point.Y));
            double hz = Math.Atan((t.PointLocal.X - s.Point.X) / (t.PointLocal.Y - s.Point.Y));

            stationToTarget.Hz = offset + hz;

            double sd = Math.Sqrt(Math.Pow(t.PointLocal.X - s.Point.X, 2) + Math.Pow(t.PointLocal.Y - s.Point.Y, 2));
            stationToTarget.V = (Math.PI / 2) - Math.Atan((t.PointLocal.Z - s.Point.Z) / sd);

            return stationToTarget;
        }
        /// <summary>
        /// Set standard calibration settings like target and aperture settings.
        /// </summary>
        /// <param name="cali">LeicaTS_Calibration</param>
        /// <param name="o">Observation</param>
        /// <returns>LeicaTS_Calibration</returns>
        private LeicaTS_Calibration setCalibrationSettings(LeicaTS_Calibration cali, Observation o)
        {
            // ## Should work with abstract method ##
            Target t = (from targets in Project.Targets
                        where targets.ID == o.TargetID
                        select targets).ToArray()[0];

            Angle a = getAngleFromStationToTarget(o);

            a = new Angle(157.4562, 104.4789, true);
            // Set calibration angle
            cali.Target = a;
            cali.TargetID = t.ID;

            // Aperture settings
            cali.AutoAperture = ON_OFF_TYPE.OFF;
            cali.CompType = CAM_COMPRESSION_TYPE.CAM_COMP_JPEG;
            cali.ExpTime = 0.9;
            cali.ISO = CAM_ISO_SPEED_TYPE.CAM_ISO_A_220;
            cali.JpgQuality = CAM_JPEG_COMPR_QUALITY_TYPE.CAM_JPGQ_BEST;
            cali.ResolutionType = CAM_RESOLUTION_TYPE.CAM_RES_640x480;
            cali.WB = CAM_WHITE_BALANCE_TYPE.CAM_WB_AUTO;
            cali.WBManualType = CAM_WB_MANUAL_TYPE.WB_D40;
            cali.Zoom = CAM_ZOOM_FACTOR_TYPE.CAM_ZOOM_4X;

            cali.HzVGridPoints = 3;

            return cali;
        }
        /// <summary>
        /// Set standard calibration settings like target and aperture settings. Creates new Calibration.
        /// </summary>
        /// <param name="o">Observation</param>
        /// <returns>LeicaTS_Calibration</returns>
        private LeicaTS_Calibration setCalibrationSettings(Observation o)
        {
            // ## Should work with abstract method ##

            LeicaTS_Calibration cali = new LeicaTS_Calibration();

            return setCalibrationSettings(cali, o);
        }

        private void btnMModeObsStop_Click(object sender, RoutedEventArgs e)
        {
            btnMModeObsStop.IsEnabled = false;
            btnMModeObsStart.IsEnabled = true;
        }

        private void btnStationsMeasure_Click(object sender, RoutedEventArgs e)
        {
            Station s = (Station)dgrStations.SelectedItem;

            StationsMeasure sm = new StationsMeasure(s);
            sm.Show();


        }
        // ##### Methods for experimantal calibration tab #####
        //private void btnCaliConnect_Click(object sender, RoutedEventArgs e)
        //{
        //    if (CaliTS.IsConnected)
        //    {
        //        CaliTS.setConnection(false);
        //        lblCaliConnect.Content = "Not connected";
        //        btnCaliConnect.Content = "Connect";
        //        CaliStep3.IsEnabled = false;
        //        CaliStep4.IsEnabled = false;
        //        CaliStep5.IsEnabled = false;
        //        CaliStep6.IsEnabled = false;
        //    }
        //    else
        //    {
        //        if (cmbCaliSelectStation.SelectedItem != null)
        //        {
        //            CaliTS.setConnection(true);
        //            if (CaliTS.IsConnected)
        //            {
        //                lblCaliConnect.Content = "Connected";
        //                btnCaliConnect.Content = "Disconnect";
        //                CaliStep3.IsEnabled = true;
        //                CaliStep4.IsEnabled = true;
        //                CaliStep5.IsEnabled = true;
        //            }
        //            else
        //            {
        //                MessageBox.Show("Could not connect.");
        //            }
        //        }
        //    }
        //}

        //private void cmbCaliSelectStation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    Station s = (Station)cmbCaliSelectStation.SelectedItem;
        //    CaliTS.SerialPortName = s.COM;
        //    Connection c = s.getConnection();
        //    CaliTS._SerialPort = c.getSerialPort();
        //    btnCaliConnect.IsEnabled = true;
        //}

        //private void btnCaliAimOK_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        imgCaliTemp.ClearValue(Image.SourceProperty);
        //        CaliTS.Calibration.Target = CaliTS.getAngle();
        //        CaliTS.Calibration.measureRefImage();

        //        BitmapImage imgRef = new BitmapImage();
        //        imgRef.BeginInit();
        //        imgRef.CacheOption = BitmapCacheOption.OnLoad;
        //        imgRef.UriSource = new Uri(CaliTS.Calibration.RefImagePath);
        //        imgRef.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
        //        imgRef.EndInit();
        //        imgCaliTemp.Source = imgRef;
        //        rctCaliTempImg.Width = 52.0 / 640 * 195.2;
        //        rctCaliTempImg.Height = 52.0 / 480 * 146.4;
        //        CaliStep6.IsEnabled = true;
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Something went wrong.");
        //    }
        //}

        //private void sldCaliTempExp_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    Slider sld = (Slider)sender;
        //    sld.Value = Math.Round(sld.Value, 2);
        //    CaliTS.CalibrationLeica.ExpTime = sld.Value;
        //}

        //private void sldCaliFieldSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    Slider sld = (Slider)sender;
        //    sld.Value = Math.Round(sld.Value, 2);
        //    CaliTS.Calibration.CaliFieldSize = new Angle(sld.Value, sld.Value, true);
        //}

        //private void sldCaliGridPoints_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    Slider sld = (Slider)sender;
        //    sld.Value = Math.Round(sld.Value, 0);
        //    CaliTS.Calibration.HzVGridPoints = (int)sld.Value;
        //}

        //private void cmbManualAperture_Checked(object sender, RoutedEventArgs e)
        //{
        //    CaliTS.CalibrationLeica.AutoAperture = ON_OFF_TYPE.OFF;
        //}

        //private void cmbManualAperture_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    CaliTS.CalibrationLeica.AutoAperture = ON_OFF_TYPE.ON;
        //}

        //private void cmbCaliISO_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    ComboBox cmbCali = (ComboBox)sender;
        //    if (cmbCali.SelectedItem != null)
        //    {
        //        CaliTS.CalibrationLeica.ISO = (CAM_ISO_SPEED_TYPE)cmbCali.SelectedItem;
        //    }
        //}
        //private void btnCaliCali_Click(object sender, RoutedEventArgs e)
        //{
        //    Button btnCali = (Button)sender;

        //    Protocol.TxtProtocol = txtCaliProtocol;


        //    btnCali.IsEnabled = false;
        //    CaliTS.Calibration.measureGrid();
        //    bool isCalibrated = CaliTS.Calibration.calcCalibration();
        //    if (isCalibrated)
        //    {
        //        // Show results
        //        string gnuPath = System.IO.Path.Combine(MainWindow.Project.ProjectPath, "gnu.dat");
        //        string gnuLine = "";
        //        int max = CaliTS.Calibration.Grid.Count();
        //        double[] xA = new double[max];
        //        double[] yA = new double[max];
        //        double[] xA_delta = new double[max];
        //        double[] yA_delta = new double[max];
        //        if (File.Exists(gnuPath))
        //            File.Delete(gnuPath);
        //        StreamWriter file = new StreamWriter(gnuPath, true);
        //        for (int i = 0; i < max; i++)
        //        {
        //            xA[i] = (CaliTS.Calibration.Grid[i].AngleI.Hz + CaliTS.Calibration.Grid[i].AngleII.Hz - Math.PI) / 2;
        //            yA[i] = (2 * Math.PI - CaliTS.Calibration.Grid[i].AngleII.V + CaliTS.Calibration.Grid[i].AngleI.V) / 2;
        //            Angle delta = TSControl.TSControl.getCorrectedAngle(CaliTS.Calibration.Grid[i].AngleI,
        //                CaliTS.Calibration.Grid[i].PointI, CaliTS.Calibration.Parameters);

        //            Angle a1 = TSControl.TSControl.getCorrectedAngle(
        //                CaliTS.Calibration.Grid[i].AngleI,
        //                CaliTS.Calibration.Grid[i].PointI,
        //                CaliTS.Calibration.Parameters);
        //            Angle a2 = TSControl.TSControl.getCorrectedAngle(
        //                CaliTS.Calibration.Grid[i].AngleII,
        //                CaliTS.Calibration.Grid[i].PointII,
        //                CaliTS.Calibration.Parameters);
        //            Angle a = new Angle(
        //                (a1.Hz + a2.Hz - Math.PI) / 2,
        //                (2 * Math.PI - a2.V + a1.V) / 2);

        //            double x = 640 - (((Math.Atan(Math.Sin(CaliTS.Calibration.Parameters.V) * Math.Tan(CaliTS.Calibration.Parameters.Hz - xA[i]))
        //                - CaliTS.Calibration.Parameters.a12 * (CaliTS.Calibration.Grid[i].PointI.Y - CaliTS.Calibration.Parameters.y))
        //                / CaliTS.Calibration.Parameters.a11) + CaliTS.Calibration.Parameters.x);
        //            double y = 480 - ((CaliTS.Calibration.Parameters.V - CaliTS.Calibration.Parameters.a21 * (CaliTS.Calibration.Grid[i].PointI.X
        //                - CaliTS.Calibration.Parameters.x) - yA[i]) / CaliTS.Calibration.Parameters.a22 + CaliTS.Calibration.Parameters.y);

        //            xA_delta[i] = TSControl.TSControl.rad2gon(a.Hz - CaliTS.Calibration.Parameters.Hz);
        //            if (xA_delta[i] < -100)
        //                xA_delta[i] += 400;
        //            yA_delta[i] = TSControl.TSControl.rad2gon(a.V - CaliTS.Calibration.Parameters.V);

        //            xA_delta[i] *= 50;
        //            yA_delta[i] *= 50;

        //            gnuLine =
        //                CaliTS.Calibration.Grid[i].AngleI.HzGon.ToString().Replace(",", ".") 
        //                + "\t" + CaliTS.Calibration.Grid[i].AngleI.VGon.ToString().Replace(",", ".")
        //                + "\t" + xA_delta[i].ToString().Replace(",", ".") 
        //                + "\t" + yA_delta[i].ToString().Replace(",", ".");
        //            file.WriteLine(gnuLine);

        //        }
        //        file.Close();
        //        GnuPlot.Plot(gnuPath, "using 1:2:3:4 with vectors head filled lt 2");

        //        //CaliTS.Calibration.Parameters
        //    }
        //    else
        //    {
        //        MessageBox.Show("Could not calibrate telescope camera.");
        //    }
        //    btnCali.IsEnabled = true;


        //}

        //private void cmbFeatureSelectStation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    Station s = (Station)cmbFeatureSelectStation.SelectedItem;
        //    CaliTS.SerialPortName = s.COM;
        //    Connection c = s.getConnection();
        //    CaliTS._SerialPort = c.getSerialPort();
        //    btnFeatureConnect.IsEnabled = true;
        //}

        //private void btnFeatureConnect_Click(object sender, RoutedEventArgs e)
        //{
        //    if (CaliTS.IsConnected)
        //    {
        //        CaliTS.setConnection(false);
        //        lblFeatureConnect.Content = "Not connected";
        //        btnFeatureConnect.Content = "Connect";
        //        FeatureStep3.IsEnabled = false;
        //        FeatureStep4.IsEnabled = false;
        //        FeatureStep5.IsEnabled = false;
        //        FeatureStep6.IsEnabled = false;
        //    }
        //    else
        //    {
        //        if (cmbFeatureSelectStation.SelectedItem != null)
        //        {
        //            CaliTS.setConnection(true);
        //            if (CaliTS.IsConnected)
        //            {
        //                lblFeatureConnect.Content = "Connected";
        //                btnFeatureConnect.Content = "Disconnect";
        //                FeatureStep3.IsEnabled = true;
        //                FeatureStep4.IsEnabled = true;
        //                FeatureStep5.IsEnabled = true;
        //            }
        //            else
        //            {
        //                MessageBox.Show("Could not connect.");
        //            }
        //        }
        //    }
        //}

        //private void btnFeatureAimOK_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        imgFeatureTemp.ClearValue(Image.SourceProperty);
        //        CaliTS.Calibration.Target = CaliTS.getAngle();
        //        CaliTS.Calibration.measureRefImage();

        //        BitmapImage imgRef = new BitmapImage();
        //        imgRef.BeginInit();
        //        imgRef.CacheOption = BitmapCacheOption.OnLoad;
        //        imgRef.UriSource = new Uri(CaliTS.Calibration.RefImagePath);
        //        imgRef.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
        //        imgRef.EndInit();
        //        imgFeatureTemp.Source = imgRef;

        //        FeatureStep6.IsEnabled = true;
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Something went wrong.");
        //    }
        //}



        //private void sldFeatureTempExp_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    Slider sld = (Slider)sender;
        //    sld.Value = Math.Round(sld.Value, 2);
        //    CaliTS.CalibrationLeica.ExpTime = sld.Value;
        //}

        //private void cmbFeatureISO_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    ComboBox cmbCali = (ComboBox)sender;
        //    if (cmbCali.SelectedItem != null)
        //    {
        //        CaliTS.CalibrationLeica.ISO = (CAM_ISO_SPEED_TYPE)cmbCali.SelectedItem;
        //    }
        //}

        //private void cmbFeatureManualAperture_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    CaliTS.CalibrationLeica.AutoAperture = ON_OFF_TYPE.ON;
        //}

        //private void cmbFeatureManualAperture_Checked(object sender, RoutedEventArgs e)
        //{
        //    CaliTS.CalibrationLeica.AutoAperture = ON_OFF_TYPE.OFF;
        //}


        //private void btnFeatureFind_Click(object sender, RoutedEventArgs e)
        //{
        //    CaliTS.ShowErrors = false;
        //    TargetMethod m = (TargetMethod)cmbFeatureMethod.SelectedItem;
        //    FeatureMethod = m;

        //    Mat img = new Mat(((BitmapImage)imgFeatureTemp.Source).UriSource.LocalPath, LoadImageType.Unchanged);

        //    switch (FeatureMethod)
        //    {
        //        case TargetMethod.Corner:
        //            Corner2D cornerApprox = new Corner2D(320, 240, -100, 0f, -1000, true);
        //            Corner2D corner = FeatureDetection.findCorner(img, cornerApprox);
        //            FeatureDetection.LineColor = new MCvScalar(100, 100, 100);
        //            FeatureDetection.drawCorner2D(img, corner);
        //            ImageViewer.Show(img);
        //            break;

        //        case TargetMethod.Ellipse:
        //            Ellipse2D eApprox = new Ellipse2D(320, 240);
        //            Ellipse2D ellipse = FeatureDetection.findEllipse(img, eApprox);
        //            img = FeatureDetection.toGray(img);
        //            FeatureDetection.LineColor = new MCvScalar(100, 100, 100);
        //            FeatureDetection.drawEllipse2D(img, ellipse);
        //            ImageViewer.Show(img);
        //            break;

        //        case TargetMethod.Gaussian2D:
        //            Sphere2D[] sphere = FeatureDetection.findSphere(img);
        //            FeatureDetection.LineColor = new MCvScalar(0, 0, 0);
        //            FeatureDetection.drawSphere2D(img, sphere);
        //            ImageViewer.Show(img);
        //            break;

        //        case TargetMethod.SubPixelMatching:
        //            MessageBox.Show("Not supported.");
        //            break;

        //        default:
        //            MessageBox.Show("Not supported.");
        //            break;
        //    }

        //}
    }
    /// <summary>
    /// Refers to MainWindow.xaml and provides project information.
    /// </summary>
    public class ProvidedFunctions
    {
        // ##### Static methods #####
        public static ObservableCollection<Station> getStations()
        {
            return MainWindow.Project.Stations;
        }
        public static bool getNumberOftargetsNotNull()
        {
            return MainWindow.Project.Targets.Count() > 0 ? true : false;
        }
    }
}
