using BikeWay03.DataServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BikeWay03.ViewModels;
using BikeWay03.Util;
using BikeWay03.DB;

namespace BikeWay03.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<StationModel> StationList { get; set; }
        public ObservableCollection<NetworkModel> NetworkList { get; set; }
        private NetworkModel network { get; set; }
        public bool IsDataLoaded;
        public bool isNetworkLoaded;
        public bool isStationsLoaded;
        

        public MainViewModel()
        {
            this.StationList = new ObservableCollection<StationModel>();
            this.NetworkList = new ObservableCollection<NetworkModel>();
                //LoadDataFromAPI();
                //this.IsDataLoaded = true;
            
        }

        public NetworkModel Network
        {
            get
            {
                return network;
            }
            set
            {
                if (this.network != value)
                {
                    
                    Debug.WriteLine("changing network to " + value.name);
                    MainPage.Status = "Downloading stations for " + value.name;
                    DataService.GetStationList(value);

                }
            }
        }

        public void LoadNetworks()
        {
            if (this.isNetworkLoaded == true)
                return;
            
            if (Settings.FirstTimeLaunch == true || Settings.IsNetworkSavedToDatabase == false)
            {
                DataService.GetNetworkList();
                this.isNetworkLoaded = true;
                return;
                
            }

            if (Settings.IsNetworkSavedToDatabase == true)
            {
                Database.getAllNetworks();
            }

            



        }

        public void LoadData()
        {
            if (this.isNetworkLoaded == true)
                return;

            if (Settings.FirstTimeLaunch == true || Settings.IsNetworkSavedToDatabase == false)
            {
                DataService.GetNetworkList();
                this.isNetworkLoaded = true;
                return;
            }            

            if (Settings.IsNetworkSavedToDatabase == true)
            {
                Database.getAllNetworksAndStations();
            }


        }





    }
}
