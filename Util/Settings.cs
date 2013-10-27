using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
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
            isStationSavedToDatabase,
            lastUpdatedTimeSpan,
            lastUserLocationLatitude,
            lastUserLocationLongitude,
            userLocation,
            currentNetworkName,
            currentNetworkLatitude,
            currentNetworkLongitude,
            firstTimeLaunch
        };

        private static bool _isNetworkSavedToDatabase = false;
        private static bool _isStationSavedToDatabase = false;
        private static TimeSpan _lastUpdatedTimeSpan;
        private static GeoCoordinate _lastUserLocation;
        //private static double _lastUserLocationLatitude;
        //private static double _lastUserLocationLongitude;
        private static bool _userLocation = true;
        private static string _currentNetworkName;
        private static double _currentNetworkLatitude;
        private static double _currentNetworkLongitude;
        private static bool _firstTimeLaunch;

        #region Variables Definitions
        public static bool IsNetworkSavedToDatabase
        {
            get
            {
                return _isNetworkSavedToDatabase;
            }
            set
            {
                if (_isNetworkSavedToDatabase != value)
                {
                    _isNetworkSavedToDatabase = value;
                    saveKey(settingKey.isNetworkSavedToDatabase, value);
                    settings.Save();
                }
            }
        }

        public static bool IsStationSavedToDatabase
        {
            get
            {
                return _isStationSavedToDatabase;
            }
            set
            {
                if (_isStationSavedToDatabase != value)
                {
                    _isStationSavedToDatabase = value;
                    saveKey(settingKey.isStationSavedToDatabase, value);
                    settings.Save();
                }
            }
        }

        public static TimeSpan LastUpdatedTimeSpan
        {
            get
            {
                return _lastUpdatedTimeSpan;
            }
            set
            {
                if (!TimeSpan.Equals(_lastUpdatedTimeSpan,value))
                {
                    _lastUpdatedTimeSpan = value;
                    saveKey(settingKey.lastUpdatedTimeSpan, value);
                    settings.Save();
                }
            }
        }

    

        public static GeoCoordinate LastUserLocation
        {
            get
            {
                return _lastUserLocation;
            }
            set
            {
                if (_lastUserLocation != value)
                {
                    _lastUserLocation = value;
                    saveKey(settingKey.lastUserLocationLongitude, value.Longitude);
                    saveKey(settingKey.lastUserLocationLatitude, value.Latitude);
                    settings.Save();
                }
            }
        }

        public static bool UserLocation
        {
            get
            {
                return _userLocation;
            }
            set
            {
                if (_userLocation != value)
                {
                    _userLocation = value;
                    saveKey(settingKey.userLocation, value);
                    settings.Save();
                }
            }
        }

        public static string currentNetworkName
        {
            get
            {
                return _currentNetworkName;
            }
            set
            {
                if (_currentNetworkName != value)
                {
                    _currentNetworkName = value;
                    saveKey(settingKey.currentNetworkName, value);
                    settings.Save();
                }
            }
        }

        public static double CurrentNetworkLatitude
        {
            get
            {
                return _currentNetworkLatitude;
            }
            set
            {
                if (_currentNetworkLatitude != value)
                {
                    _currentNetworkLatitude = value;
                    saveKey(settingKey.currentNetworkLatitude, value);
                    settings.Save();
                }
            }
        }

        public static double CurrentNetworkLongitude
        {
            get
            {
                return _currentNetworkLongitude;
            }
            set
            {
                if (_currentNetworkLongitude != value)
                {
                    _currentNetworkLongitude = value;
                    saveKey(settingKey.currentNetworkLongitude, value);
                    settings.Save();
                }
            }
        }

        public static bool FirstTimeLaunch
        {
            get
            {
                return _firstTimeLaunch;
            }
            set
            {
                if (_firstTimeLaunch != value)
                {
                    _firstTimeLaunch = value;
                    saveKey(settingKey.firstTimeLaunch, value);
                    settings.Save();
                }
            }
        }

        #endregion
        

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
            

            Debug.WriteLine("Saved Setting " + key.ToString() + " = " + value.ToString());
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
                _isNetworkSavedToDatabase = (bool)value;
                
            }
            else
            {
                _isNetworkSavedToDatabase = false;
            }

            Debug.WriteLine(settingKey.isNetworkSavedToDatabase.ToString() + " = " + _isNetworkSavedToDatabase.ToString());

            value = getKey(settingKey.isStationSavedToDatabase);
            if (value != null)
            {
                _isStationSavedToDatabase = (bool)value;
                
            }
            else
            {
                _isStationSavedToDatabase = false;
            }

            Debug.WriteLine(settingKey.isStationSavedToDatabase.ToString() + " = " + _isStationSavedToDatabase.ToString());

            value = getKey(settingKey.lastUpdatedTimeSpan);
            if (value != null)
            {
                _lastUpdatedTimeSpan = (TimeSpan)value;
            }
            else
            {
                _lastUpdatedTimeSpan = new TimeSpan();
            }

            Debug.WriteLine(settingKey.lastUpdatedTimeSpan.ToString() + " = " + _lastUpdatedTimeSpan.ToString());

            

            var value_latitude = getKey(settingKey.lastUserLocationLatitude);
            var value_longitude = getKey(settingKey.lastUserLocationLongitude);
            if (value_longitude != null && value_latitude != null)
            {
                _lastUserLocation = new GeoCoordinate();
                _lastUserLocation.Latitude = (double)value_latitude;
                _lastUserLocation.Longitude = (double)value_longitude;
                Debug.WriteLine("LastUserLocation" + " = " + _lastUserLocation.ToString());
            }
            else
            {
                _lastUserLocation = null;
                Debug.WriteLine("LastUserLocation" + " = " + "null");
            }

            

            value = getKey(settingKey.userLocation);
            if (value != null)
            {
                _userLocation = (bool)value;
            }
            else
            {
                _userLocation = true;
            }

            Debug.WriteLine(settingKey.userLocation.ToString() + " = " + _userLocation.ToString());

            value = getKey(settingKey.currentNetworkName);
            if (value != null)
            {
                _currentNetworkName = (string)value;
                Debug.WriteLine(settingKey.currentNetworkName.ToString() + " = " + _currentNetworkName.ToString());
            }
            else
            {
                _currentNetworkName = null;
                Debug.WriteLine(settingKey.currentNetworkName.ToString() + " = " + "null");
            }

            

            value = getKey(settingKey.currentNetworkLatitude);
            if (value != null)
            {
                _currentNetworkLatitude = (double)value;
            }
            else
            {
                _currentNetworkLatitude = -1;
            }

            Debug.WriteLine(settingKey.currentNetworkLatitude.ToString() + " = " + _currentNetworkLatitude.ToString());

            value = getKey(settingKey.currentNetworkLongitude);
            if (value != null)
            {
                _currentNetworkLongitude = (double)value;
            }
            else
            {
                _currentNetworkLongitude = -1;
            }

            Debug.WriteLine(settingKey.currentNetworkLongitude.ToString() + " = " + _currentNetworkLongitude.ToString());

            value = getKey(settingKey.firstTimeLaunch);
            if (value != null)
            {
                _firstTimeLaunch = (bool)value;
            }
            else
            {
                _firstTimeLaunch = true;
            }

            Debug.WriteLine(settingKey.firstTimeLaunch.ToString() + " = " + _firstTimeLaunch.ToString());

        }



    }
}
