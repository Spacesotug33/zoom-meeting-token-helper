/**
 * Utility functions for Zoom token helper.
 */

/**
 * Validate environment variables are set.
 * @returns {boolean} True if all required env vars are present
 */
function validateEnv() {
  const required = ['ZOOM_CLIENT_ID', 'ZOOM_CLIENT_SECRET', 'ZOOM_ACCOUNT_ID'];
  const missing = required.filter(key => !process.env[key]);
  
  if (missing.length > 0) {
    console.warn(`⚠️  Missing environment variables: ${missing.join(', ')}`);
    console.warn('   Copy .env.example to .env and fill in your Zoom credentials.');
    return false;
  }
  return true;
}

/**
 * Format a date for Zoom API (ISO 8601).
 * @param {Date|string} date - Date object or ISO string
 * @returns {string} Formatted date string
 */
function formatMeetingDate(date) {
  const d = new Date(date);
  return d.toISOString().replace(/\..+/, 'Z');
}

/**
 * Simple retry wrapper for async functions.
 * @param {Function} fn - Async function to retry
 * @param {number} [retries=3] - Number of retry attempts
 * @param {number} [delay=1000] - Delay between retries in ms
 * @returns {Promise<any>} Result of the function
 */
async function withRetry(fn, retries = 3, delay = 1000) {
  let lastError;
  for (let attempt = 1; attempt <= retries; attempt++) {
    try {
      return await fn();
    } catch (error) {
      lastError = error;
      if (attempt < retries) {
        console.warn(`Retry ${attempt}/${retries} after error: ${error.message}`);
        await new Promise(resolve => setTimeout(resolve, delay));
      }
    }
  }
  throw lastError;
}

module.exports = { validateEnv, formatMeetingDate, withRetry };
