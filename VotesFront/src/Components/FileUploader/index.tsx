import { useFile } from "../../hooks";
import { Loader } from "../loader";

export const FileUploader = () => {
    const { handleFile, file, limpiarArchivo, handleSubmit, error, isLoading } = useFile();

    return (
        <div className="">
            {/* Para seleccionar el archivo */}
            <div className="mb-6">
                <label className="block select-none text-white text-sm font-bold mb-4 border border-white border-dashed p-4 rounded-lg" htmlFor="file">
                    Click aquí para seleccionar un archivo valido
                </label>
                <input type="file" id="file" placeholder="selecciona un archivo" onChange={handleFile} className="hidden" />
            </div>

            {/* para presentar el archivo seleccionado */}
            <div className="text-white mb-6">
                <h1 className="font-bold select-none">Nombre del archivo</h1>
                <h3 className="text-sm text-gray-400">{file ? file.name : 'No se ha seleccionando un archivo'}</h3>
            </div>

            {/* btn */}
            <div className="flex flex-col gap-2 items-center justify-center">
                <button disabled={!file} onClick={handleSubmit} type="submit" className="bg-white hover:bg-gray-100 text-gray-900 font-bold py-2 px-4 rounded-full w-full disabled:bg-gray-200 disabled:opacity-70 disabled:cursor-not-allowed flex gap-3 items-center justify-center">
                    {isLoading && <Loader size={20} />}
                    Subir
                </button>

                <button type="button" disabled={!file} onClick={limpiarArchivo} className="text-gray-500 disabled:cursor-not-allowed font-bold">
                    Limpiar
                </button>
            </div>
            {/* errors */}
            <div className="my-6">
                {error && <p className="text-red-300">{error}</p>}
            </div>
        </div>
    )
}
