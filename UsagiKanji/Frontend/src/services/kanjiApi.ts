import axios from 'axios';
import type { KanjiListItem, PaginatedList, KanjiListParams } from '../types/kanji';

const API_BASE_URL = 'http://localhost:5261';

export const kanjiApi = {
    getAll: async (params: KanjiListParams) => {
        const response = await axios.get<PaginatedList<KanjiListItem>>(
            `${API_BASE_URL}/api/kanji`,
            { params }
        );
        return response.data;
    }
};