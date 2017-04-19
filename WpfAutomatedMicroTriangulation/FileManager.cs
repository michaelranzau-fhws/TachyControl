using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;

namespace AutomatedMicroTriangulation
{
    /// <summary>
    /// Manage file communiactions.
    /// </summary>
    public class FileManager
    {
        // ##### Static properties #####

        private static string _currentProjectFileString = @"";

        public static string CurrentProjectFileString
        {
            get { return _currentProjectFileString; }
            set { _currentProjectFileString = value; }
        }
        private static string _currentAppSettingsFileString = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                 "AppSettings.amtset");

        public static string CurrentAppSettingsFileString
        {
            get { return _currentAppSettingsFileString; }
            set { _currentAppSettingsFileString = value; }
        }

        // ##### Static methods #####

        /// <summary>
        /// Load project with path = CurrentProjectFileString.
        /// </summary>
        public static void loadProject()
        {
            if (File.Exists(CurrentProjectFileString))
            {
                try
                {
                    MainWindow.Project.IsProjectLoaded = false;
                    File.Copy(CurrentProjectFileString, CurrentProjectFileString + ".backup", true);
                    XmlSerializer reader = new XmlSerializer(typeof(XMLProject));
                    StreamReader file = new StreamReader(CurrentProjectFileString);
                    Station.Count = 0;
                    Target.Count = 0;
                    Observation.Count = 0;
                    Operation.Count = 0;
                    MainWindow.Project = (XMLProject)reader.Deserialize(file);
                    MainWindow.Project.IsProjectLoaded = true;
                    file.Close();
                    MainWindow.Project.IsSaved = true;
                }
                catch (InvalidOperationException e)
                {
                    MessageBox.Show("Invalid XML project file.\n" +
                        "Message: " + e.Message + "\n" +
                        "Inner exception: " + e.InnerException.Message);
                }
            }
            else
            {
                MessageBox.Show("Invalid file path: " + CurrentProjectFileString);
            }

        }

        /// <summary>
        /// Save project with path = CurrentProjectFileString.
        /// </summary>
        /// <param name="xmlP">Project data</param>
        /// <returns>True = successful</returns>
        public static bool saveProject(XMLProject xmlP)
        {
            if (xmlP.IsSaved == true)
            {
                try
                {
                    XmlSerializer seria = new XmlSerializer(typeof(XMLProject));
                    FileStream fs = new FileStream(CurrentProjectFileString, FileMode.Create);
                    seria.Serialize(fs, xmlP);
                    fs.Close();
                    xmlP.IsSaved = true;

                    return true;
                }
                catch (IOException e)
                {
                    MessageBox.Show("Could not save file");

                }
            }
            else
            {
                return saveProjectTo(xmlP);
            }
            return false;
        }

