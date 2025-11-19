import api from "./axiosInstance";
import type { KanjiListItem, PaginatedList, KanjiListParams } from '../types/kanji';


export const kanjiApi = {
    getAll: async (params: KanjiListParams): Promise<PaginatedList<KanjiListItem>> => {
        const response = await api.get<PaginatedList<KanjiListItem>>("/kanji", { params });
        return response.data;
    },
};