interface AuthUser {
    name: string | null;
    authenticationType: string;
    userId: string;
    email: string;
    roles: string[];
    allClaims: Array<{
        type: string;
        value: string;
    }>;
}

interface AuthCheckResponse {
    message: string;
    user: AuthUser;
}

export const checkAuthorization = async (idToken: string): Promise<AuthCheckResponse | null> => {
    try {
        const baseUrl = process.env.NEXT_PUBLIC_BACKEND_BASE_URL || 'http://localhost:2406/';
        const response = await fetch(`${baseUrl}api/auth/check-authorization`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${idToken}`,
                'Content-Type': 'application/json',
            },
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }        const data: AuthCheckResponse = await response.json();
        console.log('Auth response data:', data);

        return data;
    } catch (error) {
        console.error('Error checking authorization:', error);
        return null;
    }
};
