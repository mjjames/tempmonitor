using System;

namespace TempMonitor
{
    public class AtmosphericCondition
    {
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public float Pressure { get; set; }

        internal string ToJson()
        {
            return "{" +
                $"\"TimeStamp\": \"{TimeStamp.ToString("yyyy-MM-ddTHH:mm:ss")}\"," +
                $"\"Temperature\": {Temperature}," +
                $"\"Humidity\": {Humidity}," +
                $"\"Pressure\": {Pressure}" +
            "}";
        }
    }
}
