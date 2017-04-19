using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using TSControl;

namespace AutomatedMicroTriangulation
{
    /// <summary>
    /// Panel to set connection panels settings.
    /// </summary>
    public class SetTSConnectionPanel : Border
    {
        // ##### Properties #####

        public TextBlock txbTSInfo;

        private Connection _con = new Connection();

        public Connection Con
        {
            get { return _con; }
            set { _con = value; }
        }

        // ##### Constructor #####
        public SetTSConnectionPanel(Connection con)
        {
            Con = con;
            buildGUI();
        }

        // ##### Methods #####

        /// <summary>
        /// Build GUI for connection settings.
        /// </summary>
        private void buildGUI()
        {
            this.Child = null;
            // ### Main Border

            Width = 200;
            Height = 330;
            VerticalAlignment = VerticalAlignment.Top;
            Margin = new Thickness(5);
            Background = new SolidColorBrush(Color.FromRgb(0xf7, 0xf7, 0xf7));
            CornerRadius = new CornerRadius(4);
            BorderThickness = new Thickness(1);
            BorderBrush = new SolidColorBrush(Colors.Black);

            // ### Main Child

            DockPanel dp = new DockPanel();

            // ### COMs
            Label lblSetTSSetCOM = new Label();
            lblSetTSSetCOM.DataContext = Con;
            lblSetTSSetCOM.Margin = new Thickness(5, 5, 5, 5);
            DockPanel.SetDock(lblSetTSSetCOM, Dock.Top);
            Binding b = new Binding("COM");
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            lblSetTSSetCOM.SetBinding(Label.ContentProperty, b);

            // ### TS Type

            ComboBox cmbSetTSSetTSType = new ComboBox();
            cmbSetTSSetTSType.DataContext = Con;
            cmbSetTSSetTSType.Margin = new Thickness(5, 0, 5, 5);
            DockPanel.SetDock(cmbSetTSSetTSType, Dock.Top);
            cmbSetTSSetTSType.IsReadOnly = false;
            cmbSetTSSetTSType.ItemsSource = Enum.GetValues(typeof(TotalStationType)).Cast<TotalStationType>();

            b = new Binding("DeviceType");
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            cmbSetTSSetTSType.SetBinding(ComboBox.SelectedValueProperty, b);
            cmbSetTSSetTSType.SelectionChanged += cmbSetTSSetTSType_SelectionChanged;

            // ### Station

            Label lblSetTSSetStation = new Label();
            lblSetTSSetStation.DataContext = Con;
            b = new Binding("Name");
            b.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            lblSetTSSetStation.SetBinding(Label.ContentProperty, b);
            lblSetTSSetStation.Margin = new Thickness(5, 5, 5, 5);
            DockPanel.SetDock(lblSetTSSetStation, Dock.Top);

            // ### TS Info

            txbTSInfo = new TextBlock();
            txbTSInfo.Margin = new Thickness(5, 0, 5, 5);
            txbTSInfo.TextWrapping = TextWrapping.Wrap;
            DockPanel.SetDock(txbTSInfo, Dock.Top);
            txbTSInfo.Inlines.Add("Total Station ");
            TextBlock txbTSInfoDeviceType = new TextBlock();
            b = new Binding("DeviceType");
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            txbTSInfoDeviceType.SetBinding(TextBlock.TextProperty, b);
            txbTSInfo.Inlines.Add(txbTSInfoDeviceType);
            txbTSInfo.Inlines.Add(new LineBreak());
            txbTSInfo.Inlines.Add("Name: ");
            TextBlock txbTSInfoIName = new TextBlock();
            b = new Binding("InstrumentName");
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            txbTSInfoIName.SetBinding(TextBlock.TextProperty, b);
            txbTSInfo.Inlines.Add(txbTSInfoIName);
            txbTSInfo.Inlines.Add(new LineBreak());
            txbTSInfo.Inlines.Add("Camera: ");
            TextBlock txbTSInfoCamera = new TextBlock();
            b = new Binding("Camera");
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            txbTSInfoCamera.SetBinding(TextBlock.TextProperty, b);
            txbTSInfo.Inlines.Add(txbTSInfoCamera);
            txbTSInfo.Inlines.Add(new LineBreak());
            txbTSInfo.Inlines.Add("SerialNumber: ");
            TextBlock txbTSInfoSerial = new TextBlock();
            b = new Binding("SerialNumber");
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            txbTSInfoSerial.SetBinding(TextBlock.TextProperty, b);
            txbTSInfo.Inlines.Add(txbTSInfoSerial);

            // ### BaudRate

            Grid grdBaudRate = new Grid();
            grdBaudRate.DataContext = Con;
            DockPanel.SetDock(grdBaudRate, Dock.Top);
            ColumnDefinition cd1 = new ColumnDefinition();
            cd1.Width = new GridLength(80);
            ColumnDefinition cd2 = new ColumnDefinition();
            cd2.Width = new GridLength(1, GridUnitType.Star);
            grdBaudRate.ColumnDefinitions.Add(cd1);
            grdBaudRate.ColumnDefinitions.Add(cd2);

            Label lblBaudrate = new Label();
            lblBaudrate.Content = "BaudRate:";
            Grid.SetColumn(lblBaudrate, 0);

            ComboBox cmbSetTSSetBaudRate = new ComboBox();
            Grid.SetColumn(cmbSetTSSetBaudRate, 1);
            cmbSetTSSetBaudRate.Margin = new Thickness(5);
            cmbSetTSSetBaudRate.IsReadOnly = false;
            b = new Binding("_BaudRate");
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            cmbSetTSSetBaudRate.SetBinding(ComboBox.SelectedValueProperty, b);
            cmbSetTSSetBaudRate.ItemsSource = Enum.GetValues(typeof(BaudRate)).Cast<BaudRate>();

            grdBaudRate.Children.Add(lblBaudrate);
            grdBaudRate.Children.Add(cmbSetTSSetBaudRate);


            // ### DataBits

            Grid grdDataBits = new Grid();
            grdDataBits.DataContext = Con;
            DockPanel.SetDock(grdDataBits, Dock.Top);
            cd1 = new ColumnDefinition();
            cd1.Width = new GridLength(80);
            cd2 = new ColumnDefinition();
            cd2.Width = new GridLength(1, GridUnitType.Star);
            grdDataBits.ColumnDefinitions.Add(cd1);
            grdDataBits.ColumnDefinitions.Add(cd2);

            Label lblDataBits = new Label();
            lblDataBits.Content = "DataBits:";
            Grid.SetColumn(lblDataBits, 0);

            ComboBox cmbSetTSSetDataBits = new ComboBox();
            Grid.SetColumn(cmbSetTSSetDataBits, 1);
            cmbSetTSSetDataBits.Margin = new Thickness(5);
            cmbSetTSSetDataBits.IsReadOnly = false;
            b = new Binding("_DataBits");
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            cmbSetTSSetDataBits.SetBinding(ComboBox.SelectedValueProperty, b);
            cmbSetTSSetDataBits.ItemsSource = Enum.GetValues(typeof(DataBits)).Cast<DataBits>();

            grdDataBits.Children.Add(lblDataBits);
            grdDataBits.Children.Add(cmbSetTSSetDataBits);

            // ### Parity

            Grid grdParity = new Grid();
            grdParity.DataContext = Con;
            DockPanel.SetDock(grdParity, Dock.Top);
            cd1 = new ColumnDefinition();
            cd1.Width = new GridLength(80);
            cd2 = new ColumnDefinition();
            cd2.Width = new GridLength(1, GridUnitType.Star);
            grdParity.ColumnDefinitions.Add(cd1);
            grdParity.ColumnDefinitions.Add(cd2);

            Label lblParity = new Label();
            lblParity.Content = "Parity:";
            Grid.SetColumn(lblParity, 0);

            ComboBox cmbSetTSSetParity = new ComboBox();
            Grid.SetColumn(cmbSetTSSetParity, 1);
            cmbSetTSSetParity.Margin = new Thickness(5);
            cmbSetTSSetParity.IsReadOnly = false;
            b = new Binding("_Parity");
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            cmbSetTSSetParity.SetBinding(ComboBox.SelectedValueProperty, b);
            cmbSetTSSetParity.ItemsSource = Enum.GetValues(typeof(System.IO.Ports.Parity)).Cast<System.IO.Ports.Parity>();

            grdParity.Children.Add(lblParity);
            grdParity.Children.Add(cmbSetTSSetParity);

            // ### StopBits

            Grid grdStopBits = new Grid();
            grdStopBits.DataContext = Con;
            DockPanel.SetDock(grdStopBits, Dock.Top);
            cd1 = new ColumnDefinition();
            cd1.Width = new GridLength(80);
            cd2 = new ColumnDefinition();
            cd2.Width = new GridLength(1, GridUnitType.Star);
            grdStopBits.ColumnDefinitions.Add(cd1);
            grdStopBits.ColumnDefinitions.Add(cd2);

            Label lblStopBits = new Label();
            lblStopBits.Content = "StopBits:";
            Grid.SetColumn(lblStopBits, 0);

            ComboBox cmbSetTSSetStopBits = new ComboBox();
            Grid.SetColumn(cmbSetTSSetStopBits, 1);
            cmbSetTSSetStopBits.Margin = new Thickness(5);
            cmbSetTSSetStopBits.IsReadOnly = false;
            b = new Binding("_StopBits");
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            cmbSetTSSetStopBits.SetBinding(ComboBox.SelectedValueProperty, b);
            cmbSetTSSetStopBits.ItemsSource = Enum.GetValues(typeof(System.IO.Ports.StopBits)).Cast<System.IO.Ports.StopBits>();

            grdStopBits.Children.Add(lblStopBits);
            grdStopBits.Children.Add(cmbSetTSSetStopBits);

            // ### Buttons

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            DockPanel.SetDock(sp, Dock.Bottom);
            sp.HorizontalAlignment = HorizontalAlignment.Center;
            Button btnTest = new Button();
            btnTest.Content = "Test";
            btnTest.Width = 50;
            btnTest.Height = 20;
            btnTest.Margin = new Thickness(5);
            Button btnDel = new Button();
            btnDel.Content = "Delete";
            btnDel.Width = 40;
            btnDel.Height = 20;
            btnDel.Margin = new Thickness(5);
            Style style = (Style)Application.Current.MainWindow.FindResource("Button_TreeViewItem");
            btnDel.Style = style;
            Button btnDefault = new Button();
            btnDefault.Click += BtnDefault_Click;
            btnDefault.Content = "Default";
            btnDefault.Width = 50;
            btnDefault.Height = 20;
            btnDefault.Margin = new Thickness(5);
            style = (Style)Application.Current.MainWindow.FindResource("Button_TreeViewItem_green");
            btnDefault.Style = style;
            btnDefault.FontSize = 10;
            sp.Children.Add(btnTest);
            sp.Children.Add(btnDefault);

            btnTest.Click += BtnTest_Click;

            // Add elements to Main Child

            dp.Children.Add(lblSetTSSetCOM);
            dp.Children.Add(cmbSetTSSetTSType);
            dp.Children.Add(txbTSInfo);
            dp.Children.Add(grdBaudRate);
            dp.Children.Add(grdDataBits);
            dp.Children.Add(grdParity);
            dp.Children.Add(grdStopBits);
            dp.Children.Add(sp);

            this.Child = dp;
        }

