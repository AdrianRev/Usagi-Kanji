import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './SignUpPage.scss';
import { signUp } from '../../api/auth';
import type { SignUpRequest } from '../../types/auth';

const SignUpPage: React.FC = () => {
    const navigate = useNavigate();
    const [form, setForm] = useState<SignUpRequest>({
        username: '',
        email: '',
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
            await signUp(form);
            alert('Sign up successful! Please log in.');
            navigate('/login');
        } catch (err: any) {
            setError(err.response?.data?.errors?.[0]?.message || 'Sign up failed');
        }
    };

    return (
        <div className="signup-page">
            <form className="signup-form" onSubmit={handleSubmit}>
                <h2>Sign Up</h2>
                {error && <div className="error">{error}</div>}
                <input
                    type="text"
                    name="username"
                    placeholder="Username"
                    value={form.username}
                    onChange={handleChange}
                />
                <input
                    type="email"
                    name="email"
                    placeholder="Email"
                    value={form.email}
                    onChange={handleChange}
                />
                <input
                    type="password"
                    name="password"
                    placeholder="Password"
                    value={form.password}
                    onChange={handleChange}
                />
                <button type="submit">Sign Up</button>
            </form>
        </div>
    );
};

export default SignUpPage;
