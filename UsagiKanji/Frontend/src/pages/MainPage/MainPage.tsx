import React from 'react';
import './MainPage.scss';
import useWebsiteTitle from "../../hooks/useWebsiteTitle";

const MainPage: React.FC = () => {
    useWebsiteTitle("UsagiKanji");
    const token = localStorage.getItem('token');

    const handleLogout = () => {
        localStorage.removeItem('token');
        window.location.href = '/login';
    };

    return (
        <div className="main-page">
            <h1>Welcome to the Main Page!</h1>
            <p>Your JWT token: {token}</p>
            <button onClick={handleLogout}>Logout</button>
        </div>
    );
};

export default MainPage;