        /// <summary>
        /// Test connection.
        /// </summary>
        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            TSControl.TSControl ts;
            switch (Con.DeviceType)
            {
                case TotalStationType.Leica:
                    LeicaTS lts = new LeicaTS();
                    lts.SerialPortName = Con.COM;
                    lts._SerialPort = Con.getSerialPort();
                    ts = lts;
                    break;
                default:
                    LeicaTS ltsd = new LeicaTS();
                    ltsd.SerialPortName = Con.COM;
                    ltsd._SerialPort = Con.getSerialPort();
                    ts = ltsd;
                    break;
            }
            ts.setConnection(true);

            if (ts.IsConnected == true)
            {
                txbTSInfo.DataContext = ts;
                
                MessageBox.Show("Connection successful.");
                ts.setConnection(false);
            }
            else
            {
                MessageBox.Show("Could not connect.");
            }
        }
        /// <summary>
        /// Set connection settings to default.
        /// </summary>
        private void BtnDefault_Click(object sender, RoutedEventArgs e)
        {
            Con.resetToDefault();
        }
        /// <summary>
        /// Change total stations type.
        /// </summary>
        private void cmbSetTSSetTSType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmbTSType = (ComboBox)sender;
            switch ((TotalStationType)(cmbTSType.SelectedItem))
            {
                case TotalStationType.Leica:
                    Con.DeviceType = TotalStationType.Leica;

                    break;
            }
        }
    }
    /// <summary>
    /// Panel to set connection panels settings in measuring mode.
    /// </summary>
    public class MModeConnectionPanel : Border
    {
        // ##### Properties #####

        private Button btnConnect;
        public TextBlock txbTSInfo;
        private Button _btnEdit;
        private bool _allowCOMChange;

        public bool AllowCOMChange
        {
            get { return _allowCOMChange; }
            set { _allowCOMChange = value; }
        }

        public Button BtnEdit
        {
            get { return _btnEdit; }
            set { _btnEdit = value; }
        }
        private ComboBox _cmbSetTSSetCOM;

        public ComboBox CmbSetTSSetCOM
        {
            get { return _cmbSetTSSetCOM; }
            set { _cmbSetTSSetCOM = value; }
        }
        private Connection _con = new Connection();

        public Connection Con
        {
            get { return _con; }
            set { _con = value; }
        }

        // ##### Constructor #####
        public MModeConnectionPanel(Connection con, bool allowCOMChange = false)
        {
            AllowCOMChange = allowCOMChange;
            //_ObsGroupStation = obsGroupStation;
            //Con = _ObsGroupStation.getConnection();
            Con = con;
            this.DataContext = Con;
            buildGUI();
        }

        // ##### Methods #####

        /// <summary>
        /// Build GUI for connection settings.
        /// </summary>
        private void buildGUI()
        {
            this.Child = null;

            // ### Main Border

            Width = 200;
            Height = 190;
            VerticalAlignment = VerticalAlignment.Top;
            Margin = new Thickness(5);
            Background = new SolidColorBrush(Color.FromRgb(0xf7, 0xf7, 0xf7));
            CornerRadius = new CornerRadius(4);
            BorderThickness = new Thickness(1);
            BorderBrush = new SolidColorBrush(Colors.Black);

            // ### Main Child

            DockPanel dp = new DockPanel();

            // ### COMs
            CmbSetTSSetCOM = new ComboBox();
            CmbSetTSSetCOM.DataContext = Con;
            CmbSetTSSetCOM.Margin = new Thickness(5, 5, 5, 5);
            DockPanel.SetDock(CmbSetTSSetCOM, Dock.Top);
            CmbSetTSSetCOM.IsReadOnly = false;
            CmbSetTSSetCOM.ItemsSource = TSControl.TSControl.getCOMPorts();

            Binding b = new Binding("COM");
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            CmbSetTSSetCOM.SetBinding(ComboBox.SelectedValueProperty, b);
            if (AllowCOMChange == false)
            {
                CmbSetTSSetCOM.IsEnabled = false;
            }

            // ### TS Type

            ComboBox cmbSetTSSetTSType = new ComboBox();
            cmbSetTSSetTSType.DataContext = Con;
            cmbSetTSSetTSType.Margin = new Thickness(5, 0, 5, 5);
            DockPanel.SetDock(cmbSetTSSetTSType, Dock.Top);
            cmbSetTSSetTSType.IsReadOnly = false;
            cmbSetTSSetTSType.ItemsSource = Enum.GetValues(typeof(TotalStationType)).Cast<TotalStationType>();

            b = new Binding("DeviceType");
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            cmbSetTSSetTSType.SetBinding(ComboBox.SelectedValueProperty, b);
            cmbSetTSSetTSType.SelectionChanged += cmbSetTSSetTSType_SelectionChanged;

            // ### TS Info

            txbTSInfo = new TextBlock();
            txbTSInfo.Margin = new Thickness(5, 0, 5, 5);
            txbTSInfo.TextWrapping = TextWrapping.Wrap;
            DockPanel.SetDock(txbTSInfo, Dock.Top);
            txbTSInfo.Inlines.Add("Total Station ");
            TextBlock txbTSInfoDeviceType = new TextBlock();
            b = new Binding("DeviceType");
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            txbTSInfoDeviceType.SetBinding(TextBlock.TextProperty, b);
            txbTSInfo.Inlines.Add(txbTSInfoDeviceType);
            txbTSInfo.Inlines.Add(new LineBreak());
            txbTSInfo.Inlines.Add("Name: ");
            TextBlock txbTSInfoIName = new TextBlock();
            b = new Binding("InstrumentName");
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            txbTSInfoIName.SetBinding(TextBlock.TextProperty, b);
            txbTSInfo.Inlines.Add(txbTSInfoIName);
            txbTSInfo.Inlines.Add(new LineBreak());
            txbTSInfo.Inlines.Add("Camera: ");
            TextBlock txbTSInfoCamera = new TextBlock();
            b = new Binding("Camera");
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            txbTSInfoCamera.SetBinding(TextBlock.TextProperty, b);
            txbTSInfo.Inlines.Add(txbTSInfoCamera);
            txbTSInfo.Inlines.Add(new LineBreak());
            txbTSInfo.Inlines.Add("SerialNumber: ");
            TextBlock txbTSInfoSerial = new TextBlock();
            b = new Binding("SerialNumber");
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            txbTSInfoSerial.SetBinding(TextBlock.TextProperty, b);
            txbTSInfo.Inlines.Add(txbTSInfoSerial);

            // ### Buttons

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            DockPanel.SetDock(sp, Dock.Bottom);
            sp.HorizontalAlignment = HorizontalAlignment.Center;
            btnConnect = new Button();
            btnConnect.Content = "Test";
            btnConnect.Width = 50;
            btnConnect.Height = 20;
            btnConnect.Margin = new Thickness(5);
            BtnEdit = new Button();
            BtnEdit.Content = "edit";
            BtnEdit.Width = 50;
            BtnEdit.Height = 20;
            BtnEdit.Margin = new Thickness(5);
            Style style = (Style)Application.Current.MainWindow.FindResource("Button_TreeViewItem_green");
            BtnEdit.Style = style;
            BtnEdit.FontSize = 10;
            sp.Children.Add(btnConnect);
            sp.Children.Add(BtnEdit);

            btnConnect.Click += BtnConnect_Click;

            // Add elements to Main Child

            dp.Children.Add(CmbSetTSSetCOM);
            dp.Children.Add(cmbSetTSSetTSType);
            dp.Children.Add(txbTSInfo);
            dp.Children.Add(sp);

            this.Child = dp;
        }
        /// <summary>
        /// Test connection.
        /// </summary>
        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            TSControl.TSControl ts;
            switch (Con.DeviceType)
            {
                case TotalStationType.Leica:
                    LeicaTS lts = new LeicaTS();
                    lts.SerialPortName = Con.COM;
                    lts._SerialPort = Con.getSerialPort();
                    ts = lts;
                    break;
                default:
                    LeicaTS ltsd = new LeicaTS();
                    ltsd.SerialPortName = Con.COM;
                    ltsd._SerialPort = Con.getSerialPort();
                    ts = ltsd;
                    break;
            }
            ts.setConnection(true);

            if (ts.IsConnected == true)
            {
                txbTSInfo.DataContext = ts;

                MessageBox.Show("Connection successful.");
                ts.setConnection(false);
            }
            else
            {
                MessageBox.Show("Could not connect.");
            }
        }

        /// <summary>
        /// Change total stations type.
        /// </summary>
        private void cmbSetTSSetTSType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmbTSType = (ComboBox)sender;
            switch ((TotalStationType)(cmbTSType.SelectedItem))
            {
                case TotalStationType.Leica:
                    Con.DeviceType = TotalStationType.Leica;

                    break;
            }
        }
    }
}
