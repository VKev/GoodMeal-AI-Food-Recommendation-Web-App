import { getAuth } from "firebase/auth";
import firebaseApp from "./firebaseApp";

export const FirebaseAuth = getAuth(firebaseApp)