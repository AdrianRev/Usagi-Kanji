import React from "react";
import type { VocabularyItem } from "../../types/kanji";
import styles from "./KanjiVocabulary.module.scss";

interface Props {
    vocabulary: VocabularyItem[];
}

const KanjiVocabulary: React.FC<Props> = ({ vocabulary }) => {
    return (
        <div className={styles.vocabWrapper}>
            <h3>Vocabulary</h3>

            {vocabulary.length === 0 && <div>No vocabulary found.</div>}

            <ul className={styles.vocabList}>
                {vocabulary.map((v, i) => (
                    <li key={i} className={styles.item}>
                        <div className={styles.word}>
                            {v.text} {v.common && <span className={styles.commonTag}>common</span>}
                        </div>
                        <div className={styles.kana}>{v.kanaReadings.join(", ")}</div>
                        <div className={styles.gloss}>{v.glosses.join(", ")}</div>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default KanjiVocabulary;
