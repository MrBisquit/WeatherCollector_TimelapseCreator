using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WeatherCollector_TimelapseCreator.Core.Cache;
public static class CacheManager
{
    private static List<CacheItem> _CacheItems = new List<CacheItem>();

    public static void Initialise()
    {
        // Loads it in and checks over all of the files and make sure that they exist, if they don't, remove them from the list (And then if there's any modifications, save)
        // Also, vice versa, there's no point in having images taking up space if they aren't even indexed, since they'll never be used

        Load(); // Make sure it's loaded

        // Check over every one and make sure it exists
        for (var i = 0; i < _CacheItems.Count; i++)
        {
            if(!File.Exists(_CacheItems[i].Location))
            {
                _CacheItems.Remove(_CacheItems[i]); // Remove it from the list if it doesn't exist
            }
        }

        // Now loop through the entire directory and check it
        string[] files = Directory.GetFiles(Path.Combine(Globals.AppDataBase, "cache", "images"));
        for (var i = 0; i < files.Length; i++)
        {
            if (!SearchForCacheItem(files[i]))
            {
                File.Delete(files[i]); // Delete it if it doesn't exist, since it's not needed
            }
        }
    }

    public static void Save()
    {
        File.WriteAllText(Path.Combine(Globals.AppDataBase, "cache", "index.json"), JsonConvert.SerializeObject(_CacheItems));
    }

    public static void Load()
    {
        Debug.WriteLine("[Cache] Attempting to load...");
        PrepareDir(); // Avoid crashing
        try
        {
            _CacheItems = JsonConvert.DeserializeObject<List<CacheItem>>(File.ReadAllText(Path.Combine(Globals.AppDataBase, "cache", "index.json")));
            Debug.WriteLine($"[Cache] Loaded {_CacheItems.Count} cached images successfully!");
        } catch (Exception ex)
        {
            Debug.WriteLine("[Cache] Failed to load. Error: " + ex);
        }
    }

    private static void PrepareDir()
    {
        // Make sure that the directories and files exist that are needed

        if (!Directory.Exists(Path.Combine(Globals.AppDataBase, "cache"))) Directory.CreateDirectory(Path.Combine(Globals.AppDataBase, "cache"));
        if(!File.Exists(Path.Combine(Globals.AppDataBase, "cache", "index.json"))) Save();
        if (!Directory.Exists(Path.Combine(Globals.AppDataBase, "cache", "images"))) Directory.CreateDirectory(Path.Combine(Globals.AppDataBase, "cache", "images"));
    }

    public static CacheItem? SearchForCacheItem(string Year, string Month, string Day, string DataPoint)
    {
        for (var i = 0; i < _CacheItems.Count; i++)
        {
            if(_CacheItems[i].Year == Year && _CacheItems[i].Month == Month && _CacheItems[i].Day == Day && _CacheItems[i].DataPoint == DataPoint)
            {
                return _CacheItems[i];
            }
        }

        return null;
    }

    private static bool SearchForCacheItem(string location)
    {
        for (var i = 0; i < _CacheItems.Count; i++)
        {
            if (_CacheItems[i].Location == location) return true;
        }

        return false;
    }

    public static void CacheItem(string Year, string Month, string Day, string DataPoint, Bitmap bitmap)
    {
        if (SearchForCacheItem(Year, Month, Day, DataPoint) != null) return; // Don't re-save it, wasting space

        Debug.WriteLine("[Cache] Caching item...");

        Bitmap bm = new Bitmap(bitmap); // Just to make sure it's not locked so it doesn't throw a fit
        string id = Guid.NewGuid().ToString(); // Generate an id for it (For the file name, since we'll have a lot of these)
        string fullPath = Path.Combine(Globals.AppDataBase, "cache", "images", $"{id}.bmp"); // Generate the full path

        bm.Save(fullPath); // Save it

        // Generate index item
        CacheItem cacheItem = new CacheItem()
        {
            Location = fullPath,
            Year = Year,
            Month = Month,
            Day = Day,
            DataPoint = DataPoint
        };

        _CacheItems.Add(cacheItem); // Add it to the index
        Save(); // Re-save the index so that it knows

        Debug.WriteLine("[Cache] Cached item.");
    }
}
