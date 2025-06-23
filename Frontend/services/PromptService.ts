interface PromptSession {
    id: string;
    userId: string;
    createdAt: string;
    updatedAt: string | null;
    deletedAt: string | null;
    isDeleted: boolean;
}

interface PromptSessionResponse {
    value: {
        results: Array<{
            name: string;
            isSuccess: boolean;
            data: PromptSession[];
        }>;
    };
    isSuccess: boolean;
    isFailure: boolean;
    error: {
        code: string;
        description: string;
    };
}

export const getPromptSessions = async (idToken: string, userId: string): Promise<PromptSession[]> => {
    try {
        if (!idToken) {
            console.warn('No ID token provided');
            return [];
        }

        const baseUrl = process.env.NEXT_PUBLIC_BACKEND_BASE_URL || 'http://localhost:2406/';
        const response = await fetch(`${baseUrl}api/prompt/PromptSession/read`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${idToken}`,
                'Content-Type': 'application/json',
            },
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data: PromptSessionResponse = await response.json();
        console.log('Prompt sessions response:', data);

        if (data.isSuccess && data.value.results.length > 0) {
            // Lọc sessions theo userId và chỉ lấy những session chưa bị xóa
            const allSessions = data.value.results[0].data || [];
            return allSessions.filter(session => 
                session.userId === userId && !session.isDeleted
            );
        }

        return [];
    } catch (error) {
        console.error('Error fetching prompt sessions:', error);
        return [];
    }
};

// Helper function để format thời gian
export const formatTime = (dateString: string): string => {
    const date = new Date(dateString);
    const now = new Date();
    const diffInMs = now.getTime() - date.getTime();
    const diffInDays = Math.floor(diffInMs / (1000 * 60 * 60 * 24));

    if (diffInDays === 0) {
        return 'Today';
    } else if (diffInDays === 1) {
        return 'Yesterday';
    } else if (diffInDays < 7) {
        return `${diffInDays} days ago`;
    } else if (diffInDays < 14) {
        return '1 week ago';
    } else if (diffInDays < 21) {
        return '2 weeks ago';
    } else {
        return date.toLocaleDateString();
    }
};

interface CreateSessionResponse {
    value: {
        results: Array<{
            name: string;
            isSuccess: boolean;
            data: PromptSession;
        }>;
    };
    isSuccess: boolean;
    isFailure: boolean;
    error: {
        code: string;
        description: string;
    };
}

export const createPromptSession = async (idToken: string, userId: string): Promise<PromptSession | null> => {
    try {
        if (!idToken) {
            console.warn('No ID token provided');
            return null;
        }

        if (!userId) {
            console.warn('No User ID provided');
            return null;
        }        console.log('Creating session with userId:', userId);
        
        const baseUrl = process.env.NEXT_PUBLIC_BACKEND_BASE_URL || 'http://localhost:2406/';
        
        // API only needs userId - it will generate a new session ID automatically
        const requestBody = {
            userId: userId
        };
        
        console.log('Request body:', requestBody);
          const response = await fetch(`${baseUrl}api/prompt/PromptSession/create`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${idToken}`,
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(requestBody)
        });

        console.log('Response status:', response.status);
        console.log('Response headers:', response.headers);

        if (!response.ok) {
            // Try to get error details from response
            let errorText = '';
            try {
                errorText = await response.text();
                console.log('Error response body:', errorText);
            } catch (e) {
                console.log('Could not read error response body');
            }
            throw new Error(`HTTP error! status: ${response.status}, body: ${errorText}`);
        }

        const data: CreateSessionResponse = await response.json();
        console.log('Create session response:', data);

        if (data.isSuccess && data.value.results.length > 0) {
            return data.value.results[0].data;
        }

        return null;
    } catch (error) {
        console.error('Error creating prompt session:', error);
        return null;
    }
};
