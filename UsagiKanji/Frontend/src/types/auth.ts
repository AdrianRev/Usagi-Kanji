
export interface SignUpRequest {
	username: string;
	email: string;
	password: string;
}

export interface LoginRequest {
	usernameOrEmail: string;
	password: string;
}

export interface LoginResponse {
	token: string;
}
