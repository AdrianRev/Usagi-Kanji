import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import KanjiInfo from "../../components/KanjiInfo/KanjiInfo";
import { kanjiApi } from "../../api/kanji";
import type { KanjiDetail } from "../../types/kanji";

const KanjiDetailPage: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const [kanji, setKanji] = useState<KanjiDetail | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!id) return;
        setLoading(true);
        kanjiApi.getById(id)
            .then(data => setKanji(data))
            .catch(err => setError("Failed to load kanji"))
            .finally(() => setLoading(false));
    }, [id]);

    const handleSave = async (keyword: string, notes: string) => {
        if (!kanji) return;
        try {
            await kanjiApi.update(kanji.id, { keyword, notes });
            setKanji({ ...kanji, keyword, notes }); // update local state
            alert("Saved successfully!");
        } catch {
            alert("Failed to save");
        }
    };

    if (loading) return <p>Loading...</p>;
    if (error || !kanji) return <p>{error || "Kanji not found"}</p>;

    return <KanjiInfo kanji={kanji} onSave={handleSave} />;
};

export default KanjiDetailPage;
