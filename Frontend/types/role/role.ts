import { UserRoleBase } from "../user/userRole";

export interface Role {
  roleId: string;
  roleName: string;
  createdAt: Date | null;
  userRoles: UserRoleBase[];
}