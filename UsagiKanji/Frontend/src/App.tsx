import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Header from './components/Header/Header';
import LoginPage from './pages/LoginPage/LoginPage';
import SignUpPage from './pages/SignUpPage/SignUpPage';
import MainPage from './pages/MainPage/MainPage';
import ProtectedRoute from './components/ProtectedRoute';
import KanjiListPage from './pages/KanjiListPage/KanjiListPage';
import StudyPage from './pages/StudyPage/StudyPage';
import ReviewPage from './pages/ReviewPage/ReviewPage';

const App: React.FC = () => {
    return (
        <Router>
            <Header />
            <Routes>
                <Route path="/login" element={<LoginPage />} />
                <Route path="/signup" element={<SignUpPage />} />
                <Route
                    path="/main"
                    element={
                        <ProtectedRoute>
                            <MainPage />
                        </ProtectedRoute>
                    }
                />
                <Route
                    path="/study"
                    element={
                        <ProtectedRoute>
                            <KanjiListPage />
                        </ProtectedRoute>
                    }
                />
                <Route
                    path="/study/:id"
                    element={
                        <ProtectedRoute>
                            <StudyPage />
                        </ProtectedRoute>
                    }
                />
                <Route
                    path="/review"
                    element={
                        <ProtectedRoute>
                            <ReviewPage />
                        </ProtectedRoute>
                    }
                />
                <Route path="*" element={<LoginPage />} />
            </Routes>
        </Router>
    );
};

export default App;
