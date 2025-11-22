import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import styles from "./LoginPage.module.scss";
import { login as apiLogin } from "../../api/auth";
import type { LoginRequest, LoginResponse } from "../../types/auth";
import useWebsiteTitle from "../../hooks/useWebsiteTitle";

const LoginPage: React.FC = () => {
    const navigate = useNavigate();
    useWebsiteTitle("Log in - UsagiKanji");

    const [passwordVisible, setPasswordVisible] = useState(false);
    const [loginData, setLoginData] = useState<LoginRequest>({
        usernameOrEmail: "",
        password: "",
    });
    const [error, setError] = useState("");
    const [isLoading, setIsLoading] = useState(false);

    const togglePasswordVisibility = () => setPasswordVisible((v) => !v);

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setLoginData((prev) => ({ ...prev, [name]: value }));
        if (error) setError("");
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");
        setIsLoading(true);

        try {
            const loginResponse: LoginResponse = await apiLogin(loginData);

            const token = loginResponse.token;

            if (!token) {
                setError("Login failed: Server did not return a token");
                return;
            }

            await apiLogin(loginData);
            navigate("/main");
        } catch (err: any) {
            const serverError =
                err.response?.data?.detail ||
                err.response?.data?.message ||
                err.response?.data?.error ||
                err.message ||
                "Login failed. Please check your credentials and try again.";

            setError(serverError);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className={styles.mainContainer}>
            <div className={styles.formCard}>
                <h2 className={styles.title}>Log in</h2>

                {error && (
                    <div className={styles.errorToast} role="alert">
                        {error}
                    </div>
                )}

                <form onSubmit={handleSubmit} className={styles.form}>
                    <div className={styles.inputGroup}>
                        <input
                            type="text"
                            name="usernameOrEmail"
                            value={loginData.usernameOrEmail}
                            onChange={handleInputChange}
                            required
                            autoComplete="username"
                            className={styles.input}
                            placeholder="Username or Email"
                        />
                    </div>

                    <div className={styles.inputGroup}>
                        <input
                            type={passwordVisible ? "text" : "password"}
                            name="password"
                            value={loginData.password}
                            onChange={handleInputChange}
                            required
                            autoComplete="current-password"
                            className={styles.input}
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

                    <button
                        type="submit"
                        disabled={isLoading}
                        className={styles.submitBtn}
                    >
                        {isLoading ? <span className={styles.spinner}>Loading</span> : "Log in"}
                    </button>
                </form>
            </div>
        </div>
    );
};

export default LoginPage;