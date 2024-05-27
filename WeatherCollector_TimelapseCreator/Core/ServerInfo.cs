using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using WeatherCollector_TimelapseCreator.Core.Types;

namespace WeatherCollector_TimelapseCreator.Core
{
    public class ServerInfo
    {
        public TimeSpan ServerImageResponseTime { get; set; }
        public BitmapImage ServerImageTest { get; set; }
        public DataRequest DataRequest { get; set; }
        public Day DataRequestCurrentDay { get; set; }
    }

    public class Info
    {
        public TimeSpan PreviewGenerationTime { get; set; }
        public TimeSpan TotalPerImageTime { get; set; }
        public List<DateTime> DateRange { get; set; }

        public bool WatermarkEnabled { get; set; }
        public string Watermark { get; set; }

        public bool DateTimeEnabled { get; set; }
        public string DateTimeFormat { get; set; }

        public bool DataEnabled { get; set; }

        public int FrameRate { get; set; }
    }
}
