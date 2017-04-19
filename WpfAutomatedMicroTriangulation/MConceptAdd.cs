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

namespace AutomatedMicroTriangulation
{
    /// <summary>
    /// Options bar to add observation groups, notifications ans pauses.
    /// </summary>
    public class MConceptAdd
    {
        // ##### Static properties #####

        private static int _count = 0;

        public static int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        // ##### Properties #####

        private int _id;

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private TreeViewItem tvi;
        /// <summary>
        /// TreeViewItem which represents the options bar.
        /// </summary>
        public TreeViewItem TVI
        {
            get { return tvi; }
            set { tvi = value; }
        }

        // ##### Constructor #####

        public MConceptAdd()
        {
            ID = Count;
            Count++;
            Button observations = new Button();
            observations.Content = "Observations";
            Button pause = new Button();
            pause.Content = "Pause";
            Button notification = new Button();
            notification.Content = "Notification";

            observations.Click += new RoutedEventHandler(ButtonMConceptObservations_Click);
            pause.Click += new RoutedEventHandler(ButtonMConceptPause_Click);
            notification.Click += new RoutedEventHandler(ButtonMConceptNotification_Click);


            Style style = (Style)Application.Current.MainWindow.FindResource("BTN_Add");
            observations.Style = style;
            pause.Style = style;
            notification.Style = style;

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.Children.Add(observations);
            sp.Children.Add(pause);
            sp.Children.Add(notification);


            TVI = new TreeViewItem() { Header = sp };
            style = (Style)Application.Current.MainWindow.FindResource("TVI_Add");
            TVI.Style = style;
        }

        // ##### Action methods #####

        private void ButtonMConceptObservations_Click(object sender, RoutedEventArgs e)
        {
            int insertID;
            insertID = ((TreeView)TVI.Parent).Items.IndexOf(TVI) + 1;

            insertID = (insertID - 1) / 2;
            Operation op = new Operation();
            op.ID = insertID;
            op.Type = OperationType.ObservationGroup;

            MainWindow.Project.MConcept.Operations.Add(op);

        }
        private void ButtonMConceptPause_Click(object sender, RoutedEventArgs e)
        {
            int insertID;
            insertID = ((TreeView)TVI.Parent).Items.IndexOf(TVI) + 1;

            insertID = (insertID - 1) / 2;
            Operation op = new Operation();
            op.ID = insertID;
            op.Type = OperationType.Pause;

            MainWindow.Project.MConcept.Operations.Add(op);
        }
        private void ButtonMConceptNotification_Click(object sender, RoutedEventArgs e)
        {
            int insertID;
            insertID = ((TreeView)TVI.Parent).Items.IndexOf(TVI) + 1;

            insertID = (insertID - 1) / 2;
            Operation op = new Operation();
            op.ID = insertID;
            op.Type = OperationType.Notification;

            MainWindow.Project.MConcept.Operations.Add(op);
        }
    }
    /// <summary>
    /// Shows and handles an observation group
    /// </summary>
    public class MConceptObservations
    {
        // ##### Static properties #####

        private static int count = 0;

        public static int Count
        {
            get { return count; }
            set { count = value; }
        }

        // ##### Properties #####

        private ObservationGroup _obsGroup;

        public ObservationGroup ObsGroup
        {
            get { return _obsGroup; }
            set {
                _obsGroup = value;
                lb.DataContext = _obsGroup;
                dGrid.ItemsSource = _obsGroup.Observations;
            }
        }

        private Label lb = new Label();
        public DataGrid dGrid = new DataGrid();


        private TreeView tv;

        public TreeView TV
        {
            get { return tv; }
            set { tv = value; }
        }

        private TreeViewItem tvi;

        /// <summary>
        /// TreeViewItem which represents the observation group.
        /// </summary>
        public TreeViewItem TVI
        {
            get { return tvi; }
            set { tvi = value; }
        }
        private int id;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        private string _name;

        public string Name
        {
            get { return _name; }
            set {
                _name = value;

            }
        }

        // ##### Constructor #####

