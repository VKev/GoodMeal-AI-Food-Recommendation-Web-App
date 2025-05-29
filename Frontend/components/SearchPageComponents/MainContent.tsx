import React from 'react';
import {
    Layout,
    Card,
    Avatar,
    Typography,
    Row,
    Col,
    Flex
} from 'antd';
import {
    MessageOutlined,
    StarOutlined,
    CodeOutlined,
    BookOutlined,
    BugOutlined,
    ToolOutlined
} from '@ant-design/icons';

const { Content } = Layout;
const { Title, Text, Paragraph } = Typography;

interface QuickAction {
    icon: React.ComponentType;
    title: string;
    desc: string;
    color: string;
}

interface MainContentProps {
    selectedChat: number | null;
}

const MainContent: React.FC<MainContentProps> = ({ selectedChat }) => {
    const quickActions: QuickAction[] = [
        { icon: CodeOutlined, title: 'Code Review', desc: 'Kiểm tra và tối ưu code', color: '#ff7a00' },
        { icon: BookOutlined, title: 'Học tập', desc: 'Giải thích khái niệm lập trình', color: '#ff7a00' },
        { icon: BugOutlined, title: 'Debug', desc: 'Tìm và sửa lỗi trong code', color: '#ff7a00' },
        { icon: ToolOutlined, title: 'Tối ưu', desc: 'Cải thiện hiệu suất ứng dụng', color: '#ff7a00' }
    ];

    return (
        <Content style={{
            padding: '32px',
            overflow: 'auto',
            flex: 1,
            maxHeight: 'calc(100vh - 200px)',
            scrollbarWidth: 'thin',
            scrollbarColor: 'rgba(255, 122, 0, 0.3) transparent'
        }}>
            {!selectedChat ? (
                <div style={{ maxWidth: '1200px', margin: '0 auto' }}>
                    <div style={{ textAlign: 'center', marginBottom: '48px' }}>
                        <Avatar
                            size={80}
                            style={{
                                background: 'linear-gradient(45deg, #ff7a00 0%, #ff9500 100%)',
                                marginBottom: '24px',
                                boxShadow: '0 8px 25px rgba(255, 122, 0, 0.25)'
                            }}
                            icon={<StarOutlined style={{ fontSize: '40px' }} />}
                        />
                        <Title
                            level={1}
                            style={{
                                background: 'linear-gradient(45deg, #ffffff 0%, #b3b3b3 100%)',
                                backgroundClip: 'text',
                                WebkitBackgroundClip: 'text',
                                color: 'transparent',
                                marginBottom: '16px'
                            }}
                        >
                            Bắt đầu cuộc trò chuyện
                        </Title>
                        <Paragraph style={{ color: '#b3b3b3', fontSize: '18px' }}>
                            Chọn một trong những gợi ý bên dưới hoặc nhập câu hỏi của bạn
                        </Paragraph>
                    </div>

                    <Row gutter={[16, 16]} style={{ marginBottom: '32px' }}>
                        {quickActions.map((action, index) => (
                            <Col xs={24} md={12} key={index}>
                                <Card
                                    hoverable
                                    style={{
                                        background: 'linear-gradient(135deg, rgba(128, 128, 128, 0.1) 0%, rgba(64, 64, 64, 0.1) 100%)',
                                        border: '1px solid rgba(128, 128, 128, 0.2)',
                                        borderRadius: '12px',
                                        height: '100%'
                                    }}
                                    bodyStyle={{ padding: '24px' }}
                                >                                    <Flex align="flex-start" gap={16}>
                                        <Avatar
                                            size={48}
                                            style={{
                                                background: `linear-gradient(45deg, ${action.color}33 0%, ${action.color}1a 100%)`,
                                                color: action.color,
                                                flexShrink: 0,
                                                fontSize: '24px'
                                            }}
                                        >
                                            <action.icon />
                                        </Avatar>
                                        <div>
                                            <Title level={4} style={{ color: '#ffffff', marginBottom: '8px' }}>
                                                {action.title}
                                            </Title>
                                            <Text type="secondary" style={{ fontSize: '14px' }}>
                                                {action.desc}
                                            </Text>
                                        </div>
                                    </Flex>
                                </Card>
                            </Col>
                        ))}
                    </Row>
                </div>
            ) : (
                <div style={{
                    display: 'flex',
                    flexDirection: 'column',
                    justifyContent: 'center',
                    alignItems: 'center',
                    height: '100%'
                }}>
                    <Avatar
                        size={64}
                        style={{
                            background: 'linear-gradient(45deg, #ff7a00 0%, #ff9500 100%)',
                            marginBottom: '16px'
                        }}
                        icon={<MessageOutlined style={{ fontSize: '32px' }} />}
                    />
                    <Title level={3} style={{ color: '#ffffff', marginBottom: '8px' }}>
                        Cuộc trò chuyện được chọn
                    </Title>
                    <Text type="secondary">
                        Bắt đầu nhập tin nhắn bên dưới để tiếp tục
                    </Text>
                </div>
            )}
        </Content>
    );
};

export default MainContent;
