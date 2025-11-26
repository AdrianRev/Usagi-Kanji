import api from "./axiosInstance";
import type { DueKanji, SubmitBatchReviewDto} from '../types/review';
export const reviewApi = {
    getDue: async (): Promise<DueKanji[]> => {
        const response = await api.get<DueKanji[]>("/Review/due");
        return response.data;
    },

    submitBatch: async (dto: SubmitBatchReviewDto): Promise<void> => {
        await api.post("/Review/batch", dto);
    },
};