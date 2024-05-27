using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WeatherCollector_TimelapseCreator.Core
{
    public static class Utils
    {
        public static async Task<BitmapImage> ConvertBitmapToBitmapImageAsync(Bitmap _bitmap)
        {
            Bitmap bitmap = new Bitmap(_bitmap);
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                try
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        stream.Position = 0;
                        bitmapImage.SetSource(stream.AsRandomAccessStream());

                        return bitmapImage;
                    }
                }
                catch (ExternalException ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                    throw;
                }
            }
        }

        public static DateTime ParseDateString(string dateString)
        {
            string[] firstSplit = dateString.Split('T');
            string[] secondSplit = firstSplit[0].Split("-");

            int year = int.Parse(secondSplit[0]);
            int month = int.Parse(secondSplit[1]);
            int day = int.Parse(secondSplit[2]);

            string[] thirdSplit = firstSplit[1].Split(":");
            int hour = int.Parse(thirdSplit[0]);
            int minute = int.Parse(thirdSplit[1]);

            return new DateTime(year, month, day, hour, minute, 0);
        }

        // Bing AI
        public static IEnumerable<DateTime> GetDateRange(DateTime fromDate, DateTime toDate)
        {
            return Enumerable.Range(0, (int)(toDate - fromDate).TotalDays + 1)
                .Select(offset => fromDate.AddDays(offset));
        }
    }
}
