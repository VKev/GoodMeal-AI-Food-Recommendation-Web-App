import React, { useState } from 'react';
import { Layout } from 'antd';
import SidebarHeader from './SidebarHeader';
import ChatHistory from './ChatHistory';
import UpgradeSection from './UpgradeSection';
import { SidebarProps } from './types';

const { Sider } = Layout;

const Sidebar: React.FC<SidebarProps> = ({
    collapsed,
    setCollapsed,
    selectedChat,
    setSelectedChat,
    chatHistory,
    onCreateSession,
    isCreatingSession,
    onDeleteSession,
    onDeleteAllSessions
}) => {
    const [searchMode, setSearchMode] = useState(false);

    return (
        <Sider
            width={320}
            collapsed={collapsed}
            onCollapse={setCollapsed}
            collapsedWidth={0}
            style={{
                background: 'linear-gradient(180deg, #1a1a1d 0%, #0f0f12 100%)',
                borderRight: '1px solid #ff7a0033',
                boxShadow: '4px 0 20px rgba(0, 0, 0, 0.3)',
                height: '100vh',
                minHeight: 0,
                overflow: 'hidden'
            }}
            breakpoint="lg"
        >
            <div style={{
                height: '100%', 
                display: 'flex', 
                flexDirection: 'column',
                minHeight: 0
            }}>
                <SidebarHeader
                    collapsed={collapsed}
                    setCollapsed={setCollapsed}
                    searchMode={searchMode}
                    setSearchMode={setSearchMode}
                    setSelectedChat={setSelectedChat}
                    onCreateSession={onCreateSession}
                    isCreatingSession={isCreatingSession}
                    onDeleteAllSessions={onDeleteAllSessions}
                    chatHistoryCount={chatHistory.length}
                />

                <div style={{ 
                    flex: 1, 
                    display: 'flex', 
                    flexDirection: 'column', 
                    minHeight: 0,
                    overflow: 'hidden'
                }}>
                    <ChatHistory
                        chatHistory={chatHistory}
                        selectedChat={selectedChat}
                        setSelectedChat={setSelectedChat}
                        onDeleteSession={onDeleteSession}
                    />
                </div>

                <UpgradeSection />
            </div>
        </Sider>
    );
};

export default Sidebar;
