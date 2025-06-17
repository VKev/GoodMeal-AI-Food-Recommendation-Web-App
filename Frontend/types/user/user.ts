import { UserRoleBase } from "./userRole";

export interface User {
  userId: string;
  name: string;
  email: string;
  createdAt: Date | null;
  identityId: string | null;
  updateAt: Date | null;
  isDeleted: boolean | null;
  userRoles: UserRoleBase[];
}