import React from 'react';
import {
    Layout,
    Menu,
    Button,
    Input,
    Card,
    Avatar,
    List,
    Typography,
    Flex
} from 'antd';
import {
    MessageOutlined,
    SearchOutlined,
    PlusOutlined,
    MenuOutlined,
    CloseOutlined,
    CrownOutlined,
    StarOutlined
} from '@ant-design/icons';

const { Sider } = Layout;
const { Title, Text, Paragraph } = Typography;

interface ChatItem {
    id: number;
    title: string;
    preview: string;
    time: string;
}

interface SidebarProps {
    collapsed: boolean;
    setCollapsed: (collapsed: boolean) => void;
    selectedChat: number | null;
    setSelectedChat: (chatId: number | null) => void;
    chatHistory: ChatItem[];
}

const Sidebar: React.FC<SidebarProps> = ({
    collapsed,
    setCollapsed,
    selectedChat,
    setSelectedChat,
    chatHistory
}) => {
    const menuItems = [
        {
            key: '1',
            icon: <MessageOutlined />,
            label: 'Tất cả cuộc trò chuyện',
        }
    ];

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
            }}
            breakpoint="lg"
        >
            {/* Header */}
            <div style={{
                padding: '24px',
                borderBottom: '1px solid #ff7a0033',
                background: 'linear-gradient(90deg, #ff7a001a 0%, #ff7a000d 100%)'
            }}>
                <Flex justify="space-between" align="center" style={{ marginBottom: '16px' }}>
                    <Flex align="center" gap={12}>
                        <Avatar
                            size={32}
                            style={{
                                background: 'linear-gradient(45deg, #ff7a00 0%, #ff9500 100%)',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center'
                            }}
                            icon={<StarOutlined />}
                        />
                        <Title
                            level={4}
                            style={{
                                margin: 0,
                                background: 'linear-gradient(45deg, #ff9500 0%, #ff7a00 100%)',
                                backgroundClip: 'text',
                                WebkitBackgroundClip: 'text',
                                color: 'transparent',
                                fontWeight: 'bold'
                            }}
                        >
                            GoodMeal
                        </Title>
                    </Flex>
                    <Button
                        type="text"
                        icon={collapsed ? <MenuOutlined /> : <CloseOutlined />}
                        onClick={() => setCollapsed(!collapsed)}
                        style={{ color: '#b3b3b3' }}
                    />
                </Flex>

                <Button
                    type="primary"
                    size="large"
                    block
                    icon={<PlusOutlined />}
                    style={{
                        background: 'linear-gradient(45deg, #ff7a00 0%, #ff9500 100%)',
                        border: 'none',
                        borderRadius: '12px',
                        height: '48px',
                        fontWeight: 'medium',
                        boxShadow: '0 4px 15px rgba(255, 122, 0, 0.25)'
                    }}
                >
                    Cuộc trò chuyện mới
                </Button>
            </div>

            {/* Search */}
            <div style={{ padding: '16px' }}>
                <Input
                    placeholder="Tìm kiếm cuộc trò chuyện..."
                    prefix={<SearchOutlined style={{ color: '#b3b3b3' }} />}
                    style={{
                        background: 'rgba(128, 128, 128, 0.1)',
                        border: '1px solid rgba(128, 128, 128, 0.2)',
                        borderRadius: '8px',
                        height: '40px'
                    }}
                />
            </div>

            {/* Navigation */}
            <div style={{ padding: '0 16px 8px' }}>
                <Menu
                    mode="inline"
                    selectedKeys={['1']}
                    items={menuItems}
                    style={{
                        background: 'transparent',
                        border: 'none'
                    }}
                />
            </div>

            {/* Chat History */}
            <div style={{ flex: 1, overflow: 'hidden', padding: '0 16px' }}>
                <Text
                    type="secondary"
                    style={{
                        fontSize: '12px',
                        fontWeight: 'medium',
                        padding: '0 8px',
                        display: 'block',
                        marginBottom: '12px'
                    }}
                >
                    LỊCH SỬ
                </Text>
                <div style={{
                    height: 'calc(100% - 30px)',
                    overflowY: 'auto',
                    overflowX: 'hidden',
                    paddingRight: '8px'
                }}>
                    <List
                        dataSource={chatHistory}
                        renderItem={(chat) => (
                            <List.Item style={{ padding: 0, marginBottom: '8px' }}>
                                <Card
                                    hoverable
                                    size="small"
                                    onClick={() => setSelectedChat(chat.id)}
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
                                    bodyStyle={{ padding: '12px' }}
                                >
                                    <Flex justify="space-between" align="flex-start">
                                        <div style={{ flex: 1, minWidth: 0 }}>
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
                                            <Text
                                                type="secondary"
                                                style={{ fontSize: '12px' }}
                                                ellipsis
                                            >
                                                {chat.preview}
                                            </Text>
                                        </div>
                                        <Text type="secondary" style={{ fontSize: '12px', marginLeft: '8px', flexShrink: 0 }}>
                                            {chat.time}
                                        </Text>
                                    </Flex>
                                </Card>
                            </List.Item>
                        )}
                    />
                </div>
            </div>

            {/* Upgrade Section */}
            <div style={{ padding: '16px', borderTop: '1px solid #404040' }}>
                <Card
                    style={{
                        background: 'linear-gradient(90deg, rgba(255, 122, 0, 0.1) 0%, rgba(255, 149, 0, 0.05) 100%)',
                        border: '1px solid rgba(255, 122, 0, 0.2)',
                        borderRadius: '8px'
                    }}
                    bodyStyle={{ padding: '16px' }}
                >
                    <Flex align="center" gap={12} style={{ marginBottom: '8px' }}>
                        <CrownOutlined style={{ color: '#ff7a00', fontSize: '20px' }} />
                        <Text style={{ color: '#ff9500', fontWeight: 'medium', fontSize: '14px' }}>
                            Nâng cấp Pro
                        </Text>
                    </Flex>
                    <Paragraph style={{ color: '#b3b3b3', fontSize: '12px', margin: '0 0 12px 0' }}>
                        Truy cập không giới hạn và tính năng cao cấp
                    </Paragraph>
                    <Button
                        type="primary"
                        size="small"
                        block
                        style={{
                            background: 'linear-gradient(45deg, #ff7a00 0%, #ff9500 100%)',
                            border: 'none',
                            borderRadius: '6px',
                            height: '32px',
                            fontSize: '12px',
                            fontWeight: 'medium'
                        }}
                    >
                        Nâng cấp ngay
                    </Button>
                </Card>
            </div>
        </Sider>
    );
};

export default Sidebar;
