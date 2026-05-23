"""
Unit tests for ZoomTokenGenerator.
"""

import time
import jwt
import pytest
from src.token_generator import ZoomTokenGenerator


@pytest.fixture
def generator():
    """Fixture providing a test token generator instance."""
    return ZoomTokenGenerator(api_key="test_key", api_secret="test_secret")


class TestZoomTokenGenerator:
    """Test suite for ZoomTokenGenerator."""

    def test_generate_s2s_token_returns_string(self, generator):
        """Test that generate_s2s_token returns a string."""
        token = generator.generate_s2s_token()
        assert isinstance(token, str)
        assert len(token) > 0

    def test_generate_s2s_token_contains_valid_jwt(self, generator):
        """Test that the token is a valid JWT with correct payload."""
        token = generator.generate_s2s_token()
        payload = jwt.decode(token, "test_secret", algorithms=["HS256"])
        assert payload["iss"] == "test_key"
        assert "exp" in payload
        assert payload["exp"] > time.time()

    def test_generate_s2s_token_custom_expiration(self, generator):
        """Test custom expiration time."""
        token = generator.generate_s2s_token(expiration_seconds=60)
        payload = jwt.decode(token, "test_secret", algorithms=["HS256"])
        # Allow 1 second tolerance
        assert abs(payload["exp"] - (time.time() + 60)) < 2

    def test_generate_meeting_sdk_token_returns_string(self, generator):
        """Test that generate_meeting_sdk_token returns a string."""
        token = generator.generate_meeting_sdk_token("123456789")
        assert isinstance(token, str)

    def test_generate_meeting_sdk_token_payload(self, generator):
        """Test the payload of a meeting SDK token."""
        meeting_number = "987654321"
        token = generator.generate_meeting_sdk_token(meeting_number, role=1)
        payload = jwt.decode(token, "test_secret", algorithms=["HS256"])
        assert payload["appKey"] == "test_key"
        assert payload["token"] == meeting_number
        assert payload["roleType"] == 1

    def test_generate_meeting_sdk_token_default_role(self, generator):
        """Test default role is attendee."""
        token = generator.generate_meeting_sdk_token("111111111")
        payload = jwt.decode(token, "test_secret", algorithms=["HS256"])
        assert payload["roleType"] == 0

    def test_decode_token_returns_dict(self, generator):
        """Test decode_token returns the original payload."""
        original_token = generator.generate_s2s_token()
        decoded = generator.decode_token(original_token)
        assert isinstance(decoded, dict)
        assert decoded["iss"] == "test_key"

    def test_invalid_secret_raises_error(self, generator):
        """Test that decoding with wrong secret raises an error."""
        token = generator.generate_s2s_token()
        with pytest.raises(jwt.InvalidSignatureError):
            jwt.decode(token, "wrong_secret", algorithms=["HS256"])

    def test_expired_token_raises_error(self, generator):
        """Test that expired token raises an error."""
        # Generate token with 0 seconds expiration (already expired)
        token = generator.generate_s2s_token(expiration_seconds=0)
        # Sleep to ensure token expires
        time.sleep(0.1)
        with pytest.raises(jwt.ExpiredSignatureError):
            jwt.decode(token, "test_secret", algorithms=["HS256"], options={"verify_exp": True})
