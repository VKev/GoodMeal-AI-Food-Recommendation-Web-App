import { NextResponse } from 'next/server';

export async function POST(req: Request) {
  try {
    const { Name, DurationSeconds } = await req.json();

    if (!Name || typeof Name !== 'string' || !DurationSeconds || typeof DurationSeconds !== 'number') {
      return NextResponse.json({ message: 'Name and DurationSeconds are required and must be valid types.' }, { status: 400 });
    }

    const backendResponse = await fetch(`${process.env.BACKEND_BASE_URL}/api/resource/generate-temporary-credentials`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ Name, DurationSeconds }),
    });

    if (!backendResponse.ok) {
      const errorResponse = await backendResponse.json();
      return NextResponse.json(errorResponse, { status: backendResponse.status });
    }

    const data = await backendResponse.json();

    return NextResponse.json({ value: data.value }, { status: 200 });
  } catch (error: any) {
    return NextResponse.json(
      { message: 'Internal Server Error', error: error.message },
      { status: 500 }
    );
  }
}
