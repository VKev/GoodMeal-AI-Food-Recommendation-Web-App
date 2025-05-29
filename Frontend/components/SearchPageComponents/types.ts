// Shared types for SearchPage components

export interface ChatItem {
    id: number;
    title: string;
    preview: string;
    time: string;
}

export interface UploadedImage {
    uid: string;
    name: string;
    url: string;
}

export interface SidebarProps {
    collapsed: boolean;
    setCollapsed: (collapsed: boolean) => void;
    selectedChat: number | null;
    setSelectedChat: (chatId: number | null) => void;
    chatHistory: ChatItem[];
}

export interface ChatHistoryProps {
    chatHistory: ChatItem[];
    selectedChat: number | null;
    setSelectedChat: (chatId: number | null) => void;
}

export interface SidebarHeaderProps {
    collapsed: boolean;
    setCollapsed: (collapsed: boolean) => void;
    searchMode: boolean;
    setSearchMode: (mode: boolean) => void;
    setSelectedChat: (chatId: number | null) => void;
}

export interface InputAreaProps {
    inputMessage: string;
    setInputMessage: (message: string) => void;
}
