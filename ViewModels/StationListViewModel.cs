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

namespace BikeWay03.ViewModels
{
    public class StationListViewModel
    {
        public ObservableCollection<StationModel> StationList { get; set; }
        public bool IsDataLoaded;
        

        public StationListViewModel()
        {
            this.StationList = new ObservableCollection<StationModel>();
           
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

        }





    }
}
