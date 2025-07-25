interface Business {
    id: string;
    name: string;
    description: string;
    address: string;
    phone: string;
    email: string;
    website?: string;
    createReason: string;
    isActive: boolean;
    createdAt: string;
    updatedAt: string;
}

interface CreateBusinessRequest {
    name: string;
    description: string;
    address: string;
    phone: string;
    email: string;
    website?: string;
    createReason: string;
}

interface UpdateBusinessRequest {
    name: string;
    description: string;
    address: string;
    phone: string;
    email: string;
    website?: string;
}

interface Restaurant {
    restaurantId: string;
    id: string;
    name: string;
    address: string;
    phone: string;
    businessId: string;
    isActive: boolean;
    createdAt: string;
    updatedAt: string;
}

interface CreateRestaurantRequest {
    name: string;
    address: string;
    phone: string;
}

interface ApiResponse<T> {
    success: boolean;
    data: T;
    message: string;
}

class BusinessService {
    private baseUrl: string;

    constructor() {
        this.baseUrl = process.env.NEXT_PUBLIC_BACKEND_BASE_URL || 'http://localhost:2406/';
    }

    private async getAuthToken(): Promise<string> {
        // Import Firebase auth để lấy token
        const { FirebaseAuth } = await import('@/firebase/firebase');
        const currentUser = FirebaseAuth.currentUser;
        
        if (!currentUser) {
            throw new Error('No authenticated user found');
        }
        
        const token = await currentUser.getIdToken();
        return token;
    }

    private async makeRequest<T>(
        endpoint: string,
        options: RequestInit = {}
    ): Promise<T> {
        try {
            const token = await this.getAuthToken();
            const response = await fetch(`${this.baseUrl}${endpoint}`, {
                ...options,
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json',
                    ...options.headers,
                },
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();
            return data;
        } catch (error) {
            console.error(`Error calling ${endpoint}:`, error);
            throw error;
        }
    }

    // Business Management APIs
    async createBusiness(businessData: CreateBusinessRequest): Promise<Business> {
        return this.makeRequest<Business>('api/Business', {
            method: 'POST',
            body: JSON.stringify(businessData),
        });
    }

    async getMyBusiness(): Promise<Business | null> {
        const res = await this.makeRequest<any>('api/Business/my-business', { method: 'GET' });
        if (res && res.value) return res.value;
        return null;
    }

    async updateBusiness(businessId: string, businessData: UpdateBusinessRequest): Promise<Business> {
        return this.makeRequest<Business>(`api/Business/${businessId}`, {
            method: 'PUT',
            body: JSON.stringify(businessData),
        });
    }

    async activateBusiness(businessId: string): Promise<void> {
        return this.makeRequest<void>(`api/Business/${businessId}/activate`, {
            method: 'POST',
        });
    }

    async deactivateBusiness(businessId: string): Promise<void> {
        return this.makeRequest<void>(`api/Business/${businessId}/deactivate`, {
            method: 'POST',
        });
    }

    async deleteBusiness(businessId: string): Promise<void> {
        return this.makeRequest<void>(`api/Business/${businessId}`, {
            method: 'DELETE',
        });
    }

    // Restaurant Management APIs
    async getBusinessRestaurants(businessId: string): Promise<Restaurant[]> {
        const res = await this.makeRequest<any>(`api/Business/${businessId}/restaurants`, { method: 'GET' });
        // API trả về { value: { restaurants: [...] } }
        if (res && res.value && Array.isArray(res.value.restaurants)) {
            return res.value.restaurants
                .map((item: any) => item.restaurant ? { ...item.restaurant, id: item.id, restaurantId: item.restaurantId, isDisable: item.isDisable } : null)
                .filter(Boolean);
        }
        return [];
    }

    async createRestaurant(businessId: string, restaurantData: CreateRestaurantRequest): Promise<Restaurant> {
        return this.makeRequest<Restaurant>(`api/Business/${businessId}/restaurants`, {
            method: 'POST',
            body: JSON.stringify(restaurantData),
        });
    }
}

export const businessService = new BusinessService();

export type {
    Business,
    CreateBusinessRequest,
    UpdateBusinessRequest,
    Restaurant,
    CreateRestaurantRequest,
    ApiResponse
}; 