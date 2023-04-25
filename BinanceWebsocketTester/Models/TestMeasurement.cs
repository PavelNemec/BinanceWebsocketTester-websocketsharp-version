using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinanceWebsocketTester.Models
{
    public class TestMeasurement
    {
        public string IPAddress { get; set; }
        public DateTime Time { get; set; }
        public long ResponseTimeInTicks { get; set; }

        public int MeasurementGroupId { get; set; }
    }
}
