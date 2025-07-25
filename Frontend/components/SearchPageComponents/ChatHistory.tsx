import React from 'react';
import { Card, Typography, Flex, Button, Popconfirm } from 'antd';
import { DeleteOutlined } from '@ant-design/icons';
import { ChatHistoryProps } from './types';

const { Text } = Typography;

const ChatHistory: React.FC<ChatHistoryProps> = ({
    chatHistory,
    selectedChat,
    setSelectedChat,
    onDeleteSession
}) => {

    const handleChatSelect = (chatId: string) => {
        setSelectedChat(chatId);
        // Update URL without page reload for smooth transition
        window.history.pushState({}, '', `/c/${chatId}`);
    };

    return (
        <div style={{
            flex: 1,
            overflow: 'hidden',
            padding: '16px 16px 0 16px',
            display: 'flex',
            flexDirection: 'column',
            minHeight: 0
        }}>            <Text
                type="secondary"
                style={{
                    fontSize: '16px',
                    fontWeight: 'medium',
                    padding: '0 8px',
                    display: 'block',
                    marginBottom: '16px',
                    marginTop: '8px',
                    flexShrink: 0
                }}
            >
                Lịch sử trò chuyện
            </Text>
            <div style={{
                flex: 1,
                overflowY: 'auto',
                overflowX: 'hidden',
                paddingRight: '8px',
                minHeight: 0,
                scrollbarWidth: 'thin',
                scrollbarColor: 'rgba(255, 122, 0, 0.3) transparent'
            }}>                {chatHistory.length === 0 ? (
                    <div style={{
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        height: '100%',
                        textAlign: 'center',
                        padding: '40px 20px',
                        color: '#666',
                        fontSize: '14px'
                    }}>
                        Chưa có lịch sử trò chuyện
                    </div>
                ) : (
                    chatHistory.map((chat) => (
                    <div key={chat.id} style={{ marginBottom: '8px' }}>
                        <Card
                            hoverable
                            size="small"
                            className={`chat-item ${selectedChat === chat.id ? 'selected' : ''} session-entering`}
                            onClick={() => handleChatSelect(chat.id)}
                            style={{
                                width: '100%',
                                background: selectedChat === chat.id
                                    ? 'rgba(255, 122, 0, 0.1)'
                                    : 'transparent',
                                border: selectedChat === chat.id
                                    ? '1px solid rgba(255, 122, 0, 0.2)'
                                    : '1px solid transparent',
                                borderRadius: '8px',
                                boxShadow: selectedChat === chat.id
                                    ? '0 4px 15px rgba(255, 122, 0, 0.1)'
                                    : 'none',
                                cursor: 'pointer'
                            }}
                            styles={{ body: { padding: '12px' } }}
                        >
                            <Flex justify="space-between" align="flex-start">
                                <div 
                                    style={{ flex: 1, minWidth: 0 }}
                                    onClick={() => handleChatSelect(chat.id)}
                                >
                                    <Text
                                        strong
                                        style={{
                                            color: '#ffffff',
                                            fontSize: '14px',
                                            display: 'block',
                                            marginBottom: '4px'
                                        }}
                                        ellipsis
                                    >
                                        {chat.title}
                                    </Text>
                                </div>
                                <div style={{ display: 'flex', alignItems: 'center', gap: '4px', flexShrink: 0 }}>
                                    <Text type="secondary" style={{ fontSize: '12px' }}>
                                        {chat.time}
                                    </Text>
                                    {onDeleteSession && (
                                        <Popconfirm
                                            title="Xóa cuộc trò chuyện"
                                            description="Bạn có chắc chắn muốn xóa cuộc trò chuyện này?"
                                            onConfirm={(e) => {
                                                e?.stopPropagation();
                                                onDeleteSession(chat.id);
                                            }}
                                            okText="Xóa"
                                            cancelText="Hủy"
                                            placement="left"
                                        >
                                            <Button
                                                type="text"
                                                size="small"
                                                icon={<DeleteOutlined />}
                                                onClick={(e) => e.stopPropagation()}
                                                style={{
                                                    color: '#ff4d4f',
                                                    opacity: 0.7,
                                                    transition: 'opacity 0.2s'
                                                }}
                                                onMouseEnter={(e) => {
                                                    e.currentTarget.style.opacity = '1';
                                                }}
                                                onMouseLeave={(e) => {
                                                    e.currentTarget.style.opacity = '0.7';
                                                }}
                                            />
                                        </Popconfirm>
                                    )}
                                </div>
                            </Flex>
                        </Card>
                    </div>
                    ))
                )}
            </div>
        </div>
    );
};

export default ChatHistory;
