using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Literacy_LMS.Helpers
{
    public class EmailService
    {
        public static async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var apiKey = ConfigHelper.GetSendGridApiKey();
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("m.simbrano.534440@umindanao.edu.ph", "Library System");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);

            var response = await client.SendEmailAsync(msg);
            Console.WriteLine($"Email sent with status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                string responseBody = await response.Body.ReadAsStringAsync();
                Console.WriteLine($"SendGrid Error: {responseBody}");
            }
        }

    }
}
