import { 
  getAuth, 
  GoogleAuthProvider, 
  signInWithPopup, 
  signInWithEmailAndPassword,
  createUserWithEmailAndPassword,
  signOut,
  sendPasswordResetEmail
} from "firebase/auth";
import firebaseApp from "./firebaseApp";

export const FirebaseAuth = getAuth(firebaseApp)

export const provider = new GoogleAuthProvider();

// Google Sign In
export const signInWithGoogle = () => signInWithPopup(FirebaseAuth, provider);

// Email/Password Sign In
export const signInWithEmail = (email: string, password: string) => 
  signInWithEmailAndPassword(FirebaseAuth, email, password);

// Email/Password Sign Up
export const signUpWithEmail = (email: string, password: string) => 
  createUserWithEmailAndPassword(FirebaseAuth, email, password);

// Sign Out
export const logOut = () => signOut(FirebaseAuth);

// Password Reset
export const resetPassword = (email: string) => 
  sendPasswordResetEmail(FirebaseAuth, email);