using Avalonia.Media.Imaging;
using RDotNet;
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
        string _selectedSerial;

        private DateTime _start;
        private DateTime _end;
        private DateTime _rangeMin;
        private DateTime _rangeMax;

        bool _isLinux = false;

        ObservableCollection<string> _serialNumbers = new ObservableCollection<string>();
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

        public ObservableCollection<string> SerialNumbers
        {
            get
            {
                return _serialNumbers;
            }
            set
            {
                _serialNumbers = value;
                OnPropertyChanged();
            }
        }

        public string SelectedSerial
        {
            get 
            { 
                return _selectedSerial; 
            }
            set 
            {
                _selectedSerial = value;
                GetDistinctSensors();
                OnPropertyChanged();
            }
        }

        public DateTime Start
        {
            get { return _start; }
            set
            {
                _start = value;
                OnPropertyChanged();
            }
        }

        public DateTime End
        {
            get { return _end; }
            set 
            {
                _end = value;
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

            GetDataRange();
        }

        public void GetDistinctSensors()
        {
            SensorTypes.Clear();

            DataTable dt = _data.GetSensorDataBySerial(SelectedSerial);

            var sensorResult = dt.AsEnumerable()
               .Select(values => new
               {
                   sensor = values.Field<string>("SensorType")
               }).Distinct();

            foreach (var value in sensorResult)
            {
                SensorTypes.Add(value.sensor.ToString());
            }
        }

        public void GetDataRange()
        {
            SerialNumbers.Clear();

            DataTable dt = _data.GetAllSensorData();

            var serialResult = dt.AsEnumerable()
                .Select(values => new
                {
                    serial = values.Field<string>("SerialNumber")
                }).Distinct();

            foreach (var value in serialResult)
            {
                SerialNumbers.Add(value.serial.ToString());
            }

            var dateResult = from dates in dt.AsEnumerable()
                             select new
                             {
                                thisDate = dates.Field<DateTime>("DateTime")
                             };

            List<DateTime> dateTimes = new List<DateTime>();

            foreach (var date in dateResult)
            {
                dateTimes.Add(date.thisDate);
            }

            _rangeMin = dateTimes.Min();
            _rangeMax = dateTimes.Max();
        }

        void CreateGraph()
        {
            try
            {
                DataTableToCSV(_data.GetSensorDataBySensorSerialDate(SelectedSensor, SelectedSerial, Start, End));

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
                
                    System.Diagnostics.Process.Start("C:\\Users\\chase.mossing2\\Desktop\\Trad-Data-Monitor-3\\GraphData\\graph.bat");

                    using (Process proc = new Process())
                    {
                        proc.StartInfo.FileName = "C:\\Users\\chase.mossing2\\Desktop\\Trad-Data-Monitor-3\\GraphData\\graph.bat";
                        proc.StartInfo.CreateNoWindow = true;
                        proc.StartInfo.UseShellExecute = false;
                        proc.StartInfo.RedirectStandardOutput = true;
                        proc.Start();
                        proc.WaitForExit();
                    }
                    Graph = new Bitmap("C:\\Users\\chase.mossing2\\Desktop\\Trad-Data-Monitor-3\\GraphData\\graph.png");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(gw, $"An error occured while making the graph. Check that values are entered and try again: \n \n Error Message: \n {ex.Message}", "Graph Error", MessageBox.MessageBoxButtons.Ok);
            }
        }

        // Taken From https://stackoverflow.com/questions/4959722/c-sharp-datatable-to-csv
        void DataTableToCSV(DataTable dt)
        {
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
                File.WriteAllText("C:\\Users\\chase.mossing2\\Desktop\\Trad-Data-Monitor-3\\GraphData\\data.csv", sb.ToString());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
