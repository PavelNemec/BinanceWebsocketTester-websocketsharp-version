using BinanceWebsocketTester.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using WebSocketSharp;
using WebSocket = WebSocketSharp.WebSocket;

namespace BinanceWebsocketTester
{
    public class BinanceApiTester : IDisposable
    {
        public List<TestMeasurement> Measurements = new List<TestMeasurement>();

        private string _binanceApiUrl = "wss://ws-api.binance.com:443/ws-api/v3";

        private WebSocket _websocketClient;

        private Stopwatch _stopWatch;

        private TaskCompletionSource<string> _responseTcs;

        private int _mesurementGroupId;

        private int _counter = 0;

        public string BinanceIPAddress { get; private set; }

        public BinanceApiTester(int mesurementGroupId)
        {
            _websocketClient = new WebSocket(_binanceApiUrl);
            _websocketClient.Connect();
            _websocketClient.OnMessage += OnMessage;

            _stopWatch = new Stopwatch();
            _mesurementGroupId = mesurementGroupId;
            BinanceIPAddress = _websocketClient.ServerIPAddress.ToString().Replace("::ffff:", "");
            Console.WriteLine("Created WebSocket Connection to IP Address: " + BinanceIPAddress);
        }

        public async Task PerformTest()
        {
            var testRequest = new
            {
                method = "time",
                id = "1d7d3c72-942d-484c-1271-4e21413badb1"
            };

            var json = JsonConvert.SerializeObject(testRequest);
            _responseTcs = new TaskCompletionSource<string>();
            
            _stopWatch.Restart();
            _websocketClient.Send(json);
             await _responseTcs.Task;
            _stopWatch.Stop();

            var newMeasurment = new TestMeasurement()
            {
                IPAddress = BinanceIPAddress.ToString(),
                ResponseTimeInTicks = _stopWatch.ElapsedTicks,
                Time = DateTime.UtcNow,
                MeasurementGroupId = _mesurementGroupId
            };

            Measurements.Add(newMeasurment);

            Console.WriteLine("Ping numb. " + _counter++.ToString() + " " + newMeasurment.ResponseTimeInTicks);
        }

        public async Task CreateReportAsync()
        {
            if (Measurements.Any())
            {
                var reportName = Measurements.First().IPAddress.Replace(".", "-") + " " + DateTime.UtcNow.ToString("dd-MM HH-mm-ss") + ".csv";
                await ReportHelper.ExportToCsvAsync(Measurements, reportName);
            }
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            _responseTcs.SetResult(e.Data);
        }

        public void Dispose()
        {
            ((IDisposable)_websocketClient).Dispose();
        }
    }
}
