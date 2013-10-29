using BikeWay03.DataServices;
using BikeWay03.Util;
using BikeWay03.ViewModels;
using Microsoft.Phone.Maps.Controls;
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
                    await sqliteConnection.CreateTableAsync<FavoriteBase>();
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

        public static async void DeleteAllAndSaveStations(string networkName, ObservableCollection<StationModel> list)
        {
            string query = "DELETE FROM station";
            Debug.WriteLine(query);


            App.MainPage.IsLoadingSomething = true;
            App.MainPage.Status = "Deleting " + networkName + " stations...";

            var linesChanged = await sqliteConnection.QueryAsync<int>(query);


            App.MainPage.IsLoadingSomething = false;
            App.MainPage.Status = "";

            

            Debug.WriteLine("lines changed deleting stations: lenght = " + linesChanged.Count);
            Settings.IsStationSavedToDatabase = false;


            Debug.WriteLine("Size of stations + " + list.Count);
            List<StationBase> stationBaseList = new List<StationBase>();

            

            return;
            int i = 0;
            foreach (StationModel station in list)
            {
                Debug.WriteLine(i);
                i++;
                stationBaseList.Add(station.getStationBase());
            }
            Debug.WriteLine("Station list count " + stationBaseList.Count);
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
            string query = "SELECT * FROM favorites";
            var results = await sqliteConnection.QueryAsync<FavoriteBase>(query);
            Debug.WriteLine(query);


            Debug.WriteLine("result of favorites: lenght = " + results.Count);

            App.PivotPageViewModel.favoriteList.Clear();

            foreach (FavoriteBase stationBase in results)
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
            App.MainPage.UpdatePushpinsStationsOnMap(App.MainViewModel.StationList);
        }

        public static async void getAllNetworks()
        {
            string query = "SELECT * FROM network";
            var results = await sqliteConnection.QueryAsync<NetworkBase>(query);
            Debug.WriteLine(query);


            Debug.WriteLine("result of networks: lenght = " + results.Count);

            App.MainViewModel.NetworkList.Clear();

            foreach (NetworkBase networkBase in results)
            {
                NetworkModel network = NetworkModel.getNetworkModel(networkBase);
                App.MainViewModel.NetworkList.Add(network);
            }
            //Settings.IsNetworkSavedToDatabase = true;

            if (App.MainPage.MapControl.ZoomLevel <= 13)
            {
                App.MainPage.UpdatePushpinsNetworksOnMap(App.MainViewModel.NetworkList);
            }

        }

        public static async void getAllNetworksAndStations()
        {
            string query = "SELECT * FROM network";
            var results = await sqliteConnection.QueryAsync<NetworkBase>(query);
            Debug.WriteLine(query);


            Debug.WriteLine("result of networks (get All Networks and Stations, of course): lenght = " + results.Count);

            App.MainViewModel.NetworkList.Clear();

            foreach (NetworkBase networkBase in results)
            {
                NetworkModel network = NetworkModel.getNetworkModel(networkBase);
                App.MainViewModel.NetworkList.Add(network);
            }

            Settings.IsNetworkSavedToDatabase = true;
            Debug.WriteLine("current network name  = " + Settings.currentNetworkName);

            if (!string.IsNullOrEmpty(Settings.currentNetworkName))
            {
                if (Settings.IsStationSavedToDatabase == true)
                {
                    Debug.WriteLine("Station is saved to database. Trying to load");
                    getAllStations();
                }
                else
                {
                    Debug.WriteLine("Station is NOT saved to database. Trying to download");
                    DataService.GetStationList(new NetworkModel { name = Settings.currentNetworkName });
                }
            }
            else
            {

                 App.MainPage.UpdatePushpinsNetworksOnMap(App.MainViewModel.NetworkList);
  
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

        

        public static async void AddFavoriteStation(StationModel station, string networkName)
        {
            //string query = "UPDATE station SET IsFavorite = 1  WHERE number = " + station.number;

            FavoriteBase stationBase = station.getFavoriteBase();
            stationBase.networkName = networkName;
            int lines_changed;

            if (station.IsFavorite == true)
            {
                lines_changed = await sqliteConnection.InsertAsync(stationBase);
            }
            else
            {
                lines_changed = await sqliteConnection.DeleteAsync(stationBase);
            }


            

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


        public static async void tryGetNetwork(Microsoft.Phone.Maps.Controls.LocationRectangle locationRectangle)
        {
            int latitude_south = (int)(locationRectangle.South * 1E6);
            int latitude_north = (int)(locationRectangle.North * 1E6);

            int longitude_west = (int)(locationRectangle.West * 1E6);
            int longitude_east = (int)(locationRectangle.East * 1E6);

            string query_latitude = "lat BETWEEN " + latitude_south + " AND " + latitude_north;
            string query_longitude = "lng BETWEEN " + longitude_west + " AND " + longitude_east;

            string query = "SELECT * FROM network WHERE ( " + query_latitude + " ) AND ( " + query_longitude + " )";

            Debug.WriteLine(query);
            
            var results = await sqliteConnection.QueryAsync<NetworkModel>(query);
            Debug.WriteLine("result of network teste: lenght = " + results.Count);
            if (results.Count == 1)
            {
                App.MainViewModel.Network = results[0];
            }

        }

        public static async void tryGetNetwork(GeoCoordinate geoCoordinate)
        {
            double delta_latitude = 0.0587626732885838;
            double delta_longitude = 0.0458679534494877;

            LocationRectangle locationRectangle = new LocationRectangle(geoCoordinate, delta_latitude, delta_longitude);

            int latitude_south = (int)(locationRectangle.South * 1E6);
            int latitude_north = (int)(locationRectangle.North * 1E6);

            int longitude_west = (int)(locationRectangle.West * 1E6);
            int longitude_east = (int)(locationRectangle.East * 1E6);

            string query_latitude = "lat BETWEEN " + latitude_south + " AND " + latitude_north;
            string query_longitude = "lng BETWEEN " + longitude_west + " AND " + longitude_east;

            string query = "SELECT * FROM network WHERE ( " + query_latitude + " ) AND ( " + query_longitude + " )";

            Debug.WriteLine(query);
            
            var results = await sqliteConnection.QueryAsync<NetworkModel>(query);
            Debug.WriteLine("result of network teste: lenght = " + results.Count);
            
            if (results.Count == 1)
            {
                Debug.WriteLine("Network found::: " + results[0].name);
                App.MainViewModel.Network = results[0];
            }

        }

        public static NetworkModel tryGetNetwork(GeoCoordinate coordinate, ObservableCollection<NetworkModel> networkList)
        {
            if (coordinate == null)
            {
                Debug.WriteLine("GeoCoordinate of my Location = null");
                return null;

            }

            double delta_latitude = 0.0587626732885838;
            double delta_longitude = 0.0458679534494877;

            LocationRectangle locationRectangle = new LocationRectangle(coordinate, delta_latitude, delta_longitude);

            int latitude_south = (int)(locationRectangle.South * 1E6);
            int latitude_north = (int)(locationRectangle.North * 1E6);

            int longitude_west = (int)(locationRectangle.West * 1E6);
            int longitude_east = (int)(locationRectangle.East * 1E6);

            var networks = networkList.ToList().AsQueryable();

            var result = 
                from network in networks
                where  !(network.lat < latitude_south || network.lat > latitude_north) 
                || !(network.lng < longitude_west || network.lat > longitude_east)
                    select network;


            Debug.WriteLine("after trying to get a network.... result count == " + result.ToList().Count);
            if (result.ToList().Count == 0)
            {
                return null;
            }
            else
            {
                var myNetwork = result.ToList()[0];
                Settings.CurrentNetworkLatitude = myNetwork.GeoCoordinate.Latitude;
                Settings.CurrentNetworkLongitude = myNetwork.GeoCoordinate.Longitude;
                Settings.currentNetworkName = myNetwork.name;
                App.MainViewModel.Network = myNetwork;
                return myNetwork;
            }

        }

    }
    [Table("favorites")]
    public class FavoriteBase
    {
        [PrimaryKey]
        public int id { get; set; }
        public string name { get; set; }
        public int lat { get; set; }
        public int lng { get; set; }
        public int number { get; set; }
        public int bikes { get; set; }
        public int free { get; set; }
        public string station_url { get; set; }
        public double percentage { get; set; }
        public string _bikes_string { get; set; }
        public string _free_string { get; set; }
        public string networkName { get; set; }
        private bool isFavorite = false;
        public bool IsFavorite
        {
            get { return this.isFavorite; }
            set { this.isFavorite = value; }
        }
    }
  
}
