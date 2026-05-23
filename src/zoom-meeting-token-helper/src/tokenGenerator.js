const jwt = require('jsonwebtoken');
const axios = require('axios');

/**
 * Generate a Zoom Server-to-Server OAuth JWT token.
 * Uses client credentials flow with account-level authentication.
 * @returns {Promise<string>} Access token string
 */
async function generateMeetingToken() {
  const clientId = process.env.ZOOM_CLIENT_ID;
  const clientSecret = process.env.ZOOM_CLIENT_SECRET;
  const accountId = process.env.ZOOM_ACCOUNT_ID;

  if (!clientId || !clientSecret || !accountId) {
    throw new Error('Missing Zoom credentials. Check .env file for ZOOM_CLIENT_ID, ZOOM_CLIENT_SECRET, ZOOM_ACCOUNT_ID');
  }

  const tokenUrl = `https://zoom.us/oauth/token?grant_type=account_credentials&account_id=${accountId}`;
  const authHeader = Buffer.from(`${clientId}:${clientSecret}`).toString('base64');

  try {
    const response = await axios.post(tokenUrl, null, {
      headers: {
        'Authorization': `Basic ${authHeader}`,
        'Content-Type': 'application/x-www-form-urlencoded'
      }
    });

    return response.data.access_token;
  } catch (error) {
    const errMsg = error.response?.data?.message || error.message;
    throw new Error(`Failed to generate Zoom token: ${errMsg}`);
  }
}

/**
 * Create a Zoom meeting using the provided access token.
 * @param {string} token - Valid Zoom API access token
 * @param {object} meetingDetails - Meeting configuration object
 * @param {string} [userEmail] - Optional Zoom user email (defaults from env)
 * @returns {Promise<object>} Created meeting data
 */
async function createZoomMeeting(token, meetingDetails, userEmail) {
  const email = userEmail || process.env.ZOOM_USER_EMAIL || '';
  if (!email) {
    throw new Error('Zoom user email is required. Set ZOOM_USER_EMAIL in .env or pass as parameter.');
  }

  const url = `https://api.zoom.us/v2/users/${email}/meetings`;

  try {
    const response = await axios.post(url, meetingDetails, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });

    return response.data;
  } catch (error) {
    const errMsg = error.response?.data?.message || error.message;
    throw new Error(`Failed to create meeting: ${errMsg}`);
  }
}

module.exports = { generateMeetingToken, createZoomMeeting };
