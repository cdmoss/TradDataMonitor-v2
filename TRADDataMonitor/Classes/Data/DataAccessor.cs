using System;
using System.Collections.Generic;
using System.Text;
// Added
using System.Data.SQLite;
using System.IO;
using System.Net.Sockets;
using System.Data;
using System.ComponentModel;
using System.Net.Mail;
using System.Net;
using TRADDataMonitor.SensorTypes;
using System.Collections.ObjectModel;
using System.Net.Http;
using HtmlAgilityPack;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;

namespace TRADDataMonitor
{
    public class DataAccessor
    {
        #region private variables
        // Variables for the sensor connections and wireless connection

        // SQLite connection string and the SQLiteConnection object that uses the connection string
        private string _tradDBConnectionString = "Data Source=TradDB.sqlite;Version=3";
        private SQLiteConnection _tradDBConn;
        #endregion

        #region public properties
        // Properties for sending emails, setting sensor thresholds, etc.

        #region email
        public string  RecipientEmailAddress { get; set; }
        public string SenderEmailAddress { get; set; }
        public string SenderEmailPassword { get; set; }
        public string SenderEmailSmtpAddress { get; set; }
        public int SenderEmailSmtpPort { get; set; }
        #endregion

        #region thresholds
        public double MinSoilTemperature { get; set; }
        public double MinAirTemperature { get; set; }
        public double MinHumidity { get; set; }
        public double MinMoisture { get; set; }
        public double MinOxygen { get; set; }
        public double MinVOC { get; set; }
        public double MinCO2 { get; set; }
        public double MaxSoilTemperature { get; set; }
        public double MaxAirTemperature { get; set; }
        public double MaxHumidity { get; set; }
        public double MaxMoisture { get; set; }
        public double MaxOxygen { get; set; }
        public double MaxCO2 { get; set; }
        public double MaxVOC { get; set; }
        #endregion

        #region misc
        public int DataCollectionIntervalTime { get; set; }
        public bool GpsEnabled { get; set; }
        public ItemsChangeObservableCollection<VintHub> VintHubs { get; set; }
        #endregion

        #endregion

        #region contructor
        // Constructor; takes no parameters
        public DataAccessor()
        {
            // Instantiates the SQLiteConnection with the connection string when the class itself is instantiated
            _tradDBConn = new SQLiteConnection(_tradDBConnectionString);
        }
        #endregion

        // TODO: Integrate entity framework once we learn it in class
        #region public methods

        #region create sensors and hubs
        // Creates a phidget class based on the configuration combo box and assigns it to the correct hub port
        public PhidgetSensor CreateSensor(string sensorName, int hubPort, string serial, string hubName, bool wireless)
        {
            PhidgetSensor ret = null;
            switch (sensorName)
            {
                case "Moisture":
                    ret = new MyMoistureSensor(hubPort, sensorName, hubName, serial, MinMoisture, MaxMoisture, wireless);
                    break;
                // TODO: Replace the light sensor with the PH sensor and set an actual min and max threshold
                case "Light":
                    ret = new MyLightSensor(hubPort, sensorName, hubName, serial, -1, -1, wireless);
                    break;
                case "Humidity/Air Temperature":
                    ret = new MyHumidityAirTemperatureSensor(hubPort, sensorName, hubName, serial, MinHumidity, MaxHumidity, MinAirTemperature, MaxAirTemperature, wireless);
                    break;
                case "Oxygen":
                    ret = new MyOxygenSensor(hubPort, sensorName, hubName, serial, MinOxygen, MaxOxygen, wireless);
                    break;
                case "Soil Temperature":
                    ret = new MySoilTemperatureSensor(hubPort, sensorName, hubName, serial, MinSoilTemperature, MaxSoilTemperature, wireless);
                    break;
                case "None":
                    ret = new PhidgetSensor(hubPort, "None", hubName, "none", 0, 0, true);
                    break;
            }
            return ret;
        }

        // Creates a new empty hub that can be filled with Phidget sensors as needed
        public VintHub CreateNewHub(string serial)
        { 
            PhidgetSensor[] sensors = {
                CreateSensor("None", 0, serial, "New Hub", true),
                CreateSensor("None", 1, serial, "New Hub", true),
                CreateSensor("None", 2, serial, "New Hub", true),
                CreateSensor("None", 3, serial, "New Hub", true),
                CreateSensor("None", 4, serial, "New Hub", true),
                CreateSensor("None", 5, serial, "New Hub", true) };

            VintHub ret = new VintHub(sensors, true, "New Hub", serial);

            return ret;
        }
        #endregion

