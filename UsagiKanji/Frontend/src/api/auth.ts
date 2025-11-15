import axios from 'axios';
import type { SignUpApiRequest, LoginRequest, LoginResponse } from '../types/auth';

const API_BASE_URL = 'http://localhost:5261/api';

export const signUp = async (data: SignUpApiRequest) => {
    return axios.post(`${API_BASE_URL}/auth/signup`, data);
};

export const login = async (data: LoginRequest): Promise<LoginResponse> => {
    const response = await axios.post<LoginResponse>(`${API_BASE_URL}/auth/login`, data);
    return response.data;
};