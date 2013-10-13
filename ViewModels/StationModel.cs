using Microsoft.Phone.Maps.Controls;
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
   
    public class StationModel : StationBase
    {
        

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


        public double distance;
        //public int lat { get; set; }
        //public int lng { get; set; }
        //public int number { get; set; }
        //public int bikes { get; set; }
        //public int free { get; set; }
        //public string station_url { get; set; }


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

        public StationBase getStationBase()
        {
            StationBase station = new StationBase
            {
                
                id = this.id,
                name = this.name,
                lat = this.lat,
                lng = this.lng,
                number = this.number,
                bikes = this.bikes,
                free = this.free,
                station_url =  this.station_url,
                percentage = this.percentage,
                _bikes_string  = this._bikes_string,
                _free_string = this._free_string,
                IsFavorite = this.IsFavorite
                
            };

            return station;
        }

        public string BikesString
        {
            get
            {
                if (string.IsNullOrEmpty(this._bikes_string))
                {
                    this._bikes_string = this.bikes.ToString();
                }
                return this._bikes_string;
            }

            set
            {
                if (this._bikes_string != value)
                {
                    this._bikes_string = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public string FreeString
        {
            get
            {
                if (string.IsNullOrEmpty(this._free_string))
                {
                    this._free_string = this.free.ToString();
                }
                return this._free_string;
            }

            set
            {
                if (this._free_string != value)
                {
                    this._free_string = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public static StationModel getStationModel(StationBase stationBase)
        {
            StationModel station = new StationModel
            {
                id = stationBase.id,
                name = stationBase.name,
                lat = stationBase.lat,
                lng = stationBase.lng,
                number = stationBase.number,
                bikes = stationBase.bikes,
                free = stationBase.free,
                station_url = stationBase.station_url,
                percentage = stationBase.percentage,
                _bikes_string = stationBase._bikes_string,
                _free_string = stationBase._free_string,
                IsFavorite = stationBase.IsFavorite
            };

            return station;
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
