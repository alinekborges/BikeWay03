using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BikeWay03.ViewModels
{
    class PivotPageViewModel : INotifyPropertyChanged
    {

        public StationModel station { get; set; }
        public ObservableCollection<StationModel> favoriteList { get; set; }
        public ObservableCollection<StationModel> nearbyList { get; set; }



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
                    this.NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<StationModel> FavoriteList
        {
            get
            {
                return this.favoriteList;
            }

            set
            {
                if (this.nearbyList != value)
                {
                    this.nearbyList = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<StationModel> NearbyList
        {
            get
            {
                return this.nearbyList;
            }

            set
            {
                if (this.favoriteList != value)
                {
                    this.favoriteList = value;
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
