import { NextResponse } from 'next/server';

export async function POST(req: Request) {
  try {
    const { bucketName, objectKey, expiryDurationMinutes, httpVerb } = await req.json();

    if (!bucketName || !objectKey || !httpVerb) {
      return NextResponse.json({ message: 'Missing required parameters.' }, { status: 400 });
    }

    const backendResponse = await fetch(`${process.env.BACKEND_BASE_URL}/api/resource/generate-presigned-url`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ bucketName, objectKey, expiryDurationMinutes, httpVerb }),
    });

    if (!backendResponse.ok) {
      const errorResponse = await backendResponse.json();
      return NextResponse.json(errorResponse, { status: backendResponse.status });
    }

    const data = await backendResponse.json();
    return NextResponse.json({ presignedUrl: data.value }, { status: 200 });
  } catch (error: any) {
    return NextResponse.json({ message: 'Internal Server Error', error: error.message }, { status: 500 });
  }
}
