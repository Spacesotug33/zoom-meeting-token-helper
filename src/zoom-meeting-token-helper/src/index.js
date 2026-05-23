const dotenv = require('dotenv');
dotenv.config();

const { generateMeetingToken, createZoomMeeting } = require('./tokenGenerator');

/**
 * Main entry point: demonstrates token generation and meeting creation.
 * Run with: node src/index.js
 */
async function main() {
  const token = await generateMeetingToken();
  console.log('✅ Zoom JWT token generated successfully:', token.substring(0, 50) + '...');

  // Example: create a meeting
  const meetingDetails = {
    topic: 'Weekly Sync - Token Helper Demo',
    type: 2, // Scheduled meeting
    start_time: new Date(Date.now() + 86400000).toISOString(), // tomorrow
    duration: 30, // minutes
    timezone: 'UTC',
    settings: {
      host_video: true,
      participant_video: true,
      join_before_host: false,
      mute_upon_entry: true
    }
  };

  try {
    const meeting = await createZoomMeeting(token, meetingDetails);
    console.log('✅ Meeting created successfully!');
    console.log('Join URL:', meeting.join_url);
    console.log('Meeting ID:', meeting.id);
    console.log('Start time:', meeting.start_time);
  } catch (err) {
    console.error('❌ Failed to create meeting:', err.message);
  }
}

main();
