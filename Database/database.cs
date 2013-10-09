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
        public static string databaseName = "database9.db";
        public static string dbPath;

        public static void initializeDatabase() 
        {

            dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, databaseName);
            // Initialize database connection
            sqliteConnection = new SQLiteAsyncConnection ( dbPath );

            CreateDatabase();
            Debug.WriteLine("database initialized");
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
                    Debug.WriteLine("table created");

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
                Debug.WriteLine("database changed succesfully - lines changed: " + inserted.ToString());
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

        private async void UpdateCustomersList()
        {
            // Select all customers from database
            // 'var' is a shortcut for List<Customer> type
            //var results = await sqliteConnection.QueryAsync<Customer>("select * from person");

            // Just for a quick demo, we select the last added customer and add to our collection
            //Customer customer = results.Last();
            //customers.Add(customer);
        }
    }

    [Table("station")]
    public class Station
    {
        // The Column attribute maps the name of this property with name of database column
        [Column("name")]
        public string Name { get; set; }
        public int Free { get; set; }
        public int Bikes { get; set; }
    }

    [Table("station_model")]
    public class Station_Model
    {
        [Column("id")]
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
   

    }
}
