using Microsoft.Extensions.Configuration;
using System.IO;

namespace Literacy_LMS.Helpers
{
    public class ConfigHelper
    {
        public static string GetSendGridApiKey()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            return config["SendGrid:ApiKey"];
        }
    }
}
