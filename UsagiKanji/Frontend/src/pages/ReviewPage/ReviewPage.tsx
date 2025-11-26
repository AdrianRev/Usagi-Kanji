import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { reviewApi } from "../../api/review";
import type { DueKanji, KanjiReviewItem } from "../../types/review";
import Flashcard from "../../components/Flashcard/Flashcard";
import styles from "./ReviewPage.module.scss";

interface ReviewHistoryItem extends KanjiReviewItem {
    kanji: DueKanji;
}

export default function ReviewPage() {
    const navigate = useNavigate();
    const [queue, setQueue] = useState<DueKanji[]>([]);
    const [history, setHistory] = useState<ReviewHistoryItem[]>([]);
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
        } catch {
            alert("Failed to load reviews. Redirecting to login...");
            navigate("/login");
        } finally {
            setIsLoading(false);
        }
    };

    const answer = (rating: KanjiReviewItem["rating"]) => {
        if (!current) return;

        setHistory(h => [...h, {
            kanjiId: current.kanjiId,
            rating,
            kanji: current
        }]);
        setQueue(q => q.slice(1));
        setIsFlipped(false);
    };

    const undo = () => {
        if (history.length === 0) return;

        const lastReview = history[history.length - 1];
        setHistory(h => h.slice(0, -1));

        setQueue(q => [lastReview.kanji, ...q]);
        setIsFlipped(false);
    };

    const endSession = async () => {
        if (!history.length) {
            navigate("/study");
            return;
        }
        if (!confirm(`End session early? ${history.length} review(s) will be saved.`)) {
            return;
        }
        setIsSubmitting(true);
        try {
            const reviewMap = new Map<string, KanjiReviewItem["rating"]>();
            history.forEach(({ kanjiId, rating }) => {
                if (rating !== "Again") {
                    reviewMap.set(kanjiId, rating);
                }
            });
            const reviews = Array.from(reviewMap.entries()).map(([kanjiId, rating]) => ({ kanjiId, rating }));
            await reviewApi.submitBatch({ reviews });
            alert(`Session ended. ${reviews.length} review(s) saved.`);
            navigate("/study");
        } catch {
            alert("Failed to save reviews.");
        } finally {
            setIsSubmitting(false);
        }
    };

    const submitAll = async () => {
        setIsSubmitting(true);
        try {
            const reviewMap = new Map<string, KanjiReviewItem["rating"]>();
            history.forEach(({ kanjiId, rating }) => {
                if (rating !== "Again") {
                    reviewMap.set(kanjiId, rating);
                }
            });
            const reviews = Array.from(reviewMap.entries()).map(([kanjiId, rating]) => ({ kanjiId, rating }));
            await reviewApi.submitBatch({ reviews });
            alert(`Session complete! ${reviews.length} reviews saved.`);
            navigate("/study");
        } catch {
            alert("Failed to save reviews.");
        } finally {
            setIsSubmitting(false);
        }
    };

    useEffect(() => {
        const handler = (e: KeyboardEvent) => {
            if (!current) return;
            if (e.key === " ") { e.preventDefault(); setIsFlipped(v => !v); }
            if (e.key === "1") answer("Again");
            if (e.key === "2") answer("Hard");
            if (e.key === "3") answer("Good");
            if (e.key === "4") answer("Easy");
            if ((e.ctrlKey || e.metaKey) && e.key === "z") { e.preventDefault(); undo(); }
        };
        window.addEventListener("keydown", handler);
        return () => window.removeEventListener("keydown", handler);
    }, [current, history.length]);

    if (isLoading) {
        return <div className={styles.center}><p>Loading your reviews...</p></div>;
    }

    if (!queue.length && !history.length) {
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
                <button
                    onClick={endSession}
                    disabled={isSubmitting}
                    className={styles.endSessionBtn}
                >
                    {isSubmitting ? "Saving..." : "End Session"}
                </button>
                <div className={styles.stats}>
                    <span>Remaining: {queue.length}</span>
                    <span>Again: {history.filter(h => h.rating === "Again").length}</span>
                </div>
                <div className={styles.stats}>
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
                    <Flashcard
                        character={current.character}
                        keyword={current.keyword}
                        notes={current.notes}
                        isFlipped={isFlipped}
                        onToggle={() => setIsFlipped(v => !v)}
                    />
                    <div className={styles.buttons}>
                        <button onClick={() => answer("Again")} className={styles.again}>
                            Again <kbd>1</kbd>
                        </button>
                        <button onClick={() => answer("Hard")} className={styles.hard}>
                            Hard <kbd>2</kbd>
                        </button>
                        <button onClick={() => answer("Good")} className={styles.good}>
                            Good <kbd>3</kbd>
                        </button>
                        <button onClick={() => answer("Easy")} className={styles.easy}>
                            Easy <kbd>4</kbd>
                        </button>
                    </div>
                </>
            )}

            {/* Final finish screen when queue is done */}
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