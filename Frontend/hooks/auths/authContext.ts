import { FirebaseAuth } from "@/firebase/firebase";
import { onAuthStateChanged } from "firebase/auth";
import React, { useEffect } from "react"
import { useState } from "react"

const AuthContext = React.createContext(AuthProvider);

export function AuthProvider({ children }) {
        const currentUser = useState(null);
        const authenticated = useState(false);
        const loading = useState(false);

        useEffect(() => {
                const unsubsribe = onAuthStateChanged(FirebaseAuth, initUser);
        }, [])

        async function initUser(user){

        }
}
