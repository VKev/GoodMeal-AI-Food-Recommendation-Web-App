import { FirebaseAuth } from "@/firebase/firebase";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:2406";

interface ApiResponse<T> {
    value: T;
    isSuccess: boolean;
    isFailure: boolean;
    error: {
        code: string;
        description: string;
    };
}

// User Management Types
interface UserRole {
    identityId: string;
    email: string;
    name: string;
    roles: string[];
}

interface UserStatus {
    identityId: string;
    email: string;
    name: string;
    isDisabled: boolean;
    emailVerified: boolean;
    lastSignInTime: string;
    creationTime: string;
}

// New types based on actual API response
interface ApiUser {
    uid: string;
    email: string;
    displayName: string;
    isDisabled: boolean;
    isEmailVerified: boolean;
    lastSignInTime: string | null;
    creationTime: string;
    roles: string[];
}

interface UserSearchResponse {
    users: ApiUser[];
    nextPageToken: string | null;
    totalCount: number;
    searchTerm: string;
}

interface AddRoleRequest {
    uid: string;         // Changed to uid to match frontend usage
    roleName: string;
}

interface RemoveRoleRequest {
    uid: string;         // Changed to uid to match frontend usage
    roleName: string;
}

interface UpdateUserRequest {
    identityId: string;
    email: string;
    displayName: string;
    emailVerified: boolean;
}

// Business Management Types
interface Business {
    id: string;
    ownerId: string;
    name: string;
    description: string;
    address: string;
    phone: string;
    email: string;
    website: string;
    isActive: boolean;
    createdAt: string;
    updatedAt: string;
}

interface BusinessResponse {
    businesses: Business[];
    totalCount: number;
}

interface CreateBusinessRequest {
    name: string;
    description: string;
    address: string;
    phone: string;
    email: string;
    website: string;
}

interface UpdateBusinessRequest {
    name: string;
    description: string;
    address: string;
    phone: string;
    email: string;
    website: string;
}

class AdminService {
    private async getAuthHeaders(): Promise<HeadersInit> {
        const user = FirebaseAuth.currentUser;
        if (!user) {
            throw new Error('User not authenticated');
        }
        
        const token = await user.getIdToken();
        return {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        };
    }

    // User Management APIs
    async getUserRoles(identityId: string): Promise<UserRole> {
        const headers = await this.getAuthHeaders();
        const response = await fetch(`${API_BASE_URL}/api/Admin/roles/${identityId}`, {
            method: 'GET',
            headers
        });

        if (!response.ok) {
            throw new Error(`Failed to get user roles: ${response.statusText}`);
        }

        const data: ApiResponse<UserRole> = await response.json();
        if (!data.isSuccess) {
            throw new Error(data.error.description || 'Failed to get user roles');
        }

        return data.value;
    }

    async addRole(request: AddRoleRequest): Promise<void> {
        console.log('Add Role Request:', request);
        
        if (!request.uid || !request.roleName) {
            throw new Error('UID and Role Name are required');
        }
        
        const headers = await this.getAuthHeaders();
        
        // Backend still expects identityId, so map uid to identityId
        const backendRequest = {
            identityId: request.uid,
            roleName: request.roleName
        };
        
        const response = await fetch(`${API_BASE_URL}/api/Admin/add-role`, {
            method: 'POST',
            headers,
            body: JSON.stringify(backendRequest)
        });

        console.log('Add Role Response Status:', response.status);
        
        if (!response.ok) {
            const errorText = await response.text();
            console.error('Add Role Error:', errorText);
            throw new Error(`Failed to add role: ${response.statusText} - ${errorText}`);
        }
    }

    async removeRole(request: RemoveRoleRequest): Promise<void> {
        const headers = await this.getAuthHeaders();
        
        // Backend still expects identityId, so map uid to identityId
        const backendRequest = {
            identityId: request.uid,
            roleName: request.roleName
        };
        
        const response = await fetch(`${API_BASE_URL}/api/Admin/remove-role`, {
            method: 'POST',
            headers,
            body: JSON.stringify(backendRequest)
        });

        if (!response.ok) {
            throw new Error(`Failed to remove role: ${response.statusText}`);
        }
    }

    async disableUser(uid: string): Promise<void> {
        console.log('Disable User UID:', uid);
        
        if (!uid) {
            throw new Error('UID is required');
        }
        
        const headers = await this.getAuthHeaders();
        const requestBody = { identityId: uid };  // Backend expects identityId
        
        console.log('Disable User Request Body:', requestBody);
        
        const response = await fetch(`${API_BASE_URL}/api/Admin/disable`, {
            method: 'POST',
            headers,
            body: JSON.stringify(requestBody)
        });

        console.log('Disable User Response Status:', response.status);
        
        if (!response.ok) {
            const errorText = await response.text();
            console.error('Disable User Error:', errorText);
            throw new Error(`Failed to disable user: ${response.statusText} - ${errorText}`);
        }
    }

