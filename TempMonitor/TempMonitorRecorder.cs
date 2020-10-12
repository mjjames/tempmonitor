using Meadow.Gateway.WiFi;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TempMonitor
{
	internal class TempMonitorRecorder
	{
		private const int DefaultYearWhenDateNotSet = 1970;
		private const string ApiEndpoint = "api/AtmosphericConditions";
		private const string JsonContentType = "application/json";
		private readonly HttpClient _httpClient;
		private readonly Func<bool> _isWifiConnected;
		private readonly Func<Task<bool>> _initWifiFactory;
		private readonly Logger _logger;

		public TempMonitorRecorder(HttpClient httpClient, Func<bool> isWifiConnected, Func<Task<bool>> initWifiFactory, Logger logger)
		{
			_httpClient = httpClient;
			_isWifiConnected = isWifiConnected;
			_initWifiFactory = initWifiFactory;
			_logger = logger;
		}

		public async Task SubmitReadingAsync(Reading reading)
		{
			if (reading is null)
			{
				_logger.LogMessage(() => "Unable to Submit Reading, Null Reading");
				return;
			}
			if (!_isWifiConnected())
			{
				_logger.LogMessage(() => "Unable to Submit Reading, WiFI Adapter Not Connected");
				return;
			}
			if (DateTime.Now.Year == DefaultYearWhenDateNotSet)
			{
				_logger.LogMessage(() => "Unable to Submit Reading, Time Not Set");
				return;
			}
			var condition = new AtmosphericCondition
			{
				Humidity = reading.Humidity,
				Pressure = reading.Pressure,
				Temperature = reading.Temperature
			};
			try
			{
				_logger.LogMessage(() => $"Submitting Reading to Remote Logger");
#if DEBUG
				var sw = Stopwatch.StartNew();
#endif
				using (var jsonContent = new StringContent(condition.ToJson(), Encoding.UTF8, JsonContentType))
				using (var result = await _httpClient.PostAsync(ApiEndpoint, jsonContent))
				{
#if DEBUG
					_logger.LogMessage(() => $"Submit Reading Result: {result.StatusCode} Took: {sw.ElapsedMilliseconds}ms");
#endif
				}
			}
			catch (TimeoutException)
			{
				_logger.LogMessage(() => "Request Timed Out, resetting Wifi Adapter");
				var initResult = await _initWifiFactory();
				if (!initResult)
				{
					_logger.LogMessage(() => "Failed to Init WiFi Adapter");
				}
			}
			catch (Exception ex)
			{
				_logger.LogMessage(() => $"Failed to Submit Reading: Exception:\r\n{ex}");
			}
		}
	}
}
