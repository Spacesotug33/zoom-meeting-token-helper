# Zoom Meeting Token Helper

A Python utility for generating Zoom meeting tokens and managing meetings via the Zoom API.

## Features

- Generate Server-to-Server OAuth JWT tokens
- Generate Meeting SDK tokens for specific meetings
- Create, retrieve, and delete Zoom meetings via REST API
- Command-line interface for quick token generation

## Installation

```bash
pip install -r requirements.txt
```

## Usage

### Python API

```python
from src.token_generator import ZoomTokenGenerator
from src.zoom_api import ZoomAPIClient

# Generate a Server-to-Server token
generator = ZoomTokenGenerator("your_api_key", "your_api_secret")
token = generator.generate_s2s_token()
print(token)

# Create a meeting
client = ZoomAPIClient("your_api_key", "your_api_secret")
meeting = client.create_meeting("user@example.com", "My Meeting")
print(meeting["join_url"])
```

### Command Line

```bash
# Generate a Server-to-Server token
python -m src.cli --api-key YOUR_KEY --api-secret YOUR_SECRET generate-token --type s2s

# Generate a Meeting SDK token
python -m src.cli --api-key YOUR_KEY --api-secret YOUR_SECRET generate-token --type meeting-sdk --meeting-number 123456789

# Create a meeting
python -m src.cli --api-key YOUR_KEY --api-secret YOUR_SECRET create-meeting --user-id user@example.com --topic "My Meeting"
```

## Testing

```bash
pytest tests/
```

## License

MIT
