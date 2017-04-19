using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AutomatedMicroTriangulation
{
    /// <summary>
    /// Interaction logic for MConceptObservationsEdit.xaml
    /// </summary>
    public partial class MConceptObservationsEdit : Window
    {
        // ##### Properties #####

        private ObservationGroup obsGroup;

        // ##### Constructor #####

        public MConceptObservationsEdit(ObservationGroup observationGroup)
        {
            obsGroup = observationGroup;
            InitializeComponent();
            this.DataContext = obsGroup;
            dgrMConceptObsEdit.ItemsSource = obsGroup.Observations;
            dgcmbcStation.ItemsSource = MainWindow.Project.Stations;
            dgcmbcTarget.ItemsSource = MainWindow.Project.Targets;
            if(obsGroup.Observations.Count() > 0)
            {
                double angel0 = 0;
                foreach(Observation o in obsGroup.Observations)
                {
                    Target t = (from tar in MainWindow.Project.Targets
                               where tar.ID == o.TargetID
                               select tar).ToList()[0];
                    Station s = (from sta in MainWindow.Project.Stations
                                where sta.ID == o.StationID
                                select sta).ToList()[0];
                    o.AngleApprox = new TSControl.Angle( Math.Atan2(s.Point.Y - t.PointLocal.Y, s.Point.X - t.PointLocal.X));
                    o.AngleApprox.Hz -= angel0;
                    if (o.AngleApprox.Hz < 0)
                        o.AngleApprox.HzGon += 400;

                }
            }

            if(MainWindow.Project.Stations.Count == 0 || MainWindow.Project.Targets.Count == 0)
            {
                btnMConceptObsEditAdd.IsEnabled = false;
            }
        }

        // ##### Action methods #####

        private void btnMConceptObsEditDel_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Observation o = (Observation)b.DataContext;
            obsGroup.Observations.Remove(o);

        }

        private void btnMConceptObsEditAdd_Click(object sender, RoutedEventArgs e)
        {

            Observation o = new Observation();
            bool newTargetStationCombinationFound = false;

            // Approx station ID
            int sID = -1;
            var sIDFromStationGr = (from s in MainWindow.Project.Stations
                                    orderby s.ID
                                    select s.ID).ToArray();

            // Approx target ID
            int tID = -1;
            var tIDFromTargets = (from t in MainWindow.Project.Targets
                                  orderby t.ID
                                  select t.ID).ToArray();

            // Set new observation to approximated stationID and targetID
            if (sIDFromStationGr.Count() > 0 && tIDFromTargets.Count() > 0)
            {
                int i = 0;
                int sCount = sIDFromStationGr.Count();
                int j = 0;
                int tCount = tIDFromTargets.Count();
                while (newTargetStationCombinationFound == false)
                {
                    // search in observations for existing combination
                    var isHit = (from obs in obsGroup.Observations
                                 where obs.StationID == sIDFromStationGr[i]
                                 where obs.TargetID == tIDFromTargets[j]
                                 select obs).ToArray();
                    if (isHit.Count() > 0)
                    {
                        j++;
                        if (j >= tCount)
                        {
                            j = 0;
                            i++;
                            if (i >= sCount)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        newTargetStationCombinationFound = true;
                    }
                }
                if(newTargetStationCombinationFound == false)
                {
                    sID = sIDFromStationGr[0];
                    tID = tIDFromTargets[0];
                }
                else
                {
                    sID = sIDFromStationGr[i];
                    tID = tIDFromTargets[j];
                }
                o.StationID = sID;
                o.TargetID = tID;
            }


            obsGroup.Observations.Add(o);

        }

        private void btnMConceptObsEditClearAll_Click(object sender, RoutedEventArgs e)
        {
            obsGroup.Observations.Clear();
        }

        /// <summary>
        /// Edit observation group station settings.
        /// </summary>
        private void btnMConceptObsEditGroupStation_Click(object sender, RoutedEventArgs e)
        {
            Observation obs = (Observation)dgrMConceptObsEdit.SelectedItem;
            ObservationGroupStation obsGrStation = (ObservationGroupStation)(from oGS in obsGroup._ObservationGroupStations
                                                   where oGS.StationID == obs.StationID
                                                   select oGS).ToArray()[0];

            MModeObservationEditObservationStationEdit winObsStationEdit = new MModeObservationEditObservationStationEdit(obsGrStation);
            winObsStationEdit.Show();
        }

        /// <summary>
        /// Set gain and shutter settings to all observations like the first entry in list.
        /// </summary>
        private void btnMConceptObsEditShutterGainForAll_Click(object sender, RoutedEventArgs e)
        {
            int shutter = -1;
            int gain = -1;
            if(obsGroup.Observations.Count() > 1)
            {
                obsGroup.ObsView.MoveCurrentToFirst();
                Observation o = (Observation)obsGroup.ObsView.CurrentItem;
                gain = o.Gain;
                shutter = o.Shutter;
                for (int i = 1; i < obsGroup.Observations.Count(); i++)
                {
                    obsGroup.ObsView.MoveCurrentToNext();
                    o = (Observation)obsGroup.ObsView.CurrentItem;
                    o.Gain = gain;
                    o.Shutter = shutter;
                }
            }
        }
    }

}
