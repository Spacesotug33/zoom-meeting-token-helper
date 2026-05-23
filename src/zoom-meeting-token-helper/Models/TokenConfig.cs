using System;

namespace ZoomMeetingTokenHelper.Models
{
    /// <summary>
    /// Configuration model for token generation parameters.
    /// Can be loaded from JSON or environment variables.
    /// </summary>
    public class TokenConfig
    {
        /// <summary>
        /// Zoom API key from Zoom Marketplace.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Zoom API secret corresponding to the API key.
        /// </summary>
        public string ApiSecret { get; set; } = string.Empty;

        /// <summary>
        /// The Zoom meeting number (numeric string).
        /// </summary>
        public string MeetingNumber { get; set; } = string.Empty;

        /// <summary>
        /// Token expiration time in seconds (default 3600 = 1 hour).
        /// </summary>
        public int ExpirySeconds { get; set; } = 3600;

        /// <summary>
        /// Role for the token: 0 = attendee, 1 = host.
        /// </summary>
        public int Role { get; set; } = 0;

        /// <summary>
        /// Validates the configuration has required fields.
        /// </summary>
        /// <returns>True if valid, false otherwise.</returns>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ApiKey)
                && !string.IsNullOrWhiteSpace(ApiSecret)
                && !string.IsNullOrWhiteSpace(MeetingNumber)
                && ExpirySeconds > 0
                && (Role == 0 || Role == 1);
        }

        /// <summary>
        /// Creates a TokenConfig from environment variables.
        /// </summary>
        /// <returns>A TokenConfig instance with values from environment.</returns>
        public static TokenConfig FromEnvironment()
        {
            return new TokenConfig
            {
                ApiKey = Environment.GetEnvironmentVariable("ZOOM_API_KEY") ?? string.Empty,
                ApiSecret = Environment.GetEnvironmentVariable("ZOOM_API_SECRET") ?? string.Empty,
                MeetingNumber = Environment.GetEnvironmentVariable("ZOOM_MEETING_NUMBER") ?? string.Empty,
                ExpirySeconds = int.TryParse(Environment.GetEnvironmentVariable("ZOOM_TOKEN_EXPIRY"), out int expiry) ? expiry : 3600,
                Role = int.TryParse(Environment.GetEnvironmentVariable("ZOOM_TOKEN_ROLE"), out int role) ? role : 0
            };
        }
    }
}
