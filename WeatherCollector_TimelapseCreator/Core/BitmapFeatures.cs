using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherCollector_TimelapseCreator.Core
{
    public static class BitmapFeatures
    {
        public static Bitmap AddWatermark(Bitmap _bitmap, string watermark, string font = "New Roman", int fontSize = 80)
        {
            Bitmap bitmap = new Bitmap(_bitmap); // Unlock it if it's locked

            Graphics g = Graphics.FromImage(bitmap);

            g.DrawString(watermark, new Font(font, fontSize), Brushes.DarkGray, new PointF((bitmap.Width / 2) - ((fontSize * (float)(watermark.Length / 1.15)) / 2), (bitmap.Height / 2) - (fontSize / 2)));

            return bitmap;
        }

        public static Bitmap AddDataTime(Bitmap _bitmap, DateTime dateTime, string format, string font = "New Roman", int fontSize = 40)
        {
            Bitmap bitmap = new Bitmap(_bitmap); // Unlock it if it's locked

            Graphics g = Graphics.FromImage(bitmap);

            string content = "";

            if(format == "dd/mm/yyyy hh:mm")
            {
                content = dateTime.ToString("dd/MM/yyyy HH:mm");
            } else if(format == "mm/dd/yyyy hh:mm")
            {
                content = dateTime.ToString("MM/dd/yyyy HH:mm");
            } else if(format == "hh:mm")
            {
                content = dateTime.ToString("HH:mm");
            } else if(format == "dd/mm/yyyy")
            {
                content = dateTime.ToString("dd/MM/yyyy");
            } else if(format == "mm/dd/yyyy")
            {
                content = dateTime.ToString("MM/dd/yyyy");
            }

            // At the bottom + 10 padding on both sides
            g.DrawString(content, new Font(font, fontSize), Brushes.DarkGray, new PointF(10, (bitmap.Height - fontSize) - 40)); // Was 30 now 40

            return bitmap;
        }

        public static Bitmap AddData(Bitmap _bitmap, Types.DataPoint dp, string font = "New Roman", int fontSize = 40)
        {
            Bitmap bitmap = new Bitmap(_bitmap); // Unlock it if it's locked

            Graphics g = Graphics.FromImage(bitmap);

            Font f = new Font(font, fontSize);
            Brush b = Brushes.DarkGray;

            string temp = $"{dp.Temp}°C";
            string humidity = $"{dp.Humidity}%";
            string rain = $"{dp.Rain} mm";
            string rain_per_second = $"{dp.RainPerSecond} mm/s";
            string rain_per_hour = $"{dp.RainPerSecond * 3600} mm/h";
            string wind_direction = $"{dp.WindDir}°";
            string wind_speed = $"{dp.WindSpeed} m/s ({dp.WindSpeed / 0.44704} mph)";
            string pressure = $"{dp.Pressure} hPa";
            string light = $"{dp.Light} lx";

            g.DrawString(temp, f, b, new PointF(10, 10));
            g.DrawString(humidity, f, b, new PointF(10, 10 + ((fontSize + 10) * 1)));
            g.DrawString(rain, f, b, new PointF(10, 10 + ((fontSize + 10) * 2)));
            g.DrawString(rain_per_second, f, b, new PointF(10, 10 + ((fontSize + 10) * 3)));
            g.DrawString(rain_per_hour, f, b, new PointF(10, 10 + ((fontSize + 10) * 4)));
            g.DrawString(wind_direction, f, b, new PointF(10, 10 + ((fontSize + 10) * 5)));
            g.DrawString(wind_speed, f, b, new PointF(10, 10 + ((fontSize + 10) * 6)));
            g.DrawString(pressure, f, b, new PointF(10, 10 + ((fontSize + 10) * 7)));
            g.DrawString(light, f, b, new PointF(10, 10 + ((fontSize + 10) * 8)));

            return bitmap;
        }
    }
}
