import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { reviewApi } from "../../api/review";
import { kanjiApi } from "../../api/kanji";
import styles from "./MainPage.module.scss";

interface NextKanji {
    kanjiId: string;
    character: string;
    keyword: string;
    heisig6?: number;
}

export default function MainPage() {
    const navigate = useNavigate();
    const [dueCount, setDueCount] = useState<number>(0);
    const [nextKanji, setNextKanji] = useState<NextKanji | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        loadData();
    }, []);

    const loadData = async () => {
        setIsLoading(true);
        try {
            const dueList = await reviewApi.getDue();
            setDueCount(dueList.length);

            const sortBy = localStorage.getItem("kanji-sort-by") || "heisig6";
            const next = await kanjiApi.getNextToLearn(sortBy);
            setNextKanji(next);
        } catch (error) {
            console.error("Failed to load data:", error);
        } finally {
            setIsLoading(false);
        }
    };

    const startReviews = () => {
        navigate("/review");
    };

    const learnKanji = () => {
        if (nextKanji) {
            navigate(`/kanji/${nextKanji.kanjiId}`);
        }
    };

    if (isLoading) {
        return (
            <div className={styles.center}>
                <p>Loading...</p>
            </div>
        );
    }

    return (
        <div className={styles.container}>
            <h1>Welcome to UsagiKanji</h1>

            <div className={styles.cardGrid}>
                <div className={styles.card}>
                    <h2>Reviews</h2>
                    <div className={styles.count}>{dueCount}</div>
                    <p>cards due today</p>
                    <button
                        onClick={startReviews}
                        disabled={dueCount === 0}
                        className={styles.primaryButton}
                    >
                        {dueCount > 0 ? "Start Reviews" : "No Reviews Due"}
                    </button>
                </div>
                <div className={styles.card}>
                    <h2>Learn New</h2>
                    {nextKanji ? (
                        <>
                            <div className={styles.nextKanji}>
                                <div className={styles.character}>{nextKanji.character}</div>
                                <div className={styles.keyword}>{nextKanji.keyword}</div>
                            </div>
                            <button
                                onClick={learnKanji}
                                className={styles.primaryButton}
                            >
                                Learn This Kanji
                            </button>
                        </>
                    ) : (
                        <p>All kanji learned! 🎉</p>
                    )}
                </div>
            </div>
        </div>
    );
}