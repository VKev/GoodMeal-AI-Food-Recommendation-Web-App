export interface DownloadFileProps {
    filePath: string;
    expiryHour?: number;
    private?: boolean;
  }
  
export interface SignedCookiesState {
    isLoading: boolean;
    error: string | null;
}
  