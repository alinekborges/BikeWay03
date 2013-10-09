using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeWay03.ViewModels
{
    [Table("network")]
    public class NetworkModel
    {

        //{"city": "", "name": "valenbisi", "url": "http://api.citybik.es/valenbisi.json", "radius": 20000, "lat": 39456480, "lng": -355509, "id": 0
        [PrimaryKey]
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public int radius { get; set; }
        public int lat { get; set; }
        public int lng { get; set; }
    }
}