        public MConceptObservations()
        {

            DockPanel dp = new DockPanel();

            Button delete = new Button();
            delete.Content = "del";
            DockPanel.SetDock(delete, Dock.Right);

            Button edit = new Button();
            edit.Content = "edit";
            DockPanel.SetDock(edit, Dock.Right);

            edit.Click += new RoutedEventHandler(ButtonMConceptObservationsEdit_Click);
            delete.Click += new RoutedEventHandler(ButtonMConceptObservationsDelete_Click);

            Style style = (Style)Application.Current.MainWindow.FindResource("Button_TreeViewItem");
            delete.Style = style;
            style = (Style)Application.Current.MainWindow.FindResource("Button_TreeViewItem_green");
            edit.Height = 18;
            edit.Width = 50;
            edit.FontSize = 10;
            edit.Style = style;

            Button moveElementDown = new Button();
            style = (Style)Application.Current.MainWindow.FindResource("Button_TreeViewItem_green");
            moveElementDown.Style = style;
            moveElementDown.Height = 17;
            moveElementDown.FontSize = 10;
            moveElementDown.Content = "\u25BC";

            Button moveElementUp = new Button();
            moveElementUp.Style = style;
            moveElementUp.Height = 17;
            moveElementUp.FontSize = 10;
            moveElementUp.Content = "\u25B2";

            moveElementDown.Click += new RoutedEventHandler(ButtonMConceptObservationsMoveElementDown_Click);
            moveElementUp.Click += new RoutedEventHandler(ButtonMConceptObservationsMoveElementUp_Click);


            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;

            TextBlock t1 = new TextBlock();
            t1.SetBinding(TextBlock.TextProperty, new Binding("Name"));

            TextBlock t2 = new TextBlock();
            t2.Text = ", Faces: ";

            TextBlock t3 = new TextBlock();
            t3.SetBinding(TextBlock.TextProperty, new Binding("Faces"));

            TextBlock t4 = new TextBlock();
            t4.Text = ", Repetitions: ";

            TextBlock t5 = new TextBlock();
            t5.SetBinding(TextBlock.TextProperty, new Binding("Repetitions"));

            TextBlock t6 = new TextBlock();
            t6.Text = ", Shots: ";

            TextBlock t7 = new TextBlock();
            t7.SetBinding(TextBlock.TextProperty, new Binding("Shots"));

            sp.Children.Add(t1);
            sp.Children.Add(t2);
            sp.Children.Add(t3);
            sp.Children.Add(t4);
            sp.Children.Add(t5);
            sp.Children.Add(t6);
            sp.Children.Add(t7);
            lb.Content = sp;
            lb.FontWeight = FontWeights.Bold;

            dp.HorizontalAlignment = HorizontalAlignment.Stretch;
            dp.LastChildFill = true;
            dp.Width = 900;
            dp.Children.Add(delete);
            dp.Children.Add(edit);
            dp.Children.Add(moveElementDown);
            dp.Children.Add(moveElementUp);
            dp.Children.Add(lb);

            SolidColorBrush scbPause = new SolidColorBrush();
            scbPause.Color = (Color)ColorConverter.ConvertFromString("#FF7ED158");

            TVI = new TreeViewItem() { Header = dp };

            dGrid.Margin = new Thickness(0);
            dGrid.Background = new SolidColorBrush(Colors.White);
            dGrid.MaxHeight = 400;
            dGrid.HeadersVisibility = DataGridHeadersVisibility.All;
            dGrid.Width = 900;
            dGrid.AutoGenerateColumns = false;
            dGrid.IsReadOnly = true;

            DataGridTextColumn tc0 = new DataGridTextColumn();
            tc0.Header = "ID";
            tc0.Binding = new Binding("ID");
            tc0.SortMemberPath = "ID";
            tc0.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dGrid.Columns.Add(tc0);

            DataGridComboBoxColumn tc1 = new DataGridComboBoxColumn();
            tc1.Header = "Station";
            tc1.SelectedValueBinding = new Binding("StationID");
            tc1.SortMemberPath = "StationID";
            tc1.DisplayMemberPath = "Name";
            tc1.SelectedValuePath = "ID";
            tc1.ItemsSource = MainWindow.Project.Stations;
            tc1.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dGrid.Columns.Add(tc1);

            DataGridComboBoxColumn tc2 = new DataGridComboBoxColumn();
            tc2.Header = "Target";
            tc2.SelectedValueBinding = new Binding("TargetID");
            tc2.SortMemberPath = "TargetID";
            tc2.DisplayMemberPath = "Name";
            tc2.SelectedValuePath = "ID";
            tc2.ItemsSource = MainWindow.Project.Targets;
            tc2.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dGrid.Columns.Add(tc2);

            DataGridComboBoxColumn tc3 = new DataGridComboBoxColumn();
            tc3.Header = "Method";
            tc3.SelectedValueBinding = new Binding("Method");
            tc3.ItemsSource= Enum.GetValues(typeof(TSControl.TargetMethod)).Cast<TSControl.TargetMethod>();
            tc3.SortMemberPath = "Method";
            tc3.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dGrid.Columns.Add(tc3);

            DataGridTextColumn tc4 = new DataGridTextColumn();
            tc4.Header = "Shutter";
            tc4.Binding = new Binding("Shutter");
            tc4.SortMemberPath = "Shutter";
            tc4.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dGrid.Columns.Add(tc4);

            DataGridTextColumn tc5 = new DataGridTextColumn();
            tc5.Header = "Gain";
            tc5.Binding = new Binding("Gain");
            tc5.SortMemberPath = "Gain";
            tc5.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dGrid.Columns.Add(tc5);

            DataGridTextColumn tc6 = new DataGridTextColumn();
            tc6.Header = "Std";
            tc6.Binding = new Binding("Std");
            tc6.SortMemberPath = "Std";
            tc6.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dGrid.Columns.Add(tc6);

            TVI.Items.Add(dGrid);
            TVI.Background = scbPause;
        }

