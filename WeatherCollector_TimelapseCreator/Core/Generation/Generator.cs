using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFMpegCore;
using FFMpegCore.Enums;
using Newtonsoft.Json;
using static WinUICommunity.Localizer;

namespace WeatherCollector_TimelapseCreator.Core.Generation
{
    public class Generator
    {
        Info info;
        public string basePath = null;
        public string imagesPath = null;
        public string modifiedImagesPath = null;
        public string id = null;
        public Generator(Info info)
        {
            this.info = info;
        }

        public void GeneratePaths(string basePath = default, string imagesPath = default)
        {
            id = Guid.NewGuid().ToString();
            if(basePath == default) this.basePath = Path.Combine(Globals.AppDataBase, id);
            else this.basePath = basePath;
            Debug.WriteLine(this.basePath);

            if(!Directory.Exists(this.basePath)) Directory.CreateDirectory(this.basePath);
            Debug.WriteLine(imagesPath);

            if (imagesPath == default) this.imagesPath = Path.Combine(this.basePath, "images");
            else this.imagesPath = imagesPath;

            if(!Directory.Exists(this.imagesPath)) Directory.CreateDirectory(this.imagesPath);

            if(this.modifiedImagesPath == null)
            {
                this.modifiedImagesPath = Path.Combine(this.imagesPath, "modified");
            }

            if(!Directory.Exists(this.modifiedImagesPath)) Directory.CreateDirectory(this.modifiedImagesPath);
        }

        public class Image
        {
            public string PathToImage { get; set; }
            public string PathToModifiedImage { get; set; }
            public Types.DataPoint AssociatedDataPoint { get; set; }
        }

        public Image[] images { get { return _images.ToArray(); } }
        private List<Image> _images = new List<Image>();

        public class Progress
        {
            public string StepText = "Downloading... (1/3)";
            public int Step = 0;
            public string PStepText = "Downloading images... (0/288)";
            public int ProgressStep = 0;
            public bool Indeterminate = false;
            public bool Completed = false;
        }

