export interface DueKanji {
    kanjiId: string;
    character: string;
    keyword?: string;
    notes?: string;
    interval: number;
    nextReviewDate?: string;
}

export interface KanjiReviewItem {
    kanjiId: string;
    rating: "Again" | "Hard" | "Good" | "Easy";
}

export interface SubmitBatchReviewDto {
    reviews: KanjiReviewItem[];
}
