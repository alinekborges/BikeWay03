using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeWay03.Util
{
    public static class Settings
    {
        
        public enum settingKey{
            isNetworkSavedToDatabase,
            isStationSavedToDatabase
        };

        private static bool isNetworkSavedToDatabase = false;
        private static bool isStationSavedToDatabase = false;

        public static bool IsNetworkSavedToDatabase
        {
            get
            {
                return isNetworkSavedToDatabase;
            }
            set
            {
                if (isNetworkSavedToDatabase != value)
                {
                    isNetworkSavedToDatabase = value;
                    saveKey(settingKey.isNetworkSavedToDatabase, value);
                }
            }
        }

        public static bool IsStationSavedToDatabase
        {
            get
            {
                return isStationSavedToDatabase;
            }
            set
            {
                if (isStationSavedToDatabase != value)
                {
                    isStationSavedToDatabase = value;
                    saveKey(settingKey.isStationSavedToDatabase, value);
                }
            }
        }



        public static IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

        public static void saveKey(settingKey key, object value)
        {
            if (!settings.Contains(key.ToString()))
            {
                settings.Add(key.ToString(), value);
            }
            else
            {
                settings[key.ToString()] = value;
            }
            settings.Save();
        }

        public static object getKey(settingKey key)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key.ToString()))
            {                
                object obj =  IsolatedStorageSettings.ApplicationSettings[key.ToString()];
                return obj;
            }
            else
            {
                return null;
            }
        }

        public static void  loadAllSettings()
        {
            object value = getKey(settingKey.isNetworkSavedToDatabase);
            if (value != null)
            {
                isNetworkSavedToDatabase = (bool)value;
            }
            else
            {
                isNetworkSavedToDatabase = false;
            }

            value = getKey(settingKey.isStationSavedToDatabase);
            if (value != null)
            {
                isStationSavedToDatabase = (bool)value;
            }
            else
            {
                isStationSavedToDatabase = false;
            }

        }



    }
}
