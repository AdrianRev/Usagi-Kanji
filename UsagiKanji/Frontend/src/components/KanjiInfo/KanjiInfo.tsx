import React, { useEffect, useState } from "react";
import type { KanjiDetail } from "../../types/kanji";
import styles from "./KanjiInfo.module.scss";

interface KanjiInfoProps {
    kanji: KanjiDetail;
    onSave: (keyword: string, notes: string) => Promise<void>;
    onNavigate: (direction: "prev" | "next") => void; // callback for navigation
}

const KanjiInfo: React.FC<KanjiInfoProps> = ({ kanji, onSave, onNavigate }) => {
    const [keyword, setKeyword] = useState(kanji.keyword || "");
    const [notes, setNotes] = useState(kanji.notes || "");
    const [saving, setSaving] = useState(false);

    useEffect(() => {
        const primaryMeaning = kanji.meanings.find(m => m.isPrimary)?.value || "";
        setKeyword(kanji.keyword || primaryMeaning);
        setNotes(kanji.notes || "");
    }, [kanji]);

    const handleSave = async () => {
        setSaving(true);
        try {
            await onSave(keyword, notes);
        } finally {
            setSaving(false);
        }
    };

    // Determine the current kanji index for display
    const sortBy = localStorage.getItem("kanji-sort-by") || "heisig6";
    const kanjiIndex =
        sortBy === "grade"
            ? kanji.sortIndexGrade
            : sortBy === "jlptlevel"
                ? kanji.sortIndexJLPT
                : sortBy === "frequency"
                    ? kanji.frequencyRank
                    : sortBy === "heisig6"
                        ? kanji.heisig6Number
                        : kanji.heisigNumber || 0;

    return (
        <div className={styles.wrapper}>
            {/* Left column */}
            <div className={styles.left}>
                {/* Navigation bar above keyword */}
                

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
                        {kanji.meanings
                            .filter(m => !m.isPrimary)
                            .map(m => `, ${m.value}`)}
                    </div>
                </div>

            </div>

            {/* Right column */}
            <div className={styles.right}>
                <div className={styles.field}>
                    <div className={styles.kanjiNavigation}>
                        <button
                            onClick={() => onNavigate("prev")}
                            className={styles.navButton}
                        >
                            &lt;
                        </button>
                        <span className={styles.kanjiIndex}>
                            {kanjiIndex || "-"}
                        </span>
                        <button
                            onClick={() => onNavigate("next")}
                            className={styles.navButton}
                        >
                            &gt;
                        </button>
                    </div>
                    <label htmlFor="notes"></label>
                    <textarea
                        id="notes"
                        value={notes}
                        onChange={e => setNotes(e.target.value)}
                        className={styles.notesTextarea}
                        placeholder="Add your notes here"
                    />
                    <div className={styles.saveWrapper}>
                        <button
                            onClick={handleSave}
                            disabled={saving}
                            className={styles.saveButton}
                        >
                            {saving ? "Saving..." : "Save"}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default KanjiInfo;