        #region configuration to/from database
        // Loads the configuration for the general configuration (ie the email config) and the sensor configuration
        public string LoadConfiguration()
        {
            string result = "";

            // SQLite commands to select everything from the GeneralConfig table
            string getGeneralConfigQuery = $@"select * from GeneralConfig";
            // SQLite commands to select everything from the GeneralConfig table
            string getVintConfig = "select * from VintHubConfig";

            // Checks if a config exsists
            if (CheckForConfig())
            {
                try
                {
                    // If it does open the connection to the database
                    _tradDBConn.Open();

                    // Reads through the database and set the values in the respective public property
                    using (SQLiteCommand cmd = new SQLiteCommand(getGeneralConfigQuery, _tradDBConn))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader[0].GetType().ToString() != "System.DBNull")
                                    RecipientEmailAddress = reader.GetString(0);

                                if (reader[1].GetType().ToString() != "System.DBNull")
                                    SenderEmailAddress = reader.GetString(1);

                                if (reader[2].GetType().ToString() != "System.DBNull")
                                    SenderEmailPassword = reader.GetString(2);

                                if (reader[3].GetType().ToString() != "System.DBNull")
                                    SenderEmailSmtpAddress = reader.GetString(3);

                                if (reader[4].GetType().ToString() != "System.DBNull")
                                    SenderEmailSmtpPort = reader.GetInt32(4);

                                if (reader[5].GetType().ToString() != "System.DBNull")
                                    DataCollectionIntervalTime = reader.GetInt32(5);

                                if (reader[6].GetType().ToString() != "System.DBNull")
                                    MinSoilTemperature = reader.GetDouble(6);

                                if (reader[7].GetType().ToString() != "System.DBNull")
                                    MinAirTemperature = reader.GetDouble(7);

                                if (reader[8].GetType().ToString() != "System.DBNull")
                                    MinHumidity = reader.GetDouble(8);

                                if (reader[9].GetType().ToString() != "System.DBNull")
                                    MinMoisture = reader.GetDouble(9);

                                if (reader[10].GetType().ToString() != "System.DBNull")
                                    MinOxygen = reader.GetDouble(10);

                                if (reader[11].GetType().ToString() != "System.DBNull")
                                    MinCO2 = reader.GetDouble(11);

                                if (reader[12].GetType().ToString() != "System.DBNull")
                                    MinVOC = reader.GetDouble(12);

                                if (reader[13].GetType().ToString() != "System.DBNull")
                                    MaxSoilTemperature = reader.GetDouble(13);

                                if (reader[14].GetType().ToString() != "System.DBNull")
                                    MaxAirTemperature = reader.GetDouble(14);

                                if (reader[15].GetType().ToString() != "System.DBNull")
                                    MaxHumidity = reader.GetDouble(15);

                                if (reader[16].GetType().ToString() != "System.DBNull")
                                    MaxMoisture = reader.GetDouble(16);

                                if (reader[17].GetType().ToString() != "System.DBNull")
                                    MaxOxygen = reader.GetDouble(17);

                                if (reader[18].GetType().ToString() != "System.DBNull")
                                    MaxVOC = reader.GetDouble(18);

                                if (reader[19].GetType().ToString() != "System.DBNull")
                                    MaxCO2 = reader.GetDouble(19);

                                if (reader[20].GetType().ToString() != "System.DBNull")
                                    GpsEnabled = reader.GetBoolean(20);
                            }
                        }
                    }
                    VintHubs = new ItemsChangeObservableCollection<VintHub>();

