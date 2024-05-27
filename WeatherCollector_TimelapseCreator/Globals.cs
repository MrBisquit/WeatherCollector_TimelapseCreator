using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace WeatherCollector_TimelapseCreator
{
    public static class Globals
    {
        public static MainPage MainPage;
        public static Core.Config Config;
        public static string AppDataBase = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WTDawson", "WeatherCollector_TimelapseCreator");
        public static Core.ServerInfo ServerInfo;
        public static Bitmap Preview;
        public static Bitmap BasePreview;
        public static Core.Info Info;
    }
}
