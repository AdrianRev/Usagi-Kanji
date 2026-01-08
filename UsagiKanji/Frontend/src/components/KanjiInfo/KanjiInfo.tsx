import React, { useEffect, useState } from "react";
import type { KanjiDetail } from "../../types/kanji";
import styles from "./KanjiInfo.module.scss";
import KanjiReadings from "../KanjiReadings/KanjiReadings";
import KanjiVocabulary from "../KanjiVocabulary/KanjiVocabulary";


interface KanjiInfoProps {
    kanji: KanjiDetail;
    onSave: (keyword: string, notes: string) => Promise<void>;
    onNavigate: (direction: "prev" | "next") => void;
    prevExists: boolean;
    nextExists: boolean;
}

const KanjiInfo: React.FC<KanjiInfoProps> = ({ kanji, onSave, onNavigate, prevExists, nextExists }) => {
    const [keyword, setKeyword] = useState(kanji.keyword || "");
    const [notes, setNotes] = useState(kanji.notes || "");
    const [saveMessage, setSaveMessage] = useState("");

    useEffect(() => {
        const primaryMeaning = kanji.meanings.find(m => m.isPrimary)?.value || "";
        setKeyword(kanji.keyword || primaryMeaning);
        setNotes(kanji.notes || "");
    }, [kanji]);

    const handleSave = async () => {
        try {
            await onSave(keyword, notes);
            setSaveMessage("Keyword and notes saved successfully!");
            setTimeout(() => setSaveMessage(""), 3000);
        } catch (error) {
            setSaveMessage("Save failed");
            setTimeout(() => {
                setSaveMessage("");
            }, 3000);
        }
    };

    const sortBy = localStorage.getItem("kanji-sort-by") || "heisig6";
    const kanjiIndex = (() => {
        switch (sortBy) {
            case "grade": return kanji.sortIndex_Grade;
            case "jlptlevel": return kanji.sortIndex_JLPT;
            case "frequency": return kanji.frequencyRank;
            case "heisig6": return kanji.heisig6Number;
            case "heisig": return kanji.heisigNumber;
            default: return undefined;
        }
    })();

    return (
        <div className={styles.wrapper}>
            <div className={styles.left}>
                <input
                    type="text"
                    className={styles.keywordInput}
                    value={keyword}
                    onChange={e => setKeyword(e.target.value)}
                    placeholder="Keyword"
                />

                <div className={styles.kanjiCharacter}>{kanji.character}</div>

                <div className={styles.meaningsField}>
                    <label>Meanings:</label>
                    <div className={styles.meanings}>
                        <strong>{kanji.meanings.find(m => m.isPrimary)?.value}</strong>
                        {kanji.meanings.filter(m => !m.isPrimary).map(m => `, ${m.value}`)}
                    </div>
                </div>
                <KanjiReadings readings={kanji.readings} />
                <div className={styles.kanjiStats}>
                    <div>
                        <strong>Strokes: </strong> {kanji.strokeCount}
                    </div>
                    <div>
                        <strong>Grade:</strong> {kanji.grade || "-"}
                    </div>
                    <div>
                        <strong>JLPT exam level:</strong>N{kanji.jlptLevel || "-"}
                    </div>
                    <div>
                        <strong>Frequency Rank:</strong> {kanji.frequencyRank || "-"}
                    </div>
                </div>
            </div>

            <div className={styles.right}>
                <div className={styles.field}>
                    <div className={styles.kanjiNavigation}>
                        <button
                            onClick={() => onNavigate("prev")}
                            className={styles.navButton}
                            disabled={!prevExists}
                        >
                            &lt;
                        </button>
                        <span className={styles.kanjiIndex}>{kanjiIndex || "-"}</span>
                        <button
                            onClick={() => onNavigate("next")}
                            className={styles.navButton}
                            disabled={!nextExists}
                        >
                            &gt;
                        </button>
                    </div>

                    <div className={styles.notesContent}>
                        <textarea
                            id="notes"
                            value={notes}
                            onChange={e => setNotes(e.target.value)}
                            className={styles.notesTextarea}
                            placeholder="Add your notes here"
                        />
                    </div>

                    <div className={styles.saveWrapper}>
                        {saveMessage && (
                            <p className={styles.successMessage}>{saveMessage}</p>
                        )}
                        <button
                            onClick={handleSave}
                            className={styles.saveButton}
                        >
                            Save
                        </button>
                    </div>

                    <KanjiVocabulary vocabulary={kanji.vocabulary} />
                </div>
            </div>

        </div>
    );
};

export default KanjiInfo;