                    // Reads through the database and creates the sensors for the vint hub based on what it finds
                    using (SQLiteCommand cmd = new SQLiteCommand(getVintConfig, _tradDBConn))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            // If the database is empty then it creates a default hub which is empty (ie has no sensors)
                            if (!reader.HasRows)
                            {
                                VintHubs.Add(CreateNewHub("none"));                                
                            }
                            // If the databse is not empty then it reads the values and creates the corresponding hubs with the correct sensors
                            else
                            {
                                while (reader.Read())
                                {
                                    string hubName = reader.GetString(0);
                                    string serial = reader.GetString(1);

                                    PhidgetSensor[] sensors = new PhidgetSensor[6];
                                    for (int i = 0; i < 6; i++)
                                    {
                                        PhidgetSensor sensor = CreateSensor(reader.GetString(i + 2), i, serial, hubName, reader.GetBoolean(8));
                                        sensors[i] = sensor;
                                    }

                                    VintHub newVint = new VintHub(sensors, reader.GetBoolean(8), hubName, serial);
                                    VintHubs.Add(newVint);
                                }
                            } 
                        }
                    }
                    _tradDBConn.Close();

                    // Return good as the database was succesfully read
                    result = "good";
                }
                // If an error occurs return the error message and close the connection
                catch (Exception ex)
                {
                    result = ex.Message;
                    _tradDBConn.Close();
                    throw;
                }
            }   
            return result;
        }

        // Saves the current configuration to the configuration database
        public string SaveConfiguration()
        {
            string result = "";

            // Query to create a new GeneralConfig table
            // TODO: Replace the string query with a LINQ query
            string createGeneralConfigQuery = $@"insert into GeneralConfig
                                                    values(
                                                        @RecipientEmail,
                                                        @SenderEmail, 
                                                        @SenderPassword, 
                                                        @SMTPAddress, 
                                                        @SMTPPort, 
                                                        @DataInterval, 
                                                        @MinSoilTemperature,
                                                        @MinAirTemperature,
                                                        @MinHumidity,
                                                        @MinMoisture,
                                                        @MinOxygen,
                                                        @MinCO2,
                                                        @MinVOC,                                                                                                                
                                                        @MaxSoilTemperature,
                                                        @MaxAirTemperature,
                                                        @MaxHumidity,
                                                        @MaxMoisture,
                                                        @MaxOxygen,
                                                        @MaxVOC,
                                                        @MaxVOC,    
                                                        @Gps
                                                        )";

            // Query to update the GeneralConfig table
            // TODO: Replace the string query with a LINQ query
            string updateGeneralConfigQuery = $@"update GeneralConfig
                                                    set 
                                                    RecipientEmail = @RecipientEmail,
                                                    SenderEmail = @SenderEmail,
                                                    SenderPassword = @SenderPassword,
                                                    SMTPAddress = @SMTPAddress,
                                                    SMTPPort = @SMTPPort,
                                                    DataInterval = @DataInterval,
                                                    MinSoilTemperature = @MinSoilTemperature,
                                                    MinAirTemperature = @MinAirTemperature,
                                                    MinHumidity = @MinHumidity,
                                                    MinMoisture = @MinMoisutre,
                                                    MinOxygen = @MinOxygen,
                                                    MinVOC = @MinVOC,
                                                    MinCO2 = @MinCO2,   
                                                    MaxSoilTemperature = @MaxSoilTemperature,
                                                    MaxAirTemperature = @MaxAirTemperature,
                                                    MaxHumidity = @MaxHumidity,
                                                    MaxMoisture = @MaxMoisture,
                                                    MaxOxygen = @MaxOxygen,
                                                    MaxVOC = @MaxVOC,
                                                    MaxCO2 = @MaxCO2,
                                                    Gps = @Gps";

            // Qery to create new HubConfig table
            // TODO: Replace the string query with a LINQ query
            string createVintHubConfigQuery = $@"insert into VintHubConfig (HubName, SerialNumber, Port0, Port1, Port2, Port3, Port4, Port5, Wireless) 
                                                    values(
                                                        @HubName,
                                                        @Serial,
                                                        @Port0,
                                                        @Port1, 
                                                        @Port2, 
                                                        @Port3, 
                                                        @Port4, 
                                                        @Port5, 
                                                        @Wireless);";

            try
            {
                SQLiteCommand generalConfigCmd;

                List<SQLiteCommand> vintConfigCmds = new List<SQLiteCommand>();

                // Make sure tables are built before inserting data
                BuildDataStores();

                //if (!CheckForConfig())
                //{

                 _tradDBConn.Open();

                // Clears all the hub configurations
                SQLiteCommand deleteGeneralCmd = new SQLiteCommand("delete from GeneralConfig", _tradDBConn);
                deleteGeneralCmd.ExecuteNonQuery();

                generalConfigCmd = new SQLiteCommand(createGeneralConfigQuery, _tradDBConn);

                //}
                //else
                //{
                //    _tradDBConn.Open();

                //    generalConfigCmd = new SQLiteCommand(updateGeneralConfigQuery, _tradDBConn);
                //}

                using (generalConfigCmd)
                {
                    // Parameters for the queries, used to prevent SQLinjections
                    generalConfigCmd.Parameters.AddWithValue("@RecipientEmail", RecipientEmailAddress);
                    generalConfigCmd.Parameters.AddWithValue("@SenderEmail", SenderEmailAddress);
                    generalConfigCmd.Parameters.AddWithValue("@SenderPassword", SenderEmailPassword);
                    generalConfigCmd.Parameters.AddWithValue("@SMTPAddress", SenderEmailSmtpAddress);
                    generalConfigCmd.Parameters.AddWithValue("@SMTPPort", SenderEmailSmtpPort);
                    generalConfigCmd.Parameters.AddWithValue("@DataInterval", DataCollectionIntervalTime);
                    generalConfigCmd.Parameters.AddWithValue("@MinSoilTemperature", MinSoilTemperature);
                    generalConfigCmd.Parameters.AddWithValue("@MinAirTemperature", MinAirTemperature);
                    generalConfigCmd.Parameters.AddWithValue("@MinHumidity", MinHumidity);
                    generalConfigCmd.Parameters.AddWithValue("@MinMoisture", MinMoisture);
                    generalConfigCmd.Parameters.AddWithValue("@MinOxygen", MinOxygen);
                    generalConfigCmd.Parameters.AddWithValue("@MinVOC", MinVOC);
                    generalConfigCmd.Parameters.AddWithValue("@MinCO2", MinCO2);
                    generalConfigCmd.Parameters.AddWithValue("@MaxSoilTemperature", MaxSoilTemperature);
                    generalConfigCmd.Parameters.AddWithValue("@MaxAirTemperature", MaxAirTemperature);
                    generalConfigCmd.Parameters.AddWithValue("@MaxHumidity", MaxHumidity);
                    generalConfigCmd.Parameters.AddWithValue("@MaxMoisture", MaxMoisture);
                    generalConfigCmd.Parameters.AddWithValue("@MaxOxygen", MaxOxygen);
                    generalConfigCmd.Parameters.AddWithValue("@MaxVOC", MaxVOC);
                    generalConfigCmd.Parameters.AddWithValue("@MaxCO2", MaxCO2);
                    generalConfigCmd.Parameters.AddWithValue("@Gps", GpsEnabled);
                    
                    generalConfigCmd.ExecuteNonQuery();
                }

                // Clears all the hub configurations
                SQLiteCommand deleteHubCmd = new SQLiteCommand("delete from VintHubConfig", _tradDBConn);
                deleteHubCmd.ExecuteNonQuery();

                // Creates a command for each hub detected
                foreach (VintHub hub in VintHubs)
                {
                    // More SQL parameters
                    SQLiteCommand cmd = new SQLiteCommand(createVintHubConfigQuery, _tradDBConn);
                    cmd.Parameters.AddWithValue("@HubName", hub.HubName);
                    cmd.Parameters.AddWithValue("@Serial", hub.SerialNumber);
                    cmd.Parameters.AddWithValue("@Port0", hub.Sensor0.SensorType);
                    cmd.Parameters.AddWithValue("@Port1", hub.Sensor1.SensorType);
                    cmd.Parameters.AddWithValue("@Port2", hub.Sensor2.SensorType);
                    cmd.Parameters.AddWithValue("@Port3", hub.Sensor3.SensorType);
                    cmd.Parameters.AddWithValue("@Port4", hub.Sensor4.SensorType);
                    cmd.Parameters.AddWithValue("@Port5", hub.Sensor5.SensorType);
                    cmd.Parameters.AddWithValue("@Wireless", hub.Wireless);
                    cmd.ExecuteNonQuery();

                    //vintConfigCmds.Add(new SQLiteCommand(createVintHubConfigQuery, _tradDBConn));
                }

                //execute all hub commmands
                //foreach (SQLiteCommand cmd in vintConfigCmds)
                //{
                //    cmd.ExecuteNonQuery();
                //}

                // Closes the connection
                _tradDBConn.Close();

                // Return good, as the query was able to execute
                result = "good";
            }
            // Return the error message when an exception has been encountered
            catch (Exception ex)
            {
                _tradDBConn.Close();
                result = ex.Message;
                throw;
            }
            return result;
        }

        // Checks if a configuration database already exists
        public bool CheckForConfig()
        {
            bool configExists = false;
            DataTable test = new DataTable();

            // Query to check if the GeneralConfig table exists
            string checkQuery = $@"select * from GeneralConfig";

            // Checks if the SQLite database for the configuration already exists
            if (File.Exists("TradDB.sqlite"))
            {
                // Opens the connection
                _tradDBConn.Open();
                using (SQLiteDataAdapter adp = new SQLiteDataAdapter(checkQuery, _tradDBConn))
                {
                    // Fills a DataTable
                    adp.Fill(test);
                    // If the DataTable has more than 0 rows then the configuration exists
                    if (test.Rows.Count > 0)
                    {
                        configExists = true;
                    }
                }
                // Closes the connection
                _tradDBConn.Close();
            }
            return configExists;
        }

        // Creates all the tables in the configuration database if they do not already exists
        public bool BuildDataStores()
        {
            bool success = false;

            // SQLite query to create all the tables if they do not already exists
            // TODO: Replace the string query with a LINQ query
            string createTables = $@"create table if not exists SensorData(
                                            SensorType text,
                                            Data real,
                                            DateTime text,
                                            SerialNumber text,
                                            HubName text);

                                     create table if not exists VintHubConfig(
                                            HubName text,
                                            SerialNumber text, 
                                            Port0 text not null,
                                            Port1 text not null,
                                            Port2 text not null,
                                            Port3 text not null,
                                            Port4 text not null,
                                            Port5 text not null,
                                            Wireless bool not null
                                            );

                                    create table if not exists ExtensionHubConfig(
                                            Port0 text not null,
                                            Port1 text not null,
                                            Port2 text not null,
                                            Port3 text not null,
                                            Port4 text not null,
                                            Port5 text not null,
                                            Port6 text not null,
                                            Port7 text not null
                                            );

                                     create table if not exists GeneralConfig(
                                            RecipientEmail text not null,
                                            SenderEmail text not null,
                                            SenderPassword text not null,
                                            SMTPAddress text not null,
                                            SMTPPort int not null,
                                            DataInterval int not null,
                                            MinSoilTemperature real not null,
                                            MinAirTemperature real not null,
                                            MinHumidity real not null,
                                            MinMoisture real not null,
                                            MinOxygen real not null,
                                            MinCO2 real not null,
                                            MinVOC real not null,
                                            MaxSoilTemperature real not null,
                                            MaxAirTemperature real not null,
                                            MaxHumidity real not null,
                                            MaxMoisture real not null,
                                            MaxOxygen real not null,
                                            MaxVOC real not null,
                                            MaxCO2 real not null,
                                            Gps bool not null);";

            try
            {
                // Opens the connection
                _tradDBConn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(createTables, _tradDBConn))
                {
                    // Executes the query
                    cmd.ExecuteNonQuery();
                }
                // Closes the connection
                _tradDBConn.Close();

                // Query was succesfully executed
                success = true;
            }
            // If an exception is encountered then close the connection
            catch (Exception ex)
            {
                _tradDBConn.Close();
                throw ex;
            }

            return success;
        }

        // Inserts the sensor data into an SQLite DB
        public string InsertData(string collectionTime, string sensorType, string value, string serial, string hub)
        {
            try
            {
                // TODO: Replace the string query with a LINQ query
                string query = $@"insert into SensorData (SensorType, Data, DateTime, SerialNumber, HubName)
                                values(
                                    '{sensorType}',
                                    '{value}',
                                    '{collectionTime}',
                                    '{serial}',
                                    '{hub}')";
                // Opens the connection
                _tradDBConn.Open();

                using (SQLiteCommand cmd = new SQLiteCommand(query, _tradDBConn))
                {
                    // Executes the query
                    cmd.ExecuteNonQuery();
                }
                // The query was executed successfully, return good
                return "good";
            }
            // The query was not executed succsesfully, return the error message
            catch (Exception ex)
            {
                return $"An error occured while collection data: \n{ex.Message} \n \n Data collection still running.";
            }
            finally
            {
                // close the database conncetion
                _tradDBConn.Close();
            }
        }
        #endregion

        // TODO: Integrate entity framework once we learn it in class
        #region graphing methods
        // The following 4 methods are used to get data needed to generate graphs on the GraphWindowView
        // They are ordered by their filtering priority:
        // - first a date range is applied
        // - then a serial number is chosen
        // - then a sensor type is chosen, and a table with the above filters applied can be returned to the viewmodel

        // called on graph window load, used to get date range of the current dataset
        public List<DateTime> GetDateRange()
        {
            try
            {
                // Create a list of dateTimes
                List<DateTime> dates = new List<DateTime>();

                // TODO: Replace the string query with a LINQ query
                string query = $@"select DateTime from SensorData";
                // Open the connection to the database
                _tradDBConn.Open();

                // Create the SQLiteCommand from the query
                SQLiteCommand cmd = new SQLiteCommand(query, _tradDBConn);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Reads the dates from the database and adds them to a list of dates
                        dates.Add(DateTime.ParseExact(reader.GetString(0), "MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture));
                    }
                }
                // Closes the connection
                _tradDBConn.Close();

                // Returns the list of dates
                return dates;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Used to populate the serial number combo box and repopulate it when a new date range is chosen
        public IEnumerable<string> GetHubsByDate(DateTime start, DateTime end)
        {
            try
            {
                // Creates a list of serial numbers
                List<string> serials = new List<string>();

                // Sets the start date and end date based on the dateTime parameters
                string startDate = start.ToString("MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                string endDate = end.ToString("MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                // Sets the SQLite query string
                // TODO: Replace the string query with a LINQ query
                string query = $@"select HubName from SensorData where
                                DateTime between '{startDate}' and '{endDate}'";

                // Opens the connection to the database
                _tradDBConn.Open();

                // Creates the SQLiteCommand
                SQLiteCommand cmd = new SQLiteCommand(query, _tradDBConn);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    // Reads through the database based on the SQLiteCommand
                    while (reader.Read())
                    {
                        // Finds the serial numbers and adds them to the list
                        serials.Add(reader.GetString(0));
                    }
                }
                // Closes the connection to the database
                _tradDBConn.Close();
                // Returns the list of serial numbers
                return serials;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Used to populate the sensor type combobox when a new serialnumber is chosen
        public IEnumerable<string> GetSensorsByHub(string hub, DateTime start, DateTime end)
        {
            try
            {
                // Creates a list of sensors
                List<string> sensors = new List<string>();

                // Sets the SQLite query
                // TODO: Replace the string query with a LINQ query
                string query = $@"select SensorType from SensorData
                                  where HubName = '{hub}' and
                                  DateTime between 
                                  '{start.ToString("MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture)}' and
                                  '{end.ToString("MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture)}'";

                // Opens the connection
                _tradDBConn.Open();

                // Creates the SQLiteCommand
                SQLiteCommand cmd = new SQLiteCommand(query, _tradDBConn);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    // Reads the existing sensors and adds them to the list
                    while (reader.Read())
                    {
                        sensors.Add(reader.GetString(0));
                    }
                }

                // Closes the connection
                _tradDBConn.Close();
                // Returns the list of sensors
                return sensors;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Used to get a filtered table of the sensor data after all filters are chosen
        public DataTable GetSensorData(string hub, string sensorType, DateTime start, DateTime end)
        {
            try
            {
                // Creates a datatable and columns for the repsective values
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("SensorType"));
                dt.Columns.Add(new DataColumn("Data"));
                dt.Columns.Add(new DataColumn("DateTime"));
                dt.Columns.Add(new DataColumn("SerialNumber"));
                dt.Columns.Add(new DataColumn("HubName"));

                // Sets the SQLite query to return all values where the hub name is equal to the selected hub and the dateTimes fall within the selcted dateTime range
                // TODO: Replace the string query with a LINQ query
                string query = $@"select * from SensorData
                                  where HubName = '{hub}' and
                                  
                                  DateTime between '{start.ToString("MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture)}' and
                                  '{end.ToString("MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture)}'";

                // Opens the connection
                _tradDBConn.Open();

                // Creates the SQLiteCommand
                SQLiteCommand cmd = new SQLiteCommand(query);


                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    // Read through the database and sets the DataTable rows to the correct values for the selected hub and dateTime range
                    while (reader.Read())
                    {
                        DataRow dr = dt.NewRow();
                        dr["SensorType"] = reader.GetString(0);
                        dr["Data"] = Convert.ToInt32(reader.GetString(1).Split(' ')[0]);
                        dr["DateTime"] = DateTime.ParseExact(reader.GetString(2), "MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        dr["SerialNumber"] = reader.GetString(3);
                        dr["HubName"] = reader.GetString(4);
                    }
                }
                // Closes the connection
                _tradDBConn.Close();
                // Returns the dataTable
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #endregion
    }
}
