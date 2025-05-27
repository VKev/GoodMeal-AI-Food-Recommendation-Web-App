const crypto = require('crypto');
const AWS = require('aws-sdk');
let cachedSecret = null;

async function getSecret() {
  if (cachedSecret) {
    return cachedSecret;
  }
  // Create an SSM client in us-east-1 (Lambda@Edge functions run in us-east-1).
  const ssm = new AWS.SSM({ region: 'us-east-1' });
  const result = await ssm
    .getParameter({
      Name: '/lambda/edge/secret', // Change to your parameter name in SSM.
      WithDecryption: true,
    })
    .promise();
  cachedSecret = result.Parameter.Value;
  return cachedSecret;
}
exports.handler = async (event, context, callback) => {
  const secret = await getSecret();
  const request = event.Records[0].cf.request;
  // If user hits /set-cookie, handle it at the edge:
  if (request.uri.startsWith('/set-cookie')) {
    // 1) Parse querystring
    const { policy, signature, keypair, auth, ts } = getQueryParams(request.querystring);
    if (!policy || !signature || !keypair || !auth || !ts) {
      return callback(null, forbidden("Missing query parameters"));
    }

    // 2) Validate auth signature 
    // Recompute the signature. We'll define the string to sign in a consistent way:
    // e.g. policy + signature + keypair + ts + secret or an HMAC of them.
    const dataToSign = `${policy}:${signature}:${keypair}:${ts}`;
    const computedAuth = hmacSHA256(dataToSign, secret);

    if (computedAuth !== auth) {
      return callback(null, forbidden("Invalid auth signature"));
    }

    // (Optional) Check if ts (timestamp) is within a valid time window, e.g. 1 minute
    const nowSeconds = Math.floor(Date.now() / 1000);
    const requestTs = parseInt(ts, 10);
    if (Math.abs(nowSeconds - requestTs) > 60) {
      return callback(null, forbidden("Timestamp too old or in the future"));
    }

    // 3) Build your Set-Cookie headers for CloudFront
    const cookies = [
      `CloudFront-Policy=${policy}; Domain=.${request.headers.host[0].value}; Path=/; HttpOnly; Secure; SameSite=None`,
      `CloudFront-Signature=${signature}; Domain=.${request.headers.host[0].value}; Path=/; HttpOnly; Secure; SameSite=None`,
      `CloudFront-Key-Pair-Id=${keypair}; Domain=.${request.headers.host[0].value}; Path=/; HttpOnly; Secure; SameSite=None`,
    ];

    // 4) Return a 200 response with those cookies
    const response = {
      status: '200',
      statusDescription: 'OK',
      headers: {
        'content-type': [{ key: 'Content-Type', value: 'application/json' }],
        'set-cookie': cookies.map((val) => ({ key: 'Set-Cookie', value: val })),
      },
      body: JSON.stringify({ message: 'Signed cookies set successfully!' }),
    };
    return callback(null, response);
  }

  // For all other URIs, just pass request to origin (Wasabi)
  return callback(null, request);
};

function getQueryParams(querystring) {
  if (!querystring) return {};
  return querystring
    .split('&')
    .map((part) => part.split('='))
    .reduce((acc, [k, v]) => {
      acc[k] = decodeURIComponent(v || '');
      return acc;
    }, {});
}

// Helper to compute HMAC-SHA256 hex
function hmacSHA256(data, secret) {
  return crypto.createHmac('sha256', secret).update(data).digest('hex');
}

function forbidden(reason) {
  return {
    status: '403',
    statusDescription: 'Forbidden',
    headers: {
      'content-type': [{ key: 'Content-Type', value: 'application/json' }],
    },
    body: JSON.stringify({ error: reason }),
  };
}
