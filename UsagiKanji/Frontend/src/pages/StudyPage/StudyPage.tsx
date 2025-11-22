import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import KanjiInfo from "../../components/KanjiInfo/KanjiInfo";
import { kanjiApi } from "../../api/kanji";
import type { KanjiDetail } from "../../types/kanji";

const KanjiDetailPage: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const routerNavigate = useNavigate();
    const [kanji, setKanji] = useState<KanjiDetail | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const [prevExists, setPrevExists] = useState(false);
    const [nextExists, setNextExists] = useState(false);

    const sortBy = localStorage.getItem("kanji-sort-by") || "heisig6";

    const checkNeighbors = async (kanjiId: string) => {
        try {
            const [prev, next] = await Promise.all([
                kanjiApi.getNextOrPrev(kanjiId, "prev", sortBy).catch(() => null),
                kanjiApi.getNextOrPrev(kanjiId, "next", sortBy).catch(() => null),
            ]);
            setPrevExists(!!prev);
            setNextExists(!!next);
        } catch {
            setPrevExists(false);
            setNextExists(false);
        }
    };
    const loadKanji = async (kanjiId: string) => {
        setLoading(true);
        try {
            const data = await kanjiApi.getById(kanjiId);
            setKanji(data);
            setError(null);

            await checkNeighbors(kanjiId);
        } catch (err) {
            setError("Failed to load kanji");
            setPrevExists(false);
            setNextExists(false);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (!id) return;
        loadKanji(id);
    }, [id]);

    const handleSave = async (keyword: string, notes: string) => {
        if (!kanji) return;
        try {
            await kanjiApi.update(kanji.id, { keyword, notes });
            setKanji({ ...kanji, keyword, notes });
            alert("Saved successfully!");
        } catch {
            alert("Failed to save");
        }
    };

    const handleNavigate = async (direction: "prev" | "next") => {
        if (!kanji) return;

        try {
            const data = await kanjiApi.getNextOrPrev(kanji.id, direction, sortBy);
            if (data) {
                setKanji(data);
                routerNavigate(`/study/${data.id}`, { replace: true });
                await checkNeighbors(data.id);
            }
        } catch {
            if (direction === "prev") setPrevExists(false);
            if (direction === "next") setNextExists(false);
        }
    };

    const handleNavigateWrapper = async (direction: "prev" | "next") => {
        await handleNavigate(direction);
    };

    if (loading) return <p>Loading...</p>;
    if (error || !kanji) return <p>{error || "Kanji not found"}</p>;

    return (
        <KanjiInfo
            kanji={kanji}
            onSave={handleSave}
            onNavigate={handleNavigateWrapper}
            prevExists={prevExists}
            nextExists={nextExists}
        />
    );
};

export default KanjiDetailPage;