        public async Task StartGeneration(IProgress<Progress> progress)
        {
            if (info == null || basePath == null || imagesPath == null) return; // Don't do anything if there's no information present

            int w = 100;
            int h = 100;

            // Begin downloading the images
            Debug.WriteLine(info.DateRange.Count + " total days");
            for (int i = 0; i < info.DateRange.Count; i++)
            {
                Types.Day day = await Data.RequestDay(Globals.Config.AuthToken, info.DateRange[i].Year.ToString(), info.DateRange[i].Month.ToString(), info.DateRange[i].Day.ToString());

                if(day.DataPoints == null) continue;

                for (int j = 0; j < day.DataPoints.Count; j++)
                {
                    string id = Guid.NewGuid().ToString();
                    Image image = new Image()
                    {
                        PathToImage = Path.Combine(imagesPath, $"{id}.bmp"),
                        //PathToModifiedImage = Path.Combine(modifiedImagesPath, $"{id}.bmp"),
                        PathToModifiedImage = Path.Combine(modifiedImagesPath, $"img{j.ToString().PadLeft(3)}.bmp"), // For FFMPEG
                        AssociatedDataPoint = day.DataPoints[j]
                    };

                    Debug.WriteLine("Downloaded an image.");
                    progress.Report(new Progress() { Step = 100 / 3, StepText = "Downloading... (1/3)", ProgressStep = (int)(((double)i / (double)info.DateRange.Count) * 100), PStepText = $"Downloading images... ({j}/{info.DateRange.Count * 288})" });

                    Bitmap bm;

                    // Check if it's in the cache
                    DateTime dt = Utils.ParseDateString(day.DataPoints[i].Date);
                    Cache.CacheItem? _cacheItem = Cache.CacheManager.SearchForCacheItem(dt.Year.ToString(), dt.Month.ToString(), dt.Day.ToString(), j.ToString());
                    if (_cacheItem != null)
                    {
                        bm = new Bitmap(new Bitmap(_cacheItem.Location)); // Unlock it since it hates being loaded from files lol
                    }
                    else
                    {
                        try
                        {
                            bm = new Bitmap(await Data.RequestSpecificImageBitmap(Globals.Config.AuthToken, info.DateRange[i].Year.ToString(), info.DateRange[i].Month.ToString(), info.DateRange[i].Day.ToString(), j.ToString()));
                        }
                        catch
                        {
                            // Probably an issue related to something like rate-limiting or the server just gave up for a second
                            Debug.WriteLine("Failed, retrying in 1.5 seconds");
                            await Task.Delay(1500);
                            try
                            {
                                bm = new Bitmap(await Data.RequestSpecificImageBitmap(Globals.Config.AuthToken, info.DateRange[i].Year.ToString(), info.DateRange[i].Month.ToString(), info.DateRange[i].Day.ToString(), j.ToString()));
                            }
                            catch
                            {
                                // The server decided that it hates you
                                Debug.WriteLine("Failed, retrying in 5 seconds (Last attempt)");
                                await Task.Delay(5);
                                try
                                {
                                    bm = new Bitmap(await Data.RequestSpecificImageBitmap(Globals.Config.AuthToken, info.DateRange[i].Year.ToString(), info.DateRange[i].Month.ToString(), info.DateRange[i].Day.ToString(), j.ToString()));
                                }
                                catch
                                {
                                    bm = new Bitmap(w, h);
                                }
                            }
                        }
                    }

                    // Now add it to the cache if it wasn't in previously
                    if(_cacheItem == null)
                    {
                        Cache.CacheManager.CacheItem(dt.Year.ToString(), dt.Month.ToString(), dt.Day.ToString(), j.ToString(), bm);
                    }

                    if(i == 0 && j == 0)
                    {
                        w = bm.Width;
                        h = bm.Height;
                    }

                    bm.Save(image.PathToImage);
                    bm = null;
                    _cacheItem = null;
                    GC.Collect();

                    _images.Add(image);
                }

                //progress.Report(new Progress() { Step = 100 / 3, StepText = "Downloading... (1/3)", ProgressStep = (int)(((double)i / (double)info.DateRange.Count) * 100), PStepText = $"Downloading images... ({i * 288}/{info.DateRange.Count})" });
            }

            File.WriteAllText(Path.Combine(basePath, "images.json"), JsonConvert.SerializeObject(_images, Formatting.Indented));

            // Then we go over all of them, load them up, change them and then save them again
            progress.Report(new Progress() { Step = (100 / 3) * 2, StepText = "Formatting... (2/3)", ProgressStep = 0, PStepText = $"Formatting images... (0/{_images.Count})" });
            for (int i = 0; i < _images.Count; i++)
            {
                Debug.WriteLine("Attempting to format an image...");
                progress.Report(new Progress() { Step = (100 / 3) * 2, StepText = "Formatting... (2/3)", ProgressStep = (int)(((double)i / (double)info.DateRange.Count) * 100), PStepText = $"Formatting images... ({i}/{_images.Count})" });

                Bitmap bm = new Bitmap(new Bitmap(_images[i].PathToImage)); // Unlock it if locked
                Debug.WriteLine("Loaded an image...");

                //Types.DataPoint dp = Globals.ServerInfo.DataRequestCurrentDay.DataPoints[Globals.ServerInfo.DataRequestCurrentDay.DataPoints.Count - 1];
                Types.DataPoint dp = _images[i].AssociatedDataPoint;

                if (info.WatermarkEnabled)
                {
                    bm = BitmapFeatures.AddWatermark(bm, info.Watermark);
                }

                if (info.DateTimeEnabled)
                {
                    //DateTime dt = DateTime.ParseExact(dp.Date.Split(".")[0], "yyyy-MM-ddTHH:mm", System.Globalization.CultureInfo.InvariantCulture);
                    bm = BitmapFeatures.AddDataTime(bm, Utils.ParseDateString(dp.Date), info.DateTimeFormat, fontSize: 80);
                }

                if (info.DataEnabled)
                {
                    bm = BitmapFeatures.AddData(bm, dp, fontSize: 80);
                }

                Debug.WriteLine("Saving image...");
                bm.Save(_images[i].PathToModifiedImage); // Save the modified version to a different location

                bm = new Bitmap(1, 1);
                GC.Collect();
            }

            // Time for the actual timelapse bit
            //string localStateFolderPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path; // The real path (Path combine this at the start)
            string localStateFolderPath = Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path; // The real path (Path combine this at the start)

            List<string> _imgs = new List<string>(); // Convert to string[] when done
            for (var i = 0; i < _images.Count; i++)
            {
                //Debug.WriteLine(Path.Combine(localStateFolderPath, _images[i].PathToModifiedImage));
                //Debug.WriteLine(localStateFolderPath);
                //Debug.WriteLine(_images[i].PathToModifiedImage);
                Debug.WriteLine(Path.Combine(localStateFolderPath, _images[i].PathToModifiedImage.Substring(_images[i].PathToModifiedImage.IndexOf("Roaming", StringComparison.OrdinalIgnoreCase))));
                //_imgs.Add(Path.Combine(localStateFolderPath, _images[i].PathToModifiedImage));
                _imgs.Add("file '" + Path.Combine(localStateFolderPath, _images[i].PathToModifiedImage.Substring(_images[i].PathToModifiedImage.IndexOf("Roaming", StringComparison.OrdinalIgnoreCase))) + "'");
            }

            string _imgs_path = Path.Combine(localStateFolderPath, "Roaming", "WTDawson", "WeatherCollector_TimelapseCreator", id, "image_list.txt");
            File.WriteAllLines(_imgs_path, _imgs);

            try
            {
                Debug.WriteLine("Launching FFMPEG...");
                progress.Report(new Progress() { Step = 100, StepText = "Generating timelapse... (3/3)", ProgressStep = 0, PStepText = $"Running FFMPEG (Can take a while)", Indeterminate = true });
                Debug.WriteLine(_imgs.ToArray().Length);
                //FFMpeg.JoinImageSequence(Path.Combine(localStateFolderPath, "Roaming", "WTDawson", "WeatherCollector_TimelapseCreator", id, "output.mp4"), Globals.Info.FrameRate, _imgs.ToArray());

                // Temporary (I hope)
                string[] parts = _imgs[0].Split('\\');

                // Remove the last element
                Array.Resize(ref parts, parts.Length - 1);

                // Using some totally-not-dodgy-method-of-using-FFMPEG by Bing AI
                //string ffmpegCommand = $"-framerate 30 -pattern_type glob -i \"{string.Join("\\", parts)}\\*.bmp\" -s:v 1920x1080 -c:v libx264 -crf 17 -pix_fmt yuv420p \"{Path.Combine(localStateFolderPath, "Roaming", "WTDawson", "WeatherCollector_TimelapseCreator", id, "output.mp4")}\"";
                string ffmpegCommand = $"-r {info.FrameRate} -f concat -safe 0 -i \"{_imgs_path}\" -s:v {w}x{h} -c:v libx264 -crf 17 -pix_fmt yuv420p \"{Path.Combine(localStateFolderPath, "Roaming", "WTDawson", "WeatherCollector_TimelapseCreator", id, "output.mp4")}\"";
                Debug.WriteLine(ffmpegCommand);

                ProcessStartInfo processInfo = new ProcessStartInfo("ffmpeg", ffmpegCommand)
                {
                    CreateNoWindow = true,
                    //UseShellExecute = false,
                    //RedirectStandardOutput = true,
                    //RedirectStandardError = true
                };

                using (Process process = new Process())
                {
                    process.StartInfo = processInfo;
                    process.Start();
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                        Debug.WriteLine("Timelapse video created successfully!");
                    else
                        Debug.WriteLine("Error creating timelapse video.");
                }

                progress.Report(new Progress() { Step = 100, StepText = "Generating timelapse... (3/3)", ProgressStep = 100, PStepText = $"FFMPEG process exited", Indeterminate = false, Completed = true });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("FFMPEG failed, error: " + ex);
            }
        }
    }
}
