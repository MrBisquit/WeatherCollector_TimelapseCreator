using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherCollector_TimelapseCreator.Core.Cache;
public class CacheItem
{
    public string Location { get; set; } = null;
    public string Year { get; set; } = null;
    public string Month { get; set; } = null;
    public string Day { get; set; } = null;
    public string DataPoint { get; set; } = null;
}
