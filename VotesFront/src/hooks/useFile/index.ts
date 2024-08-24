import axios from "axios";
import { useState } from "react";
import { useVotesInfo } from "../../store";

export function useFile() {
    const [file, setFile] = useState<File | null>(null);
    const {setVoteInfo} = useVotesInfo();
    const [error, setError] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(false)

    function handleFile(event: React.ChangeEvent<HTMLInputElement>) {
        event.stopPropagation();

        setError(null);

        const { target: { files } } = (event || { target: { files: [] } });

        const localFile = files?.length ? files[0] : null

        setFile(localFile)
    };

    function limpiarArchivo() {
        setError(null);
        setFile(null);
        setVoteInfo({} as VotesInfo);
    }

    const handleSubmit = async () => {
        setError(null);

        if (!file) {
            setError('Por favor, selecciona un archivo.');
            return;
        }

        const formData = new FormData();
        formData.append('file', file);

        setIsLoading(true);

        try {
            const response = await axios.post(import.meta.env.VITE_API_URL + '/votes/upload', formData, {
                    headers: {
                        'Content-Type': 'multipart/form-data',
                    },
                });

            const data = response.data as VotesInfo;
            setVoteInfo(data);
            setError(null);
        } catch (err) {
            console.error(err)
            setError('Error en la subida del archivo');
        } finally {
            setIsLoading(false);
        }
    };


    return {
        handleFile,
        limpiarArchivo,
        file,
        handleSubmit,
        error,
        isLoading
    }
}