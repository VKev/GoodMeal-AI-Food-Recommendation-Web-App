// Location prompt management utilities

const LOCATION_PROMPT_STATUS_KEY = 'goodmeal_location_prompt_status';
const LOCATION_PROMPT_TIMESTAMP_KEY = 'goodmeal_location_prompt_timestamp';
const ONCE_DENIED_DURATION = 24 * 60 * 60 * 1000; // 24 hours in milliseconds

export type LocationPromptStatus = 'always_granted' | 'always_denied' | 'once_denied' | null;

export const locationPromptUtils = {
  
  // Get current location prompt status
  getStatus(): LocationPromptStatus {
    const status = localStorage.getItem(LOCATION_PROMPT_STATUS_KEY) as LocationPromptStatus;
    
    // Check if "once_denied" has expired
    if (status === 'once_denied') {
      const timestamp = localStorage.getItem(LOCATION_PROMPT_TIMESTAMP_KEY);
      if (timestamp) {
        const deniedTime = parseInt(timestamp, 10);
        const now = Date.now();
        
        // If more than 24 hours have passed, reset the status
        if (now - deniedTime > ONCE_DENIED_DURATION) {
          this.clearStatus();
          return null;
        }
      }
    }
    
    return status;
  },
  
  // Set location prompt status
  setStatus(status: LocationPromptStatus): void {
    if (status === null) {
      this.clearStatus();
      return;
    }
    
    localStorage.setItem(LOCATION_PROMPT_STATUS_KEY, status);
    
    // Save timestamp for "once_denied" to track expiration
    if (status === 'once_denied') {
      localStorage.setItem(LOCATION_PROMPT_TIMESTAMP_KEY, Date.now().toString());
    }
  },
  
  // Clear location prompt status
  clearStatus(): void {
    localStorage.removeItem(LOCATION_PROMPT_STATUS_KEY);
    localStorage.removeItem(LOCATION_PROMPT_TIMESTAMP_KEY);
  },
  
  // Check if should show prompt
  shouldShowPrompt(hasLocation: boolean, permission: string): boolean {
    const status = this.getStatus();
    
    return !hasLocation && 
           status !== 'always_denied' && 
           status !== 'always_granted' &&
           permission !== 'denied';
  },
  
  // Reset "once" choices on logout but keep "always" choices
  handleLogout(): void {
    const status = this.getStatus();
    if (status === 'once_denied') {
      this.clearStatus();
    }
  }
};

export default locationPromptUtils;
