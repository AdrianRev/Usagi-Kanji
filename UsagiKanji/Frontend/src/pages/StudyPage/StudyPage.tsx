import React, { useState, useEffect } from "react";
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

    const sortBy = localStorage.getItem("kanji-sort-by") || "heisig6";

    const loadKanji = async (kanjiId: string) => {
        setLoading(true);
        try {
            const data = await kanjiApi.getById(kanjiId);
            setKanji(data);
            setError(null);
        } catch {
            setError("Failed to load kanji");
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
            } else {
                alert(`No ${direction === "next" ? "next" : "previous"} kanji`);
            }
        } catch {
            alert("Failed to load kanji");
        }
    };

    if (loading) return <p>Loading...</p>;
    if (error || !kanji) return <p>{error || "Kanji not found"}</p>;

    return (
        <KanjiInfo
            kanji={kanji}
            onSave={handleSave}
            onNavigate={handleNavigate}
        />
    );
};

export default KanjiDetailPage;
