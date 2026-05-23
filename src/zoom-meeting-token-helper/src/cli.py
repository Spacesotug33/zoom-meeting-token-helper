"""
Command-line interface for Zoom meeting token generation.

Allows generating tokens and creating meetings from the terminal.
"""

import argparse
import sys
from .token_generator import ZoomTokenGenerator
from .zoom_api import ZoomAPIClient


def main():
    """Main CLI entry point."""
    parser = argparse.ArgumentParser(
        description="Zoom Meeting Token Helper - Generate tokens and manage meetings"
    )
    parser.add_argument(
        "--api-key",
        required=True,
        help="Zoom API Key or SDK Key"
    )
    parser.add_argument(
        "--api-secret",
        required=True,
        help="Zoom API Secret or SDK Secret"
    )

    subparsers = parser.add_subparsers(dest="command", help="Available commands")

    # Token generation command
    token_parser = subparsers.add_parser("generate-token", help="Generate a JWT token")
    token_parser.add_argument(
        "--type",
        choices=["s2s", "meeting-sdk"],
        default="s2s",
        help="Type of token to generate (default: s2s)"
    )
    token_parser.add_argument(
        "--meeting-number",
        help="Meeting number (required for meeting-sdk token)"
    )
    token_parser.add_argument(
        "--role",
        type=int,
        choices=[0, 1],
        default=0,
        help="Role: 0=attendee, 1=host (default: 0)"
    )

    # Create meeting command
    meeting_parser = subparsers.add_parser("create-meeting", help="Create a Zoom meeting")
    meeting_parser.add_argument(
        "--user-id",
        required=True,
        help="Zoom user ID or email"
    )
    meeting_parser.add_argument(
        "--topic",
        required=True,
        help="Meeting topic"
    )
    meeting_parser.add_argument(
        "--duration",
        type=int,
        default=30,
        help="Meeting duration in minutes (default: 30)"
    )

    args = parser.parse_args()

    if args.command == "generate-token":
        generator = ZoomTokenGenerator(args.api_key, args.api_secret)
        if args.type == "s2s":
            token = generator.generate_s2s_token()
        else:
            if not args.meeting_number:
                print("Error: --meeting-number is required for meeting-sdk token", file=sys.stderr)
                sys.exit(1)
            token = generator.generate_meeting_sdk_token(
                meeting_number=args.meeting_number,
                role=args.role
            )
        print(token)

    elif args.command == "create-meeting":
        client = ZoomAPIClient(args.api_key, args.api_secret)
        try:
            meeting = client.create_meeting(
                user_id=args.user_id,
                topic=args.topic,
                duration=args.duration
            )
            print(f"Meeting created: {meeting['join_url']}")
            print(f"Meeting ID: {meeting['id']}")
        except Exception as e:
            print(f"Error creating meeting: {e}", file=sys.stderr)
            sys.exit(1)

    else:
        parser.print_help()


if __name__ == "__main__":
    main()
