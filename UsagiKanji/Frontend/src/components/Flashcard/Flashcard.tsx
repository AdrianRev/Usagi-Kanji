import { memo } from "react";
import cn from "classnames";
import styles from "./Flashcard.module.scss";

interface FlashcardProps {
    character: string;
    keyword?: string;
    notes?: string;
    isFlipped: boolean;
    onToggle: () => void;
}

function Flashcard({ character, keyword, notes, isFlipped, onToggle }: FlashcardProps) {
    return (
        <div className={styles.cardWrapper}>
            <div
                className={cn(styles.card, { [styles.flipped]: isFlipped })}
                onClick={onToggle}
            >
                <div className={styles.front}>
                    <div className={styles.character}>{character}</div>
                    <p className={styles.hint}>Click or press Space to reveal</p>
                </div>

                <div className={styles.back}>
                    <div className={styles.keyword}>{keyword || "— no keyword —"}</div>
                    {notes && <div className={styles.notes}>{notes}</div>}
                    <p className={styles.hint}>Click to hide</p>
                </div>
            </div>
        </div>
    );
}

export default memo(Flashcard);