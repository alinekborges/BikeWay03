using BikeWay03.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BikeWay03.DB;
using BikeWay03.Util;
using Microsoft.Phone.Maps.Controls;
using System.Windows;
using System.Collections.ObjectModel;

namespace BikeWay03.DataServices
{
    public static class DataService
    {
        public static string filename = "stations.txt";
        public static string result;
        public static bool error;

        public static List<StationModel> stationListInBackground;

        public static string networkUrl = "http://api.citybik.es/networks.json";


        public static void GetStationList(string urlToCall)
        {
            if (App.OfflineMode == true)
            {
                //GetStationListOffline();
            }
            else
            {
                WebClient client = new WebClient();
                client.DownloadStringCompleted += client_DownloadStringCompleted;
                client.DownloadStringAsync(new Uri(urlToCall));
            }
        }

        public static void GetStationList(NetworkModel network)
        {

            string urlToCall = "http://api.citybik.es/" + network.name + ".json";
            Debug.WriteLine(urlToCall);
            if (App.OfflineMode == true)
            {
                //GetStationListOffline();
            }
            else
            {
                WebClient client = new WebClient();
                client.DownloadStringCompleted += client_DownloadStringCompleted;
                client.DownloadStringAsync(new Uri(urlToCall));
                App.MainPage.Status = "Downloading stations for " + network.name;
                App.MainPage.IsLoadingSomething = true;
            }
        }



        private static void client_DownloadStringCompletedInBackground(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                result = e.Result;
                Debug.WriteLine("reading from API...");
                Debug.WriteLine(result);
                //saveText(filename, result);
                stationListInBackground = JsonConvert.DeserializeObject<List<StationModel>>(result);
                App.MainPage.IsLoadingSomething = false;

            }
            else
            {
                Debug.WriteLine("ERROR downloading api");
                error = true;
                App.MainPage.IsLoadingSomething = false;
                MessageBox.Show("Error downloading station information. Check your internet connection");
            }
        }

        public static void GetNetworkList()
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += client_DownloadStringNetworkCompleted;
            client.DownloadStringAsync(new Uri(networkUrl));
        }


        private static void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                result = e.Result;
                Debug.WriteLine("reading from API...");
                Debug.WriteLine(result);
                //saveText(filename, result);
                List<StationModel> stationList = JsonConvert.DeserializeObject<List<StationModel>>(result);

                if (stationList != null)
                {
                    ObservableCollection<StationModel> stations = new ObservableCollection<StationModel>(stationList);
                    foreach (StationModel station in stationList)
                    {
                        App.MainViewModel.StationList.Add(station);                   
                    }

                    if (Settings.IsStationSavedToDatabase == false)
                    {
                        Database.SaveStations(App.MainViewModel.StationList);                        
                    }

                    if (Settings.IsStationSavedToDatabase == true)
                    {
                        Debug.WriteLine("Size of stations + " + App.MainViewModel.StationList.Count);
                        Database.DeleteAllAndSaveStations("", App.MainViewModel.StationList);
                    }

                    LocationRectangle locationRectangle = App.MainPage.getLocationRectangle();
                    App.MainPage.UpdatePushpinsStationsOnMap(App.MainViewModel.StationList);
                    App.MainPage.Status = "";
                    App.MainPage.IsLoadingSomething = false;
                    //App.MainPage.UpdatePushpinsStationsOnMap(App.MainViewModel.StationList);
                }

            }
            else
            {
                Debug.WriteLine("ERROR downloading api of stations ");
                App.MainPage.IsLoadingSomething = false;
                MessageBox.Show("Error downloading stations information. Check your internet connection");
            }
        }

        private static void client_DownloadStringNetworkCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                result = e.Result;
                Debug.WriteLine("reading networks......");
                Debug.WriteLine(result);
                //saveText(filename, result);
                List<NetworkModel> networkList = JsonConvert.DeserializeObject<List<NetworkModel>>(result);

                if (networkList != null)
                {

                    foreach (NetworkModel network in networkList)
                    {
                        App.MainViewModel.NetworkList.Add(network);
                       
                    }

                    if (Settings.IsNetworkSavedToDatabase == false)
                    {
                        Database.SaveNetworks(App.MainViewModel.NetworkList);
                        
                    }

                    Debug.WriteLine("network size = " + networkList.Count);
                    App.MainPage.IsLoadingSomething = false;
                    if (Settings.FirstTimeLaunch == true)
                    {
                        if (Settings.UserLocation == true)
                        {
                            var currentNetwork = Database.tryGetNetwork(App.MainPage.myLocation, App.MainViewModel.NetworkList);
                            App.MainViewModel.Network = currentNetwork;
                        }
                        else
                        {
                            App.MainPage.UpdatePushpinsNetworksOnMap(App.MainViewModel.NetworkList);
                        }
                        Settings.FirstTimeLaunch = false;
                    }
                }

            }
            else
            {
                App.MainPage.IsLoadingSomething = false;
                MessageBox.Show("Error downloading network information. Check your internet connection");
            }
        }

        private static void saveText(string filename, string text)
            {
                using (IsolatedStorageFile isf =
                              IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream rawStream =
                                     isf.CreateFile(filename))
                    {
                        StreamWriter writer = new StreamWriter(rawStream);
                        writer.Write(text);
                        writer.Close();
                    }
                }
            }

            private static bool loadText(string filename, out string result)
            {
                result = "";
                using (IsolatedStorageFile isf =
                    IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isf.FileExists(filename))
                    {
                        try
                        {
                            using (IsolatedStorageFileStream rawStream =
                                isf.OpenFile(filename, System.IO.FileMode.Open))
                            {
                                StreamReader reader = new StreamReader(rawStream);
                                result = reader.ReadToEnd();
                                reader.Close();
                            }
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
    }


    
}
