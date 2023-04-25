using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinanceWebsocketTester
{
    public class BinanceApiTesterFactory
    {
        public BinanceApiTesterFactory()
        {

        }

        public BinanceApiTester CreateApiTesterWithDifferentIPAddress(IEnumerable<string> ipAddresses, int mesurementGroupId)
        {
            if (!ipAddresses.Any())
            {
                return new BinanceApiTester(mesurementGroupId);
            }

            BinanceApiTester tester = null;

            for (int i = 0; i < 200; i++)
            {
                tester = new BinanceApiTester(mesurementGroupId);
                if (!ipAddresses.Any(e => e.Contains(tester.BinanceIPAddress.ToString())))
                {
                    break;
                }
                tester.Dispose();
                Console.WriteLine("Canceling connection because this IP address was already tested.");
                tester = null;
            }

            return tester;
        }
    }
}
