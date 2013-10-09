using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeWay03.ViewModels
{
     [Table("station")]
    public class StationBase
    {
        [PrimaryKey]
        public int id { get; set; }
        public string name { get; set; }
        public int lat { get; set; }
        public int lng { get; set; }
        public int number { get; set; }
        public int bikes { get; set; }
        public int free { get; set; }
        public string station_url { get; set; }
        public double percentage { get; set; }
        public string _bikes_string { get; set; }
        public string _free_string { get; set; }
        private bool isFavorite = false;
        public bool IsFavorite
        {
            get { return this.isFavorite; }
            set { this.isFavorite = value; }
        }
    }
}
