"use client";
import { useState } from "react";
import { useSharedFileUrl } from "@/hooks/resources/useSharedFileUrl";
import { GetSharedUrlButtonProps } from "@/types/resources/shared";


export default function GetSharedUrlButton({ fileName, expiryHour }: GetSharedUrlButtonProps) {
  const { getSharedUrl, sharedUrl, message } = useSharedFileUrl();
  const [isLoading, setIsLoading] = useState(false);

  const handleGetUrl = async () => {
    if (!fileName.trim()) {
      alert("Please enter a valid filename.");
      return;
    }
    if (expiryHour <= 0) {
      alert("Expiry time must be greater than 0 hours.");
      return;
    }

    setIsLoading(true);
    const expiryMinutes = expiryHour * 60; // Convert hours to minutes
    await getSharedUrl(fileName, expiryMinutes);
    setIsLoading(false);
  };

  return (
    <div className="flex flex-col gap-4 p-4 border rounded-lg max-w-md mx-auto">
      <button
        onClick={handleGetUrl}
        disabled={isLoading || !fileName.trim() || expiryHour <= 0}
        className="px-4 py-2 bg-blue-500 text-white rounded-lg disabled:opacity-50"
      >
        {isLoading ? "Generating URL..." : "Get Shared URL"}
      </button>

      {sharedUrl && (
        <div className="mt-2">
          <label className="font-medium">Shared URL</label>
          <input
            type="text"
            value={sharedUrl}
            readOnly
            className="border px-4 py-2 rounded-lg w-full text-gray-700"
          />
          <p className="text-sm text-gray-500 mt-1">
            Copy and share this URL before it expires.
          </p>
        </div>
      )}

      {message && <p className="text-green-500 mt-2">{message}</p>}
    </div>
  );
}