        // ##### Action methods #####

        private void ButtonMConceptObservationsMoveElementUp_Click(object sender, RoutedEventArgs e)
        {
            Operation o = (from ops in MainWindow.Project.MConcept.Operations
                           where ops.IDOfType == ObsGroup.ID &&
                           ops.Type == OperationType.ObservationGroup
                           select ops).ToList()[0];
            int tviID = o.ID;
            if(tviID > 0)
                MainWindow.Project.MConcept.Operations.Move(tviID, tviID - 1);
        }

        private void ButtonMConceptObservationsMoveElementDown_Click(object sender, RoutedEventArgs e)
        {
            Operation o = (from ops in MainWindow.Project.MConcept.Operations
                           where ops.IDOfType == ObsGroup.ID &&
                           ops.Type == OperationType.ObservationGroup
                           select ops).ToList()[0];
            int tviID = o.ID;

            if (tviID < (MainWindow.Project.MConcept.Operations.Count() - 1))
                MainWindow.Project.MConcept.Operations.Move(tviID, tviID + 1);
        }

        private void ButtonMConceptObservationsDelete_Click(object sender, RoutedEventArgs e)
        {
            TV = (TreeView)TVI.Parent;
            Operation o = (from ops in MainWindow.Project.MConcept.Operations
                          where ops.IDOfType == ObsGroup.ID &&
                          ops.Type == OperationType.ObservationGroup
                          select ops).ToList()[0];
            MainWindow.Project.MConcept.Operations.Remove(o);
            MainWindow.Project.MConcept.ObservationGroups.Remove(ObsGroup);
            int tviID = TV.Items.IndexOf(TVI);
            TV.Items.RemoveAt(tviID);
            TV.Items.RemoveAt(tviID);
        }

        private void ButtonMConceptObservationsEdit_Click(object sender, RoutedEventArgs e)
        {
            MConceptObservationsEdit winMConceptObsEdit = new MConceptObservationsEdit(ObsGroup);
            winMConceptObsEdit.Show();
        }
    }

    /// <summary>
    /// Shows and handles a pause.
    /// </summary>
    public class MConceptPause
    {
        // ##### Static properties #####

        private static int count = 0;

        public static int Count
        {
            get { return count; }
            set { count = value; }
        }

        // ##### Properties #####

        private Pause _pauseGroup;

        public Pause PauseGroup
        {
            get { return _pauseGroup; }
            set
            {
                _pauseGroup = value;
                lb.DataContext = _pauseGroup;
            }
        }
        Label lb = new Label();


        private TreeView tv;

        public TreeView TV
        {
            get { return tv; }
            set { tv = value; }
        }

        private TreeViewItem tvi;

