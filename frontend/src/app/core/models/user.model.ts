export interface AppUserDto {
  id: string;
  username: string;
  email: string | null;
  roles: string[];
  lockedOut: boolean;
}

export interface CreateUserRequest {
  username: string;
  email: string | null;
  password: string;
  roles: string[];
}

export interface UpdateUserRequest {
  email: string | null;
  roles: string[];
}
