"use client";

import { useState } from "react";
import { SignedCookiesState } from "@/types/resources/download";
import { useSignedCookies } from "@/hooks/resources/useSignedCookies";

export default function useFileDownload(filePath: string, expiryHour: number, isPrivate: boolean) {
  const [isDownloading, setIsDownloading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const cloudFrontBaseUrl = process.env.NEXT_PUBLIC_CLOUDFRONT_BASE_URL || "";

  const { isLoading, error: cookieError }: SignedCookiesState = useSignedCookies(`/*`, expiryHour);

  const handleDownload = async () => {
    if (isLoading) return;
    setIsDownloading(true);
    setError(null);

    try {
      const fileUrl = `${cloudFrontBaseUrl}/${filePath}`;

      const headResponse = await fetch(fileUrl, {
        method: "HEAD",
        credentials: isPrivate ? "include" : "omit",
      });

      if (!headResponse.ok) {
        throw new Error("Unauthorized or file not found");
      }

      const contentLength = headResponse.headers.get("content-length");
      const fileSize = contentLength ? parseInt(contentLength, 10) : 0;
      const fileName = filePath.split("/").pop() || "downloaded-file";

      if (fileSize > 5 * 1024 * 1024) {
        // Large file: Use iframe approach
        const iframe = document.createElement("iframe");
        iframe.style.display = "none";
        iframe.src = fileUrl;
        document.body.appendChild(iframe);

        setTimeout(() => {
          document.body.removeChild(iframe);
        }, 5000);
      } else {
        // Small file: Fetch and download using blob
        const response = await fetch(fileUrl, {
          method: "GET",
          credentials: isPrivate ? "include" : "omit",
        });

        if (!response.ok) {
          throw new Error("Failed to fetch the file.");
        }

        const blob = await response.blob();
        const blobUrl = window.URL.createObjectURL(blob);
        const link = document.createElement("a");
        link.href = blobUrl;
        link.download = fileName;
        document.body.appendChild(link);
        link.click();

        // Cleanup
        document.body.removeChild(link);
        window.URL.revokeObjectURL(blobUrl);
      }
    } catch (err) {
      console.error("Download error:", err);
      setError("Failed to download file. Check your access.");
    } finally {
      setIsDownloading(false);
    }
  };

  return { handleDownload, isDownloading, isLoading, error, cookieError };
}
