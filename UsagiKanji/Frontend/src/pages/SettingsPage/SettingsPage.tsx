import { useState } from "react";
import styles from "./SettingsPage.module.scss";

type KanjiSortSetting = "grade" | "jlptlevel" | "frequency" | "heisig";
type HeisigEdition = "6th" | "older";

export default function SettingsPage() {
    // Learning settings
    const [sortBy, setSortBy] = useState<KanjiSortSetting>(() =>
        (localStorage.getItem("kanji-sort-by") as KanjiSortSetting) || "grade"
    );
    const [heisigEdition, setHeisigEdition] = useState<HeisigEdition>(() =>
        (localStorage.getItem("heisig-edition") as HeisigEdition) || "6th"
    );

    const [saveMessage, setSaveMessage] = useState("");

    const handleSaveSettings = () => {
        localStorage.setItem("kanji-sort-by", sortBy);
        localStorage.setItem("heisig-edition", heisigEdition);

        setSaveMessage("Settings saved successfully!");
        setTimeout(() => setSaveMessage(""), 3000);
    };


    return (
        <div className={styles.container}>
            <div className={styles.settingsGrid}>
                {/* Learning Preferences */}
                <section className={styles.section}>
                    <h2>Kanji Sort Order preferences</h2>

                    <div className={styles.settingGroup}>
                        <label htmlFor="sortBy">
                            Kanji Sort Order
                            <span className={styles.description}>
                                How kanji are ordered for study
                            </span>
                        </label>
                        <select
                            id="sortBy"
                            value={sortBy}
                            onChange={(e) =>
                                setSortBy(e.target.value as KanjiSortSetting)
                            }
                            className={styles.select}
                        >
                            <option value="grade">Grade</option>
                            <option value="jlptlevel">JLPT Level</option>
                            <option value="frequency">Frequency</option>
                            <option value="heisig">Heisig</option>
                        </select>
                    </div>

                    <div className={styles.settingGroup}>
                        <label className={styles.switchLabel}>
                            Use Heisig 6th Edition
                            <span className={styles.description}>
                                Toggle between 6th edition and older RTK ordering
                            </span>
                        </label>
                        <input
                            type="checkbox"
                            checked={heisigEdition === "6th"}
                            onChange={(e) =>
                                setHeisigEdition(e.target.checked ? "6th" : "older")
                            }
                        />
                    </div>

                    <button
                        onClick={handleSaveSettings}
                        className={styles.primaryButton}
                    >
                        Save Kanji Order Preferences
                    </button>
                    {saveMessage && (
                        <p className={styles.successMessage}>{saveMessage}</p>
                    )}
                </section>
            </div>
        </div>
    );
}
