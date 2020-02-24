using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace TRADDataMonitor
{
    public class GraphWindowViewModel : INotifyPropertyChanged
    {
        GraphWindow gw;

        DataAccessor _data;

        string _selectedSensor;
        string _selectedHub;

        private DateTime _start;
        private DateTime _end;

        bool _isLinux = false;

        ObservableCollection<string> _hubs = new ObservableCollection<string>();
        ObservableCollection<string> _sensorTypes = new ObservableCollection<string>();

        Bitmap _graph;

        public ObservableCollection<string> SensorTypes
        {
            get
            {
                return _sensorTypes;
            }
            set
            {
                _sensorTypes = value;
                OnPropertyChanged();
            }
        }

        public string SelectedSensor
        {
            get
            {
                return _selectedSensor;
            }
            set
            {
                _selectedSensor = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Hubs
        {
            get
            {
                return _hubs;
            }
            set
            {
                _hubs = value;
                OnPropertyChanged();
            }
        }

        public string SelectedHub
        {
            get
            {
                return _selectedHub;
            }
            set
            {
                _selectedHub = value;
                GetSensorsFromHub();
                OnPropertyChanged();
            }
        }

        public DateTime Start
        {
            get { return _start; }
            set
            {
                _start = value;
                if (_start != DateTime.MinValue)
                {
                    GetHubsFromDateRange();
                }
                OnPropertyChanged();
            }
        }

        public DateTime End
        {
            get { return _end; }
            set
            {
                _end = value;
                if (_end != DateTime.MinValue)
                {
                    GetHubsFromDateRange();
                }
                OnPropertyChanged();
            }
        }

        public bool IsLinux
        {
            get { return _isLinux; }
            set
            {
                _isLinux = value;
                OnPropertyChanged();
            }
        }

        public Bitmap Graph
        {
            get
            {
                return _graph;
            }
            set
            {
                _graph = value;
                OnPropertyChanged();
            }
        }

        public GraphWindowViewModel(GraphWindow gw)
        {
            this.gw = gw;

            _data = new DataAccessor();

            InitializeDateRange();
        }

        // called on program load, the values are used to limit the range of dates the 
        // user can select to the date range of the current dataset
        public void InitializeDateRange()
        {
            List<DateTime> dates = _data.GetDateRange();

            Start = dates.Min();
            End = dates.Max();
        }

        // called when a date range is picked, populates the 
        // serial number combobox with only the serials used within the chosen date range
        public void GetHubsFromDateRange()
        {
            Hubs = new ObservableCollection<string>(_data.GetHubsByDate(_start, _end).Distinct());
        }

        // called when a serial number is picked, populates the 
        // sensor type combobox with only the sensors associated with the chosen serial number
        public void GetSensorsFromHub()
        {
            SensorTypes = new ObservableCollection<string>(_data.GetSensorsByHub(_selectedHub, _start, _end).Distinct());
        }

        // makes a .csv file containing the filtered sensor data which is turned into a .png of a graph by python
        public void MakeDataFile()
        {
            DataTable dt = _data.GetSensorData(_selectedHub, _selectedSensor, _start, _end);

            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>()
                .Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            if (_isLinux)
            {
                File.WriteAllText("/home/pi/Trad-Data-Monitor-3/GraphData/data.csv", sb.ToString());
            }
            else
            {
                File.WriteAllText("C:\\Users\\cheze\\Desktop\\TRADDataMonitor\\GraphData\\data.csv", sb.ToString());
            }
        }

        // does what the name says, called when create graph button is clicked. 
        // starts a cmd.exe (or bash session if on linux)
        // that runs a python script to make a graph out of the .csv file made at the beginning of the method
        void CreateGraph()
        {
            try
            {
                MakeDataFile();

                if (_isLinux)
                {
                    using (Process proc = new Process())
                    {
                        proc.StartInfo.FileName = "sh";
                        proc.StartInfo.Arguments = "/home/pi/Trad-Data-Monitor/GraphData/graph.sh";
                        proc.StartInfo.CreateNoWindow = true;
                        proc.StartInfo.UseShellExecute = false;
                        proc.StartInfo.CreateNoWindow = true;
                        proc.StartInfo.RedirectStandardOutput = true;
                        proc.Start();
                        proc.WaitForExit();
                    }
                    Graph = new Bitmap("/home/pi/Trad-Data-Monitor/GraphData/graph.png");
                }
                else
                {
                    using (Process proc = new Process())
                    {
                        proc.StartInfo.FileName = "C:\\Users\\cheze\\Desktop\\TRADDataMonitor\\GraphData\\graph.bat";
                        proc.StartInfo.CreateNoWindow = true;
                        proc.StartInfo.UseShellExecute = false;
                        proc.StartInfo.RedirectStandardOutput = true;
                        proc.Start();
                        proc.WaitForExit();
                    }
                    Graph = new Bitmap("C:\\Users\\cheze\\Desktop\\TRADDataMonitor\\GraphData\\graph.png");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(gw, $"An error occured while making the graph. Check that values are entered and try again: \n \n Error Message: \n {ex.Message}", "Graph Error", MessageBox.MessageBoxButtons.Ok);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
