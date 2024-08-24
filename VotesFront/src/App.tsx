import { Summary } from './Components';
import { FileUploader } from './Components/FileUploader/index';

export function App() {

  return (
    <div className="h-screen flex gap-4">
      {/* Cotrollers */}
      <div className="bg-gray-950 p-5 col-auto">
        {/* App name */}
        <h1 className="font-semibold text-white mb-2 select-none">Verifica tus votos</h1>
        <div className="border-b border-gray-700 mb-6"></div>
        
        {/* File uploader */}
        <FileUploader />
      </div>
      {/* dashboard */}
      <div className="w-full flex justify-center items-center gap-4">
        <Summary />
      </div>
    </div>
  )
}
