"use client"

import { FirebaseAuth } from "@/firebase/firebase";
import { onAuthStateChanged, User } from "firebase/auth";
import { useRouter } from "next/navigation";
import React, { useEffect, useState, useContext, ReactNode, createContext } from "react";

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
    authenticated: boolean;
    loading: boolean;
    userRoles: string[];
    navigateByRole: () => void;
    hasRole: (role: string) => boolean;
    isAdmin: () => boolean;
    isBusiness: () => boolean;
    refreshToken: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
    const [currentUser, setCurrentUser] = useState<User | null>(null);
    const [tokenData, setTokenData] = useState<TokenData | null>(null);
    const [authenticated, setAuthenticated] = useState(false);
    const [loading, setLoading] = useState(true);
    const [userRoles, setUserRoles] = useState<string[]>([]);
    const router = useRouter();

    const extractTokenData = async (user: User): Promise<TokenData | null> => {
        try {
            const idToken = await user.getIdToken();
            console.log('id token:', idToken);

            const idTokenResult = await user.getIdTokenResult();

            const claims = idTokenResult.claims;
            const tokenData: TokenData = {
                email: user.email || '',
                name: user.displayName || '',
                roles: Array.isArray(claims.roles) ? claims.roles : [],
                user_id: user.uid,
                system_user_id: typeof claims.system_user_id === 'string' ? claims.system_user_id : '',
                email_verified: user.emailVerified,
                iss: typeof claims.iss === 'string' ? claims.iss : undefined,
                aud: typeof claims.aud === 'string' ? claims.aud : undefined,
                auth_time: typeof claims.auth_time === 'number' ? claims.auth_time : undefined,
                iat: typeof claims.iat === 'number' ? claims.iat : undefined,
                exp: typeof claims.exp === 'number' ? claims.exp : undefined,
            };

            return tokenData;
        } catch (error) {
            console.error('Error extracting token data:', error);
            return null;
        }
    };

    const navigateByRole = () => {
        if (!userRoles.length) return;

        if (userRoles.includes(UserRole.ADMIN)) {
            router.push('/admin');
        } else if (userRoles.includes(UserRole.BUSINESS)) {
            router.push('/bussiness');
        } else if (userRoles.includes(UserRole.USER)) {
            router.push('/');
        } else {

            router.push('/');
        }
    };

    const hasRole = (role: string): boolean => {
        return userRoles.includes(role);
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
                }
            } else {
                setTokenData(null);
                setUserRoles([]);
            }

            setLoading(false);
        });
        return () => unsubscribe();
    }, []);

    useEffect(() => {
        if (authenticated && userRoles.length > 0 && !loading) {

        }
    }, [userRoles, authenticated, loading]);

    return (
        <AuthContext.Provider value={{
            currentUser,
            tokenData,
            authenticated,
            loading,
            userRoles,
            navigateByRole,
            hasRole,
            isAdmin,
            isBusiness,
            refreshToken
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