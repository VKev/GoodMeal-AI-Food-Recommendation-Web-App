export interface GetUserRolesQuery {
    identityId: string;
}

export interface GetUserResponse {
    name: string;
    email: string;
}

export interface GetUserRolesResponse {
    userId: string;
    email: string;
    name: string;
    identityId: string;
    roles: string[];
}

