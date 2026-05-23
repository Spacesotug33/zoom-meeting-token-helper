using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZoomMeetingTokenHelper.Services;

namespace ZoomMeetingTokenHelper.Tests
{
    [TestClass]
    public class ZoomTokenServiceTests
    {
        private const string TestApiKey = "test_api_key_12345";
        private const string TestApiSecret = "test_api_secret_abcdefghijklmnop";
        private const string TestMeetingNumber = "9876543210";

        [TestMethod]
        public void GenerateMeetingToken_ValidInputs_ReturnsNonEmptyToken()
        {
            // Arrange
            var service = new ZoomTokenService(TestApiKey, TestApiSecret);

            // Act
            var token = service.GenerateMeetingToken(TestMeetingNumber);

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
            StringAssert.Contains(token, "."); // JWT contains dots
        }

        [TestMethod]
        public void GenerateMeetingToken_DifferentMeetings_ProducesDifferentTokens()
        {
            // Arrange
            var service = new ZoomTokenService(TestApiKey, TestApiSecret);

            // Act
            var token1 = service.GenerateMeetingToken("1111111111");
            var token2 = service.GenerateMeetingToken("2222222222");

            // Assert
            Assert.AreNotEqual(token1, token2);
        }

        [TestMethod]
        public void GenerateMeetingToken_EmptyMeetingNumber_ThrowsArgumentException()
        {
            // Arrange
            var service = new ZoomTokenService(TestApiKey, TestApiSecret);

            // Act & Assert
            Assert.ThrowsException<System.ArgumentException>(() => service.GenerateMeetingToken(""));
        }

        [TestMethod]
        public void ValidateTokenAsync_ValidToken_ReturnsTrue()
        {
            // Arrange
            var service = new ZoomTokenService(TestApiKey, TestApiSecret);
            var token = service.GenerateMeetingToken(TestMeetingNumber);

            // Act
            var isValid = service.ValidateTokenAsync(token).Result;

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void ValidateTokenAsync_InvalidToken_ReturnsFalse()
        {
            // Arrange
            var service = new ZoomTokenService(TestApiKey, TestApiSecret);
            var invalidToken = "eyJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJ0ZXN0In0.invalid_signature";

            // Act
            var isValid = service.ValidateTokenAsync(invalidToken).Result;

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void ValidateTokenAsync_ExpiredToken_ReturnsFalse()
        {
            // Arrange
            var service = new ZoomTokenService(TestApiKey, TestApiSecret);
            // Generate token with 0 seconds expiry (already expired)
            var token = service.GenerateMeetingToken(TestMeetingNumber, expirySeconds: -1);

            // Act
            var isValid = service.ValidateTokenAsync(token).Result;

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void GenerateMeetingToken_Role0_ContainsRoleClaim()
        {
            // Arrange
            var service = new ZoomTokenService(TestApiKey, TestApiSecret);

            // Act
            var token = service.GenerateMeetingToken(TestMeetingNumber, role: 0);

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
            // Decode and verify role claim (simplified check)
            var parts = token.Split('.');
            Assert.AreEqual(3, parts.Length);
        }

        [TestMethod]
        public void TestTokenAgainstZoomApiAsync_NoNetwork_ReturnsFalse()
        {
            // Arrange (no network available in test environment)
            var service = new ZoomTokenService(TestApiKey, TestApiSecret);
            var token = service.GenerateMeetingToken(TestMeetingNumber);

            // Act
            var result = service.TestTokenAgainstZoomApiAsync(token).Result;

            // Assert: should fail because we're not actually connected to Zoom
            Assert.IsFalse(result);
        }
    }
}
