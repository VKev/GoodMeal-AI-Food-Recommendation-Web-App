import React, { useState } from 'react';
import {
    Layout,
    Button,
    theme,
    ConfigProvider
} from 'antd';
import {
    MenuOutlined
} from '@ant-design/icons';

// Import components
import Sidebar from './Sidebar';
import SearchHeader from './SearchHeader';
import MainContent from './MainContent';
import InputArea from './InputArea';


const SearchPage = () => {
    const [selectedChat, setSelectedChat] = useState<number | null>(null);
    const [message, setMessage] = useState('');
    const [collapsed, setCollapsed] = useState(false);

    const chatHistory = [
        { id: 1, title: 'Thiết kế cơ sở dữ liệu', preview: 'Tạo schema MongoDB cho hệ thống...', time: 'Hôm qua' },
        { id: 2, title: 'API Authentication', preview: 'Implement JWT token...', time: 'Hôm qua' },
        { id: 3, title: 'React Hooks', preview: 'Giải thích useEffect và useState...', time: '2 ngày trước' },
    ];    return (
        <ConfigProvider
            theme={{
                algorithm: theme.darkAlgorithm,
                token: {
                    colorPrimary: '#ff7a00',
                    colorBgContainer: '#1f1f23',
                    colorBgElevated: '#262629',
                    colorBorder: '#ff7a0033',
                    colorText: '#ffffff',
                    colorTextSecondary: '#b3b3b3',
                },
            }}
        >
            <Layout style={{ height: '100vh', maxHeight: '100vh', overflow: 'hidden', background: 'linear-gradient(135deg, #1a1a1d 0%, #16161a 50%, #0f0f12 100%)' }}>
                {/* Sidebar */}
                <Sidebar
                    collapsed={collapsed}
                    setCollapsed={setCollapsed}
                    selectedChat={selectedChat}
                    setSelectedChat={setSelectedChat}
                    chatHistory={chatHistory}
                />
                
                {/* Main Content */}
                <Layout style={{ background: 'linear-gradient(180deg, #1a1a1d 0%, #0f0f12 100%)', height: '100vh', maxHeight: '100vh', overflow: 'hidden', display: 'flex', flexDirection: 'column' }}>
                    {/* Floating Toggle Button when sidebar is collapsed - Updated styling */}
                    {collapsed && (
                        <Button
                            type="text"
                            shape="circle"
                            icon={<MenuOutlined />}
                            onClick={() => setCollapsed(false)}
                            style={{
                                position: 'absolute',
                                top: '16px',
                                left: '16px',
                                zIndex: 1000,
                                backgroundColor: 'transparent',
                                border: 'none',
                                color: '#ffffff',
                                fontSize: '18px',
                                width: '40px',
                                height: '40px',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                boxShadow: 'none'
                            }}
                        />
                    )}
                    
                    {/* Header */}
                    <SearchHeader collapsed={collapsed} />
                    
                    {/* Content Area */}
                    <MainContent selectedChat={selectedChat} />
                    
                    {/* Input Area */}
                    <InputArea message={message} setMessage={setMessage} />
                </Layout>
            </Layout>
        </ConfigProvider>
    );
};

export default SearchPage;