import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth, UserRole } from './authContext';

interface UseAuthGuardOptions {
    requiredRoles?: string[];
    redirectTo?: string;
    allowUnauthenticated?: boolean;
}

export function useAuthGuard(options: UseAuthGuardOptions = {}) {
    const {
        requiredRoles = [],
        redirectTo = '/sign-in',
        allowUnauthenticated = false
    } = options;

    const { authenticated, loading, userRoles, hasRole } = useAuth();
    const router = useRouter();

    useEffect(() => {
        if (loading) return;

        if (!authenticated && !allowUnauthenticated) {
            router.push(redirectTo);
            return;
        }

        if (requiredRoles.length > 0 && authenticated) {
            const hasRequiredRole = requiredRoles.some(role => hasRole(role));

            if (!hasRequiredRole) {
                if (hasRole(UserRole.ADMIN)) {
                    router.push('/admin');
                } else if (hasRole(UserRole.BUSINESS)) {
                    router.push('/bussiness');
                } else {
                    router.push('/');
                }
            }
        }
    }, [authenticated, loading, userRoles, requiredRoles, redirectTo, allowUnauthenticated, hasRole, router]);

    return {
        authenticated,
        loading,
        userRoles,
        hasRequiredAccess: requiredRoles.length === 0 || requiredRoles.some(role => hasRole(role))
    };
}

// Specific hooks cho từng role
export function useAdminGuard(redirectTo?: string) {
    return useAuthGuard({
        requiredRoles: [UserRole.ADMIN],
        redirectTo
    });
}

export function useBusinessGuard(redirectTo?: string) {
    return useAuthGuard({
        requiredRoles: [UserRole.BUSINESS, UserRole.ADMIN], // Admin có thể access business routes
        redirectTo
    });
}

export function useUserGuard(redirectTo?: string) {
    return useAuthGuard({
        requiredRoles: [UserRole.USER, UserRole.BUSINESS, UserRole.ADMIN], // Tất cả roles có thể access user routes
        redirectTo
    });
} 