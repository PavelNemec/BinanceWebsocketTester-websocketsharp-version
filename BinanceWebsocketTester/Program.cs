using BinanceWebsocketTester;

// Show results
var allTestMeasurements = await ReportHelper.ReadAllCsvFilesAsync();
ReportHelper.WriteReportSummary(allTestMeasurements);
Console.WriteLine("Press any key to start new measurements...");
Console.ReadKey();
Console.WriteLine("Starting new measurements...");

// Start new mesurements
for (int numbOfMeasurements = 0; numbOfMeasurements < 48; numbOfMeasurements++)
{
    allTestMeasurements = await ReportHelper.ReadAllCsvFilesAsync();
    List<string> ipAdresses = new List<string>();

    var measurementGroupId = 1;
    if (allTestMeasurements.Any())
    {
        measurementGroupId = allTestMeasurements.Max(e => e.MeasurementGroupId) + 1;
    }

    Console.WriteLine();
    Console.WriteLine("Processing group measurement number " + measurementGroupId);
    var binanceFactory = new BinanceApiTesterFactory();
    while (true)
    {
        using (var binanceApiTester = binanceFactory.CreateApiTesterWithDifferentIPAddress(ipAdresses, measurementGroupId))
        {
            if (binanceApiTester == null)
            {
                break;
            }

            ipAdresses.Add(binanceApiTester.BinanceIPAddress);
            for (int i = 0; i < 200; i++)
            {
                await binanceApiTester.PerformTest();
            }
           
            await binanceApiTester.CreateReportAsync();
        }
    }

    Console.WriteLine("Group measurement number " + measurementGroupId + " completed.");
    Thread.Sleep(TimeSpan.FromMinutes(30));
}



