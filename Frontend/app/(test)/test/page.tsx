"use client";

import { useState } from "react";
import DownloadFileButton from "@/components/Test/DownloadFileButton";
import UploadFileButton from "@/components/Test/UploadFileButton";
import GetSharedUrlButton from "@/components/Test/GetSharedUrlButton";
 
export default function Test() {
  const [fileName, setFileName] = useState("");

  return (
    <div className="p-6 max-w-xl mx-auto space-y-6">
      Download Private File
      <div>
        <h1 className="text-lg font-semibold">Download Private File</h1>
        <DownloadFileButton filePath="private/100MB.zip" private={true} />
      </div>

      <div>
        <h1 className="text-lg font-semibold">Download Public File</h1>
        <DownloadFileButton filePath="public/100MB.zip" private={false} />
      </div>

      <div>
        <h1 className="text-lg font-semibold">Upload File</h1>
        <UploadFileButton filePath="private/" />
      </div>

      <div>
        <h1 className="text-lg font-semibold">Get Shared URL</h1>
        <input
          type="text"
          placeholder="Enter file name..."
          value={fileName}
          onChange={(e) => setFileName(e.target.value)}
          className="border px-4 py-2 rounded-lg w-full"
        />
        <GetSharedUrlButton fileName={fileName} expiryHour={12} />
      </div>
    </div>
  );
}
