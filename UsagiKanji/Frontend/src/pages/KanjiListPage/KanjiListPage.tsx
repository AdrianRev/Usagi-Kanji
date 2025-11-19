import React, { useEffect, useState } from 'react';
import { kanjiApi } from "../../api/kanji";
import type { KanjiListItem, PaginatedList } from "../../types/kanji";
import styles from "./KanjiListPage.module.scss";


export default function KanjiListPage() {
    const [kanji, setKanji] = useState<KanjiListItem[]>([]);
    const [pageIndex, setPageIndex] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [sortBy, setSortBy] = useState<string>("");
    const [loading, setLoading] = useState(false);

    const loadKanji = async () => {
        setLoading(true);
        try {
            const data: PaginatedList<KanjiListItem> = await kanjiApi.getAll({
                pageIndex,
                pageSize: 25,
                sortBy: sortBy || undefined
            });
            setKanji(data.items);
            setTotalPages(data.totalPages);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadKanji();
    }, [pageIndex, sortBy]);

    return (
        <div className={styles.wrapper}>
            <div className={styles.controls}>
                <label className={styles.label}>Sort by:</label>
                <select
                    className={styles.select}
                    value={sortBy}
                    onChange={(e) => setSortBy(e.target.value)}
                >
                    <option value="grade">Grade</option>
                    <option value="jlptlevel">JLPT Level</option>
                    <option value="frequency">Frequency</option>
                    <option value="heisig">Heisig</option>
                    <option value="heisig6">Heisig 6th Ed</option>
                </select>
            </div>

            {loading ? (
                <p className={styles.loading}>Loading...</p>
            ) : (
                <div className={styles.grid}>
                    {kanji.map((k, index) => (
                        <div key={k.id || index} className={styles.card}>
                            <div className={styles.id}>{(pageIndex - 1) * 25 + index + 1}</div>
                            <div className={styles.character}>{k.character}</div>
                            <div className={styles.meaning}>{k.primaryMeaning}</div>
                        </div>
                    ))}
                </div>
            )}

            <div className={styles.pagination}>
                <button
                    className={styles.button}
                    onClick={() => setPageIndex(p => Math.max(1, p - 1))}
                    disabled={pageIndex === 1 || loading}
                >
                    Previous
                </button>
                <span className={styles.pageInfo}>
                    Page {pageIndex} of {totalPages}
                </span>
                <button
                    className={styles.button}
                    onClick={() => setPageIndex(p => p + 1)}
                    disabled={pageIndex === totalPages || loading}
                >
                    Next
                </button>
            </div>
        </div>
    );
}