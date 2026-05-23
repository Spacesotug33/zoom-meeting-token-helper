"""
Zoom Meeting Token Generator

Generates JWT tokens for Zoom Server-to-Server OAuth apps
or Meeting SDK apps. Supports both meeting and webinar tokens.
"""

import time
import jwt
from typing import Optional

class ZoomTokenGenerator:
    """
    Generate and validate Zoom meeting tokens.

    For Server-to-Server OAuth apps, use generate_s2s_token().
    For Meeting SDK apps, use generate_meeting_sdk_token().
    """

    def __init__(self, api_key: str, api_secret: str):
        """
        Initialize the token generator.

        Args:
            api_key: Zoom API Key or SDK Key.
            api_secret: Zoom API Secret or SDK Secret.
        """
        self.api_key = api_key
        self.api_secret = api_secret

    def generate_s2s_token(
        self,
        expiration_seconds: int = 3600,
        issuer: Optional[str] = None
    ) -> str:
        """
        Generate a Server-to-Server OAuth JWT token.

        Args:
            expiration_seconds: Token lifetime in seconds (default 1 hour).
            issuer: Optional custom issuer claim (defaults to api_key).

        Returns:
            Encoded JWT token string.
        """
        payload = {
            "iss": issuer or self.api_key,
            "exp": int(time.time()) + expiration_seconds,
        }
        return jwt.encode(payload, self.api_secret, algorithm="HS256")

    def generate_meeting_sdk_token(
        self,
        meeting_number: str,
        role: int = 0,
        expiration_seconds: int = 7200
    ) -> str:
        """
        Generate a Meeting SDK JWT token for a specific meeting.

        Args:
            meeting_number: Zoom meeting ID (numeric string).
            role: 0 for attendee, 1 for host.
            expiration_seconds: Token lifetime in seconds (default 2 hours).

        Returns:
            Encoded JWT token string.
        """
        payload = {
            "appKey": self.api_key,
            "iat": int(time.time()),
            "exp": int(time.time()) + expiration_seconds,
            "token": meeting_number,
            "roleType": role
        }
        return jwt.encode(payload, self.api_secret, algorithm="HS256")

    def decode_token(self, token: str) -> dict:
        """
        Decode and verify a JWT token (without validation for debugging).

        Args:
            token: JWT token string.

        Returns:
            Decoded payload dictionary.
        """
        return jwt.decode(token, self.api_secret, algorithms=["HS256"])
