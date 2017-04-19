using Features;
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
using System.Windows.Shapes;
using TSControl;

namespace AutomatedMicroTriangulation
{
    /// <summary>
    /// Measure and calculate stations coordinates from three known points.
    /// </summary>
    public partial class StationsMeasure : Window
    {

        // ##### Properties #####

        private Station _station;

        public Station _Station
        {
            get { return _station; }
            set { _station = value; }
        }

        private Target _t1 = new Target();

        public Target T1
        {
            get { return _t1; }
            set { _t1 = value; }
        }
        private Target _t2 = new Target();

        public Target T2
        {
            get { return _t2; }
            set { _t2 = value; }
        }
        private Target _t3 = new Target();

        public Target T3
        {
            get { return _t3; }
            set { _t3 = value; }
        }

        // ##### Constructor #####

        public StationsMeasure(Station s)
        {
            _Station = s;

            InitializeComponent();


            cmbStationMeasureTarget1.ItemsSource = MainWindow.Project.Targets;
            cmbStationMeasureTarget1.DataContext = T1;

            cmbStationMeasureTarget2.DataContext = T2;
            cmbStationMeasureTarget2.ItemsSource = MainWindow.Project.Targets;

            cmbStationMeasureTarget3.DataContext = T3;
            cmbStationMeasureTarget3.ItemsSource = MainWindow.Project.Targets;

        }

        // ##### Action methods #####

        private void cmbStationMeasureCOM_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBox cmbSender = (ComboBox)sender;
            if(cmbSender.SelectedItem != null)
            {
                var cons = (from connnects in MainWindow.Project.Connections
                            where connnects.COM == cmbSender.SelectedItem.ToString()
                            select connnects).ToArray();
                Connection con;
                if (cons.Count() > 0)
                {
                    con = cons[0];
                }
                else
                {
                    con = new Connection();
                    con.COM = cmbSender.SelectedValue.ToString();
                    MainWindow.Project.Connections.Add(con);
                }
                MModeConnectionPanel conPanel = new MModeConnectionPanel(con);
                cmbStationMeasureCOM.IsEnabled = false;
                stpStationMeasureConPanel.Children.Add(conPanel);
            }
        }

        /// <summary>
        /// Measurs and calculates the stationing
        /// </summary>
        private void btnStationMeasureMeasure_Click(object sender, RoutedEventArgs e)
        {
            // Have to be implemented as abstract total station method
            LeicaTS ts = new LeicaTS();
            try
            {
                Connection con = (from c in MainWindow.Project.Connections
                                  where c.COM == cmbStationMeasureCOM.SelectedItem.ToString()
                                  select c).ToArray()[0];
                ts.SerialPortName = con.COM;
                ts._SerialPort = con.getSerialPort();
                ts.setConnection(true);

                // Do measure T1, T2, (2nd face)
                T1 = (Target)cmbStationMeasureTarget1.SelectedItem;
                T2 = (Target)cmbStationMeasureTarget2.SelectedItem;
                T3 = (Target)cmbStationMeasureTarget3.SelectedItem;


                MessageBox.Show("Turn total station to target 1, face 1.");
                Angle t1_f1 = ts.getAngle();
                Observation o1 = new Observation();
                o1.TargetID = T1.ID;
                o1.StationID = _Station.ID;
                o1.AngleObs = t1_f1;

                MessageBox.Show("Turn total station to target 2, face 1.");
                Angle t2_f1 = ts.getAngle();
                Observation o2 = new Observation();
                o2.TargetID = T2.ID;
                o2.StationID = _Station.ID;
                o2.AngleObs = t2_f1;

                MessageBox.Show("Turn total station to target 3, face 1.");
                Angle t3_f1 = ts.getAngle();
                Observation o3 = new Observation();
                o3.TargetID = T3.ID;
                o3.StationID = _Station.ID;
                o3.AngleObs = t3_f1;

                // Calculate station
                Point3D pStation = GeoCalc.resection(T1.PointLocal, T2.PointLocal, T3.PointLocal, o1.AngleObs, o2.AngleObs, o3.AngleObs);

                // Show results
                if (MessageBox.Show("Save new station coordinates: " + pStation.ToString() + "?",
                    "Save?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _Station.Point = pStation;
                    _Station.OrientedObservation = o1;
                    _Station.OrientedObservation.SerialNumber = ts.SerialNumber;
                    _Station.IsOrientated = true;
                }
                else
                {
                }
                ts.setConnection(false);

            }
            catch
            {
                MessageBox.Show("Something went wrong.");
            }
        }
    }
}
