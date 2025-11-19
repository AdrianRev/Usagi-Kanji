import { useState, useEffect} from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import styles from "./Header.module.scss";

const Header: React.FC = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const token = localStorage.getItem("token");
    const [mobileMenuOpen, setMobileMenuOpen] = useState(false);
    const [showLogoutToast, setShowLogoutToast] = useState(false);
    const [isDark, setIsDark] = useState(document.documentElement.classList.contains("dark"));

    useEffect(() => {
        const observer = new MutationObserver(() => {
            setIsDark(document.documentElement.classList.contains("dark"));
        });
        observer.observe(document.documentElement, { attributes: true, attributeFilter: ["class"] });
        return () => observer.disconnect();
    }, []);

    const handleLogout = () => {
        localStorage.removeItem("token");
        setShowLogoutToast(true);
        setTimeout(() => {
            setShowLogoutToast(false);
            navigate("/login");
        }, 1200);
    };

    const toggleMobileMenu = () => setMobileMenuOpen((v) => !v);

    const toggleDarkMode = () => {
        document.documentElement.classList.toggle("dark");
    };

    const isActive = (path: string) => location.pathname === path;

    return (
        <>
            <header className={styles.header}>
                <div className={styles.container}>
                    <nav className={styles.navLeft}>
                        <Link to={token ? "/main" : "/"} className={styles.logo}>
                            <span className={styles.logoIcon}>兔</span>
                        </Link>
                        {!token ? (
                            <>

                            </>
                        ) : (
                            <>
                                <Link
                                    to="/study"
                                    className={`${styles.navLink} ${isActive("/study") ? styles.active : ""}`}
                                >
                                    Study
                                </Link>
                                <Link
                                    to="/review"
                                    className={`${styles.navLink} ${isActive("/review") ? styles.active : ""}`}
                                >
                                    Review
                                </Link>
                            </>
                        )}
                        
                    </nav>
                    <nav className={styles.nav}>
                        {!token ? (
                            <>
                                <Link
                                    to="/login"
                                    className={`${styles.navLink} ${isActive("/login") ? styles.active : ""}`}
                                >
                                    Log in
                                </Link>
                                <Link
                                    to="/signup"
                                    className={`${styles.navLink} ${isActive("/signup") ? styles.active : ""}`}
                                >
                                    Sign up
                                </Link>
                            </>
                        ) : (
                            <>
                                <button onClick={handleLogout} className={styles.button}>
                                    Logout
                                </button>

                                <Link
                                    to="/settings"
                                    className={`${styles.navLink} ${isActive("/settings") ? styles.active : ""}`}
                                >
                                        ⚙️
                                </Link>
                            </>
                        )}

                        {/* DARK MODE TOGGLE – Desktop */}
                        <button onClick={toggleDarkMode} className={styles.themeToggle}>
                            {isDark ? "🌞" : "🌛"}
                        </button>
                    </nav>

                    {/* Mobile Menu Toggle */}
                    <button
                        onClick={toggleMobileMenu}
                        className={styles.mobileToggle}
                        aria-label="Toggle menu"
                        aria-expanded={mobileMenuOpen}
                    >
                        <span className={mobileMenuOpen ? styles.hamburgerClose : styles.hamburger}>
                            {mobileMenuOpen ? "×" : "Menu"}
                        </span>
                    </button>
                </div>

                {/* Mobile Nav */}
                {mobileMenuOpen && (
                    <div className={styles.mobileNav}>
                        {!token ? (
                            <>
                                <Link to="/login" onClick={toggleMobileMenu} className={styles.mobileLink}>
                                    Login
                                </Link>
                                <Link to="/signup" onClick={toggleMobileMenu} className={styles.mobileLink}>
                                    Sign Up
                                </Link>
                            </>
                        ) : (
                            <>
                                <button onClick={handleLogout} className={styles.mobileLogout}>
                                    Logout
                                </button>

                            </>
                        )}

                        {/* DARK MODE TOGGLE – Mobile */}
                        <button onClick={toggleDarkMode} className={styles.themeToggle}>
                            {isDark ? "🌞" : "🌛"}
                        </button>
                    </div>
                )}
            </header>

            {/* Logout Toast */}
            {showLogoutToast && (
                <div className={styles.toast}>
                    <span>Logged out successfully</span>
                </div>
            )}
        </>
    );
};

export default Header;