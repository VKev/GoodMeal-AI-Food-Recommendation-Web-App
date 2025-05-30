"use client";

import { useState } from "react";
import useMultipartUpload from "@/hooks/resources/useMultipartUpload";
import { UploadFileProps } from "@/types/resources/upload";

export default function UploadFileButton({ filePath, maxChunkSize = 5 * 1024 * 1024 }: UploadFileProps) {
  const { handleUpload, isUploading, uploadProgress, error, successMessage } = useMultipartUpload();
  const [selectedFile, setSelectedFile] = useState<File | null>(null);

  const onFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      setSelectedFile(file);
    }
  };

  const onUploadClick = () => {
    if (selectedFile) {
      handleUpload(selectedFile, filePath, maxChunkSize);
    }
  };

  return (
    <div className="space-y-4">
      <input
        type="file"
        onChange={onFileChange}
        disabled={isUploading}
        className="block w-full text-sm text-gray-500 file:mr-4 file:py-2 file:px-4 
                 file:rounded-lg file:border-0 file:text-sm file:font-semibold 
                 file:bg-blue-500 file:text-white disabled:opacity-50"
      />
      <button
        onClick={onUploadClick}
        disabled={isUploading || !selectedFile}
        className="px-4 py-2 bg-green-500 text-white rounded-lg disabled:opacity-50"
      >
        {isUploading ? "Uploading..." : "Upload File"}
      </button>

      {uploadProgress && (
        <div className="w-full bg-gray-200 rounded-full h-2.5">
          <div 
            className="bg-blue-500 h-2.5 rounded-full transition-all duration-300"
            style={{ width: `${uploadProgress.percentage}%` }}
          />
          <p className="text-sm text-gray-600 mt-1">
            Uploaded {Math.round(uploadProgress.loaded / (1024 * 1024))}MB 
            of {Math.round(uploadProgress.total / (1024 * 1024))}MB 
            ({uploadProgress.percentage}%)
          </p>
        </div>
      )}

      {error && <p className="text-red-500">{error}</p>}
      {successMessage && <p className="text-green-500">{successMessage}</p>}
    </div>
  );
}
