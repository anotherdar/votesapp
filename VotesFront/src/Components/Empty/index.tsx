export const Indicator: React.FC<IndicatorProps> = ({ text }) => {
    return <div className="flex gap-4 items-center justify-center">
        <h1>{text || 'No hay resultados para mostrar.'}</h1>
    </div>
}

export interface IndicatorProps {
    text?: string;
}