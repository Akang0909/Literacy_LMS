using System;

namespace Literacy_LMS.Helpers
{
    public class ConfigHelper
    {
        public static string GetSendGridApiKey()
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("SendGrid API Key is missing. Set it as an environment variable.");
            }

            return apiKey;
        }
    }
}
