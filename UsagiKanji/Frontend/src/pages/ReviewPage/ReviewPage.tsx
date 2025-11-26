// src/pages/ReviewPage/ReviewPage.tsx
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { reviewApi } from "../../api/review";
import type { DueKanji, KanjiReviewItem } from "../../types/review";
import styles from "./ReviewPage.module.scss";

export default function ReviewPage() {
    const navigate = useNavigate();

    const [queue, setQueue] = useState<DueKanji[]>([]);
    const [history, setHistory] = useState<KanjiReviewItem[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [isFlipped, setIsFlipped] = useState(false);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const current = queue[0];

    useEffect(() => {
        loadDue();
    }, []);

    const loadDue = async () => {
        setIsLoading(true);
        try {
            const due = await reviewApi.getDue();
            setQueue(due);
        } catch (err) {
            alert("Failed to load reviews. Redirecting to login...");
            navigate("/login");
        } finally {
            setIsLoading(false);
        }
    };

    const answer = (rating: KanjiReviewItem["rating"]) => {
        if (!current) return;

        setHistory((h) => [...h, { kanjiId: current.kanjiId, rating }]);
        setQueue((q) => q.slice(1));
        setIsFlipped(false);
    };

    const undo = () => {
        if (history.length === 0) return;

        const last = history[history.length - 1];
        setHistory((h) => h.slice(0, -1));
        setQueue((q) => [current!, ...q]);
        setIsFlipped(false);
    };

    const submitAll = async () => {
        if (history.length === 0) {
            navigate("/study");
            return;
        }

        setIsSubmitting(true);
        try {
            await reviewApi.submitBatch({ reviews: history });
            alert(`Session complete! ${history.length} reviews saved.`);
            navigate("/study");
        } catch (err) {
            alert("Failed to save reviews. Please try again.");
        } finally {
            setIsSubmitting(false);
        }
    };

    // Keyboard shortcuts
    useEffect(() => {
        const handler = (e: KeyboardEvent) => {
            if (!current) return;
            if (e.key === " ") { e.preventDefault(); setIsFlipped(v => !v); }
            if (e.key === "1") answer("Again");
            if (e.key === "2") answer("Hard");
            if (e.key === "3") answer("Good");
            if (e.key === "4") answer("Easy");
            if (e.key === "z" && (e.ctrlKey || e.metaKey)) { e.preventDefault(); undo(); }
        };
        window.addEventListener("keydown", handler);
        return () => window.removeEventListener("keydown", handler);
    }, [current, history]);

    if (isLoading) {
        return <div className={styles.center}><p>Loading your reviews...</p></div>;
    }

    if (queue.length === 0 && history.length === 0) {
        return (
            <div className={styles.center}>
                <h1>No reviews due today!</h1>
                <p>You're all caught up. Great work!</p>
                <button className={styles.button} onClick={() => navigate("/study")}>
                    Browse Kanji
                </button>
            </div>
        );
    }

    return (
        <div className={styles.wrapper}>
            <div className={styles.header}>
                <button className={styles.backBtn} onClick={() => navigate("/study")}>
                    ? Back
                </button>
                <div className={styles.stats}>
                    <span>Due: {queue.length}</span>
                    <span>Done: {history.length}</span>
                    {history.length > 0 && (
                        <button onClick={undo} className={styles.undoBtn}>
                            Undo (Ctrl+Z)
                        </button>
                    )}
                </div>
            </div>

            {current && (
                <>
                    <div className={styles.cardWrapper}>
                        <div
                            className={`${styles.card} ${isFlipped ? styles.flipped : ""}`}
                            onClick={() => setIsFlipped(!isFlipped)}
                        >
                            <div className={styles.front}>
                                <div className={styles.character}>{current.character}</div>
                                <p className={styles.hint}>Click or press Space to reveal</p>
                            </div>

                            <div className={styles.back}>
                                <div className={styles.keyword}>
                                    {current.keyword || "— no keyword —"}
                                </div>
                                {current.notes && (
                                    <div className={styles.notes}>{current.notes}</div>
                                )}
                                <p className={styles.hint}>Click to hide</p>
                            </div>
                        </div>
                    </div>

                    <div className={styles.buttons}>
                        <button onClick={() => answer("Again")} className={styles.again}>Again <kbd>1</kbd></button>
                        <button onClick={() => answer("Hard")} className={styles.hard}>Hard <kbd>2</kbd></button>
                        <button onClick={() => answer("Good")} className={styles.good}>Good <kbd>3</kbd></button>
                        <button onClick={() => answer("Easy")} className={styles.easy}>Easy <kbd>4</kbd></button>
                    </div>
                </>
            )}

            {queue.length === 0 && history.length > 0 && (
                <div className={styles.finish}>
                    <button
                        onClick={submitAll}
                        disabled={isSubmitting}
                        className={styles.finishButton}
                    >
                        {isSubmitting ? "Saving..." : `Finish & Save (${history.length})`}
                    </button>
                </div>
            )}
        </div>
    );
}