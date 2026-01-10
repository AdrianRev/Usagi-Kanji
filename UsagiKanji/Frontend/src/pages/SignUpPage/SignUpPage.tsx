import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import styles from "../LoginPage/LoginPage.module.scss";
import { signUp as apiSignup } from "../../api/auth";
import type { SignUpRequest, SignUpApiRequest } from "../../types/auth";
import useWebsiteTitle from "../../hooks/useWebsiteTitle";

const SignUpPage: React.FC = () => {
    const navigate = useNavigate();
    useWebsiteTitle("Sign up - UsagiKanji");
    const [passwordVisible, setPasswordVisible] = useState(false);

    const [signUpData, setSignUpData] = useState<SignUpRequest>({
        username: "",
        email: "",
        password: "",
        confirmPassword: "",
    });
    const [error, setError] = useState("");
    const [isLoading, setIsLoading] = useState(false);

    const togglePasswordVisibility = () => setPasswordVisible((v) => !v);

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setSignUpData((prev) => ({ ...prev, [name]: value }));
        if (error) setError("");
    };

    const validateUsername = (name: string): string | null => {
        if (!name) return "Username is required";
        if (name.length < 3) return "Username must be at least 3 characters long";
        if (name.length > 20) return "Username cannot be longer than 20 characters long";
        if (!/^[a-zA-Z0-9_-]+$/.test(name))
            return "Username can only contain letters, numbers, _ and -";
        return null;
    };

    const validatePassword = (pwd: string): string | null => {
        const minLength = 8;
        const maxLength = 50;
        const hasUpper = /[A-Z]/.test(pwd);
        const hasLower = /[a-z]/.test(pwd);
        const hasDigit = /\d/.test(pwd);
        if (pwd.length > maxLength) return `Password cannot be longer than ${maxLength} characters long`;
        if (pwd.length < minLength) return `Password must be at least ${minLength} characters long`;
        if (!hasUpper) return "Password must contain at least one uppercase letter";
        if (!hasLower) return "Password must contain at least one lowercase letter";
        if (!hasDigit) return "Password must contain at least one digit";
        return null;
    };
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");
        setIsLoading(true);

        const userError = validateUsername(signUpData.username);
        if (userError) {
            setError(userError);
            setIsLoading(false);
            return;
        }

        if (signUpData.password !== signUpData.confirmPassword) {
            setError("Passwords do not match");
            setIsLoading(false);
            return;
        }

        const pwdError = validatePassword(signUpData.password);
        if (pwdError) {
            setError(pwdError);
            setIsLoading(false);
            return;
        }

        const payload: SignUpApiRequest = {
            username: signUpData.username,
            email: signUpData.email,
            password: signUpData.password,
        };

        try {
            const res = await apiSignup(payload);

            const token = res.token;

            if (!token) {
                alert("Sign-up successful! You can now log in.");
                navigate("/login");
                return;
            }
            await apiSignup(signUpData);

            navigate("/main");
        } catch (err: any) {
            const serverMsg =
                err.response?.data?.detail ||
                err.response?.data?.message ||
                err.response?.data?.error ||
                "Sign-up failed. Please try again.";

            setError(serverMsg);
        } finally {
            setIsLoading(false);
        }


    };

    return (
        <div className={styles.mainContainer}>
            <div className={styles.formCard}>
                <h2 className={styles.title}>Create account</h2>

                {error && (
                    <div className={styles.errorToast} role="alert">
                        {error}
                    </div>
                )}

                <form onSubmit={handleSubmit} className={styles.form}>
                    <div className={styles.inputGroup}>
                        <input
                            type="text"
                            name="username"
                            value={signUpData.username}
                            onChange={handleInputChange}
                            required
                            autoComplete="username"
                            className={styles.input}
                            id="username"
                            placeholder="Username"
                        />
                    </div>

                    <div className={styles.inputGroup}>
                        <input
                            type="email"
                            name="email"
                            value={signUpData.email}
                            onChange={handleInputChange}
                            required
                            autoComplete="email"
                            className={styles.input}
                            id="email"
                            placeholder="Email"
                        />
                    </div>

                    <div className={styles.inputGroup}>
                        <input
                            type={passwordVisible ? "text" : "password"}
                            name="password"
                            value={signUpData.password}
                            onChange={handleInputChange}
                            required
                            autoComplete="new-password"
                            className={styles.input}
                            id="password"
                            placeholder="Password"
                        />
                        <button
                            type="button"
                            onClick={togglePasswordVisibility}
                            className={styles.toggleBtn}
                            aria-label={passwordVisible ? "Hide password" : "Show password"}
                        >
                            {passwordVisible ? "🙈" : "👁️"}
                        </button>
                    </div>

                    <div className={styles.inputGroup}>
                        <input
                            type={passwordVisible ? "text" : "password"}
                            name="confirmPassword"
                            value={signUpData.confirmPassword}
                            onChange={handleInputChange}
                            required
                            autoComplete="new-password"
                            className={styles.input}
                            id="confirmPassword"
                            placeholder="Repeat password"
                        />
                    </div>

                    <button
                        type="submit"
                        disabled={isLoading}
                        className={styles.submitBtn}
                        aria-busy={isLoading}
                    >
                        {isLoading ? <span className={styles.spinner}>Signing up</span> : "Sign up"}
                    </button>
                </form>
            </div>
        </div>
    );
};

export default SignUpPage;