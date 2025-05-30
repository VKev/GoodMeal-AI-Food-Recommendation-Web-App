import { NextRequest, NextResponse } from 'next/server';

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();
    const { ResourceUrl, ExpiryHour } = body;

    if (!ResourceUrl || !ExpiryHour) {
      return NextResponse.json(
        { error: 'Missing ResourceUrl or ExpiryHour in the request body' },
        { status: 400 }
      );
    }
    // 1) Call your own backend for the signed cookies
    const backendResponse = await fetch(`${process.env.BACKEND_BASE_URL}/api/resource/generate-signed-cookie`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ ResourceUrl, ExpiryHour }),
    });

    if (!backendResponse.ok) {
      const error = await backendResponse.json();
      return NextResponse.json(
        { error: `Failed to fetch signed cookies: ${JSON.stringify(error)}` },
        { status: backendResponse.status }
      );
    }

    const result = await backendResponse.json();
    const { value: cookies, isSuccess } = result;

    if (!isSuccess || !cookies) {
      return NextResponse.json(
        { error: 'Failed to generate signed cookies' },
        { status: 500 }
      );
    }

    const responseHeaders = new Headers();
    return new NextResponse(
      JSON.stringify({
        message: 'Got CloudFront cookie data. Local marker set.',
        data: cookies, // e.g. { CloudFront-Policy, CloudFront-Signature, Key-Pair-Id, auth, ts, ... }
        expiryHour: ExpiryHour,
      }),
      {
        status: 200,
        headers: {
          ...Object.fromEntries(responseHeaders.entries()),
          'Content-Type': 'application/json',
        },
      }
    );
  } catch (error) {
    console.error('Error in set-cookie-marker route:', error);
    return NextResponse.json(
      { error: 'Internal server error' },
      { status: 500 }
    );
  }
}
