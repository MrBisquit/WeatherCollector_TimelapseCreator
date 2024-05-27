using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherCollector_TimelapseCreator.Core.Types
{
    public class DataRequest
    {
        [JsonProperty("years")]
        public Dictionary<string, Year> Years { get; set; }
    }

    public class Year
    {
        [JsonProperty("months")]
        public Dictionary<string, Month> Months { get; set; }
    }

    public class Month
    {
        [JsonProperty("days")]
        public Dictionary<string, string> Days { get; set; }
    }

    public enum Months
    {
        January,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }

    public class Day
    {
        [JsonProperty("points")]
        public List<DataPoint> DataPoints;
    }

    public class AverageDay
    {
        public string Date;

        public float Temp;

        public float Humidity;

        public float WindSpeed;

        public float WindDir;

        public float Rain;
    }

    public class DataPoint
    {
        [JsonProperty("date")]
        public string Date; // Was DateTime but it complained

        [JsonProperty("temp")]
        public float Temp;

        [JsonProperty("humidity")]
        public float Humidity;

        [JsonProperty("pressure")]
        public float Pressure;

        [JsonProperty("light")]
        public float Light;

        [JsonProperty("wind_speed")]
        public float WindSpeed;

        [JsonProperty("rain")]
        public float Rain;

        [JsonProperty("rain_per_second")]
        public float RainPerSecond;

        [JsonProperty("wind_direction")]
        public float WindDir;
    }
}
