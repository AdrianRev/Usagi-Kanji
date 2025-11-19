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