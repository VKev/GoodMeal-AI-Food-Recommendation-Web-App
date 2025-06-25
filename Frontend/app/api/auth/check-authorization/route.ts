import { NextResponse } from 'next/server';

export async function GET(req: Request) {
  try {
    const authHeader = req.headers.get('authorization');
    
    if (!authHeader) {
      return NextResponse.json({ message: 'Authorization header is required' }, { status: 401 });
    }

    const backendResponse = await fetch(`${process.env.BACKEND_BASE_URL}/api/Auth/check-authorization`, {
      method: 'GET',
      headers: {
        'Authorization': authHeader,
        'Content-Type': 'application/json',
      },
    });

    if (!backendResponse.ok) {
      const errorResponse = await backendResponse.json().catch(() => ({ message: 'Backend error' }));
      return NextResponse.json(errorResponse, { status: backendResponse.status });
    }

    const data = await backendResponse.json();
    return NextResponse.json(data, { status: 200 });
  } catch (error: any) {
    console.error('Check authorization error:', error);
    return NextResponse.json({ message: 'Internal Server Error', error: error.message }, { status: 500 });
  }
}
