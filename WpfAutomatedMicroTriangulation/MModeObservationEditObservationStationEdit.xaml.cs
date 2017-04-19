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

namespace AutomatedMicroTriangulation
{
    /// <summary>
    /// Interaction logic for MModeObservationEditObservationStationEdit.xaml
    /// </summary>
    public partial class MModeObservationEditObservationStationEdit : Window
    {
        // ##### Constructor #####
        public MModeObservationEditObservationStationEdit(ObservationGroupStation obsGrStation)
        {
            InitializeComponent();
            obsGrStation.StationID = obsGrStation.StationID;
            this.DataContext = obsGrStation;
            var cons = (from connnects in MainWindow.Project.Connections
                              where connnects.COM == obsGrStation.COM
                              select connnects).ToArray();
            Connection con;
            if (cons.Count() > 0)
            {
                con = cons[0];
            }
            else
            {
                con = new Connection();
                MainWindow.Project.Connections.Add(con);
            }
            MModeConnectionPanel conPanel = new MModeConnectionPanel(con, true);
            stpMMOGrStationCOM.Children.Add(conPanel);

            Binding b = new Binding("COM");
            b.Source = obsGrStation;
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            conPanel.CmbSetTSSetCOM.SetBinding(ComboBox.SelectedValueProperty, b);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
