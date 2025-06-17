export interface CreateUserCommand {
  name: string;
  email: string;
}

export interface DeleteUserCommand {
  userId: string;
}

export interface EditUserNameCommand {
  userId: string;
  name: string;
}

export interface AddUserRoleCommand {
  userId: string;
  roleId: string;
}

export interface RemoveUserRoleCommand {
  userId: string;
  roleId: string;
}