        /// <summary>
        /// Save project and ask for path with dialog.
        /// </summary>
        /// <param name="xmlP">Project data</param>
        /// <returns>True = successful</returns>
        public static bool saveProjectTo(XMLProject xmlP)
        {
            try
            {

                SaveFileDialog sfd = new SaveFileDialog();

                sfd.Filter = "amt files (*.amt)|*.amt|All files (*.*)|*.*";
                sfd.FilterIndex = 1;
                sfd.RestoreDirectory = true;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    CurrentProjectFileString = sfd.FileName;
                    XmlSerializer seria = new XmlSerializer(typeof(XMLProject));
                    FileStream fs = new FileStream(CurrentProjectFileString, FileMode.Create);
                    seria.Serialize(fs, xmlP);
                    fs.Close();
                    xmlP.IsSaved = true;
                    MainWindow.Project.ProjectPath = Path.GetDirectoryName(CurrentProjectFileString);
                    TSControl.Protocol.ProtocolPath = MainWindow.Project.ProjectPath + @"\Protocol_" + DateTime.Today.ToString("yyyyMMdd") + ".txt";

                    return true;
                }
            }
            catch (IOException e)
            {
                MessageBox.Show("Could not save file");

            }
            return false;
        }

        /// <summary>
        /// Open project with file path dialog.
        /// </summary>
        public static void openProject()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "amt files (*.amt)|*.amt|All files (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                CurrentProjectFileString = ofd.FileName;
                MainWindow.Project.ProjectPath = Path.GetDirectoryName(CurrentProjectFileString);
                TSControl.Protocol.ProtocolPath = MainWindow.Project.ProjectPath + @"\Protocol_" + DateTime.Today.ToString("yyyyMMdd") + ".txt";
                CurrentAppSettingsFileString = MainWindow.Project.ProjectPath + @"\AppSettings.amtset";
                
                loadProject();
            }

        }

        /// <summary>
        /// Load general app settings.
        /// </summary>
        /// <returns>App settings data</returns>
        public static XMLAppSettings loadAppSettings()
        {
            if (File.Exists(CurrentAppSettingsFileString))
            {
                try
                {
                    File.Copy(CurrentAppSettingsFileString, CurrentAppSettingsFileString + ".backup", true);
                    XmlSerializer reader = new XmlSerializer(typeof(XMLAppSettings));
                    StreamReader file = new StreamReader(CurrentAppSettingsFileString);
                    XMLAppSettings xmlAS = (XMLAppSettings)reader.Deserialize(file);
                    xmlAS.IsLoaded = true;
                    file.Close();
                    return xmlAS;
                }
                catch (InvalidOperationException e)
                {
                    //MessageBox.Show("Invalid XML project file.\n" +
                    //    "Message: " + e.Message + "\n" +
                    //    "Inner exception: " + e.InnerException.Message);
                }
            }
            else
            {
                MessageBox.Show("Invalid file path for app settings: " + CurrentAppSettingsFileString);
            }

            return new XMLAppSettings();
        }
        /// <summary>
        /// Save general app settings.
        /// </summary>
        /// <param name="xmlAS">App settings data</param>
        /// <returns>True = successful</returns>
        public static bool saveAppSetting(XMLAppSettings xmlAS)
        {
            try
            {
                XmlSerializer seria = new XmlSerializer(typeof(XMLAppSettings));
                FileStream fs = new FileStream(CurrentAppSettingsFileString, FileMode.Create);
                seria.Serialize(fs, xmlAS);
                fs.Close();

                return true;
            }
            catch (IOException e)
            {
                MessageBox.Show("Could not save file");

            }
            return false;
        }
        /// <summary>
        /// Load targets from file.
        /// Format: Name, x, y, z, Description
        /// Decimal point: .
        /// Seperator: , and tabulator
        /// </summary>
        internal static void loadTargets()
        {
            string[] seperator = { ",", "\t" };

            MessageBox.Show("'Name, x, y, z, Description' \nDecimal point: '.' \nSeperator: , and tabulator");
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "xyz files (*.xyz)|*.xyz|All files (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StreamReader file = new StreamReader(ofd.FileName);

                    // ## Read targets from file.
                    string[] lines = System.IO.File.ReadAllLines(ofd.FileName);

                    foreach (string line in lines)
                    {

                        // ## Extract Target per row
                        string[] split = line.Split(seperator, StringSplitOptions.None);
                        if(split.Length > 0)
                        {
                            int i = 0;
                            Target t = new Target();
                            if(split.Length > 3)
                            {
                                t.Name = split[0];
                                if (split.Length > 4)
                                    t.Description = split[4];
                                i++;
                            }
                            float x = float.Parse(split[i], CultureInfo.InvariantCulture.NumberFormat);
                            float y = float.Parse(split[i + 1], CultureInfo.InvariantCulture.NumberFormat);
                            float z = float.Parse(split[i + 2], CultureInfo.InvariantCulture.NumberFormat);
                            t.PointLocal = new Features.Point3D(x, y, z);
                            // ## Save targets to Project.Targets
                            MainWindow.Project.Targets.Add(t);
                        }
                    }


                    file.Close();
                }
                catch (InvalidOperationException e)
                {
                    MessageBox.Show("Could not read file.\n" +
                        "Message: " + e.Message + "\n" +
                        "Inner exception: " + e.InnerException.Message);
                }
            }
        }
        /// <summary>
        /// Load stations from file.
        /// Format: Name, x, y, z, Description
        /// Decimal point: .
        /// Seperator: , and tabulator
        /// </summary>
        internal static void loadStations()
        {
            string[] seperator = { ",", "\t" };

            MessageBox.Show("'Name, x, y, z, Description' \nDecimal point: '.' \nSeperator: , and tabulator");

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "xyz files (*.xyz)|*.xyz|All files (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StreamReader file = new StreamReader(ofd.FileName);

                    // ## Read stations from file.
                    string[] lines = System.IO.File.ReadAllLines(ofd.FileName);

                    foreach (string line in lines)
                    {

                        // ## Extract stations per row
                        string[] split = line.Split(seperator, StringSplitOptions.None);
                        if (split.Length > 0)
                        {
                            int i = 0;
                            Station s = new Station();
                            if (split.Length > 3)
                            {
                                s.Name = split[0];
                                if (split.Length > 4)
                                    s.Description = split[4];
                                i++;
                            }
                            float x = float.Parse(split[i], CultureInfo.InvariantCulture.NumberFormat);
                            float y = float.Parse(split[i + 1], CultureInfo.InvariantCulture.NumberFormat);
                            float z = float.Parse(split[i + 2], CultureInfo.InvariantCulture.NumberFormat);
                            s.Point = new Features.Point3D(x, y, z);
                            // Save stations to Project.Stations
                            MainWindow.Project.Stations.Add(s);
                        }
                    }


                    file.Close();
                }
                catch (InvalidOperationException e)
                {
                    MessageBox.Show("Could not read file.\n" +
                        "Message: " + e.Message + "\n" +
                        "Inner exception: " + e.InnerException.Message);
                }
            }
        }
    }
}
