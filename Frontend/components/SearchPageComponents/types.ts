// Shared types for SearchPage components

export interface ChatItem {
    id: string;
    title: string;
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
    selectedChat: string | null;
    setSelectedChat: (chatId: string | null) => void;
    chatHistory: ChatItem[];
    onCreateSession: () => void;
    isCreatingSession?: boolean;
    onDeleteSession?: (sessionId: string) => void;
}

export interface ChatHistoryProps {
    chatHistory: ChatItem[];
    selectedChat: string | null;
    setSelectedChat: (chatId: string | null) => void;
    onDeleteSession?: (sessionId: string) => void;
}

export interface SidebarHeaderProps {
    collapsed: boolean;
    setCollapsed: (collapsed: boolean) => void;
    searchMode: boolean;
    setSearchMode: (mode: boolean) => void;
    setSelectedChat: (chatId: string | null) => void;
    onCreateSession: () => void;
    isCreatingSession?: boolean;
    currentSessionId?: string;
    onSendMessage?: (message: string, sessionId: string) => void;
}

export interface InputAreaProps {
    inputMessage: string;
    setInputMessage: (message: string) => void;
}
