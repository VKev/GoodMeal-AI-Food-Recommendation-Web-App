export interface UploadFileProps {
  filePath: string;
  maxChunkSize?: number;
}

export interface UploadProgress {
  loaded: number;
  total: number;
  percentage: number;
}
