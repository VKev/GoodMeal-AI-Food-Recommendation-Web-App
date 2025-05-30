'use client';
import { useEffect, useState } from 'react';

function getCookie(name: string): string | undefined {
  if (typeof document === 'undefined') return undefined;
  const match = document.cookie.match(new RegExp(`(^|; )${name}=([^;]+)`));
  return match ? decodeURIComponent(match[2]) : undefined;
}

export function useSignedCookies(resourceUrl: string, defaultExpiryHour: number) {
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const existingExpiryHour = getCookie('expiryHour');

    if (!existingExpiryHour) {
      console.log('expiryHour cookie not found. Fetching fresh cookies...');
      fetch('/api/resource/get-signed-cookie', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ ResourceUrl: resourceUrl, ExpiryHour: defaultExpiryHour }),
      })
        .then(async (res) => {
          if (!res.ok) {
            const err = await res.json();
            throw new Error(err.error || 'Failed to retrieve CloudFront cookie data');
          }
          return res.json();
        })
        .then(async (respData) => {
          console.log('Received CF cookies:', respData.data);

          const returnedExpiryHour = respData.expiryHour ?? defaultExpiryHour;
          const maxAgeSeconds = returnedExpiryHour * 3600;
          document.cookie = `expiryHour=${returnedExpiryHour}; Path=/; Max-Age=${maxAgeSeconds}; Secure; SameSite=Lax`;
          
          await callCloudFrontSetCookie(respData.data);
        })
        .catch((err) => {
          console.error(err);
          setError(err.message);
        })
        .finally(() => setIsLoading(false));
    } else {
      setIsLoading(false);
    }
  }, [resourceUrl, defaultExpiryHour]);

  return { isLoading, error };
}


async function callCloudFrontSetCookie(cookiesData: Record<string, string>) {
  const policy    = encodeURIComponent(cookiesData['CloudFront-Policy'] || '');
  const signature = encodeURIComponent(cookiesData['CloudFront-Signature'] || '');
  const keypair   = encodeURIComponent(cookiesData['CloudFront-Key-Pair-Id'] || '');
  const auth      = encodeURIComponent(cookiesData['auth'] || '');
  const ts        = encodeURIComponent(cookiesData['ts'] || '');

  const domain = process.env.NEXT_PUBLIC_CLOUDFRONT_BASE_URL;
  const setCookieUrl = `${domain}/set-cookie?policy=${policy}&signature=${signature}&keypair=${keypair}&auth=${auth}&ts=${ts}`;

  console.log('Calling CloudFront /set-cookie URL:', setCookieUrl);
  const res = await fetch(setCookieUrl, {
    method: 'GET',
    credentials: 'include', 
  });

  if (!res.ok) {
    const text = await res.text();
    throw new Error(`CloudFront /set-cookie failed: ${text}`);
  }

  console.log('CloudFront cookies set successfully!');
}
