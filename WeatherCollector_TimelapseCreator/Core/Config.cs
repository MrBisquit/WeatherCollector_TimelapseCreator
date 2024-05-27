using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security;
using System.Security.AccessControl;

namespace WeatherCollector_TimelapseCreator.Core
{
    public class Config
    {
        [JsonRequired()]
        [JsonProperty("auth")]
        public string AuthToken { get; set; } = null;

        [JsonRequired()]
        [JsonProperty("username")]
        public string Username { get; set; } = null;

        [JsonRequired()]
        [JsonProperty("password")]
        public string Password { get; set; } = null;

        [JsonRequired()]
        [JsonProperty("server")]
        public string ServerLocation { get; set; } = null;

        public Config()
        {
            // Init values
            AuthToken = "";
            Username = "";
            Password = "";
            ServerLocation = "";
        }

        public void Prepare()
        {
            Debug.WriteLine("Preparing configuration...");
            Debug.Indent();
            if (!Directory.Exists(Globals.AppDataBase)) Directory.CreateDirectory(Globals.AppDataBase);
            if (!File.Exists(Path.Combine(Globals.AppDataBase, "config.cfg"))) File.WriteAllText(Path.Combine(Globals.AppDataBase, "config.cfg"), "{}");
            Debug.WriteLine("Config file exists: " + File.Exists(Path.Combine(Globals.AppDataBase, "config.cfg")));
            Debug.WriteLine("Config file location: " + Path.Combine(Globals.AppDataBase, "config.cfg"));
            Debug.WriteLine("AppDataBase directory exists: " + Directory.Exists(Globals.AppDataBase));
            AuthorizationRuleCollection accessRule = new DirectoryInfo(Globals.AppDataBase).GetAccessControl().GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
            bool CanAccessADB = false;
            foreach (FileSystemAccessRule rule in accessRule)
            {
                if ((rule.FileSystemRights & FileSystemRights.Read) == FileSystemRights.Read && (rule.FileSystemRights & FileSystemRights.Write) == FileSystemRights.Write)
                {
                    if (rule.AccessControlType == AccessControlType.Allow)
                    {
                        CanAccessADB = true;
                    }
                }
            }
            Debug.WriteLine("Has permissions to access AppDataBase: " + CanAccessADB);
            AuthorizationRuleCollection accessRulef = new FileInfo(Path.Combine(Globals.AppDataBase, "config.cfg")).GetAccessControl().GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
            bool CanAccessADBC = false;
            foreach (FileSystemAccessRule rule in accessRule)
            {
                if ((rule.FileSystemRights & FileSystemRights.Read) == FileSystemRights.Read && (rule.FileSystemRights & FileSystemRights.Write) == FileSystemRights.Write)
                {
                    if (rule.AccessControlType == AccessControlType.Allow)
                    {
                        CanAccessADBC = true;
                    }
                }
            }
            Debug.WriteLine("Has permissions to access config.cfg: " + CanAccessADBC);
            Debug.IndentSize = 0;

            //Directory.CreateDirectory(Globals.AppDataBase);
            //File.WriteAllText(Path.Combine(Globals.AppDataBase, "config.cfg"), "{}");
            Debug.WriteLine(File.ReadAllText(Path.Combine(Globals.AppDataBase, "config.cfg")));
        }

        public void SaveConfig(string location = default)
        {
            if(location == default)
            {
                location = Path.Combine(Globals.AppDataBase, "config.cfg");
            }

            File.WriteAllText(location, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public void LoadConfig(string location = default)
        {
            if (location == default)
            {
                location = Path.Combine(Globals.AppDataBase, "config.cfg");
            }

            if(!File.Exists(location))
            {
                throw new Exception("Config file does not exist.");
            }

            try
            {
                LoadAs(JsonConvert.DeserializeObject<Config>(File.ReadAllText(location)));
            }
            catch {
                throw new Exception("Config file invalid.");
            }
        }

        public void LoadAs(Config cfg)
        {
            this.AuthToken = cfg.AuthToken;
            this.Username = cfg.Username;
            this.Password = cfg.Password;
            this.ServerLocation = cfg.ServerLocation;
        }
    }
}
