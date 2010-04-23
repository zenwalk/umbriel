using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.Runtime;
using System.Reflection;
using System.IO;

namespace Umbriel.ArcMapUI.Util
{
    class Settings
    {
           /// <summary>
        /// Initializes a new instance of the <see cref="UserSettingDatabase"/> class.
        /// </summary>
        public Settings()
        {
            this.ChangedSettings = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSettingDatabase"/> class.
        /// </summary>
        /// <param name="settingsDBPath">The settings DB path.</param>
        public Settings(string settingsDBPath)
        {
            this.ChangedSettings = new Dictionary<string, object>();
            this.SettingsDBFilePath = settingsDBPath;
        }

        /// <summary>
        /// Gets the changed settings.
        /// </summary>
        /// <value>The changed settings.</value>
        public Dictionary<string, object> ChangedSettings { get; private set; }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public Dictionary<string, object> SettingsDictionary { get; private set; }

        /// <summary>
        /// Gets or sets the settings DB file path.
        /// </summary>
        /// <value>The settings DB file path.</value>
        private string SettingsDBFilePath { get; set; }

        /// <summary>
        /// Reads the setting.
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <returns>setting value string</returns>
        public static string ReadSetting(string settingName)
        {
            string settingValue = string.Empty;

            string databasePath = GetDatabasePath();
            string connectionString = "Data Source=" + databasePath + ";Version=3;";

            string sql = "SELECT settingname,settingvalue,serializeAs FROM setting WHERE settingname='{0}'";

            sql = String.Format(sql, settingName);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            settingValue = reader["settingvalue"].ToString();
                        }
                    }
                }

                connection.Close();
            }

            return settingValue;
        }

        /// <summary>
        /// Gets the setting value.
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <returns>the object stored as setting for the settingname param</returns>
        public static object GetSettingValue(string settingName)
        {
            object val = null;

            string databasePath = GetDatabasePath();
            string connectionString = "Data Source=" + databasePath + ";Version=3;";

            string sql = "SELECT settingname,settingvalue,serializeAs FROM setting WHERE settingname='{0}'";

            sql = String.Format(sql, settingName);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            val = reader["settingvalue"];
                        }
                    }
                }

                connection.Close();
            }

            return val;
        }

        /// <summary>
        /// Writes the setting.
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="settingValue">The setting value.</param>
        public static void WriteSetting(string settingName, object settingValue)
        {
            string databasePath = GetDatabasePath();
            string connectionString = "Data Source=" + databasePath + ";Version=3;";

            string sql = "INSERT OR REPLACE INTO setting VALUES  ('{0}','{1}','String')";

            sql = String.Format(sql, settingName, settingValue);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }

            return;
        }

        /// <summary>
        /// Adds a setting name/value to the setting dictionary.  Values are not immediately 
        /// written to the database file.
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="settingValue">The setting value.</param>
        public void AddSetting(string settingName, object settingValue)
        {
            if (this.ChangedSettings.ContainsKey(settingName))
            {
                this.ChangedSettings[settingName] = settingValue;
            }
            else
            {
                this.ChangedSettings.Add(settingName, settingValue);
            }
        }

        /// <summary>
        /// Clears any setting changes (adds/updates) from the ChangedSettings dictionary
        /// </summary>
        public void ClearSettings()
        {
            this.ChangedSettings.Clear();
        }

        /// <summary>
        /// Flushes the settings to the database
        /// </summary>
        public void FlushSettings()
        {
            foreach (KeyValuePair<string, object> item in this.ChangedSettings)
            {
                WriteSetting(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Reads all settings into a dictionary
        /// </summary>
        public void ReadAllSettings()
        {
            string settingName = string.Empty;
            object settingValue = null;

            if (this.SettingsDictionary != null)
            {
                this.SettingsDictionary.Clear();
            }
            else
            {
                this.SettingsDictionary = new Dictionary<string, object>();
            }

            string databasePath = string.Empty;

            if (string.IsNullOrEmpty(this.SettingsDBFilePath))
            {
                databasePath = GetDatabasePath();
            }
            else
            {
                databasePath = this.SettingsDBFilePath;
            }

            string connectionString = "Data Source=" + databasePath + ";Version=3;";

            string sql = "SELECT settingname,settingvalue,serializeAs FROM setting";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            settingName = string.Empty;
                            settingValue = null;

                            if (!reader["settingname"].Equals(System.DBNull.Value))
                            {
                                settingName = reader["settingname"].ToString();
                                settingValue = reader["settingvalue"];

                                this.SettingsDictionary.Add(settingName, settingValue);
                            }
                        }
                    }
                }

                connection.Close();
            }

            return;
        }

        /// <summary>
        /// Gets the database path.
        /// </summary>
        /// <param name="databaseFileName">Name of the database file.</param>
        /// <returns>path to the setting database</returns>
        private static string GetDatabasePath()
        {
            string applicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

            string directoryPath = applicationData + "\\" + assemblyName;

            string databaseFileName = assemblyName + ".settings.sqlite";

            string databasePath = string.Empty;

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            databasePath = directoryPath + "\\" + databaseFileName;

            if (!File.Exists(databasePath))
            {
                CreateNewSettingDatabase(directoryPath, databaseFileName);
            }

            return databasePath;
        }

        /// <summary>
        /// Creates the new setting database.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <returns>path to the new settings database</returns>
        private static string CreateNewSettingDatabase(string fullpath)
        {
            return CreateNewSettingDatabase(Path.GetDirectoryName(fullpath), Path.GetFileName(fullpath));
        }

        /// <summary>
        /// Creates the new setting database.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <param name="databaseFileName">Name of the database file.</param>
        /// <returns>path to the new settings database</returns>
        private static string CreateNewSettingDatabase(string directoryPath, string databaseFileName)
        {
            string databasePath = directoryPath + "\\" + databaseFileName;

            if (!File.Exists(databasePath))
            {
                Assembly a = Assembly.GetExecutingAssembly();
                string[] rnames = a.GetManifestResourceNames();
                //Umbriel.ArcMapUI.Umbriel.ArcMapUI.dll.sqlite
                
                Stream stream = a.GetManifestResourceStream(a.GetName().Name + "." + databaseFileName);

                FileStream writeStream = new FileStream(databasePath, FileMode.Create, FileAccess.Write);

                int length = 256;
                byte[] buffer = new byte[length];
                int bytesRead = stream.Read(buffer, 0, length);

                // write the required bytes
                while (bytesRead > 0)
                {
                    writeStream.Write(buffer, 0, bytesRead);
                    bytesRead = stream.Read(buffer, 0, length);
                }

                stream.Close();
                writeStream.Close();

                return databasePath;
            }
            else
            {
                throw new ArgumentException(databasePath + " already exists.");
            }
        }

        /// <summary>
        /// Gets the assembly directory.
        /// </summary>
        /// <value>The assembly directory.</value>
        static public string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}

