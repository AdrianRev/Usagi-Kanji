import React, { useState, useRef, useEffect } from "react";
import type { VocabularyItem } from "../../types/kanji";
import styles from "./KanjiVocabulary.module.scss";

interface Props {
    vocabulary: VocabularyItem[];
}

const ITEM_HEIGHT = 70;
const HEADER_HEIGHT = 40;
const BUTTON_HEIGHT = 30;

const KanjiVocabulary: React.FC<Props> = ({ vocabulary }) => {
    const commonVocab = vocabulary.filter(v => v.common);
    const [showAll, setShowAll] = useState(false);
    const [baseCount, setBaseCount] = useState(5);
    const wrapperRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        const calculateBaseCount = () => {
            if (!wrapperRef.current) return;

            const availableHeight = wrapperRef.current.clientHeight;
            const listSpace = availableHeight - HEADER_HEIGHT - BUTTON_HEIGHT;
            const count = Math.max(1, Math.floor(listSpace / ITEM_HEIGHT));

            setBaseCount(count);
        };

        calculateBaseCount();
        window.addEventListener("resize", calculateBaseCount);
        return () => window.removeEventListener("resize", calculateBaseCount);
    }, []);

    if (commonVocab.length === 0) return <div>No common vocabulary found.</div>;

    const visibleVocab = showAll ? commonVocab : commonVocab.slice(0, baseCount);
    const hasMore = commonVocab.length > baseCount;

    return (
        <div className={styles.vocabWrapper} ref={wrapperRef}>
            <h3>Vocabulary</h3>
            <ul className={`${styles.vocabList} ${showAll ? styles.scrollable : ""}`}>
                {visibleVocab.map((v, i) => (
                    <li key={i} className={styles.item}>
                        <span className={styles.word}>{v.text}</span>
                        <span className={styles.kana}>{v.kanaReadings.join(", ")}</span>
                        <span className={styles.gloss}>{v.glosses.join(", ")}</span>
                    </li>
                ))}
            </ul>
            {hasMore && (
                <div className={styles.showMoreWrapper}>
                    <button
                        className={styles.showMoreBtn}
                        onClick={() => setShowAll(true)}
                        style={{ visibility: showAll ? "hidden" : "visible" }}
                    >
                        Show more
                    </button>
                </div>
            )}
        </div>
    );
};

export default KanjiVocabulary;