        public TreeViewItem TVI
        {
            get { return tvi; }
            set { tvi = value; }
        }
        private int id;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;

            }
        }

        // ##### Constructor #####

        public MConceptPause()
        {
            ID = Count;
            Count++;
            DockPanel dp = new DockPanel();

            Button delete = new Button();
            delete.Content = "del";
            DockPanel.SetDock(delete, Dock.Right);

            Button edit = new Button();
            edit.Content = "edit";
            DockPanel.SetDock(edit, Dock.Right);

            edit.Click += new RoutedEventHandler(ButtonMConceptPauseEdit_Click);
            delete.Click += new RoutedEventHandler(ButtonMConceptPauseDelete_Click);

            Style style = (Style)Application.Current.MainWindow.FindResource("Button_TreeViewItem");
            delete.Style = style;
            edit.Style = style;

            Button moveElementDown = new Button();
            style = (Style)Application.Current.MainWindow.FindResource("Button_TreeViewItem_gray");
            moveElementDown.Style = style;
            moveElementDown.Height = 17;
            moveElementDown.FontSize = 10;
            moveElementDown.Content = "\u25BC";

            Button moveElementUp = new Button();
            moveElementUp.Style = style;
            moveElementUp.Height = 17;
            moveElementUp.FontSize = 10;
            moveElementUp.Content = "\u25B2";

            lb.Content = "Pause: 0:30h";
            lb.FontWeight = FontWeights.Bold;

            dp.HorizontalAlignment = HorizontalAlignment.Stretch;
            dp.LastChildFill = true;
            dp.Width = 900;
            dp.Children.Add(delete);
            dp.Children.Add(edit);
            dp.Children.Add(moveElementDown);
            dp.Children.Add(moveElementUp);
            dp.Children.Add(lb);

            moveElementDown.Click += new RoutedEventHandler(ButtonMConceptPauseMoveElementDown_Click);
            moveElementUp.Click += new RoutedEventHandler(ButtonMConceptPauseMoveElementUp_Click);


            TVI = new TreeViewItem() { Header = dp };

            DataGrid dp2 = new DataGrid();
            dp2.Margin = new Thickness(0);
            dp2.Background = new SolidColorBrush(Colors.White);
            dp2.Height = 50;
            dp2.HeadersVisibility = DataGridHeadersVisibility.All;
            dp2.Width = 900;

            DataGridTextColumn tc1 = new DataGridTextColumn();
            tc1.Header = "Station";
            tc1.Binding = new Binding("Station");
            tc1.SortMemberPath = "Station";
            tc1.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dp2.Columns.Add(tc1);

            DataGridTextColumn tc2 = new DataGridTextColumn();
            tc2.Header = "Target";
            tc2.Binding = new Binding("Target");
            tc2.SortMemberPath = "Target";
            tc2.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dp2.Columns.Add(tc2);

            DataGridTextColumn tc3 = new DataGridTextColumn();
            tc3.Header = "Method";
            tc3.Binding = new Binding("Method");
            tc3.SortMemberPath = "Method";
            tc3.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dp2.Columns.Add(tc3);

            DataGridTextColumn tc4 = new DataGridTextColumn();
            tc4.Header = "Shutter";
            tc4.Binding = new Binding("Shutter");
            tc4.SortMemberPath = "Shutter";
            tc4.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dp2.Columns.Add(tc4);

            DataGridTextColumn tc5 = new DataGridTextColumn();
            tc5.Header = "Gain";
            tc5.Binding = new Binding("Gain");
            tc5.SortMemberPath = "Gain";
            tc5.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dp2.Columns.Add(tc5);

            DataGridTextColumn tc6 = new DataGridTextColumn();
            tc6.Header = "Std";
            tc6.Binding = new Binding("Std");
            tc6.SortMemberPath = "Std";
            tc6.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dp2.Columns.Add(tc6);

            SolidColorBrush scbPause = new SolidColorBrush();
            scbPause.Color = (Color)ColorConverter.ConvertFromString("#FFDDDDDD");
            TVI.Background = scbPause;
            TVI.Items.Add(dp2);
        }

        // ##### Action methods #####
        private void ButtonMConceptPauseMoveElementUp_Click(object sender, RoutedEventArgs e)
        {
            Operation o = (from ops in MainWindow.Project.MConcept.Operations
                           where ops.IDOfType == PauseGroup.ID &&
                           ops.Type == OperationType.Pause
                           select ops).ToList()[0];
            int tviID = o.ID;
            if (tviID > 0)
                MainWindow.Project.MConcept.Operations.Move(tviID, tviID - 1);
        }

        private void ButtonMConceptPauseMoveElementDown_Click(object sender, RoutedEventArgs e)
        {
            Operation o = (from ops in MainWindow.Project.MConcept.Operations
                           where ops.IDOfType == PauseGroup.ID &&
                           ops.Type == OperationType.Pause
                           select ops).ToList()[0];
            int tviID = o.ID;

            if (tviID < (MainWindow.Project.MConcept.Operations.Count() - 1))
                MainWindow.Project.MConcept.Operations.Move(tviID, tviID + 1);
        }
        private void ButtonMConceptPauseEdit_Click(object sender, RoutedEventArgs e)
        {
            MConceptPauseEdit winMConceptObsEdit = new MConceptPauseEdit();
            winMConceptObsEdit.Show();
        }
        private void ButtonMConceptPauseDelete_Click(object sender, RoutedEventArgs e)
        {
            TV = (TreeView)TVI.Parent;
            Operation o = (from ops in MainWindow.Project.MConcept.Operations
                           where ops.IDOfType == PauseGroup.ID &&
                           ops.Type == OperationType.Pause
                           select ops).ToList()[0];
            MainWindow.Project.MConcept.Operations.Remove(o);
            MainWindow.Project.MConcept.Pauses.Remove(PauseGroup);
            int tviID = TV.Items.IndexOf(TVI);
            TV.Items.RemoveAt(tviID);
            TV.Items.RemoveAt(tviID);
        }
    }
    /// <summary>
    /// Shows and handles a notification.
    /// </summary>
    public class MConceptNotification
    {
        // ##### Static properties #####

        private static int count = 0;

        public static int Count
        {
            get { return count; }
            set { count = value; }
        }

        // ##### Properties #####

        private Notification _notificationGroup;

        public Notification NotificationGroup
        {
            get { return _notificationGroup; }
            set
            {
                _notificationGroup = value;
                lb.DataContext = _notificationGroup;
            }
        }
        Label lb = new Label();


        private TreeView tv;

        public TreeView TV
        {
            get { return tv; }
            set { tv = value; }
        }

        private TreeViewItem tvi;

        public TreeViewItem TVI
        {
            get { return tvi; }
            set { tvi = value; }
        }
        private int id;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;

            }
        }

        // ##### Constructor #####

        public MConceptNotification()
        {
            ID = Count;
            Count++;
            DockPanel dp = new DockPanel();

            Button delete = new Button();
            delete.Content = "del";
            DockPanel.SetDock(delete, Dock.Right);

            Button edit = new Button();
            edit.Content = "edit";
            DockPanel.SetDock(edit, Dock.Right);

            edit.Click += new RoutedEventHandler(ButtonMConceptNotificationEdit_Click);
            delete.Click += new RoutedEventHandler(ButtonMConceptNotificationDelete_Click);

            Style style = (Style)Application.Current.MainWindow.FindResource("Button_TreeViewItem");
            delete.Style = style;
            edit.Style = style;

            Button moveElementDown = new Button();
            style = (Style)Application.Current.MainWindow.FindResource("Button_TreeViewItem_gray");
            moveElementDown.Style = style;
            moveElementDown.Height = 17;
            moveElementDown.FontSize = 10;
            moveElementDown.Content = "\u25BC";

            Button moveElementUp = new Button();
            moveElementUp.Style = style;
            moveElementUp.Height = 17;
            moveElementUp.FontSize = 10;
            moveElementUp.Content = "\u25B2";

            moveElementDown.Click += new RoutedEventHandler(ButtonMConceptNotificationMoveElementDown_Click);
            moveElementUp.Click += new RoutedEventHandler(ButtonMConceptNotificationMoveElementUp_Click);

            lb.Content = "Notification: To: michael.ranzau@fhws.de";
            lb.FontWeight = FontWeights.Bold;

            dp.HorizontalAlignment = HorizontalAlignment.Stretch;
            dp.LastChildFill = true;
            dp.Width = 900;
            dp.Children.Add(delete);
            dp.Children.Add(edit);
            dp.Children.Add(moveElementDown);
            dp.Children.Add(moveElementUp);
            dp.Children.Add(lb);

            TVI = new TreeViewItem() { Header = dp };

            DataGrid dp2 = new DataGrid();
            dp2.Margin = new Thickness(0);
            dp2.Background = new SolidColorBrush(Colors.White);
            dp2.Height = 50;
            dp2.HeadersVisibility = DataGridHeadersVisibility.All;
            dp2.Width = 900;

            DataGridTextColumn tc1 = new DataGridTextColumn();
            tc1.Header = "Station";
            tc1.Binding = new Binding("Station");
            tc1.SortMemberPath = "Station";
            tc1.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dp2.Columns.Add(tc1);

            DataGridTextColumn tc2 = new DataGridTextColumn();
            tc2.Header = "Target";
            tc2.Binding = new Binding("Target");
            tc2.SortMemberPath = "Target";
            tc2.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dp2.Columns.Add(tc2);

            DataGridTextColumn tc3 = new DataGridTextColumn();
            tc3.Header = "Method";
            tc3.Binding = new Binding("Method");
            tc3.SortMemberPath = "Method";
            tc3.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dp2.Columns.Add(tc3);

            DataGridTextColumn tc4 = new DataGridTextColumn();
            tc4.Header = "Shutter";
            tc4.Binding = new Binding("Shutter");
            tc4.SortMemberPath = "Shutter";
            tc4.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dp2.Columns.Add(tc4);

            DataGridTextColumn tc5 = new DataGridTextColumn();
            tc5.Header = "Gain";
            tc5.Binding = new Binding("Gain");
            tc5.SortMemberPath = "Gain";
            tc5.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dp2.Columns.Add(tc5);

            DataGridTextColumn tc6 = new DataGridTextColumn();
            tc6.Header = "Std";
            tc6.Binding = new Binding("Std");
            tc6.SortMemberPath = "Std";
            tc6.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dp2.Columns.Add(tc6);

            SolidColorBrush scbPause = new SolidColorBrush();
            scbPause.Color = (Color)ColorConverter.ConvertFromString("#FFDDDDDD");
            TVI.Background = scbPause;
            TVI.Items.Add(dp2);
        }

        // ##### Action methods #####

        private void ButtonMConceptNotificationMoveElementUp_Click(object sender, RoutedEventArgs e)
        {
            Operation o = (from ops in MainWindow.Project.MConcept.Operations
                           where ops.IDOfType == NotificationGroup.ID &&
                           ops.Type == OperationType.Notification
                           select ops).ToList()[0];
            int tviID = o.ID;
            if (tviID > 0)
                MainWindow.Project.MConcept.Operations.Move(tviID, tviID - 1);
        }

        private void ButtonMConceptNotificationMoveElementDown_Click(object sender, RoutedEventArgs e)
        {
            Operation o = (from ops in MainWindow.Project.MConcept.Operations
                           where ops.IDOfType == NotificationGroup.ID &&
                           ops.Type == OperationType.Notification
                           select ops).ToList()[0];
            int tviID = o.ID;

            if (tviID < (MainWindow.Project.MConcept.Operations.Count() - 1))
                MainWindow.Project.MConcept.Operations.Move(tviID, tviID + 1);
        }
        private void ButtonMConceptNotificationDelete_Click(object sender, RoutedEventArgs e)
        {
            TV = (TreeView)TVI.Parent;
            Operation o = (from ops in MainWindow.Project.MConcept.Operations
                           where ops.IDOfType == NotificationGroup.ID &&
                           ops.Type == OperationType.Notification
                           select ops).ToList()[0];
            MainWindow.Project.MConcept.Operations.Remove(o);
            MainWindow.Project.MConcept.Notifications.Remove(NotificationGroup);
            int tviID = TV.Items.IndexOf(TVI);
            TV.Items.RemoveAt(tviID);
            TV.Items.RemoveAt(tviID);
        }
        private void ButtonMConceptNotificationEdit_Click(object sender, RoutedEventArgs e)
        {
            MConceptNotificationEdit winMConceptObsEdit = new MConceptNotificationEdit();
            winMConceptObsEdit.Show();
        }
    }
}
