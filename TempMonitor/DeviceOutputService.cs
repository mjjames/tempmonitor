using Meadow.Foundation;
using Meadow.Foundation.Displays.Lcd;
using Meadow.Foundation.Leds;
using Meadow.Hardware;
using Meadow.Peripherals.Leds;
using static Meadow.Devices.F7Micro;

namespace TempMonitor
{
    class DeviceOutputService
    {
        private readonly RgbPwmLed _onboardLed;
        private readonly RgbPwmLed _tempLed;
        private readonly CharacterDisplay _display;

        public DeviceOutputService(IIODevice device, F7MicroPinDefinitions pins)
        {
            _onboardLed = new RgbPwmLed(device: device,
                                        redPwmPin: pins.OnboardLedRed,
                                        greenPwmPin: pins.OnboardLedGreen,
                                        bluePwmPin: pins.OnboardLedBlue,
                                        3.3f, 3.3f, 3.3f,
                                        IRgbLed.CommonType.CommonAnode);

            _tempLed = new RgbPwmLed(device, pins.D02, pins.D03, pins.D04);
            _display = new CharacterDisplay(
                device,
                pinRS: pins.D10,
                pinE: pins.D11,
                pinD4: pins.D12,
                pinD5: pins.D13,
                pinD6: pins.D14,
                pinD7: pins.D15,
                rows: 2, columns: 16
            );
        }

        public void UpdateDisplayAndLed(Reading reading)
        {
            _display.WriteLine($"{reading.Temperature:0.#}c   |  {reading.Humidity:0.#}%", 0);
            _display.WriteLine($"{reading.MinimumTemperature:0.#}c   |  {reading.MaximumTemperature:0.#}c", 1);
            var color = GetColor(reading.Temperature);
            _onboardLed.SetColor(color);
            _tempLed.SetColor(color);
        }

        private Color GetColor(float temperature) => temperature switch
        {
            var t when t <= 16 => Color.LightBlue,
            var t when t <= 18 => Color.Blue,
            var t when t <= 21 => Color.Green,
            var t when t <= 24 => Color.Orange,
            var t when t >= 25 => Color.OrangeRed,
            var t when t >= 28 => Color.Red,
            _ => Color.White
        };
    }
}
