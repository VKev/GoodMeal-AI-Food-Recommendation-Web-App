interface RegisterRequest {
  email: string;
  password: string;
  name: string;
}

interface RegisterResponse {
  // Define the response structure based on your API
  success: boolean;
  message?: string;
  data?: any;
}

export const registerUser = async (userData: RegisterRequest): Promise<RegisterResponse> => {
  const apiUrl = process.env.NEXT_PUBLIC_BACKEND_BASE_URL;
  console.log('API URL from env:', apiUrl);
  
  if (!apiUrl) {
    console.error('NEXT_PUBLIC_BACKEND_BASE_URL is not defined in environment variables');
    throw new Error('API URL is not configured');
  }

  const fullUrl = `${apiUrl}api/Auth/register`;
  console.log('Full registration URL:', fullUrl);

  try {
    const response = await fetch(fullUrl, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(userData),
    });

    console.log('Response status:', response.status);
    console.log('Response headers:', response.headers);

    if (!response.ok) {
      const errorData = await response.text();
      console.error('Registration failed:', errorData);
      throw new Error(`Registration failed: ${response.status} ${response.statusText}`);
    }

    const data = await response.json();
    console.log('Registration response data:', data);
    
    return data;
  } catch (error) {
    console.error('Registration error:', error);
    throw error;
  }
};
