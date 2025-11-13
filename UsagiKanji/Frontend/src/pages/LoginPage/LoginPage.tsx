import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './LoginPage.scss';
import { login } from '../../api/auth';
import type { LoginRequest, LoginResponse } from '../../types/auth';

const LoginPage: React.FC = () => {
    const navigate = useNavigate();
    const [form, setForm] = useState<LoginRequest>({
        usernameOrEmail: '',
        password: '',
    });
    const [error, setError] = useState<string>('');

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');

        try {
            const response = await login(form);
            localStorage.setItem('token', response.token);
            navigate('/main');
        } catch (err: any) {
            setError(err.response?.data?.errors?.[0]?.message || 'Login failed');
        }
    };

    return (
        <div className="login-page">
            <form className="login-form" onSubmit={handleSubmit}>
                <h2>Login</h2>
                {error && <div className="error">{error}</div>}
                <input
                    type="text"
                    name="usernameOrEmail"
                    placeholder="Username or Email"
                    value={form.usernameOrEmail}
                    onChange={handleChange}
                />
                <input
                    type="password"
                    name="password"
                    placeholder="Password"
                    value={form.password}
                    onChange={handleChange}
                />
                <button type="submit">Login</button>
            </form>
        </div>
    );
};

export default LoginPage;
