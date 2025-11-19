import api from "./axiosInstance";
import type { KanjiListItem, PaginatedList, KanjiListParams, KanjiDetail } from '../types/kanji';


export const kanjiApi = {
    getAll: async (params: KanjiListParams): Promise<PaginatedList<KanjiListItem>> => {
        const response = await api.get<PaginatedList<KanjiListItem>>("/kanji", { params });
        return response.data;
    },
    getById: async (id: string): Promise<KanjiDetail> => {
        const response = await api.get<KanjiDetail>(`/kanji/${id}`);
        return response.data;
    },
    update: async (id: string, payload: { keyword?: string; notes?: string }) => {
        const response = await api.post(`/kanji/${id}`, payload);
        return response.data;
    },
};