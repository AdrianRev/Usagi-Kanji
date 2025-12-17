import { useEffect, useState } from "react";
import { kanjiApi } from "../../api/kanji";
import type { KanjiListItem, PaginatedList } from "../../types/kanji";
import styles from "./KanjiListPage.module.scss";
import { useNavigate } from "react-router-dom";
import useWebsiteTitle from "../../hooks/useWebsiteTitle";

type KanjiSortSetting = "grade" | "jlptlevel" | "frequency" | "heisig";
type HeisigEdition = "6th" | "older";
type KanjiSortApi =
    | "grade"
    | "jlptlevel"
    | "frequency"
    | "heisig"
    | "heisig6";

export default function KanjiListPage() {
    useWebsiteTitle("Study - UsagiKanji");

    const [kanji, setKanji] = useState<KanjiListItem[]>([]);
    const [pageIndex, setPageIndex] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [loading, setLoading] = useState(false);

    // 🔹 Settings defaults
    const [settingsSortBy, setSettingsSortBy] =
        useState<KanjiSortSetting>(() =>
            (localStorage.getItem("kanji-sort-by") as KanjiSortSetting) ||
            "grade"
        );

    const [heisigEdition, setHeisigEdition] =
        useState<HeisigEdition>(() =>
            (localStorage.getItem("heisig-edition") as HeisigEdition) ||
            "6th"
        );

    // 🔹 Local dropdown state (always a real value)
    const [localSortBy, setLocalSortBy] =
        useState<KanjiSortSetting>(settingsSortBy);

    // 🔹 Has user manually changed it?
    const [isOverridden, setIsOverridden] = useState(false);

    const navigate = useNavigate();

    // 🔹 Resolve final sort
    const resolveSortBy = (): KanjiSortApi => {
        const sort = isOverridden ? localSortBy : settingsSortBy;

        if (sort !== "heisig") return sort;
        return heisigEdition === "6th" ? "heisig6" : "heisig";
    };

    const loadKanji = async () => {
        setLoading(true);
        try {
            const data: PaginatedList<KanjiListItem> =
                await kanjiApi.getAll({
                    pageIndex,
                    pageSize: 25,
                    sortBy: resolveSortBy(),
                });

            setKanji(data.items);
            setTotalPages(data.totalPages);
        } finally {
            setLoading(false);
        }
    };

    // 🔹 Load when relevant state changes
    useEffect(() => {
        loadKanji();
    }, [pageIndex, localSortBy, settingsSortBy, heisigEdition]);

    // 🔹 React to settings changes
    useEffect(() => {
        const handler = () => {
            const newSettingsSort =
                (localStorage.getItem(
                    "kanji-sort-by"
                ) as KanjiSortSetting) || "grade";

            const newEdition =
                (localStorage.getItem(
                    "heisig-edition"
                ) as HeisigEdition) || "6th";

            setSettingsSortBy(newSettingsSort);
            setHeisigEdition(newEdition);

            // Only update dropdown if user hasn't overridden
            if (!isOverridden) {
                setLocalSortBy(newSettingsSort);
            }
        };

        window.addEventListener("storage", handler);
        return () => window.removeEventListener("storage", handler);
    }, [isOverridden]);

    const handleSortChange = (value: KanjiSortSetting) => {
        setLocalSortBy(value);
        setIsOverridden(true);
        setPageIndex(1);
    };

    const handleKanjiClick = (id: string) => {
        navigate(`/study/${id}`);
    };

    return (
        <div className={styles.wrapper}>
            <div className={styles.controls}>
                <label className={styles.label}>Sort by:</label>
                <select
                    className={styles.select}
                    value={localSortBy}
                    onChange={(e) =>
                        handleSortChange(
                            e.target.value as KanjiSortSetting
                        )
                    }
                >
                    <option value="grade">Grade</option>
                    <option value="jlptlevel">JLPT Level</option>
                    <option value="frequency">Frequency</option>
                    <option value="heisig">Heisig</option>
                </select>
            </div>

            {loading ? (
                <p className={styles.loading}>Loading...</p>
            ) : (
                <div className={styles.grid}>
                    {kanji.map((k, index) => (
                        <div
                            key={k.id}
                            className={`${styles.card} ${k.isLearned ? styles.learned : ""
                                }`}
                            onClick={() => handleKanjiClick(k.id)}
                        >
                            <div className={styles.id}>
                                {(pageIndex - 1) * 25 + index + 1}
                            </div>
                            <div className={styles.character}>
                                {k.character}
                            </div>
                            <div className={styles.meaning}>
                                {k.primaryMeaning}
                            </div>
                        </div>
                    ))}
                </div>
            )}

            <div className={styles.pagination}>
                <button
                    className={styles.button}
                    onClick={() =>
                        setPageIndex((p) => Math.max(1, p - 1))
                    }
                    disabled={pageIndex === 1 || loading}
                >
                    Previous
                </button>

                <span className={styles.pageInfo}>
                    Page {pageIndex} of {totalPages}
                </span>

                <button
                    className={styles.button}
                    onClick={() => setPageIndex((p) => p + 1)}
                    disabled={pageIndex === totalPages || loading}
                >
                    Next
                </button>
            </div>
        </div>
    );
}
