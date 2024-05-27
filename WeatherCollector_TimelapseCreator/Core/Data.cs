using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;
using WeatherCollector_TimelapseCreator.Core.Types;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace WeatherCollector_TimelapseCreator.Core
{
    static class Data
    {
        public static async Task<Types.Auth> RequestAuth(string pass)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };
            client.BaseAddress = new Uri("http://" + Globals.Config.ServerLocation, UriKind.Absolute);
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/login/?password=" + pass);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<Types.Auth>(await response.Content.ReadAsStringAsync());
        }

        public static async Task<Types.DataRequest> RequestFull(string auth)
        {
            if (string.IsNullOrEmpty(auth)) return null;

            var client = new HttpClient();
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };
            client.BaseAddress = new Uri("http://" + Globals.Config.ServerLocation, UriKind.Absolute);
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/full?format=CSJSON");
            //request.Headers.Add("authorization", auth);
            request.Headers.Add("auth", auth);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<Types.DataRequest>(await response.Content.ReadAsStringAsync());
        }

        public static async Task<Types.Day> RequestDay(string auth, string year, string month, string day)
        {
            if (string.IsNullOrEmpty(auth)) return null;

            var client = new HttpClient();
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };
            client.BaseAddress = new Uri("http://" + Globals.Config.ServerLocation, UriKind.Absolute);
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/data/{year}/{month}/{day}/");
            //request.Headers.Add("authorization", auth);
            request.Headers.Add("auth", auth);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<Types.Day>(await response.Content.ReadAsStringAsync());
        }

        /*public static async Task<BitmapImage> RequestLatestImage(string auth)
        {
            BitmapImage bitmapImage = new BitmapImage();

            var client = new HttpClient();
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };
            client.BaseAddress = new Uri("http://" + Globals.Config.ServerLocation, UriKind.Absolute);
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/latest/image/");
            request.Headers.Add("authorization", auth);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            using (Stream inputStream = await response.Content.ReadAsStreamAsync())
            {
                Debug.WriteLine(inputStream.Length);
                bitmapImage.SetSource(inputStream.AsRandomAccessStream());
                Debug.WriteLine(bitmapImage.PixelWidth * bitmapImage.PixelHeight);
            }

            return bitmapImage;
        }*/

        public static async Task<BitmapImage> RequestLatestImage(string auth)
        {
            BitmapImage bitmapImage = new BitmapImage();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
                    {
                        NoCache = true,
                    };

                    client.BaseAddress = new Uri("http://" + Globals.Config.ServerLocation, UriKind.Absolute);

                    var request = new HttpRequestMessage(HttpMethod.Get, "/api/latest/image/?as=base64");
                    //request.Headers.Add("authorization", $"{auth}");
                    request.Headers.Add("auth", auth);
                    //request.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);

                    using (var response = await client.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();

                        // Read the response content as a string
                        string responseContent = await response.Content.ReadAsStringAsync();

                        // Log the first 20 characters of the Base64 string
                        string first20Characters = responseContent.Substring(0, Math.Min(responseContent.Length, 20));
                        Debug.WriteLine($"First 20 characters of Base64 string: {first20Characters}");

                        // Convert Base64 string to byte array
                        byte[] imageBytes = Convert.FromBase64String(responseContent);

                        // Create a stream from the byte array
                        using (MemoryStream memoryStream = new MemoryStream(imageBytes))
                        {
                            // Set the stream as the source for BitmapImage
                            await bitmapImage.SetSourceAsync(memoryStream.AsRandomAccessStream());
                        }

                        // Debugging output
                        Debug.WriteLine(bitmapImage.PixelWidth * bitmapImage.PixelHeight);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Debug.WriteLine($"Error: {ex}");
            }

            return bitmapImage;
        }

        public static async Task<Bitmap> RequestLatestImageBitmap(string auth)
        {
            Bitmap bitmap = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
                    {
                        NoCache = true,
                    };

                    client.BaseAddress = new Uri("http://" + Globals.Config.ServerLocation, UriKind.Absolute);

                    var request = new HttpRequestMessage(HttpMethod.Get, "/api/latest/image/?as=base64");
                    //request.Headers.Add("authorization", $"{auth}");
                    request.Headers.Add("auth", auth);
                    //request.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);

                    using (var response = await client.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();

                        // Read the response content as a string
                        string responseContent = await response.Content.ReadAsStringAsync();

                        // Log the first 20 characters of the Base64 string
                        string first20Characters = responseContent.Substring(0, Math.Min(responseContent.Length, 20));
                        Debug.WriteLine($"First 20 characters of Base64 string: {first20Characters}");

                        // Convert Base64 string to byte array
                        byte[] imageBytes = Convert.FromBase64String(responseContent);

                        // Create a stream from the byte array
                        using (MemoryStream memoryStream = new MemoryStream(imageBytes))
                        {
                            // Create a Bitmap from the memory stream
                            bitmap = new Bitmap(memoryStream);
                        }

                        // Debugging output
                        Debug.WriteLine(bitmap.Width * bitmap.Height);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Debug.WriteLine($"Error: {ex}");
            }

            return bitmap;
        }

        public static async Task<Bitmap> RequestSpecificImageBitmap(string auth, string year, string month, string day, string dp)
        {
            Bitmap bitmap = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
                    {
                        NoCache = true,
                    };

                    client.BaseAddress = new Uri("http://" + Globals.Config.ServerLocation, UriKind.Absolute);

                    var request = new HttpRequestMessage(HttpMethod.Get, $"/api/image/{year}/{month}/{day}/{dp}/?as=base64");
                    //request.Headers.Add("authorization", $"{auth}");
                    request.Headers.Add("auth", auth);
                    //request.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);

                    using (var response = await client.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();

                        // Read the response content as a string
                        string responseContent = await response.Content.ReadAsStringAsync();

                        // Log the first 20 characters of the Base64 string
                        string first20Characters = responseContent.Substring(0, Math.Min(responseContent.Length, 20));
                        Debug.WriteLine($"First 20 characters of Base64 string: {first20Characters}");

                        // Convert Base64 string to byte array
                        byte[] imageBytes = Convert.FromBase64String(responseContent);

                        // Create a stream from the byte array
                        using (MemoryStream memoryStream = new MemoryStream(imageBytes))
                        {
                            // Create a Bitmap from the memory stream
                            bitmap = new Bitmap(memoryStream);
                        }

                        // Debugging output
                        Debug.WriteLine(bitmap.Width * bitmap.Height);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Debug.WriteLine($"Error: {ex}");
            }

            return bitmap;
        }
    }
}
