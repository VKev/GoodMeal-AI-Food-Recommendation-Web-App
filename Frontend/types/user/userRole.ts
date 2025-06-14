import { Role } from "../role/role";
import { User } from "./user";

export interface UserRole {
    userId: string;
    roleId: string;
    assignedAt: Date | null;
    role: Role;
    user: User;
}

export interface UserRoleBase {
    userId: string;
    roleId: string;
    assignedAt: Date | null;
}