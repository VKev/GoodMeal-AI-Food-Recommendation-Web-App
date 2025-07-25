"use client"

import { FirebaseAuth } from "@/firebase/firebase";
import { onAuthStateChanged, User, signOut } from "firebase/auth";
import { useRouter } from "next/navigation";
import React, { useEffect, useState, useContext, ReactNode, createContext } from "react";
import { checkAuthorization } from "@/services/Auth";

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

interface TokenData {
    email: string;
    name: string;
    roles: string[];
    user_id: string;
    system_user_id: string;
    email_verified: boolean;
    iss?: string;
    aud?: string;
    auth_time?: number;
    iat?: number;
    exp?: number;
}

export enum UserRole {
    ADMIN = 'admin',
    BUSINESS = 'business',
    USER = 'user'
}

interface AuthContextType {
    currentUser: User | null;
    tokenData: TokenData | null;
    authUser: AuthUser | null;
    authenticated: boolean;
    loading: boolean;
    userRoles: string[];
    navigateByRole: () => void;
    hasRole: (role: string) => boolean;
    isAdmin: () => boolean;
    isBusiness: () => boolean;
    refreshToken: () => Promise<void>;
    logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
    const [currentUser, setCurrentUser] = useState<User | null>(null);
    const [tokenData, setTokenData] = useState<TokenData | null>(null);
    const [authUser, setAuthUser] = useState<AuthUser | null>(null);
    const [authenticated, setAuthenticated] = useState(false);
    const [loading, setLoading] = useState(true);
    const [userRoles, setUserRoles] = useState<string[]>([]);
    const router = useRouter();    const extractTokenData = async (user: User): Promise<TokenData | null> => {
        try {
            const idToken = await user.getIdToken();
            // console.log(idToken);

            // Call the check-authorization API with timeout
            let rolesFromBackend: string[] = [];
            try {
                const authResponse = await Promise.race([
                    checkAuthorization(idToken),
                    new Promise<null>((_, reject) => 
                        setTimeout(() => reject(new Error('Authorization check timeout')), 5000)
                    )
                ]);
                
                if (authResponse) {
                    setAuthUser(authResponse.user);
                    rolesFromBackend = authResponse.user.roles || [];
                } else {
                    console.log('Authorization check returned null');
                }
            } catch (authError) {
                console.error('Authorization check failed:', authError);
                // Continue without authorization data
            }

            const idTokenResult = await user.getIdTokenResult();

            console.log('idTokenResult:', idTokenResult);
            const claims = idTokenResult.claims;
            
            // Prioritize roles from backend, then from Firebase claims, then default
            let finalRoles: string[] = [];
            if (rolesFromBackend.length > 0) {
                finalRoles = rolesFromBackend;
            } else if (Array.isArray(claims.roles) && claims.roles.length > 0) {
                finalRoles = claims.roles;
            } else {
                finalRoles = ['user']; // Default role
                console.log('Using default role: user');
            }
            const tokenData: TokenData = {
                email: user.email || '',
                name: user.displayName || '',
                roles: finalRoles,
                user_id: user.uid,
                system_user_id: typeof claims.system_user_id === 'string' ? claims.system_user_id : '',
                email_verified: user.emailVerified,
                iss: typeof claims.iss === 'string' ? claims.iss : undefined,
                aud: typeof claims.aud === 'string' ? claims.aud : undefined,
                auth_time: typeof claims.auth_time === 'number' ? claims.auth_time : undefined,
                iat: typeof claims.iat === 'number' ? claims.iat : undefined,
                exp: typeof claims.exp === 'number' ? claims.exp : undefined,
            };

            console.log('Token data extracted:', tokenData);
            return tokenData;
        } catch (error) {
            console.error('Error extracting token data:', error);
            // Return minimal token data to allow navigation
            return {
                email: user.email || '',
                name: user.displayName || '',
                roles: ['user'], // Default role
                user_id: user.uid,
                system_user_id: '',
                email_verified: user.emailVerified,
            };
        }
    };

    const navigateByRole = () => {
      
        // Don't navigate if roles are empty - wait for them to load
        if (!userRoles.length) {
            return;
        }

        if (userRoles.some(role => role.toLowerCase() === UserRole.ADMIN.toLowerCase())) {
            router.push('/admin');
        } else if (userRoles.some(role => role.toLowerCase() === UserRole.BUSINESS.toLowerCase())) {
            router.push('/bussiness');
        } else if (userRoles.some(role => role.toLowerCase() === UserRole.USER.toLowerCase())) {
            router.push('/c');
        } else {
            router.push('/c');
        }
    };

    const hasRole = (role: string): boolean => {
        return userRoles.some(userRole => userRole.toLowerCase() === role.toLowerCase());
    };

    const isAdmin = (): boolean => {
        return hasRole(UserRole.ADMIN);
    };

    const isBusiness = (): boolean => {
        return hasRole(UserRole.BUSINESS);
    };

    const refreshToken = async (): Promise<void> => {
        if (!currentUser) return;

        try {
            const extractedData = await extractTokenData(currentUser);
            setTokenData(extractedData);

            if (extractedData?.roles) {
                setUserRoles(extractedData.roles);
            }
        } catch (error) {
            console.error('Error refreshing token:', error);
        }
    };

    useEffect(() => {
        const unsubscribe = onAuthStateChanged(FirebaseAuth, async (user) => {
            setCurrentUser(user);
            setAuthenticated(!!user);

            if (user) {
                const extractedData = await extractTokenData(user);
                setTokenData(extractedData);

                if (extractedData?.roles) {
                    setUserRoles(extractedData.roles);
                } else {
                    console.log('No roles found in token data');
                    setUserRoles([]);
                }
            } else {
                setTokenData(null);
                setAuthUser(null);
                setUserRoles([]);
            }

            setLoading(false);
        });
        return () => unsubscribe();
    }, []);

    useEffect(() => {
        
        // Only navigate when user is authenticated, not loading, and has roles
        // and when user is on login/signup pages or root page
        if (authenticated && !loading && userRoles.length > 0) {
            const currentPath = window.location.pathname;
            const shouldRedirect = currentPath === '/' || 
                                 currentPath === '/sign-in' || 
                                 currentPath === '/create-account';
            
            if (shouldRedirect) {
                // Add a small delay to ensure all state is updated
                setTimeout(() => {
                    navigateByRole();
                }, 100);
            }
        }
    }, [authenticated, loading, userRoles]); // Add userRoles to dependency array

    const logout = async () => {
        try {
            await signOut(FirebaseAuth);
            // Clear all state
            setCurrentUser(null);
            setTokenData(null);
            setAuthUser(null);
            setAuthenticated(false);
            setUserRoles([]);
            // Navigate to login page
            router.push('/sign-in');
        } catch (error) {
            console.error('Error during logout:', error);
        }
    };

    return (        <AuthContext.Provider value={{
            currentUser,
            tokenData,
            authUser,
            authenticated,
            loading,
            userRoles,
            navigateByRole,
            hasRole,
            isAdmin,
            isBusiness,
            refreshToken,
            logout
        }}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth(): AuthContextType {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error("useAuth must be used within an AuthProvider");
    }
    return context;
}