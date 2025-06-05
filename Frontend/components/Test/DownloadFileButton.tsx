"use client";

import useFileDownload from "@/hooks/resources/useFileDownload";
import { DownloadFileProps } from "@/types/resources/download";

export default function DownloadFileButton({
  filePath,
  expiryHour = 1,
  private: isPrivate = false,
}: DownloadFileProps) {
  const { handleDownload, isDownloading, isLoading, error, cookieError } = useFileDownload(filePath, expiryHour, isPrivate);

  return (
    <div>
      <button
        onClick={handleDownload}
        disabled={isDownloading || isLoading}
        className="px-4 py-2 bg-blue-500 text-white rounded-lg disabled:opacity-50"
      >
        {isDownloading
          ? "Starting Download..."
          : isLoading
          ? "Loading Cookies..."
          : "Download File"}
      </button>
      {error && <p className="text-red-500 mt-2">{error}</p>}
      {cookieError && (
        <p className="text-red-500 mt-2">Cookie Error: {cookieError}</p>
      )}
    </div>
  );
}
