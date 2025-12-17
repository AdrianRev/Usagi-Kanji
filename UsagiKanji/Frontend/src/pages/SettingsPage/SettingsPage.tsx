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

    const [currentPassword, setCurrentPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [passwordMessage, setPasswordMessage] = useState("");

    const [saveMessage, setSaveMessage] = useState("");

    const handleSaveSettings = () => {
        localStorage.setItem("kanji-sort-by", sortBy);
        localStorage.setItem("heisig-edition", heisigEdition);

        setSaveMessage("Settings saved successfully!");
        setTimeout(() => setSaveMessage(""), 3000);
    };

    const handlePasswordChange = async () => {
        if (newPassword !== confirmPassword) {
            setPasswordMessage("Passwords don't match!");
            return;
        }
        if (newPassword.length < 8) {
            setPasswordMessage("Password must be at least 8 characters!");
            return;
        }

        try {
            // API call goes here
            // await userApi.changePassword(currentPassword, newPassword);

            setPasswordMessage("Password changed successfully!");
            setCurrentPassword("");
            setNewPassword("");
            setConfirmPassword("");
            setTimeout(() => setPasswordMessage(""), 3000);
        } catch {
            setPasswordMessage("Failed to change password. Please try again.");
        }
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
                        Kanji Order Preferences
                    </button>
                    {saveMessage && (
                        <p className={styles.successMessage}>{saveMessage}</p>
                    )}
                </section>

                <section className={styles.section}>
                    <h2>Change Password</h2>

                    <div className={styles.settingGroup}>
                        <label htmlFor="currentPassword">Current Password</label>
                        <input
                            id="currentPassword"
                            type="password"
                            value={currentPassword}
                            onChange={(e) => setCurrentPassword(e.target.value)}
                            className={styles.input}
                        />
                    </div>

                    <div className={styles.settingGroup}>
                        <label htmlFor="newPassword">New Password</label>
                        <input
                            id="newPassword"
                            type="password"
                            value={newPassword}
                            onChange={(e) => setNewPassword(e.target.value)}
                            className={styles.input}
                        />
                    </div>

                    <div className={styles.settingGroup}>
                        <label htmlFor="confirmPassword">Confirm New Password</label>
                        <input
                            id="confirmPassword"
                            type="password"
                            value={confirmPassword}
                            onChange={(e) => setConfirmPassword(e.target.value)}
                            className={styles.input}
                        />
                    </div>

                    <button
                        onClick={handlePasswordChange}
                        className={styles.primaryButton}
                    >
                        Change Password
                    </button>

                    {passwordMessage && (
                        <p
                            className={
                                passwordMessage.includes("success")
                                    ? styles.successMessage
                                    : styles.errorMessage
                            }
                        >
                            {passwordMessage}
                        </p>
                    )}
                </section>
            </div>
        </div>
    );
}
