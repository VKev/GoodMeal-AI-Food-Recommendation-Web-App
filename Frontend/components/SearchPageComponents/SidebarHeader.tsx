import React from 'react';
import { Button, Input, Popconfirm } from 'antd';
import { SearchOutlined, PlusOutlined, MenuOutlined, CloseOutlined, DeleteOutlined } from '@ant-design/icons';
import { SidebarHeaderProps } from './types';

const SidebarHeader: React.FC<SidebarHeaderProps> = ({
    collapsed,
    setCollapsed,
    searchMode,
    setSearchMode,
    onCreateSession,
    isCreatingSession,
    onDeleteAllSessions,
    chatHistoryCount = 0
}) => {
    return (
        <div style={{ 
            padding: '16px', 
            flexShrink: 0,
            borderBottom: '1px solid rgba(255, 122, 0, 0.1)'
        }}>
            {searchMode ? (
                // Search mode - full width input
                <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                    <Input
                        placeholder="Search conversations..."
                        prefix={<SearchOutlined style={{ color: '#b3b3b3' }} />}
                        style={{
                            background: 'rgba(128, 128, 128, 0.1)',
                            border: '1px solid rgba(128, 128, 128, 0.2)',
                            borderRadius: '8px',
                            height: '40px',
                            flex: 1
                        }}
                        autoFocus
                        onBlur={() => setSearchMode(false)}
                        onPressEnter={() => setSearchMode(false)}
                    />
                    <Button
                        type="text"
                        icon={<CloseOutlined />}
                        onClick={() => setSearchMode(false)}
                        style={{ color: '#b3b3b3', flexShrink: 0 }}
                    />
                </div>
            ) : (
                // Normal mode - buttons row
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <Button
                        type="text"
                        icon={collapsed ? <MenuOutlined /> : <CloseOutlined />}
                        onClick={() => setCollapsed(!collapsed)}
                        style={{ color: '#b3b3b3' }}
                    />
                    <div style={{ display: 'flex', gap: '8px' }}>
                        <Button
                            type="text"
                            icon={<SearchOutlined />}
                            onClick={() => setSearchMode(true)}
                            style={{ color: '#b3b3b3' }}
                        />
                        {chatHistoryCount > 0 && onDeleteAllSessions && (
                            <Popconfirm
                                title="Xóa tất cả cuộc trò chuyện"
                                description={`Bạn có chắc chắn muốn xóa tất cả ${chatHistoryCount} cuộc trò chuyện?`}
                                onConfirm={onDeleteAllSessions}
                                okText="Xóa tất cả"
                                cancelText="Hủy"
                                placement="bottom"
                            >
                                <Button
                                    type="text"
                                    icon={<DeleteOutlined />}
                                    style={{ 
                                        color: '#ff4d4f',
                                        fontSize: '14px'
                                    }}
                                    title="Xóa tất cả cuộc trò chuyện"
                                />
                            </Popconfirm>
                        )}
                        <Button
                            type="text"
                            icon={<PlusOutlined />}
                            onClick={onCreateSession}
                            loading={isCreatingSession}
                            disabled={isCreatingSession}
                            className={`action-button ${isCreatingSession ? 'creating-session' : ''}`}
                            style={{ 
                                color: isCreatingSession ? '#ff7a00' : '#b3b3b3'
                            }}
                        />
                    </div>
                </div>
            )}
        </div>
    );
};

export default SidebarHeader;
