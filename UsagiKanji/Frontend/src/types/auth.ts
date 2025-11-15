export interface SignUpRequest {
	username: string;
	email: string;
	password: string;
	confirmPassword: string;
}
export type SignUpApiRequest = Omit<SignUpRequest, 'confirmPassword'>;

export interface LoginRequest {
	usernameOrEmail: string;
	password: string;
}

export interface LoginResponse {
	token: string;
}
