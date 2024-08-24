export const Loader: React.FC<LoaderProps> = ({ size = 24 }) => {
    return <div style={{ width: size, height: size }} className={`animate-spin rounded-full border-gray-500 border-l-gray-800 border-4`}></div>
}

export interface LoaderProps {
    size?: number
}