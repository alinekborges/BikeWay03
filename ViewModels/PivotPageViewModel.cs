using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BikeWay03.ViewModels;
using System.Diagnostics;

namespace BikeWay03.ViewModels
{
    public class PivotPageViewModel : INotifyPropertyChanged
    {

        private StationModel station { get; set; }
        public ObservableCollection<StationModel> favoriteList { get; set; }
        public ObservableCollection<StationModel> nearbyList { get; set; }

        public PivotPageViewModel()
        {
            this.favoriteList = new ObservableCollection<StationModel>();
            this.nearbyList = new ObservableCollection<StationModel>();
        }

        public PivotPageViewModel(StationModel station) 
        {
            this.station = station;
            this.favoriteList = new ObservableCollection<StationModel>();
            this.nearbyList = new ObservableCollection<StationModel>();

            

            

            //this.favoriteList.Add(App.StationListViewModel.StationList[50]);
            //this.favoriteList.Add(App.StationListViewModel.StationList[53]);
            //this.favoriteList.Add(App.StationListViewModel.StationList[54]);
        }


        public ObservableCollection<StationModel> calculateNearbyList(StationModel station)
        {

            for (int i = 1; i < 8; i++)
            {
                this.nearbyList.Add(App.StationListViewModel.StationList[i]);

            }

            //this.favoriteList.Add(App.StationListViewModel.StationList[50]);
            //this.favoriteList.Add(App.StationListViewModel.StationList[53]);
            //this.favoriteList.Add(App.StationListViewModel.StationList[54]);

            return this.nearbyList;
        }

        public StationModel Station
        {
            get
            {
                return this.station;
            }

            set
            {
                if (this.station != value)
                {
                    
                    this.station = value;
                    calculateNearbyList(this.station);
                    this.NotifyPropertyChanged();
                }
            }
        }

   

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }
}
