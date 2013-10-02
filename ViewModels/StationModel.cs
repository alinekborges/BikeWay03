using Microsoft.Phone.Maps.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BikeWay03
{
    public class StationModel : INotifyPropertyChanged
    {
        public int id { get; set; }
        public string name { get; set; }
        public int lat { get; set; }
        public int lng { get; set; }
        public int number { get; set; }
        public int bikes { get; set; }
        public int free { get; set; }
        public string station_url { get; set; }

        public int ID
        {
            get
            {
                return this.id;
            }

            set
            {
                if (this.id != value)
                {
                    this.id = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.NotifyPropertyChanged();
                }
            }

        }
        //public int lat { get; set; }
        //public int lng { get; set; }
        //public int number { get; set; }
        //public int bikes { get; set; }
        //public int free { get; set; }
        //public string station_url { get; set; }


        private GeoCoordinate _geoCoordinate;

        [TypeConverter(typeof(GeoCoordinateConverter))]
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



        //        id: CityBikes station id
        //name: Station name
        //lat: Latitude in E6 format
        //lng: Longitude in E6 format
        //bikes: Number of bikes in the station
        //free: Number of free slots
        //timestamp: The last time the station has been updated

        /// <summary>
        /// Generic NotifyPropertyChanged
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        /// 

        /// <summary>
        /// Event to be raised when a property value has changed
        /// </summary>
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
