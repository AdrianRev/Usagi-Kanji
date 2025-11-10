// src/components/KanjiList.tsx
import { useState, useEffect } from 'react';
import { kanjiApi } from '../services/kanjiApi';
import type { KanjiListItem } from '../types/kanji';

type SortOption = 'grade' | 'jlptlevel' | 'frequency' | 'heisig' | 'heisig6' | '';

function KanjiList() {
    const [kanji, setKanji] = useState<KanjiListItem[]>([]);
    const [pageIndex, setPageIndex] = useState(1);
    const [totalPages, setTotalPages] = useState(0);
    const [loading, setLoading] = useState(false);
    const [sortBy, setSortBy] = useState<SortOption>('');

    useEffect(() => {
        const fetchKanji = async () => {
            setLoading(true);
            try {
                const data = await kanjiApi.getAll({
                    pageIndex,
                    pageSize: 20,
                    ...(sortBy && { sortBy })
                });
                setKanji(data.items);
                setTotalPages(data.totalPages);
            } catch (error) {
                console.error('Failed to fetch kanji:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchKanji();
    }, [pageIndex, sortBy]);

    const handleSortChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setSortBy(e.target.value as SortOption);
        setPageIndex(1); // Reset to first page when sorting changes
    };

    return (
        <div>
            <h1>Kanji List</h1>

            <div style={{ marginBottom: '1rem' }}>
                <label htmlFor="sort">Sort by: </label>
                <select id="sort" value={sortBy} onChange={handleSortChange}>
                    <option value="">Default</option>
                    <option value="grade">Grade</option>
                    <option value="jlptlevel">JLPT Level</option>
                    <option value="frequency">Frequency</option>
                    <option value="heisig">Heisig</option>
                    <option value="heisig6">Heisig 6th Ed</option>
                </select>
            </div>

            {loading ? (
                <p>Loading...</p>
            ) : (
                <div>
                    {kanji.map(k => (
                        <div key={k.id} style={{ padding: '0.5rem', borderBottom: '1px solid #ccc' }}>
                            <span style={{ fontSize: '2rem', marginRight: '1rem' }}>{k.character}</span>
                            <span>{k.primaryMeaning}</span>
                        </div>
                    ))}
                </div>
            )}

            <div style={{ marginTop: '1rem' }}>
                <button
                    onClick={() => setPageIndex(p => Math.max(1, p - 1))}
                    disabled={pageIndex === 1 || loading}
                >
                    Previous
                </button>
                <span style={{ margin: '0 1rem' }}>Page {pageIndex} of {totalPages}</span>
                <button
                    onClick={() => setPageIndex(p => p + 1)}
                    disabled={pageIndex === totalPages || loading}
                >
                    Next
                </button>
            </div>
        </div>
    );
}

export default KanjiList;