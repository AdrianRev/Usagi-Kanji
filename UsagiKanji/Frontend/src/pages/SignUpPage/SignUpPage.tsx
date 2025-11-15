import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import styles from "../LoginPage/LoginPage.module.scss";
import { signUp as apiSignup } from "../../api/auth";
import type { SignUpRequest, SignUpApiRequest } from "../../types/auth";

const SignUpPage: React.FC = () => {
    const navigate = useNavigate();

    const [passwordVisible, setPasswordVisible] = useState(false);

    const [formData, setFormData] = useState<SignUpRequest>({
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
        setFormData((prev) => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");
        setIsLoading(true);

        if (formData.password !== formData.confirmPassword) {
            setError("Passwords are not the same");
            setIsLoading(false);
            return;
        }

        const payload: SignUpApiRequest = {
            username: formData.username,
            email: formData.email,
            password: formData.password,
        };

        try {
            const res = await apiSignup(payload);   // ← returns LoginResponse
            localStorage.setItem("token", res.token); // ← res.token (not res.data.token)
            navigate("/main");
        } catch (err: any) {
            setError(err.response?.data?.message || "Sign‑up failed");
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
                            value={formData.username}
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
                            value={formData.email}
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
                            value={formData.password}
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
                            aria-label={passwordVisible ? "Show password" : "Hide password"}
                        >
                            {passwordVisible ? "🙈" : "👁️"}
                        </button>
                    </div>

                    <div className={styles.inputGroup}>
                        <input
                            type={passwordVisible ? "text" : "password"}
                            name="confirmPassword"
                            value={formData.confirmPassword}
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
                        {isLoading ? <span className={styles.spinner}>Przetwarzanie</span> : "Sign up"}
                    </button>
                </form>
            </div>
        </div>
    );
};

export default SignUpPage;