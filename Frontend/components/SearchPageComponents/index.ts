// Index file for SearchPage components

// Main components
export { default as Sidebar } from './Sidebar';
export { default as SearchHeader } from './SearchHeader';
export { default as MainContent } from './MainContent';
export { default as InputArea } from './InputArea';

// InputArea sub-components
export { default as ImageUploadArea } from './ImageUploadArea';
export { default as MessageInput } from './MessageInput';
export { default as SendButton } from './SendButton';
export { default as InputAreaFooter } from './InputAreaFooter';

// Sidebar sub-components
export { default as SidebarHeader } from './SidebarHeader';
export { default as ChatHistory } from './ChatHistory';
export { default as UpgradeSection } from './UpgradeSection';

// Custom hooks
export { useImageUpload } from './hooks/useImageUpload';

// Types and interfaces
export type { ImageUploadHookReturn } from './hooks/useImageUpload';
export type { ChatItem, UploadedImage, SidebarProps, ChatHistoryProps, SidebarHeaderProps, InputAreaProps } from './types';
