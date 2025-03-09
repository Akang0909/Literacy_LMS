using System;

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

            // Retrieve the SendGrid API key from the appsettings.json file
            var apiKey = config["SendGrid:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("SendGrid API Key is missing. Please ensure it is set in appsettings.json.");
            }

            return apiKey;
        }

    }
}
