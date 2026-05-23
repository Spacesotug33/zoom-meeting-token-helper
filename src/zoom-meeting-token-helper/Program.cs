using System;
using System.Threading.Tasks;
using ZoomMeetingTokenHelper.Services;

namespace ZoomMeetingTokenHelper
{
    /// <summary>
    /// Entry point for the Zoom meeting token generator application.
    /// Generates JWT tokens for Zoom meetings using API credentials.
    /// </summary>
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Zoom Meeting Token Helper");
            Console.WriteLine("=========================\n");

            // Load configuration from environment variables or appsettings
            var apiKey = Environment.GetEnvironmentVariable("ZOOM_API_KEY") ?? "your_api_key_here";
            var apiSecret = Environment.GetEnvironmentVariable("ZOOM_API_SECRET") ?? "your_api_secret_here";
            var meetingNumber = args.Length > 0 ? args[0] : "1234567890";

            if (apiKey == "your_api_key_here" || apiSecret == "your_api_secret_here")
            {
                Console.WriteLine("Warning: Using default API credentials. Set ZOOM_API_KEY and ZOOM_API_SECRET environment variables.");
            }

            try
            {
                var tokenService = new ZoomTokenService(apiKey, apiSecret);
                var token = tokenService.GenerateMeetingToken(meetingNumber);
                Console.WriteLine($"Meeting Token for {meetingNumber}:\n{token}");

                // Optional: verify token validity
                var isValid = await tokenService.ValidateTokenAsync(token);
                Console.WriteLine($"\nToken Valid: {isValid}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error generating token: {ex.Message}");
                Environment.ExitCode = 1;
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
