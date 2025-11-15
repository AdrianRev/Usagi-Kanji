import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import styles from "./LoginPage.module.scss";
import { login as apiLogin } from "../../api/auth";
import type { LoginRequest } from "../../types/auth";
import useWebsiteTitle from "../../hooks/useWebsiteTitle";

const LoginPage: React.FC = () => {
    const navigate = useNavigate();
    useWebsiteTitle("Sign in page");
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
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");
        setIsLoading(true);

        try {
            const res = await apiLogin(loginData);
            localStorage.setItem("token", res.token);
            navigate("/main");
        } catch (err: any) {
            setError(err.response?.data?.message || "Login failed");
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className={styles.mainContainer}>
            <div className={styles.formCard}>
                <h2 className={styles.title}>Sign In</h2>

                {/* Error toast */}
                {error && (
                    <div className={styles.errorToast} role="alert">
                        {error}
                    </div>
                )}

                <form onSubmit={handleSubmit} className={styles.form}>
                    {/* Username / Email */}
                    <div className={styles.inputGroup}>
                        <input
                            type="text"
                            name="usernameOrEmail"
                            value={loginData.usernameOrEmail}
                            onChange={handleInputChange}
                            required
                            autoComplete="username"
                            className={styles.input}
                            id="usernameOrEmail"
                            placeholder="Username or Email"
                        />
                    </div>

                    {/* Password */}
                    <div className={styles.inputGroup}>
                        <input
                            type={passwordVisible ? "text" : "password"}
                            name="password"
                            value={loginData.password}
                            onChange={handleInputChange}
                            required
                            autoComplete="current-password"
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

                    {/* Submit */}
                    <button
                        type="submit"
                        disabled={isLoading}
                        className={styles.submitBtn}
                        aria-busy={isLoading}
                    >
                        {isLoading ? <span className={styles.spinner}>⟳</span> : "Sign In"}
                    </button>
                </form>
            </div>
        </div>
    );
};

export default LoginPage;
