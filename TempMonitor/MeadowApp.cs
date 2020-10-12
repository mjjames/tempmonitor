using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TempMonitor
{
	public class MeadowApp : App<F7Micro, MeadowApp>
	{
		private readonly string _wifiSSID = "{changeme}";
		private readonly string _wifiPassword = "{changeme}";
		private readonly II2cBus _i2c;
		private readonly DeviceOutputService _deviceOutputService;
		private readonly TemperatureMonitor _tempMonitorService;
		private readonly TimeService _timeService;
		private readonly TempMonitorRecorder _tempMonitorRecorder;
		private readonly Logger _logger;
		private static readonly Uri _tempMonitorUri = new Uri("{changeme}");

		public MeadowApp()
		{
			_logger = new Logger();
			_logger.LogMessage(() => "Initialize hardware...");

			_i2c = Device.CreateI2cBus();
			_deviceOutputService = new DeviceOutputService(Device, Device.Pins);
			_tempMonitorService = new TemperatureMonitor(_i2c, _logger);

			var httpClient = new HttpClient
			{
				BaseAddress = _tempMonitorUri
			};

			bool isWifiConnectedFactory() => (Device.WiFiAdapter?.IsConnected).GetValueOrDefault();
			Task<bool> initWifiFactory() => Device.InitWiFiAdapter();
			
			_timeService = new TimeService(httpClient, Device, _logger);
			_tempMonitorRecorder = new TempMonitorRecorder(httpClient, isWifiConnectedFactory, initWifiFactory, _logger);

			_tempMonitorService.PropertyChanged += async (o, e) =>
			{
				if (e.PropertyName == nameof(TemperatureMonitor.CurrentReading))
				{
					var reading = _tempMonitorService.CurrentReading;
					_logger.LogMessage(() => "New Reading");
					_deviceOutputService.UpdateDisplayAndLed(reading);
					_logger.LogMessage(() => "Display Updated");
					try
					{
						_logger.LogMessage(() => "Submitting Reading");
						await _tempMonitorRecorder.SubmitReadingAsync(reading);
						_logger.LogMessage(() => "Submitted Reading");
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Failed to Submit Reading: {ex}");
					}
				}
			};
			_deviceOutputService.UpdateDisplayAndLed(_tempMonitorService.CurrentReading);
			_ = ConfigureWifi();
		}

		private async Task ConfigureWifi()
		{
			try
			{
				_logger.LogMessage(() => "Init WiFi Adapter");
				var initResult = await Device.InitWiFiAdapter();
				if (!initResult)
				{
					_logger.LogMessage(() => "Failed to Init WiFi Adapter");
				}
				_logger.LogMessage(() => "Attempting to connect to Federation");
				var result = Device.WiFiAdapter.Connect(_wifiSSID, _wifiPassword);
				_logger.LogMessage(() => $"WiFi Connection Result: {result.ConnectionStatus}");
				if (result.ConnectionStatus == Meadow.Gateway.WiFi.ConnectionStatus.Success)
				{
					await _timeService.UpdateDeviceTimeAsync();
					await _tempMonitorRecorder.SubmitReadingAsync(_tempMonitorService.CurrentReading);
				}
			}
			catch (Exception ex)
			{
				_logger.LogMessage(() => $"Failed to Configure Wifi: \r\n{ex}");
			}
		}
	}
}
