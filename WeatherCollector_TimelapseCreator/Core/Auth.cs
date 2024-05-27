using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherCollector_TimelapseCreator.Core
{
    public class Auth
    {
        public string Token { get; set; } = "";
        public async static Task<Auth?> Authenticate(string username, string password)
        {
            // No real point in doing this other than it's upgradable easily later on
            Types.Auth auth = await Data.RequestAuth(password);
            if(string.IsNullOrEmpty(auth.Token))
            {
                return null;
            } else
            {
                return new Auth() { Token = auth.Token };
            }
        }
    }
}
