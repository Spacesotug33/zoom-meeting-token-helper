using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace ZoomMeetingTokenHelper.Services
{
    /// <summary>
    /// Service for generating and validating Zoom meeting tokens.
    /// Implements Zoom's JWT token specification for meeting authentication.
    /// </summary>
    public class ZoomTokenService
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the ZoomTokenService.
        /// </summary>
        /// <param name="apiKey">Zoom API key from Zoom App Marketplace.</param>
        /// <param name="apiSecret">Zoom API secret corresponding to the API key.</param>
        public ZoomTokenService(string apiKey, string apiSecret)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _apiSecret = apiSecret ?? throw new ArgumentNullException(nameof(apiSecret));
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Generates a JWT token for a Zoom meeting.
        /// Token includes issuer, expiration, and meeting-specific claims.
        /// </summary>
        /// <param name="meetingNumber">The Zoom meeting number (numeric ID).</param>
        /// <param name="role">Optional role: 0 for attendee, 1 for host (default: 0).</param>
        /// <param name="expirySeconds">Token expiry in seconds from now (default: 3600).</param>
        /// <returns>Encoded JWT token string.</returns>
        public string GenerateMeetingToken(string meetingNumber, int role = 0, int expirySeconds = 3600)
        {
            if (string.IsNullOrWhiteSpace(meetingNumber))
                throw new ArgumentException("Meeting number cannot be empty.", nameof(meetingNumber));

            var now = DateTime.UtcNow;
            var claims = new List<Claim>
            {
                new Claim("iss", _apiKey),
                new Claim("iat", new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("exp", new DateTimeOffset(now.AddSeconds(expirySeconds)).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("app_meeting_number", meetingNumber),
                new Claim("role", role.ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_apiSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _apiKey,
                claims: claims,
                expires: now.AddSeconds(expirySeconds),
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenDescriptor);
        }

        /// <summary>
        /// Validates a Zoom JWT token by checking signature and expiration.
        /// </summary>
        /// <param name="token">The JWT token string to validate.</param>
        /// <returns>True if token is valid, false otherwise.</returns>
        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_apiSecret));

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = _apiKey,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal != null;
            }
            catch (SecurityTokenException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Tests the token against Zoom's OAuth endpoint to verify it works.
        /// </summary>
        /// <param name="token">The JWT token to test.</param>
        /// <returns>True if API responds successfully, false otherwise.</returns>
        public async Task<bool> TestTokenAgainstZoomApiAsync(string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.zoom.us/v2/users/me");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        /// <summary>
        /// Disposes the HttpClient when the service is no longer needed.
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
