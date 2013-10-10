using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeWay03.ViewModels
{
    [Table("network")]
    public class NetworkBase
    {
        [PrimaryKey]
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public int radius { get; set; }
        public int lat { get; set; }
        public int lng { get; set; }
    }
}
