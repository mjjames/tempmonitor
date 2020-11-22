using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Meadow.Hardware;

namespace TempMonitor
{
    internal class TimeService
    {
        private const string TimeApiEndpoint = "api/time";
        private readonly HttpClient _httpClient;
        private readonly IIODevice _device;
        private readonly Logger _logger;

        public TimeService(HttpClient httpClient, IIODevice device, Logger logger)
        {
            _httpClient = httpClient;
            _device = device;
            _logger = logger;
        }

        public async Task UpdateDeviceTimeAsync()
        {
            _logger.LogMessage(() => "Fetching Time");
            string timeResult = string.Empty;
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif
            using (var timeResponse = await _httpClient.GetAsync(TimeApiEndpoint))
            {
                _logger.LogMessage(() => $"Time Result: {timeResponse.StatusCode}");
                if (timeResponse.IsSuccessStatusCode)
                {
                    timeResult = await timeResponse.Content.ReadAsStringAsync();
#if DEBUG
                    _logger.LogMessage(() => $"Time Result: {timeResult} Took :{sw.ElapsedMilliseconds}ms");
#endif
                }
            }
            _logger.LogMessage(() => "Time Response Disposed");
            timeResult = ParseTimeResult(timeResult);
            _logger.LogMessage(() => $"Updated TimeResult: {timeResult}");
            if (DateTime.TryParse(timeResult, out var serverDateTime))
            {
                _logger.LogMessage(() => "Setting Time");
                _device.SetClock(serverDateTime);
                _logger.LogMessage(() => $"Device Time: {DateTime.Now}");
            }
            else
            {
                _logger.LogMessage(() => "Failed to Parse Date Time");
            }
        }

        private static string ParseTimeResult(string timeResult) => timeResult[1..^1];
    }
}
