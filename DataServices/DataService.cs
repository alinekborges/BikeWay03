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

namespace BikeWay03.DataServices
{
    public static class DataService
    {
        public static string filename = "stations.txt";
        public static string result;

        public static string networkUrl = "http://api.citybik.es/networks.json";


        public static void GetStationList(string urlToCall)
        {
            if (App.OfflineMode == true)
            {
                GetStationListOffline();
            }
            else
            {
                WebClient client = new WebClient();
                client.DownloadStringCompleted += client_DownloadStringCompleted;
                client.DownloadStringAsync(new Uri(urlToCall));
            }
        }

        public static void GetNetworkList()
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += client_DownloadStringNetworkCompleted;
            client.DownloadStringAsync(new Uri(networkUrl));
        }

        public static bool GetStationListOffline()
        {
            string stations;

            if (loadText(filename, out stations) == true)
            {
                Debug.WriteLine("reding saved stations");
                Debug.WriteLine(stations);

                List<StationModel> stationList = JsonConvert.DeserializeObject<List<StationModel>>(stations);


                if (stationList != null)
                {
                    

                    foreach (StationModel station in stationList)
                    {
                        App.MainViewModel.StationList.Add(station);
                    }

                   
                    
                    //MainPage.UpdatePushpinsOnTheMap(App.StationListViewModel.StationList);
                    //Database.initializeDatabase();
                    
                    return true;

                }
                else
                {
                    return false;
                }

                
            }
            else
            {
                return false;
            }

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
                    
                    foreach (StationModel station in stationList)
                    {
                        App.MainViewModel.StationList.Add(station);                       
                    }
                    Database.SaveStations(App.MainViewModel.StationList);
                    App.MainPage.UpdatePushpinsOnTheMap(App.MainViewModel.StationList);
                }

            }
            else
            {
                Debug.WriteLine("ERROR downloading api");
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
                        //App.StationListViewModel.StationList.Add(station);
                    }

                    Database.SaveNetworks(App.MainViewModel.NetworkList);
                    Debug.WriteLine("network size = " + networkList.Count);
                }

            }
            else
            {
                Debug.WriteLine("ERROR downloading api");
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
