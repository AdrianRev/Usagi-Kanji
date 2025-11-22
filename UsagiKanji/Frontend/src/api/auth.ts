import api from "./axiosInstance";
import type { SignUpApiRequest, SignUpResponse, LoginRequest, LoginResponse } from '../types/auth';

export const signUp = async (data: SignUpApiRequest): Promise<SignUpResponse> => {
    const response = await api.post<SignUpResponse>("/auth/signup", data);

    if (response.data.token) {
        localStorage.setItem("access_token", response.data.token);
    }

    return response.data;
};

export const login = async (data: LoginRequest): Promise<LoginResponse> => {
    const response = await api.post<LoginResponse>("/auth/login", data);

    if (response.data.token) {
        localStorage.setItem("access_token", response.data.token);
    }

    return response.data;
};

export const logout = () => {
    localStorage.removeItem("access_token");
};

export const isLoggedIn = (): boolean => {
    return !!localStorage.getItem("access_token");
};