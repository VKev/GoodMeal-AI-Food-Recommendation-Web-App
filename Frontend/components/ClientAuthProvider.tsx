"use client"

import { AuthProvider } from "@/hooks/auths/authContext";
import { ReactNode } from "react";

interface ClientAuthProviderProps {
    children: ReactNode;
}

export default function ClientAuthProvider({ children }: ClientAuthProviderProps) {
    return (
        <AuthProvider>
            {children}
        </AuthProvider>
    );
}
