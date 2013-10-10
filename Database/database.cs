using BikeWay03.Util;
using BikeWay03.ViewModels;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
