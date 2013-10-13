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
        public bool IsDataLoaded;
        

        public MainViewModel()
        {
            this.StationList = new ObservableCollection<StationModel>();
            this.NetworkList = new ObservableCollection<NetworkModel>();
                //LoadDataFromAPI();
                //this.IsDataLoaded = true;
            
        }

       

        public void LoadDataFromAPI()
        {
            if (this.IsDataLoaded == false)
            {
                DataService.GetStationList("http://api.citybik.es/citycycle.json");
                this.IsDataLoaded = true;
            }

            if (Settings.IsNetworkSavedToDatabase == false)
            {
                DataService.GetNetworkList();
            }
            else
            {
                Database.getAllNetworks();
            }

        }





    }
}
