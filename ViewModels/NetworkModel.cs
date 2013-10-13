using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BikeWay03.ViewModels
{

    public class NetworkModel : INotifyPropertyChanged
    {
        //{"city": "", "name": "valenbisi", "url": "http://api.citybik.es/valenbisi.json", "radius": 20000, "lat": 39456480, "lng": -355509, "id": 0

        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public int radius { get; set; }
        public int lat { get; set; }
        public int lng { get; set; }

        public NetworkBase getNetworkBase()
        {
            return new NetworkBase
            {
                    id = this.id,
                    name = this.name,
                    url = this.url,
                    radius = this.radius,
                    lat = this.lat,
                    lng = this.lng
            };
        }

        public static NetworkModel getNetworkModel(NetworkBase network)
        {
            return new NetworkModel
            {
                id = network.id,
                name = network.name,
                url = network.url,
                radius = network.radius,
                lat = network.lat,
                lng = network.lng
            };
        }

        private GeoCoordinate _geoCoordinate;

        public GeoCoordinate GeoCoordinate
        {
            get
            {
                if (this._geoCoordinate == null)
                {
                    _geoCoordinate = new GeoCoordinate(lat / 1e6, lng / 1e6);
                }
                return this._geoCoordinate;
            }

            set
            {
                if (this._geoCoordinate != value)
                {
                    this._geoCoordinate = value;
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
