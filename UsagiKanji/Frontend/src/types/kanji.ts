export interface KanjiListItem {
    id: number;
    character: string;
    primaryMeaning?: string;
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