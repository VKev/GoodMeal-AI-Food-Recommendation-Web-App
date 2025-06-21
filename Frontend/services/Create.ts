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
    });    console.log('Response status:', response.status);
    console.log('Response headers:', response.headers);

    if (!response.ok) {
      const errorData = await response.text();
      console.error('Registration failed - Status:', response.status);
      console.error('Registration failed - Error Data:', errorData);
      console.error('Registration failed - Status Text:', response.statusText);
      
      // Try to parse error as JSON if possible
      let errorMessage = `Registration failed: ${response.status} ${response.statusText}`;
      try {
        const errorJson = JSON.parse(errorData);
        errorMessage = errorJson.message || errorJson.error || errorMessage;
        console.error('Parsed error JSON:', errorJson);
      } catch (parseError) {
        console.error('Could not parse error as JSON:', parseError);
      }
      
      throw new Error(errorMessage);
    }

    const data = await response.json();
    console.log('Registration response data:', data);
    
    return data;
  } catch (error) {
    console.error('Registration error:', error);
    throw error;
  }
};
