const { generateMeetingToken, createZoomMeeting } = require('../src/tokenGenerator');
const { validateEnv } = require('../src/utils');

/**
 * Simple test suite for Zoom token helper.
 * Run with: npm test
 */

async function runTests() {
  console.log('🧪 Zoom Meeting Token Helper - Test Suite');
  console.log('==========================================\n');

  let passed = 0;
  let failed = 0;

  // Test 1: Environment validation
  console.log('Test 1: Environment variable validation');
  try {
    const envOk = validateEnv();
    if (typeof envOk === 'boolean') {
      console.log(`   ✅ Environment check returned ${envOk} (boolean)`);
      passed++;
    } else {
      throw new Error('validateEnv did not return boolean');
    }
  } catch (err) {
    console.error(`   ❌ Failed: ${err.message}`);
    failed++;
  }

  // Test 2: Token generation (requires valid .env)
  console.log('Test 2: Token generation (skipped if no credentials)');
  try {
    if (!process.env.ZOOM_CLIENT_ID || process.env.ZOOM_CLIENT_ID === 'your_client_id_here') {
      console.log('   ⏭️  Skipped: No Zoom credentials configured');
      passed++; // Not a failure, just skipped
    } else {
      const token = await generateMeetingToken();
      if (token && typeof token === 'string' && token.length > 20) {
        console.log('   ✅ Token generated successfully (length: ' + token.length + ')');
        passed++;
      } else {
        throw new Error('Token is invalid or too short');
      }
    }
  } catch (err) {
    console.error(`   ❌ Failed: ${err.message}`);
    failed++;
  }

  // Test 3: createZoomMeeting parameter validation
  console.log('Test 3: createZoomMeeting parameter validation');
  try {
    await createZoomMeeting('fake-token', {}, '');
    console.error('   ❌ Should have thrown error for missing email');
    failed++;
  } catch (err) {
    if (err.message.includes('Zoom user email is required')) {
      console.log('   ✅ Correctly rejected missing email');
      passed++;
    } else {
      console.error(`   ❌ Unexpected error: ${err.message}`);
      failed++;
    }
  }

  // Summary
  console.log(`\n📊 Results: ${passed} passed, ${failed} failed`);
  if (failed > 0) {
    process.exit(1);
  }
}

runTests();