    async enableUser(uid: string): Promise<void> {
        console.log('Enable User UID:', uid);
        
        if (!uid) {
            throw new Error('UID is required');
        }
        
        const headers = await this.getAuthHeaders();
        const requestBody = { identityId: uid };  // Backend expects identityId
        
        console.log('Enable User Request Body:', requestBody);
        
        const response = await fetch(`${API_BASE_URL}/api/Admin/enable`, {
            method: 'POST',
            headers,
            body: JSON.stringify(requestBody)
        });

        console.log('Enable User Response Status:', response.status);
        
        if (!response.ok) {
            const errorText = await response.text();
            console.error('Enable User Error:', errorText);
            throw new Error(`Failed to enable user: ${response.statusText} - ${errorText}`);
        }
    }

    async getUserStatus(identityId: string): Promise<UserStatus> {
        const headers = await this.getAuthHeaders();
        const response = await fetch(`${API_BASE_URL}/api/Admin/status/${identityId}`, {
            method: 'GET',
            headers
        });

        if (!response.ok) {
            throw new Error(`Failed to get user status: ${response.statusText}`);
        }

        const data: ApiResponse<UserStatus> = await response.json();
        if (!data.isSuccess) {
            throw new Error(data.error.description || 'Failed to get user status');
        }

        return data.value;
    }

    async updateUser(request: UpdateUserRequest): Promise<void> {
        const headers = await this.getAuthHeaders();
        const response = await fetch(`${API_BASE_URL}/api/Admin/users`, {
            method: 'PUT',
            headers,
            body: JSON.stringify(request)
        });

        if (!response.ok) {
            throw new Error(`Failed to update user: ${response.statusText}`);
        }
    }

    async deleteUser(uid: string): Promise<void> {
        const headers = await this.getAuthHeaders();
        const response = await fetch(`${API_BASE_URL}/api/Admin/users/${uid}`, {
            method: 'DELETE',
            headers
        });

        if (!response.ok) {
            throw new Error(`Failed to delete user: ${response.statusText}`);
        }
    }

    async getAllUsers(): Promise<UserSearchResponse> {
        const headers = await this.getAuthHeaders();
        const response = await fetch(`${API_BASE_URL}/api/Admin/users/search`, {
            method: 'GET',
            headers
        });

        if (!response.ok) {
            throw new Error(`Failed to get users: ${response.statusText}`);
        }

        const data: ApiResponse<UserSearchResponse> = await response.json();
        if (!data.isSuccess) {
            throw new Error(data.error.description || 'Failed to get users');
        }

        return data.value;
    }

    // Business Management APIs
    async getAllBusinesses(): Promise<BusinessResponse> {
        const headers = await this.getAuthHeaders();
        const response = await fetch(`${API_BASE_URL}/api/Business`, {
            method: 'GET',
            headers
        });

        if (!response.ok) {
            throw new Error(`Failed to get businesses: ${response.statusText}`);
        }

        const data: ApiResponse<BusinessResponse> = await response.json();
        if (!data.isSuccess) {
            throw new Error(data.error.description || 'Failed to get businesses');
        }

        return data.value;
    }

    async createBusiness(request: CreateBusinessRequest): Promise<void> {
        const headers = await this.getAuthHeaders();
        const response = await fetch(`${API_BASE_URL}/api/Business`, {
            method: 'POST',
            headers,
            body: JSON.stringify(request)
        });

        if (!response.ok) {
            throw new Error(`Failed to create business: ${response.statusText}`);
        }
    }

    async updateBusiness(businessId: string, request: UpdateBusinessRequest): Promise<void> {
        const headers = await this.getAuthHeaders();
        const response = await fetch(`${API_BASE_URL}/api/Business/${businessId}`, {
            method: 'PUT',
            headers,
            body: JSON.stringify(request)
        });

        if (!response.ok) {
            throw new Error(`Failed to update business: ${response.statusText}`);
        }
    }

    async deleteBusiness(businessId: string): Promise<void> {
        const headers = await this.getAuthHeaders();
        const response = await fetch(`${API_BASE_URL}/api/Business/${businessId}`, {
            method: 'DELETE',
            headers
        });

        if (!response.ok) {
            throw new Error(`Failed to delete business: ${response.statusText}`);
        }
    }
}

export const adminService = new AdminService();
export type { 
    UserRole, 
    UserStatus, 
    UserSearchResponse,
    ApiUser,
    AddRoleRequest, 
    RemoveRoleRequest, 
    UpdateUserRequest,
    Business,
    BusinessResponse,
    CreateBusinessRequest,
    UpdateBusinessRequest
};
