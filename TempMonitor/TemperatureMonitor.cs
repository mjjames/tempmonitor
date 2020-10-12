using Meadow;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors.Atmospheric;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TempMonitor
{
	class TemperatureMonitor : INotifyPropertyChanged
	{

		private const int DefaultMinimum = 100;
		private const int DefaultMaximum = -1;
		private const int DefaultValue = 0;
		private readonly Bme280 _bme280;
		private readonly Logger _logger;
		private float _currentTemperature = DefaultValue;
		private float _maximumTemperature = DefaultMaximum;
		private float _minimumTemperature = DefaultMinimum;
		private float _currentHumidity = -DefaultValue;
		private float _currentPressure = DefaultValue;

		public event PropertyChangedEventHandler PropertyChanged;

		public TemperatureMonitor(II2cBus i2CBus, Logger logger)
		{
			_logger = logger;
			_bme280 = new Bme280(i2CBus, Bme280.I2cAddress.Adddress0x77);
			_bme280.Subscribe(new FilterableChangeObserver<AtmosphericConditionChangeResult, AtmosphericConditions>(
				h => ProcessAtmosphericChange(h.New),
				e => Math.Abs(e.Delta.Temperature.GetValueOrDefault()) > 0.1
			));

			// get chip id
			_logger.LogMessage(() => $"BME280 ChipID: {_bme280.GetChipID():X2}");

			// get an initial reading
			ReadConditions().ContinueWith(t => _bme280.StartUpdating());

		}

		public Reading CurrentReading => new Reading
		{
			Humidity = _currentHumidity,
			MaximumTemperature = _maximumTemperature,
			MinimumTemperature = _minimumTemperature,
			Pressure = _currentPressure,
			Temperature = _currentTemperature
		};

		private void ProcessAtmosphericChange(AtmosphericConditions atmosphericConditions)
		{
			UpdateTemperature(atmosphericConditions.Temperature);
			_currentHumidity = atmosphericConditions.Humidity.GetValueOrDefault();
			_currentPressure = atmosphericConditions.Pressure.GetValueOrDefault();
			_logger.LogMessage(() => $"Temp: {_currentTemperature}C | Min: {_minimumTemperature}C | Max: {_maximumTemperature}\r\nHumidity: {_currentHumidity}% | Pressure: {_currentPressure}hPa");
			NotifyPropertyChanged(nameof(CurrentReading));
		}

		private void UpdateTemperature(float? temperature)
		{
			_currentTemperature = temperature.GetValueOrDefault();
			if (_currentTemperature > _maximumTemperature)
			{
				_maximumTemperature = _currentTemperature;
			}
			if (_currentTemperature < _minimumTemperature)
			{
				_minimumTemperature = _currentTemperature;
			}
		}

		protected async Task ReadConditions()
		{
			var conditions = await _bme280.Read();
			_logger.LogMessage(() => "Initial Readings:");
			_logger.LogMessage(() => $"  Temperature: {conditions.Temperature}ºC");
			_logger.LogMessage(() => $"  Pressure: {conditions.Pressure}hPa");
			_logger.LogMessage(() => $"  Relative Humidity: {conditions.Humidity}%");
			ProcessAtmosphericChange(conditions);
		}

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
