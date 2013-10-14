using BikeWay03.Util;
using BikeWay03.ViewModels;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BikeWay03.DB
{
    public class Database
    {
        public static SQLiteAsyncConnection sqliteConnection;
        public static ObservableCollection<StationModel> stationList;
        public static string databaseName = "database_bikeway.db";
        public static string dbPath;

        public static void initializeDatabase() 
        {

            dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, databaseName);
            // Initialize database connection
            sqliteConnection = new SQLiteAsyncConnection ( dbPath );

            CreateDatabase();
            
        }

        private static async void CreateDatabase()
        {
            // Here we check whether the database alredy exists
            // It's not required, but added just to show an example of IsolatedStorage
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!storage.FileExists(databaseName))
                {
                    await sqliteConnection.CreateTableAsync<StationBase>();
                    await sqliteConnection.CreateTableAsync<NetworkBase>();
                    Debug.WriteLine("Database Tables Created");

                }
            }
        }

        public static async void SaveStations(ObservableCollection<StationModel> stationList)
        {
            //int inserted = await sqlite
            //int inserted = await sqliteConnection.in

            List<StationBase> stationBaseList = new List<StationBase>();

            foreach (StationModel station in stationList)
            {
                stationBaseList.Add(station.getStationBase());
            }

            //StationBase station = stationList[0].getStationBase();

            int inserted = await sqliteConnection.InsertAllAsync(stationBaseList);

            // If insertion successfull we update our view
            if (inserted > 0)
            {
                //UpdateCustomersList();
                Debug.WriteLine("adding stations : database changed succesfully - lines changed: " + inserted.ToString());
                Settings.IsStationSavedToDatabase = true;
            }
        }
        public static async void SaveNetworks(ObservableCollection<NetworkModel> networkList)
        {
            //int inserted = await sqlite
            //int inserted = await sqliteConnection.in

            List<NetworkBase> networkBaseList = new List<NetworkBase>();

            foreach (NetworkModel network in networkList)
            {
                networkBaseList.Add(network.getNetworkBase());
            }

            //StationBase station = stationList[0].getStationBase();
            
            int inserted = await sqliteConnection.InsertAllAsync(networkBaseList);

            // If insertion successfull we update our view
            if (inserted > 0)
            {
                //UpdateCustomersList();
                Debug.WriteLine("adding networks : database changed succesfully - lines changed: " + inserted.ToString());
                Settings.IsNetworkSavedToDatabase = true;
            }
        }


        public static async void getFavorites()
        {
            string query = "SELECT * FROM station WHERE IsFavorite = 1 ";
            var results = await sqliteConnection.QueryAsync<StationBase>(query);
            Debug.WriteLine(query);


            Debug.WriteLine("result of favorites: lenght = " + results.Count);

            App.PivotPageViewModel.favoriteList.Clear();

            foreach (StationBase stationBase in results)
            {
                StationModel station = StationModel.getStationModel(stationBase);
                App.PivotPageViewModel.favoriteList.Add(station);
            }
        }

        public static async void getAllStations()
        {
            string query = "SELECT * FROM station";
            var results = await sqliteConnection.QueryAsync<StationBase>(query);
            Debug.WriteLine(query);


            Debug.WriteLine("result of stations: lenght = " + results.Count);

            App.MainViewModel.StationList.Clear();

            foreach (StationBase stationBase in results)
            {
                StationModel station = StationModel.getStationModel(stationBase);
                App.MainViewModel.StationList.Add(station);
            }
        }

        public static async void getAllNetworks()
        {
            string query = "SELECT * FROM network";
            var results = await sqliteConnection.QueryAsync<NetworkBase>(query);
            Debug.WriteLine(query);


            Debug.WriteLine("result of stations: lenght = " + results.Count);

            App.MainViewModel.NetworkList.Clear();

            foreach (NetworkBase networkBase in results)
            {
                NetworkModel network = NetworkModel.getNetworkModel(networkBase);
                App.MainViewModel.NetworkList.Add(network);
            }
        }



        //search in database all stations within a small percentage of longitude and latitude
        //it's and attempt to reduce the number of results so the ordering algorithm will be faster
        public static async void getNearbyStations(GeoCoordinate coordinate)
        {
            int latitude = (int)(coordinate.Latitude * 1E6);
            int longitude = (int)(coordinate.Longitude * 1E6);

            double percentage = 0.0001;
            double percentage_1 = 0.001;
            
            int d_latitude = (int)Math.Abs(latitude * percentage);
            int d_longitude = (int)Math.Abs(longitude * percentage);

            int latitude_low = latitude - d_latitude;
            int latitude_high = latitude + d_latitude;

            int longitude_low = longitude - d_longitude;
            int longitude_high = longitude + d_longitude;

            
            string query_latitude = "lat BETWEEN " + latitude_low + " AND " + latitude_high;
            string query_longitude = "lng BETWEEN " + longitude_low + " AND " + longitude_high;

            string query = "SELECT * FROM station WHERE ( " + query_latitude + " ) AND ( " + query_longitude + " )";

            Debug.WriteLine(query);


            var results = await sqliteConnection.QueryAsync<StationBase>(query);
            Debug.WriteLine("result of nearby teste: lenght = " + results.Count);

            var nearbyList = OrderNearbyStations(coordinate, results);

            //App.PivotPageViewModel.nearbyList.Clear();

            int count = 7;
            if (nearbyList.Count <= 7)
            {
                count = nearbyList.Count;
            }

            for (int i = 0; i < count; i++)
            {
                App.PivotPageViewModel.nearbyList[i] =  nearbyList[i];
                Debug.WriteLine("added nearby " + i);
            }



        }

        public static List<StationModel> OrderNearbyStations(GeoCoordinate coordinate, List<StationBase> stationList)
        {
            List<StationModel> tempList = new List<StationModel>();

            foreach (StationBase station in stationList)
            {
                StationModel stationModel = StationModel.getStationModel(station);
                double distance = coordinate.GetDistanceTo(stationModel.GeoCoordinate);
                stationModel.distance = distance;
                tempList.Add(stationModel);
            }

            var nearbyList = tempList.OrderBy(x => x.distance).ToList();

            return nearbyList;
        }

        public static async void UpdateFavoriteStation(StationModel station)
        {
            string query = "UPDATE station SET IsFavorite = 1  WHERE number = " + station.number;

            StationBase stationBase = station.getStationBase();

            int lines_changed = await sqliteConnection.UpdateAsync(stationBase);


            //int lines_changed = await sqliteConnection.QueryAsync<StationBase>(query);
            Debug.WriteLine("isFavorite = " + station.IsFavorite.ToString());

            if (lines_changed > 0)
            {
                //UpdateCustomersList();
                Debug.WriteLine("database changed succesfully - lines changed: " + lines_changed.ToString());

                if (station.IsFavorite == true)
                {
                    App.PivotPageViewModel.favoriteList.Add(station);
                }
                else
                {
                    App.PivotPageViewModel.favoriteList.Remove(station);
                }
            }
            
        }

    }

  
}
