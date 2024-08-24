import { useVotesInfo } from "../../store"
import { Indicator } from "../Empty";
import { IoFemaleOutline, IoMale } from "react-icons/io5";
import { MdVerified } from "react-icons/md";

export const Summary: React.FC = () => {
    const { votesInfo } = useVotesInfo();
    if (!votesInfo.porcentajes) {
        return <Indicator />
    }

    return (
        <div className="w-full h-full p-16 flex flex-col gap-4">
            <h1 className="text-xl font-bold text-gray-700">Resultados de la Votación</h1>

            <div className="border rounded-lg border-gray-200 w-fit p-4">
                <h3 className="text-gray-700 font-semibold">Total de votos emitidos</h3>
                <p className="text-4xl font-bold text-green-700">{votesInfo?.totalDeVotaciones}</p>
            </div>

            <div>
                <h2 className="mb-4 font-semibold text-gray-700">
                    Votos por género
                </h2>
                <div className="flex gap-4">
                    <div className="border rounded-lg border-gray-200 w-fit p-4">
                        <h3 className="text-gray-700 font-semibold">Hombres</h3>
                        <div className="flex gap-2 items-center">
                            <IoMale size={24} className="text-blue-700" />
                            <p className="text-4xl font-bold text-blue-700">{votesInfo?.votosDeHombres}</p>
                        </div>
                    </div>
                    <div className="border rounded-lg border-gray-200 w-fit p-4">
                        <h3 className="text-gray-700 font-semibold">Mujeres</h3>
                        <div className="flex gap-2 items-center">
                            <IoFemaleOutline size={24} className="text-pink-700" />
                            <p className="text-4xl font-bold text-pink-700">{votesInfo?.votosDeMujer}</p>
                        </div>
                    </div>
                </div>
            </div>

            {/*  */}
            <div>
                <h2 className="mb-4 font-semibold text-gray-700">
                    Candidato con mayor votación
                </h2>
                <div className="flex gap-2 items-center p-2 pr-4 border border-green-400 bg-green-200 w-fit rounded-lg">
                    <MdVerified size={24} className="text-green-500" />
                    <p className="font-bold text-green-600">{votesInfo?.mejorCandidato}</p>
                </div>
            </div>
            {/*  */}
            <h2 className="font-semibold text-gray-700 mt-2">Porcentajes de votos por candidato:</h2>
            {/*  */}
            <ul className="flex gap-4 list-none flex-wrap">
                {(votesInfo?.porcentajes || []).map((candidate) => (
                    <li key={candidate.name} className="border border-gray-200 rounded-lg p-4 w-1/5">
                        <div className="flex items-center gap-2">
                            <MdVerified size={18} className="text-gray-500" />
                            <h4 className="text-gray-700 font-bold text-sm text-ellipsis">{candidate.name}</h4>
                        </div>
                        <p className="text-gray-600">{candidate.value}%</p>
                    </li>
                ))}
            </ul>
        </div>
    )
}
