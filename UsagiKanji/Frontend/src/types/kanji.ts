export interface KanjiListItem {
    id: string;
    character: string;
    primaryMeaning?: string;
    isLearned: boolean;
}

export interface PaginatedList<T> {
    items: T[];
    pageIndex: number;
    totalPages: number;
}

export interface KanjiListParams {
    pageIndex: number;
    pageSize: number;
    sortBy?: string;
}

export type ReadingType = "Onyomi" | "Kunyomi" | "Nanori";

export interface Reading {
    value: string;
    type: ReadingType;
}

export interface Meaning {
    value: string;
    isPrimary: boolean;
}

export interface VocabularyItem {
    text: string;
    common: boolean;
    kanaReadings: string[];
    glosses: string[];
}

export interface KanjiDetail {
    id: string;
    character: string;
    strokeCount: number;
    grade: number;
    jlptLevel?: number;
    frequencyRank?: number;
    heisigNumber?: number;
    heisig6Number?: number;
    readings: Reading[];
    meanings: Meaning[];
    vocabulary: VocabularyItem[];
    notes?: string;
    keyword?: string;
    isLearned: boolean;
}