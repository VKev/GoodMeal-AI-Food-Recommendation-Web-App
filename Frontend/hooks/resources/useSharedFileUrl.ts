"use client"
import { useState } from "react";

export const useSharedFileUrl = () => {
  const [message, setMessage] = useState<string>("");
  const [sharedUrl, setSharedUrl] = useState<string | null>(null);

  const getSharedUrl = async (fileName: string, expiryMinutes: number) => {
    if (!fileName) {
      setMessage("Please enter a file name.");
      setSharedUrl(null);
      return;
    }

    if (!expiryMinutes || expiryMinutes <= 0) {
      setMessage("Please enter a valid expiry duration.");
      setSharedUrl(null);
      return;
    }

    try {
      const response = await fetch("/api/resource/get-presigned-url", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          bucketName: process.env.NEXT_PUBLIC_WASABI_BUCKET_NAME,
          objectKey: `${fileName}`,
          expiryDurationMinutes: expiryMinutes,
          httpVerb: "GET",
        }),
      });

      if (!response.ok) {
        throw new Error("Failed to get shared URL.");
      }

      const { presignedUrl } = await response.json();
      setSharedUrl(presignedUrl);
      setMessage("Shared URL generated successfully!");
    } catch (error) {
      setSharedUrl(null);
      if (error instanceof Error) {
        setMessage(`Error: ${error.message}`);
      } else {
        setMessage("An unknown error occurred.");
      }
    }
  };

  return { getSharedUrl, sharedUrl, message };
};
