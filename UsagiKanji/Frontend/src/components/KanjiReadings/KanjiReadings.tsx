import React from "react";
import type { Reading } from "../../types/kanji";
import styles from "./KanjiReadings.module.scss";

interface Props {
    readings: Reading[];
}

const KanjiReadings: React.FC<Props> = ({ readings }) => {
    const grouped = {
        Onyomi: readings.filter(r => r.type === "Onyomi").map(r => r.value),
        Kunyomi: readings.filter(r => r.type === "Kunyomi").map(r => r.value),
        Nanori: readings.filter(r => r.type === "Nanori").map(r => r.value),
    };

    return (
        <div className={styles.readingsWrapper}>
            <h3>Readings</h3>
            <div className={styles.list}>
                <div><strong>Onyomi:</strong> {grouped.Onyomi.join(", ") || "-"}</div>
                <div><strong>Kunyomi:</strong> {grouped.Kunyomi.join(", ") || "-"}</div>
                <div><strong>Nanori:</strong> {grouped.Nanori.join(", ") || "-"}</div>
            </div>
        </div>
    );
};

export default KanjiReadings;
