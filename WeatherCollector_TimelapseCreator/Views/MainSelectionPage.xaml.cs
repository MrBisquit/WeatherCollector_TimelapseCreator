using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.UI.Popups;

namespace WeatherCollector_TimelapseCreator.Views
{
    public sealed partial class MainSelectionPage : Page
    {
        public MainSelectionPage()
        {
            this.InitializeComponent();
            LoadInfo();
            LoadPreview();
            Debug.WriteLine("ServerImageTest");
            Debug.Indent();
            Debug.WriteLine("PixelWidth: " + Globals.ServerInfo.ServerImageTest.PixelWidth);
            Debug.WriteLine("PixelHeight: " + Globals.ServerInfo.ServerImageTest.PixelHeight);
            Debug.IndentLevel = 0;
        }

        public async void LoadInfo()
        {
            TimeSpan SRTts = Globals.ServerInfo.ServerImageResponseTime;
            //SRT.Text = Globals.ServerInfo.ServerImageResponseTime.ToString("h'h 'm'm 's's'");
            SRT.Text = $"Server response time: {SRTts.Hours}h {SRTts.Minutes}m {SRTts.Seconds}s {SRTts.Milliseconds}ms";

            TimeSpan IGTts = Globals.Info.PreviewGenerationTime;

            IGT.Text = $"Image generation time: {IGTts.Hours}h {IGTts.Minutes}m {IGTts.Seconds}s {IGTts.Milliseconds}ms";

            // Do some calculations for working out how many days, dps and how long it'll take based on the CalendarDatePickers
            if(FromCDP.Date != null && ToCDP.Date != null)
            {
                DateTime a = FromCDP.Date.Value.DateTime;
                DateTime b = ToCDP.Date.Value.DateTime;

                if (b < a)
                {
                    Debug.WriteLine("INVALID_DATE_RANGE");
                } else
                {
                    IEnumerable<DateTime> dates = Core.Utils.GetDateRange(a, b);

                    // (Total amount of days * 288) * (Download time + Image generation time) = Total amount of time
                    int totalDays = dates.Count();
                    int totalDataPoints = totalDays * 288;
                    TimeSpan totalPerImage = Globals.ServerInfo.ServerImageResponseTime.Add(Globals.Info.PreviewGenerationTime).Multiply(totalDataPoints);

                    TI.Text = $"Total images: {totalDataPoints}";
                    ETA.Text = $"Estimated time: {totalPerImage.Hours}h {totalPerImage.Minutes}m {totalPerImage.Seconds}s";

                    Globals.Info.DateRange = dates.ToList();
                    Globals.Info.TotalPerImageTime = totalPerImage;
                }
            }
        }

        public async void LoadPreview()
        {
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            Globals.Preview = new System.Drawing.Bitmap(Globals.BasePreview);

            Core.Types.DataPoint dp = Globals.ServerInfo.DataRequestCurrentDay.DataPoints[Globals.ServerInfo.DataRequestCurrentDay.DataPoints.Count - 1];

            if(ShowWatermark.IsChecked == true)
            {
                Globals.Preview = Core.BitmapFeatures.AddWatermark(Globals.Preview, Watermark.Text);
            }

            if (ShowDT.IsChecked == true)
            {
                //DateTime dt = DateTime.ParseExact(dp.Date.Split(".")[0], "yyyy-MM-ddTHH:mm", System.Globalization.CultureInfo.InvariantCulture);
                Globals.Preview = Core.BitmapFeatures.AddDataTime(Globals.Preview, Core.Utils.ParseDateString(dp.Date), ((ComboBoxItem)DTFormat.Items[DTFormat.SelectedIndex]).Content.ToString(), fontSize:80);
            }

            if(ShowData.IsChecked == true)
            {
                Globals.Preview = Core.BitmapFeatures.AddData(Globals.Preview, dp, fontSize:80);
            }

            sw.Stop();
            Globals.Info.PreviewGenerationTime = sw.Elapsed;

            Preview.Source = await Core.Utils.ConvertBitmapToBitmapImageAsync(Globals.Preview);

            LoadInfo(); // Update how long it took

            Debug.WriteLine(Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path);
            Debug.WriteLine(Path.Combine(Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path, "..", "LocalCache", "Roaming"));
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadPreview();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            App.Current.JsonNavigationViewService.NavigateTo("WeatherCollector_TimelapseCreator.Views.HomeLandingPage");
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            Globals.Info.WatermarkEnabled = ShowWatermark.IsChecked == true;
            Globals.Info.Watermark = Watermark.Text;

            Globals.Info.DateTimeEnabled = ShowDT.IsChecked == true;
            Globals.Info.DateTimeFormat = ((ComboBoxItem)DTFormat.Items[DTFormat.SelectedIndex]).Content.ToString();

            Globals.Info.DataEnabled = ShowData.IsChecked == true;

            Globals.Info.FrameRate = int.Parse(FrameRate.Text);

            // Redirect and begin process
            App.Current.JsonNavigationViewService.NavigateTo("WeatherCollector_TimelapseCreator.Views.MainPreparingPage");
        }
    }
}
