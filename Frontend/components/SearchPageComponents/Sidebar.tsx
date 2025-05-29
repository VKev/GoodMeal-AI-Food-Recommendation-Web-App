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
    chatHistory
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
                display: 'flex',
                flexDirection: 'column',
                height: '100vh'
            }}
            breakpoint="lg"
        >
            <SidebarHeader
                collapsed={collapsed}
                setCollapsed={setCollapsed}
                searchMode={searchMode}
                setSearchMode={setSearchMode}
                setSelectedChat={setSelectedChat}
            />

            <ChatHistory
                chatHistory={chatHistory}
                selectedChat={selectedChat}
                setSelectedChat={setSelectedChat}
            />

            <UpgradeSection />
        </Sider>
    );
};

export default Sidebar;
